using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        private record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

        public struct KDrawLayer
        {
            public uint VertexCount = 0;
            public RenderStates States = RenderStates.Default;
            public VertexBuffer Buffer;

            public KDrawLayer(VertexBuffer buffer)
            {
                VertexCount = 0;
                Buffer = buffer;
            }

            public void SubmitDraw(Vertex[] vertices, uint length)
            {
                Buffer.Update(vertices, length, VertexCount);
                VertexCount += length;
            }

            public void DrawBufferToTarget(RenderTarget target)
            {
                Buffer.Draw(target, States);
                VertexCount = 0;
            }

            public void DrawBufferToTarget(RenderTarget target, in RenderStates states)
            {
                Buffer.Draw(target, states);
                VertexCount = 0;
            }
        }

        #region static

        public ArrayPool<Vertex> VertexArrayPool => ArrayPool<Vertex>.Shared;

        public static void CreateTextLayer(Font font, byte fontSize)
        {
            KDrawLayer layer = new KDrawLayer(new VertexBuffer(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream));
            layer.States.Texture = font.GetTexture(fontSize);
        }

        public FloatRect CreateTextbox(KText text, Font font, Vertex[] buffer, byte fontSize, float posX, float posY, int wrapThreshold = 0)
        {
            bool pass = false;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();

            posY += fontSize;

            for (int i = 0, cp = 0; i < buffer.Length / 4; i++)
            {
                GlyphHandle handle = new(chars[i], fontSize, text.Bold, text.LineThickness);

                if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
                {
                    glyph = font.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
                    _glyphCache.Add(handle, glyph);

                    Console.WriteLine($"Glyph cached: {chars[i]}");
                }

                if (chars[i] == '\n')
                {
                    buffer[i * 4] = new();
                    buffer[i * 4 + 1] = new();
                    buffer[i * 4 + 2] = new();
                    buffer[i * 4 + 3] = new();

                    xoffset = 0;
                    height += fontSize;

                    continue;
                }
                if (chars[i] == ' ')
                {
                    cp = i + 1;
                    pass = false;
                }
                else if (!pass && wrapThreshold != 0 && xoffset != 0 && xoffset + glyph.Advance > wrapThreshold)
                {
                    i = cp;
                    xoffset = 0;
                    height += fontSize;
                    pass = true;
                }

                var coords = glyph.TextureRect;
                var rect = glyph.Bounds;

                buffer[i * 4] = new()
                {
                    Position = (posX + xoffset + rect.Left,
                                posY + height + rect.Top),
                    TexCoords = (coords.Left, coords.Top),
                    Color = text.Color,
                };
                buffer[i * 4 + 1] = new()
                {
                    Position = (posX + xoffset + rect.Left + rect.Width,
                                posY + height + rect.Top),
                    TexCoords = (coords.Left + coords.Width, coords.Top),
                    Color = text.Color,
                };
                buffer[i * 4 + 2] = new()
                {
                    Position = (posX + xoffset + rect.Left + rect.Width,
                                posY + height + rect.Top + rect.Height),
                    TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
                    Color = text.Color,
                };
                buffer[i * 4 + 3] = new()
                {
                    Position = (posX + xoffset + rect.Left,
                                posY + height + rect.Top + rect.Height),
                    TexCoords = (coords.Left, coords.Top + coords.Height),
                    Color = text.Color,
                };

                xoffset += glyph.Advance;

                if (xoffset > width) width = xoffset;
            }

            posY -= fontSize;
            return new FloatRect(posX, posY, width, height);
        }

        #endregion

        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public Font Font;
        public View ScreenView;
        public KDrawLayer[] DrawLayers;

        private Dictionary<GlyphHandle, Glyph> _glyphCache = new(128);

        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public KRenderManager(Font font, RenderWindow window)
        {
            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            Window = window;
            Font = font;

            ScreenView = Window.GetView();
        }

        public void Load()
        {

        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KDrawLayer[] drawLayers)
        {
            DrawLayers = drawLayers;
            Window.Resized += ResizeView;
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                DrawLayers[i].DrawBufferToTarget(Window);
            }

            Window.Display();
        }

        public void SubmitDraw(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            Vertex[] vertices = ArrayPool<Vertex>.Shared.Rent(4);
            vertices[0] = new Vertex((rec.Left, rec.Top), dat.Color, dat.Sprite.TopLeft);
            vertices[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            vertices[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            vertices[3] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);

            DrawLayers[layer].SubmitDraw(vertices, 4);
            ArrayPool<Vertex>.Shared.Return(vertices);
        }

        public void SubmitDraw(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            Vertex[] vertices = VertexArrayPool.Rent(4);
            vertices[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            vertices[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            vertices[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            vertices[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            
            DrawLayers[layer].SubmitDraw(vertices, 4);
            VertexArrayPool.Return(vertices);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            ScreenView.Size = new Vector2f(e.Width, e.Height);
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;
using System.Resources;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        private record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

        public record struct KRenderLayer(RenderStates RenderStates, VertexBuffer Buffer, Color BackGround, uint BufferOffset = 0);
        public record struct KTextLayer(Font Font, RenderStates RenderStates, VertexBuffer Buffer, uint BufferOffset = 0);

        #region static

        private static Dictionary<GlyphHandle, Glyph> s_glyphCache = new(128);

        #endregion

        private Vertex[] _drawBounds;

        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public KResourceManager ResourceManager;
        public View[] Cameras;
        public KRenderLayer[] RenderLayers;
        public KTextLayer[] TextLayers;

        public KTextureAtlas[] Atlases;

        public int TopLayer => RenderLayers.Length - 1;
        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public KRenderManager(RenderWindow window, KResourceManager resourceManager)
        {
            _drawBounds = [ new(), new(), new(), new() ];

            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            Window = window;
            ResourceManager = resourceManager;

            Cameras = [ window.GetView() ];
            RenderLayers = [];
            TextLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KRenderLayer[] renderLayers)
        {
            RenderLayers = renderLayers;
            Window.Resized += ResizeView;
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < RenderLayers.Length; i++)
            {
                States.Texture = RenderLayers[i].DrawFrame(this).Texture;
                var halfSize = (Vector2f) States.Texture.Size / 2;

                _drawBounds[0] = new(
                    ScreenCenter + -halfSize, 
                    Color.White, 
                    new(0, 0));

                _drawBounds[1] = new(
                    ScreenCenter + (halfSize.X, -halfSize.Y), 
                    Color.White, 
                    new(States.Texture.Size.X, 0));

                _drawBounds[2] = new(
                    ScreenCenter + halfSize, 
                    Color.White,
                    (Vector2f) States.Texture.Size);

                _drawBounds[3] = new(
                    ScreenCenter + (-halfSize.X, halfSize.Y), 
                    Color.White,
                    new(0, States.Texture.Size.Y));

                Window.Draw(_drawBounds, PrimitiveType.Quads, States);
            }

            for (int i = 0; i < TextLayers.Length; i++)

            Window.Display();
        }

        public void SubmitDraw(Vertex[] vertices, uint vertexCount, int layer = 0) =>
            RenderLayers[layer].SubmitDraw(vertices, vertexCount);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            Vertex[] vertices = _arrayPool.Rent(4);
            vertices[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            vertices[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            vertices[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            vertices[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            
            RenderLayers[layer].SubmitDraw(vertices, 4);
            _arrayPool.Return(vertices);
        }

        public void SubmitDraw(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            Vertex[] vertices = ArrayPool<Vertex>.Shared.Rent(4);
            vertices[0] = new Vertex((rec.Left, rec.Top), dat.Color, dat.Sprite.TopLeft);
            vertices[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            vertices[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            vertices[3] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);

            RenderLayers[layer].SubmitDraw(vertices, 4);
            ArrayPool<Vertex>.Shared.Return(vertices);
        }

        public void SubmitTextDraw(byte fontSize, KText text, float posX, float posY, out FloatRect bounds, float wrapThreshold = 0)
        {
            bool pass = false;
            uint vCount = 0;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();
            Vertex[] vertices = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            posY += fontSize;

            for (int i = 0, cp = 0; i < chars.Length; i++)
            {
                GlyphHandle handle = new(chars[i], fontSize, text.Bold, text.LineThickness);

                if (!s_glyphCache.TryGetValue(handle, out Glyph glyph))
                {
                    glyph = Font.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
                    s_glyphCache.Add(handle, glyph);

                    Console.WriteLine($"Glyph cached: {chars[i]}");
                }

                if (chars[i] == '\n')
                {
                    vertices[i * 4] = new();
                    vertices[i * 4 + 1] = new();
                    vertices[i * 4 + 2] = new();
                    vertices[i * 4 + 3] = new();
                    vCount += 4;

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

                vertices[i * 4] = new()
                {
                    Position = (posX + xoffset + rect.Left,
                                posY + height + rect.Top),
                    TexCoords = (coords.Left, coords.Top),
                    Color = text.Color,
                };
                vertices[i * 4 + 1] = new()
                {
                    Position = (posX + xoffset + rect.Left + rect.Width,
                                posY + height + rect.Top),
                    TexCoords = (coords.Left + coords.Width, coords.Top),
                    Color = text.Color,
                };
                vertices[i * 4 + 2] = new()
                {
                    Position = (posX + xoffset + rect.Left + rect.Width,
                                posY + height + rect.Top + rect.Height),
                    TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
                    Color = text.Color,
                };
                vertices[i * 4 + 3] = new()
                {
                    Position = (posX + xoffset + rect.Left,
                                posY + height + rect.Top + rect.Height),
                    TexCoords = (coords.Left, coords.Top + coords.Height),
                    Color = text.Color,
                };
                vCount += 4;

                xoffset += glyph.Advance;

                if (xoffset > width) width = xoffset;
            }

            _vertexBuffer.Update(vertices, vCount, _bufferOffset);
            _bufferOffset += (uint)(text.Text.Length * 4);
            _arrayPool.Return(vertices);

            posY -= fontSize;
            bounds = new FloatRect(posX, posY, width, height);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            ScreenView.Size = new Vector2f(e.Width, e.Height);
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

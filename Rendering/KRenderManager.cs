using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;
using System.Reflection.Emit;
using static Elements.Rendering.KRenderManager;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        public record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

        #region static

        //will need to update when font is changed.
        private static Dictionary<GlyphHandle, Glyph> s_glyphCache = new(128);
        private static Dictionary<Font, Texture> s_glyphset = new();

        private static Dictionary<GlyphHandle, Glyph> GlyphCache 
        { 
            get => s_glyphCache;
            set 
            {
                OnFontChange?.Invoke(value, s_glyphCache);
                s_glyphCache = value;
            } 
        }

        public static Action<Dictionary<GlyphHandle, Glyph>, Dictionary<GlyphHandle, Glyph>>? OnFontChange;

        public ArrayPool<Vertex> VertexArrayPool => ArrayPool<Vertex>.Shared;

        public static FloatRect CreateTextbox(in KText text, Font font, Vertex[] buffer, float posX, float posY, uint fontSize = 12, int wrapThreshold = 0)
        {
            bool pass = false;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            var chars = text.Text.AsSpan();

            posY += fontSize;

            for (int i = 0, cp = 0; i < chars.Length; i++)
            {
                GlyphHandle handle = new(chars[i], (byte)fontSize, text.Bold, text.LineThickness);

                //Glyph caching
                if (!s_glyphCache.TryGetValue(handle, out Glyph glyph))
                {
                    glyph = font.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
                    s_glyphCache.Add(handle, glyph);

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

                xoffset += (int)glyph.Advance;

                if (xoffset > width) width = xoffset;
            }

            posY -= fontSize;
            return new FloatRect(posX, posY, width, height < 1 ? fontSize : height);
        }

        #endregion

        //TODO add support for points, lines, and polygons.


        private uint ScreenVertexCount;

        public Color BackgroundColor;
        public Color RenderColor;
        public RenderStates States;
        public RenderWindow Window;
        public View ScreenView;
        public VertexBuffer ScreenBuffer;
        public KDrawLayer[] DrawLayers;
        public Vertex[] QuadBuffer;

        public Font? Font;

        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public View View
        {
            get => ScreenView;
            set
            {
                ScreenView = value;
                Window.SetView(ScreenView);
            }
        }

        public KRenderManager(RenderWindow window, VertexBuffer screenBuffer)
        {
            ScreenView = window.GetView();

            Window = window;
            BackgroundColor = Color.Black;
            RenderColor = Color.White;
            States = RenderStates.Default;
            ScreenView = Window.GetView();
            QuadBuffer = new Vertex[4];
            DrawLayers = [];
            ScreenBuffer = screenBuffer;
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(KDrawLayer[] drawLayers)
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

            QuadBuffer[0] = new Vertex((0, 0), RenderColor, (0, 0));
            QuadBuffer[1] = new Vertex((Window.Size.X, 0), RenderColor, (Window.Size.X, 0));
            QuadBuffer[2] = new Vertex((Window.Size.X, Window.Size.Y), RenderColor, (Window.Size.X, Window.Size.Y));
            QuadBuffer[3] = new Vertex((0, Window.Size.Y), RenderColor, (0, Window.Size.Y));

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                Window.Draw(QuadBuffer, PrimitiveType.Quads, 
                    new RenderStates(DrawLayers[i].RenderFrame()));
            }

            if (ScreenVertexCount > 0)
            {
                ScreenBuffer.Draw(Window, States);
                ScreenVertexCount = 0;
            }

            Window.Display();
        }

        public void DrawBuffer(Vertex[] vertices, uint vCount, int layer = 0) => 
            DrawLayers[layer].Draw(vertices, vCount);

        public void DrawRect(float x, float y, float width, float height, Color color, int layer = 0)
        {
            QuadBuffer[0] = new Vertex((x, y), color);
            QuadBuffer[1] = new Vertex((x + width, y), color);
            QuadBuffer[2] = new Vertex((x + width, y + height), color);
            QuadBuffer[3] = new Vertex((x, y + height), color);
            DrawLayers[layer].Draw(QuadBuffer, 4);
        }

        public void DrawRect(FloatRect rec, Color color, int layer = 0)
        {
            QuadBuffer[0] = new Vertex(rec.Position, color);
            QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            QuadBuffer[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), color);
            QuadBuffer[3] = new Vertex((rec.Left, rec.Top + rec.Height), color);
            DrawLayers[layer].Draw(QuadBuffer, 4);
        }

        public void DrawRect(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            QuadBuffer[0] = new Vertex(rec.Position, dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[3] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            DrawLayers[layer].Draw(QuadBuffer, 4);
        }

        public void DrawRect(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            QuadBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            DrawLayers[layer].Draw(QuadBuffer, 4);
        }

        public void DrawText(in KText text, float posX, float posY, int wrapThreshold = 0, int layer = 0)
        {
            if (string.IsNullOrEmpty(text.Text)) return;

            Vertex[] buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            //Cursed but works for now. 
            CreateTextbox(text, KProgram.Fonts[0], buffer, posX, posY, KProgram.FontSize, wrapThreshold);
            DrawLayers[layer].States.Texture = KProgram.Fonts[0].GetTexture(KProgram.FontSize); //Heap go brrrrrrrr.
            DrawLayers[layer].Draw(buffer, (uint) text.Text.Length * 4);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        public void DrawTextBox(in KText text, float posX, float posY, out FloatRect box, int wrapThreshold = 0, int layer = 0)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                box = new FloatRect();
                return;
            }

            Vertex[] buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            //Cursed but works for now.
            box = CreateTextbox(text, KProgram.Fonts[0], buffer, posX, posY, KProgram.FontSize, wrapThreshold);
            DrawLayers[layer].States.Texture = KProgram.Fonts[0].GetTexture(KProgram.FontSize);
            DrawLayers[layer].Draw(buffer, (uint)text.Text.Length * 4);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        public void DrawToScreen(Vertex[] vertices, uint vCount, int layer = 0)
        {
            ScreenBuffer.Update(vertices, vCount, ScreenVertexCount);
            ScreenVertexCount += vCount;
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            ScreenView.Size = new Vector2f(e.Width, e.Height);
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

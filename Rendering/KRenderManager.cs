using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        public record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

        #region static

        //will need to update when font is changed.
        private static Dictionary<GlyphHandle, Glyph> s_glyphCache = new(128);

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

        public static KDrawLayer CreateTextLayer(Font font, byte fontSize = DEFAULT_FONT_SIZE)
        {
            KDrawLayer layer = new KDrawLayer(new VertexBuffer(4096, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream));
            layer.States.Texture = font.GetTexture(fontSize);
            return layer;
        }

        #endregion

        public const int DEFAULT_FONT_SIZE = 12;

        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public View ScreenView;
        public KResourceManager ResourceManager;
        public KDrawLayer[] DrawLayers;
        public Vertex[] QuadBuffer;

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
            Window = window;
            ResourceManager = resourceManager;
            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            ScreenView = Window.GetView();
            QuadBuffer = new Vertex[4];
            DrawLayers = [];
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

        public void SubmitDrawQuad(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            QuadBuffer[0] = new Vertex((rec.Left, rec.Top), dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[3] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            DrawLayers[layer].SubmitDraw(QuadBuffer, 4);
        }

        public void SubmitDrawQuad(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            QuadBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);            
            DrawLayers[layer].SubmitDraw(QuadBuffer, 4);
        }

        public void SubmitDrawText(in KText text, Font font, float posX, float posY, out FloatRect bounds, byte fontSize = DEFAULT_FONT_SIZE, int wrapThreshold = 0, int layer = 0)
        {
            Vertex[] buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            bounds = CreateTextbox(text, font, buffer, posX, posY, fontSize, wrapThreshold);
            DrawLayers[layer].SubmitDraw(buffer, (uint) text.Text.Length * 4);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        //May want to cache this.
        public FloatRect CreateTextbox(in KText text, Font font, Vertex[] buffer, float posX, float posY, byte fontSize = 12, int wrapThreshold = 0)
        {
            bool pass = false;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            var chars = text.Text.AsSpan();

            posY += fontSize;

            //for (int i = 0, cp = 0; i < chars.Length / 4; i++)
            //{
            //    GlyphHandle handle = new(chars[i], fontSize, text.Bold, text.LineThickness);

            //    if (!s_glyphCache.TryGetValue(handle, out Glyph glyph))
            //    {
            //        glyph = font.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
            //        s_glyphCache.Add(handle, glyph);

            //        Console.WriteLine($"Glyph cached: {chars[i]}");
            //    }

            //    if (chars[i] == '\n')
            //    {
            //        buffer[i * 4] = new();
            //        buffer[i * 4 + 1] = new();
            //        buffer[i * 4 + 2] = new();
            //        buffer[i * 4 + 3] = new();

            //        xoffset = 0;
            //        height += fontSize;

            //        continue;
            //    }
            //    if (chars[i] == ' ')
            //    {
            //        cp = i + 1;
            //        pass = false;
            //    }
            //    else if (!pass && wrapThreshold != 0 && xoffset != 0 && xoffset + glyph.Advance > wrapThreshold)
            //    {
            //        i = cp;
            //        xoffset = 0;
            //        height += fontSize;
            //        pass = true;
            //    }

            //    var coords = glyph.TextureRect;
            //    var rect = glyph.Bounds;

            //    buffer[i * 4] = new()
            //    {
            //        Position = (posX + xoffset + rect.Left,
            //                    posY + height + rect.Top),
            //        TexCoords = (coords.Left, coords.Top),
            //        Color = text.Color,
            //    };
            //    buffer[i * 4 + 1] = new()
            //    {
            //        Position = (posX + xoffset + rect.Left + rect.Width,
            //                    posY + height + rect.Top),
            //        TexCoords = (coords.Left + coords.Width, coords.Top),
            //        Color = text.Color,
            //    };
            //    buffer[i * 4 + 2] = new()
            //    {
            //        Position = (posX + xoffset + rect.Left + rect.Width,
            //                    posY + height + rect.Top + rect.Height),
            //        TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
            //        Color = text.Color,
            //    };
            //    buffer[i * 4 + 3] = new()
            //    {
            //        Position = (posX + xoffset + rect.Left,
            //                    posY + height + rect.Top + rect.Height),
            //        TexCoords = (coords.Left, coords.Top + coords.Height),
            //        Color = text.Color,
            //    };

            //    xoffset += (int)glyph.Advance;

            //    if (xoffset > width) width = xoffset;
            //}

            //posY -= fontSize;
            return new FloatRect(posX, posY, width, height);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            ScreenView.Size = new Vector2f(e.Width, e.Height);
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

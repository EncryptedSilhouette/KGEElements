using System.Buffers;
using SFML.Graphics;
using SFML.System;

namespace Elements.Rendering
{
    public record struct KTextLayer(int RenderLayer, int BufferRegion, byte FontID, byte FontSize); 
    public record struct KTextBox(FloatRect bounds, KGlyphHandle[] glyphs);
    public record struct KGlyphHandle(byte FontID, char Chr, byte Size, bool Bold);
    
    public class KTextHandler
    {
        private Dictionary<KGlyphHandle, Glyph> _glyphCache = new(52);
        private HashSet<KTextBox> _cache = new();

        public Font[] Fonts;
        public KTextLayer[] TextLayers;
        public event Action<KGlyphHandle>? GlyphCacheUpdated; //int: fontID

        public KTextHandler()
        {
            TextLayers = [];
            Fonts = [];
        }

        public void Init(Font[] fonts, KTextLayer[] textLayers)
        {
            Fonts = fonts;
            TextLayers = textLayers;
        }
        
        public void Update()
        {
            
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            if (Fonts.Length < 1 || TextLayers.Length < 1) return;
                
            Font font;

            for (int i = 0; i < TextLayers.Length; i++)
            {
                font = Fonts[TextLayers[i].FontID];

                renderer.DrawLayers[TextLayers[i].Handle].RenderFrame(
                    new RenderStates(font.GetTexture(TextLayers[i].FontSize)));
            }
        }

        public void DrawTextBox(KTextBox textBox, int layer = 0)
        {
            var buffer = ArrayPool<Vertex>.Shared.Rent(textBox.glyphs.Length * 6);

            for (int i = 0; i < textBox.glyphs.Length; i++)
            {
                var handle = textBox.glyphs[i];
                var glyph = GetGlyphFromCache(handle);
                
            }

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        public void Draw(string text, int layer = 0)
        {
            
        }

        public KTextBox CreateTextBox(byte fontID, byte fontSize, string text, Vector2f position, Color color, 
            bool bold = false, 
            byte lnSpacing = 4,
            int wrapThreshold = 0)
        {
            FloatRect bounds = new FloatRect(position, (0,0));

            if (string.IsNullOrEmpty(text)) return new KTextBox(bounds, []);

            var chars = text.AsSpan();
            var buffer = new KGlyphHandle[chars.Length];

            for (int i = 0; i < chars.Length; i++)
            {
                var handle = new KGlyphHandle(fontID, chars[fontID], fontSize, bold);

                if (chars[i] == '\n')
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                    buffer[i] = new KGlyphHandle(fontID, chars[i], fontSize, bold);
                    continue;    
                }

                var glyph = GetGlyphFromCache(handle);

                if (wrapThreshold > 0 && bounds.Size.X + glyph.Advance > wrapThreshold)
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                }
            }
            return new KTextBox(new FloatRect(position, bounds.Size), buffer);
        }

        public Glyph GetGlyphFromCache(KGlyphHandle handle)
        {
            if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
            {
                glyph = Fonts[handle.FontID].GetGlyph(handle.Chr, handle.Size, handle.Bold, 0);
                _glyphCache.Add(handle, glyph);
#if DEBUG
                KProgram.LogManager.DebugLog($"Glyph cached: fontID: {handle.FontID}, char: {handle.Chr}, bold: {handle.Bold}");

#endif
                GlyphCacheUpdated?.Invoke(handle);
            }
            return glyph;
        }
    }
}

#if false
//ABC
                VertexBuffer[i * 6] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 1] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 2] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                //BCD
                VertexBuffer[i * 6 + 3] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 4] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                VertexBuffer[i * 6 + 5] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                bounds.Position.X += glyph.Advance;
            }
#endif
using System.Buffers;
using SFML.Graphics;
using SFML.System;

namespace Elements.Rendering
{
    public struct KTextLayer
    {
        public RenderStates RenderStates;
        public byte RenderLayer;
        public byte BufferRegion;
        public byte FontID; 
        public byte FontSize; 

        public KTextLayer(byte renderLayer, byte bufferRegion, byte fontID, byte fontSize, RenderStates states)
        {
            RenderLayer = renderLayer;
            BufferRegion = bufferRegion;
            FontID = fontID;
            FontSize = fontSize;
            RenderStates = states;
        }
    }

    public struct KTextBox
    {
        public FloatRect Bounds; 
        public KGlyphHandle[] Glyphs;

        public KTextBox(FloatRect bounds, KGlyphHandle[] glyphs)
        {
            Bounds = bounds;
            Glyphs = glyphs;
        }
    }
    
    public struct KGlyphHandle 
    {
        public bool Bold;
        public byte FontID;
        public byte Size; 
        public char Chr; 

        public KGlyphHandle(byte fontID, char chr, byte size, bool bold)
        {
            FontID = fontID ;
            Chr = chr;
            Size = size;
            Bold = bold;
        }
    }
    
    public class KTextHandler
    {
        #region static

        private static void UpdateTexture(KTextHandler handler, KGlyphHandle glyph)
        {
            for (int i = 0; i < handler.TextLayers.Length; i++)
            {
                if ((glyph.FontID, glyph.Size) == (handler.TextLayers[i].FontID, handler.TextLayers[i].FontSize))
                {
                    handler.TextLayers[i].RenderStates.Texture = handler.Fonts[glyph.FontID].GetTexture(glyph.FontID);    
                    return;
                }
            }
#if DEBUG
            KProgram.LogManager.DebugLog($"failed to locate and update font texture: id:{glyph.FontID} size:{glyph.Size}.");
#endif
        }

        #endregion

        private KRenderManager _renderer;
        private Dictionary<KGlyphHandle, Glyph> _glyphCache;
        //private HashSet<KTextBox> _cache;

        public Font[] Fonts;
        public KTextLayer[] TextLayers;
        public event Action<KTextHandler, KGlyphHandle>? GlyphCacheUpdated; //int: fontID

        public KTextHandler(KRenderManager renderer)
        {
            _renderer = renderer;
            _glyphCache = new(52);
            //_cache = new();
            TextLayers = [];
            Fonts = [];
        }

        public void Init(Font[] fonts, KTextLayer[] layers)
        {
            
        }
        
        public void Update()
        {
            
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            if (Fonts.Length < 1 || TextLayers.Length < 1) return;
                
            for (int i = 0; i < TextLayers.Length; i++)
            {
                //renderer.RenderLayers[TextLayers[i].Handle].RenderFrame(TextLayers[i].RenderStates);
            }
        }

        public void DrawText(Vector2f pos, string text, byte fontID, byte fontSize, bool bold, Color color,
            byte lnSpacing = 0,
            byte wrapThreshold = 0)
        {
            var chars = text.AsSpan();
            var buffer = ArrayPool<Vertex>.Shared.Rent(chars.Length * 6); 

            for (int i = 0; i < chars.Length; i++)
            {
                var handle = new KGlyphHandle(fontID, chars[fontID], fontSize, bold);
                var bounds = new FloatRect(pos, (0,0));

                if (chars[i] == '\n')
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                    buffer[i] = default;
                    continue;    
                }

                var glyph = GetGlyphFromCache(handle);

                //ABD
                buffer[i * 6] = new Vertex 
                { 
                    Position = pos + glyph.Bounds.Position,
                    Color = color,
                    TexCoords = (Vector2f)glyph.TextureRect.Position,
                };
                buffer[i * 6 + 1] = new Vertex 
                { 
                    Position = (pos.X + glyph.Bounds.Left + glyph.Bounds.Width, pos.Y + glyph.Bounds.Top),
                    Color = color,
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                };
                buffer[i * 6 + 2] = new Vertex 
                { 
                    Position = (pos.X + glyph.Bounds.Left, pos.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    Color = color,
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                };
                //BCD
                buffer[i * 6  + 3] = new Vertex 
                { 
                    Position = (pos.X + glyph.Bounds.Left + glyph.Bounds.Width, pos.Y + glyph.Bounds.Top),
                    Color = color,
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                };
                buffer[i * 6 + 4] = new Vertex 
                { 
                    Position = (pos.X + glyph.Bounds.Left + glyph.Bounds.Width, pos.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    Color = color,
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top + glyph.TextureRect.Height),
                };
                buffer[i * 6 + 5] = new Vertex 
                { 
                    Position = (pos.X + glyph.Bounds.Left, pos.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    Color = color,
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                };

                if (wrapThreshold > 0 && bounds.Size.X + glyph.Advance > wrapThreshold)
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                }
            }

            //_renderer.DrawBufferToLayer(buffer, (uint)chars.Length * 6);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        public KTextBox CreateTextBox(Vector2f position, string text, Color color, byte fontID, byte fontSize, 
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
                GlyphCacheUpdated?.Invoke(this, handle);
            }
            return glyph;
        }
    }
}
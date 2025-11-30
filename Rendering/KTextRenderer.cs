using SFML.Graphics;
using System.Buffers;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        public record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

        public record struct TextDrawData(int VertexCount, Vertex[] Vertices, );
        
        #region static

        private static Dictionary<GlyphHandle, Glyph> _glyphCache = new(128);
        private static ArrayPool<Vertex> ArrayPool => ArrayPool<Vertex>.Shared;

        //public static void CreateTextDrawData(KText text, float posX, float posY, out FloatRect bounds, float wrapThreshold = 0)
        //{

        //}

        public static void CacheGlyph(in GlyphHandle handle, Glyph glyph) => _glyphCache.TryAdd(handle, glyph);

        #endregion

        private uint _bufferOffset = 0;
        private VertexBuffer _vertexBuffer;

        public byte FontSize;
        public KRenderManager RenderManager;
        public RenderStates RenderStates;
        public Font Font;

        public KTextRenderer(Font font, KRenderManager renderManager, byte fontSize = 12)
        {
            _vertexBuffer = new(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            RenderStates = RenderStates.Default;
            
            Font = font;
            RenderManager = renderManager;
            FontSize = fontSize;
        }

        public void SubmitDraw(KText text, float posX, float posY, out FloatRect bounds, float wrapThreshold = 0)
        {
            bool pass = false;
            uint vCount = 0;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();
            Vertex[] vertices = ArrayPool.Rent(chars.Length * 4);

            posY += FontSize;

            for (int i = 0, cp = 0; i < chars.Length; i++)
            {
                GlyphHandle handle = new(chars[i], FontSize, text.Bold, text.LineThickness);

                if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
                {
                    glyph = Font.GetGlyph(chars[i], FontSize, text.Bold, text.LineThickness);
                    _glyphCache.Add(handle, glyph);

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
                    height += FontSize;

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
                    height += FontSize;
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
            _bufferOffset += (uint) (text.Text.Length * 4);
            ArrayPool.Return(vertices);

            posY -= FontSize;
            bounds = new FloatRect(posX, posY, width, height);
        }

        public void SubmitDraw(string text, float posX, float posY, out FloatRect bounds, float wrapThreshold = 0)
        {
            SubmitDraw(new KText(text), posX, posY, out FloatRect b, wrapThreshold);
            bounds = b;
        }

        public void DrawText(RenderTarget target)
        {
            RenderStates.Texture = Font.GetTexture(FontSize);
            _vertexBuffer.Draw(target, 0, _bufferOffset, RenderStates);
            _bufferOffset = 0;
        }
    }
}

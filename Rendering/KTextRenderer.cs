using SFML.Graphics;
using System.Buffers;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        public record struct GlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);
        
        #region static

        private static Dictionary<GlyphHandle, Glyph> _glyphCache = new(128);
        private static ArrayPool<Vertex> ArrayPool => ArrayPool<Vertex>.Shared;

        public static FloatRect CreateTextDrawData(KText text, float posX, float posY, Font font, byte fontSize, Vertex[] vertices, uint vertexCount, float wrapThreshold = 0)
        {
            bool pass = false;
            float width = 0;
            float height = 0;
            float xoffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();

            //I'm too lazy to do this a different way.
            posY += fontSize;

            for (int i = 0, cp = 0; i < chars.Length; i++)
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
                    vertices[i * 4] = new();
                    vertices[i * 4 + 1] = new();
                    vertices[i * 4 + 2] = new();
                    vertices[i * 4 + 3] = new();

                    xoffset = 0;
                    height += fontSize;

                    continue;
                }
                if (chars[i] == ' ')
                {
                    cp = i + 1;
                    pass = false;
                }
                else if (!pass && wrapThreshold != 0 && xoffset != 0 &&
                         xoffset + glyph.Advance > wrapThreshold)
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

                xoffset += glyph.Advance;

                if (xoffset > width) width = xoffset;
            }

            posY -= fontSize;

            return new FloatRect(posX, posY, width, height);
        }

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

        public void SubmitDraw(KText text, float posX, float posY, float wrapThreshold = 0)
        {
            int vertexCount = text.Text.Length;
            Vertex[] vertices = ArrayPool.Rent(vertexCount);

            var drawData = CreateTextDrawData(text, posX, posY, Font, FontSize, vertices, (uint) vertexCount);

            _vertexBuffer.Update(vertices, (uint) vertexCount, _bufferOffset);
            _bufferOffset += (uint) (text.Text.Length * 4);
            ArrayPool.Return(vertices);
        }

        public void SubmitDraw(string text, float posX, float posY, float wrapThreshold = 0) => SubmitDraw(new KText(text), posX, posY, wrapThreshold);

        public void DrawText(RenderTarget target)
        {
            RenderStates.Texture = Font.GetTexture(FontSize);
            _vertexBuffer.Draw(target, 0, _bufferOffset, RenderStates);
            _bufferOffset = 0;
        }
    }
}

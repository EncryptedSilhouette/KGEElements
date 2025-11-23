using SFML.Graphics;
using System.Buffers;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        #region static (HELL)

        private static Dictionary<ulong, Glyph> _glyphCache = new(128);
        private static ArrayPool<Vertex> ArrayPool => ArrayPool<Vertex>.Shared;

        //!!!Syntactic hell below!!!
        private static string DecimalToBinaryString(ulong value)
        {
            char[] chars = new char[sizeof(ulong) * 8];

            for (int i = 0; i < chars.Length; i++)
            {
                //WHY CAN'T THE ALLMIGHTY COMPILER DO THIS FOR ME.
                //                    v                  v
                chars[i] = (value & (ulong)1 << i) == (ulong)1 << i ? '1' : '0';
            }

            return new(chars);
        }

        public static ulong CreateGlypthHandle(char c, byte size, bool bold, byte thickness)
        {
            //STFU- I WAS JUST ABOUT TO SAY SOMETHING ABOUT THE CODE BELOW,
            //BUT THEN THE FUCKING AI AUTO-COMPLETE WAS TRYING TO AUTO-COMPLETE MY BITCHING LIKE IT FUCKING KNOWS ME.
            //But yeah this code is something...
            ulong handle = c;
            handle <<= 8;
            handle |= (byte)size;
            handle <<= 8;
            handle |= (byte)thickness;
            handle <<= 1;
            handle |= bold ? (byte)1 : (byte)0;
            handle <<= 3;
            return handle;
        }

        #endregion 

        private uint _vertexCount;
        private VertexBuffer _vertexBuffer;

        public byte FontSize;
        public KRenderManager RenderManager;
        public RenderStates RenderStates;
        public Font Font;

        public KTextRenderer(Font font, KRenderManager renderManager, byte fontSize = 12)
        {
            _vertexCount = 0;
            _vertexBuffer = new(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            RenderStates = RenderStates.Default;
            
            Font = font;
            RenderManager = renderManager;
            FontSize = fontSize;
        }

        public void SubmitDraw(KText text, float posX, float posY, float wrapThreshold = 0)
        {
            float xOffset = 0;
            float yOffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();

            Vertex[] vertices = RenderManager.ArrayPool.Rent(chars.Length * 4);

            for (int i = 0; i < chars.Length; i++)
            {
                var handle = CreateGlypthHandle(chars[i], FontSize, text.Bold, text.LineThickness);

                if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
                {
                    glyph = Font.GetGlyph(chars[i], FontSize, text.Bold, text.LineThickness);
                    _glyphCache.Add(handle, glyph);

                    Console.WriteLine($"Glyph cached: {chars[i]}");
                }

                var coords = glyph.TextureRect;
                var bounds = glyph.Bounds;

                if (chars[i] == '\n' || (wrapThreshold != 0 && xOffset + bounds.Width > wrapThreshold))
                {
                    xOffset = 0;
                    yOffset += text.LineSpacing + coords.Height;
                    continue;
                }

                vertices[i * 4] = new()
                {
                    Position = (bounds.Left + posX + xOffset,
                                bounds.Top + posY + yOffset),
                    TexCoords = (coords.Left, coords.Top),
                    Color = text.Color,
                };
                vertices[i * 4 + 1] = new()
                {
                    Position = (bounds.Left + bounds.Width + posX + xOffset,
                                bounds.Top + posY + yOffset),
                    TexCoords = (coords.Left + coords.Width, coords.Top),
                    Color = text.Color,
                };
                vertices[i * 4 + 2] = new()
                {
                    Position = (bounds.Left + bounds.Width + posX + xOffset,
                                bounds.Top + bounds.Height + posY + yOffset),
                    TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
                    Color = text.Color,
                };
                vertices[i * 4 + 3] = new()
                {
                    Position = (bounds.Left + posX + xOffset,
                                bounds.Top + bounds.Height + posY + yOffset),
                    TexCoords = (coords.Left, coords.Top + coords.Height),
                    Color = text.Color,
                };

                xOffset += glyph.Advance;
            }

            _vertexBuffer.Update(vertices, (uint) vertices.Length, _vertexCount);
            _vertexCount += (uint) (text.Text.Length * 4);

            RenderManager.ArrayPool.Return(vertices);
        }

        public void DrawText(RenderTarget target)
        {
            RenderStates.Texture = Font.GetTexture(FontSize);
            _vertexBuffer.Draw(target, 0, _vertexCount, RenderStates);
            _vertexCount = 0;
        }
    }
}

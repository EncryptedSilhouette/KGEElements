using SFML.Graphics;
using System.Buffers;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        #region static

        private static ArrayPool<Vertex> ArrayPool => ArrayPool<Vertex>.Shared;

        #region unused glyph caching 

        //private static Dictionary<ulong, Glyph> _glyphCache = new(128);

        //public static ulong CreateGlypthHandle(char c, byte size, bool bold, byte thickness)
        //{
        //    //STFU- I WAS JUST ABOUT TO SAY SOMETHING ABOUT THE CODE BELOW,
        //    //BUT THEN THE FUCKING AI AUTO-COMPLETE WAS TRYING TO AUTO-COMPLETE MY BITCHING LIKE IT FUCKING KNOWS ME.
        //    //But yeah this code is something...
        //    return
        //        (ulong)c << 48 |
        //        (ulong)size << 40 |
        //        (ulong)thickness << 32 |
        //        (ulong)(bold ? 1 : 0) << 31;
        //}

        ////!!!Syntactic hell below!!!
        //public static string DecimalToBinaryString(ulong value)
        //{
        //    char[] chars = new char[sizeof(ulong) * 8];

        //    for (int i = 0; i < chars.Length; i++)
        //    {
        //        //WHY CAN'T THE ALLMIGHTY COMPILER DO THIS FOR ME.
        //        //                    v                  v
        //        chars[i] = (value & (ulong)1 << i) == (ulong)1 << i ? '1' : '0';
        //    }

        //    return new(chars);
        //}

        //public void SubmitDraw(KText text, float posX, float posY, float wrapThreshold = 0)
        //{
        //    uint vertexCount = 0;
        //    float xOffset = 0;
        //    float yOffset = 0;
        //    ReadOnlySpan<char> chars = text.Text.AsSpan();

        //    Vertex[] vertices = ArrayPool.Rent(chars.Length * 4);

        //    for (int i = 0; i < chars.Length; i++)
        //    {
        //        var handle = CreateGlypthHandle(chars[i], FontSize, text.Bold, text.LineThickness);

        //        //if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
        //        //{
        //        //    glyph = Font.GetGlyph(chars[i], FontSize, text.Bold, text.LineThickness);
        //        //    _glyphCache.Add(handle, glyph);

        //        //    Console.WriteLine($"Glyph cached: {chars[i]}");
        //        //}

        #endregion

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
            uint vertexCount = 0;
            float xOffset = 0;
            float yOffset = 0;
            ReadOnlySpan<char> chars = text.Text.AsSpan();

            Vertex[] vertices = ArrayPool.Rent(chars.Length * 4);

            for (int i = 0; i < chars.Length; i++)
            {
                var glyph = Font.GetGlyph(chars[i], FontSize, text.Bold, text.LineThickness);
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

                vertexCount += 4;
                xOffset += glyph.Advance;
            }

            _vertexBuffer.Update(vertices, vertexCount, _bufferOffset);
            _bufferOffset += (uint) (text.Text.Length * 4);

            ArrayPool.Return(vertices);
        }

        public void SubmitDraw(string text, float posX, float posY, float wrapThreshold = 0) =>
            SubmitDraw(new KText(text), posX, posY, wrapThreshold);

        public void DrawText(RenderTarget target)
        {
            RenderStates.Texture = Font.GetTexture(FontSize);
            _vertexBuffer.Draw(target, 0, _bufferOffset, RenderStates);
            _bufferOffset = 0;
        }
    }
}

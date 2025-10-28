using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        private uint _bufferOffset;

        public Font Font;
        public VertexBuffer Buffer;
        public Texture Texture;
        public RenderStates States;

        public KTextRenderer()
        {
            Buffer = new(256, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
        }

        public void SubmitDraw(KTextBox textbox)
        {
            uint xOffset = 0;
            uint yOffset = 0;

            var chars = textbox.Text.AsSpan();
            for (int i = 0; i < chars.Length; i++)
            {
                var glyph = Font.GetGlyph(chars[i], textbox.FontSize, false, 1);
                glyph.TextureRect.Deconstruct(out int top, out int left, out int width, out int height);
                
                if (xOffset + width > textbox.LineLength)
                {
                    yOffset += textbox.LineSpacing;
                    xOffset = 0;
                }

                Vertex[] vertices =
                [
                    new((xOffset, yOffset), Color.White, (left, top)),
                    new((xOffset + width, yOffset), Color.White, (width, top)),
                    new((xOffset + width, yOffset + height), Color.White, (width, height)),
                    new((xOffset, yOffset + height), Color.White, (left, height)),
                ];

                xOffset += (uint) glyph.Advance;

                Buffer.Update(vertices, _bufferOffset);
                _bufferOffset += (uint) vertices.Length;
            }
        }

        public void DrawText(KRenderManager renderer, RenderTarget target) =>
            Buffer.Draw(target, 0, _bufferOffset, States);
    }
}

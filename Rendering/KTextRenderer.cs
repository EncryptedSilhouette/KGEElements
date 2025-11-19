using SFML.Graphics;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        private uint _vertexCount;
        private VertexBuffer _vertexBuffer;

        public uint FontSize = 32;
        public RenderStates RenderStates;
        public Font Font;

        public KTextRenderer(Font font)
        {
            RenderStates = RenderStates.Default;
            _vertexCount = 0;
            _vertexBuffer = new(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            
            Font = font;
        }

        public void SubmitDraw(KText text, int posX, int posY, int wrapThreshold = 0)
        {
            var textBox = text.CreateTextBox(posX, posY, Font, FontSize);
            _vertexBuffer.Update(textBox, (uint) textBox.Length, _vertexCount);
            _vertexCount += (uint) (text.Text.Length * 4);
        }

        public void SubmitDraw(string text, int posX, int posY, int wrapThreshold = 0)
        {
            var textBox = new KText(text, Color.White).CreateTextBox(posX, posY, Font, FontSize);
            _vertexBuffer.Update(textBox, (uint) textBox.Length, _vertexCount);
            _vertexCount += (uint)(text.Length * 4);
        }

        public void DrawText(RenderTarget target)
        {
            RenderStates.Texture = Font.GetTexture(FontSize);

            _vertexBuffer.Draw(target, 0, _vertexCount, RenderStates);
            _vertexCount = 0;
        }
    }
}

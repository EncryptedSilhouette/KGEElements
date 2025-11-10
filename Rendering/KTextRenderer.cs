using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Rendering
{
    public class KTextRenderer
    {
        private uint _vertexCount;
        private RenderStates _renderStates;
        private Font _font;
        private KRenderManager _renderManager;
        private VertexBuffer _vertexBuffer;

        public KTextRenderer(Font font, KRenderManager renderManager)
        {
            _vertexCount = 0;
            _font = font;
            _renderManager = renderManager;
            _vertexBuffer = new(1024, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
        }

        public void SubmitDraw(KText text)
        {
            
        }

        public void DrawText(RenderTarget target)
        {
            _font.GetTexture
            _vertexBuffer.Draw(target, 0, _vertexCount, _renderStates);
        }
    }
}

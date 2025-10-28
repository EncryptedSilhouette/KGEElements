using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KUIRenderer
    {

        private uint _bufferOffset;

        public Font Font;
        public VertexBuffer Buffer;
        public Texture Texture;
        public RenderStates States;
        public RenderTexture RenderTexture;
        public KTextRenderer TextRenderer;

        public RenderTexture DrawFrame(KRenderManager renderer)
        {
            //Draw texture.
            RenderTexture.Clear(BackgroundColor);
            Buffer.Draw(RenderTexture, 0, _bufferOffset, States);
            TextRenderer.DrawText(renderer, RenderTexture);
            //DrawGizmos();
            RenderTexture.Display();

            _bufferOffset = 0;

            return RenderTexture;
        }
    }
}

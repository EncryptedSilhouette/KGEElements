using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KDrawLayer
    {
        public uint VertexCount = 0;
        public RenderStates States = RenderStates.Default;
        public VertexBuffer Buffer;

        public KDrawLayer(VertexBuffer buffer)
        {
            VertexCount = 0;
            Buffer = buffer;
        }

        public void SubmitDraw(Vertex[] vertices, uint length)
        {
            Buffer.Update(vertices, length, VertexCount);
            VertexCount += length;
        }

        public void DrawBufferToTarget(RenderTarget target)
        {
            Buffer.Draw(target, States);
            VertexCount = 0;
        }

        public void DrawBufferToTarget(RenderTarget target, in RenderStates states)
        {
            Buffer.Draw(target, states);
            VertexCount = 0;
        }
    }
}

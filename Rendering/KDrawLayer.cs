using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KDrawLayer
    {
        public uint VertexCount = 0;
        public RenderStates States;
        public VertexBuffer Buffer;

        public KDrawLayer(VertexBuffer buffer, RenderStates states)
        {
            VertexCount = 0;
            States = states;
            Buffer = buffer;
        }

        public void SubmitDraw(Vertex[] vertices, uint length)
        {
            var b = Buffer.Update(vertices, length, VertexCount);
            VertexCount += length;
        }

        public void DrawBufferToTarget(RenderTarget target)
        {
            Buffer.Draw(target, 0, VertexCount, States);
            VertexCount = 0;
        }

        public void DrawBufferToTarget(RenderTarget target, in RenderStates states)
        {
            Buffer.Draw(target, 0, VertexCount, states);
            VertexCount = 0;
        }
    }
}

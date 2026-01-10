using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KDrawLayer
    {
        public uint VertexCount = 0;
        public Color ClearColor;
        required public RenderStates States;
        required public VertexBuffer Buffer;
        required public RenderTexture RenderTexture;

        public KDrawLayer() { }

        public KDrawLayer(VertexBuffer buffer, RenderStates states)
        {
            VertexCount = 0;
            ClearColor = Color.Transparent;
            States = states;
            Buffer = buffer;
        }

        public void Draw(Vertex[] vertices, uint length)
        {
            Buffer.Update(vertices, length, VertexCount);
            VertexCount += length;
        }

        public Texture RenderFrame()
        {
            RenderTexture.Clear(ClearColor);
            
            Buffer.Draw(RenderTexture, 0, VertexCount, States);
            VertexCount = 0;

            RenderTexture.Display();
            return RenderTexture.Texture;
        }
    }
}

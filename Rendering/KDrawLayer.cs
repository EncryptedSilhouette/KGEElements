using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KDrawLayer
    {
        public uint VertexCount = 0;
        public Color ClearColor;
        public FloatRect DrawBounds;
        required public RenderTexture RenderTexture;
        required public RenderStates States;
        required public VertexBuffer Buffer;

        public KDrawLayer() 
        {
            ClearColor = Color.Transparent;
            DrawBounds = new();
        }

        public KDrawLayer(VertexBuffer buffer, in RenderStates states, in FloatRect bounds) : this()
        {
            VertexCount = 0;
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

        public Texture RenderFrame(in RenderStates states)
        {
            RenderTexture.Clear(ClearColor);
            
            Buffer.Draw(RenderTexture, 0, VertexCount, states);
            VertexCount = 0;

            RenderTexture.Display();
            return RenderTexture.Texture;
        } 
    }
}

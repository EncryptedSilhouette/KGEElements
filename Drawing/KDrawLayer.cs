using Elements.Core;
using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KDrawLayer
    {
        private uint _bufferOffset;

        public RenderStates States;
        public RenderTexture RenderTexture;
        public VertexBuffer Buffer;

#nullable disable
        public KDrawLayer() => (_bufferOffset, States) = (0, RenderStates.Default);
#nullable enable

        public KDrawLayer(in RenderStates renderStates, RenderTexture renderTexture, VertexBuffer buffer, Vertex[] vertices) =>
            (States, RenderTexture, Buffer) = (renderStates, renderTexture, buffer);

        public void DrawFrame()
        {
            RenderTexture.Clear(Color.Black);
            Buffer.Draw(RenderTexture, 0, _bufferOffset, States);
            RenderTexture.Display();

            _bufferOffset = 0;
        }

        public void SubmitDraw(Vertex[] vertices)
        {
            Buffer.Update(vertices, (uint) vertices.Length, _bufferOffset);
            _bufferOffset += (uint) vertices.Length;
        }

        public void SubmitDraw(in KDrawData dat, in KRectangle rec)
        {
            Vertex[] vertices =
            [
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TopLeft,
                    Position = rec.TopLeft
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TopRight,
                    Position = rec.TopRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.BottomRight,
                    Position = rec.BottomRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.BottomLeft,
                    Position = rec.BottomLeft
                }
            ];
            SubmitDraw(vertices);
        }
    }
}

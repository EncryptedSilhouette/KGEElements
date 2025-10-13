using Elements.Core;
using SFML.Graphics;
using SFML.System;

namespace Elements.Drawing
{
    public struct KRenderLayer
    {
        public bool Resize;
        public byte Scale;
        public byte Camera;
        public uint BufferOffset;
        public RenderStates States;
        public RenderTexture RenderTexture;
        public VertexBuffer Buffer;

        public Vector2u Resolution
        {
            get => RenderTexture.Size;
            set
            {
                RenderTexture.Dispose();
                RenderTexture = new(value.X, value.Y);
            }
        }

#nullable disable
        public KRenderLayer() => 
            (Resize, Scale, Camera, BufferOffset, States) = (false, 1, 0, 0, RenderStates.Default);
#nullable enable

        public KRenderLayer(in RenderStates renderStates, RenderTexture renderTexture, VertexBuffer buffer) =>
            (States, RenderTexture, Buffer) = (renderStates, renderTexture, buffer);

        public void DrawFrame(View view)
        {
            RenderTexture.SetView(view);
            RenderTexture.Clear(Color.Transparent);
            Buffer.Draw(RenderTexture, 0, BufferOffset, States);
            RenderTexture.Display();
            BufferOffset = 0;
        }

        public void SubmitDraw(Vertex[] vertices)
        {
            if (BufferOffset + vertices.Length > Buffer.VertexCount)
            {
                uint newSize = Buffer.VertexCount * (uint)(BufferOffset + vertices.Length / Buffer.VertexCount);
                VertexBuffer newBuffer = new(newSize, Buffer.PrimitiveType, Buffer.Usage);
                newBuffer.Swap(Buffer);
                Buffer = newBuffer;
            }
            Buffer.Update(vertices, (uint) vertices.Length, BufferOffset);
            BufferOffset += (uint) vertices.Length;
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

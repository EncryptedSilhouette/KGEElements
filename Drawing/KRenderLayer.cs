#nullable disable

using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KRenderLayer
    {
        private uint _bufferOffset;
        public int Camera;
        public RenderTexture RenderTexture;
        public VertexBuffer Buffer;
        public RenderStates States;

        public KRenderLayer() => _bufferOffset = 0;

        public KRenderLayer(RenderStates states, RenderTexture renderTexture, VertexBuffer buffer) =>
            (_bufferOffset, States, RenderTexture, Buffer) = (0, states, renderTexture, buffer);

        public void SubmitDraw(Vertex[] vertices)
        {
            Buffer.Update(vertices, _bufferOffset);
            _bufferOffset += (uint) vertices.Length;
        }

        public RenderTexture DrawFrame(View view)
        {
            RenderTexture.Clear(Color.Transparent);

            Buffer.Draw(RenderTexture, 0, _bufferOffset, States);

            _bufferOffset = 0;
            Vertex[] lines =
            {
                //Line A
                new(new(0, 0), Color.Green),
                new(new(RenderTexture.Size.X, RenderTexture.Size.Y), Color.Green),

                //Line B
                new(new(RenderTexture.Size.X, 0), Color.Green),
                new(new(0, RenderTexture.Size.Y), Color.Green)
            };

            RenderTexture.Draw(lines, PrimitiveType.Lines);
            RenderTexture.Display();

            return RenderTexture;
        }

        private void DrawGizmos()
        {
            Vertex[] lines =
            {
                //Line A
                new(new(0, 0), Color.Green),
                new(new(RenderTexture.Size.X, RenderTexture.Size.Y), Color.Green),

                //Line B
                new(new(RenderTexture.Size.X, 0), Color.Green),
                new(new(0, RenderTexture.Size.Y), Color.Green)
            };

            RenderTexture.Draw(lines, PrimitiveType.Lines);
        }
    }
}

#nullable enable

//namespace Elements.Drawing
//{
//    public struct KRenderLayer
//    {
//        public bool Resize;
//        public byte Scale;
//        public byte Camera;
//        public uint _bufferOffset;
//        public RenderStates States;
//        public RenderTexture RenderTexture;
//        public VertexBuffer Buffer;

//        public Vector2u Resolution
//        {
//            get => RenderTexture.Size;
//            set
//            {
//                RenderTexture.Dispose();
//                RenderTexture = new(value.X, value.Y);
//            }
//        }

//#nullable disable
//        public KRenderLayer() => 
//            (Resize, Scale, Camera, _bufferOffset, States) = (false, 1, 0, 0, RenderStates.Default);
//#nullable enable

//        public KRenderLayer(in RenderStates renderStates, RenderTexture renderTexture, VertexBuffer buffer) =>
//            (States, RenderTexture, Buffer) = (renderStates, renderTexture, buffer);

//        public void DrawFrame(View view)
//        {
//            RenderTexture.SetView(view);
//            RenderTexture.Clear(Color.Transparent);
//            Buffer.Draw(RenderTexture, 0, _bufferOffset, States);
//            RenderTexture.Display();
//            _bufferOffset = 0;
//        }

//        public void SubmitDraw(Vertex[] vertices)
//        {
//            if (_bufferOffset + vertices.Length > Buffer.VertexCount)
//            {
//                uint newSize = Buffer.VertexCount * (uint)(_bufferOffset + vertices.Length / Buffer.VertexCount);
//                VertexBuffer newBuffer = new(newSize, Buffer.PrimitiveType, Buffer.Usage);
//                newBuffer.Swap(Buffer);
//                Buffer = newBuffer;
//            }
//            Buffer.Update(vertices, (uint) vertices.Length, _bufferOffset);
//            _bufferOffset += (uint) vertices.Length;
//        }

//        public void SubmitDraw(in KDrawData dat, in KRectangle rec)
//        {
//            Vertex[] vertices =
//            [
//                new Vertex()
//                {
//                    Color = dat.Color,
//                    TexCoords = dat.Sprite.TopLeft,
//                    Position = rec.TopLeft
//                },
//                new Vertex()
//                {
//                    Color = dat.Color,
//                    TexCoords = dat.Sprite.TopRight,
//                    Position = rec.TopRight
//                },
//                new Vertex()
//                {
//                    Color = dat.Color,
//                    TexCoords = dat.Sprite.BottomRight,
//                    Position = rec.BottomRight
//                },
//                new Vertex()
//                {
//                    Color = dat.Color,
//                    TexCoords = dat.Sprite.BottomLeft,
//                    Position = rec.BottomLeft
//                }
//            ];
//            SubmitDraw(vertices);
//        }
//    }
//}

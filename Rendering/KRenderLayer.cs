
using SFML.Graphics;
using SFML.System;

namespace Elements.Rendering
{
    public struct KRenderLayer
    {
        private uint _bufferOffset;

        public int Camera;
        public Color LineColor; 
        public Color BackgroundColor;
        public FloatRect Bounds;
        public RenderTexture RenderTexture;
        public VertexBuffer Buffer;
        public RenderStates States;

        public Vector2u Resolution => RenderTexture.Size;

#nullable disable
        public KRenderLayer() => (_bufferOffset, BackgroundColor, LineColor) = (0, Color.White, Color.Green);
#nullable enable

        public KRenderLayer(RenderStates states, RenderTexture renderTexture, VertexBuffer buffer) : this() =>
            (States, RenderTexture, Buffer) = (states, renderTexture, buffer);

        public void SubmitDraw(Vertex[] vertices, uint vertexCount)
        {
            Buffer.Update(vertices, vertexCount, _bufferOffset);
            _bufferOffset += vertexCount;
        }

        public RenderTexture DrawFrame(KRenderManager manager)
        {
            //Draw texture.
            RenderTexture.Clear(BackgroundColor);
            Buffer.Draw(RenderTexture, 0, _bufferOffset, States);
            //DrawGizmos();
            RenderTexture.Display();

            _bufferOffset = 0;
            return RenderTexture;
        }

        //private void DrawGizmos()
        //{
        //    Vertex[] lines = 
        //    {
        //        //Line A
        //        new(new(0, 0), LineColor),
        //        new(new(RenderTexture.Size.X, RenderTexture.Size.Y), LineColor),

        //        //Line B
        //        new(new(RenderTexture.Size.X, 0), LineColor),
        //        new(new(0, RenderTexture.Size.Y), LineColor)
        //    };

        //    RenderTexture.Draw(lines, PrimitiveType.Lines);
        //}
    }
}


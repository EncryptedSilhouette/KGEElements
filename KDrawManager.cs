using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements
{
    public class KDrawManager
    {
        private RenderStates _states;

        public Color BackgroundColor;
        public RenderWindow Window;
        public KDrawLayer[] DrawLayers;

        public int TopLayer => DrawLayers.Length - 1;

        public KDrawManager(RenderWindow window)
        {
            _states = RenderStates.Default;
            BackgroundColor = Color.White;
            Window = window;
            DrawLayers = [];
        }

        public void Init(KDrawLayer[] drawLayers)
        {
            DrawLayers = drawLayers;
        }

        public void FrameUpdate()
        {
            //Not my perfered way of doing this. Need a layout manager.
            var windowBounds = new KRectangle(Window.Size.X, Window.Size.Y);

            Window.Clear(BackgroundColor);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                _states.Texture = DrawLayers[i].RenderTexture.Texture;

                var textureBounds = new KRectangle(_states.Texture.Size.X, _states.Texture.Size.Y);

                Vertex[] vertices =
                [
                    new Vertex(windowBounds.TopLeft, Color.White, textureBounds.TopLeft),
                    new Vertex(windowBounds.TopRight, Color.White, textureBounds.TopRight),
                    new Vertex(windowBounds.BottomRight, Color.White, textureBounds.BottomRight),
                    new Vertex(windowBounds.BottomLeft, Color.White, textureBounds.BottomLeft),
                ];

                Window.Draw(vertices, PrimitiveType.Quads, _states);
            }

            Window.Display();
        }

        public void SubmitDraw(in int layer, in KDrawData dat, in KRectangle rec) =>
            DrawLayers[layer].SubmitDraw(dat, rec);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec) =>
            DrawLayers[TopLayer].SubmitDraw(dat, rec);

        public void SubmitDraw(in int layer, Vertex[] vertices) =>
            DrawLayers[layer].SubmitDraw(vertices);

        public void SubmitDraw(Vertex[] vertices) =>
            DrawLayers[TopLayer].SubmitDraw(vertices); 

        //public KDrawLayer CreateDrawLayer(uint bufferSize, Texture atlas)
        //{
        //    return new()
        //    {
        //        States = RenderStates.Default,
        //        RenderTexture = new(Window.Size.X, Window.Size.Y),
        //        Buffer = new VertexBuffer(bufferSize, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
        //        Vertices =
        //        [
        //            new Vertex(new(0, 0), Color.White, new(0, 0)),
        //            new Vertex(new(Window.Size.X, 0), Color.White, new(Window.Size.X, 0)),
        //            new Vertex(new(Window.Size.X, Window.Size.Y), Color.White, new(Window.Size.X, Window.Size.Y)),
        //            new Vertex(new(0, Window.Size.Y), Color.White, new(0, Window.Size.Y)),
        //        ]
        //    };
        //}
    }
}

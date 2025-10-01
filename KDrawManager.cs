using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements
{
    public class KDrawManager
    {
        private RenderStates _states;

        public Color BackgroundColor;
        public View CameraView;
        public RenderWindow Window;
        public KDrawLayer[] DrawLayers;

        public int TopLayer => DrawLayers.Length - 1;

        public KDrawManager(RenderWindow window)
        {
            _states = RenderStates.Default;
            BackgroundColor = Color.Black;
            Window = window;
            DrawLayers = [];
            CameraView = Window.GetView();
            CameraView.Center = new(0, 0);
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
                DrawLayers[i].DrawFrame(CameraView);
                _states.Texture = DrawLayers[i].RenderTexture.Texture;

                Vertex[] vertices =
                [
                    new Vertex() 
                    {
                        TexCoords = new(0, 0),
                        Position = new(0, 0), 
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(_states.Texture.Size.X, 0),
                        Position = new(Window.Size.X, 0), 
                        Color = Color.White 
                    },
                    new Vertex() 
                    {
                        TexCoords = new(_states.Texture.Size.X, _states.Texture.Size.Y),
                        Position = new(Window.Size.X, Window.Size.Y), 
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(0, _states.Texture.Size.Y),
                        Position= new(0, Window.Size.Y), 
                        Color = Color.White
                    },
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

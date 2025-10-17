using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Drawing
{
    public class KRenderManager
    {
        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public View[] CameraViews;
        public KRenderLayer[] DrawLayers;

        public int TopLayer => DrawLayers.Length - 1;
        public float CenterX => Window.Size.X / 2;
        public float CenterY => Window.Size.Y / 2;
        public Vector2f Center => (Vector2f) Window.Size / 2;
        public Vector2f TopLeft => new(0, 0);
        public Vector2f TopRight => new(Window.Size.X, 0);
        public Vector2f BottomRight => (Vector2f) Window.Size;
        public Vector2f BottomLeft => new(0, Window.Size.Y);

        public KRenderManager(RenderWindow window)
        {
            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            Window = window;
            CameraViews = [];
            DrawLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KRenderLayer[] drawLayers)
        {
            //I will loose my mind otherwise.
            Window.SetView(new(new(0, 0), (Vector2f) Window.Size));

            CameraViews = cameraViews;
            //DrawLayers = drawLayers;
            Window.Resized += ResizeView;
            CameraViews =
            [
                //Life truly is hell.
                new(new(0, 0), new(640, 480))
            ];
            DrawLayers =
            [
                new()
                {
                    Camera = 0,
                    //States = new(ResourceManager.TextureAtlases["atlas"].Texture),
                    States = RenderStates.Default,
                    RenderTexture = new(Window.Size.X, Window.Size.Y),
                    Buffer = new(256, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
                }
            ];
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Vertex[] v =
            [
                new(new(-32, -32), Color.White),
                new(new(32, -32), Color.White),
                new(new(32, 32), Color.White),
                new(new(-32, 32), Color.White),
            ];
            SubmitDraw(v);

            var camera = CameraViews[DrawLayers[0].Camera];
            var layer = DrawLayers[0].DrawFrame(camera);
            States = RenderStates.Default;
            States.Texture = layer.Texture;

            Window.Clear(Color.Black);

            Vertex[] bounds =
            {
                new Vertex() 
                {
                    Color = Color.White,
                    Position = camera.Center + new Vector2f(-layer.Size.X, -layer.Size.Y) / 2,
                    TexCoords = new(0, 0),
                },
                new Vertex()
                {
                    Color = Color.White,
                    Position = camera.Center + new Vector2f(layer.Size.X, -layer.Size.Y) / 2,
                    TexCoords = new(layer.Texture.Size.X, 0),
                },
                new Vertex()
                {
                    Color = Color.White,
                    Position = camera.Center + new Vector2f(layer.Size.X, layer.Size.Y) / 2,
                    TexCoords = new(layer.Texture.Size.X, layer.Texture.Size.Y),
                },
                new Vertex()
                {
                    Color = Color.White,
                    Position = camera.Center + new Vector2f(-layer.Size.X, layer.Size.Y) / 2,
                    TexCoords = new(0, layer.Texture.Size.Y),
                }
            };

            DrawGizmos();
            Window.Draw(bounds, PrimitiveType.Quads, States);
            Window.Display();
        }

        public void SubmitDraw(Vertex[] vertices, int layer = 0) =>
            DrawLayers[layer].SubmitDraw(vertices);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            Vertex[] vertices = 
            {
                new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight),
                new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft),
                new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.TopLeft),
                new Vertex(rec.BottomRight, dat.Color, dat.Sprite.TopRight),
            };
            DrawLayers[layer].SubmitDraw(vertices);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            Window.SetView(
                new View(
                    new(0, 0), //Center
                    new(e.Width, e.Height)));       //Size
        }

        private void DrawGizmos()
        {
            Vertex[] lines =
            {
                //Line A
                new(new Vector2f(-Window.Size.X, -Window.Size.Y) / 2),
                new(new Vector2f(Window.Size.X, Window.Size.Y) / 2),

                //Line B
                new(new Vector2f(Window.Size.X, -Window.Size.Y) / 2),
                new(new Vector2f(-Window.Size.X, Window.Size.Y) / 2),
            };

            Window.Draw(lines, PrimitiveType.Lines);
        }
    }
}

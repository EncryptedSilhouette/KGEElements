using Elements.Core;
using SFML.Graphics;
using SFML.System;

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
        public Vector2f BottomRight => (Vector2f)Window.Size;
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
            CameraViews = cameraViews;
            DrawLayers = drawLayers;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                ref KRenderLayer layer = ref DrawLayers[i];

                layer.DrawFrame(CameraViews[DrawLayers[i].Camera]);
                States.Texture = DrawLayers[i].RenderTexture.Texture;

                Vertex[] vertices =
                [
                    new Vertex() 
                    {
                        TexCoords = new(0, 0),
                        Position = Center - (Vector2f) States.Texture.Size / 2 * layer.Scale, 
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(States.Texture.Size.X, 0),
                        Position = Center + new Vector2f(States.Texture.Size.X / 2, -States.Texture.Size.Y / 2) * layer.Scale, 
                        Color = Color.White 
                    },
                    new Vertex() 
                    {
                        TexCoords = new(States.Texture.Size.X, States.Texture.Size.Y),
                        Position = Center + (Vector2f) States.Texture.Size / 2 * layer.Scale,
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(0, States.Texture.Size.Y),
                        Position = Center + new Vector2f(-States.Texture.Size.X / 2, States.Texture.Size.Y / 2) * layer.Scale,
                        Color = Color.White
                    },
                ];
                Window.Draw(vertices, PrimitiveType.Quads, States);
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
    }
}

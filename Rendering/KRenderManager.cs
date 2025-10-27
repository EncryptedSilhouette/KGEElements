using Elements.Core;
using Elements.Rendering;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Drawing
{
    public class KRenderManager
    {
        private Vertex[] _drawBounds;

        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public KTextRenderer TextRenderer;
        public View[] CameraViews;
        public KRenderLayer[] RenderLayers;

        public int TopLayer => RenderLayers.Length - 1;

        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public KRenderManager(RenderWindow window)
        {
            _drawBounds = [ new(), new(), new(), new() ];

            BackgroundColor = Color.White;
            States = RenderStates.Default;
            Window = window;
            CameraViews = [];
            RenderLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KRenderLayer[] renderLayers)
        {
            CameraViews = cameraViews;
            RenderLayers = renderLayers;
            Window.Resized += ResizeView;
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < RenderLayers.Length; i++)
            {
                States.Texture = RenderLayers[i].DrawFrame(this).Texture;
                var halfSize = (Vector2f) States.Texture.Size / 2;

                _drawBounds[0] = new(
                    ScreenCenter + -halfSize, 
                    Color.White, 
                    new(0, 0));

                _drawBounds[1] = new(
                    ScreenCenter + (halfSize.X, -halfSize.Y), 
                    Color.White, 
                    new(States.Texture.Size.X, 0));

                _drawBounds[2] = new(
                    ScreenCenter + halfSize, 
                    Color.White,
                    (Vector2f) States.Texture.Size);

                _drawBounds[3] = new(
                    ScreenCenter + (-halfSize.X, halfSize.Y), 
                    Color.White,
                    new(0, States.Texture.Size.Y));

                Window.Draw(_drawBounds, PrimitiveType.Quads, States);
            }

            TextRenderer.FrameUpdate(this);
            
            Window.Display();
        }

        public void SubmitDraw(Vertex[] vertices, int layer = 0) =>
            RenderLayers[layer].SubmitDraw(vertices);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            Vertex[] vertices = 
            {
                new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight),
                new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft),
                new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.TopLeft),
                new Vertex(rec.BottomRight, dat.Color, dat.Sprite.TopRight),
            };
            RenderLayers[layer].SubmitDraw(vertices);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            Window.SetView(
                new View(
                    new Vector2f(e.Width, e.Height) / 2,     //Center
                    new Vector2f(e.Width, e.Height)));       //Size
        }
    }
}

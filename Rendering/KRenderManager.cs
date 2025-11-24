using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        private View _screenView;
        private Vertex[] _drawBounds;

        public Color BackgroundColor;
        public RenderStates States;
        public RenderWindow Window;
        public KRenderLayer[] RenderLayers;
        public KTextRenderer[] TextRenderers;

        public ArrayPool<Vertex> ArrayPool => ArrayPool<Vertex>.Shared;

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
            _screenView = window.GetView();
            _drawBounds = [ new(), new(), new(), new() ];

            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            Window = window;
            RenderLayers = [];
            TextRenderers = []; 
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KRenderLayer[] renderLayers, KTextRenderer[] textRenderers)
        {
            TextRenderers = textRenderers;
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

            for (int i = 0; i < TextRenderers.Length; i++) 
            {
                TextRenderers[i].DrawText(Window);
            }

            Window.Display();
        }

        public void SubmitDraw(Vertex[] vertices, uint vertexCount, int layer = 0) =>
            RenderLayers[layer].SubmitDraw(vertices, vertexCount);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            Vertex[] vertices = ArrayPool.Rent(4);
            vertices[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            vertices[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            vertices[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            vertices[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            
            RenderLayers[layer].SubmitDraw(vertices, 4);
            ArrayPool.Return(vertices);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            _screenView.Size = new Vector2f(e.Width, e.Height);
            _screenView.Center = _screenView.Size / 2;
            Window.SetView(_screenView);
        }
    }
}

using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        private uint _bufferOffset;
        private View _view;

        public Color BackgroundColor;
        public Color RenderColor;
        public RenderStates States;
        public RenderWindow Window;
        public VertexBuffer ScreenBuffer;
        public KDrawLayer[] DrawLayers;
        public Vertex[] drawBuffer;

        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public KRenderManager(RenderWindow window, VertexBuffer screenBuffer)
        {
            _bufferOffset = 0;
            _view = window.DefaultView;

            BackgroundColor = Color.Black;
            States = RenderStates.Default;
            Window = window;
            ScreenBuffer = screenBuffer;
            drawBuffer = new Vertex[6];
            DrawLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(KDrawLayer[] drawLayers, Font[] fonts)
        {
            DrawLayers = drawLayers;
            Window.Resized += ResizeView;
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                ref var bounds = ref DrawLayers[i].DrawBounds;
                var texture = DrawLayers[i].RenderFrame();

                //ABD
                drawBuffer[0] = new Vertex((bounds.Left, bounds.Top), Color.White, (0, 0)); 
                drawBuffer[1] = new Vertex((bounds.Left + bounds.Width, bounds.Top), Color.White, (texture.Size.X, 0)); 
                drawBuffer[2] = new Vertex((bounds.Left, bounds.Top + bounds.Height), Color.White, (0, texture.Size.Y)); 
                //BCD
                drawBuffer[3] = new Vertex((bounds.Left + bounds.Width, bounds.Top), Color.White, (texture.Size.X, 0));  
                drawBuffer[4] = new Vertex((bounds.Left + bounds.Width, bounds.Top + bounds.Height), Color.White, (texture.Size.X, texture.Size.Y)); 
                drawBuffer[5] = new Vertex((bounds.Left, bounds.Top + bounds.Height), Color.White, (0, texture.Size.Y)); 

                Window.Draw(drawBuffer, PrimitiveType.Triangles, new RenderStates(texture));
            }

            if (_bufferOffset > 0)
            {
                ScreenBuffer.Draw(Window, States);
                _bufferOffset = 0;
            }

            Window.Display();
        }

        //Draw to screen. 
        public void DrawBufferToScreen(Vertex[] vertices, uint vCount)
        {
            ScreenBuffer.Update(vertices, vCount, _bufferOffset);
            _bufferOffset += vCount;
        }

        public void DrawRectToScreen(Vector2f a, Vector2f b, Color color)
        {
            //ABD
            drawBuffer[0] = new Vertex(a, color);
            drawBuffer[1] = new Vertex((b.X, a.Y), color);
            drawBuffer[2] = new Vertex((a.X, b.Y), color);
            //BCD
            drawBuffer[3] = new Vertex((b.X, a.Y), color);
            drawBuffer[4] = new Vertex((b.X, b.Y), color);
            drawBuffer[5] = new Vertex((a.X, b.Y), color);

            ScreenBuffer.Update(drawBuffer, 6, _bufferOffset);
            _bufferOffset += 6;
        }

        //Draw Layers.
        public void DrawBuffer(Vertex[] vertices, uint vCount, int layer = 0) => 
            DrawLayers[layer].Draw(vertices, vCount);

        public void DrawRect(Vector2f a, Vector2f b, Color color, int layer = 0)
        {
            //ABD
            drawBuffer[0] = new Vertex(a, color);
            drawBuffer[1] = new Vertex((b.X, a.Y), color);
            drawBuffer[2] = new Vertex((a.X, b.Y), color);
            //BCD
            drawBuffer[3] = new Vertex((b.X, a.Y), color);
            drawBuffer[4] = new Vertex((b.X, b.Y), color);
            drawBuffer[5] = new Vertex((a.X, b.Y), color);

            DrawLayers[layer].Draw(drawBuffer, 6);
        }

        public void DrawRect(in FloatRect rec, Color color, int layer = 0) => 
            DrawRect(rec.Position, rec.Size, color, layer = 0);
        
        //Draw w/ texture data.
        public void DrawRect(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            //ABD
            drawBuffer[0] = new Vertex(rec.Position, dat.Color, dat.Sprite.TopLeft);
            drawBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            drawBuffer[2] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            //BCD
            drawBuffer[3] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            drawBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            drawBuffer[5] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);

            DrawLayers[layer].Draw(drawBuffer, 6);
        }

        public void DrawRect(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            //ABD
            drawBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            drawBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            drawBuffer[2] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            //BCD
            drawBuffer[3] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            drawBuffer[4] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            drawBuffer[5] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);

            DrawLayers[layer].Draw(drawBuffer, 6);
        }

        //Draw text.
        public void DrawText(in KText text, int layer = 1) =>
            DrawLayers[layer].Draw(SFML.Graphics.Text.VertexBuffer, SFML.Graphics.Text.VertexCount);

        private void ResizeView(object? _, SizeEventArgs e)
        {
            _view.Size = (Vector2f)e.Size;
            _view.Center = _view.Size / 2;
            Window.SetView(_view);
        }
    }
}

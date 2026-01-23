using System.Buffers;
using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Rendering
{
    public class KRenderManager
    {
        private uint _sBuffvCount; //I give up naming this.

        public Color BackgroundColor;
        public Color RenderColor;
        public RenderStates States;
        public RenderWindow Window;
        public View ScreenView;
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

        public View View
        {
            get => ScreenView;
            set
            {
                ScreenView = value;
                Window.SetView(ScreenView);
            }
        }

        public KRenderManager(RenderWindow window, VertexBuffer screenBuffer)
        {
            BackgroundColor = Color.Black;
            RenderColor = Color.White;
            States = RenderStates.Default;

            Window = window;
            ScreenBuffer = screenBuffer;
            ScreenView = window.GetView();
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

                //Old (SFML 2.6.2)
                //A QuadBuffer[0] = new Vertex((bounds.Left, bounds.Top), Color.White, (0, 0)); 
                //B QuadBuffer[1] = new Vertex((bounds.Left + bounds.Width, bounds.Top), Color.White, (texture.Size.X, 0)); 
                //C QuadBuffer[2] = new Vertex((bounds.Left + bounds.Width, bounds.Top + bounds.Height), Color.White, (texture.Size.X, texture.Size.Y)); 
                //D QuadBuffer[3] = new Vertex((bounds.Left, bounds.Top + bounds.Height), Color.White, (0, texture.Size.Y)); 
                
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

            if (_sBuffvCount > 0)
            {
                ScreenBuffer.Draw(Window, States);
                _sBuffvCount = 0;
            }

            Window.Display();
        }

        public void DrawBuffer(Vertex[] vertices, uint vCount, int layer = 0) => 
            DrawLayers[layer].Draw(vertices, vCount);

            public void DrawToScreen(Vertex[] vertices, uint vCount)
        {
            ScreenBuffer.Update(vertices, vCount, _sBuffvCount);
            _sBuffvCount += vCount;
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

            ScreenBuffer.Update(drawBuffer, 4, _sBuffvCount);
            _sBuffvCount += 4;
        }

        public void DrawRect(float x, float y, float width, float height, Color color, int layer = 0)
        {
            //ABD
            drawBuffer[0] = new Vertex((x, y), color);
            drawBuffer[1] = new Vertex((x + width, y), color);
            drawBuffer[2] = new Vertex((x, y + height), color);
            //BCD
            drawBuffer[3] = new Vertex((x + width, y), color);
            drawBuffer[4] = new Vertex((x + width, y + height), color);
            drawBuffer[5] = new Vertex((x, y + height), color);
            
            DrawLayers[layer].Draw(drawBuffer, 6);
        }
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


        public void DrawRect(in FloatRect rec, Color color, int layer = 0)
        {
            //ABD
            drawBuffer[0] = new Vertex(rec.Position, color);
            drawBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            drawBuffer[2] = new Vertex((rec.Left, rec.Top + rec.Height), color);
            //BCD
            drawBuffer[3] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            drawBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), color);
            drawBuffer[5] = new Vertex((rec.Left, rec.Top + rec.Height), color);

            DrawLayers[layer].Draw(drawBuffer, 6);
        }

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

        public void DrawText(KTextBox textBox, int layer = 1)
        {
            DrawLayers[layer].Draw(textBox.Text.VertexBuffer, textBox.Text.VertexCount);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            
            ScreenView.Size = (Vector2f)e.Size;
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

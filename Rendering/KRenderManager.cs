using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Buffers;

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
        public KTextHandler TextHandler;
        public KDrawLayer[] DrawLayers;
        public Vertex[] QuadBuffer;

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
            TextHandler = new(this);   
            QuadBuffer = new Vertex[6];
            DrawLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(KDrawLayer[] drawLayers)
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
                QuadBuffer[0] = new Vertex((bounds.Left, bounds.Top), Color.White, (0, 0)); 
                QuadBuffer[1] = new Vertex((bounds.Left + bounds.Width, bounds.Top), Color.White, (texture.Size.X, 0)); 
                QuadBuffer[2] = new Vertex((bounds.Left, bounds.Top + bounds.Height), Color.White, (0, texture.Size.Y)); 
                //BCD
                QuadBuffer[3] = new Vertex((bounds.Left + bounds.Width, bounds.Top), Color.White, (texture.Size.X, 0));  
                QuadBuffer[4] = new Vertex((bounds.Left + bounds.Width, bounds.Top + bounds.Height), Color.White, (texture.Size.X, texture.Size.Y)); 
                QuadBuffer[5] = new Vertex((bounds.Left, bounds.Top + bounds.Height), Color.White, (0, texture.Size.Y)); 

                Window.Draw(QuadBuffer, PrimitiveType.Triangles, new RenderStates(texture));
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
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex(a, color);
            //B QuadBuffer[1] = new Vertex((b.X, a.Y), color);
            //C QuadBuffer[2] = new Vertex((b.X, b.Y), color);
            //D QuadBuffer[3] = new Vertex((a.X, b.Y), color);

            //ABD
            QuadBuffer[0] = new Vertex(a, color);
            QuadBuffer[1] = new Vertex((b.X, a.Y), color);
            QuadBuffer[2] = new Vertex((a.X, b.Y), color);
            //BCD
            QuadBuffer[3] = new Vertex((b.X, a.Y), color);
            QuadBuffer[4] = new Vertex((b.X, b.Y), color);
            QuadBuffer[5] = new Vertex((a.X, b.Y), color);

            ScreenBuffer.Update(QuadBuffer, 4, _sBuffvCount);
            _sBuffvCount += 4;
        }

        public void DrawRect(float x, float y, float width, float height, Color color, int layer = 0)
        {
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex((x, y), color);
            //B QuadBuffer[1] = new Vertex((x + width, y), color);
            //C QuadBuffer[2] = new Vertex((x + width, y + height), color);
            //D QuadBuffer[3] = new Vertex((x, y + height), color);

            //ABD
            QuadBuffer[0] = new Vertex((x, y), color);
            QuadBuffer[1] = new Vertex((x + width, y), color);
            QuadBuffer[2] = new Vertex((x, y + height), color);
            //BCD
            QuadBuffer[3] = new Vertex((x + width, y), color);
            QuadBuffer[4] = new Vertex((x + width, y + height), color);
            QuadBuffer[5] = new Vertex((x, y + height), color);
            
            DrawLayers[layer].Draw(QuadBuffer, 6);
        }
        public void DrawRect(Vector2f a, Vector2f b, Color color, int layer = 0)
        {
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex(a, color);
            //B QuadBuffer[1] = new Vertex((b.X, a.Y), color);
            //C QuadBuffer[2] = new Vertex((b.X, b.Y), color);
            //D QuadBuffer[3] = new Vertex((a.X, b.Y), color);

            //ABD
            QuadBuffer[0] = new Vertex(a, color);
            QuadBuffer[1] = new Vertex((b.X, a.Y), color);
            QuadBuffer[2] = new Vertex((a.X, b.Y), color);
            //BCD
            QuadBuffer[3] = new Vertex((b.X, a.Y), color);
            QuadBuffer[4] = new Vertex((b.X, b.Y), color);
            QuadBuffer[5] = new Vertex((a.X, b.Y), color);

            DrawLayers[layer].Draw(QuadBuffer, 6);
        }


        public void DrawRect(in FloatRect rec, Color color, int layer = 0)
        {
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex(rec.Position, color);
            //B QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            //C QuadBuffer[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), color);
            //D QuadBuffer[3] = new Vertex((rec.Left, rec.Top + rec.Height), color);

            //ABD
            QuadBuffer[0] = new Vertex(rec.Position, color);
            QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            QuadBuffer[2] = new Vertex((rec.Left, rec.Top + rec.Height), color);
            //BCD
            QuadBuffer[3] = new Vertex((rec.Left + rec.Width, rec.Top), color);
            QuadBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), color);
            QuadBuffer[5] = new Vertex((rec.Left, rec.Top + rec.Height), color);

            DrawLayers[layer].Draw(QuadBuffer, 6);
        }

        public void DrawRect(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex(rec.Position, dat.Color, dat.Sprite.TopLeft);
            //B QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            //C QuadBuffer[2] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            //D QuadBuffer[3] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            
            //ABD
            QuadBuffer[0] = new Vertex(rec.Position, dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            //BCD
            QuadBuffer[3] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            QuadBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[5] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);

            DrawLayers[layer].Draw(QuadBuffer, 6);
        }

        public void DrawRect(in KDrawData dat, in KRectangle rec, int layer = 0)
        {
            //Old (SFML 2.6.2)
            //A QuadBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            //B QuadBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            //C QuadBuffer[2] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            //D QuadBuffer[3] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            
            //ABD
            QuadBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
            QuadBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            QuadBuffer[2] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
            //BCD
            QuadBuffer[3] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
            QuadBuffer[4] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
            QuadBuffer[5] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);

            DrawLayers[layer].Draw(QuadBuffer, 6);
        }

        public void DrawText(in KText text, Vector2f position, int wrapThreshold = 0, int layer = 0)
        {
            var buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 6);
            var textBox = TextHandler.CreateTextbox(text, buffer, position, KProgram.FontSize, wrapThreshold);

            //Cursed but works for now. 
            DrawLayers[layer].States.Texture = KProgram.Fonts[0].GetTexture(KProgram.FontSize); //Heap go brrrrrrrr.
            DrawLayers[layer].Draw(textBox.Buffer, (uint) text.Text.Length * 4);

            ArrayPool<Vertex>.Shared.Return(textBox.Buffer);
        }

        public void DrawTextBox(in KText text, Vector2f position, out FloatRect box, int wrapThreshold = 0, int layer = 0)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                box = new FloatRect();
                return;
            }

            Vertex[] buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            //Cursed but works for now.
            box = CreateTextbox(text, KProgram.Fonts[0], buffer, position.X, position.Y, KProgram.FontSize, wrapThreshold);
            DrawLayers[layer].States.Texture = KProgram.Fonts[0].GetTexture(KProgram.FontSize);
            DrawLayers[layer].Draw(buffer, (uint)text.Text.Length * 4);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            
            ScreenView.Size = (Vector2f)e.Size;
            ScreenView.Center = ScreenView.Size / 2;
            Window.SetView(ScreenView);
        }
    }
}

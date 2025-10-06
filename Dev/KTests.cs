using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;
using SFML.System;

namespace Elements.D
{
    public class KTests
    {
        public void Init()
        {

        }

        public void Update()
        {
            if (KProgram.InputManager.ScrollDelta < 0) KProgram.DrawManager.View.Zoom(-KProgram.InputManager.ScrollDelta);
            if (KProgram.InputManager.ScrollDelta > 0) KProgram.DrawManager.View.Zoom(1 / KProgram.InputManager.ScrollDelta);

            Vector2f offset = new Vector2f(0, 0);
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.W)) offset.Y -= 5;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.S)) offset.Y += 5;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.A)) offset.X -= 5;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.D)) offset.X += 5;
            KProgram.DrawManager.View.Move(offset);
        }

        public void FrameUpdate(KDrawManager drawManager)
        {
            var width = drawManager.View.Size.X;
            var height = drawManager.View.Size.Y;
            var posX = drawManager.View.Center.X - width / 2;
            var posY = drawManager.View.Center.Y - height / 2;
            Color color = new(50, 50, 50);

            drawManager.SubmitDraw(
                new KDrawData() 
                {
                    Color = Color.Green,
                },
                new KRectangle() 
                {
                    Width = 16,
                    Height = 16,
                }
            );

            for (int row = 0; row < height / 16; row++)
            {
                var y = MathF.Round(posY / 16) * 16 + row * 16;
                drawManager.SubmitDraw(new Vertex[]
                {
                    new Vertex()
                    {
                        Position = new(posX, y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(posX + width, y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(posX + width, y + 1),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(posX, y + 1),
                        Color = color,
                    },
                });
            }
            for (int col = 0; col < width / 16; col++)
            {
                var x = MathF.Round(posX / 16) * 16 + col * 16;
                drawManager.SubmitDraw(new Vertex[]
                {
                    new Vertex()
                    {
                        Position = new(x, posY),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x + 1, posY),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x + 1, posY + height),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x, posY + height),
                        Color = color,
                    },
                });
            }
        }
    }
}

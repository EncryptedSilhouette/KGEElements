using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Dev
{
    public class KDebugger
    {
        public KDebugger() 
        {

        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            int cellWidth = 16, cellHeight = 16;
            Color color = new(50,50,50);
            var size = renderManager.CameraViews[0].Size;
            var position = renderManager.CameraViews[0].Center - size / 2;
            
            for (int col = 0; col < renderManager.Window.Size.X / cellWidth; col++) 
            {
                var x = MathF.Round(position.X / cellWidth) * cellWidth + cellWidth * col;

                Vertex[] vertices =
                [
                    new Vertex() 
                    {
                        Position = new(x, position.Y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x + 1, position.Y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x + 1, position.Y + size.Y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(x, position.Y + size.Y),
                        Color = color,
                    }
                ];
                renderManager.SubmitDraw(vertices);
            }

            for (int row = 0; row < renderManager.Window.Size.Y / cellHeight; row++)
            {
                var y = MathF.Round(position.Y / cellHeight) * cellHeight + cellHeight * row;

                Vertex[] vertices =
                [
                    new Vertex()
                    {
                        Position = new(position.X, y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(position.X + size.X + 1, y),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(position.X + size.X, y + 1),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new(position.X, y + 1),
                        Color = color,
                    }
                ];
                renderManager.SubmitDraw(vertices);
            }

            Vertex[] v =
            [
                new Vertex()
                {
                    Position = new(KProgram.Window.Size.X / 2 - 4, KProgram.Window.Size.Y / 2 - 4),
                    Color = color,
                },
                new Vertex()
                {
                    Position = new(KProgram.Window.Size.X / 2 + 4, KProgram.Window.Size.Y / 2 - 4),
                    Color = color,
                },
                new Vertex()
                {
                    Position = new(KProgram.Window.Size.X / 2 + 4, KProgram.Window.Size.Y / 2 + 4),
                    Color = color,
                },
                new Vertex()
                {
                    Position = new(KProgram.Window.Size.X / 2 - 4, KProgram.Window.Size.Y / 2 + 4),
                    Color = color,
                }
            ];
            renderManager.SubmitDraw(v);

        }
    }
}

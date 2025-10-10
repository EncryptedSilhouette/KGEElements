using Elements.Core;
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
            Color color = Color.White;

            for (int i = 0; i < renderManager.Window.Size.X / cellWidth; i++) 
            {
                renderManager.Cameras[0].Viewport
                    .Deconstruct(out float left, out float top, out float width, out float height);

                Vertex[] vertices =
                [
                    new Vertex() 
                    {
                        Position = new((int) (left / cellWidth) * cellWidth, top),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new((int) (left / cellWidth) * cellWidth + cellWidth, top),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new((int) (left / cellWidth) * cellWidth + cellWidth, height),
                        Color = color,
                    },
                    new Vertex()
                    {
                        Position = new((int) (left / cellWidth) * cellWidth, height),
                        Color = color,
                    }
                ];
            }

            for (int i = 0; i < renderManager.Window.Size.Y / cellHeight; i++)
            {

            }

        }
    }
}

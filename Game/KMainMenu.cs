using Elements.Core;
using Elements.Rendering;

namespace Elements.Game
{
    public class KMainMenu
    {
        const int OPTIONS = 1;

        private KDrawData[] _elements;
        private KRectangle[] _elementBounds; 

        public KMainMenu()
        {
            _elements = 
            [
                new KDrawData() 
                {
                    Layer = 0,
                    Order = 0,
                    Color = new(100,100,100,100),
                    Sprite = new(),
                }   ,
                new KDrawData()
                {
                    Layer = 0,
                    Order = 0,
                    Color = new(100,100,100,100),
                    Sprite = new(),
                }
            ];
            _elementBounds =
            [
                new KRectangle()
                {
                    Width = 200,
                    Height = 300,
                    Transform = new(
                        KProgram.RenderManager.ScreenCenter.X,
                        KProgram.RenderManager.ScreenCenter.Y)
                },
                new KRectangle()
                {
                    Width = 150,
                    Height = 250,
                    Transform = new(
                        KProgram.RenderManager.ScreenCenter.X,
                        KProgram.RenderManager.ScreenCenter.Y)
                },
            ];
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                renderManager.SubmitDrawRect(_elements[i], _elementBounds[i]);
            }
        }
    }
}

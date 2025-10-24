using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Game
{
    public class MainMenu
    {
        private KDrawData[] _elements;
        private KRectangle[] _elementBounds; 

        public MainMenu()
        {
            _elements = 
            [
                new KDrawData() 
                {
                    Layer = 0,
                    Order = 0,
                    Color = Color.White,
                    Sprite = new(),
                }    
            ];
            _elementBounds = 
            [
                new KRectangle() 
                {
                    Width = 200,
                    Height = 50,
                } 
            ];
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                renderManager.SubmitDraw(_elements[i], new());
            }
        }
    }
}

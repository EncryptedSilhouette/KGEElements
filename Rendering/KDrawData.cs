using Elements.Core;
using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KDrawData
    {
        public Color Color;
        public KRectangle Sprite;

        public KDrawData()
        {
            Color = Color.White;
            Sprite = new KRectangle();
        }
    }
}

using Elements.Core;
using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KDrawData
    {
        public byte Layer;
        public byte Order;
        public Color Color;
        public KRectangle Sprite;
        //public KRectangle Bounds;

        public KDrawData()
        {
            Layer = 0;
            Order = 0;
            Color = Color.White;
            Sprite = new KRectangle();
        }
    }
}

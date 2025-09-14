using SFML.Graphics;

namespace Elements.Core.Drawing
{
    public struct KDrawData
    {
        public int Layer;
        public int Order;
        public Color Color;
        public KRectangle Sprite;
        public KRectangle DrawBounds;
    }
}

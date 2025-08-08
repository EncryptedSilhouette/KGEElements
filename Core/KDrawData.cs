using SFML.Graphics;

namespace Elements.Core
{
    public struct KDrawData
    {
        public int Layer;
        public float Rotation;
        public Color Color;
        public KSprite Sprite;
        public KDrawData[] Children;
    }
}

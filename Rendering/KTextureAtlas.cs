using Elements.Core;
using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KTextureAtlas
    {
        public Texture Texture;
        public Dictionary<string, KRectangle> Sprites;
    }
}
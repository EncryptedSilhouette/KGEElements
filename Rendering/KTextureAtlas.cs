using Elements.Core;
using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KTextureAtlas
    {
        public Texture Texture;
        public Dictionary<string, KRectangle> Sprites;
    }
}
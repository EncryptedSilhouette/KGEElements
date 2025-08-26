using SFML.Graphics;

namespace Elements.Core
{
    public struct KSprite
    {
        public KRectangle TextureBounds;
        public string Name;

        public KSprite()
        {
            TextureBounds = new()
            {
                Width = 1,
                Height = 1,
                Transform = new(),
            };
            Name = string.Empty;
        }

        public KSprite(in KRectangle textureBounds, string name, Texture texture) =>
            (TextureBounds, Name) = (textureBounds, name);

        public KSprite(in int width, in int height, in int posX, in int posY, string name)
        {
            TextureBounds = new()
            {
                Width = width,
                Height = height,
                Transform = new()
                {
                    PosX = posX,
                    PosY = posY,
                }
            };
            Name = name;
        }
    }
}

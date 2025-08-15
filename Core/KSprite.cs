using SFML.Graphics;

namespace Elements.Core
{
    public struct KSprite
    {
        public KRectangle TextureBounds;
        public string Name;
        public Texture Texture;

        public KSprite(in KRectangle textureBounds, string name, Texture texture) =>
            (TextureBounds, Name, Texture) = (textureBounds, name, texture);

        public KSprite(in int width, in int height, in int posX, in int posY, string name, Texture texture)
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
            (Name, Texture) = (name, texture);
        }
    }
}

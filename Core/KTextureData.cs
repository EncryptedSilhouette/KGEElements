using SFML.Graphics;

namespace Elements.Core
{
    public class KTextureData
    {
        #region Static

        public static KTextureData Load(string fileDataPath)
        {
            string[] contents = File.ReadAllLines(fileDataPath);
            
            Texture texture = new(contents[1].Trim(','));
            KSprite[] sprites = new KSprite[contents.Length - 2];

            for (int i = 2; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');
                Console.WriteLine(i);
                sprites[i - 2] = new(
                    texture,
                    values[0],
                    Convert.ToInt32(values[1]),
                    Convert.ToInt32(values[2]),
                    Convert.ToInt32(values[3]),
                    Convert.ToInt32(values[4]),
                    Convert.ToInt32(values[5]),
                    Convert.ToInt32(values[6]));
            }
            return new(texture, sprites);
        }

        #endregion

        private KSprite[] _sprites;

        public Texture Texture { get; }

        public ref KSprite this[int spriteID] => ref _sprites[spriteID];

        public KTextureData(Texture texture, KSprite[] sprites)
        {
            Texture = texture;
            _sprites = sprites;
        }
    }

    public struct KSprite
    {
        public int Width;
        public int Height;
        public int TexCoordsX;
        public int TexCoordsY;
        public int CenterX;
        public int CenterY;
        public string Name;
        public Texture Texture;

        public KSprite(Texture texture, string name, int width, int height, int texCoordsX, int texCoordsY, int centerX, int centerY)
        {
            Width = width;
            Height = height;
            TexCoordsX = texCoordsX;
            TexCoordsY = texCoordsY;
            CenterX = centerX;
            CenterY = centerY;
            Name = name;
            Texture = texture;
        }
    }
}

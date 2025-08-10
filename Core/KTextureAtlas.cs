#nullable disable

using SFML.Graphics;
using System.Diagnostics;

namespace Elements.Core
{
    public class KTextureAtlas
    {
        public static KTextureAtlas Load(string fileDataPath)
        {
            string[] contents = File.ReadAllLines(fileDataPath);

            //Bad :(
            Debug.Assert(contents != null && contents.Length > 0);
            
            Texture texture = new(contents[1].Trim(','));
            //contents.Length - 1 skips first line, sprites[0] is for the texture itself.
            KSprite[] sprites = new KSprite[contents.Length - 1];

            //Saves the texture itself as a sprite
            sprites[0] = new KSprite(texture)
            {
                Width = (int) texture.Size.X,
                Height = (int) texture.Size.Y,
                TexCoordsX = 0,
                TexCoordsY = 0
            };

            //Goes over each line, reading comma seperated values.
            for (int i = 2; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');
                sprites[i - 1] = new(texture, values[0])
                {
                    Width = Convert.ToInt32(values[1]),
                    Height = Convert.ToInt32(values[2]),
                    TexCoordsX = Convert.ToInt32(values[3]),
                    TexCoordsY = Convert.ToInt32(values[4])
                };
            }
            return new(texture, sprites);
        }

        public Texture Texture { get; init; }
        public List<KSprite> Sprites { get; init; }

        //Use with init block only
        public KTextureAtlas() { }

        public KTextureAtlas(Texture texture, IEnumerable<KSprite> sprites)
        {
            Texture = texture;
            Sprites = new(sprites);
        }
    }

    public class KSprite
    {
        public int Width;
        public int Height;
        public int TexCoordsX;
        public int TexCoordsY;
        public string Name;
        public Texture Texture;

        public KSprite(Texture texture, string? name = null)
        {
            Name = name ?? string.Empty;
            Texture = texture;
        }
    }
}

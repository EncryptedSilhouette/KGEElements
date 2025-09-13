using SFML.Graphics;
using System.Diagnostics;

namespace Elements.Core.Drawing
{
    public class KTextureAtlas
    {
        public static KTextureAtlas? Load(string fileDataPath)
        {
            string[] contents = File.ReadAllLines(fileDataPath);

            Texture? texture = null;
            Dictionary<string, KRectangle> sprites = new(64);

            for (int i = 1; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');

                if (values[i] == "tex") texture = new Texture(values[i]);

                if (values[i] == "sprite")
                {
                    sprites.Add(values[i++], new KRectangle()
                    {
                        Width = Convert.ToInt32(values[i++]),
                        Height = Convert.ToInt32(values[i++]),
                        Transform = new()
                        {
                            PosX = Convert.ToInt32(values[i++]),
                            PosY = Convert.ToInt32(values[i++])
                        }
                    });
                }
            }

            if (texture == null) return null;
            else return new(texture, sprites);
        }

        public Texture Texture { get; private set; }
        public Dictionary<string, KRectangle> Sprites { get; private set; }

        public KRectangle Self => Sprites[string.Empty];

        public KTextureAtlas(Texture texture, IDictionary<string, KRectangle> sprites)
        {
            Texture = texture;
            Sprites = new(sprites);
        }
    }
}
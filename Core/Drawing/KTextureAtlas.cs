using SFML.Graphics;

namespace Elements.Core.Drawing
{
    public class KTextureAtlas
    {
        public static KTextureAtlas? Load(string fileDataPath)
        {
            string[] contents = File.ReadAllLines(fileDataPath);

            Texture? texture = null;
            Dictionary<string, KRectangle> sprites = new(64);

            for (int i = 0; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');
                var element = 0;

                switch (values[element])
                {
                    case "tex":
                        texture = new Texture(values[element++]);
                        break;

                    case "sprite":
                        sprites.Add(values[element++], new KRectangle()
                        {
                            Width = Convert.ToInt32(values[element++]),
                            Height = Convert.ToInt32(values[element++]),
                            Transform = new()
                            {
                                PosX = Convert.ToInt32(values[element++]),
                                PosY = Convert.ToInt32(values[element++])
                            }
                        });
                        break;

                    default:
                        break;
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
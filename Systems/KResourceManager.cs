using Elements.Core;

namespace Elements.Systems
{
    public class KResourceManager
    {
        public KTextureAtlas[] KTextureAtlases = [];

        public void Load()
        {
            var resources = File.ReadAllLines("res.csv");
            foreach (var line in resources) 
            {
                var values = line.Split(',');

                switch (values[0])
                {
                    case "atlases":
                        KTextureAtlases = LoadAtlases(values.AsSpan(1, values.Length - 1));
                        break;

                    default:
                        break;
                }
            }
        }

        public KTextureAtlas[] LoadAtlases(Span<string> filePaths)
        {
            KTextureAtlas[] textures = new KTextureAtlas[filePaths.Length];
            for (var i = 0; i < filePaths.Length; i++)
            {
                textures[i] = KTextureAtlas.Load(filePaths[i]);
            }
            return textures;
        }
    }
}

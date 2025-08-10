namespace Elements.Core.Systems
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
                        KTextureAtlases = LoadAtlases(values.AsSpan(1, values.Length - 1)).ToArray();
                        break;

                    default:
                        break;
                }
            }
        }

        public IEnumerable<KTextureAtlas> LoadAtlases(Span<string> filePaths)
        {
            for (var i = 0; i < filePaths.Length; i++)
            {
                yield return KTextureAtlas.Load(filePaths[i]);
            }
        }
    }
}

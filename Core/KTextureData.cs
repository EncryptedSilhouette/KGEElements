using SFML.Graphics;

namespace Elements.Core
{
    public class KTextureData
    {
        Texture Texture;
        KDrawData[] sprites;

        public KTextureData(string dataFilePath)
        {
            var contents = File.ReadAllText("template.txt").Split(',');

            Console.WriteLine();
            for (int i = 0; i < contents.Length; i++)
            {
                var token = contents[i];
                switch (token)
                {
                    case "-s":

                        break;

                    default:
                        break;
                }
            }
        }
    }
}

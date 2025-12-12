using Elements.Core;
using SFML.Graphics;

namespace Elements
{
    public record struct KTextureAtlas(Texture Texture, Dictionary<string, KRectangle> Sprites);

    public class KResourceManager
    {

        public string ConfigPath;
        public Dictionary<string, KTextureAtlas> TextureAtlases = [];
        public Font[] Fonts = [];

        public KResourceManager(string configPath)
        {
            ConfigPath = configPath;
        }

        public void Load()
        {
            var resources = File.ReadAllLines(ConfigPath);

            for (int i = 0; i < resources.Length; i++)
            {
                var values = resources[i].Split(',');

                switch (values[0])
                {
                    case "title":
                        try { KProgram.Title = values[1]; }
                        catch (Exception) { Console.WriteLine("failed to read \'title\'"); }
                        break;

                    case "frame_target":
                        try { KProgram.FrameLimit = Convert.ToUInt32(values[1]); }
                        catch (Exception) { Console.WriteLine("failed to read \'frame_target\'"); }
                        break;

                    case "vsync":
                        try { KProgram.VSync = bool.TrueString == values[1]; }
                        catch (Exception) { Console.WriteLine("failed to read \'vsync\'"); }
                        break;

                    case "resolution":
                        try
                        {
                            KProgram.TargetResolution = new SFML.System.Vector2u(
                                Convert.ToUInt32(values[1]),
                                Convert.ToUInt32(values[2]));
                        }
                        catch (Exception) { Console.WriteLine("failed to read \'resolution\'"); }
                        break;

                    case "atlases":
                        for (int j = 1; j < values.Length; j++) LoadAtlas(values[j]);
                        break;

                    case "fonts":
                        Fonts = new Font[values.Length];

                        for (int j = 1; j < Fonts.Length; j++) 
                        {
                            Fonts[j - 1] = new Font(values[j]);
                            KProgram.LogManager.DebugLog($"Loading font: {Path.GetFileNameWithoutExtension(values[j])}.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public void LoadAtlas(string filePath)
        {
            var contents = File.ReadAllLines(filePath);

            //First line should be the texture path.
            KTextureAtlas atlas = new()
            {
                Texture = new(contents[0]),
                Sprites = new(64) 
            };
            //Add the full texture as default sprite.
            atlas.Sprites.Add(string.Empty, new(atlas.Texture.Size.X, atlas.Texture.Size.Y));

            KProgram.LogManager.DebugLog($"Loading texture atlas: {Path.GetFileNameWithoutExtension(filePath)}.");

            for (int i = 1; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');

                switch (values[0])
                {
                    case "sprite":
                        atlas.Sprites.Add(values[1], new KRectangle()
                        {
                            Transform = new()
                            {
                                PosX = Convert.ToInt32(values[2]),
                                PosY = Convert.ToInt32(values[3])
                            },
                            Width = Convert.ToInt32(values[4]),
                            Height = Convert.ToInt32(values[5])
                        });
                        KProgram.LogManager
                            .DebugLog($"Added sprite: {values[1]} to atlas: {Path.GetFileNameWithoutExtension(filePath)}.");
                        break;

                    default:
                        break;
                }
            }  
            TextureAtlases.Add(Path.GetFileNameWithoutExtension(contents[0]), atlas);
        }
    }
}

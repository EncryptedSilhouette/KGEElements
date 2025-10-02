using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements
{
    public class KResourceManager
    {
        public string ConfigPath;
        public Dictionary<string, KTextureAtlas> TextureAtlases = [];
        public Dictionary<string, Font> Fonts = [];

        public KResourceManager(string configPath)
        {
            ConfigPath = configPath;
        }

        public void Load()
        {
            var resources = File.ReadAllLines(ConfigPath);
            foreach (var line in resources) 
            {
                var values = line.Split(',');

                switch (values[0])
                {
                    case "title":
                        try { KProgram.Title = values[1]; }
                        catch (Exception) { break; }
                        break;

                    case "frame_target":
                        try { KProgram.FrameLimit = Convert.ToUInt32(values[1]); }
                        catch (Exception) { break; }
                        break;

                    case "vsync":
                        try { KProgram.VSync = bool.TrueString == values[1]; }
                        catch (Exception) { break; }
                        break;

                    case "atlases":
                        for (int i = 1; i < values.Length; i++) LoadAtlas(values[i]);
                        break;

                    case "fonts":
                        for (int i = 1; i < values.Length; i++) LoadFont(values[i]);
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

        public void LoadFont(string filePath)
        {
            Fonts.Add(Path.GetFileNameWithoutExtension(filePath), new Font(filePath));
            KProgram.LogManager.DebugLog($"Loading font: {Path.GetFileNameWithoutExtension(filePath)}.");
        }
    }
}

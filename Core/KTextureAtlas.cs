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
            sprites[0] = new()
            {
                Name = string.Empty,
                TextureBounds = new KRectangle()
                {
                    Width = (int)texture.Size.X,
                    Height = (int)texture.Size.Y,
                    Transform = new()
                    {
                        PosX = 0,
                        PosY = 0
                    }
                }
            }; 

            //Goes over each line, reading comma seperated values.
            for (int i = 2; i < contents.Length; i++)
            {
                var values = contents[i].Split(',');

                sprites[i - 1] = new()
                {
                    Name = values[0],
                    TextureBounds= new KRectangle()
                    {
                        Width = Convert.ToInt32(values[1]),
                        Height = Convert.ToInt32(values[2]),
                        Transform = new()
                        {
                            PosX = Convert.ToInt32(values[3]),
                            PosY = Convert.ToInt32(values[4])
                        }
                    }
                };
            }
            return new(texture, sprites);
        }

        private int _spriteCount;

        public Texture Texture;
        public KSprite[] Sprites;

        public KTextureAtlas(Texture texture, KSprite[] sprites)
        {
            Texture = texture;
            Sprites = sprites;
            _spriteCount = sprites.Length;
        }

        public void AddSprite(KSprite sprite)
        {
            if (_spriteCount >= Sprites.Length)
            {
                var newArr = new KSprite[Sprites.Length * 2];
                Array.Copy(Sprites, newArr, _spriteCount);
                Sprites = newArr;
            }
            Sprites[_spriteCount] = sprite;
            _spriteCount++;
        }

        //wip
        //public void AddSprites(KSprite[] sprites)
        //{
        //    var reqSize = _spriteCount + sprites.Length;

        //    if (reqSize >= Sprites.Length)
        //    {
        //        var newArr = new KSprite[Sprites.Length * 2 * reqSize / Sprites.Length];
        //        Array.Copy(Sprites, newArr, _spriteCount);
        //        Sprites = newArr;
        //    }
        //    Array.Copy(sprites, 0, Sprites, _spriteCount, sprites.Length);
        //    _spriteCount = reqSize;
        //}
    }
}

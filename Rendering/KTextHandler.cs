using SFML.Graphics;

namespace Elements.Rendering
{
    public class KTextManager
    {
        public int[] TextLayers;
        public Font[] Fonts;

        public KTextManager()
        {
            TextLayers = [];
            Fonts = [];
        }

        public KTextManager(int[] textLayers, Font[] fonts)
        {
            TextLayers = textLayers;
            Fonts = fonts;
        }
        
        public void Update()
        {
            
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            for (int i = 0; i < TextLayers.Length; i++)
            {
                renderer.DrawLayers[i].Draw();
            }
        }

        public KTextBox CreateTextBox(int fontID, Vector2f position, Color color, 
            bool bold = false, 
            byte lnSpacing = 4,
            byte lnThickness = 0, 
            byte fontSize = 14, 
            int wrapThreshold = 0)
        {
            FloatRect bounds = new FloatRect(position, (0,0));

            if (string.IsNullOrEmpty(Text)) return new KTextBox(bounds, this);

            var chars = Text.AsSpan();

            for (int i = 0; i < chars.Length && i * 6 <= VertexBuffer.Length; i++)
            {
                if (chars[i] == '\n')
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                    continue;    
                }

                KGlyphHandle handle = new(0, chars[fontID], fontSize, false, 0);

                var glyph = KProgram.GetGlyphFromCache(0, handle);

                if (wrapThreshold > 0 && bounds.Size.X + glyph.Advance > wrapThreshold)
                {
                    bounds.Position.X = 0;
                    bounds.Position.Y -= fontSize + lnSpacing;
                }

                //ABC
                VertexBuffer[i * 6] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 1] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 2] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                //BCD
                VertexBuffer[i * 6 + 3] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Color = color,
                };
                VertexBuffer[i * 6 + 4] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                VertexBuffer[i * 6 + 5] = new()
                {
                    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
                                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                    Color = color,
                };
                bounds.Position.X += glyph.Advance;
            }

            return new KTextBox(bounds, this);
        }
    }
}
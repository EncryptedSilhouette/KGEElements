using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KText
    {
        public bool IsBold;
        public int LineThickness;
        public int Spacing;
        public uint FontSize;    
        public string Text;
        public Font Font;
        public KDrawData DrawData;

        public KText(Font font)
        {
            Spacing = 0;
            FontSize = 12;
            Text = string.Empty;
            Font = font;
            DrawData = new()
            {
                Color = Color.White,
                Layer = 0,
                Order = 0,
                Sprite = new()
                {
                    Width = font.GetTexture(FontSize).Size.X,
                    Height = font.GetTexture(FontSize).Size.X,
                }
            };
        }

        public Vertex[] CreateTextBox(float posX, float posY, int width, int height, int wrapThreshold = 0)
        {
            int offsetX = 0;
            int offsetY = 0;

            Vertex[] buffer = new Vertex[Text.Length * 4];
            var characters = Text.AsSpan();

            if (wrapThreshold <= 0) wrapThreshold = width;

            for (int i = 0; i < characters.Length; i++)
            {
                var glyph = Font.GetGlyph(characters[i], FontSize, IsBold, LineThickness);

                buffer[i] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Left, glyph.TextureRect.Top), 
                    Position = new(offsetX, offsetY),
                    Color = DrawData.Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Position = new(offsetX + glyph.Bounds.Width, offsetY),
                    Color = DrawData.Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Width, glyph.TextureRect.Height),
                    Position = new(offsetX + glyph.Bounds.Width, glyph.Bounds.Height),
                    Color = DrawData.Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Left, glyph.TextureRect.Height),
                    Position = new(offsetX, glyph.Bounds.Height),
                    Color = DrawData.Color
                };

                offsetX += (int) glyph.Advance;

                if (posX >= wrapThreshold) 
                {
                    offsetX = 0;
                    offsetY += (int) glyph.Bounds.Height + Spacing;
                }
            }
            return buffer;
        }
    }
}

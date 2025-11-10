using SFML.Graphics;

namespace Elements.Drawing
{
    public struct KText
    {
        public bool Bold;
        public byte LineThickness;
        public byte Spacing;
        public byte FontSize;    
        public Color Color;
        public string Text;

        public KText(Font font)
        {
            Bold = false;
            LineThickness = 1;
            Spacing = 0;
            FontSize = 12;
            Color = Color.White;
            Text = string.Empty;
        }

        public Vertex[] CreateTextBox(float posX, float posY, int width, int height, Font font, int wrapThreshold = 0)
        {
            int offsetX = 0;
            int offsetY = 0;

            Vertex[] buffer = new Vertex[Text.Length * 4];
            var characters = Text.AsSpan();

            if (wrapThreshold <= 0) wrapThreshold = width;

            for (int i = 0; i < characters.Length; i++)
            {
                var glyph = font.GetGlyph(characters[i], FontSize, Bold, LineThickness);

                buffer[i] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Left, glyph.TextureRect.Top), 
                    Position = new(offsetX, offsetY),
                    Color = Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Width, glyph.TextureRect.Top),
                    Position = new(offsetX + glyph.Bounds.Width, offsetY),
                    Color = Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Width, glyph.TextureRect.Height),
                    Position = new(offsetX + glyph.Bounds.Width, glyph.Bounds.Height),
                    Color = Color
                };
                buffer[i++] = new Vertex()
                {
                    TexCoords = new(glyph.TextureRect.Left, glyph.TextureRect.Height),
                    Position = new(offsetX, glyph.Bounds.Height),
                    Color = Color
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

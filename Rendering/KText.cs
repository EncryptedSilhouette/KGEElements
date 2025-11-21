using SFML.Graphics;

namespace Elements.Rendering
{
    public struct KText
    {
        public bool Bold;
        public byte LineThickness;
        public byte LineSpacing;
        public Color Color;
        public string Text;

        public KText(string text)
        {
            Bold = false;
            LineThickness = 1;
            LineSpacing = 4;
            Color = Color.White;
            Text = text;
        }

        public KText(string text, Color color, bool bold = false, byte lineThickness = 0, byte lineSpacing = 4)
        {
            Bold = bold;
            LineThickness = lineThickness;
            LineSpacing = lineSpacing;
            Color = color;
            Text = text;
        }

        public Vertex[] CreateTextBox(float posX, float posY, Font font, uint size, int wrapThreshold = 0)
        {
            float xOffset = 0;
            float yOffset = 0;
            ReadOnlySpan<char> chars = Text.AsSpan();
            Vertex[] vertices = new Vertex[chars.Length * 4];

            for (int i = 0; i < chars.Length; i++)
            {
                var glyph = font.GetGlyph(chars[i], size, Bold, LineThickness);
                var coords = glyph.TextureRect;
                var bounds = glyph.Bounds;

                if ("\n".Contains(chars[i]) || (wrapThreshold != 0 && xOffset > wrapThreshold))
                {
                    xOffset = 0;
                    yOffset += LineSpacing + coords.Height;
                    continue;
                }

                vertices[i * 4] = new()
                {
                    Position = (bounds.Left + posX + xOffset, 
                                bounds.Top + posY + yOffset),
                    TexCoords = (coords.Left, coords.Top),
                    Color = Color,
                };
                vertices[i * 4 + 1] = new()
                {
                    Position = (bounds.Left + bounds.Width + posX + xOffset, 
                                bounds.Top + posY + yOffset),
                    TexCoords = (coords.Left + coords.Width, coords.Top),
                    Color = Color,
                };
                vertices[i * 4 + 2] = new()
                {
                    Position = (bounds.Left + bounds.Width + posX + xOffset, 
                                bounds.Top + bounds.Height + posY + yOffset),
                    TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
                    Color = Color,
                };
                vertices[i * 4 + 3] = new()
                {
                    Position = (bounds.Left + posX + xOffset,
                                bounds.Top + bounds.Height + posY + yOffset),
                    TexCoords = (coords.Left, coords.Top + coords.Height),
                    Color = Color,
                };

                xOffset += glyph.Advance;
            }
            return vertices;
        }
    }
}

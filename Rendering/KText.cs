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
            LineThickness = 0;
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
    }
}

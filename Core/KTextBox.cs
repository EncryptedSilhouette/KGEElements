namespace Elements.Core
{
    public enum KTextAlignment
    {
        Left, Center, Right, //Justify if i can ever justify it.
    }

    public struct KTextBox
    {
        public uint FontSize;
        public uint LineLength;
        public uint LineSpacing;
        public KTextAlignment Alignment;
        public KRectangle Bounds;
        public string? Text;

        public KTextBox(int x, int y, int width, int height, string? text)
        {
            FontSize = 12;
            LineLength = 200;
            LineSpacing = 8;
            Text = text;
            Bounds = new(width, height, new(x, y)); 
        }
    }
}

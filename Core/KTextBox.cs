namespace Elements.Core
{
    public enum KTextAlignment
    {
        Left, Center, Right, //Justify too hard
    }

    public struct KTextBox
    {
        public uint FontSize;
        public uint LineLength;
        public uint LineSpacing;
        public KTextAlignment Alignment;
        public KRectangle Bounds;
        public string Text;

        public KTextBox(string text)
        {
            Text = text;
        }
    }
}

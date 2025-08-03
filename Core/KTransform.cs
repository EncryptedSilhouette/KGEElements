namespace Elements.Core
{
    public struct KTransform
    {
        public float posX;
        public float posY;
        public float scaleX;
        public float scaleY;
        public float rotation;
    }

    public struct KRectangle
    {
        public float width;
        public float height;
        public KTransform transform;
    }
}

namespace Elements.Core
{
    public struct KTransform
    {
        public float PosX;
        public float PosY;
        public float ScaleX;
        public float ScaleY;
        public float Rotation;

        public KTransform() => (PosX, PosY, ScaleX, ScaleY, Rotation) = (0, 0, 1, 1, 0);

        public KTransform(in float posX, in float posY) => 
            (PosX, PosY, ScaleX, ScaleY, Rotation) = (posX, posY, 1, 1, 0);

        public KTransform(in float posX, in float posY, in float scaleX, in float scaleY) => 
            (PosX, PosY, ScaleX, ScaleY, Rotation) = (posX, posY, scaleX, scaleY, 0);

        public KTransform(in float posX, in float posY, in float scaleX, in float scaleY, in float rotation) =>
            (PosX, PosY, ScaleX, ScaleY, Rotation) = (posX, posY, scaleX, scaleY, rotation);
    }
}

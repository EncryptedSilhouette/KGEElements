using SFML.System;

namespace Elements.Core
{
    public struct KTransform
    {
        public float PosX;
        public float PosY;
        public float ScaleX;
        public float ScaleY;
        public float Rotation;
    }

    public struct KRectangle
    {
        public float Width;
        public float Height;
        public KTransform Transform;

        public Vector2f TopLeft
        {
            get
            {
                var tl = new Vector2f(-Width / 2 * Transform.ScaleX, -Height / 2 * Transform.ScaleY);
                
                if (Transform.Rotation != 0)
                {
                    tl.X = tl.X * MathF.Cos(Transform.Rotation) - tl.Y * MathF.Sin(Transform.Rotation);
                    tl.Y = tl.X * MathF.Sin(Transform.Rotation) - tl.Y * MathF.Cos(Transform.Rotation);
                }

                tl.X += Transform.PosX;
                tl.Y += Transform.PosY;
                return tl;
            }
        }

        public Vector2f TopRight
        {
            get
            {
                var tr = new Vector2f(Width / 2 * Transform.ScaleX, -Height / 2 * Transform.ScaleY);

                if (Transform.Rotation != 0)
                {
                    tr.X = tr.X * MathF.Cos(Transform.Rotation) - tr.Y * MathF.Sin(Transform.Rotation);
                    tr.Y = tr.X * MathF.Sin(Transform.Rotation) - tr.Y * MathF.Cos(Transform.Rotation);
                }

                tr.X += Transform.PosX;
                tr.Y += Transform.PosY;
                return tr;
            }
        }

        public Vector2f BottomRight
        {
            get
            {
                var br = new Vector2f(Width / 2 * Transform.ScaleX, Height / 2 * Transform.ScaleY);

                if (Transform.Rotation != 0)
                {
                    br.X = br.X * MathF.Cos(Transform.Rotation) - br.Y * MathF.Sin(Transform.Rotation);
                    br.Y = br.X * MathF.Sin(Transform.Rotation) - br.Y * MathF.Cos(Transform.Rotation);
                }

                br.X += Transform.PosX;
                br.Y += Transform.PosY;
                return br;
            }
        }

        public Vector2f BottomLeft
        {
            get
            {
                var bl = new Vector2f(-Width / 2 * Transform.ScaleX, Height / 2 * Transform.ScaleY);

                if (Transform.Rotation != 0)
                {
                    bl.X = bl.X * MathF.Cos(Transform.Rotation) - bl.Y * MathF.Sin(Transform.Rotation);
                    bl.Y = bl.X * MathF.Sin(Transform.Rotation) - bl.Y * MathF.Cos(Transform.Rotation);
                }

                bl.X += Transform.PosX;
                bl.Y += Transform.PosY;
                return bl;
            }
        }
    }
}

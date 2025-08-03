namespace Elements.Core
{
    public class KCollision
    {
        public static bool CheckCirclePointCollision(in float centerX, in float centerY, in float radius, in float posX, in float posY)
        {
            double distX = posX - centerX;
            double distY = posY - centerY;
            //checks hypotenuse without sqrt. Squareing negates negatives.
            //  a^2   +         b^2              c^2
            if (distX * distX + distY * distY <= radius * radius) return true;
            else return false;
        }

        public static bool CheckRectPointCollision(in float centerX, in float centerY, in float width, in float height, in float posX, in float posY)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            //if within x bounds
            if (posX >= centerX - halfWidth && posX <= centerX + halfWidth)
            {
                //if within y bounds
                if (posY >= centerY - halfHeight && posY <= centerY + halfHeight) return true;
            }
            return false;
        }

        public static bool CheckRectPointCollision(in KRectangle rectangle, in float posX, in float posY) =>
            //apply rotation to point
            CheckRectPointCollision(
                rectangle.transform.posX, rectangle.transform.posY,
                rectangle.width * rectangle.transform.scaleX, rectangle.height * rectangle.transform.scaleY,
                posX, posY);
    }
}

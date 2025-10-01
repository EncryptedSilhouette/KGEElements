namespace Elements.Extensions
{
    public static class KCollectionExtensions
    {
        public static bool IsValidIndex(this Array self, int i)
        {
            if (0 <= i && i < self.Length) return true;
            return false;
        }
    }
}

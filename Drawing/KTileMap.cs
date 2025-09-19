using Elements.Core;

namespace Elements.Drawing
{
    public struct KTileMap
    {
        public KDrawData[] TileSprites;
        public KGrid Grid;

        public KTileMap(int rows, int columns, KDrawData[] tileSprites)
        {
            (Grid, TileSprites) = (new(rows, columns), tileSprites);
        }
    }
}

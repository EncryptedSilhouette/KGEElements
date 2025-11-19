using Elements.Core;

namespace Elements.Rendering
{
    public struct KTileMap
    {
        public KDrawData[] TileSprites;
        public KGrid Grid;

        public KTileMap(int rows, int columns, int cellWidth, int cellHeight, KDrawData[] tileSprites)
        {
            (Grid, TileSprites) = (new(rows, columns, 16, 16), tileSprites);
        }
    }
}

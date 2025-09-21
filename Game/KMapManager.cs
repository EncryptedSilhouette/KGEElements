using Elements.Core;
using Elements.Drawing;

namespace Elements.Game
{
    public class KMapManager
    {
        public KTileMap TileMap;
        public KGameManager GameManager;

        const int RESOURCE_NODE_TILE = 10;

        public KMapManager(KGameManager gameManager) 
        {
            GameManager = gameManager;
        }

        public void Generate(int players)
        {
            var grid = TileMap.Grid;
            var nodes = new int[players * 4];

            for (int i = 0; i < nodes.Length; i++)
            {
                int index;
                do index = KProgram.RNG.Next(grid.Rows) * grid.Columns + KProgram.RNG.Next(grid.Columns);
                while (grid.Cells[index] == RESOURCE_NODE_TILE);

                grid.Cells[index] = RESOURCE_NODE_TILE;
                nodes[i] = index;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                
            }
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KDrawManager drawManager)
        {
            //Draw tile map.
            for (int i = 0; i < TileMap.Grid.CellCount; i++)
            {
                var rec = new KRectangle()
                {
                    Transform = new(TileMap.Grid.Columns % i * TileMap.Grid.CellWidth,
                                    TileMap.Grid.Columns / i * TileMap.Grid.CellHeight),
                    Width = TileMap.Grid.CellWidth,
                    Height = TileMap.Grid.CellHeight
                };
                drawManager.SubmitDraw(KGameManager.TILE_MAP_LAYER, TileMap.TileSprites[TileMap.Grid.Cells[i]], rec);
            }
        }
    }
}

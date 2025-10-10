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

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KRenderManager renderManager)
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
                renderManager.SubmitDraw(KGameManager.TILE_MAP_LAYER, TileMap.TileSprites[TileMap.Grid.Cells[i]], rec);
            }
        }

        public void Generate(int players)
        {
            bool filled = false, canPlace = false;
            Span<(int row, int col)> nodes = stackalloc (int, int)[players * 4];

            for (int n = 0; n < nodes.Length; n++)
            {
                nodes[n] = (KProgram.RNG.Next(TileMap.Grid.Rows), KProgram.RNG.Next(TileMap.Grid.Columns));
                TileMap.Grid[nodes[n].row, nodes[n].col] = 2;
            }

            for (int row, col, spread = 0; !filled && spread < 100; spread++)
            {
                for (int n = 0; n < nodes.Length; n++)
                {
                    //BottomRight
                    row = Math.Clamp(nodes[n].row + spread, 0, TileMap.Grid.Rows - 1);
                    col = Math.Clamp(nodes[n].col + spread, 0, TileMap.Grid.Columns - 1);
                    TryPlaceResourceNode(row, col, 1);

                    //BottomLeft
                    row = Math.Clamp(nodes[n].row + spread, 0, TileMap.Grid.Rows - 1);
                    col = Math.Clamp(nodes[n].col - spread, 0, TileMap.Grid.Columns - 1);
                    TryPlaceResourceNode(row, col, 1);


                    //TopRight
                    row = Math.Clamp(nodes[n].row - spread, 0, TileMap.Grid.Rows - 1);
                    col = Math.Clamp(nodes[n].col - spread, 0, TileMap.Grid.Columns - 1);
                    TryPlaceResourceNode(row, col, 1);


                    //TopLeft
                    row = Math.Clamp(nodes[n].row - spread, 0, TileMap.Grid.Rows - 1);
                    col = Math.Clamp(nodes[n].col + spread, 0, TileMap.Grid.Columns - 1);
                    TryPlaceResourceNode(row, col, 1);
                }

                filled = true;

                for (int r = 0; r < nodes.Length; r++)
                {
                    for (int c = 0; c < nodes.Length; c++)
                    {
                        if (TileMap.Grid[r, c] == 0) filled = false;
                    }
                }
            }
        }

        private void TryPlaceResourceNode(int row, int col, int nodeValue)
        {
            if (TileMap.Grid[row, col] > 8) return;

            
        }
    }
}

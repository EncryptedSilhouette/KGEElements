using Elements.Core;
using Elements.Drawing;

namespace Elements
{
    [Flags]
    public enum KGameState
    {
        PAUSED,
    }

    public class KGameManager
    {
        const int TILE_MAP_LAYER = 0;

        public KGameState GameState;
        public KTileMap TileMap;
        public KDrawManager DrawManager;
        //public IKEntityHandler EntityHandler;

        public KGameManager(KDrawManager drawManager) 
        {
            DrawManager = drawManager;
        }

        public void Update(in uint currentUpdate)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MainMenu.Update();
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KDrawManager drawManager)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;

            //Draw tile map.
            for (int i = 0; i < TileMap.Cells.Length; i++)
            {
                var rec = new KRectangle()
                {
                    Transform = new(TileMap.Columns % i * TileMap.CellWidth, 
                                    TileMap.Columns / i * TileMap.CellHeight),
                    Width = TileMap.CellWidth,
                    Height = TileMap.CellHeight
                };
                DrawManager.SubmitDraw(TILE_MAP_LAYER, TileMap.TileSprites[TileMap.Cells[i]], rec);
            }
        }
    }
}

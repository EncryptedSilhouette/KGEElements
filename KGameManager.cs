using Elements.Game;

namespace Elements
{
    [Flags]
    public enum KGameState
    {
        PAUSED,
    }

    public class KGameManager
    {
        public const int TILE_MAP_LAYER = 0;

        public KGameState GameState;
        public KMapManager MapManager;
        public KDrawManager DrawManager;
        //public IKEntityHandler EntityHandler;

        public KGameManager(KDrawManager drawManager) 
        {
            DrawManager = drawManager;
            MapManager = new(this);
        }

        public void Update(in uint currentUpdate)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MainMenu.Update();
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KDrawManager drawManager)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;

            MapManager.FrameUpdate(currentUpdate, currentFrame, drawManager);
        }
    }
}

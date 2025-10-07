using Elements.D;
using Elements.Drawing;

namespace Elements.Game
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

        public KTests Tests = new();

        public KGameManager(KDrawManager drawManager) 
        {
            DrawManager = drawManager;
            MapManager = new(this);
        }

        public void Update(in uint currentUpdate)
        {
            Tests.Update();
            //if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MainMenu.Update();
        }

        public void FrameUpdate(KDrawManager drawManager, in uint currentUpdate, in uint currentFrame, in double deltaTime)
        {
            Tests.FrameUpdate(drawManager);
            //if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MapManager.FrameUpdate(currentUpdate, currentFrame, drawManager);
        }
    }
}

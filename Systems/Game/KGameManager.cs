using Elements.Systems.UI;

namespace Elements.Systems.Game
{
    [Flags]
    public enum KGameState
    {
        PAUSED
    }

    public class KGameManager
    {
        public KGameState GameState;
        //public KMainMenu MainMenu;

        public IKEntityHandler EntityHandler;

        public KGameManager() 
        {
            //MainMenu = new();
        }

        public void Update(in uint currentUpdate)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MainMenu.Update();
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KWindowManager window)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;

            //MainMenu.FrameUpdate(window);
        }
    }
}

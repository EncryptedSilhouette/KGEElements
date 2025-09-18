namespace Elements
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

        //public IKEntityHandler EntityHandler;

        public KGameManager() 
        {
            //MainMenu = new();
        }

        public void Init(KDrawManager drawManager)
        {
            drawManager.CreateDrawLayer(256);
        }

        public void Update(in uint currentUpdate)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;
            //MainMenu.Update();
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KDrawManager drawManager)
        {
            if (!GameState.HasFlag(KGameState.PAUSED)) return;

            //MainMenu.FrameUpdate(window);
        }
    }
}

using Elements.Core;
using Elements.Systems.UI;

namespace Elements.Systems
{
    public enum KGameState
    {
        TITLE,
    }

    public class KGameManager
    {
        public KMainMenu MainMenu;

        public IKEntityHandler EntityHandler;

        public KGameManager() 
        {
            MainMenu = new();
        }

        public void Update(in uint currentUpdate)
        {
            MainMenu.Update();
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame, KWindowManager window)
        {
            MainMenu.FrameUpdate(window);
        }
    }
}

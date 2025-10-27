using Elements.Drawing;

namespace Elements.Game
{
    [Flags]
    public enum KGameFlags
    {
        DEBUG = 1 << 0,
    }

    public class KGameManager
    {
        public KMainMenu MainMenu;

        public KGameManager()
        {
            MainMenu = new();
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            MainMenu.FrameUpdate(renderManager);
        }
    }
}

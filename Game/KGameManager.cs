using Elements.Rendering;

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
            //MainMenu.FrameUpdate(renderManager);

            renderManager.TextRenderers[0].SubmitDraw("Demo Game Menu", 200, 200);


            renderManager.TextRenderers[0].SubmitDraw(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\r\n", 1009, 789);

        }
    }
}

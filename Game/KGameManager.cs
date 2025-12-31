using Elements.Rendering;
using SFML.Graphics;

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

        }

        public void CreateNewGame()
        {
            //Create game file

            Texture texture = new Texture(800, 600);

            KGame game = new KGame();

            //Save game file
        }
    }
}

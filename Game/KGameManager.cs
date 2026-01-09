using Elements.Core;
using Elements.Game.Map;
using Elements.Rendering;

namespace Elements.Game
{
    [Flags]
    public enum KGameFlags
    {
        DEBUG = 1 << 0,
    }
    [Flags]
    public enum KGameStates
    {
        DEBUG = 1 << 0,
        GAME = 1 << 1,
    }

    public class KGameManager
    {
        public KGameStates GameStates;
        public KGameMap GameMap;
        public KInputManager InputManager;
        public KCameraCrontroller CameraCrontroller;

        public KButton Button;

        public KGameManager(KRenderManager renderer, KInputManager inputManager)
        {
            InputManager = inputManager;
            GameMap = new KGameMap(0, 0, 32, 32);
            CameraCrontroller = new KCameraCrontroller(renderer.Window.GetView());

            Button = new(50,50,64,64,"Button");
            Button.OnPressed += () => Console.WriteLine("Press");
            Button.OnHold += () => Console.WriteLine("Hold");
            Button.OnReleased += () => Console.WriteLine("Release");

        }

        public void Init()
        {
            GameMap.Init(ResourceManager.TextureAtlases["atlas"], 0, 0, 30, 30, 5);
        }

        public void Update(in uint currentUpdate)
        {
            GameUpdate(currentUpdate);
            Button.Update(InputManager.MousePosX, InputManager.MousePosY);
            CameraCrontroller.Update();
            GameMap.Update();
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            CameraCrontroller.FrameUpdate(InputManager, renderer);
            GameMap.FrameUpdate(renderer);
            Button.FrameUpdate(renderer);
        }

        public void GameUpdate(in uint currentUpdate)
        {
            
        }

        public void ButtonPressed()
        {
            
        }
    }
}

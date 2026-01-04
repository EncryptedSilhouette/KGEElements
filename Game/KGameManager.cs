using Elements.Game.Map;
using Elements.Rendering;
using SFML.Graphics;
using SFML.Window;

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
        public KResourceManager ResourceManager;
        public KInputManager InputManager;
        public KCameraCrontroller CameraCrontroller;

        public KGameManager(KResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
            GameMap = new KGameMap(0, 0, 32, 32);
        }

        public void Init()
        {
            GameMap.Init(ResourceManager.TextureAtlases["atlas"], 0, 0, 40, 40, 10);
        }

        public void Update(in uint currentUpdate)
        {

            GameUpdate(currentUpdate);
            GameMap.Update();
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            GameMap.FrameUpdate(renderer);
        }

        public void GameUpdate(in uint currentUpdate)
        {
            if (InputManager.MousePosX < 16) CameraCrontroller.View.Move((-1, 0));
            if (InputManager.MousePosX > 16) CameraCrontroller.View.Move((1, 0));

            if (InputManager.MousePosX < 16) CameraCrontroller.View.Move((-1, 0));
            if (InputManager.MousePosX < 16) CameraCrontroller.View.Move((-1, 0));
        }
    }
}

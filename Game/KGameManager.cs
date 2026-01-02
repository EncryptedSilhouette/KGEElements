using Elements.Game.Map;
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
        public KGameMap GameMap;
        public KResourceManager ResourceManager;

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
            GameMap.Update();
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            GameMap.FrameUpdate(renderer);
        }
    }
}

using Elements.Game.Units;

namespace Elements.Game
{
    public class KPlayer
    {
        public KPlayerUnit Unit;
        public KGameScene CurrentScene;
        public KUnitController UnitController;

        public KPlayer(KGameScene currentScene)
        {
            CurrentScene = currentScene;
        }
    }
}

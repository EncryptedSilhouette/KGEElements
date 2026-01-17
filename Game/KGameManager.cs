using Elements.Core;
using Elements.Game.Map;
using Elements.Game.Units;
using Elements.Rendering;
using SFML.System;

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
        private int _unitCount;

        public KGameStates GameStates;
        public KGameMap GameMap;
        public KInputManager InputManager;
        public KCameraCrontroller CameraCrontroller;
        public KButton ResetWindowButton;

        public KUnit[] Units;

        public KGameManager(KRenderManager renderer, KInputManager inputManager)
        {
            InputManager = inputManager;
            GameMap = new KGameMap(0, 0, 32, 32);
            CameraCrontroller = new KCameraCrontroller(renderer.Window.DefaultView);

            ResetWindowButton = new(4,4,64,64,"Reset Window");
            ResetWindowButton.OnPressed += CameraCrontroller.ResetCamera; //ResetCamera;

            _unitCount = 0;
            Units = [];
        }

        public void Init()
        {
            GameMap.Init(KProgram.TextureAtlases[0], 0, 0, 30, 30, 5);
            Units = new KUnit[10];
        }

        public void Update(in uint currentUpdate)
        {
            ResetWindowButton.Update(InputManager, InputManager.MousePosX, InputManager.MousePosY);
            CameraCrontroller.Update();
            GameMap.Update();
        }

        bool select = false;
        Vector2f PointA;
        Vector2f PointB;
        public void FrameUpdate(KRenderManager renderer)
        {
            CameraCrontroller.FrameUpdate(InputManager, renderer);
            GameMap.FrameUpdate(renderer);
            ResetWindowButton.FrameUpdate(renderer);

            if (InputManager.IsMousePressed(KMouseStates.Mouse_1) && !select)
            {
                select = true;
                PointA = (InputManager.MousePosX, InputManager.MousePosY);
            }
            else if (InputManager.IsMouseReleased(KMouseStates.Mouse_1) && select)
            {
                select = false;
            }
            if (select)
            {
                PointB = (InputManager.MousePosX, InputManager.MousePosY);
                renderer.DrawRectToScreen(PointA, PointB, new(0, 0, 200, 100));
            }

            for (int i = 0; i < _unitCount; i++)
            {
                renderer.DrawRect(Units[i].DrawData, Units[i].Bounds);
            }
        }
    }
}

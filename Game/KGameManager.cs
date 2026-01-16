using Elements.Core;
using Elements.Game.Map;
using Elements.Game.Units;
using Elements.Rendering;
using SFML.Graphics;
using SFML.Graphics.Glsl;
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
        public KButton Button;
        public KUnit[] Units;

        public KGameManager(KRenderManager renderer, KInputManager inputManager)
        {
            InputManager = inputManager;
            GameMap = new KGameMap(0, 0, 32, 32);
            CameraCrontroller = new KCameraCrontroller(renderer.Window.GetView());

            Button = new(50,50,64,64,"Button");
            Button.OnPressed += SpawnUnit;

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
            Button.Update(InputManager.MousePosX, InputManager.MousePosY);
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
            Button.FrameUpdate(renderer);

            if (InputManager.IsMousePressed(KMouseStates.Mouse_1) && !select)
            {
                select = true;
                PointA = (InputManager.MousePosX, InputManager.MousePosY);
                Console.WriteLine("start selection");
            }
            else if (InputManager.IsMouseReleased(KMouseStates.Mouse_1) && select)
            {
                select = false;
                Console.WriteLine("end selection");
            }
            if (select)
            {
                PointB = (InputManager.MousePosX, InputManager.MousePosY);
                renderer.DrawRect(PointA, PointB, new(0, 0, 200, 100));

            }

            for (int i = 0; i < _unitCount; i++)
            {
                renderer.DrawRect(Units[i].DrawData, Units[i].Bounds);
            }
        }

        public void SpawnUnit()
        {
            if (_unitCount >= Units.Length) return;

            Units[_unitCount] = new KUnit
            {
                Bounds = new(16, 16, new(0, 0)),
                DrawData = new()
                {
                    Color = Color.White,
                    Sprite = new()
                }
            };
            _unitCount++;
        }
    }
}

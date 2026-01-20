using Elements.Core;
using Elements.Game.Map;
using Elements.Game.Units;
using Elements.Rendering;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Game
{
    public class KGameManager
    {
        public record struct KSelectionBox(Vector2f A, Vector2f B, bool Selected);
        
        private int _unitCount;

        public KGameMap GameMap;
        public KButton ResetWindowButton;
        public KSelectionBox SelectionBox;
        public KCameraCrontroller CameraCrontroller;
        public KInputManager InputManager;
        public KCommandManager CommandManager;
        public List<int> SelectedUnits;
        public KUnit[] Units;

        public static void SelectGroup(Vector2f a, Vector2f b)
        {
            var gameManager = KProgram.GameManager;

            for (int i = 0; i < gameManager.Units.Length; i++)
            {
                ref var unit = ref gameManager.Units[i];
                
                if (KProgram.CheckRectPointCollision(
                    new FloatRect(a, b - a), 
                    unit.Bounds.Transform.Position))
                {
                    
                }
            }
        }

        public KGameManager(KRenderManager renderer, KInputManager inputManager)
        {
            InputManager = inputManager;
            SelectedUnits = [];

            SelectionBox = new();

            GameMap = new KGameMap(0, 0, 32, 32);
            CameraCrontroller = new KCameraCrontroller(renderer.Window.DefaultView);

            ResetWindowButton = new(4, 4, 64, 64,"Reset Window");
            ResetWindowButton.OnPressed += CameraCrontroller.ResetCamera; //ResetCamera;

            CommandManager = new KCommandManager();

            _unitCount = 0;
            Units = new KUnit[100];

            for (int i = 0; i < 10; i++, _unitCount++)
            {
                Units[i] = new KUnit
                {
                    DrawData = new KDrawData
                    {
                        Color = Color.Red,
                    },
                    Bounds = new(32, 32, new KTransform(KProgram.RNG.Next(600), KProgram.RNG.Next(600)))
                };   
            }
        }

        public void Init()
        {
            GameMap.Init(KProgram.TextureAtlases[0], 0, 0, 30, 30, 5);
            //Units = new KUnit[10];
        }

        public void Update(in uint currentUpdate)
        {
            ResetWindowButton.Update(InputManager, InputManager.MousePosX, InputManager.MousePosY);
            CameraCrontroller.Update();
            GameMap.Update();

        }

        public void FrameUpdate(KRenderManager renderer)
        {
            CameraCrontroller.FrameUpdate(InputManager, renderer);
            GameMap.FrameUpdate(renderer);
            ResetWindowButton.FrameUpdate(renderer);
            
            //Select
            if (InputManager.IsMouseDown(KMouseStates.Mouse_1))
            {
                if (!SelectionBox.Selected)
                {
                    SelectionBox.Selected = true;
                    SelectionBox.A = (InputManager.MousePosX, InputManager.MousePosY);
                }

                SelectionBox.B = (InputManager.MousePosX, InputManager.MousePosY);
                renderer.DrawRectToScreen(SelectionBox.A, SelectionBox.B, new(0, 0, 200, 100));
            }
            else if (SelectionBox.Selected)
            {
                SelectionBox.Selected = false;
                SelectedUnits = new(Units.Length);

                for (int i = 0; i < Units.Length; i++)
                {
                    ref var unit = ref Units[i];
                    
                    if (KProgram.CheckRectPointCollision(
                        new FloatRect(SelectionBox.A, SelectionBox.B - SelectionBox.A), 
                        unit.Bounds.Transform.Position))
                    {
                        SelectedUnits.Add(i);
                        Console.WriteLine($"Selected Unit {i}");
                    }
                }
            }

            for (int i = 0; i < _unitCount; i++)
            {
                renderer.DrawRect(Units[i].DrawData, Units[i].Bounds);
            }
        }
    }
}

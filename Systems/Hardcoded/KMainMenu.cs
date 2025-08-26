using Elements.Core;

namespace Elements.Systems.UI
{
    public class KMainMenu
    {
        public const int BUTTON_EXIT = 0;

        private KRectangle bounds;
        private KDrawData drawData;
        public KButton[] buttons;

        public KMainMenu()
        {
            drawData = new()
            {
                Layer = 0,
            };
            buttons = 
            [
                new() 
                {
                    Bounds = new() 
                    {
                        Width = 32,
                        Height = 32,
                        Transform = new() 
                        {
                            PosX = 8,
                            PosY = 8,
                        }
                    },
                },
                new() 
                {
                    Bounds = new()
                    {
                        Width = 32,
                        Height = 32,
                        Transform = new()
                        {
                            PosX = 8,
                            PosY = 48,
                        }
                    },
                },
                new() 
                {
                    Bounds = new()
                    {
                        Width = 32,
                        Height = 32,
                        Transform = new()
                        {
                            PosX = 8,
                            PosY = 88,
                        }
                    },
                }
            ];
        }

        public void Init()
        {
            buttons[BUTTON_EXIT].OnPressed += Exit;
        }

        public void Update()
        {
            var mousePos = (KProgram.InputManager.MousePosX, KProgram.InputManager.MousePosY);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Update(mousePos.MousePosX, mousePos.MousePosY);
            }
        }

        public void FrameUpdate(KWindowManager mainMenu)
        {
            mainMenu.SubmitDraw(drawData, bounds);
            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].FrameUpdate(mainMenu);
            }
        }

        public void Exit()
        {
            KProgram.Running = false;
        }
    }
}

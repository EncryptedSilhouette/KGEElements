using Elements.Core;
using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Hardcoded
{
    public class KMainMenu
    {
        public const int BUTTON_EXIT = 0;

        private KRectangle bounds;
        private KDrawData drawData;
        private KButton[] buttons;

        public KMainMenu()
        {
            bounds = new()
            {
                Width = 128,
                Height = 128,
                Transform = new()
                {
                    PosX = 0,
                    PosY = 0,
                },
            };
            drawData = new()
            {
                Order = 0,
                Color = Color.Cyan,
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
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Update(KProgram.InputManager.MousePosX, KProgram.InputManager.MousePosY);
            }
        }

        public void FrameUpdate(KDrawManager drawManager)
        {
            drawManager.SubmitDraw(drawData, bounds);
            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].FrameUpdate(drawManager);
            }
        }

        public void Exit()
        {
            KProgram.Running = false;
        }
    }
}

using Elements.Core;
using Elements.Drawing;

namespace Elements.Game.Hardcoded
{
    public class KMainMenu
    {
        public const int BUTTON_EXIT = 0;

        private KRectangle bounds;
        private KDrawData drawData;
        private KButton[] buttons;

        public KMainMenu()
        {
            
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

        public void FrameUpdate(KRenderManager renderManager)
        {
            renderManager.SubmitDraw(drawData, bounds);
            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].FrameUpdate(renderManager);
            }
        }

        public void Exit()
        {
            KProgram.Running = false;
        }
    }
}

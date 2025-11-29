using Elements.Core;
using Elements.Rendering;
using System.Text;

namespace Elements.Game.UI.ECS
{
    public class KTextInputField
    {
        private bool _focused = false;
        private StringBuilder _inputBuffer = new();

        public KButton Button;

        public KTextInputField(int x, int y, int width, int height, string text)
        {
            Button = new KButton(x, y, width, height, text);
        }

        public void Update(in float mPosX, in float mPosY)
        {
            if (_focused)
            {
                if (KProgram.InputManager.TextBufferLength > 0) 
                {
                    _inputBuffer.Append(KProgram.InputManager.ReadTextBuffer());
                }
            }
            Button.Update(mPosX, mPosY);
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            
            //Button.FrameUpdate(mPosX, mPosY);
        }
    }
}

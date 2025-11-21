using Elements.Core;
using Elements.Rendering;
using System.Text;

namespace Elements.Game.UI
{
    public class KTextInput
    {
        public bool Active = false;
        public KText ButtonText;
        public KButton Button;
        public StringBuilder TextBuffer = new();

        public KTextInput(float x, float y, float width, float height)
        {
            ButtonText = new(string.Empty);
            Button = new KButton();
        }

        public KTextInput(string prompt, float x, float y, float width, float height)
        {
            ButtonText = new(prompt);
            Button = new KButton();
            Button.OnPressed += () =>
            {

            };
        }

        public void Update()
        {
            int x = KProgram.InputManager.MousePosX;
            int y = KProgram.InputManager.MousePosY;
            Button.Update(x, y);
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            Button.FrameUpdate(renderManager);
        }
    }
}

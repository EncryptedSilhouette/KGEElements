using Elements.Core;
using Elements.Rendering;
using System.Text;

namespace Elements.Game.UI
{
    public struct KTextInput
    {
        public bool CanGrow = false;
        public bool DisplayLast = true;
        public bool Active = false;
        public int maxLength = 256;
        public KButton Button;
        public StringBuilder TextBuffer = new();

        public Action? OnEnter;

        public KRectangle Bounds
        {
            get => Button.Bounds;
            set => Button.Bounds = value;
        }

        public KTransform Transform
        {
            get => Button.Bounds.Transform; 
            set => Button.Bounds.Transform = value;
        }

        public KText Text
        {
            get => Button.TextBox;
            set => Button.TextBox = value;
        }

        public KTextInput(float x, float y, float width, float height)
        {
            Button = new KButton(x, y, width, height, string.Empty);
        }

        public KTextInput(float x, float y, float width, float height, string prompt)
        {
            Button = new KButton(x, y, width, height, prompt);
        }

        public void Init()
        {
            Button.OnPressed += Pressed;
        }

        public void Deinit()
        {
            Button.OnPressed -= Pressed;
        }

        public void Update(KInputManager inputManager)
        {
            Button.Update(inputManager.MousePosX, inputManager.MousePosY);
                
            if (Active)
            {
                if (!KCollision.CheckRectPointCollision(Bounds, inputManager.MousePosX, inputManager.MousePosY))
                {
                    Active = false;
                    return;
                }

                if (inputManager.BufferCharCount > 0)
                {
                    TextBuffer.Append(inputManager.ReadTextBuffer());
                    Button.TextBox.Text = TextBuffer.ToString();
                }
                //OnEnter?.Invoke();
            }
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            Button.FrameUpdate(renderManager);
        }

        private void Pressed()
        {
            Active = false;
            KProgram.LogManager.DebugLog("entered input field");
        }
    }
}

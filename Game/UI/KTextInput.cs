using Elements.Core;
using Elements.Rendering;
using System.Text;

namespace Elements.Game.UI
{
    //public struct KTextInput
    //{
    //    public bool CanGrow = false;
    //    public bool DisplayLast = true;
    //    public bool Active = false;
    //    public int maxLength = 256;
    //    public KButton ResetWindowButton;
    //    public StringBuilder TextBuffer = new();

    //    public Action? OnEnter;

    //    public KRectangle Bounds
    //    {
    //        get => ResetWindowButton.Bounds;
    //        set => ResetWindowButton.Bounds = value;
    //    }

    //    public KTransform Transform
    //    {
    //        get => ResetWindowButton.Bounds.Transform; 
    //        set => ResetWindowButton.Bounds.Transform = value;
    //    }

    //    public KText Text
    //    {
    //        get => ResetWindowButton.TextBox;
    //        set => ResetWindowButton.TextBox = value;
    //    }

    //    public KTextInput(float x, float y, float width, float height)
    //    {
    //        ResetWindowButton = new KButton(x, y, width, height, string.Empty);
    //    }

    //    public KTextInput(float x, float y, float width, float height, string prompt)
    //    {
    //        ResetWindowButton = new KButton(x, y, width, height, prompt);
    //    }

    //    public void Init()
    //    {
    //        ResetWindowButton.OnPressed += Pressed;
    //    }

    //    public void Deinit()
    //    {
    //        ResetWindowButton.OnPressed -= Pressed;
    //    }

    //    public void Update(KInputManager inputManager)
    //    {
    //        ResetWindowButton.Update(inputManager.MousePosX, inputManager.MousePosY);
                
    //        if (Active)
    //        {
    //            if (!KProgram.CheckRectPointCollision(Bounds, inputManager.MousePosX, inputManager.MousePosY))
    //            {
    //                Active = false;
    //                return;
    //            }

    //            if (inputManager.BufferCharCount > 0)
    //            {
    //                TextBuffer.Append(inputManager.ReadTextBuffer());
    //                ResetWindowButton.TextBox.Text = TextBuffer.ToString();
    //            }
    //            //OnEnter?.Invoke();
    //        }
    //    }

    //    public void FrameUpdate(KRenderManager renderManager)
    //    {
    //        ResetWindowButton.FrameUpdate(renderManager);
    //    }

    //    private void Pressed()
    //    {
    //        Active = false;
    //        KProgram.LogManager.DebugLog("entered input field");
    //    }
    //}
}

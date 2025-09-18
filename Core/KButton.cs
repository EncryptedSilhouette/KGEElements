using Elements.Drawing;
using SFML.Graphics;

namespace Elements.Core
{
    public struct KButton
    {
        private bool _isDown;

        public Color HeldColor;
        public KRectangle Bounds;
        public KDrawData DrawData;

        public Action? OnHover;
        public Action? OnPressed;
        public Action? OnHold;
        public Action? OnReleased;

        public KButton() 
        {     
            _isDown = false;
        }

        public void Update(in float mPosX, in float mPosY)
        {
            if (KCollision.CheckRectPointCollision(Bounds, mPosX, mPosY))
            {
                DrawData.Color = HeldColor;
                OnHover?.Invoke();
            }
            else if (_isDown) //Not in bounds but m1 still held, release button.
            {
                _isDown = false;
                OnReleased?.Invoke();
                return;
            }
            else return; 

            if (KProgram.InputManager.IsMouseReleased(KMouseStates.Mouse_1))
            {
                _isDown = false;
                OnReleased?.Invoke();
                return;
            }
            if (KProgram.InputManager.IsMousePressed(KMouseStates.Mouse_1))
            {
                _isDown = true;
                OnPressed?.Invoke();
            }
            //Can only reach if in bounds and m1 held.
            OnHold?.Invoke();
        }

        public void FrameUpdate(KDrawManager drawManager)
        {
            drawManager.SubmitDraw(DrawData, Bounds);
        }
    }
}

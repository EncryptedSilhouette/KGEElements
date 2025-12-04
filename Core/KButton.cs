using Elements.Rendering;
using SFML.Graphics;

namespace Elements.Core
{
    public struct KButton
    {
        private bool _isDown;

        public Color HeldColor;
        public KRectangle Bounds;
        public KDrawData DrawData;
        public KText TextBox;

        public Action? OnHover;
        public Action? OnPressed;
        public Action? OnHold;
        public Action? OnReleased;

        public KButton(float x, float y, float width, float height, string text)
        {
            HeldColor = Color.White;
            DrawData = new();
            TextBox = new(text);
            _isDown = false;

            Bounds = new()
            {
                Width = width,
                Height = height,
                Transform = new()
                {
                    PosX = x,
                    PosY = y,
                }
            };
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

        public void FrameUpdate(KRenderManager renderManager)
        {
            if (TextBox.Text != string.Empty)
            {
                renderManager.TextRenderers[0].SubmitDraw(TextBox, Bounds.TopLeft.X, Bounds.TopLeft.Y, out FloatRect bounds);
                Console.WriteLine(Bounds.TopLeft.X);
            }
            renderManager.SubmitDraw(DrawData, Bounds);
        }
    }
}

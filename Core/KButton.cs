using Elements.Rendering;
using SFML.Graphics;
using System.Buffers;

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
            KText text = new(TextBox.Text);

            Vertex[] buffer = ArrayPool<Vertex>.Shared.Rent(text.Text.Length * 4);

            var bounds = KRenderManager.CreateTextbox(text, KProgram.Fonts[0], buffer, 50, 50, KProgram.FontSize);
            renderManager.DrawRect(bounds, Color.Blue, layer: 1);
            //renderManager.SubmitDrawText(text, KProgram.Fonts[0], 50, 50, KProgram.FontSize, Color.White, layer: 1);

            ArrayPool<Vertex>.Shared.Return(buffer);
        }
    }
}

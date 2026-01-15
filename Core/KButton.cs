using Elements.Rendering;
using SFML.Graphics;

namespace Elements.Core
{
    public struct KButton
    {
        private bool _isDown = false;

        public Color Color;
        public Color HeldColor;
        public Color DownColor;
        public FloatRect Bounds;
        public KDrawData DrawData;
        public KText TextBox;

        public Action? OnHover;
        public Action? OnPressed;
        public Action? OnHold;
        public Action? OnReleased;
        public Action? OnExit;

        public KButton(float x, float y, float width, float height, string text)
        {
            Color = new(200, 200, 200);
            HeldColor = new(150, 150, 150);
            DownColor = new(100, 100, 100);
            DrawData = new();
            TextBox = new(text);

            Bounds = new(x, y, width, height);
        }

        public void Update(in float mPosX, in float mPosY)
        {
            if (KProgram.CheckRectPointCollision(Bounds, mPosX, mPosY))
            {
                OnHover?.Invoke();
                DrawData.Color = HeldColor;

                if (KProgram.InputManager.IsMouseDown(KMouseStates.Mouse_1))
                {
                    if (!_isDown)
                    {
                        _isDown = true;
                        OnPressed?.Invoke();
                    }
                    OnHold?.Invoke();
                }
                else if (_isDown)
                {
                    _isDown = false;
                    OnReleased?.Invoke();
                }
            }
            else if (_isDown)
            {
                _isDown = false;
                DrawData.Color = Color;
                OnReleased?.Invoke();
                OnExit?.Invoke();
            }
            else
            {
                DrawData.Color = Color;
            }
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            renderManager.DrawRect(Bounds, DrawData.Color, layer: 1);
            renderManager.DrawText(TextBox, 50, 50, wrapThreshold: (int)Bounds.Width, layer: 1);
        }
    }
}

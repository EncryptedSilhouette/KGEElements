using Elements.Rendering;
using SFML.Graphics;
using SFML.System;

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
        //public KText TextBox;

        public Action? OnHover;
        public Action? OnPressed;
        public Action? OnHold;
        public Action? OnReleased;
        public Action? OnExit;

        public KButton(float x, float y, float width, float height, string text)
        {
            Color = new(200, 200, 200);
            HeldColor = new(125, 125, 125);
            DownColor = new(100, 100, 100);
            DrawData = new();
            //TextBox = new(text, new Vertex[text.Length * 6]);

            Bounds = new((x, y), (width, height));
        }

        public KButton(Vector2f position, Vector2f size, string text) : 
            this(position.X, position.Y, size.X, size.Y, text) { }

        public void Update(KInputManager inputManager, in float mPosX, in float mPosY)
        {
            if (KProgram.CheckRectPointCollision(Bounds, mPosX, mPosY))
            {
                OnHover?.Invoke();
                DrawData.Color = HeldColor;

                if (inputManager.IsMouseDown(KMouseStates.Mouse_1))
                {
                    if (!_isDown && !inputManager.PreviousMouseStates.HasFlag(KMouseStates.Mouse_1))
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
            //renderManager.DrawRect(Bounds, DrawData.Color, layer: 1);
            //renderManager.DrawText(TextBox, Bounds.Position.X, Bounds.Position.Y, wrapThreshold: (int)Bounds.Width, layer: 1);
        }
    }
}

#if false
x -= IMG_SIZE / 2; 
y -= IMG_SIZE / 2; 
center->x -= (x * new_zoom) - (x * old_zoom); 
center->y -= (y * new_zoom) - (y * old_zoom);
#endif
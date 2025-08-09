using Elements.Core.Systems;
using SFML.Graphics;

namespace Elements.Core.UI
{
    public struct KButton
    {
        private bool _isDown;
        private bool _isHovering;

        public Color HeldColor;
        public KRectangle Transform;
        public KDrawData TextureData;

        public Action? OnHover;
        public Action? OnPressed;
        public Action? OnHold;
        public Action? OnReleased;

        public void Update(in float mousePosX, in float mousePosY)
        {
            _isHovering = KCollision.CheckRectPointCollision(Transform, mousePosX, mousePosY);

            if (!_isHovering)
            {
                if (_isDown)
                {
                    _isDown = false;
                    OnReleased?.Invoke();
                }
                return;
            }

            OnHover?.Invoke();

            //if (KProgram.InputManager.CheckMouseState(KMouseStates.M1_PRESSED))
            //{
            //    if (!_isDown)
            //    {
            //        OnPressed?.Invoke();
            //        _isDown = true;
            //    }
            //    if (_isDown) OnHold?.Invoke();
            //}
            //else if (!KProgram.InputManager.CheckMouseState(KMouseStates.M1_PRESSED))
            //{
            //    _isDown = false;
            //    OnReleased?.Invoke();
            //}
        }

        public void FrameUpdate()
        {

        }
    }
}

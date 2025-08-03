using Elements.Core.UI;
using SFML.Window;

namespace Elements.Core.Systems
{
    [Flags]
    public enum KMouseStates
    {
        M1_PRESSED = 1 << 0,
        M2_PRESSED = 1 << 1,
        M3_PRESSED = 1 << 2,
        M4_PRESSED = 1 << 3,
        M5_PRESSED = 1 << 4,
    }

    public struct KInputManager
    {
        #region Static 
     
        private static void UpdateMousePosition(object? ignored, MouseMoveEventArgs e) =>
            (KProgram.InputManager.MousePosX, KProgram.InputManager.MousePosY) = (e.X, e.Y);

        private static void RegisterButtonPress(object? ignored, MouseButtonEventArgs e) =>
            KProgram.InputManager.MouseStates |= (KMouseStates) e.Button;

        private static void RegisterButtonRelease(object? ignored, MouseButtonEventArgs e) =>
            KProgram.InputManager.MouseStates |= ~(KMouseStates) e.Button;

        #endregion

        private bool _menuOpenend;
        private KButton _buttonMainMenu;
        private KButton _buttonReturn;
        private KButton _buttonOptions;
        private KButton _buttonExit;

        public int MousePosX;
        public int MousePosY;
        public KMouseStates MouseStates;
        public KMouseStates PreviousMouseStates;

        public KInputManager()
        {
            _menuOpenend = true;
            MousePosX = MousePosY = 0;
            MouseStates = PreviousMouseStates = 0;
        }

        public void Init(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved += UpdateMousePosition;
            windowManager.Window.MouseButtonPressed += RegisterButtonPress;
            windowManager.Window.MouseButtonReleased += RegisterButtonRelease;
        }

        public void Deinit(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved -= UpdateMousePosition;
            windowManager.Window.MouseButtonPressed -= RegisterButtonPress;
            windowManager.Window.MouseButtonReleased -= RegisterButtonRelease;
        }

        public void Update()
        {
            _buttonMainMenu.Update(MousePosX, MousePosY);

            if (_menuOpenend)
            {
                _buttonReturn.Update(MousePosX, MousePosY);
                _buttonOptions.Update(MousePosX, MousePosY);
                _buttonExit.Update(MousePosX, MousePosY);
            }
        }

        public void FrameUpdate()
        {
            if (_menuOpenend)
            {

            }

            PreviousMouseStates = MouseStates;
        }

        public bool CheckMouseState(KMouseStates mouseState) => (MouseStates & mouseState) == mouseState;
    }
}

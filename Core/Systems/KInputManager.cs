using System.Text;
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
        SCROLL_UP = 1 << 5,
        SCROLL_DOWN = 1 << 6,
    }

    public struct KInputManager
    {
        #region Static 
     
        private static void UpdateMousePosition(object? ignored, MouseMoveEventArgs e) =>
            (KProgram.InputManager.MousePosX, KProgram.InputManager.MousePosY) = (e.X, e.Y);

        private static void UpdateScrollDelta(object? ignored, MouseWheelScrollEventArgs e)
        {
            KProgram.InputManager.ScrollDelta += e.Delta;
            if (e.Delta == 0)       KProgram.InputManager.MouseStates |= ~KMouseStates.SCROLL_UP | ~KMouseStates.SCROLL_DOWN;
            else if (e.Delta > 0)   KProgram.InputManager.MouseStates |= KMouseStates.SCROLL_UP | ~KMouseStates.SCROLL_DOWN;
            else                    KProgram.InputManager.MouseStates |= ~KMouseStates.SCROLL_UP | KMouseStates.SCROLL_DOWN;
        }

        private static void RegisterButtonPress(object? ignored, MouseButtonEventArgs e) =>
            KProgram.InputManager.MouseStates |= (KMouseStates) e.Button;

        private static void RegisterButtonRelease(object? ignored, MouseButtonEventArgs e) =>
            KProgram.InputManager.MouseStates |= ~(KMouseStates) e.Button;

        private static void UpdateTextBuffer(object? ignored, TextEventArgs e) =>
            KProgram.InputManager._stringBuilder.Append(e.Unicode);

        #endregion

        private bool _menuOpenend;
        private KButton _buttonMainMenu;
        private KButton _buttonReturn;
        private KButton _buttonOptions;
        private KButton _buttonExit;
        private StringBuilder _stringBuilder;

        public int MousePosX;
        public int MousePosY;
        public float ScrollDelta;
        public KMouseStates MouseStates;
        public KMouseStates PreviousMouseStates;

        public KInputManager()
        {
            _menuOpenend = true;
            _stringBuilder = new(128);
            MousePosX = MousePosY = 0;
            MouseStates = PreviousMouseStates = 0;
        }

        public void Init(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved += UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled += UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed += RegisterButtonPress;
            windowManager.Window.MouseButtonReleased += RegisterButtonRelease;
            windowManager.Window.TextEntered += UpdateTextBuffer;
        }

        public void Deinit(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved -= UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled -= UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed -= RegisterButtonPress;
            windowManager.Window.MouseButtonReleased -= RegisterButtonRelease;
            windowManager.Window.TextEntered -= UpdateTextBuffer;
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

            _stringBuilder.Clear();
            PreviousMouseStates = MouseStates;
        }

        public bool CheckMouseState(KMouseStates mouseState) => (MouseStates & mouseState) == mouseState;

        public string ReadTextBuffer() => _stringBuilder.ToString();
    }
}

using System.Text;
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

        M1_PRESSED_LAST_FRAME = 1 << 5,
        M2_PRESSED_LAST_FRAME = 1 << 6,
        M3_PRESSED_LAST_FRAME = 1 << 7,
        M4_PRESSED_LAST_FRAME = 1 << 8,
        M5_PRESSED_LAST_FRAME = 1 << 9,

        SCROLL_UP = 1 << 10,
        SCROLL_DOWN = 1 << 11,

        PRESSED_ONLY_FILTER =
            M1_PRESSED |
            M2_PRESSED |
            M3_PRESSED |
            M4_PRESSED |
            M5_PRESSED
    }

    [Flags]
    public enum KKeyboardStates
    {
        //State
        PRESSED = 1 << 0,
        PRESSED_LAST_FRAME = 1 << 1,

        //ModifiedState
        SHIFT = 1 << 2,
        CONTROL = 1 << 3,
        ALTERNATE = 1 << 4,
        SYSTEM = 1 << 5,
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

        private static void RegisterMouseButtonPress(object? ignored, MouseButtonEventArgs e) =>
            KProgram.InputManager.MouseStates |= (KMouseStates)(1 << (int)e.Button); //Converts Button enum to bitflag.

        ////Converts Button enum to bitflag.
        //private static void RegisterMouseButtonRelease(object? ignored, MouseButtonEventArgs e) =>
        //    KProgram.InputManager.MouseStates &= ~(KMouseStates)(1 << (int)e.Button); //Converts Button enum to bitflag.

        private static void UpdateTextBuffer(object? ignored, TextEventArgs e) =>
            KProgram.InputManager._stringBuilder.Append(e.Unicode);

        private static void RegisterKeyPress(object? ignored, KeyEventArgs e)
        {
            
        }

        private static void RegisterKeyRelease(object? ignored, KeyEventArgs e)
        {

        }


        #endregion
       
        private KKeyboardStates[] _keyboardStates;
        private StringBuilder _stringBuilder;

        public int MousePosX;
        public int MousePosY;
        public float ScrollDelta;
        public KMouseStates MouseStates;

        public KInputManager()
        {
            MousePosX = MousePosY = 0;
            _stringBuilder = new(128);
            _keyboardStates = new KKeyboardStates[(int) Keyboard.Key.KeyCount];
            
            for (int i = 0; i < _keyboardStates.Length; i++) _keyboardStates[i] = 0;
        }

        public void Init(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved += UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled += UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed += RegisterMouseButtonPress;
            //windowManager.Window.MouseButtonReleased += RegisterMouseButtonRelease;
            windowManager.Window.TextEntered += UpdateTextBuffer;
            windowManager.Window.KeyPressed += RegisterKeyPress;
            windowManager.Window.KeyReleased += RegisterKeyRelease;
        }

        public void Deinit(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved -= UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled -= UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed -= RegisterMouseButtonPress;
            //windowManager.Window.MouseButtonReleased -= RegisterMouseButtonRelease;
            windowManager.Window.TextEntered -= UpdateTextBuffer;
            windowManager.Window.KeyPressed -= RegisterKeyPress;
            windowManager.Window.KeyReleased -= RegisterKeyRelease;
        }

        public void Update()
        {
         
        }

        public void FrameUpdate()
        {
            _stringBuilder.Clear();
            PreservelastInputStates();
        }

        public bool CheckMouseState(KMouseStates mouseState) => (MouseStates & mouseState) == mouseState;

        public bool CheckKeyState(Keyboard.Key key, KKeyboardStates keyState)
        {
            if (key > Keyboard.Key.KeyCount || key < 0) return false;
            return _keyboardStates[(int) key].HasFlag(keyState);
        }

        public string ReadTextBuffer() => _stringBuilder.ToString();

        private void PreservelastInputStates()
        {
            var previousStates = (int)(MouseStates & KMouseStates.PRESSED_ONLY_FILTER) << 5;
            MouseStates = (KMouseStates)previousStates;
        }
    }
}

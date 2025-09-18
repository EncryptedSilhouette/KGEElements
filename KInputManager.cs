using SFML.Graphics;
using SFML.Window;
using System.Text;

//FINALIZED
namespace Elements
{
    [Flags]
    public enum KMouseStates : byte
    {
        Mouse_1 = 1 << 0,
        Mouse_2 = 1 << 1,
        Mouse_3 = 1 << 2,
        Mouse_4 = 1 << 3,
        Mouse_5 = 1 << 4,
    }

    [Flags]
    public enum KKeyStates : byte
    {
        PRESSED = 1 << 0,
        RELEASED = 1 << 2,
        SHIFT = 1 << 3,
        CONTROL = 1 << 4,
        ALTERNATE = 1 << 5,
        SYSTEM = 1 << 6,
    }

    public class KInputManager
    {        
        public const byte MAX_KEYS = 128;

        private byte _activeKeyCount; //The current amount of active keys.
        private StringBuilder _stringBuilder;
        private KKeyStates[] _keyStates;
        private byte[] _activeKeys;

        public int MousePosX;
        public int MousePosY;
        public float ScrollDelta;
        public KMouseStates MouseStates;
        public KMouseStates PreviousMouseStates;

        public KInputManager()
        {
            ScrollDelta = MousePosX = MousePosY = _activeKeyCount = 0;
            MouseStates = PreviousMouseStates = 0;

            _stringBuilder = new(128);
            _keyStates = new KKeyStates[MAX_KEYS];
            _activeKeys = new byte[MAX_KEYS];

            for (byte i = 0; i < _activeKeys.Length; i++)
            {
                _keyStates[i] = 0;
                _activeKeys[i] = MAX_KEYS;
            }
        }

        public void Init(RenderWindow window)
        {
            window.MouseMoved += UpdateMousePosition;
            window.MouseWheelScrolled += UpdateScrollDelta;
            window.MouseButtonPressed += RegisterMouseButtonPress;
            window.MouseButtonReleased += RegisterMouseButtonRelease;
            window.KeyPressed += RegisterKeyPress;
            window.KeyReleased += RegisterKeyRelease;
            window.TextEntered += UpdateTextBuffer;
        }

        public void Deinit(RenderWindow window)
        {
            window.MouseMoved -= UpdateMousePosition;
            window.MouseWheelScrolled -= UpdateScrollDelta;
            window.MouseButtonPressed -= RegisterMouseButtonPress;
            window.MouseButtonReleased -= RegisterMouseButtonRelease;
            window.KeyPressed -= RegisterKeyPress;
            window.KeyReleased -= RegisterKeyRelease;
            window.TextEntered -= UpdateTextBuffer;
        }

        public void Update()
        {
            //Clear text buffer.
            _stringBuilder.Clear();

            //Update Mouse.
            ScrollDelta = 0;
            PreviousMouseStates = MouseStates;

            //Update active keys.
            for (int i = 0; i < _activeKeyCount; i++)
            {
                if (_activeKeys[i] == MAX_KEYS) break;
                if (_keyStates[_activeKeys[i]].HasFlag(KKeyStates.RELEASED))
                {
                    _keyStates[_activeKeys[i]] = 0;
                    _activeKeys[i] = MAX_KEYS;
                    _activeKeyCount--;
                }
            }
        }

        private void UpdateMousePosition(object? ignored, MouseMoveEventArgs e) => 
            (MousePosX, MousePosY) = (e.X, e.Y);

        private void UpdateScrollDelta(object? ignored, MouseWheelScrollEventArgs e) => ScrollDelta += e.Delta;

        private void RegisterMouseButtonPress(object? ignored, MouseButtonEventArgs e) =>
            MouseStates |= (KMouseStates)(1 << (byte)e.Button);

        private void RegisterMouseButtonRelease(object? ignored, MouseButtonEventArgs e) =>
            MouseStates &= ~(KMouseStates)(1 << (byte)e.Button);

        private void UpdateTextBuffer(object? ignored, TextEventArgs e) =>
            _stringBuilder.Append(e.Unicode);

        private void RegisterKeyPress(object? ignored, KeyEventArgs e)
        {
            KKeyStates states = KKeyStates.PRESSED;
            if (e.Shift) states |= KKeyStates.SHIFT;
            if (e.Control) states |= KKeyStates.CONTROL;
            if (e.Alt) states |= KKeyStates.ALTERNATE;
            if (e.System) states |= KKeyStates.SYSTEM;

            _keyStates[(int)e.Code] = states; //Sets key state.

            if (_keyStates[(int)e.Code].HasFlag(KKeyStates.PRESSED)) return; //Checks if key is active.
            _activeKeys[_activeKeyCount] = (byte)e.Code; //If key is not active, add it.
            _activeKeyCount++;
        }

        private void RegisterKeyRelease(object? ignored, KeyEventArgs e)
        {
            if (_keyStates[(int)e.Code].HasFlag(KKeyStates.PRESSED))
            {
                _keyStates[(int)e.Code] |= KKeyStates.RELEASED;
            }
        }

        public string ReadTextBuffer() => _stringBuilder.ToString();

        public bool IsMouseDown(KMouseStates mouseStates) => MouseStates.HasFlag(mouseStates);

        public bool IsMousePressed(KMouseStates mouseStates) =>
            MouseStates.HasFlag(mouseStates) && !PreviousMouseStates.HasFlag(mouseStates);

        public bool IsMouseReleased(KMouseStates mouseStates) =>
            !MouseStates.HasFlag(mouseStates) && PreviousMouseStates.HasFlag(mouseStates);

        public bool CheckKeyStates(in byte keyCode, in KKeyStates states) =>
            keyCode < MAX_KEYS ? _keyStates[keyCode].HasFlag(states) : false;

        public bool IsKeyDown(in byte keyCode) => !CheckKeyStates(keyCode, KKeyStates.RELEASED);

        public bool IsKeyPressed(in byte keyCode) => CheckKeyStates(keyCode, KKeyStates.PRESSED);

        public bool IsKeyReleased(in byte keyCode) => CheckKeyStates(keyCode, KKeyStates.RELEASED);

        //SFML support
        public bool IsKeyDown(in Keyboard.Key key) => !CheckKeyStates((byte)key, KKeyStates.RELEASED);

        public bool IsKeyPressed(in Keyboard.Key key) => CheckKeyStates((byte)key, KKeyStates.PRESSED);

        public bool IsKeyReleased(in Keyboard.Key key) => CheckKeyStates((byte)key, KKeyStates.RELEASED);
    }
}

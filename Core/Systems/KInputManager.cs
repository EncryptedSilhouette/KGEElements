using SFML.Window;
using System.Text;

namespace Elements.Core.Systems
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
        public record struct KKey(in byte KeyCode, in KKeyStates States);

        private byte _activeKeyCount; //The current amount of active keys.
        private KKey[] _activeKeys;
        private StringBuilder _stringBuilder; 

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
            _activeKeys = new KKey[128]; 

            for (int i = 0; i < _activeKeys.Length; i++)
            {
                _activeKeys[i] = new(byte.MaxValue, 0);
            }
        }

        public void Init(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved += UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled += UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed += RegisterMouseButtonPress;
            windowManager.Window.MouseButtonReleased += RegisterMouseButtonRelease;
            windowManager.Window.KeyPressed += RegisterKeyPress;
            windowManager.Window.KeyReleased += RegisterKeyRelease;
            windowManager.Window.TextEntered += UpdateTextBuffer;
        }

        public void Deinit(in KWindowManager windowManager)
        {
            windowManager.Window.MouseMoved -= UpdateMousePosition;
            windowManager.Window.MouseWheelScrolled -= UpdateScrollDelta;
            windowManager.Window.MouseButtonPressed -= RegisterMouseButtonPress;
            windowManager.Window.MouseButtonReleased -= RegisterMouseButtonRelease;
            windowManager.Window.KeyPressed -= RegisterKeyPress;
            windowManager.Window.KeyReleased -= RegisterKeyRelease;
            windowManager.Window.TextEntered -= UpdateTextBuffer;
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
                //Remove any released keys. 
                if (_activeKeys[i].States.HasFlag(KKeyStates.RELEASED))
                {
                    _activeKeys[i] = _activeKeys[_activeKeyCount - 1];
                    _activeKeyCount--;
                    break;
                }
                //Removes states.
                //Key remains active until released, thus held state is a given, and no "held" state required.
                _activeKeys[i].States = 0;
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

            for (int i = 0; i < _activeKeyCount; i++)
            {
                //Checks if key is active.
                if (_activeKeys[i].KeyCode == (byte)e.Code)
                {
                    _activeKeys[i].States = states;
                    return;
                }
            }
            //If key is not active, add it.
            _activeKeys[_activeKeyCount] = new((byte)e.Code, states);
            _activeKeyCount++;
        }

        private void RegisterKeyRelease(object? ignored, KeyEventArgs e)
        {
            for (int i = 0; i < _activeKeyCount; i++)
            {
                //Checks if key is active.
                if (_activeKeys[i].KeyCode == (byte)e.Code)
                {
                    //Key will be removed when updated.
                    //Released state will only exist for an update.
                    _activeKeys[i].States |= KKeyStates.RELEASED;
                    return;
                }
            }
        }

        public string ReadTextBuffer() => _stringBuilder.ToString();

        public bool IsMouseDown(KMouseStates mouseStates) => MouseStates.HasFlag(mouseStates);

        public bool IsMousePressed(KMouseStates mouseStates) =>
            MouseStates.HasFlag(mouseStates) && !PreviousMouseStates.HasFlag(mouseStates);

        public bool IsMouseReleased(KMouseStates mouseStates) =>
            !MouseStates.HasFlag(mouseStates) && PreviousMouseStates.HasFlag(mouseStates);

        public bool CheckKeyStates(in byte keyCode, in KKeyStates states)
        {
            for (int i = 0; i < _activeKeyCount; i++)
            {
                //Checks if key exists, then checks flags.
                if (_activeKeys[i].KeyCode == keyCode)
                {
                    return _activeKeys[i].States.HasFlag(states);
                }
            }
            return false;
        }

        public bool IsKeyDown(in byte keyCode) => !CheckKeyStates(keyCode, KKeyStates.RELEASED);

        public bool IsKeyPressed(in byte keyCode) => CheckKeyStates(keyCode, KKeyStates.PRESSED);

        public bool IsKeyReleased(in byte keyCode) => CheckKeyStates(keyCode, KKeyStates.RELEASED);

        //SFML support
        public bool IsKeyDown(in Keyboard.Key key) => !CheckKeyStates((byte)key, KKeyStates.RELEASED);

        public bool IsKeyPressed(in Keyboard.Key key) => CheckKeyStates((byte)key, KKeyStates.PRESSED);

        public bool IsKeyReleased(in Keyboard.Key key) => CheckKeyStates((byte)key, KKeyStates.RELEASED);
    }
}

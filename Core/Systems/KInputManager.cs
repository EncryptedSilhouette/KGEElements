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
        PREVIOUS = 1 << 1,
        RELEASED = 1 << 2,
        SHIFT = 1 << 3,
        CONTROL = 1 << 4,
        ALTERNATE = 1 << 5,
        SYSTEM = 1 << 6,
    }

    public class KInputManager
    {
        public record struct KKey(in byte KeyCode, in KKeyStates States);

        private byte _activeKeyCount;
        private StringBuilder _stringBuilder;
        private KKey[] _activeKeys;

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
         
        }

        public void FrameUpdate()
        {
            _stringBuilder.Clear();
            PreviousMouseStates = MouseStates;
            UpdateActiveKeys();
        }

        private void UpdateMousePosition(object? ignored, MouseMoveEventArgs e) => 
            (MousePosX, MousePosY) = (e.X, e.Y);

        private void UpdateScrollDelta(object? ignored, MouseWheelScrollEventArgs e) => ScrollDelta = e.Delta;

        private void RegisterMouseButtonPress(object? ignored, MouseButtonEventArgs e) =>
            MouseStates |= (KMouseStates)(1 << (byte)e.Button);

        private void RegisterMouseButtonRelease(object? ignored, MouseButtonEventArgs e) =>
            MouseStates &= ~(KMouseStates)(1 << (byte)e.Button);

        private void UpdateTextBuffer(object? ignored, TextEventArgs e) =>
            _stringBuilder.Append(e.Unicode);

        private void RegisterKeyPress(object? ignored, KeyEventArgs e)
        {
            KKey key = new()
            {
                KeyCode = (byte)e.Code,
                States = KKeyStates.PRESSED
            };
            if (e.Shift) key.States |= KKeyStates.SHIFT;
            if (e.Control) key.States |= KKeyStates.CONTROL;
            if (e.Alt) key.States |= KKeyStates.ALTERNATE;
            if (e.System) key.States |= KKeyStates.SYSTEM;

            AddActiveKey(key);
        }

        private void RegisterKeyRelease(object? ignored, KeyEventArgs e) => RemoveActiveKey((byte)e.Code);

        private void AddActiveKey(in KKey key)
        {
            for (int i = 0; i < _activeKeyCount; i++)
            {
                if (_activeKeys[i].KeyCode == key.KeyCode) return;
            }
            _activeKeys[_activeKeyCount] = key;
            _activeKeyCount++;
        }

        private void RemoveActiveKey(in byte keyCode)
        {
            for (int i = 0; i < _activeKeyCount; i++)
            {
                if (_activeKeys[i].KeyCode == keyCode)
                {
                    _activeKeys[i].States |= KKeyStates.RELEASED;
                }
            }
        }

        private void UpdateActiveKeys()
        {
            for (int i = 0; i < _activeKeys.Length; i++)
            {
                //Check if the key is valid.
                if (_activeKeys[i].KeyCode == byte.MaxValue) return;

                //Check if the key was released.
                if (_activeKeys[i].States.HasFlag(KKeyStates.RELEASED))
                {
                    //Get item in last index and replace item to be removed.
                    _activeKeys[i] = _activeKeys[_activeKeyCount - 1];
                    //After last index is copied clear last index.
                    _activeKeys[_activeKeyCount - 1] = new(byte.MaxValue, 0);
                    _activeKeyCount--;
                    return;
                }

                //Set Previous state to current Pressed state.
                if (_activeKeys[i].States.HasFlag(KKeyStates.PRESSED))
                {
                    //Get current Pressed state and bit shift it by one to batck bit allignment of Previous state.
                    var newState = (KKeyStates)((int)(_activeKeys[i].States & KKeyStates.PRESSED) << 1);
                    //Clear last Previous state.
                    _activeKeys[i].States &= ~KKeyStates.PREVIOUS;
                    //Set new Previous state.
                    _activeKeys[i].States |= newState; 
                }
            }
        }

        public string ReadTextBuffer() => _stringBuilder.ToString();

        public bool IsMouseDown(KMouseStates mouseStates) => MouseStates.HasFlag(mouseStates);

        public bool IsMousePressed(KMouseStates mouseStates) =>
            MouseStates.HasFlag(mouseStates) && !PreviousMouseStates.HasFlag(mouseStates);

        public bool IsMouseReleased(KMouseStates mouseStates) =>
            !MouseStates.HasFlag(mouseStates) && PreviousMouseStates.HasFlag(mouseStates);

        public bool CheckKeyStates(in byte keyCode, in KMouseStates states)
        {
            for (int i = 0; i < _activeKeyCount; i++)
            {
                if (_activeKeys[i].KeyCode == keyCode)
                {
                    return _activeKeys[i].States.HasFlag(states);
                }
            }
            return false;
        }
    }
}

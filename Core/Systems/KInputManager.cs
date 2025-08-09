using SFML.Window;
using System.Text;
using static SFML.Window.Keyboard;
using static System.Runtime.CompilerServices.RuntimeHelpers;

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
        HELD = 1 << 1,
        SHIFT = 1 << 2,
        CONTROL = 1 << 3,
        ALTERNATE = 1 << 4,
        SYSTEM = 1 << 5,
    }

    public class KInputManager
    {
        public record struct KKey(byte KeyCode, KKeyStates States);

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
            //for (int i = 0; i < _activeKeyCount; i++)
            //{
            //    if (_activeKeys[i].KeyCode == keyCode)
            //    {
            //        _activeKeys[i] = _activeKeys[_activeKeyCount - 1];
            //        _activeKeyCount--;
            //        return;
            //    }
            //}
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
            AddActiveKey(new((byte)e.Code, KKeyStates.PRESSED));
        }

        private void RegisterKeyRelease(object? ignored, KeyEventArgs e)
        {
            RemoveActiveKey
        }

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
                ref var key = ref _activeKeys[i];
                if (_activeKeys[i].KeyCode == keyCode)
                {
                    key.States &= KKeyStates.PRESSED | KKeyStates.HELD;
                }
            }
        }

        public string ReadTextBuffer() => _stringBuilder.ToString();

        public bool IsMouseDown(KMouseStates mouseStates) => MouseStates.HasFlag(mouseStates);

        public bool IsMousePressed(KMouseStates mouseStates) =>
            MouseStates.HasFlag(mouseStates) && !PreviousMouseStates.HasFlag(mouseStates);

        public bool IsMouseReleased(KMouseStates mouseStates) =>
            !MouseStates.HasFlag(mouseStates) && PreviousMouseStates.HasFlag(mouseStates);

    }
}

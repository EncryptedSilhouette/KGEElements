using Elements.Rendering;
using SFML.Graphics;
using System.Text;

namespace Elements
{
    public class KCLI
    {
        private bool _enabled = true;
        private bool _isReadingInput = false;
        private Color _color = new Color(100,100,100);
        private StringBuilder _textBuffer = new();
        private KCommandManager _commandManager;

        public Color TextColor;

        public Color InterfaceColor
        {
            get => _color;
            set => _color = new(value.R, value.G, value.B, InterfaceOpacity);
        }
        
        public byte InterfaceOpacity
        {
            get => _color.A;
            set => _color.A = value;
        }

        public KCLI(KCommandManager commandManager)
        {
            _commandManager = commandManager;
        }

        public void Update(KInputManager inputManager)
        {
            if (!_enabled) return;

            if (_isReadingInput)
            {
                _textBuffer.Append(inputManager.ReadTextBuffer());
            }
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            if (!_enabled) return;
            Console.WriteLine(KProgram.LogManager.GetLog(KLogManager.DEBUG_LOG));
            renderManager.TextRenderers[0].SubmitDraw(KProgram.LogManager.GetLog(KLogManager.DEBUG_LOG), 0, 32);
        }

        public void StartReadTextBuffer()
        {
            _textBuffer.Clear();
            _isReadingInput = true;
        }

        public void StopReadTextBuffer() => _isReadingInput = false;

        public void SubmitCommandString() => _commandManager.SubmitCommandString(_textBuffer.ToString());

        public string ReadTextBuffer() => _textBuffer.ToString();
    }
}

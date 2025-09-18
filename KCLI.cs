using System.Text;

namespace Elements
{
    public class KCLI
    {
        private bool _isReadingInput = false; 
        private StringBuilder _textBuffer = new();
        private KCommandManager _commandManager;

        public KCLI(KCommandManager commandManager)
        {
            _commandManager = commandManager;
        }

        public void Update(KInputManager inputManager)
        {
            if (_isReadingInput)
            {
                _textBuffer.Append(inputManager.ReadTextBuffer());
            }
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

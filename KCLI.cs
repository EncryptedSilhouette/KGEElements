using Elements.Core;
using Elements.Game.UI;
using Elements.Rendering;
using SFML.Graphics;
using System.Text;

namespace Elements
{
    public class KCLI
    {
        private bool _enabled = true;
        private bool _isReadingInput = false;
        private Color _color;
        private StringBuilder _textBuffer = new();
        private KCommandManager _commandManager;

        //Draw background
        private KDrawData drawData;
        private KRectangle drawBounds;

        //Input box
        private KTextInput _textInput;

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
            _color = new(100, 100, 100);
            _commandManager = commandManager;
            
            drawData = new()
            {
                Color = _color
            };

            drawBounds = new()
            {
                Width = 800, 
                Height = 1600,
                Transform = new()
                {
                    PosX = 400,
                    PosY = 800
                }
            };

            _textInput = new(0, 0, KProgram.Window.Size.X, 32);
        }

        public void Update(KInputManager inputManager)
        {
            if (!_enabled) return;

            _textInput.Update(inputManager);
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            if (!_enabled) return;
            renderManager.TextRenderers[0].SubmitDraw(KProgram.LogManager.GetLog(KLogManager.DEBUG_LOG), 0, 0, out FloatRect aBounds, 512);       
            renderManager.SubmitDraw(drawData, aBounds);

            renderManager.SubmitDraw(_textInput.Button.DrawData, _textInput.Button.Bounds);
            renderManager.TextRenderers[0].SubmitDraw(
                _textInput.Button.TextBox, 
                _textInput.Button.Bounds.TopLeft.X, 
                _textInput.Button.Bounds.TopLeft.Y, 
                out FloatRect bBounds, 10);


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

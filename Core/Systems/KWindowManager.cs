using SFML.Graphics;
using SFML.Window;

namespace Elements.Core.Systems
{
    public class KWindowManager
    {
        public static VideoMode DesktopMode = VideoMode.DesktopMode;
        public static VideoMode[] FullscreenModes = VideoMode.FullscreenModes;

        private string _title;

        public Color BackgroundColor;
        public KDrawHandler DrawHandler;
        public RenderWindow Window;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Window.SetTitle(value);
            }
        }

        public KWindowManager() 
        {
            _title = "Elements";
            BackgroundColor = Color.Black;

            Window = new(DesktopMode, _title);
            DrawHandler = new(this, KDrawHandler.DEFAULT_ATLAS);
        }

        public void Init(KResourceManager resourceManager)
        {
            DrawHandler.Init(resourceManager);
        }

        public void Update()
        {
            Window.DispatchEvents();
        }

        public void FrameUpdate()
        {
            DrawHandler.DrawFrame();
        }
    }
}

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
        public KDrawHandler DrawManager;
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
            DrawManager = new(this, KTextureAtlas.Load("texture_data.txt"));
        }

        public void Update()
        {
            Window.DispatchEvents();
        }

        public void FrameUpdate()
        {
            DrawManager.DrawFrame();
        }
    }
}

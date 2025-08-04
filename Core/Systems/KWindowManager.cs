using SFML.Graphics;
using SFML.Window;

namespace Elements.Core.Systems
{
    public struct KWindowManager
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
            DrawManager = new(this, KTextureData.Load("texture_data.txt"));
        }

        public void FrameUpdate()
        {
            Window.DispatchEvents();
            DrawManager.DrawFrame();
        }
    }
}

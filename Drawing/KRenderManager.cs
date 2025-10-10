using Elements.Core;
using SFML.Graphics;

namespace Elements.Drawing
{
    public class KRenderManager
    {
        private RenderStates _states;
        private View[] _cameraViews;
        private KDrawLayer[] _drawLayers;

        public Color BackgroundColor;
        public RenderWindow Window;

        public int TopLayer => _drawLayers.Length;

        public View[] Cameras => _cameraViews;

        public KRenderManager(RenderWindow window)
        {
            _states = RenderStates.Default;
            _cameraViews = [];
            _drawLayers = [];
            BackgroundColor = Color.Black;
            Window = window;
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(View[] cameraViews, KDrawLayer[] drawLayers)
        {
            _cameraViews = cameraViews;
            _drawLayers = drawLayers;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < _drawLayers.Length; i++)
            {
                _drawLayers[i].DrawFrame(_cameraViews[_drawLayers[i].CameraID]);
                _states.Texture = _drawLayers[i].RenderTexture.Texture;

                Vertex[] vertices =
                [
                    new Vertex() 
                    {
                        TexCoords = new(0, 0),
                        Position = new(0, 0), 
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(_states.Texture.Size.X, 0),
                        Position = new(Window.Size.X, 0), 
                        Color = Color.White 
                    },
                    new Vertex() 
                    {
                        TexCoords = new(_states.Texture.Size.X, _states.Texture.Size.Y),
                        Position = new(Window.Size.X, Window.Size.Y), 
                        Color = Color.White
                    },
                    new Vertex() 
                    {
                        TexCoords = new(0, _states.Texture.Size.Y),
                        Position = new(0, Window.Size.Y), 
                        Color = Color.White
                    },
                ];
                Window.Draw(vertices, PrimitiveType.Quads, _states);
            }

            Window.Display();
        }

        public void SubmitDraw(in int layer, in KDrawData dat, in KRectangle rec) =>
            _drawLayers[layer].SubmitDraw(dat, rec);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec) =>
            _drawLayers[TopLayer].SubmitDraw(dat, rec);

        public void SubmitDraw(in int layer, Vertex[] vertices) =>
            _drawLayers[layer].SubmitDraw(vertices);

        public void SubmitDraw(Vertex[] vertices) =>
            _drawLayers[TopLayer].SubmitDraw(vertices);
    }
}

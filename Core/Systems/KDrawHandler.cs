using SFML.Graphics;

namespace Elements.Core.Systems
{
    public struct KDrawHandler
    {
        private RenderWindow _window;
        private KTextureData _masterAtlas;
        private List<KDrawLayer> _drawLayers;

        public KDrawHandler(KWindowManager windowManager, KTextureData atlas)
        {
            _window = windowManager.Window;
            _masterAtlas = atlas;
            _drawLayers = new();
        }

        public void DrawFrame()
        {
            _window.Clear();



            _window.Display();
        }

        public void CreateLayer() => _drawLayers.Add(new(256, _window.Size.X, _window.Size.Y, _masterAtlas));
    }

    public struct KDrawLayer
    {
        private KTextureData _atlas;
        private RenderTexture _renderTexture;
        private VertexBuffer _buffer;

        public KDrawLayer(in uint bufferSize, in uint width, in uint height, KTextureData atlas) 
        {
            _renderTexture = new(width, height);
            _buffer = new(bufferSize, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            _atlas = atlas;
        }
    }
}

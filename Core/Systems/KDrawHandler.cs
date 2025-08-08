using SFML.Graphics;

namespace Elements.Core.Systems
{
    public struct KDrawHandler
    {
        private RenderWindow _window;
        private KTextureAtlas _masterAtlas;
        private KRenderLayer[] _drawLayers;

        public int DrawLayerCount => _drawLayers.Length;

        public KDrawHandler(KWindowManager windowManager, KTextureAtlas atlas)
        {
            _window = windowManager.Window;
            _masterAtlas = atlas;
            _drawLayers = new KRenderLayer[8] 
            {
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
                new KRenderLayer(),
            };
        }

        public void DrawFrame()
        {
            _window.Clear();

            foreach (var layer in _drawLayers)
            {
                
            }

            _window.Display();
        }

        public void SubmitDraw(int layer, in KDrawData drawData, in KTransform transform)
        {
            KRectangle drawBounds = new()
            {
                Width = drawData.Sprite.Width,
                Height = drawData.Sprite.Height,
                Transform = transform
            };
            SubmitDraw(layer, drawData, drawBounds);
        }

        public void SubmitDraw(int layer , in KDrawData drawData, in KRectangle rectangle)
        {
            if (layer > DrawLayerCount - 1) layer = DrawLayerCount - 1;

            _drawLayers[layer].SubmitDraw(drawData, rectangle);
        }
    }

    public struct KRenderLayer
    {
        private uint _bufferOffset;
        private KTextureAtlas _atlas;
        private RenderStates _renderStates;
        private VertexBuffer _buffer;

        public RenderTexture RenderTexture { get; private set; }

        public KRenderLayer(in uint width, in uint height, in uint bufferSize, in RenderStates renderStates, KTextureAtlas atlas) 
        {
            RenderTexture = new(width, height);
            _buffer = new(bufferSize, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            _renderStates = renderStates;
            _atlas = atlas;
            _renderStates.Texture = _atlas.Texture;
        }

        public void SubmitDraw(in KDrawData dat, in KRectangle rec)
        {
            Vertex[] vertices =
            [
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = new(dat.Sprite.TexCoordsX, dat.Sprite.TexCoordsY),
                    Position = rec.TopLeft
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = new(dat.Sprite.TexCoordsX + dat.Sprite.Width, dat.Sprite.TexCoordsY),
                    Position = rec.TopRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = new(dat.Sprite.TexCoordsX + dat.Sprite.Width, dat.Sprite.TexCoordsY + dat.Sprite.Height),
                    Position = rec.BottomRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = new(dat.Sprite.TexCoordsX, dat.Sprite.TexCoordsY + dat.Sprite.Height),
                    Position = rec.BottomLeft
                }
            ];
            _buffer.Update(vertices, 4, _bufferOffset);
            _bufferOffset += 4;
        }

        public void DrawFrame()
        {
            _buffer.Draw(RenderTexture, 0, _bufferOffset, _renderStates);
            _bufferOffset = 0;
        }
    }
}

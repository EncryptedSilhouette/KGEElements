using SFML.Graphics;

namespace Elements.Core.Systems
{
    public struct KDrawHandler
    {
        public RenderWindow Window;
        public RenderStates RenderStates;
        public KDrawLayer[] DrawLayers;

        public int DrawLayerCount => DrawLayers.Length;

        public KDrawHandler(KWindowManager windowManager, KTextureAtlas atlas, int layers = 8)
        {
            Window = windowManager.Window;
            RenderStates = RenderStates.Default;
            RenderStates.Texture = atlas.Texture;
            DrawLayers = new KDrawLayer[layers];

            for (int i = 0; i < layers; i++) 
            {
                DrawLayers[i] = new KDrawLayer(Window.Size.X, Window.Size.Y, 256, RenderStates);
            }
        }

        public void DrawFrame()
        {
            Vertex[] vertices =
            [
                new Vertex()
                {
                    Color = Color.White,
                    TexCoords = new(0, 0),
                    Position = new(0, 0)
                },
                new Vertex()
                {
                    Color = Color.White,
                    TexCoords = new(RenderStates.Texture.Size.X, 0),
                    Position = new(Window.Size.X, 0)
                },
                new Vertex()
                {
                    Color = Color.White,
                    TexCoords = new(RenderStates.Texture.Size.X, RenderStates.Texture.Size.Y),
                    Position = new(Window.Size.X, Window.Size.Y)
                },
                new Vertex()
                {
                    Color = Color.White,
                    TexCoords = new(0, RenderStates.Texture.Size.Y),
                    Position = new(0, Window.Size.Y)
                }
            ];

            Window.Clear(Color.White);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                RenderStates.Texture = DrawLayers[i].DrawFrame().Texture;
                Window.Draw(vertices, PrimitiveType.Quads, RenderStates);
            }

            Window.Display();
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
            DrawLayers[layer].SubmitDraw(drawData, rectangle);
        }
    }

    public struct KDrawLayer
    {
        private uint _bufferOffset = 0;

        public VertexBuffer Buffer;
        public RenderStates RenderStates;
        public RenderTexture RenderTexture;

        public KDrawLayer(in uint width, in uint height, in uint bufferSize, in RenderStates renderStates) 
        {
            Buffer = new(bufferSize, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic);
            RenderTexture = new(width, height);
            RenderStates = renderStates;
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
            Buffer.Update(vertices, 4, _bufferOffset);
            _bufferOffset += 4;
        }

        public RenderTexture DrawFrame()
        {
            RenderTexture.Clear(Color.Black);
            Buffer.Draw(RenderTexture, 0, _bufferOffset, RenderStates);
            RenderTexture.Display();

            _bufferOffset = 0;
            return RenderTexture;
        }
    }
}

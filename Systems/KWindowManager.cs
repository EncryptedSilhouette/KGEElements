using Elements.Core;
using SFML.Graphics;
using SFML.Window;

//640x360 

namespace Elements.Systems
{
    public class KWindowManager
    {
        public readonly static VideoMode DESKTOP_MODE = VideoMode.DesktopMode;
        public readonly static VideoMode[] FULLSCREEN_MODES = VideoMode.FullscreenModes;
        public readonly static Texture DEFAULT_TEXTURE = new Texture(new Image(16, 16, Color.Magenta));
        
        private string _title;
        private RenderStates _renderStates;

        public Color BackgroundColor;
        public RenderWindow Window;
        public KDrawLayer[] DrawLayers;

        public int TopLayer => DrawLayers.Length - 1;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Window.SetTitle(value);
            }
        }

        public KWindowManager(string title)
        {
            _title = title;
            _renderStates = RenderStates.Default;
            BackgroundColor = Color.White;
            Window = new(DESKTOP_MODE, _title);
            Window.Closed += (_, _) => KProgram.Running = false;

            DrawLayers = [CreateDrawLayer(256)];
        }

        public KWindowManager(string title, KDrawLayer[] layers) : this(title)
        {
            DrawLayers = layers;
        }

        public void Init(KDrawLayer[] layers)
        {
            DrawLayers = layers;
        }

        public void Update() => Window.DispatchEvents();

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < DrawLayers.Length; i++)
            {
                _renderStates.Texture = DrawLayers[i].RenderTexture.Texture;
                Window.Draw(DrawLayers[i].Vertices, PrimitiveType.Quads, _renderStates);
            }

            Window.Display();
        }

        public void SubmitDraw(int layer, in KDrawData dat, in KRectangle rec) =>
            KDrawLayer.SubmitDraw(ref DrawLayers[layer], dat, rec);

        public void SubmitDraw(in KDrawData dat, in KRectangle rec) =>
            KDrawLayer.SubmitDraw(ref DrawLayers[TopLayer], dat, rec);

        public KDrawLayer CreateDrawLayer(uint bufferSize)
        {
            return new()
            {
                RenderStates = RenderStates.Default,
                RenderTexture = new(Window.Size.X, Window.Size.Y),
                Buffer = new VertexBuffer(bufferSize, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
                Vertices = 
                [
                    new Vertex(new(0, 0), Color.White, new(0, 0)),
                    new Vertex(new(Window.Size.X, 0), Color.White, new(Window.Size.X, 0)),
                    new Vertex(new(Window.Size.X, Window.Size.Y), Color.White, new(Window.Size.X, Window.Size.Y)),
                    new Vertex(new(0, Window.Size.Y), Color.White, new(0, Window.Size.Y)),
                ]
            };
        }
    }

    public struct KDrawLayer
    {
        #region Static 

        public static void SubmitDraw(ref KDrawLayer drawLayer, in KDrawData dat, in KRectangle rec)
        {
            Vertex[] vertices =
            [
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TextureBounds.TopLeft,
                    Position = rec.TopLeft
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TextureBounds.TopRight,
                    Position = rec.TopRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TextureBounds.BottomRight,
                    Position = rec.BottomRight
                },
                new Vertex()
                {
                    Color = dat.Color,
                    TexCoords = dat.Sprite.TextureBounds.BottomLeft,
                    Position = rec.BottomLeft
                }
            ];
            drawLayer.Buffer.Update(vertices, 4, drawLayer._bufferOffset);
            drawLayer._bufferOffset += 4;
        }

        //public static void SubmitDraw(ref KDrawLayer drawLayer, in KDrawData drawData, in KTransform transform)
        //{
        //    KRectangle drawBounds = new()
        //    {
        //        Width = drawData.Sprite.Width,
        //        Height = drawData.Sprite.Height,
        //        Transform = transform
        //    };
        //    SubmitDraw(ref drawLayer, drawData, drawBounds);
        //}

        #endregion

        private uint _bufferOffset;

        public RenderStates RenderStates;
        public RenderTexture RenderTexture;
        public VertexBuffer Buffer;
        public Vertex[] Vertices;

#nullable disable
        public KDrawLayer() => (_bufferOffset, RenderStates) = (0, RenderStates.Default);
#nullable enable

        public KDrawLayer(in RenderStates renderStates, RenderTexture renderTexture, VertexBuffer buffer, Vertex[] vertices) =>
            (RenderStates, RenderTexture, Buffer, Vertices) = (renderStates, renderTexture, buffer, vertices);

        public void DrawFrame()
        {
            RenderTexture.Clear(Color.Black);
            Buffer.Draw(RenderTexture, 0, _bufferOffset, RenderStates);
            RenderTexture.Display();

            _bufferOffset = 0;
        }
    }
}

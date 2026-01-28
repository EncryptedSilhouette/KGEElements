using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Rendering
{
    public record struct KBufferRegion(uint Offset, uint Max, uint Count = 0);

    public struct KRenderLayer
    {
        public int BufferRegion;
        public Color ClearColor;
        public FloatRect DrawBounds;
        public RenderStates RenderStates;
        required public IRenderTarget RenderTarget;

        public KRenderLayer()
        {
            BufferRegion = 0;
            ClearColor = Color.Transparent;
            RenderStates = RenderStates.Default;
            DrawBounds = new();
        }
    }

    public class KRenderManager
    {
        //Render using PrimitiveType.TriangleStrip for quads. It only uses 4 vertices instead of 6.
        //  _____   The order of vertices is different though as we're still drawing 2 triangles.
        // |A   B|  The usual order for Quads is ABCD.
        // |     |  For Triangles its ABD-BCD. 
        // |D___C|  For TriangleStrip its ABDC, Swapping points C & D.

        //Rendering with a single large vertex buffer allows up to avoid OpenGL context switching.

        private View _view;
        private VertexBuffer _vertexBuffer;
        private Vertex[] _drawBuffer;

        public Color BackgroundColor;
        public RenderStates RenderStates;
        public RenderWindow Window;
        public KTextHandler TextHandler;
        public KBufferRegion[] BufferRegions;
        public KRenderLayer[] RenderLayers;

        public float ScreenLeft => 0;
        public float ScreenRight => Window.Size.X;
        public float ScreenTop => 0;
        public float ScreenBottom => Window.Size.Y;
        public Vector2f ScreenTopLeft => (0, 0);
        public Vector2f ScreenTopRight => (Window.Size.X, 0);
        public Vector2f ScreenBottomRight => (Vector2f) Window.Size;
        public Vector2f ScreenBottomLeft => (0, Window.Size.Y);
        public Vector2f ScreenCenter => (Vector2f) Window.Size / 2;

        public KRenderManager(RenderWindow window, VertexBuffer vertexBuffer)
        {
            _view = window.DefaultView;
            _vertexBuffer = vertexBuffer;
            _drawBuffer = new Vertex[16];

            TextHandler = new(this);
            BackgroundColor = Color.Black;
            RenderStates = RenderStates.Default;
            Window = window;
            BufferRegions = [];
            RenderLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(KBufferRegion[] regions, KRenderLayer[] layers, Font[] fonts, KTextLayer[] textLayers)
        {
            BufferRegions = regions;
            RenderLayers = layers;
            TextHandler.Init(fonts, textLayers);
            Window.Resized += ResizeView;
        }

        public void Init(VertexBuffer vertexBuffer, KBufferRegion[] regions, KRenderLayer[] layers, Font[] fonts, KTextLayer[] textLayers)
        {
            _vertexBuffer = vertexBuffer;
            Init(regions, layers, fonts, textLayers);   
        }

        public void Deinit()
        {
            Window.Resized -= ResizeView;
        }

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            _drawBuffer[0] = new Vertex((0,0), Color.White);
            _drawBuffer[1] = new Vertex((100,0), Color.Red);
            _drawBuffer[2] = new Vertex((0,100), Color.Green);
            _drawBuffer[3] = new Vertex((100,100), Color.Blue);
            DrawBuffer(_drawBuffer, 4, 0);
            
            for (int i = 0; i < RenderLayers.Length; i++) //Iterates from the last index to
            {
                ref var layer = ref RenderLayers[i];
                ref var buffer = ref BufferRegions[layer.BufferRegion];

                _vertexBuffer.Draw(layer.RenderTarget, buffer.Offset, buffer.Count, layer.RenderStates);

                if (layer.RenderTarget is RenderTexture rt)
                {
                    var b = layer.DrawBounds;

                    _drawBuffer[0] = new Vertex(b.Position, Color.White, (0, 0));
                    _drawBuffer[1] = new Vertex((b.Position.X + b.Size.X, b.Position.Y), Color.White, (rt.Size.X, 0));
                    _drawBuffer[2] = new Vertex((b.Position.X, b.Position.Y + b.Size.Y), Color.White, (0,rt.Size.Y));
                    _drawBuffer[3] = new Vertex(b.Position + b.Size, Color.White, (Vector2f)rt.Size);

                    Window.Draw(_drawBuffer, PrimitiveType.TriangleFan, new RenderStates(rt.Texture));
                }
            }

            TextHandler.FrameUpdate(this);

            Window.Display();
        }

        //Draw to screen. 
        public void DrawBuffer(Vertex[] vertices, uint vCount, int layer = 0)
        {
            ref var region = ref BufferRegions[RenderLayers[layer].BufferRegion];
            if (vCount + region.Count > region.Max) vCount = region.Max - region.Count;
            _vertexBuffer.Update(vertices, vCount, region.Offset);
            region.Count += vCount;
        }

        public void DrawRect(Vector2f a, Vector2f b, Color color, int layer = 0)
        {
            _drawBuffer[0] = new Vertex(a, color);
            _drawBuffer[1] = new Vertex((b.X, a.Y), color);
            _drawBuffer[2] = new Vertex((a.X, b.Y), color);
            _drawBuffer[3] = new Vertex((b.X, b.Y), color);
            DrawBuffer(_drawBuffer, 4, layer);
        }

        public void DrawRect(FloatRect rect, Color color, int layer = 0) => 
            DrawRect(rect.Position, rect.Size, color, layer);

        ////Draw w/ texture data.
        public void DrawRect(in KDrawData dat, in FloatRect rec, int layer = 0)
        {
            //ABD
            _drawBuffer[0] = new Vertex(rec.Position, dat.Color, dat.Sprite.TopLeft);
            _drawBuffer[1] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            _drawBuffer[2] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);
            _drawBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            DrawBuffer(_drawBuffer, 4, layer);
        }

        public void DrawText()
        {
            
        }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            _view.Size = (Vector2f)e.Size;
            _view.Center = _view.Size / 2;
            Window.SetView(_view);
        }
    }
}

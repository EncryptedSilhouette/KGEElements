using Elements.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Elements.Rendering
{
    public struct KBufferRegion
    {
        public static KBufferRegion[] CreateBufferRegions(uint[] bufferSizes)
        {
            uint offset = 0;
            KBufferRegion[] regions = new KBufferRegion[bufferSizes.Length];

            for (int i = 0; i < regions.Length; i++)
            {
                regions[i] = new(offset, bufferSizes[i]);
                offset += bufferSizes[i];
            }

            return regions;
        }

        public uint Offset; 
        public uint Capacity; 
        public uint Count = 0;
    
        public KBufferRegion() { }

        public KBufferRegion(uint offset, uint max)
        {
            Count = 0;
            Offset = offset;
            Capacity = max;
        }
    }

    public struct KRenderLayer
    {
        public Color ClearColor;
        public FloatRect Bounds;
        public RenderStates RenderStates;
        public KBufferRegion BufferRegion;
        required public RenderTexture RenderTexture;

        public KRenderLayer()
        {
            ClearColor = Color.Transparent;
            RenderStates = RenderStates.Default;
            Bounds = new();
        }

        public void Clear() => RenderTexture.Clear(ClearColor);

        public void Draw(VertexBuffer vertexBuffer) => vertexBuffer.Draw(RenderTexture, BufferRegion.Offset, BufferRegion.Count, RenderStates);
    }

    public struct KDrawData
    {
        public Color Color;
        public KRectangle Sprite;

        public KDrawData()
        {
            Color = Color.White;
            Sprite = new KRectangle();
        }
    }

    public class KRenderManager
    {
        public const int SCREEN_LAYER = -1; 

        //Rendering with a single large vertex buffer allows up to avoid OpenGL context switching.
        private View _view;
        private VertexBuffer _vertexBuffer;
        private Vertex[] _drawBuffer;

        public Color BackgroundColor;
        public RenderStates RenderStates;
        public RenderWindow Window;
        public KTextHandler TextHandler;
        public KBufferRegion BufferRegion;
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
            _drawBuffer = new Vertex[6];

            TextHandler = new(this);
            BackgroundColor = Color.Black;
            RenderStates = RenderStates.Default;
            Window = window;
            RenderLayers = [];
        }

        //use during scene swapping if additional layers/cameras are needed.
        public void Init(KBufferRegion windowBuffer, KRenderLayer[] layers, Font[] fonts, KTextLayer[] textLayers)
        {
            BufferRegion = windowBuffer;
            RenderLayers = layers;
            TextHandler.Init(fonts, textLayers);
            Window.Resized += ResizeView;
        }

        public void Deinit() => Window.Resized -= ResizeView;

        public void FrameUpdate()
        {
            Window.Clear(BackgroundColor);

            for (int i = 0; i < RenderLayers.Length; i++)
            {
                ref var layer = ref RenderLayers[i];

                if (layer.BufferRegion.Count == 0) continue;

                var rt = layer.RenderTexture; 
                layer.Clear();
                layer.Draw(_vertexBuffer);
                rt.Display();

                _drawBuffer[0] = new Vertex(layer.Bounds.Position, Color.White, (0, 0));
                _drawBuffer[1] = new Vertex((layer.Bounds.Position.X + layer.Bounds.Size.X, layer.Bounds.Position.Y), Color.White, (rt.Size.X, 0));
                _drawBuffer[2] = new Vertex((layer.Bounds.Position.X, layer.Bounds.Position.Y + layer.Bounds.Size.Y), Color.White, (0,rt.Size.Y));
                                    
                _drawBuffer[3] = new Vertex((layer.Bounds.Position.X + layer.Bounds.Size.X, layer.Bounds.Position.Y), Color.White, (rt.Size.X, 0));
                _drawBuffer[4] = new Vertex(layer.Bounds.Position + layer.Bounds.Size, Color.White, (Vector2f)rt.Size);
                _drawBuffer[5] = new Vertex((layer.Bounds.Position.X, layer.Bounds.Position.Y + layer.Bounds.Size.Y), Color.White, (0,rt.Size.Y));

                Window.Draw(_drawBuffer, PrimitiveType.Triangles, new RenderStates(layer.RenderTexture.Texture));
            }

            if (BufferRegion.Count > 1) _vertexBuffer.Draw(Window, BufferRegion.Offset, BufferRegion.Count, RenderStates);

            Window.Display();   
        }

        public void DrawToWindow(Vertex[] vertices, uint vCount)
        {
            if (BufferRegion.Count + vCount > BufferRegion.Capacity) vCount = BufferRegion.Capacity - BufferRegion.Count;
            _vertexBuffer.Update(vertices, vCount, BufferRegion.Offset);
            BufferRegion.Count += vCount;
        }

        public void DrawToLayer(Vertex[] vertices, uint vCount, int layer = 0)
        {
            ref var l = ref RenderLayers[layer];
            if (l.BufferRegion.Count + vCount > l.BufferRegion.Capacity) vCount = l.BufferRegion.Capacity - l.BufferRegion.Count;
            _vertexBuffer.Update(vertices, vCount, l.BufferRegion.Offset);
            l.BufferRegion.Count += vCount;
        }

        public void DrawRect(Vector2f pointA, Vector2f pointB, Color color, int layer = 0)
        {
            _drawBuffer[0] = new Vertex(pointA, color);
            _drawBuffer[1] = new Vertex((pointB.X, pointA.Y), color);
            _drawBuffer[2] = new Vertex((pointA.X, pointB.Y), color);
                                    
            _drawBuffer[3] = new Vertex((pointB.X, pointA.Y), color);
            _drawBuffer[4] = new Vertex(pointB, color);
            _drawBuffer[5] = new Vertex((pointA.X, pointB.Y), color);

            if (layer < 0) DrawToWindow(_drawBuffer, 6);
            else DrawToLayer(_drawBuffer, 6, layer);
        }

        public void DrawRect(FloatRect rect, Color color, int layer = 0) => DrawRect(rect.Position, rect.Position + rect.Size, color, layer);

        // public void DrawRect(KDrawData drawData, int layer = 0)
        // {
        //     _drawBuffer[0] = new Vertex(pointA, color);
        //     _drawBuffer[1] = new Vertex((pointB.X, pointA.Y), color);
        //     _drawBuffer[2] = new Vertex((pointA.X, pointB.Y), color);
                                    
        //     _drawBuffer[3] = new Vertex((pointB.X, pointA.Y), color);
        //     _drawBuffer[4] = new Vertex(pointB, color);
        //     _drawBuffer[5] = new Vertex((pointA.X, pointB.Y), color);

        //     if (layer < 0) DrawToWindow(_drawBuffer, 6);
        //     else DrawToLayer(_drawBuffer, 6, layer);
        // }

        private void ResizeView(object? _, SizeEventArgs e)
        {
            _view.Size = (Vector2f)e.Size;
            _view.Center = _view.Size / 2;
            Window.SetView(_view);
        }

        public void ResizeBuffer(uint newSize)
        {
            var buffer = new VertexBuffer(newSize, _vertexBuffer.PrimitiveType, _vertexBuffer.Usage);
            buffer.Update(_vertexBuffer);   //Copy old buffer contents to new buffer.
            _vertexBuffer.Dispose();        //Dispose old object.
            _vertexBuffer = buffer;         //Assign refrence to new object.
        }
    }
}

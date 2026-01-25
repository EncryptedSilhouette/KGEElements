using Elements.Core;
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
        required public RenderTexture RenderTexture;

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
        public const int SCREEN_LAYER = -1; 

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

        public KRenderManager(RenderWindow window, VertexBuffer screenBuffer)
        {
            _view = window.DefaultView;
            _vertexBuffer = screenBuffer;
            _drawBuffer = new Vertex[6];

            BackgroundColor = Color.Black;
            RenderStates = RenderStates.Default;
            Window = window;
            TextHandler = new();
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

            for (int i = 0; i < RenderLayers.Length; i++)
            {
                ref var layer = ref RenderLayers[i];
                ref var region = ref BufferRegions[layer.BufferRegion];
                
                layer.RenderTexture.Clear(layer.ClearColor);

                _vertexBuffer.Draw(layer.RenderTexture, region.Offset, region.Count, layer.RenderStates);
                region.Count = 0; //Reset buffer region.

                layer.RenderTexture.Display();

                //ABD
                _drawBuffer[0] = new Vertex((layer.DrawBounds.Left, layer.DrawBounds.Top), Color.White, (0, 0)); 
                _drawBuffer[1] = new Vertex((layer.DrawBounds.Left + layer.DrawBounds.Width, layer.DrawBounds.Top), Color.White, (layer.RenderTexture.Size.X, 0)); 
                _drawBuffer[2] = new Vertex((layer.DrawBounds.Left, layer.DrawBounds.Top + layer.DrawBounds.Height), Color.White, (0, layer.RenderTexture.Size.Y)); 
                //BCD
                _drawBuffer[3] = new Vertex((layer.DrawBounds.Left + layer.DrawBounds.Width, layer.DrawBounds.Top), Color.White, (layer.RenderTexture.Size.X, 0));  
                _drawBuffer[4] = new Vertex((layer.DrawBounds.Left + layer.DrawBounds.Width, layer.DrawBounds.Top + layer.DrawBounds.Height), Color.White, (layer.RenderTexture.Size.X, layer.RenderTexture.Size.Y)); 
                _drawBuffer[5] = new Vertex((layer.DrawBounds.Left, layer.DrawBounds.Top + layer.DrawBounds.Height), Color.White, (0, layer.RenderTexture.Size.Y)); 

                Window.Draw(_drawBuffer, PrimitiveType.Triangles, new RenderStates(layer.RenderTexture.Texture));
            }

            TextHandler.FrameUpdate(this);

            Window.Display();
        }

        public void Draw(Vertex[] vertices, uint vCount, int layer = 0)
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

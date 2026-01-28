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

            for (int i = 0; i < RenderLayers.Length; i++)
            {
                switch (RenderLayers[i].RenderTarget)
                {
                    case RenderWindow win:
                    
                    break;

                    case RenderTexture rt:
                        ref var layer = ref RenderLayers[i];
                        ref var region = ref BufferRegions[layer.BufferRegion];

                        rt.Clear(layer.ClearColor);

                        _vertexBuffer.Draw(rt, region.Offset, region.Count, layer.RenderStates);
                        region.Count = 0; //Reset buffer region.

                        rt.Display();

                        //ABDC
                        _drawBuffer[0] = new Vertex((layer.DrawBounds.Left, layer.DrawBounds.Top), Color.White, 
                                                    (0, 0)); 
                        _drawBuffer[1] = new Vertex((layer.DrawBounds.Left + layer.DrawBounds.Width, layer.DrawBounds.Top), Color.White, 
                                                    (rt.Texture.Size.X, 0)); 
                        _drawBuffer[2] = new Vertex((layer.DrawBounds.Left, layer.DrawBounds.Top + layer.DrawBounds.Height), Color.White, 
                                                    (0, rt.Texture.Size.Y)); 
                        _drawBuffer[3] = new Vertex((layer.DrawBounds.Left + layer.DrawBounds.Width, layer.DrawBounds.Top + layer.DrawBounds.Height), Color.White, 
                                                    (rt.Texture.Size.X, rt.Texture.Size.Y)); 

                        layer.RenderStates.Texture = rt.Texture;
                        Window.Draw(_drawBuffer, PrimitiveType.TriangleStrip, layer.RenderStates);
                    break;
                }
            }

            TextHandler.FrameUpdate(this);

            Window.Display();
        }

        //Draw to screen. 
        public void DrawBuffer(Vertex[] vertices, uint vCount, int layer = 0)
        {
            var region = BufferRegions[RenderLayers[layer].BufferRegion];
            if (vCount > region.Count) vCount = region.Count;
            _vertexBuffer.Update(vertices, vCount, region.Offset);
            region.Count += vCount;
        }

        public void DrawRect(Vector2f a, Vector2f b, Color color, int layer = 0)
        {
            //ABD
            _drawBuffer[0] = new Vertex(a, color);
            _drawBuffer[1] = new Vertex((b.X, a.Y), color);
            _drawBuffer[2] = new Vertex((a.X, b.Y), color);
            //BCD
            _drawBuffer[3] = new Vertex((b.X, a.Y), color);
            _drawBuffer[4] = new Vertex((b.X, b.Y), color);
            _drawBuffer[5] = new Vertex((a.X, b.Y), color);
            
            DrawBuffer();
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
            //BCD
            _drawBuffer[3] = new Vertex((rec.Left + rec.Width, rec.Top), dat.Color, dat.Sprite.TopRight);
            _drawBuffer[4] = new Vertex((rec.Left + rec.Width, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomRight);
            _drawBuffer[5] = new Vertex((rec.Left, rec.Top + rec.Height), dat.Color, dat.Sprite.BottomLeft);

            DrawBuffer(_drawBuffer, 6);
        }

        //public void DrawRect(in KDrawData dat, in KRectangle rec, int layer = 0)
        //{
        //    //ABD
        //    _drawBuffer[0] = new Vertex(rec.TopLeft, dat.Color, dat.Sprite.TopLeft);
        //    _drawBuffer[1] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
        //    _drawBuffer[2] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);
        //    //BCD
        //    _drawBuffer[3] = new Vertex(rec.TopRight, dat.Color, dat.Sprite.TopRight);
        //    _drawBuffer[4] = new Vertex(rec.BottomRight, dat.Color, dat.Sprite.BottomRight);
        //    _drawBuffer[5] = new Vertex(rec.BottomLeft, dat.Color, dat.Sprite.BottomLeft);

        //    RenderLayers[layer].(_drawBuffer, 6);
        //}

        private void ResizeView(object? _, SizeEventArgs e)
        {
            _view.Size = (Vector2f)e.Size;
            _view.Center = _view.Size / 2;
            Window.SetView(_view);
        }
    }
}

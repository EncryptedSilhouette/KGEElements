using Elements.Rendering;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Mouse;

namespace Elements.Game
{
    public class KCameraCrontroller
    {
        private float _zoomLevel = 1f;
        private float _zoomStrength = 0.1f;

        public float resolutionX;
        public float resolutionY;

        public int PanSpeed = 16;
        public int PanBorderSize = 8;
        public View View;

        public KCameraCrontroller(View view)
        {
            View = view;
        }

        public void Init(KRenderManager renderManager)
        {

        }

        public void Update()
        {

        }

        public void FrameUpdate(KInputManager inputManager, KRenderManager renderManager)
        {
            ZoomCamera(inputManager, renderManager, in renderManager.DrawLayers[0]);
            PanCamera(inputManager, renderManager);
        }

        public void PanCamera(KInputManager inputManager, KRenderManager renderManager)
        {
            Vector2f panAmount = new(0, 0);

            if (inputManager.MousePosX < PanBorderSize)
            {
                panAmount.X -= PanSpeed;
            }
            else if (inputManager.MousePosX > renderManager.Window.Size.X - PanBorderSize)
            {
                panAmount.X += PanSpeed;
            }

            if (inputManager.MousePosY < PanBorderSize)
            {
                panAmount.Y -= PanSpeed;
            }
            else if (inputManager.MousePosY > renderManager.Window.Size.Y - PanBorderSize)
            {
                panAmount.Y += PanSpeed;
            }

            View.Move(panAmount);
            renderManager.DrawLayers[0].RenderTexture.SetView(View);
        }

        public void ZoomCamera(KInputManager inputManager, KRenderManager renderManager, in KDrawLayer layer)
        {
            if (inputManager.ScrollDelta == 0) return;

            _zoomLevel += (inputManager.ScrollDelta > 0) ? -_zoomStrength : _zoomStrength;
            if (_zoomLevel <= 0) _zoomLevel = 0.1f;

            var baseRes = layer.RenderTexture.Size;
            var newSize = new Vector2f(_zoomLevel * renderManager.Window.Size.X, _zoomLevel * renderManager.Window.Size.Y);

            var screenRatio = new Vector2f
            {
                X = (float) inputManager.MousePosX / renderManager.Window.Size.X,
                Y = (float) inputManager.MousePosY / renderManager.Window.Size.Y,
            };

            Console.WriteLine($"A:{inputManager.MousePosX}, {inputManager.MousePosY}.");

            var mouse = new Vector2f
            {
                X = View.Size.X * screenRatio.X,
                Y = View.Size.Y * screenRatio.Y,
            };

            Console.WriteLine($"B:{mouse.X}, {mouse.Y}.");

            View.Size = newSize;
            View.Center = new Vector2f 
            {
                X = View.Size.X * screenRatio.X,
                Y = View.Size.Y * screenRatio.Y,
            };

            layer.RenderTexture.SetView(View);
        }

        public void ResetCamera()
        {
            View = KProgram.RenderManager.Window.DefaultView;
            _zoomLevel = 1;
        }

        //public void RotateCamera(float angle)
        //{
        //    View.Rotate(angle);
        //}

        //public void ResetCamera()
        //{
        //    View.Center = (0, 0);
        //    View.Rotation = 0;
        //    View.Size = new Vector2f(800, 600);
        //}
    }
}

using Elements.Rendering;
using SFML.Graphics;
using SFML.System;

namespace Elements.Game
{
    public class KCameraCrontroller
    {
        private float _zoomLevel = 1.1f;
        private float _zoomStrength = 1.1f;

        public float ZoomSpeed = 50f;
        public int PanSpeed = 16;
        public int PanBorderSize = 4;
        public View View;

        public KCameraCrontroller(View cameraView)
        {
            _zoomLevel = _zoomStrength;
            View = cameraView;
        }

        public void Init(KRenderManager renderManager)
        {
            View.Center = (0, 0);
        }

        public void Update()
        {

        }

        public void FrameUpdate(KInputManager inputManager, KRenderManager renderManager)
        {
            ZoomCamera(inputManager, renderManager);
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
            Console.WriteLine(View);
        }

        public void ZoomCamera(KInputManager inputManager, KRenderManager renderManager)
        {
            if (inputManager.ScrollDelta == 0) return;

            for (int i = 0; i < MathF.Abs(inputManager.ScrollDelta); i++)
            {
                if (inputManager.ScrollDelta >= 1 && _zoomLevel < 4) 
                {
                    _zoomLevel *= _zoomStrength;
                    renderManager.View.Zoom(_zoomStrength);
                }
                else if (inputManager.ScrollDelta <= -1 && _zoomLevel > 0.5f)
                {
                    _zoomLevel /= _zoomStrength;
                    renderManager.View.Zoom(1 / _zoomStrength);
                }
            }
        }

        public void RotateCamera(float angle)
        {
            View.Rotate(angle);
        }

        public void ResetCamera()
        {
            View.Center = (0, 0);
            View.Rotation = 0;
            View.Size = new Vector2f(800, 600);
        }
    }
}

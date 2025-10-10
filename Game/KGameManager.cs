using Elements.Dev;
using Elements.Drawing;
using SFML.System;
using System.Diagnostics;
using static SFML.Window.Keyboard;

namespace Elements.Game
{
    [Flags]
    public enum KGameFlags
    {
        DEBUG = 1 << 0,
    }

    public class KGameManager
    {
        public const int GAME_CAMERA = 0;
        public const int TILE_MAP_LAYER = 0;

        public KDebugger Debugger;
        public KInputManager InputManager;
        public KGameFlags GameFlags;
        public KMapManager MapManager;
        public KRenderManager DrawManager;

        public KGameManager(KRenderManager renderManager, KInputManager inputManager) 
        {
            Debugger = new();
            InputManager = inputManager;
            DrawManager = renderManager;
            MapManager = new(this);
        }

        public void Update(in uint currentUpdate)
        {
            if (InputManager.IsKeyPressed(Key.LAlt) && InputManager.IsKeyPressed(Key.D))
            {
                if (GameFlags.HasFlag(KGameFlags.DEBUG)) GameFlags &= ~KGameFlags.DEBUG;
                else GameFlags |= KGameFlags.DEBUG;
            }
        }

        public void FrameUpdate(KRenderManager renderManager, in uint currentUpdate, in uint currentFrame, in double deltaTime)
        {
            HandleScreenPanning(renderManager, deltaTime);
            Debugger.FrameUpdate(renderManager);
        }

        //Call in FrameUpdate
        public void HandleScreenPanning(KRenderManager renderManager, in double deltaTime)
        {
            Vector2f moveOffset = new(); 

            if (InputManager.IsKeyDown(Key.W))
            {
                moveOffset.Y += -1;
            }
            if (InputManager.IsKeyDown(Key.S))
            {
                moveOffset.Y += 1;
            }
            if (InputManager.IsKeyDown(Key.A))
            {
                moveOffset.X += -1;
            }
            if (InputManager.IsKeyDown(Key.D))
            {
                moveOffset.X += 1;
            }

            renderManager.Cameras[GAME_CAMERA].Move(moveOffset * (float) deltaTime);
        }
    }
}

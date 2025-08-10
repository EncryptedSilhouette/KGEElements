namespace Elements.Core.Systems
{
    public enum KGameState
    {
        TITLE,
    }

    public class KGameManager
    {
        KPlayer Player;

        public KGameManager() 
        {
            Player = new KPlayer()
            {
                Transform = new()
                {
                    PosX = 0,
                    PosY = 0,
                    ScaleX = 1,
                    ScaleY = 2,
                    Rotation = 0,
                },
                DrawData = new()
                {
                    Color = SFML.Graphics.Color.White,
                    Sprite = 
                }
            };
        }

        public void Update(in uint currentUpdate)
        {

        }

        public void FrameUpdate(in uint currentUpdate)
        {

        }
    }

    public struct KPlayer
    {
        public KTransform Transform;
        public KDrawData DrawData;

        public float Speed;

        public void Update(in uint currentUpdate)
        {
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.A)) Transform.PosX -= 1;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.D)) Transform.PosX += 1;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.W)) Transform.PosY -= 1;
            if (KProgram.InputManager.IsKeyDown(SFML.Window.Keyboard.Key.S)) Transform.PosY += 1;
        }

        public void FrameUpdate(in uint currentUpdate, in uint currentFrame)
        {
            KProgram.WindowManager.DrawHandler.SubmitDraw(0, DrawData, Transform);
        }
    }
}

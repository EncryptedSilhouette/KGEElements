using Elements.Game;
using SFML.Graphics;
using SFML.System;

namespace Elements.Rendering
{
    public struct KRenderCamera
    {
        public Vector2u DisplayResolution;
        public Vector2u ScaledResolution;
        public Vector2u AspectRatio;
        public View View;

        public KRenderCamera(View view)
        {
            View = view;
        }
    }
}

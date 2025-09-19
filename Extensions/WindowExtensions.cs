using SFML.Graphics;

namespace Elements.Extensions
{
    public static class WindowExtensions
    {
        public static RenderTexture CreateRenderTexture(this RenderWindow window) => new(window.Size.X, window.Size.Y);
    }
}

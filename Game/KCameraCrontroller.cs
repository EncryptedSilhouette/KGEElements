using Elements.Core;
using Elements.Rendering;
using SFML.Graphics;

namespace Elements.Game
{
    public class KCameraCrontroller
    {
        public KRenderManager RenderManager;
        public View View;
        //public KTransform Transform => ;

        public KCameraCrontroller(KRenderManager renderManager, View view)
        {
            RenderManager = renderManager;
            View = new View(view);
        }

    }
}

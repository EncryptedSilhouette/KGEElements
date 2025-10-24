using Elements.Drawing;
using SFML.Graphics;
using System.Drawing;

namespace Elements.Core
{
    public class KText
    {
        public KRectangle Bounds;
        public string Text;
        public Font Font;
        public KDrawData DrawData;

        public KText(string text)
        {
            Text = text;
        }

        public void FrameUpdate(KRenderManager renderManager)
        {
            renderManager.SubmitDraw(DrawData, Bounds);
        }
    }
}

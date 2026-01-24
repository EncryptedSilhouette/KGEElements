using SFML.Graphics;
using SFML.System;

namespace Elements.Rendering
{
    public record struct KTextBox(FloatRect Bounds, in KText Text);
    
    public struct KText
    {    
        public uint VertexCount;
        public string Text;
        public Vertex[] VertexBuffer; 

        public KText()
        {
            Text = string.Empty;
            VertexCount = 0;
            VertexBuffer = [];
        }

        public KText(string text, Vertex[] vBuffer)
        {
            Text = text;
            VertexCount = 0;
            VertexBuffer = vBuffer;
        }
    }
}

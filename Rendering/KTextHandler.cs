using System.Buffers;
using Elements.Rendering;
using SFML.Graphics;
using SFML.System;

public record struct KGlyphHandle(char Character, byte FontSize, bool Bold, byte LineThickness);

public class KTextHandler
{

    #region NULL QUARANTINE
    //DANGER NULLABLE AHEAD!!!
    //DO NOT MESS WITH THIS.
    private Font? _currentFont;

    public bool GetFont(out Font? font)
    {
        font = _currentFont;
        return font is not null;
    }

    public void SetFont(Font font) => _currentFont = font;

    #endregion 

    private Dictionary<KGlyphHandle, Glyph> _glyphCache = new(128);

    private  Dictionary<Font, Texture> _glyphset = new();

    private Dictionary<KGlyphHandle, Glyph> GlyphCache 
    { 
        get => _glyphCache;
        set 
        {
            //OnFontChange?.Invoke(value, _glyphCache);
            _glyphCache = value;
        } 
    }

    public Action<KTextHandler>? OnFontChange;

    public ArrayPool<Vertex> VertexArrayPool => ArrayPool<Vertex>.Shared;

    public KTextHandler(KRenderManager renderer)
    {
        _currentFont = null; //God have mercy on our soul. 
    }

    public KTextHandler(KRenderManager renderer, Font font) : this(renderer)
    {

    }

    public void StartTextDraw()
    {
        
    }

    public void EndTextDraw()
    {
        
    }
}

public struct KTextBox
{
    public FloatRect Bounds;
    public Vertex[] vBuffer;

    public KTextBox(in KText text, Font font, Vector2f position, byte fontSize = 12, int wrapThreshold = 0)
    {
        Bounds = new FloatRect(position, (0,0));

        if (string.IsNullOrEmpty(text.Text))
        {
            vBuffer = [];
            return;
        }

        var chars = text.Text.AsSpan();
        vBuffer = new Vertex[text.Text.Length * 6];

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == '\n')
            {
                
                Bounds.Position.X = 0;
                Bounds.Position.Y -= fontSize;
                continue;    
            }

            KGlyphHandle handle = new(chars[i], fontSize, false, 0);

            if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
            {
                glyph = _currentFont.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
                _glyphCache.Add(handle, glyph);
#if DEBUG
                KProgram.LogManager.DebugLog($"Glyph cached: {chars[i]}");
#endif
            }

            if (wrapThreshold > 0 && Bounds.Size.X + glyph.Advance > wrapThreshold)
            {
                Bounds.Position.X = 0;
                Bounds.Position.Y -= fontSize;
            }

            //Old (SFML 2.6.2)
            //A buffer[i * 4] = new()
            //{
            //    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
            //                position.Y + bounds.Position.Y + glyph.Bounds.Top),
            //    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top),
            //    Color = text.Color,
            //};
            //B buffer[i * 4 + 1] = new()
            //{
            //    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
            //                position.Y + bounds.Position.Y + glyph.Bounds.Top),
            //    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
            //    Color = text.Color,
            //};
            //C buffer[i * 4 + 2] = new()
            //{
            //    Position = (position.X + bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
            //                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
            //    TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top + glyph.TextureRect.Height),
            //    Color = text.Color,
            //};
            //D buffer[i * 4 + 3] = new()
            //{
            //    Position = (position.X + bounds.Position.X + glyph.Bounds.Left,
            //                position.Y + bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
            //    TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
            //    Color = text.Color,
            //};

            //ABC
            buffer[i * 6] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top),
                TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top),
                Color = text.Color,
            };
            buffer[i * 6 + 1] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top),
                TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                Color = text.Color,
            };
            buffer[i * 6 + 2] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                Color = text.Color,
            };
            //BCD
            buffer[i * 6 + 3] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top),
                TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top),
                Color = text.Color,
            };
            buffer[i * 6 + 4] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left + glyph.Bounds.Width,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                TexCoords = (glyph.TextureRect.Left + glyph.TextureRect.Width, glyph.TextureRect.Top + glyph.TextureRect.Height),
                Color = text.Color,
            };
            buffer[i * 6 + 5] = new()
            {
                Position = (position.X + Bounds.Position.X + glyph.Bounds.Left,
                            position.Y + Bounds.Position.Y + glyph.Bounds.Top + glyph.Bounds.Height),
                TexCoords = (glyph.TextureRect.Left, glyph.TextureRect.Top + glyph.TextureRect.Height),
                Color = text.Color,
            };
            Bounds.Position.X += glyph.Advance;
        }
    }
}

#if false
    public KTextBox CreateTextbox(in KText text, Font font, Vector2f position, uint fontSize = 12, int wrapThreshold = 0)
    {
        bool pass = false;
        float width = 0;
        float height = 0;
        float xoffset = 0;
        var chars = text.Text.AsSpan();

        posY += fontSize;

        for (int i = 0, cp = 0; i < chars.Length; i++)
        {
            GlyphHandle handle = new(chars[i], (byte)fontSize, text.Bold, text.LineThickness);

            //Glyph caching
            if (!_glyphCache.TryGetValue(handle, out Glyph glyph))
            {
                glyph = font.GetGlyph(chars[i], fontSize, text.Bold, text.LineThickness);
                _glyphCache.Add(handle, glyph);

            }

            if (chars[i] == '\n')
            {
                buffer[i * 4] = new();
                buffer[i * 4 + 1] = new();
                buffer[i * 4 + 2] = new();
                buffer[i * 4 + 3] = new();

                xoffset = 0;
                height += fontSize;

                continue;
            }
            if (chars[i] == ' ')
            {
                cp = i + 1;
                pass = false;
            }
            else if (!pass && wrapThreshold != 0 && xoffset != 0 && xoffset + glyph.Advance > wrapThreshold)
            {
                i = cp;
                xoffset = 0;
                height += fontSize;
                pass = true;
            }

            var coords = glyph.TextureRect;
            var rect = glyph.Bounds;

            buffer[i * 4] = new()
            {
                Position = (posX + xoffset + rect.Left,
                            posY + height + rect.Top),
                TexCoords = (coords.Left, coords.Top),
                Color = text.Color,
            };
            buffer[i * 4 + 1] = new()
            {
                Position = (posX + xoffset + rect.Left + rect.Width,
                            posY + height + rect.Top),
                TexCoords = (coords.Left + coords.Width, coords.Top),
                Color = text.Color,
            };
            buffer[i * 4 + 2] = new()
            {
                Position = (posX + xoffset + rect.Left + rect.Width,
                            posY + height + rect.Top + rect.Height),
                TexCoords = (coords.Left + coords.Width, coords.Top + coords.Height),
                Color = text.Color,
            };
            buffer[i * 4 + 3] = new()
            {
                Position = (posX + xoffset + rect.Left,
                            posY + height + rect.Top + rect.Height),
                TexCoords = (coords.Left, coords.Top + coords.Height),
                Color = text.Color,
            };

            xoffset += (int)glyph.Advance;

            if (xoffset > width) width = xoffset;
        }

        posY -= fontSize;
        return new FloatRect
        {
            Position = (posX, posY), 
            Size = (width, height < 1 ? fontSize : height)
        };
            
    }
#endif
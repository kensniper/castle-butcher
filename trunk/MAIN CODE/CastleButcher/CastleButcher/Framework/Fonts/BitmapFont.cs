using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections;

namespace Framework.Fonts
{
    public enum Align { Left, Center, Right };
    /// <summary>Represents a single bitmap character set.</summary>
    class BitmapCharacterSet
    {
        public int LineHeight;
        public int Base;
        public int RenderedSize;
        public int Width;
        public int Height;
        //public BitmapCharacter[] Characters;
        public Hashtable Characters;

        /// <summary>Creates a new BitmapCharacterSet</summary>
        public BitmapCharacterSet()
        {
            //Characters = new BitmapCharacter[65536];
            Characters = new Hashtable(100);
            //for (int i = 0; i < 65536; i++)
            //{
            //    //Characters[i] = new BitmapCharacter();
            //}
        }
    }

    /// <summary>Represents a single bitmap character.</summary>
    public class BitmapCharacter : ICloneable
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;
        public int XAdvance;
        public List<Kerning> KerningList = new List<Kerning>();

        /// <summary>Clones the BitmapCharacter</summary>
        /// <returns>Cloned BitmapCharacter</returns>
        public object Clone()
        {
            BitmapCharacter result = new BitmapCharacter();
            result.X = X;
            result.Y = Y;
            result.Width = Width;
            result.Height = Height;
            result.XOffset = XOffset;
            result.YOffset = YOffset;
            result.XAdvance = XAdvance;
            result.KerningList.AddRange(KerningList);
            return result;
        }
    }

    /// <summary>Represents kerning information for a character.</summary>
    public class Kerning
    {
        public int Second;
        public int Amount;
    }
    /// <summary>Bitmap font wrapper.</summary>
    public class BitmapFont : IDisposable, IDeviceRelated,IComparable
    {
        private string m_name;
        private BitmapCharacterSet m_charSet;
        private string m_fntFile;
        private string m_textureFile;
        private Texture m_texture = null;
        private VertexBuffer m_vb = null;
        private const int MaxVertices = 1024;
        private int m_nextChar;

        
        /// <summary>Creates a new bitmap font.</summary>
        public BitmapFont(string fntFile)
        {
            m_fntFile = fntFile;
            m_charSet = new BitmapCharacterSet();
            ParseFNTFile();
        }

        /// <summary>Parses the FNT file.</summary>
        private void ParseFNTFile()
        {
            StreamReader stream = new StreamReader(m_fntFile);
            string line;
            char[] separators = new char[] { ' ', '=' };
            while ((line = stream.ReadLine()) != null)
            {
                string[] tokens = line.Split(separators);
                if (tokens[0] == "info")
                {
                    // Get rendered size
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i] == "size")
                        {
                            m_charSet.RenderedSize = int.Parse(tokens[i + 1]);
                        }
                        if (tokens[i] == "face")
                        {
                            m_name = tokens[i + 1].Substring(1,tokens[i+1].Length-2);
                        }
                    }
                }
                else if (tokens[0] == "common")
                {
                    // Fill out BitmapCharacterSet fields
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i] == "lineHeight")
                        {
                            m_charSet.LineHeight = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "base")
                        {
                            m_charSet.Base = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "scaleW")
                        {
                            m_charSet.Width = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "scaleH")
                        {
                            m_charSet.Height = int.Parse(tokens[i + 1]);
                        }
                    }
                }
                else if (tokens[0] == "page")
                {
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i] == "file")
                        {
                            m_textureFile = DefaultValues.FontPath+tokens[i + 1].Substring(1, tokens[i + 1].Length - 2);
                        }
                    }
                }
                else if (tokens[0] == "char")
                {
                    // New BitmapCharacter
                    BitmapCharacter character = new BitmapCharacter();
                    char index ;
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i] == "id")
                        {
                            index = (char)int.Parse(tokens[i + 1]);
                            m_charSet.Characters[index] = character;
                        }
                        else if (tokens[i] == "x")
                        {
                            character.X = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "y")
                        {
                            character.Y = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "width")
                        {
                            character.Width = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "height")
                        {
                            character.Height = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "xoffset")
                        {
                            character.XOffset = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "yoffset")
                        {
                            character.YOffset = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "xadvance")
                        {
                            character.XAdvance = int.Parse(tokens[i + 1]);
                        }
                    }
                }
                else if (tokens[0] == "kerning")
                {
                    // Build kerning list
                    char index;
                    Kerning k = new Kerning();
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i] == "first")
                        {
                            index = (char)int.Parse(tokens[i + 1]);
                            ((BitmapCharacter)m_charSet.Characters[index]).KerningList.Add(k);
                        }
                        else if (tokens[i] == "second")
                        {
                            k.Second = int.Parse(tokens[i + 1]);
                        }
                        else if (tokens[i] == "amount")
                        {
                            k.Amount = int.Parse(tokens[i + 1]);
                        }
                    }
                    
                }
            }
            stream.Close();
        }

        /// <summary>Call when the device is created.</summary>
        /// <param name="device">D3D device.</param>
        public void OnCreateDevice(Device device)
        {
            ImageInformation info = TextureLoader.ImageInformationFromFile(m_textureFile);
            Format f = info.Format;
            if (f == Format.L8 || f == Format.A8)
                f = Format.A8;
            else
                f = Format.Dxt3;
            m_texture = TextureLoader.FromFile(device, m_textureFile, 0, 0, 0, Usage.None, f, Pool.Managed, Filter.Linear, Filter.Linear, 0);
        }


        /// <summary>Call when the device is destroyed.</summary>
        public void OnDestroyDevice(Device device)
        {
            if (m_texture != null)
            {
                m_texture.Dispose();
                m_texture = null;
            }
        }

        /// <summary>Call when the device is reset.</summary>
        /// <param name="device">D3D device.</param>
        public void OnResetDevice(Device device)
        {
            m_vb = new VertexBuffer(device, MaxVertices * CustomVertex.TransformedColoredTextured.StrideSize, Usage.WriteOnly | Usage.Dynamic, CustomVertex.TransformedColoredTextured.Format, Pool.Default);
        }

        /// <summary>Call when the device is lost.</summary>
        public void OnLostDevice(Device device)
        {
            if (m_vb != null)
            {
                m_vb.Dispose();
                m_vb = null;
            }

        }
        public Texture Texture
        {
            get
            {
                return m_texture;
            }
        }
        public float Size
        {
            get
            {
                return m_charSet.RenderedSize;
            }
        }

        /// <summary>Renders the strings.</summary>
        /// <param name="device">D3D Device</param>
        public void Render(Device device, List<Quad> quads)
        {
            if (quads.Count == 0)
                return;
            // Add vertices to the buffer
            GraphicsStream gb =
                m_vb.Lock(0, 6 * quads.Count * CustomVertex.TransformedColoredTextured.StrideSize, LockFlags.Discard);

            Quad q;
            for (int i = 0; i < quads.Count; i++)
            {
                q = (Quad)quads[i].Clone();
                q.X -= 0.5f;
                q.Y -= 0.5f;
                gb.Write(q.Vertices);
            }

            m_vb.Unlock();

            // Set render states
            device.SetRenderState(RenderStates.ZBufferWriteEnable, false);
            device.SetRenderState(RenderStates.ZEnable, false);
            device.SetRenderState(RenderStates.Lighting, false);
            device.SetRenderState(RenderStates.AlphaBlendEnable, true);
            device.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            device.SetStreamSource(0, m_vb, 0, CustomVertex.TransformedColoredTextured.StrideSize);
            device.SetTexture(0, m_texture);
            device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2 * quads.Count);
        }



        private bool ClampToViewport(Quad q, RectangleF vp)
        {
            if (q.Right <= vp.Left || q.Top >= vp.Bottom || q.Left >= vp.Right || q.Bottom <= vp.Top)
                return false;   //quad completely out of viewport

            float xs = q.TWidth / (q.Width);
            float ys = q.THeight / (q.Height);
            if (q.Left < vp.Left)
            {
                q.Width -= (vp.Left - q.Left);
                q.X = vp.Left;
                q.TLeft = q.TRight - xs * q.Width;
            }
            if (q.Right > vp.Right)
            {
                q.Width -= (q.Right - vp.Right);
                q.TRight = q.TLeft + xs * q.Width;
            }

            if (q.Top < vp.Top)
            {
                q.Height -= (vp.Top - q.Top);
                q.Y = vp.Top;
                q.TTop = q.TBottom - ys * q.Height;
            }
            if (q.Bottom > vp.Bottom)
            {
                q.Height -= (q.Bottom - vp.Bottom);
                q.TBottom = q.TTop + ys * q.Height;
            }
            return true; //quad at least partially in viewport

        }
        public List<Quad> GetProcessedQuads(StringBlock b)
        {
            List<Quad> quads = new List<Quad>();
            if (b.Text == null)
                return quads;   //empty list
            
            string text = b.Text;

            float max_y = Math.Min(b.TextRect.Bottom, b.ViewportRect.Bottom);
            float x = b.TextRect.X;
            float y = b.TextRect.Y;

            float maxWidth = b.TextRect.Width;
            Align alignment = b.Alignment;
            float lineWidth = 0f;
            float sizeScale = b.Size / (float)m_charSet.LineHeight;
            char lastChar = new char();
            int lineNumber = 1;
            int wordNumber = 1;
            //int numWords = 0;
            int quadsInWord = 0;
            int quadsInLine = 0;
            //float wordWidth = 0f;
            bool firstCharOfLine = true;

            float z = 0f;
            float rhw = 1f;

            for (int i = 0; i < text.Length; i++)
            {
                //if (text[i] < 0 || text[i] > 255)
                //    continue;
                BitmapCharacter c = (BitmapCharacter)m_charSet.Characters[text[i]];
                if (c == null)
                    continue;
                float xOffset = c.XOffset * sizeScale;
                float yOffset = c.YOffset * sizeScale;
                float xAdvance = c.XAdvance * sizeScale;
                float width = c.Width * sizeScale;
                float height = c.Height * sizeScale;

                // Check vertical bounds
                if (y > max_y)
                {
                    break;
                }
                //if(

                //check other bounds


                // Newline
                if (text[i] == '\n' || text[i] == '\r' || (lineWidth >= maxWidth))
                {
                    if (alignment == Align.Left)
                    {
                        // Start at left
                        x = b.TextRect.X;
                    }
                    if (alignment == Align.Center)
                    {
                        // Start in center
                        x = b.TextRect.X + (maxWidth / 2f);
                    }
                    else if (alignment == Align.Right)
                    {
                        // Start at right
                        x = b.TextRect.Right;
                    }

                    y += m_charSet.LineHeight * sizeScale;
                    // Check vertical bounds

                    float offset = 0f;

                    if ((lineWidth + xAdvance >= maxWidth) && (wordNumber != 1))
                    {
                        // Next character extends past text box width
                        // We have to move the last word down one line
                        lineWidth = 0f;
                        i -= quadsInWord;
                        for (int j = quads.Count - quadsInWord; j < quads.Count; j++, i++)
                        {
                            if (alignment == Align.Left)
                            {
                                // Move current word to the left side of the text box

                                //quads[j].LineNumber++;
                                //quads[j].WordNumber = 1;
                                quads[j].X = x + ((BitmapCharacter)m_charSet.Characters[text[i]]).XOffset * sizeScale;
                                quads[j].Y = y + ((BitmapCharacter)m_charSet.Characters[text[i]]).YOffset * sizeScale;
                                x += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                lineWidth += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                if (b.Kerning && j != quads.Count - 1)
                                {
                                    m_nextChar = text[i + 1];
                                    Kerning kern = ((BitmapCharacter)m_charSet.Characters[text[i]]).KerningList.Find(FindKerningNode);
                                    if (kern != null)
                                    {
                                        x += kern.Amount * sizeScale;
                                        lineWidth += kern.Amount * sizeScale;
                                    }
                                }

                            }
                            else if (alignment == Align.Center)
                            {

                                // First move word down to next line
                                //quads[j].LineNumber++;
                                //quads[j].WordNumber = 1;
                                quads[j].X = x + ((BitmapCharacter)m_charSet.Characters[text[i]]).XOffset * sizeScale;
                                quads[j].Y = y + ((BitmapCharacter)m_charSet.Characters[text[i]]).YOffset * sizeScale;
                                x += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                lineWidth += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                offset += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale / 2f;
                                float kerning = 0f;
                                if (b.Kerning && j != quads.Count - 1)
                                {
                                    m_nextChar = text[i + 1];
                                    Kerning kern = ((BitmapCharacter)m_charSet.Characters[text[i]]).KerningList.Find(FindKerningNode);
                                    if (kern != null)
                                    {
                                        kerning = kern.Amount * sizeScale;
                                        x += kerning;
                                        lineWidth += kerning;
                                        offset += kerning / 2f;
                                    }
                                }

                            }
                            else if (alignment == Align.Right)
                            {

                                // Move character down to next line
                                //quads[j].LineNumber++;
                                //quads[j].WordNumber = 1;
                                quads[j].X = x + ((BitmapCharacter)m_charSet.Characters[text[i]]).XOffset * sizeScale;
                                quads[j].Y = y + ((BitmapCharacter)m_charSet.Characters[text[i]]).YOffset * sizeScale;
                                lineWidth += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                x += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                offset += ((BitmapCharacter)m_charSet.Characters[text[i]]).XAdvance * sizeScale;
                                float kerning = 0f;
                                if (b.Kerning && j != quads.Count - 1)
                                {
                                    m_nextChar = text[i + 1];
                                    Kerning kern = ((BitmapCharacter)m_charSet.Characters[text[i]]).KerningList.Find(FindKerningNode);
                                    if (kern != null)
                                    {
                                        kerning = kern.Amount * sizeScale;
                                        x += kerning;
                                        lineWidth += kerning;
                                        offset += kerning;
                                    }
                                }

                            }
                            //newLineLastChar = quads[j].Character;
                        }

                        // Make post-newline justifications
                        if (alignment == Align.Center || alignment == Align.Right)
                        {
                            // Justify the new line
                            for (int k = quads.Count - quadsInWord; k < quads.Count; k++)
                            {
                                quads[k].X -= offset;
                            }
                            x -= offset;

                            // Rejustify the line it was moved from
                            for (int k = quads.Count - quadsInLine; k < quads.Count - quadsInWord; k++)
                            {
                                quads[k].X += offset;
                            }
                        }
                        quadsInLine = quadsInWord;
                    }
                    else
                    {
                        // New line without any "carry-down" word
                        firstCharOfLine = true;
                        lineWidth = 0f;
                        quadsInLine = 0;
                        quadsInWord = 0;
                    }

                    wordNumber = 1;
                    lineNumber++;


                } // End new line check

                // Don't print these
                if (text[i] == '\n' || text[i] == '\r' || text[i] == '\t')
                {
                    continue;
                }


                // Set starting cursor for alignment
                if (firstCharOfLine)
                {
                    if (alignment == Align.Left)
                    {
                        // Start at left
                        x = b.TextRect.Left;
                    }
                    if (alignment == Align.Center)
                    {
                        // Start in center
                        x = b.TextRect.Left + (maxWidth / 2f);
                    }
                    else if (alignment == Align.Right)
                    {
                        // Start at right
                        x = b.TextRect.Right;
                    }
                }

                // Adjust for kerning
                float kernAmount = 0f;
                if (b.Kerning && !firstCharOfLine)
                {
                    m_nextChar = (char)text[i];
                    Kerning kern = ((BitmapCharacter)m_charSet.Characters[lastChar]).KerningList.Find(FindKerningNode);
                    if (kern != null)
                    {
                        kernAmount = kern.Amount * sizeScale;
                        x += kernAmount;
                        lineWidth += kernAmount;
                        //wordWidth += kernAmount;
                    }
                }



                // Create the vertices
                CustomVertex.TransformedColoredTextured topLeft = new CustomVertex.TransformedColoredTextured(
                    x + xOffset, y + yOffset, z, rhw, b.Color.ToArgb(),
                    (float)c.X / (float)m_charSet.Width,
                    (float)c.Y / (float)m_charSet.Height);

                CustomVertex.TransformedColoredTextured topRight = new CustomVertex.TransformedColoredTextured(
                    topLeft.X + width, topLeft.Y, z, rhw, b.Color.ToArgb(),
                    (float)(c.X + c.Width) / (float)m_charSet.Width,
                    topLeft.Tv);
                CustomVertex.TransformedColoredTextured bottomRight = new CustomVertex.TransformedColoredTextured(
                    topRight.X, topLeft.Y + height, z, rhw, b.Color.ToArgb(),
                    topRight.Tu,
                    (float)(c.Y + c.Height) / (float)m_charSet.Height);
                CustomVertex.TransformedColoredTextured bottomLeft = new CustomVertex.TransformedColoredTextured(
                    topLeft.X, bottomRight.Y, z, rhw, b.Color.ToArgb(),
                    topLeft.Tu,
                    bottomRight.Tv);

                // Create the quad
                Quad q = new Quad(topLeft, topRight, bottomLeft, bottomRight);

                quads.Add(q);

                quadsInLine++;
                quadsInWord++;


                if (text[i] == ' ')
                {
                    wordNumber++;
                    //wordWidth = 0f;
                    //numWords++;
                    quadsInWord = 0;
                }


                x += xAdvance;
                lineWidth += xAdvance;
                lastChar = text[i];
                //wordWidth += xAdvance;


                // Rejustify text
                if (alignment == Align.Center)
                {
                    // We have to recenter all Quads since we addded a 
                    // new character
                    float offset = xAdvance / 2f;
                    if (b.Kerning && !firstCharOfLine)
                    {
                        offset += kernAmount / 2f;
                    }
                    for (int j = quads.Count - quadsInLine; j < quads.Count; j++)
                    {
                        quads[j].X -= offset;
                    }
                    x -= offset;
                }
                else if (alignment == Align.Right)
                {
                    // We have to rejustify all Quads since we addded a 
                    // new character
                    float offset = xAdvance;
                    if (b.Kerning && !firstCharOfLine)
                    {
                        offset += kernAmount;
                    }
                    for (int j = quads.Count - quadsInLine; j < quads.Count; j++)
                    {
                        quads[j].X -= offset;
                    }
                    x -= offset;
                }
                firstCharOfLine = false;
            }
            for (int j = 0; j < quads.Count; j++)
            {
                if (!ClampToViewport(quads[j], b.ViewportRect))
                {
                    quads.RemoveAt(j);
                    j--;
                }
            }
            return quads;
        }

        public float GetStringWidth(string s, float size, bool kerning)
        {
            float width = 0;
            float scale = size / m_charSet.LineHeight;
            for (int i = 0; i < s.Length; i++)
            {

                //if (s[i] < 0 || s[i] > 255)
                //    continue;
                BitmapCharacter c = (BitmapCharacter)m_charSet.Characters[s[i]];
                if (c == null)
                    continue;
                width += (float)c.XAdvance * scale;
                if (kerning && i < s.Length - 1)
                {
                    foreach (Kerning kern in c.KerningList)
                    {
                        if (kern.Second == s[i + 1])
                        {
                            width += (float)kern.Amount * scale;
                            break;
                        }
                    }
                }
            }
            return width;
        }

        /// <summary>Search predicate used to find nodes in m_kerningList</summary>
        /// <param name="node">Current node.</param>
        /// <returns>true if the node's name matches the desired node name, false otherwise.</returns>
        private bool FindKerningNode(Kerning node)
        {
            return (node.Second == m_nextChar);
        }

        /// <summary>Gets the font texture.</summary>
        public Texture FontTexture
        {
            get { return m_texture; }
        }

        public bool ContainsChar(char c)
        {
            if (m_charSet.Characters[c]!= null )
                return true;
            else
                return false;
        }

        /// <summary>
        /// Font name
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }
        public string FntFile
        {
            get
            {
                return m_fntFile;
            }
        }

        public string BitmapFile
        {
            get
            {
                return m_textureFile;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_vb != null)
            {
                m_vb.Dispose();
                m_vb = null;
            }
            if (m_texture != null)
            {
                m_texture.Dispose();
                m_texture = null;
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return (int)(Size - ((BitmapFont)obj).Size);
        }

        #endregion
    }


    /// <summary>Individual string to load into vertex buffer.</summary>
    public struct StringBlock
    {
        public string Text;
        public RectangleF TextRect;
        public RectangleF ViewportRect;
        public Align Alignment;
        public float Size;
        public ColorValue Color;
        public bool Kerning;

        /// <summary>Creates a new StringBlock</summary>
        /// <param name="text">Text to render</param>
        /// <param name="textBox">Text box to constrain text</param>
        /// <param name="alignment">Font alignment</param>
        /// <param name="size">Font size in pixels(max height of line)</param>
        /// <param name="color">Color</param>
        /// <param name="kerning">true to use kerning, false otherwise.</param>
        public StringBlock(string text, RectangleF textRect, RectangleF viewRect, Align alignment,
            float size, ColorValue color, bool kerning)
        {
            Text = text;
            TextRect = textRect;
            ViewportRect = viewRect;
            Alignment = alignment;
            Size = size;
            Color = color;
            Kerning = kerning;
        }

        /// <summary>Creates a new StringBlock</summary>
        /// <param name="text">Text to render</param>
        /// <param name="textBox">Text box to constrain text</param>
        /// <param name="alignment">Font alignment</param>
        /// <param name="size">Font size in pixels(max height of line)</param>
        /// <param name="color">Color</param>
        /// <param name="kerning">true to use kerning, false otherwise.</param>
        public StringBlock(string text, RectangleF textRect, Align alignment,
            float size, ColorValue color, bool kerning)
        {
            Text = text;
            TextRect = textRect;
            ViewportRect = textRect;
            Alignment = alignment;
            Size = size;
            Color = color;
            Kerning = kerning;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Framework.Fonts
{
    /// <summary>Font wrapper</summary>
    class SimpleD3DFont
    {
        Microsoft.DirectX.Direct3D.Font m_font = null;
        private int m_size;

        public enum Align { Left, Center, Right, TopRight, TopLeft, BottomRight, BottomLeft };
        public SimpleD3DFont()
        {
            //Empty
        }
        /// <summary>
        /// A constructor that creates a new font
        /// </summary>
        /// <param name="device">Direct3D Device</param>
        /// <param name="faceName">Font face name</param>
        /// <param name="size">Size of the font</param>
        /// <param name="bold">true for bold text, false for normal text</param>
        /// <param name="italic">true for italic text, false for normal text</param> 
        public SimpleD3DFont(Device device, string faceName, int size, bool bold, bool italic)
        {
            m_size = size;
            m_font = new Microsoft.DirectX.Direct3D.Font(device, -size, 0,
            (bold) ? FontWeight.Bold : FontWeight.Normal, 1, italic,
            CharacterSet.Default, Precision.Default, FontQuality.Default,
            PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, faceName);
        }

        /// <summary>Prints 2D text.</summary>
        /// <param name="text">Text to print</param>
        /// <param name="xPosition">X position in window coordinates</param>
        /// <param name="yPosition">Y position in window coordinates</param>
        /// <param name="color">Color of the text</param>
        public void Print(Sprite sprite, string text, int xPosition, int yPosition,
            int textBoxWidth, int textBoxHeight, int color, Align alignment)
        {
            if (m_font == null)
            {
                return;
            }
            DrawTextFormat format = 0;
            if (textBoxWidth == 0)
            {
                format |= DrawTextFormat.NoClip;
            }
            else
            {
                format |= DrawTextFormat.WordBreak;
                switch (alignment)
                {
                    case Align.Left:
                        format |= DrawTextFormat.Left;
                        break;
                    case Align.Center:
                        format |= DrawTextFormat.Center;
                        break;
                    case Align.Right:
                        format |= DrawTextFormat.Right;
                        break;
                    case Align.TopRight:
                        format |= DrawTextFormat.Right | DrawTextFormat.Top;
                        break;
                    case Align.BottomRight:
                        format |= DrawTextFormat.Right | DrawTextFormat.Bottom;
                        break;
                    case Align.TopLeft:
                        format |= DrawTextFormat.Left | DrawTextFormat.Top;
                        break;
                    case Align.BottomLeft:
                        format |= DrawTextFormat.Left | DrawTextFormat.Bottom;
                        break;
                }
                if (textBoxHeight == 0)
                {
                    // A width is specified, but not a height.
                    // Make it seem like height is infinite
                    textBoxHeight = 2000;
                }
            }
            format |= DrawTextFormat.ExpandTabs;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xPosition, yPosition, textBoxWidth, textBoxHeight);
            m_font.DrawText(sprite, text, rect, (DrawTextFormat)format, color);
        }

        /// <summary>Prints 2D text in a simplified way</summary>
        /// <param name="text">Text to print</param>
        /// <param name="xPosition">X position in window coordinates</param>
        /// <param name="yPosition">Y position in window coordinates</param>
        /// <param name="color">Color of the text</param>
        public void Print(string text, int xPosition, int yPosition, int color)
        {
            Print(null, text, xPosition, yPosition, 0, 0, color, Align.Left);
        }

        /// <summary>Call after the device is reset.</summary>
        public void OnResetDevice(Device device)
        {
            if (m_font != null)
            {
                m_font.OnResetDevice();
            }
        }

        /// <summary>Call when the device is lost</summary>
        public void OnLostDevice()
        {
            if (m_font != null && !m_font.Disposed)
            {
                m_font.OnLostDevice();
            }
        }

        /// <summary>Call when the device is destrroyed</summary>
        public void OnDestroyDevice()
        {
            if (m_font != null)
            {
                m_font.Dispose();
                m_font = null;
            }
        }

        /// <summary>Gets the font size.</summary>
        public int Size
        {
            get { return m_size; }
        }

        public int LineHeight
        {
            get
            {
                return m_size;
            }
        }
        //poprawic
        public int GetLineHeight(string line)
        {
            return m_size;
        }
        public int GetLineWidth(string line)
        {
            return line.Length * m_size;
        }
    }
}

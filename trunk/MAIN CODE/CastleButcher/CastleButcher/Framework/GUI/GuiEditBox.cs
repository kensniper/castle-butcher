using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Framework.Fonts;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;


namespace Framework.GUI
{
    public delegate void EditBoxEventHandler(string text);
    public class GuiEditBox : GuiControl
    {
        #region Fields
        private string m_text;
        private Color m_color;
        private Align m_alignment;
        private List<Quad> m_textQuads = null;
        private List<Quad> m_normalBoxQuads = new List<Quad>();
        private List<Quad> m_disabledBoxQuads = new List<Quad>();
        private List<Quad> m_cursorQuad = new List<Quad>(1);
        private RectangleF m_textRect;


        //cursor related
        private PointF m_cursorPos;
        private int m_cursorOffset; //cursor position in number of letters
        private static float BlinkRate = 0.5f;//cursor shows/hides every 0.5 sec
        private float m_cursorTime = 0;
        private bool m_drawCursor = false;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Control's size and position</param>
        /// <param name="text">Control's text</param>
        /// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        public GuiEditBox(string text, RectangleF rect, float fontSize)
            : base(rect, GM.GetUniqueName())
        {
            m_text = "";
            m_fontSize = (fontSize == 0) ? DefaultValues.TextSize : fontSize;
            FontName = "Default";
            m_alignment = Align.Left;
            m_color = Color.Black;

            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());

            Text = text;
            BuildTextQuads();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Control's size and position</param>
        /// <param name="text">Control's text</param>
        /// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        /// <param name="fontName">Font</param>
        /// <param name="alignment">Text alignment</param>
        /// <param name="color">Text color</param>
        public GuiEditBox(string text, Rectangle rect, float fontSize,
            string fontName, Align alignment, Color color)
            : base(rect, GM.GetUniqueName())
        {
            m_text = "";
            m_fontSize = (fontSize == 0) ? DefaultValues.TextSize : fontSize;
            FontName = fontName;
            m_alignment = alignment;
            m_color = color;

            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());

            
            BuildTextQuads();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Control's unique name</param>
        /// <param name="rect">Control's size and position</param>
        /// <param name="text">Control's text</param>
        /// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        /// <param name="fontName">Font</param>
        /// <param name="alignment">Text alignment</param>
        /// <param name="color">Text color</param>
        public GuiEditBox(string name, string text, Rectangle rect, float fontSize,
            string fontName, Align alignment, Color color)
            : base(rect, name)
        {
            m_text = "";
            m_fontSize = (fontSize == 0) ? DefaultValues.TextSize : fontSize;
            FontName = fontName;
            m_alignment = alignment;
            m_color = color;

            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());

            Text = text;
            BuildTextQuads();
        }

        protected override void ProcessStyle(GUIStyle style)
        {

            base.ProcessStyle(style);

            ImageNode bottomB, leftB, rightB, topB, trC, tlC, brC, blC, windowBg, cursor;
            bottomB = style.GetNodeByName("EditBox").GetNodeByName("NormalBottomBorder");
            topB = style.GetNodeByName("EditBox").GetNodeByName("NormalTopBorder");
            leftB = style.GetNodeByName("EditBox").GetNodeByName("NormalLeftBorder");
            rightB = style.GetNodeByName("EditBox").GetNodeByName("NormalRightBorder");
            trC = style.GetNodeByName("EditBox").GetNodeByName("NormalTopRightCorner");
            tlC = style.GetNodeByName("EditBox").GetNodeByName("NormalTopLeftCorner");
            brC = style.GetNodeByName("EditBox").GetNodeByName("NormalBottomRightCorner");
            blC = style.GetNodeByName("EditBox").GetNodeByName("NormalBottomLeftCorner");
            windowBg = style.GetNodeByName("EditBox").GetNodeByName("NormalBackground");
            cursor = style.GetNodeByName("EditBox").GetNodeByName("NormalCursor");



            float left, right, top, bottom;
            left = leftB.Rectangle.Width;
            right = rightB.Rectangle.Width;
            top = topB.Rectangle.Height;
            bottom = bottomB.Rectangle.Height;


            m_textRect = new RectangleF();
            m_textRect.X = m_position.X + left;
            m_textRect.Y = m_position.Y + top;
            m_textRect.Width = m_size.Width - left - right;
            m_textRect.Height = m_size.Height - top - bottom;

            float line_width = GM.FontManager.GetFont(FontName, m_fontSize).GetStringWidth(m_text, m_fontSize, true);
            m_cursorPos = new PointF(m_textRect.X + line_width, m_textRect.Y);
            m_cursorOffset = m_text.Length;
            //m_cursorOffset = 0;

            float cursorH, cursorW;
            cursorW = cursor.Rectangle.Width;
            cursorH = m_fontSize;



            float tWidth = (float)style.ImageInfo.Width;
            float tHeight = (float)style.ImageInfo.Height;

            //Positioning Quads
            ImageNode i;
            RectangleF rect;
            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            #region Normal Borders
            //leftBorder
            i = leftB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topBorder
            i = topB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //rightBorder
            i = rightB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_textRect.Width + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width + right, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_textRect.Width + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width + right, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomBorder
            i = bottomB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Normal Corners
            //topLeftCorner
            i = tlC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topRightCorner
            i = trC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomRightCorner
            i = brC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomLeftCorner
            i = blC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Normal Background & Cursor
            //WindowBackground
            i = windowBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_textRect.Left, m_textRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_textRect.Right, m_textRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_textRect.Left, m_textRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_textRect.Right, m_textRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Cursor
            i = cursor;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_cursorPos.X, m_cursorPos.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_cursorPos.X + cursorW, m_cursorPos.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_cursorPos.X, m_cursorPos.Y + cursorH, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_cursorPos.X + cursorW, m_cursorPos.Y + cursorH, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_cursorQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion


            bottomB = style.GetNodeByName("Button").GetNodeByName("DisabledBottomBorder");
            topB = style.GetNodeByName("Button").GetNodeByName("DisabledTopBorder");
            leftB = style.GetNodeByName("Button").GetNodeByName("DisabledLeftBorder");
            rightB = style.GetNodeByName("Button").GetNodeByName("DisabledRightBorder");
            trC = style.GetNodeByName("Button").GetNodeByName("DisabledTopRightCorner");
            tlC = style.GetNodeByName("Button").GetNodeByName("DisabledTopLeftCorner");
            brC = style.GetNodeByName("Button").GetNodeByName("DisabledBottomRightCorner");
            blC = style.GetNodeByName("Button").GetNodeByName("DisabledBottomLeftCorner");
            windowBg = style.GetNodeByName("Button").GetNodeByName("DisabledBackground");

            #region Disabled Borders
            //leftBorder
            i = leftB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topBorder
            i = topB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //rightBorder
            i = rightB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_textRect.Width + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width + right, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_textRect.Width + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width + right, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomBorder
            i = bottomB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Disabled Corners
            //topLeftCorner
            i = tlC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topRightCorner
            i = trC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomRightCorner
            i = brC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_textRect.Width, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomLeftCorner
            i = blC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + bottom + top + m_textRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_textRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Disabled Background
            //WindowBackground
            i = windowBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_textRect.Left, m_textRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_textRect.Right, m_textRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_textRect.Left, m_textRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_textRect.Right, m_textRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion


            foreach (Quad q in m_normalBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }

            foreach (Quad q in m_disabledBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            m_cursorQuad[0].X += m_positionDelta.X;
            m_cursorQuad[0].Y += m_positionDelta.Y;
        }

        protected override void BuildTextQuads()
        {
            StringBlock b = new StringBlock(m_text, m_textRect, m_alignment, m_fontSize, ColorValue.FromColor(Color.Black), true);
            m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);

            foreach (Quad q in m_textQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
        }
        /// <summary>
        /// EditBoxes receive focus
        /// </summary>
        public override bool RecievesFocus
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets/sets control's position delta
        /// Position delta is the offset from a coordinate system in which
        /// control was created, to a coordinate system of the screen.
        /// <example>If you have a window, all controls in it are placed 
        /// relative to window's position.</example>
        /// </summary>
        public override PointF PositionDelta
        {
            get
            {
                return base.PositionDelta;
            }
            set
            {
                PointF delta = new PointF(value.X - m_positionDelta.X, value.Y - m_positionDelta.Y);
                foreach (Quad q in m_textQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_normalBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_cursorQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                base.PositionDelta = value;
            }
        }

        /// <summary>
        /// Changes controls PositionDelta
        /// </summary>
        /// <param name="xDelta">x delta</param>
        /// <param name="yDelta">y delta</param>
        public override void ChangePositionDelta(float xDelta, float yDelta)
        {
            base.ChangePositionDelta(xDelta, yDelta);

            foreach (Quad q in m_textQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_cursorQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            m_positionDelta.X += xDelta;
            m_positionDelta.Y += yDelta;
        }

        /// <summary>
        /// This method is called when control recieves focus.
        /// </summary>
        public override void OnBeginFocus()
        {
            base.OnBeginFocus();
            m_cursorTime = 0;
            m_drawCursor = true;
        }
        /// <summary>
        /// Fires then control's text has changed.
        /// </summary>
        public event EditBoxEventHandler OnTextChange;

        /// <summary>
        /// Gets or sets EditBox's text
        /// </summary>
        public virtual string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                string old = m_text;
                m_text = value;

                PointF p = new PointF(m_positionDelta.X + m_textRect.X, m_positionDelta.Y + m_textRect.Y);
                StringBlock b = new StringBlock(m_text, new RectangleF(p, m_textRect.Size), m_alignment, m_fontSize, ColorValue.FromColor(Color.Black), true);
                m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);

                //check cursor position
                if (m_cursorOffset > m_text.Length)
                {
                    m_cursorOffset = m_text.Length;
                    m_cursorPos.X = m_textRect.X + GM.FontManager.GetFont(FontName, m_fontSize).GetStringWidth(
                            m_text.Substring(0, m_cursorOffset), m_fontSize, true);
                }

                //if (old.CompareTo(m_text) != 0 && OnTextChange != null)
                //    OnTextChange(m_text);


            }
        }

        /// <summary>
        /// Renders control
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void Render(Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);

            //first the window
            GUIStyle style = GM.GUIStyleManager.GetStyleByName(m_styleName);
            m_cursorTime += elapsedTime;
            if (m_cursorTime >= BlinkRate)
            {
                m_drawCursor = !m_drawCursor;
                m_cursorTime = 0;
            }
            switch (m_state)
            {
                case State.Normal:
                    style.Render(device, m_normalBoxQuads);
                    if (HasFocus && m_drawCursor)
                        style.Render(device, m_cursorQuad);
                    break;
                case State.Disabled:
                    style.Render(device, m_disabledBoxQuads);
                    break;
            }
            //then the text
            GM.FontManager.GetFont(FontName,m_fontSize).Render(device, m_textQuads);
        }

        /// <summary>
        /// Processes mouse data.
        /// </summary>
        /// <param name="position">Mouse position</param>
        /// <param name="xDelta">x delta (in pixels)</param>
        /// <param name="yDelta">y delta (in pixels)</param>
        /// <param name="zDelta">Scroll delta (in ??)</param>
        /// <param name="pressedButtons">Pressed buttons (0-left button)</param>
        /// <param name="releasedButtons">Released buttons (0-left button)</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void OnMouse(PointF position, float xDelta, float yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

            if (pressedButtons[0] && m_textRect.Contains(position))
            {
                position.X -= m_textRect.X;
                m_cursorPos.X = m_textRect.X;
                int i = 0;
                float width = 0;
                BitmapFont font = GM.FontManager.GetFont(FontName, m_fontSize);
                if (m_text.Length > 0)
                    width = font.GetStringWidth(m_text.Substring(0, i + 1), m_fontSize, true);
                while (i < m_text.Length && (width < position.X))
                {

                    i++;
                    if (i != m_text.Length)
                        width = font.GetStringWidth(m_text.Substring(0, i + 1), m_fontSize, true);
                }
                if (i != m_text.Length)
                {
                    float width2 = font.GetStringWidth(m_text.Substring(0, i), m_fontSize, true);
                    if (position.X - width2 <= width - position.X)
                    {
                        m_cursorPos.X = m_textRect.X + width2;
                        m_cursorOffset = i;
                    }
                    else
                    {
                        m_cursorPos.X = m_textRect.X + width;
                        m_cursorOffset = i+1;
                    }
                }
                else
                {
                    m_cursorPos.X = m_textRect.X + width;
                    m_cursorOffset = i;
                }

                m_cursorQuad[0].X = m_cursorPos.X + m_positionDelta.X;
                m_cursorQuad[0].Y = m_cursorPos.Y + m_positionDelta.Y;
            }
        }

        /// <summary>
        /// Processes keyboard data.
        /// </summary>
        /// <param name="pressedKeys">List of pressed keys</param>
        /// <param name="releasedKeys">List of released keys (since last frame)</param>
        /// <param name="pressedChar">Last pressed key as a character</param>
        /// <param name="pressedKey">Last pressed key as int</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void OnKeyboard(List<Keys> pressedKeys, List<Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);

            if (pressedKey == 0 && pressedChar == 0)
                return;
            BitmapFont font = GM.FontManager.GetFont(FontName, m_fontSize);
            switch (pressedKey)
            {
                case (int)Keys.Left:
                    if (m_cursorOffset != 0)
                    {
                        m_cursorOffset--;
                        m_cursorPos.X = m_textRect.X + font.GetStringWidth(
                            m_text.Substring(0, m_cursorOffset), m_fontSize, true);

                    }
                    m_cursorTime = 0;
                    m_drawCursor = true;
                    break;
                case (int)Keys.Right:
                    if (m_cursorOffset != m_text.Length)
                    {
                        m_cursorOffset++;
                        m_cursorPos.X = m_textRect.X + font.GetStringWidth(
                            Text.Substring(0, m_cursorOffset), m_fontSize, true);
                    }
                    m_cursorTime = 0;
                    m_drawCursor = true;
                    break;
                case (int)Keys.Back:            //backspace
                    if (Text.Length >= 0 && m_cursorOffset > 0)
                    {
                        m_cursorOffset--;
                        m_cursorPos.X = m_textRect.X + font.GetStringWidth(
                            m_text.Substring(0, m_cursorOffset), m_fontSize, true);
                        Text = Text.Remove(m_cursorOffset, 1);
                        BuildTextQuads();
                    }
                    break;
                case (int)Keys.Delete:
                    if (m_text.Length >= 0 && m_cursorOffset != m_text.Length)
                    {
                        Text = Text.Remove(m_cursorOffset, 1);
                        BuildTextQuads();
                    }
                    break;
                default:
                    if (font.GetStringWidth(m_text + pressedChar, m_fontSize, true) < m_textRect.Width)
                    {
                        if (font.ContainsChar(pressedChar))
                        {
                            string old=Text;
                            Text = Text.Insert(m_cursorOffset, new string(pressedChar, 1));
                            if (old.CompareTo(Text) != 0)
                            {
                                BuildTextQuads();
                                m_cursorOffset++;
                                m_cursorPos.X = m_textRect.X + font.GetStringWidth(
                                    Text.Substring(0, m_cursorOffset), m_fontSize, true);
                                if (OnTextChange != null)
                                    OnTextChange(Text);
                            }
                            
                        }
                    }
                    break;
            }
            m_cursorQuad[0].X = m_cursorPos.X + m_positionDelta.X;
            m_cursorQuad[0].Y = m_cursorPos.Y + m_positionDelta.Y;
        }

    }
}

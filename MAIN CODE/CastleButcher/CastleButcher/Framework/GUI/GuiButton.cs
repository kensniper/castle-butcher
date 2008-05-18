using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Framework.Fonts;

namespace Framework.GUI
{
    public delegate void ButtonEventHandler();

    /// <summary>
    /// A button class.
    /// </summary>
    public class GuiButton : GuiControl
    {
        #region Fields
        private List<Quad> m_normalButtonQuads = new List<Quad>();
        private List<Quad> m_overButtonQuads = new List<Quad>();
        private List<Quad> m_downButtonQuads = new List<Quad>();
        private List<Quad> m_disabledButtonQuads = new List<Quad>();
        private List<Quad> m_textQuads = new List<Quad>();

        //button specific

        private string m_buttonText;
        private RectangleF m_textRect;

        #endregion

        public GuiButton(string buttonText, RectangleF buttonRect)
            : base(buttonRect, GM.GetUniqueName())
        {
            m_buttonText = buttonText;

            GUIStyle style = GM.GUIStyleManager.GetCurrentStyle();
            ProcessStyle(style);

            m_fontSize = m_textRect.Height;
            //StringBlock b = new StringBlock(m_buttonText, m_textRect, Align.Left,
            //    m_fontSize, ColorValue.FromColor(Color.Black), true);
            //m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);
            BuildTextQuads();
        }

        //public GuiButton(string name, string buttonText, RectangleF buttonRect)
        //    : base(buttonRect, name)
        //{
        //    m_buttonText = buttonText;

        //    GUIStyle style = GM.GUIStyleManager.GetCurrentStyle();
        //    ProcessStyle(style);
        //    //StringBlock b = new StringBlock(m_buttonText, m_textRect, Align.Left,
        //    //    m_textRect.Height, ColorValue.FromColor(Color.Black), true);
        //    //m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);
        //    BuildTextQuads();
        //}

        /// <summary>
        /// Fires when the button is pressed and then released
        /// </summary>
        public event ButtonEventHandler OnClick;



        protected override void BuildTextQuads()
        {
            StringBlock b = new StringBlock(m_buttonText, new RectangleF(m_textRect.X + PositionDelta.X, m_textRect.Y + PositionDelta.Y, m_textRect.Width, m_textRect.Height), Align.Center, m_fontSize, ColorValue.FromColor(Color.Black), true);
            m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);
        }


        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode bottomB, leftB, rightB, topB, trC, tlC, brC, blC, windowBg;
            bottomB = style.GetNodeByName("Button").GetNodeByName("NormalBottomBorder");
            topB = style.GetNodeByName("Button").GetNodeByName("NormalTopBorder");
            leftB = style.GetNodeByName("Button").GetNodeByName("NormalLeftBorder");
            rightB = style.GetNodeByName("Button").GetNodeByName("NormalRightBorder");
            trC = style.GetNodeByName("Button").GetNodeByName("NormalTopRightCorner");
            tlC = style.GetNodeByName("Button").GetNodeByName("NormalTopLeftCorner");
            brC = style.GetNodeByName("Button").GetNodeByName("NormalBottomRightCorner");
            blC = style.GetNodeByName("Button").GetNodeByName("NormalBottomLeftCorner");
            windowBg = style.GetNodeByName("Button").GetNodeByName("NormalBackground");


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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Normal Background
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
            m_normalButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion

            bottomB = style.GetNodeByName("Button").GetNodeByName("OverBottomBorder");
            topB = style.GetNodeByName("Button").GetNodeByName("OverTopBorder");
            leftB = style.GetNodeByName("Button").GetNodeByName("OverLeftBorder");
            rightB = style.GetNodeByName("Button").GetNodeByName("OverRightBorder");
            trC = style.GetNodeByName("Button").GetNodeByName("OverTopRightCorner");
            tlC = style.GetNodeByName("Button").GetNodeByName("OverTopLeftCorner");
            brC = style.GetNodeByName("Button").GetNodeByName("OverBottomRightCorner");
            blC = style.GetNodeByName("Button").GetNodeByName("OverBottomLeftCorner");
            windowBg = style.GetNodeByName("Button").GetNodeByName("OverBackground");

            #region Over Borders
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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Over Corners
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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Over Background
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
            m_overButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion

            bottomB = style.GetNodeByName("Button").GetNodeByName("DownBottomBorder");
            topB = style.GetNodeByName("Button").GetNodeByName("DownTopBorder");
            leftB = style.GetNodeByName("Button").GetNodeByName("DownLeftBorder");
            rightB = style.GetNodeByName("Button").GetNodeByName("DownRightBorder");
            trC = style.GetNodeByName("Button").GetNodeByName("DownTopRightCorner");
            tlC = style.GetNodeByName("Button").GetNodeByName("DownTopLeftCorner");
            brC = style.GetNodeByName("Button").GetNodeByName("DownBottomRightCorner");
            blC = style.GetNodeByName("Button").GetNodeByName("DownBottomLeftCorner");
            windowBg = style.GetNodeByName("Button").GetNodeByName("DownBackground");

            #region Down Borders
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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Down Corners
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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Down Background
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
            m_downButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
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
            m_disabledButtonQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion


            foreach (Quad q in m_normalButtonQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_overButtonQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_downButtonQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_disabledButtonQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
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
                foreach (Quad q in m_normalButtonQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overButtonQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downButtonQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledButtonQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_textQuads)
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
            foreach (Quad q in m_normalButtonQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overButtonQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downButtonQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledButtonQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_textQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            m_positionDelta.X += xDelta;
            m_positionDelta.Y += yDelta;
        }

        /// <summary>
        /// This method is called when mouse enters control's space.
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            m_state = State.Over;
        }

        /// <summary>
        /// This method is called when mouse leaves control's space.
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            m_state = State.Normal;

        }

        /// <summary>
        /// Renders control
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            //first the window
            GUIStyle style = GM.GUIStyleManager.GetStyleByName(m_styleName);
            switch (m_state)
            {
                case State.Normal:
                    style.Render(device, m_normalButtonQuads);
                    break;
                case State.Over:
                    style.Render(device, m_overButtonQuads);
                    break;
                case State.Down:
                    style.Render(device, m_downButtonQuads);
                    break;
                case State.Disabled:
                    style.Render(device, m_disabledButtonQuads);
                    break;
            }


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

            if (pressedButtons[0])
            {
                m_state = State.Down;
                LocksMouse = true;
            }
            if (m_state == State.Down && releasedButtons[0])
            {
                if (ContainsPoint(position))
                {
                    if (OnClick != null)
                        OnClick();
                }
                m_state = State.Normal;
                LocksMouse = false;
            }
        }
    }
}

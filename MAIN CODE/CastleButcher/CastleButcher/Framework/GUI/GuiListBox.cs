using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Framework.Fonts;
using System.Windows.Forms;


namespace Framework.GUI
{

    public delegate void ListEventHandler();

    internal struct ListItem
    {
        public ListItem(string text, Object data)
        {
            this.text = text;
            this.data = data;
        }
        public string text;
        public Object data;
    }
    public class GuiListBox : GuiControl
    {
        #region Fields
        List<Quad> m_normalBoxQuads = new List<Quad>();
        List<Quad> m_normalHighlightQuad = new List<Quad>(1);
        List<Quad> m_normalUpArrowQuad = new List<Quad>(1);
        List<Quad> m_normalDownArrowQuad = new List<Quad>(1);
        List<Quad> m_normalMarkerQuad = new List<Quad>(1);


        List<Quad> m_disabledBoxQuads = new List<Quad>();
        List<Quad> m_disabledHighlightQuad = new List<Quad>(1);
        List<Quad> m_disabledUpArrowQuad = new List<Quad>(1);
        List<Quad> m_disabledDownArrowQuad = new List<Quad>(1);
        List<Quad> m_disabledMarkerQuad = new List<Quad>(1);


        List<Quad> m_overUpArrowQuad = new List<Quad>(1);
        List<Quad> m_overDownArrowQuad = new List<Quad>(1);
        List<Quad> m_overMarkerQuad = new List<Quad>(1);

        List<Quad> m_downUpArrowQuad = new List<Quad>(1);
        List<Quad> m_downDownArrowQuad = new List<Quad>(1);
        List<Quad> m_downMarkerQuad = new List<Quad>(1);

        List<Quad> m_textQuads = new List<Quad>();

        //Rectangles
        RectangleF m_itemsRect;
        RectangleF m_scrollBgRect;
        RectangleF m_scrollMarkerRect;  //m_scrollBgRect - marker
        RectangleF m_upArrowRect;
        RectangleF m_downArrowRect;
        RectangleF m_markerRect;

        Color textColor;


        float m_itemHeight = DefaultValues.TextSize;
        int m_numVisibleItems = DefaultValues.ListVisibleItems;

        //List data
        List<ListItem> m_items = new List<ListItem>();
        int m_selectedItem = -1;  //-1 means no selection
        int m_startingItem = 0; //first visible item
        bool m_itemClicked = false;
        int m_clickedItem;
        //scroll
        bool m_scrollMoving = false;
        float m_initialScrollPosition;
        float m_scrollPosition = 0;
        float m_mouseScrollDelta;
        float m_arrowPressTime = 0;
        State m_upArrowState = State.Normal;
        State m_downArrowState = State.Normal;
        State m_markerState = State.Normal;
        #endregion

        /// <summary>
        /// Fires when list's selected item has changed.
        /// </summary>
        public ListEventHandler OnSelectionChange = null;

        public GuiListBox(RectangleF rect, int numVisibleItems)
            : base(rect, GM.GetUniqueName())
        {
            m_numVisibleItems = numVisibleItems;

            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());
        }
        //public GuiListBox(string name, RectangleF rect, int numVisibleItems)
        //    : base(rect, name)
        //{
        //    m_numVisibleItems = numVisibleItems;

        //    ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());
        //}



        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            textColor = style.GetNodeByName("ListBox").Defaults.Color;
            ImageNode bottomB, leftB, rightB, topB, trC, tlC, brC, blC, windowBg, scrollBg, highlight;
            bottomB = style.GetNodeByName("ListBox").GetNodeByName("NormalBottomBorder");
            topB = style.GetNodeByName("ListBox").GetNodeByName("NormalTopBorder");
            leftB = style.GetNodeByName("ListBox").GetNodeByName("NormalLeftBorder");
            rightB = style.GetNodeByName("ListBox").GetNodeByName("NormalRightBorder");
            trC = style.GetNodeByName("ListBox").GetNodeByName("NormalTopRightCorner");
            tlC = style.GetNodeByName("ListBox").GetNodeByName("NormalTopLeftCorner");
            brC = style.GetNodeByName("ListBox").GetNodeByName("NormalBottomRightCorner");
            blC = style.GetNodeByName("ListBox").GetNodeByName("NormalBottomLeftCorner");
            windowBg = style.GetNodeByName("ListBox").GetNodeByName("NormalBackground");
            scrollBg = style.GetNodeByName("ListBox").GetNodeByName("NormalScrollBody");
            highlight = style.GetNodeByName("ListBox").GetNodeByName("NormalHighlight");

            ImageNode upArrow, downArrow, marker;
            upArrow = style.GetNodeByName("ListBox").GetNodeByName("NormalScrollUp");
            downArrow = style.GetNodeByName("ListBox").GetNodeByName("NormalScrollDown");
            marker = style.GetNodeByName("ListBox").GetNodeByName("NormalScrollMarker");

            float left, right, top, bottom;
            left = leftB.Rectangle.Width;
            right = rightB.Rectangle.Width;
            top = topB.Rectangle.Height;
            bottom = bottomB.Rectangle.Height;


            m_itemsRect = new RectangleF(m_position.X + left, m_position.Y + top,
                m_size.Width - left - right - scrollBg.Rectangle.Width, m_size.Height - top - bottom);
            m_scrollBgRect = new RectangleF(m_itemsRect.Right, m_itemsRect.Top + upArrow.Rectangle.Height,
                scrollBg.Rectangle.Width, m_itemsRect.Height - upArrow.Rectangle.Height - downArrow.Rectangle.Height);
            m_scrollMarkerRect = new RectangleF(m_itemsRect.Right, m_itemsRect.Top + upArrow.Rectangle.Height,
                scrollBg.Rectangle.Width, 0);

            m_itemHeight = (m_numVisibleItems==0 )? DefaultValues.TextSize:m_itemsRect.Height / m_numVisibleItems;
            if (m_numVisibleItems == 0)
                m_numVisibleItems = (int)Math.Ceiling(m_itemsRect.Height / m_itemHeight);

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
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topBorder
            i = topB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //rightBorder
            i = rightB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_itemsRect.Width + left + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + right + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_itemsRect.Width + left + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + right + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomBorder
            i = bottomB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height,
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

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomRightCorner
            i = brC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomLeftCorner
            i = blC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_itemsRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Normal Background & Highlight
            //Window Background
            i = windowBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Highlight
            i = highlight;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top + m_itemHeight, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top + m_itemHeight, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalHighlightQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Scroll Background
            i = scrollBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Left, m_scrollBgRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Right, m_scrollBgRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Left, m_scrollBgRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Right, m_scrollBgRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion

            bottomB = style.GetNodeByName("ListBox").GetNodeByName("DisabledBottomBorder");
            topB = style.GetNodeByName("ListBox").GetNodeByName("DisabledTopBorder");
            leftB = style.GetNodeByName("ListBox").GetNodeByName("DisabledLeftBorder");
            rightB = style.GetNodeByName("ListBox").GetNodeByName("DisabledRightBorder");
            trC = style.GetNodeByName("ListBox").GetNodeByName("DisabledTopRightCorner");
            tlC = style.GetNodeByName("ListBox").GetNodeByName("DisabledTopLeftCorner");
            brC = style.GetNodeByName("ListBox").GetNodeByName("DisabledBottomRightCorner");
            blC = style.GetNodeByName("ListBox").GetNodeByName("DisabledBottomLeftCorner");
            windowBg = style.GetNodeByName("ListBox").GetNodeByName("DisabledBackground");
            scrollBg = style.GetNodeByName("ListBox").GetNodeByName("DisabledScrollBody");
            highlight = style.GetNodeByName("ListBox").GetNodeByName("DisabledHighlight");

            #region Disabled Borders
            //leftBorder
            i = leftB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topBorder
            i = topB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //rightBorder
            i = rightB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_itemsRect.Width + left + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + right + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_itemsRect.Width + left + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + right + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomBorder
            i = bottomB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height,
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

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomRightCorner
            i = brC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_itemsRect.Width + m_scrollBgRect.Width, m_position.Y + bottom + top + m_itemsRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomLeftCorner
            i = blC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + bottom + top + m_itemsRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_itemsRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Disabled Background & Highlight
            //WindowBackground
            i = windowBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Highlight
            i = highlight;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_itemsRect.Left, m_itemsRect.Top + m_itemHeight, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_itemsRect.Right, m_itemsRect.Top + m_itemHeight, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledHighlightQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Scroll Background
            i = scrollBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Left, m_scrollBgRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Right, m_scrollBgRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Left, m_scrollBgRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_scrollBgRect.Right, m_scrollBgRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion




            m_upArrowRect = new RectangleF(m_scrollBgRect.Left, m_scrollBgRect.Top - upArrow.Rectangle.Height, upArrow.Rectangle.Width, upArrow.Rectangle.Height);
            m_downArrowRect = new RectangleF(m_scrollBgRect.Left, m_scrollBgRect.Bottom,
                downArrow.Rectangle.Width, downArrow.Rectangle.Height);
            m_markerRect = new RectangleF(m_upArrowRect.Left, m_upArrowRect.Bottom, marker.Rectangle.Width, m_scrollBgRect.Height);

            #region Normal Scroll

            //Up Arrow
            i = upArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalUpArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Down Arrow
            i = downArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalDownArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Marker
            i = marker;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            upArrow = style.GetNodeByName("ListBox").GetNodeByName("OverScrollUp");
            downArrow = style.GetNodeByName("ListBox").GetNodeByName("OverScrollDown");
            marker = style.GetNodeByName("ListBox").GetNodeByName("OverScrollMarker");

            #region Over Scroll

            //Up Arrow
            i = upArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overUpArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Down Arrow
            i = downArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overDownArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Marker
            i = marker;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            upArrow = style.GetNodeByName("ListBox").GetNodeByName("DownScrollUp");
            downArrow = style.GetNodeByName("ListBox").GetNodeByName("DownScrollDown");
            marker = style.GetNodeByName("ListBox").GetNodeByName("DownScrollMarker");

            #region Down Scroll

            //Up Arrow
            i = upArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downUpArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Down Arrow
            i = downArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downDownArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Marker
            i = marker;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            upArrow = style.GetNodeByName("ListBox").GetNodeByName("DisabledScrollUp");
            downArrow = style.GetNodeByName("ListBox").GetNodeByName("DisabledScrollDown");
            marker = style.GetNodeByName("ListBox").GetNodeByName("DisabledScrollMarker");

            #region Disabled Scroll

            //Up Arrow
            i = upArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Left, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_upArrowRect.Right, m_upArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledUpArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Down Arrow
            i = downArrow;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Left, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_downArrowRect.Right, m_downArrowRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledDownArrowQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //Marker
            i = marker;
            rect = i.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion


            BuildScrollMarker();
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
                foreach (Quad q in m_normalBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }

                foreach (Quad q in m_normalHighlightQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_textQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledHighlightQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_normalMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_normalUpArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overUpArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downUpArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledUpArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_normalDownArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overDownArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downDownArrowQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledDownArrowQuad)
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
            foreach (Quad q in m_normalBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalHighlightQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }

            foreach (Quad q in m_textQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }

            foreach (Quad q in m_disabledBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledHighlightQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalUpArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overUpArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downUpArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledUpArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalDownArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overDownArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downDownArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledDownArrowQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            m_positionDelta.X += xDelta;
            m_positionDelta.Y += yDelta;
        }

        /// <summary>
        /// Adds item to list.
        /// </summary>
        /// <param name="text">Item's text</param>
        /// <param name="data">Item's additional data. Can be null</param>
        /// <returns>Item's index on list</returns>
        public int AddItem(string text, Object data)
        {
            m_items.Add(new ListItem(text, data));
            if (m_numVisibleItems < m_items.Count)
            {
                m_markerRect.Height = ((float)m_numVisibleItems / m_items.Count) * m_scrollBgRect.Height;
                m_scrollMarkerRect.Height = m_scrollBgRect.Height - m_markerRect.Height;
                BuildScrollMarker();
            }

            BuildTextQuads();
            return m_items.Count - 1;
        }

        /// <summary>
        /// Adds item with no additional data to list.
        /// </summary>
        /// <param name="text">Item's text</param>
        /// <returns>Item's index on list</returns>
        public int AddItem(string text)
        {
            return AddItem(text, null);
        }

        /// <summary>
        /// Removes an item from list.
        /// </summary>
        /// <param name="index">Item's index</param>
        public void RemoveItem(int index)
        {
            if (index >= 0 && index < m_items.Count)
            {
                m_items.RemoveAt(index);
                BuildTextQuads();
                if (m_numVisibleItems < m_items.Count)
                {
                    m_markerRect.Height = ((float)m_numVisibleItems / m_items.Count) * m_scrollBgRect.Height;
                    m_scrollMarkerRect.Height = m_scrollBgRect.Height - m_markerRect.Height;
                    BuildScrollMarker();
                }
            }
        }

        /// <summary>
        /// Removes an item from list.
        /// </summary>
        /// <param name="text">Item's text</param>
        /// <remarks>If there are multiple item's with this text, only
        /// the first is removed</remarks>
        public void RemoveItem(string text)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i].text.CompareTo(text) == 0)
                {
                    m_items.RemoveAt(i);
                    BuildTextQuads();
                    if (m_numVisibleItems < m_items.Count)
                    {
                        m_markerRect.Height = ((float)m_numVisibleItems / m_items.Count) * m_scrollBgRect.Height;
                        m_scrollMarkerRect.Height = m_scrollBgRect.Height - m_markerRect.Height;
                        BuildScrollMarker();
                    }
                    return;
                }
            }
        }
        /// <summary>
        /// Removes all items from list.
        /// </summary>
        public void RemoveAll()
        {
            m_items.Clear();
            BuildScrollMarker();
            BuildSelection();
            BuildTextQuads();
        }

        /// <summary>
        /// Selected item.
        /// </summary>
        public int SelectedItem
        {
            set
            {
                m_selectedItem = value;
                if (m_selectedItem < 0 || m_selectedItem >= m_items.Count)
                {
                    m_selectedItem = -1;
                    BuildSelection();
                }
            }
            get
            {
                return m_selectedItem;
            }

        }

        /// <summary>
        /// Selected item's text.
        /// </summary>
        public string SelectedItemText
        {
            //set
            //{
            //    if (m_selectedItem >= 0 && m_selectedItem < m_items.Count)
            //        m_items[m_selectedItem].text=value;
            //}
            get
            {
                if (m_selectedItem >= 0 && m_selectedItem < m_items.Count)
                    return m_items[m_selectedItem].text;
                else
                    return null;
            }
        }

        /// <summary>
        /// Selected item's data;
        /// </summary>
        public Object SelectedItemData
        {
            //set
            //{
            //    if (m_selectedItem >= 0 && m_selectedItem < m_items.Count)
            //        m_items[m_selectedItem].data=value;
            //}
            get
            {
                if (m_selectedItem >= 0 && m_selectedItem < m_items.Count)
                    return m_items[m_selectedItem].data;
                else
                    return null;
            }
        }

        protected override void BuildTextQuads()
        {
            m_textQuads.Clear();
            for (int i = m_startingItem; i < m_items.Count && i < m_startingItem + m_numVisibleItems; i++)
            {
                StringBlock b = new StringBlock(m_items[i].text, new RectangleF(m_itemsRect.Left + m_positionDelta.X,
                    m_itemsRect.Top + m_positionDelta.Y + (i - m_startingItem) * m_itemHeight, m_itemsRect.Width, m_itemHeight),
                    Align.Left, m_itemHeight, ColorValue.FromColor(textColor), true);
                m_textQuads.AddRange(GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b));
            }
        }

        private void BuildScrollMarker()
        {
            m_markerRect.Y = m_scrollMarkerRect.Top + m_scrollPosition * m_scrollMarkerRect.Height;

            foreach (Quad q in m_normalMarkerQuad)
            {
                q.Y = m_markerRect.Y + m_positionDelta.Y;
                q.Height = m_markerRect.Height;
            }
            foreach (Quad q in m_overMarkerQuad)
            {
                q.Y = m_markerRect.Y + m_positionDelta.Y;
                q.Height = m_markerRect.Height;
            }
            foreach (Quad q in m_downMarkerQuad)
            {
                q.Y = m_markerRect.Y + m_positionDelta.Y;
                q.Height = m_markerRect.Height;
            }
        }

        private void BuildSelection()
        {
            if (m_selectedItem == -1)
                return;     //nothing to do

            if (m_selectedItem >= m_startingItem && m_selectedItem < m_startingItem + m_numVisibleItems)
            {
                //selection is visible
                m_normalHighlightQuad[0].Y = m_positionDelta.Y + m_itemsRect.Top +
                    (m_selectedItem - m_startingItem) * m_itemHeight;

            }
        }

        private float ScrollPosition
        {
            get
            {
                return m_scrollPosition;
            }
            set
            {

                m_scrollPosition = value;
                if (m_scrollPosition < 0)
                    m_scrollPosition = 0;
                else if (m_scrollPosition > 1)
                    m_scrollPosition = 1;
                else
                {
                    float d = 0;
                    int di = 0;

                    for (int i = 1; i < m_items.Count - m_numVisibleItems + 1; i++)
                    {
                        if (m_scrollPosition >= (float)i / (m_items.Count - m_numVisibleItems))
                        {
                            d = (float)i / (m_items.Count - m_numVisibleItems);
                            di = i;

                        }
                        else
                        {
                            if ((float)i / (m_items.Count - m_numVisibleItems) - m_scrollPosition < m_scrollPosition - d)
                            {
                                d = (float)i / (m_items.Count - m_numVisibleItems);
                                di = i;
                            }
                            break;
                        }
                    }
                    m_scrollPosition = d;
                    m_startingItem = di;

                    BuildScrollMarker();
                    BuildSelection();
                    BuildTextQuads();
                }
            }
        }

        private int ScrollValue
        {
            get
            {
                return m_startingItem;
            }
            set
            {

                m_startingItem = value;

                if (m_startingItem >= m_items.Count - m_numVisibleItems + 1)
                {
                    m_startingItem = m_items.Count - m_numVisibleItems;
                }
                if (m_startingItem < 0)
                    m_startingItem = 0;

                if (m_items.Count > m_numVisibleItems)
                {
                    m_scrollPosition = (float)m_startingItem / (m_items.Count - m_numVisibleItems);
                }
                else
                {
                    m_scrollPosition = 0;
                }

                BuildScrollMarker();
                BuildSelection();
                BuildTextQuads();


            }
        }

        /// <summary>
        /// This method is called when mouse leaves control's space.
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            m_state = State.Normal;
            m_markerState = State.Normal;
            m_upArrowState = State.Normal;
            m_downArrowState = State.Normal;
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

            if (this.LocksMouse)
            {
                if (m_itemClicked && releasedButtons[0])
                {
                    float y = position.Y - m_itemsRect.Top;
                    int item = m_startingItem + (int)Math.Floor(m_numVisibleItems * (y / m_itemsRect.Height));
                    if (item != m_clickedItem)
                    {

                    }
                    else
                    {
                        if (m_selectedItem == m_clickedItem)
                        {
                            m_selectedItem = -1;
                        }
                        else
                        {
                            m_selectedItem = m_clickedItem;
                        }

                        BuildSelection();
                        if (OnSelectionChange != null)
                            OnSelectionChange();
                    }
                    this.LocksMouse = false;
                    m_itemClicked = false;
                }
                else if (m_scrollMoving)
                {
                    //user is dragging scroll by mouse
                    if (releasedButtons[0])
                    {
                        this.LocksMouse = false;
                        m_scrollMoving = false;

                        //slider was released
                    }
                    else
                    {
                        m_mouseScrollDelta += yDelta;
                        this.ScrollPosition = m_initialScrollPosition + m_mouseScrollDelta / m_scrollMarkerRect.Height;

                    }
                }

            }
            else
            {
                if (zDelta != 0)
                {
                    m_upArrowState = State.Normal;
                    m_downArrowState = State.Normal;
                    m_markerState = State.Normal;
                    this.ScrollValue -= (int)zDelta / 100;
                }
                if (m_upArrowRect.Contains(position))
                {
                    m_upArrowState = State.Over;
                    m_downArrowState = State.Normal;
                    m_markerState = State.Normal;
                    if (pressedButtons[0] && (GM.RunningTime - m_arrowPressTime > 0.2))
                    {
                        m_arrowPressTime = GM.RunningTime;
                        m_upArrowState = State.Down;
                        this.ScrollValue--;
                    }
                }
                else if (m_downArrowRect.Contains(position))
                {
                    m_upArrowState = State.Normal;
                    m_downArrowState = State.Over;
                    m_markerState = State.Normal;
                    if (pressedButtons[0] && (GM.RunningTime - m_arrowPressTime > 0.2))
                    {
                        m_arrowPressTime = GM.RunningTime;
                        m_downArrowState = State.Down;
                        this.ScrollValue++;
                    }
                }
                else if (m_markerRect.Contains(position))
                {
                    m_upArrowState = State.Normal;
                    m_downArrowState = State.Normal;
                    m_markerState = State.Over;
                    if (pressedButtons[0])
                    {
                        m_markerState = State.Down;
                        this.LocksMouse = true;
                        m_scrollMoving = true;
                        m_initialScrollPosition = this.ScrollPosition;
                        m_mouseScrollDelta = 0;

                    }
                }
                else
                {
                    m_upArrowState = State.Normal;
                    m_downArrowState = State.Normal;
                    m_markerState = State.Normal;

                    //we check for item selection
                    if (pressedButtons[0] && m_itemsRect.Contains(position))
                    {
                        float y = position.Y - m_itemsRect.Top;
                        m_clickedItem = m_startingItem + (int)Math.Floor(m_numVisibleItems * (y / m_itemsRect.Height));
                        m_itemClicked = true;
                        this.LocksMouse = true;
                    }
                }

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
        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);

            switch (pressedKey)
            {
                case (int)Keys.Up:
                    this.ScrollValue--;
                    break;
                case (int)Keys.Down:
                    this.ScrollValue++;
                    break;
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

            GUIStyle style = GM.GUIStyleManager.GetStyleByName(m_styleName);
            switch (m_state)
            {
                case State.Normal:
                    style.Render(device, m_normalBoxQuads);

                    //render highlight if visible
                    if (m_selectedItem >= m_startingItem && m_selectedItem < m_startingItem + m_numVisibleItems && m_selectedItem < m_items.Count)
                        style.Render(device, m_normalHighlightQuad);
                    GM.FontManager.GetFont(FontName, m_fontSize).Render(device, m_textQuads);

                    switch (m_upArrowState)
                    {
                        case State.Normal:
                            style.Render(device, m_normalUpArrowQuad);
                            break;
                        case State.Over:
                            style.Render(device, m_overUpArrowQuad);
                            break;
                        case State.Down:
                            style.Render(device, m_downUpArrowQuad);
                            break;
                    }
                    switch (m_downArrowState)
                    {
                        case State.Normal:
                            style.Render(device, m_normalDownArrowQuad);
                            break;
                        case State.Over:
                            style.Render(device, m_overDownArrowQuad);
                            break;
                        case State.Down:
                            style.Render(device, m_downDownArrowQuad);
                            break;
                    }
                    if (m_items.Count > m_numVisibleItems)
                    {
                        switch (m_markerState)
                        {
                            case State.Normal:
                                style.Render(device, m_normalMarkerQuad);
                                break;
                            case State.Over:
                                style.Render(device, m_overMarkerQuad);
                                break;
                            case State.Down:
                                style.Render(device, m_downMarkerQuad);
                                break;
                        }
                    }
                    break;
                case State.Disabled:
                    style.Render(device, m_disabledBoxQuads);
                    style.Render(device, m_disabledUpArrowQuad);
                    style.Render(device, m_disabledDownArrowQuad);

                    break;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Framework.Fonts;
using Microsoft.DirectX.Direct3D;

namespace Framework.GUI
{
    /// <summary>
    /// A class representing a text label.
    /// </summary>
    public class GuiTextLabel : GuiControl
    {
        #region Fields
        private string m_text;
        private Color m_color;
        private Align m_alignment;
        private List<Quad> m_textQuads = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Control's size and position</param>
        /// <param name="text">Control's text</param>
        /// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        public GuiTextLabel(string text, RectangleF rect, float fontSize)
            : base(rect, GM.GetUniqueName())
        {
            //FontName = .FontName;
            m_text = text;
            
            m_alignment = Align.Left;

            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());
            if(fontSize != 0) m_fontSize= fontSize;
            //StringBlock b = new StringBlock(m_text, new RectangleF(m_position, m_size), m_alignment, m_fontSize, ColorValue.FromColor(Color.Black), true);
            //m_textQuads = GM.FontManager.GetFont(m_fontName, m_fontSize).GetProcessedQuads(b);
            BuildTextQuads();
        }

        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);
            //m_fo
            m_color = style.GetNodeByName("TextLabel").Defaults.Color;

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Control's size and position</param>
        /// <param name="text">Control's text</param>
        /// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        /// <param name="font">Font, "" for default to current style</param>
        /// <param name="alignment">Text alignment</param>
        /// <param name="color">Text color</param>
        public GuiTextLabel(string text, RectangleF rect, float fontSize,
            string fontName, Align alignment, Color color)
            : base(rect, GM.GetUniqueName())
        {
            m_text = text;
            
            
            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());
            if (fontSize != 0) m_fontSize = fontSize;
            m_alignment = alignment;
            m_color = color;
            //StringBlock b = new StringBlock(m_text, new RectangleF(m_position, m_size), m_alignment, m_fontSize, ColorValue.FromColor(Color.Black), true);
            //m_textQuads = GM.FontManager.GetFont(m_fontName, m_fontSize).GetProcessedQuads(b);
            BuildTextQuads();
        }
        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="name">Control's unique name</param>
        ///// <param name="rect">Control's size and position</param>
        ///// <param name="text">Control's text</param>
        ///// <param name="fontSize">Font size in pixels(max line height). 0 for default</param>
        ///// <param name="font">Font</param>
        ///// <param name="alignment">Text alignment</param>
        ///// <param name="color">Text color</param>
        //public GuiTextLabel(string name, string text, Rectangle rect, float fontSize,
        //    string fontName, Align alignment, Color color)
        //    : base(rect, name)
        //{
        //    m_text = text;
        //    m_fontSize = (fontSize == 0) ? DefaultValues.TextSize : fontSize;
        //    FontName = fontName;
        //    m_alignment = alignment;
        //    m_color = color;
        //    BuildTextQuads();
            
        //}
        protected override void BuildTextQuads()
        {
            StringBlock b = new StringBlock(m_text, new RectangleF(m_position.X+PositionDelta.X,m_position.Y+PositionDelta.Y, m_size.Width,m_size.Height),
                m_alignment, m_fontSize, ColorValue.FromColor(m_color), true);
            m_textQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);
        }

        //protected override string FontName
        //{
        //    get
        //    {
        //        return base.FontName;
        //    }
        //    set
        //    {
        //        base.FontName = value;
        //        BuildTextQuads();
        //    }
        //}
        /// <summary>
        /// TextLabels don't receive focus
        /// </summary>
        public override bool RecievesFocus
        {
            get
            {
                return false;
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
                base.PositionDelta = value;
            }
        }

        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                PointF delta = new PointF(value.X - m_position.X, value.Y - m_position.Y);
                foreach (Quad q in m_textQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                base.Position = value;


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
            m_positionDelta.X += xDelta;
            m_positionDelta.Y += yDelta;

        }

        /// <summary>
        /// Renders control
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            GM.FontManager.GetFont(FontName,m_fontSize).Render(device, m_textQuads);

        }



        public string Text
        {
            get { return m_text; }
            set {
                m_text = (value == null) ? "" : value;
                BuildTextQuads();
            }
        }

    }
}


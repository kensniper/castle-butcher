using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace Framework.GUI
{
    class GuiRadioButton : GuiControl
    {
        #region Fields
        private List<Quad> m_normalBoxQuads = new List<Quad>(1);
        private List<Quad> m_overBoxQuads = new List<Quad>(1);
        private List<Quad> m_downBoxQuads = new List<Quad>(1);
        private List<Quad> m_disabledBoxQuads = new List<Quad>(1);
        private List<Quad> m_checkMarkQuads = new List<Quad>(1);


        private bool m_checked = false;
        #endregion

        public GuiRadioButton(RectangleF buttonRect)
            : base(buttonRect, GM.GetUniqueName())
        {
            GUIStyle style = GM.GUIStyleManager.GetCurrentStyle();
            ProcessStyle(style);
        }

        public GuiRadioButton(string name, RectangleF buttonRect)
            : base(buttonRect, name)
        {
            GUIStyle style = GM.GUIStyleManager.GetCurrentStyle();
            ProcessStyle(style);
        }


        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode normal, over, down, disabled, mark;
            normal = style.GetNodeByName("RadioButton").GetNodeByName("Normal");
            over = style.GetNodeByName("RadioButton").GetNodeByName("Over");
            down = style.GetNodeByName("RadioButton").GetNodeByName("Down");
            disabled = style.GetNodeByName("RadioButton").GetNodeByName("Disabled");
            mark = style.GetNodeByName("RadioButton").GetNodeByName("RadioMark");


            float tWidth = (float)style.ImageInfo.Width;
            float tHeight = (float)style.ImageInfo.Height;

            //Positioning Quads
            ImageNode i;
            RectangleF rect;
            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;


            //normal
            i = normal;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //over
            i = over;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //down
            i = down;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //disabled
            i = disabled;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledBoxQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //mark
            i = mark;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_checkMarkQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));



            foreach (Quad q in m_normalBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_overBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_downBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_disabledBoxQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_checkMarkQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
        }

        /// <summary>
        /// Fires when radio button is selected.
        /// </summary>
        public event ButtonEventHandler OnClick;

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
                foreach (Quad q in m_overBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledBoxQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_checkMarkQuads)
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
            foreach (Quad q in m_overBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledBoxQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_checkMarkQuads)
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
        /// Checked/unchecked state.
        /// </summary>
        public bool Checked
        {
            get
            {
                return m_checked;
            }
            set
            {
                m_checked = value;
            }
        }

        /// <summary>
        /// Deselects button.
        /// </summary>
        public void Deselect()
        {
            m_checked = false;
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
                    style.Render(device, m_normalBoxQuads);
                    break;
                case State.Over:
                    style.Render(device, m_overBoxQuads);
                    break;
                case State.Down:
                    style.Render(device, m_downBoxQuads);
                    break;
                case State.Disabled:
                    style.Render(device, m_disabledBoxQuads);
                    break;
            }
            if (m_checked)
                style.Render(device, m_checkMarkQuads);
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
                    if (m_checked == false)
                    {
                        m_checked = true;
                        if (OnClick != null)
                            OnClick();
                    }
                }
                m_state = State.Normal;
                LocksMouse = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Framework.Fonts;

namespace Framework.GUI
{
    public delegate void WindowDataHandler(object data); 
    public class GuiWindow : GuiControl
    {
        #region Fields
        //managing child controls
        protected List<GuiControl> m_controls = new List<GuiControl>();
        private GuiControl m_currentControl;  //the control that has mouse pointer above
        private GuiControl m_controlInFocus;  //the control that has input focus 
        private List<Quad> m_windowQuads = new List<Quad>();
        private List<Quad> m_titleQuads;

        //window specific

        protected string m_windowTitle;
        protected RectangleF m_titleRect;
        protected RectangleF m_usableRect;
        protected bool m_isMoving = false;
        protected GuiCheckBox m_closingCross;


        #endregion

        public GuiWindow(string text, RectangleF rect)
            : base(rect, GM.GetUniqueName())
        {
            m_windowTitle = text;
            GUIStyle style = GM.GUIStyleManager.GetCurrentStyle();
            ProcessStyle(style);

            BuildTextQuads();

            m_closingCross = new GuiCheckBox(new RectangleF(m_usableRect.Width - 15, -m_titleRect.Height, 15, 15));
            m_closingCross.Checked = true;
            AddControl(m_closingCross);
        }

        public GuiWindow(string name, string text, RectangleF rect)
            : base(rect, name)
        {
            m_windowTitle = text;
            GUIStyle style = GM.GUIStyleManager.GetDefaultStyle();
            ProcessStyle(style);
            BuildTextQuads();

            m_closingCross = new GuiCheckBox(new RectangleF(m_usableRect.Width - 15, -m_titleRect.Height, 15, 15));
            m_closingCross.Checked = true;
            AddControl(m_closingCross);
        }


        protected override void BuildTextQuads()
        {
            StringBlock b = new StringBlock(m_windowTitle,new RectangleF( m_titleRect.X + m_positionDelta.X, m_titleRect.Y + m_positionDelta.Y,
                m_titleRect.Width, m_titleRect.Height), Align.Left, m_titleRect.Height, ColorValue.FromColor(Color.Black), true);
            m_fontSize = m_titleRect.Height;
            m_titleQuads = GM.FontManager.GetFont(FontName, m_fontSize).GetProcessedQuads(b);
        }
        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode bottomB, leftB, rightB, topB, trC, tlC, brC, blC, windowBg, titleBg;
            bottomB = style.GetNodeByName("Window").GetNodeByName("BottomBorder");
            topB = style.GetNodeByName("Window").GetNodeByName("TopBorder");
            leftB = style.GetNodeByName("Window").GetNodeByName("LeftBorder");
            rightB = style.GetNodeByName("Window").GetNodeByName("RightBorder");
            trC = style.GetNodeByName("Window").GetNodeByName("TopRightCorner");
            tlC = style.GetNodeByName("Window").GetNodeByName("TopLeftCorner");
            brC = style.GetNodeByName("Window").GetNodeByName("BottomRightCorner");
            blC = style.GetNodeByName("Window").GetNodeByName("BottomLeftCorner");
            windowBg = style.GetNodeByName("Window").GetNodeByName("Background");
            titleBg = style.GetNodeByName("Window").GetNodeByName("TitleBackground");


            float left, right, top, bottom, title;
            left = leftB.Rectangle.Width;
            right = rightB.Rectangle.Width;
            top = topB.Rectangle.Height;
            bottom = bottomB.Rectangle.Height;
            title = DefaultValues.TitleTextSize;

            m_usableRect = new RectangleF();
            m_usableRect.X = m_position.X + left;
            m_usableRect.Y = m_position.Y + top + title;
            m_usableRect.Width = m_size.Width - left - right;
            m_usableRect.Height = m_size.Height - top - title - bottom;

            m_titleRect = new RectangleF();
            m_titleRect.X = m_position.X + left;
            m_titleRect.Y = m_position.Y + top;
            m_titleRect.Width = m_size.Width - left - right;
            m_titleRect.Height = title;


            float tWidth = (float)style.ImageInfo.Width;
            float tHeight = (float)style.ImageInfo.Height;

            //Positioning Quads
            ImageNode i;
            RectangleF rect;
            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            #region Borders
            //leftBorder
            i = leftB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topBorder
            i = topB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //rightBorder
            i = rightB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_usableRect.Width + left, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width + right, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + m_usableRect.Width + left, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width + right, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomBorder
            i = bottomB;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Corners
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
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //topRightCorner
            i = trC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_usableRect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_usableRect.Width, m_position.Y + top,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomRightCorner
            i = brC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_usableRect.Width, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + left + m_usableRect.Width, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + right + left + m_usableRect.Width, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //bottomLeftCorner
            i = blC;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + left, m_position.Y + bottom + top + m_usableRect.Height + m_titleRect.Height,
                0, 1, i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            #region Backgrounds
            //WindowBackground
            i = windowBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_usableRect.Left, m_usableRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_usableRect.Right, m_usableRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_usableRect.Left, m_usableRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_usableRect.Right, m_usableRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //TitleBackground
            i = titleBg;
            rect = i.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_titleRect.Left, m_titleRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_titleRect.Right, m_titleRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_titleRect.Left, m_titleRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_titleRect.Right, m_titleRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_windowQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion


        }

        private void Move(float xDelta, float yDelta)
        {
            m_position.X += xDelta;
            m_position.Y += yDelta;
            m_titleRect.X += xDelta;
            m_titleRect.Y += yDelta;
            m_usableRect.X += xDelta;
            m_usableRect.Y += yDelta;

            foreach (Quad q in m_windowQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_titleQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }

            foreach (GuiControl control in m_controls)
            {
                control.ChangePositionDelta(xDelta, yDelta);
            }
        }

        /// <summary>
        /// Adds a new control to the window.
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(GuiControl control)
        {
            control.PositionDelta = this.Offset;
            m_controls.Add(control);
        }

        /// <summary>
        /// Removes a control from the window.
        /// </summary>
        /// <param name="control"></param>
        public void RemoveControl(GuiControl control)
        {
            m_controls.Remove(control);
        }

        /// <summary>
        /// Finds a control by it's unique name.
        /// </summary>
        /// <param name="name">Control's unique name</param>
        /// <returns>GenericControl object if successful, null otherwise</returns>
        public GuiControl GetControlByName(string name)
        {
            foreach (GuiControl control in m_controls)
            {
                if (control.ControlName == name)
                    return control;
            }
            return null;
        }



        private PointF Offset
        {
            get
            {
                return m_usableRect.Location;
            }
        }

        /// <summary>
        /// Closes the window. 
        /// </summary>
        public virtual void Close()
        {
            this.HasFinished = true;
        }
        /// <summary>
        /// Closes the window and fires OnExit event. 
        /// </summary>
        protected virtual void Close(object exitData)
        {
            this.HasFinished = true;
            SubmitData(exitData);
        }
        protected virtual void SubmitData(object data)
        {
            if (OnExit != null)
                OnExit(data);
        }

        public event WindowDataHandler OnExit;
        /// <summary>
        /// This method is called when this control loses focus. 
        /// </summary>
        public override void OnEndFocus()
        {
            base.OnEndFocus();
            if (m_controlInFocus != null)
            {
                m_controlInFocus.OnEndFocus();
                m_controlInFocus = null;
            }
        }

        /// <summary>
        /// This method is called when control recieves focus.
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            if (m_currentControl != null)
            {
                m_currentControl.OnMouseLeave();
                m_currentControl = null;
            }
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

            //first we translate mouse coords to window space
            //because all controls require it(except for windows)
            position.X -= m_usableRect.X;
            position.Y -= m_usableRect.Y;

            //if currentControl is moving, we don't process anything else
            if (m_currentControl != null && m_currentControl.LocksMouse)
            {
                m_currentControl.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

                if (m_currentControl.LocksMouse == false)
                    this.LocksMouse = false;
                return;
            }

            //first we check if mouse was clicked on the title bar
            //if so,ten window is going to move
            if (pressedButtons[0])
            {
                if (m_isMoving)
                {
                    Move(xDelta, yDelta);
                    return;
                }
                else if (m_titleRect.Contains(position.X + m_usableRect.X, position.Y + m_usableRect.Y))
                {
                    if (m_closingCross.ContainsPoint(position))
                    {
                        Close();
                    }
                    m_isMoving = true;
                    LocksMouse = true;
                    Move(xDelta, yDelta);
                    return;
                }
            }
            //if we got this far, the window is not in moving state
            //so we perform normal control management
            m_isMoving = false;
            LocksMouse = false;





            if (m_currentControl != null)
            {
                if (m_currentControl.ContainsPoint(position))
                {
                    //m_currentControl.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
                }
                else
                {
                    m_currentControl.OnMouseLeave();
                    m_currentControl = null;
                }
            }

            for (int i = 0; i < m_controls.Count; i++)
            {
                if (m_controls[i].ContainsPoint(position) && !m_controls[i].IsDisabled)
                {
                    if (m_controls[i] != m_currentControl)
                    {
                        if (m_currentControl != null)
                        {
                            m_currentControl.OnMouseLeave();
                        }
                        m_controls[i].OnMouseEnter();
                        //we have a new currentControl
                        m_currentControl = m_controls[i];
                    }
                    m_controls[i].OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
                    break;
                }
            }

            if (pressedButtons[0])
            {
                //if there is a currentControl, we can set HasFocus property
                if (m_currentControl != null && m_currentControl != m_controlInFocus)
                {
                    if (m_controlInFocus != null)
                    {
                        m_controlInFocus.OnEndFocus();
                    }
                    if (m_currentControl.RecievesFocus)
                    {
                        m_currentControl.OnBeginFocus();
                        m_controlInFocus = m_currentControl;
                    }
                    else
                        m_controlInFocus = null;

                    //control in focus goes to the front of the list
                    //ToFront(m_controls.IndexOf(m_controlInFocus));
                }
                //if there is no currentControl, but there is controlInFocus
                //it means that focus has been lost
                else if (m_controlInFocus != null && m_currentControl == null)
                {
                    m_controlInFocus.OnEndFocus();
                    m_controlInFocus = null;
                }
            }
            //at the end we check if m_currentControl locks mouse
            //if so, then this window has to lock mouse too
            if (m_currentControl != null && m_currentControl.LocksMouse)
            {
                this.LocksMouse = true;
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
            if (pressedKey == (int)Keys.Tab)
            {
                if (m_controlInFocus != null)
                {
                    //m_controlInFocus = null;
                    int b = m_controls.IndexOf(m_controlInFocus);
                    for (int i = 1; i < m_controls.Count-1 ; i++)
                    {
                        if (m_controls[(b + i) % m_controls.Count].RecievesFocus && !m_controls[(b + i) % m_controls.Count].IsDisabled)
                        {
                            m_controlInFocus.OnEndFocus();
                            m_controlInFocus = m_controls[(b + i) % m_controls.Count];
                            m_controlInFocus.OnBeginFocus();
                            break;
                        }
                    }

                }
                else
                {
                    if (m_controls.Count > 0)
                    {
                        for (int i = 0; i < m_controls.Count; i++)
                        {
                            if (m_controls[i].RecievesFocus && !m_controls[i % m_controls.Count].IsDisabled)
                            {
                                m_controlInFocus = m_controls[i];
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_controlInFocus != null && !m_controlInFocus.IsDisabled)
                {
                    m_controlInFocus.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
                }

            }
        }

        /// <summary>
        /// Renders window and it's controls
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            //first the window
            GUIStyle style = GM.GUIStyleManager.GetStyleByName(m_styleName);
            style.Render(device, m_windowQuads);

            //and title
            GM.FontManager.GetFont(FontName,m_fontSize).Render(device, m_titleQuads);

            //then other controls
            foreach (GuiControl control in m_controls)
            {
                if (!control.HasFocus)
                    control.Render(device, elapsedTime);
            }
            //control in focus renders last, because it can overlap some other controls
            if (m_controlInFocus != null)
                m_controlInFocus.Render(device, elapsedTime);
        }
    }
}

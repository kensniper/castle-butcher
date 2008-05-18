using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Framework.GUI
{
    /// <summary>
    /// Base class for all controls
    /// </summary>
    public abstract class GuiControl
    {
        #region Fields
        protected string m_styleName = "Default";
        private string m_fontName = "Default";
        protected float m_fontSize;
        protected string m_controlName;
        protected PointF m_position;
        protected SizeF m_size;
        protected PointF m_positionDelta = new PointF(0, 0);
        private bool _hasFocus = false;
        private bool _isDisabled = false;
        private bool _locksMouse = false;
        protected bool _hasFinished = false;

        protected enum State { Normal, Over, Down, Disabled }
        protected State m_state = State.Normal;
        #endregion

        public GuiControl(RectangleF rect, string name)
        {
            m_position = rect.Location;
            m_size = rect.Size;
            m_controlName = name;
            m_fontSize = DefaultValues.TextSize;

        }
        public GuiControl(RectangleF rect, PointF posDelta, string name)
        {
            m_position = rect.Location;
            m_size = rect.Size;
            m_positionDelta = posDelta;
            m_controlName = name;
        }
        /// <summary>
        /// Processes a GUIStyle object
        /// </summary>
        /// <param name="style"></param>
        protected virtual void ProcessStyle(GUIStyle style)
        {
            m_styleName = style.Name;
            FontName = style.FontName;

        }
        /// <summary>
        /// Control's unique name
        /// </summary>
        public virtual string ControlName
        {
            get
            {
                return m_controlName;
            }
        }



        protected virtual string FontName
        {
            get { return m_fontName; }
            set
            { 
                m_fontName = value;
                BuildTextQuads();
            }
        }

        protected virtual void BuildTextQuads() { }
        /// <summary>
        /// Tells if control has focus. When a control has focus it recieves keyboard data.
        /// </summary>
        public bool HasFocus
        {
            get { return _hasFocus; }
            set
            {
                _hasFocus = value;
            }
        }


        /// <summary>
        /// Tells if control has finished and is ready to be removed.
        /// </summary>
        public bool HasFinished
        {
            get
            {
                return _hasFinished;
            }
            protected set
            {
                _hasFinished = value;
            }
        }

        /// <summary>
        /// Tells if control is disabled. Dsabled control doesn't 
        /// recieve any input from user.
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return _isDisabled;
            }
            set
            {
                _isDisabled = value;
            }
        }

        /// <summary>
        /// Tells if control can recieve focus
        /// <example>Text lables don't recieve focus, but edit boxes do.</example>
        /// </summary>
        public virtual bool RecievesFocus
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Tells if control has a lock on mouse, meaning that all
        /// mouse data goes to this control.
        /// <example>When you move a scroll, the scroll has mouse lock so
        /// that you can drag the mouse all around the screen and it still affects the scroll. Normally a control
        /// recieves mouse data only if it contains the mouse.</example>
        /// </summary>
        public virtual bool LocksMouse
        {
            get
            {
                return _locksMouse;
            }
            protected set
            {
                _locksMouse = value;
            }
        }

        /// <summary>
        /// Gets/sets control's position delta
        /// Position delta is the offset from a coordinate system in which
        /// control was created, to a coordinate system of the screen.
        /// <example>If you have a window, all controls in it are placed 
        /// relative to window's position.</example>
        /// </summary>
        public virtual PointF PositionDelta
        {
            get
            {
                return m_positionDelta;
            }
            set
            {
                m_positionDelta = value;
            }
        }

        /// <summary>
        /// Changes controls PositionDelta
        /// </summary>
        /// <param name="xDelta">x delta</param>
        /// <param name="yDelta">y delta</param>
        public virtual void ChangePositionDelta(float xDelta, float yDelta)
        {

        }

        /// <summary>
        /// Control's position in local coords.
        /// </summary>
        public virtual PointF Position
        {
            get
            {
                return m_position;
            }
            set
            {
            }
        }

        #region Virtual methods
        /// <summary>
        /// This method is called when mouse enters control's space.
        /// </summary>
        public virtual void OnMouseEnter() { }

        /// <summary>
        /// This method is called when mouse leaves control's space.
        /// </summary>
        public virtual void OnMouseLeave() { }

        /// <summary>
        /// This method is called when control recieves focus.
        /// </summary>
        public virtual void OnBeginFocus()
        {
            HasFocus = true;
        }

        /// <summary>
        /// This method is called when this control loses focus. 
        /// </summary>
        public virtual void OnEndFocus()
        {
            HasFocus = false;
        }
        /// <summary>
        /// Checks if control contains point p(in local coords).
        /// </summary>
        /// <param name="p">Point in local cords.</param>
        /// <returns>True if contains, false otherwise.</returns>
        public virtual bool ContainsPoint(PointF p)
        {
            if (p.X > m_position.X && p.X < m_position.X + m_size.Width)
                if (p.Y > m_position.Y && p.Y < m_position.Y + m_size.Height)
                    return true;
            return false;
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
        public virtual void OnMouse(PointF position, float xDelta, float yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime) { }

        /// <summary>
        /// Processes keyboard data.
        /// </summary>
        /// <param name="pressedKeys">List of pressed keys</param>
        /// <param name="releasedKeys">List of released keys (since last frame)</param>
        /// <param name="pressedChar">Last pressed key as a character</param>
        /// <param name="pressedKey">Last pressed key as int</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public virtual void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime) { }

        /// <summary>
        /// Renders control
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public virtual void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime) { }

        /// <summary>
        /// Disables control, so that it is visible, but user can't interact with it.
        /// </summary>
        public virtual void Disable()
        {
            m_state = State.Disabled;
            _isDisabled = true;

        }
        /// <summary>
        /// Enables control, so that user can interact with it.
        /// </summary>
        public virtual void Enable()
        {
            m_state = State.Normal;
            _isDisabled = false;
        }
        #endregion

        ~GuiControl()
        {
            GM.RemoveUniqueName(m_controlName);
        }
    }
}

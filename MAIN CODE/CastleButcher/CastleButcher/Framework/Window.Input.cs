using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Framework
{
    public sealed partial class FrameworkWindow
    {
        #region Fields related to keyboard
        List<Keys> m_pressedKeys = new List<Keys>();
        List<Keys> m_releasedKeys = new List<Keys>();
        List<Keys> m_lockedKeys = new List<Keys>();
        List<Keys> m_keyLocks = new List<Keys>();
        char m_pressedChar = char.MinValue;
        //protected int m_numPressedKeys = 0;
        int m_pressedKey = 0;
        #endregion

        #region Fields related to mouse
        bool[] m_pressedButtons = new bool[3];
        bool[] m_releasedButtons = new bool[3];
        Point m_mousePosition;
        int m_numPressedButtons = 0;
        int m_xDelta = 0;
        int m_yDelta = 0;
        int m_zDelta = 0;
        #endregion

        #region Methods related to keyboard
        /// <summary>Key down event</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!m_pressedKeys.Contains(e.KeyCode) && !m_lockedKeys.Contains(e.KeyCode))
            {
                m_pressedKeys.Add(e.KeyCode);
                if (m_keyLocks.Contains(e.KeyCode))
                {
                    m_lockedKeys.Add(e.KeyCode);
                }
            }
            m_pressedKey = e.KeyValue;
            //NeedsUpdate();
        }

        /// <summary>Key press event</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // OnKeyPress lets us grab character keys
            m_pressedChar = e.KeyChar;
            //NeedsUpdate();
        }

        /// <summary>Key up event</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            m_pressedKeys.Remove(e.KeyCode);
            m_lockedKeys.Remove(e.KeyCode);
            if (!m_releasedKeys.Contains(e.KeyCode))
                m_releasedKeys.Add(e.KeyCode);

            //NeedsUpdate();
        }

        /// <summary>Locks a key so it is only read once per key down</summary>
        /// <param name="key">Key to lock</param>
        public void LockKey(Keys key)
        {
            if (!m_lockedKeys.Contains(key))
            {
                m_lockedKeys.Add(key);
                //m_pressedKeys.Remove(key);
            }
        }
        /// <summary>Unlocks a key so it can be repeated</summary>
        /// <param name="key">Key to unlock</param>
        public void UnlockKey(Keys key)
        {
            m_lockedKeys.Remove(key);
        }

        /// <summary>Adds a key lock so that it will be locked automatically when it is pressed</summary>
        /// <param name="key">Key to lock</param>
        public void AddKeyLock(Keys key)
        {
            if (!m_keyLocks.Contains(key))
            {
                m_keyLocks.Add(key);
                //m_pressedKeys.Remove(key);
            }
        }
        /// <summary>Removes a key lock</summary>
        /// <param name="key">Key to unlock</param>		
        public void RemoveKeyLock(Keys key)
        {
            m_keyLocks.Remove(key);
        }
        #endregion

        #region Methods related to mouse
        /// <summary>Mouse move event.</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            System.Drawing.Point p = this.PointToClient(Cursor.Position);
            m_xDelta = p.X - m_mousePosition.X;
            m_yDelta = p.Y - m_mousePosition.Y;
            m_mousePosition = p;

            //NeedsUpdate();
        }
        /// <summary>Mouse wheel event</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            m_zDelta = e.Delta;

            //NeedsUpdate();
        }

        /// <summary>Mouse down event</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_numPressedButtons++;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_pressedButtons[0] = true;
                    break;
                case MouseButtons.Middle:
                    m_pressedButtons[1] = true;
                    break;
                case MouseButtons.Right:
                    m_pressedButtons[2] = true;
                    break;
            }
            //NeedsUpdate();
        }

        /// <summary>Mouse up event.</summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            m_numPressedButtons--;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_pressedButtons[0] = false;
                    m_releasedButtons[0] = true;
                    break;
                case MouseButtons.Middle:
                    m_pressedButtons[1] = false;
                    m_releasedButtons[1] = true;
                    break;
                case MouseButtons.Right:
                    m_pressedButtons[2] = false;
                    m_releasedButtons[2] = true;
                    break;
            }
            //NeedsUpdate();
        }

        /// <summary>Returns true if the main mouse buttons are all up</summary>
        private bool NoButtonsDown
        {
            get { return m_pressedButtons[0] == m_pressedButtons[1] == m_pressedButtons[2] == false; }
        }

        /// <summary>Returns true if no buttons were released</summary>
        private bool NoButtonsReleased
        {
            get { return m_releasedButtons[0] == m_releasedButtons[1] == m_releasedButtons[2] == false; }
        }

        /// <summary>Gets and sets the cursor positon</summary>
        public Point MouseCursorPosition
        {
            get { return m_mousePosition; }
            set
            {
                m_mousePosition = value;
                Cursor.Position = this.PointToScreen(value);
            }
        }
        /// <summary>
        /// Sets mouse cursor to the middle of the window.
        /// </summary>
        public void CenterCursor()
        {
            if (m_windowed)
            {
                m_mousePosition = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
                Cursor.Position = PointToScreen(m_mousePosition);
            }
            else
            {
                m_mousePosition = new Point(m_fullscreenParameters.BackBufferWidth / 2, m_fullscreenParameters.BackBufferHeight / 2);
                Cursor.Position = new Point(m_fullscreenParameters.BackBufferWidth / 2, m_fullscreenParameters.BackBufferHeight / 2);
            }
        }
        #endregion
    }
}
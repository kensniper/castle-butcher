using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace Framework.GUI
{
    /// <summary>
    /// A class that serves as a container for controls. It is the most fundamental class
    /// for any GUI. Provides mouse and keyboard events handling and notifies appropriate controls about those events.
    /// </summary>
    public class ControlContainer : GameLayer
    {
        #region Fields
        List<GuiControl> m_controls = new List<GuiControl>();
        GuiControl m_currentControl;  //the control that has mouse pointer above
        GuiControl m_controlInFocus;  //the control that has input focus 
        #endregion

        public ControlContainer()
        {
            locksMouse = true;
            locksKeyboard = true;
            isTransparent = true;
        }
        /// <summary>
        /// Adds a control
        /// </summary>
        /// <param name="control">Control to be added</param>
        public void AddControl(GuiControl control)
        {
            m_controls.Add(control);
            ToFront(control);
        }
        /// <summary>
        /// Removes a control
        /// </summary>
        /// <param name="control">Control to be added</param>
        public void RemoveControl(GuiControl control)
        {
            m_controls.Remove(control);
            if (m_controlInFocus == control)
                m_controlInFocus = null;
            if (m_currentControl == control)
                m_currentControl = null;
        }

        /// <summary>
        /// Finds ad returns a control by its name
        /// </summary>
        /// <param name="name">Control's unique name</param>
        /// <returns>A GenericControl object if successful, null otherwise</returns>
        public GuiControl GetControlByName(string name)
        {
            foreach (GuiControl control in m_controls)
            {
                if (control.ControlName == name)
                    return control;
            }
            return null;
        }

        private void ToFront(int controlIndex)
        {
            if (controlIndex == 0)
                return;
            GuiControl control = m_controls[controlIndex];
            m_controls.RemoveAt(controlIndex);
            m_controls.Insert(0, control);
        }
        private void ToFront(GuiControl control)
        {
            m_controls.Remove(control);
            m_controls.Insert(0, control);
        }
        public override void OnMouse(Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

            //if control is moving, we don't process anytrhing else
            if (m_currentControl != null && m_currentControl.LocksMouse)
            {
                m_currentControl.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
            }
            else
            {
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
                    if (m_controls[i].ContainsPoint(position))
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
                        m_currentControl.OnBeginFocus();
                        m_controlInFocus = m_currentControl;
                        //control in focus goes to the front of the list
                        ToFront(m_controls.IndexOf(m_controlInFocus));
                    }
                    //if there is no currentControl, but there is controlInFocus
                    //it means that focus has been lost
                    else if (m_controlInFocus != null && m_currentControl == null)
                    {
                        m_controlInFocus.OnEndFocus();
                        m_controlInFocus = null;
                    }


                }
            }
            
            //we check for controls that have to be closed
            for (int i = 0; i < m_controls.Count; i++)
            {
                if (m_controls[i].HasFinished)
                {
                    if (m_controls[i] == m_currentControl)
                        m_currentControl = null;
                    if (m_controls[i] == m_controlInFocus)
                        m_controlInFocus = null;
                    m_controls.RemoveAt(i);
                    GC.Collect(0);
                    i--;
                }
            }
        }
        public override void OnKeyboard(List<Keys> pressedKeys, List<Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            //if (pressedKey == (int)Keys.Tab)
            //{
            //    if (m_controlInFocus != null)
            //    {
            //        int i = (m_controls.IndexOf(m_controlInFocus) + 1) % m_controls.Count;
            //        m_controlInFocus = m_controls[i];
            //        ToFront(i);
            //    }
            //    else
            //    {
            //        if (m_controls.Count > 0)
            //            m_controlInFocus = m_controls[0];
            //    }
            //}
            //else
            //{
                if (m_controlInFocus != null)
                    m_controlInFocus.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            //}

            //we check for controls that have to be closed
            for (int i = 0; i < m_controls.Count; i++)
            {
                if (m_controls[i].HasFinished)
                {
                    if (m_controls[i] == m_currentControl)
                        m_currentControl = null;
                    if (m_controls[i] == m_controlInFocus)
                        m_controlInFocus = null;
                    m_controls.RemoveAt(i);
                    GC.Collect(0);
                    i--;
                }
            }
        }
        public override void OnRenderFrame(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.OnRenderFrame(device, elapsedTime);
            device.RenderState.CullMode = Cull.None;
            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                m_controls[i].Render(device, elapsedTime);
            }
        }
    }
}

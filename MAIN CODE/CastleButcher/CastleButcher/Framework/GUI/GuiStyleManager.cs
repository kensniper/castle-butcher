using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Framework.GUI
{
    /// <summary>
    /// A GUIStyle manager. Handles style's creation, disposing and On*Device events
    /// </summary>
    public class GuiStyleManager : IDisposable, IDeviceRelated
    {
        private List<GUIStyle> m_styles = new List<GUIStyle>();
        private int m_default = 0;
        private int m_current=0;

        public GuiStyleManager()
        {
        }

        /// <summary>
        /// Adds a new style.
        /// </summary>
        /// <param name="xmlFile">XML file describing the new style</param>
        public void AddStyle(string xmlFile)
        {
            m_styles.Add(new GUIStyle(xmlFile));
        }

        #region Getting styles

        /// <summary>
        /// Gets style by name
        /// </summary>
        /// <param name="name">Font's name</param>
        /// <returns></returns>
        public GUIStyle GetStyleByName(string name)
        {
            if (name == "Default" || name == "default")
                return GetDefaultStyle();
            foreach (GUIStyle s in m_styles)
            {
                if (s.Name == name)
                    return s;
            }
            return null;
        }
        /// <summary>
        /// Gets defaut style
        /// </summary>
        /// <returns></returns>
        public GUIStyle GetDefaultStyle()
        {
            if (m_styles.Count > m_default)
                return m_styles[m_default];
            else
                return null;
        }
        /// <summary>
        /// Gets defaut style
        /// </summary>
        /// <returns></returns>
        public GUIStyle GetCurrentStyle()
        {
            if (m_styles.Count > m_current)
                return m_styles[m_current];
            else
                return GetDefaultStyle();
        }

        /// <summary>
        /// Sets current style, which can be obtained by calling GetCurrentStyle().
        /// </summary>
        /// <param name="style">Style object to be set as current</param>
        public void SetCurrentStyle(GUIStyle style)
        {
            for (int i = 0; i < m_styles.Count; i++)
            {
                if (m_styles[i] == style)
                {
                    m_current = i;
                }
            }
        }
        /// <summary>
        /// Sets current style, which can be obtained by calling GetCurrentStyle().
        /// </summary>
        /// <param name="style">Style object's name to be set as current</param>
        public void SetCurrentStyle(string name)
        {
            for (int i = 0; i < m_styles.Count; i++)
            {
                if (m_styles[i].Name == name)
                {
                    m_current = i;
                }
            }
        }
        /// <summary>
        /// Gets style by it's .xml file
        /// </summary>
        /// <param name="fntFile">.xml file</param>
        /// <returns></returns>
        public GUIStyle GetStyleByFile(string xmlFile)
        {
            foreach (GUIStyle s in m_styles)
            {
                if (s.XmlFile == xmlFile)
                    return s;
            }
            return null;
        }

        #endregion

        #region On*Device functions
        public void OnCreateDevice(Device device)
        {
            foreach (GUIStyle s in m_styles)
            {
                s.OnCreateDevice(device);
            }
        }

        public void OnResetDevice(Device device)
        {
            foreach (GUIStyle s in m_styles)
            {
                s.OnResetDevice(device);
            }
        }
        public void OnLostDevice(Device device)
        {
            foreach (GUIStyle s in m_styles)
            {
                s.OnLostDevice(device);
            }
        }
        public void OnDestroyDevice(Device device)
        {
            foreach (GUIStyle s in m_styles)
            {
                s.OnDestroyDevice(device);
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_styles != null)
            {
                for (int i = 0; i < m_styles.Count; i++)
                {
                    if (m_styles[i] != null)
                    {
                        m_styles[i].Dispose();
                        m_styles[i] = null;
                    }
                }
                m_styles = null;
            }

        }

        #endregion
    }
}

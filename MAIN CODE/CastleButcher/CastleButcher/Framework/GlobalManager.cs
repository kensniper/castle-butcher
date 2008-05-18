using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;


namespace Framework
{

    class GM : IDisposable
    {
        private static Log m_generalLog;
        private static FrameworkWindow m_appWindow;
        private static Fonts.FontManager m_fontManager;
        private static GUI.GuiStyleManager m_guiStyleManager;
        //private static ResourceCache m_resourceCache;

        //public static ResourceCache ResourceCache
        //{
        //    get { return GM.m_resourceCache; }
        //    //set { GM.m_resourceManager = value; }
        //}

        #region GUI related
        private static List<string> m_names;


        public static string GetUniqueName()
        {
            int i = m_names.Count;
            while (m_names.Contains("Control" + i.ToString()))
                i++;

            if (m_generalLog != null)
                GM.GeneralLog.Write("Name aquired");
            return "Control" + i.ToString();
        }
        public static void RemoveUniqueName(string name)
        {
            m_names.Remove(name);
            if (m_generalLog != null)
                GM.GeneralLog.Write("Name released");
        }

        /// <summary>
        /// Global FontManager
        /// </summary>
        public static Fonts.FontManager FontManager
        {
            get
            {
                return m_fontManager;
            }
        }
        /// <summary>
        /// Global GUIStyleManager
        /// </summary>
        public static GUI.GuiStyleManager GUIStyleManager
        {
            get
            {
                return m_guiStyleManager;
            }
        }
        #endregion;


        public GM()
        {
            m_generalLog = new Log();
            m_fontManager = new Fonts.FontManager();
            m_guiStyleManager = new GUI.GuiStyleManager();
            m_names = new List<string>();
            //m_resourceCache = new ResourceCache();
        }


        #region Static Properties
        /// <summary>
        /// General Log
        /// </summary>
        public static Log GeneralLog
        {
            get
            {
                return m_generalLog;
            }
        }
        /// <summary>
        /// Framework window
        /// </summary>
        public static FrameworkWindow AppWindow
        {
            get
            {
                return m_appWindow;
            }
            set
            {
                m_appWindow = value;
            }
        }
        /// <summary>
        /// Total running time since application start.
        /// </summary>
        public static float RunningTime
        {
            get
            {
                return m_appWindow.RunningTime;
            }
        }
        #endregion

        #region On*Device functions

        public static void OnCreateDevice(Device device)
        {
            if (m_fontManager != null)
            {
                m_fontManager.OnCreateDevice(device);
            }

            if (m_guiStyleManager != null)
            {
                m_guiStyleManager.OnCreateDevice(device);
            }
            //if (m_resourceCache != null)
            //{
            //    m_resourceCache.OnCreateDevice(device);
            //}
        }

        public static void OnResetDevice(Device device)
        {
            if (m_fontManager != null)
            {
                m_fontManager.OnResetDevice(device);
            }
            if (m_guiStyleManager != null)
            {
                m_guiStyleManager.OnResetDevice(device);
            }
            //if (m_resourceCache != null)
            //{
            //    m_resourceCache.OnResetDevice(device);
            //}
        }

        public static void OnLostDevice(Device device)
        {
            if (m_fontManager != null)
            {
                m_fontManager.OnLostDevice(device);
            }
            if (m_guiStyleManager != null)
            {
                m_guiStyleManager.OnLostDevice(device);
            }
            //if (m_resourceCache != null)
            //{
            //    m_resourceCache.OnLostDevice(device);
            //}
        }
        public static void OnDestroyDevice(Device device)
        {
            if (m_fontManager != null)
            {
                m_fontManager.OnDestroyDevice(device);
            }
            if (m_guiStyleManager != null)
            {
                m_guiStyleManager.OnDestroyDevice(device);
            }
            //if (m_resourceCache != null)
            //{
            //    m_resourceCache.OnDestroyDevice(device);
            //}
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.Collect();
            if (m_generalLog != null)
            {
                m_generalLog.Write("Log is disposing");
                m_generalLog.Dispose();
                m_generalLog = null;
            }

            if (m_fontManager != null)
            {
                m_fontManager.Dispose();
                m_fontManager = null;
            }
            if (m_guiStyleManager != null)
            {
                m_guiStyleManager.Dispose();
                m_guiStyleManager = null;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Framework
{
    /// <summary>
    /// A static class that contains some frequently used values as static properties.
    /// </summary>
    static partial class DefaultValues
    {

        public static string FontName
        {
            get
            {
                return "Arial";
            }
        }

        public static float G
        {
            get
            {
                return 10f;
            }
        }
        #region Sizes
        public static int ListVisibleItems
        {
            get
            {
                return 4;
            }
        }
        public static float TextSize
        {
            get
            {
                return 22f;
            }
        }
        public static float TitleTextSize
        {
            get
            {
                return 35f;
            }
        }
        public static int SceenWidth
        {
            get
            {
                return 800;
            }
        }
        public static int ScreenHeight
        {
            get
            {
                return 700;
            }
        }
        static Size ScreenSize
        {
            get
            {
                return new Size(SceenWidth, ScreenHeight);
            }
        }
        static SizeF GUIWindowSize
        {
            get
            {
                return new SizeF(200, 200);
            }
        }
        #endregion

        #region Paths
        public static string ShaderPath
        {
            get
            {
                return "..\\..\\shaders\\";
            }
        }
        public static string FontPath
        {
            get
            {
                return "..\\..\\media\\";
            }
        }
        public static string MeshPath
        {
            get
            {
                return "..\\..\\media\\";
            }
        }
        public static string MediaPath
        {
            get
            {
                return "..\\..\\media\\";
            }
        }
        public static string LogPath
        {
            get
            {
                return "..\\..\\log\\";
            }
        }

        public static float BulletLifetime
        {
            get
            {
                return 10f;
            }
        }

        public static float RocketLifetime
        {
            get
            {
                return 50f;
            }
        }

        #endregion
    }
}

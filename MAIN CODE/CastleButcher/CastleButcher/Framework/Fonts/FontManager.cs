using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Framework.Fonts
{
    /// <summary>
    /// Basic font manager.
    /// </summary>
    class FontManager : IDisposable
    {
        private List<FontFamily> m_fonts = new List<FontFamily>();
        private int m_default = 0;

        public FontManager()
        {

        }
        
        /// <summary>
        /// Adds a new font to collection
        /// </summary>
        /// <param name="fntFile">Font's .fnt file</param>
        /// <param name="bitmapFile">Font's .tga file</param>
        /// <returns>BitmapFont object.</returns>
        public BitmapFont AddFont(string fntFile)
        {
            BitmapFont result = null;
            foreach (FontFamily fam in m_fonts)
            {
                result=fam.GetFont(fntFile);
                if (result != null)
                    return result;
            }
            //nie ma jeszcze tego, dodajemy
            result = new BitmapFont(fntFile);
            foreach (FontFamily fam in m_fonts)
            {
                if (fam.FamilyName == result.Name)
                {
                    fam.AddFont(result);
                    return result;
                }
            }
            //nie ma jeszcze odpowiedniej FontFamily
            m_fonts.Add(new FontFamily(result));
            return result;
        }

        #region On*Device functions
        public void OnCreateDevice(Device device)
        {
            foreach (FontFamily f in m_fonts)
            {
                f.OnCreateDevice(device);
            }
        }

        public void OnResetDevice(Device device)
        {
            foreach (FontFamily f in m_fonts)
            {
                f.OnResetDevice(device);
            }
        }
        public void OnLostDevice(Device device)
        {
            foreach (FontFamily f in m_fonts)
            {
                f.OnLostDevice(device);
            }
        }
        public void OnDestroyDevice(Device device)
        {
            foreach (FontFamily f in m_fonts)
            {
                f.OnDestroyDevice(device);
            }
        }
        #endregion

        #region Getting fonts
        /// <summary>
        /// Gets font by name(for example "Arial")
        /// </summary>
        /// <param name="name">Font's name</param>
        /// <returns>BitmapFont object</returns>
        public FontFamily GetFamily(string familyName)
        {
            if (familyName == "Default" || familyName == "default")
                return GetDefaultFamily();
            foreach (FontFamily f in m_fonts)
            {
                if (f.FamilyName == familyName)
                    return f;
            }
            return null;
        }
        /// <summary>
        /// Gets defaut font
        /// </summary>
        /// <returns>BitmapFont object</returns>
        public FontFamily GetDefaultFamily()
        {
            if (m_fonts.Count > m_default)
                return m_fonts[m_default];
            else
                return null;
        }
        /// <summary>
        /// Gets font by it's .fnt file
        /// </summary>
        /// <param name="fntFile">.fnt file</param>
        /// <returns>BitmapFont object</returns>
        public BitmapFont GetFont(string fntFile)
        {
            BitmapFont result;
            foreach (FontFamily fam in m_fonts)
            {
                result = fam.GetFont(fntFile);
                if (result != null)
                    return result;
            }
            //nie ma jeszcze tego, dodajemy
            result = new BitmapFont(fntFile);
            foreach (FontFamily fam in m_fonts)
            {
                if (fam.FamilyName == result.Name)
                {
                    fam.AddFont(result);
                    return result;
                }
            }
            //nie ma jeszcze odpowiedniej FontFamily
            m_fonts.Add(new FontFamily(result));
            return result;
        }
        public BitmapFont GetFont(string fontFamily, float size)
        {
            FontFamily fam=GetFamily(fontFamily);
            if (fam != null)
                return fam.GetFont(size);
            else
                return null;
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_fonts != null)
            {
                for (int i = 0; i < m_fonts.Count; i++)
                {
                    if (m_fonts[i] != null)
                    {
                        m_fonts[i].Dispose();
                        m_fonts[i] = null;
                    }
                }
                m_fonts = null;
            }

        }

        #endregion
    }
}

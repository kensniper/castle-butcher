using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Framework.Fonts
{
    class FontFamily:IDeviceRelated,IDisposable
    {
        string m_familyName;
        List<BitmapFont> m_fonts=new List<BitmapFont>();

        public FontFamily(string name)
        {
            m_familyName = name;
        }
        public FontFamily(BitmapFont font)
        {
            m_familyName = font.Name;
            m_fonts.Add(font);
        }

        public string FamilyName
        {
            get { return m_familyName; }
        }
        public BitmapFont AddFont(BitmapFont font)
        {
            if (font.Name == m_familyName)
            {
                if (!m_fonts.Contains(font))
                {
                    m_fonts.Add(font);
                    m_fonts.Sort();
                    GM.GeneralLog.Write("FontFamily " + m_familyName + ":dodano czcionke " + font.Size);
                    return font;
                }
                else
                    return null;
            }
            else
                return null;
        }
        public BitmapFont AddFont(string fntFile)
        {
            BitmapFont font = new BitmapFont(fntFile);
            if (font != null && font.Name==m_familyName)
            {
                if (!m_fonts.Contains(font))
                {
                    m_fonts.Add(font);
                    m_fonts.Sort();
                    return font;
                }
                else
                    return null;
            }
            else
                return null;            
        }

        public bool RemoveFont(BitmapFont font)
        {
            
            if (m_fonts.Contains(font))
            {
                m_fonts.Remove(font);
                GM.GeneralLog.Write("FontFamily " + m_familyName + ":usuniêto czcionke " + font.Size);
                return true;
            }
            else
                return false;
            
        }
        public void RemoveAll()
        {
            m_fonts.Clear();
        }

        public BitmapFont GetFont(float size)
        {
            if (m_fonts.Count == 0)
                return null;
            int i = 0;
            while (i < m_fonts.Count-1 && m_fonts[i].Size < size)
                i++;
            return m_fonts[i];
        }
        public BitmapFont GetFont(string fntFile)
        {
            if (m_fonts.Count == 0)
                return null;
            foreach (BitmapFont f in m_fonts)
            {
                if (f.FntFile == fntFile)
                    return f;
            }
            return null;
        }

        public List<Quad> GetProcessedQuads(StringBlock b)
        {
            BitmapFont font = GetFont(b.Size);
            if (font != null)
                return font.GetProcessedQuads(b);
            else
                return new List<Quad>();
        }




        #region IDeviceRelated Members

        public void OnCreateDevice(Device device)
        {
            foreach (BitmapFont f in m_fonts)
            {
                f.OnCreateDevice(device);
            }
        }

        public void OnResetDevice(Device device)
        {
            foreach (BitmapFont f in m_fonts)
            {
                f.OnResetDevice(device);
            }
        }
        public void OnLostDevice(Device device)
        {
            foreach (BitmapFont f in m_fonts)
            {
                f.OnLostDevice(device);
            }
        }
        public void OnDestroyDevice(Device device)
        {
            foreach (BitmapFont f in m_fonts)
            {
                f.OnDestroyDevice(device);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (BitmapFont f in m_fonts)
            {
                f.Dispose();
            }
        }

        #endregion
    }
}

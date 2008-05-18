using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Framework
{
    /// <summary>
    /// A class that provides basic log functionality
    /// </summary>
    class Log : IDisposable
    {
        protected StreamWriter m_file;
        protected int m_numTabs;
        protected string m_tabs;
        /// <summary>
        /// Constructs a log for writing in file "Log.txt"
        /// </summary>
        public Log()
        {
            //we open a file for writing
            //if it exists its content is destroyed
            try
            {
                m_file = new StreamWriter(DefaultValues.LogPath + "Log.txt", false);
                WriteHeader();
                m_numTabs = 0;
                m_tabs = "";
            }
            catch
            {

            }
        }

        /// <summary>
        /// Constructs a log for writing in a file specified by
        /// name parameter
        /// </summary>
        /// <param name="name">File's path</param>
        public Log(string name)
        {
            //we open a file for writing
            //if it exists its content is destroyed
            try
            {
                m_file = new StreamWriter(DefaultValues.LogPath + name, false);
                WriteHeader();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Constructs a log for writing in a file specified by
        /// name parameter 
        /// </summary>
        /// <param name="name">File's path</param>
        /// <param name="header">Log's header</param>
        public Log(string name, string header)
        {
            try
            {
                //we open a file for writing
                //if it exists its content is destroyed
                m_file = new StreamWriter(DefaultValues.LogPath + name, false);
                //Log's header
                m_file.WriteLine(header);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Writes standard header into Log's file
        /// </summary>
        protected virtual void WriteHeader()
        {
            if(m_file!=null)
                m_file.WriteLine(System.DateTime.Now.ToString());
        }

        /// <summary>
        /// Writes a string into the file
        /// and adds a newline at the end.
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public virtual void Write(string text)
        {
            if (m_file != null)
                m_file.WriteLine(m_tabs + text);
        }
        /// <summary>
        /// Writes a string into the file
        /// and adds a newline at the end and increases current indent.
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public virtual void BeginBlock(string text)
        {
            Write(text);
            m_numTabs++;
            m_tabs += "\t";
        }
        /// <summary>
        /// Writes a string into the file
        /// and adds a newline at the end and decreases current indent.
        /// </summary>
        /// <param name="text">Text to be written.</param>
        public virtual void EndBlock(string text)
        {
            m_numTabs--;
            m_tabs = m_tabs.Remove(m_tabs.Length - 1);
            Write(text);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if(m_file!=null)
                m_file.Close();
        }

        #endregion
    }
}

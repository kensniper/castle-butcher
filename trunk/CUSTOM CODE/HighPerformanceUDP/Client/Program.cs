using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           Application.Run(new StartForm());
           // Application.Run(new Form1());
        }
    }
}
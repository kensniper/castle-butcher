using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    static class Program
    {
        private static UDPClientServerCommons.Client.ClientSide cs = new UDPClientServerCommons.Client.ClientSide();

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

            //Application.Run(new GameplayForm(cs));
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Fatal(e.Exception);
        }
    }
}
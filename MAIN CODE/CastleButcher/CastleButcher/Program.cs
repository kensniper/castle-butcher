using System;
using System.Windows.Forms;
using Framework;
using Framework.GUI;
using System.Drawing;
using System.Threading;
using CastleButcher.GameEngine;

namespace CastleButcher
{
    


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            using (Framework.GM gmanager = new Framework.GM())
            {
                using (Framework.FrameworkWindow form = new Framework.FrameworkWindow(true, AppConfig.WindowWidth, AppConfig.WindowHeight))
                {
                    form.Text = "CastleButcher";
                    
                    ///All resources that are allocated during OnCreate/OnResetDevice should be created
                    ///before InitializeGraphics(), so that when the device is created those resources
                    ///can be properly allocated. Creating resources after InitializeGraphics() can lead to 
                    ///crashes because of unallocated resources(After InitializeGraphics() OnCreateDevice may not
                    ///be called).
                    
                    


                    //add guistyles/fonts
                    GM.GUIStyleManager.AddStyle(DefaultValues.MediaPath + "mystyle.xml");
                    GM.GUIStyleManager.AddStyle(DefaultValues.MediaPath + "PlayerInfoStyle.xml");
                    GM.GUIStyleManager.SetCurrentStyle("mystyle");

                    if (!form.InitializeGraphics())
                    {
                        MessageBox.Show("Unable to initialize DirectX");
                        form.Dispose();
                    }

                    //push some layers to display
                    form.PushLayer(new AppControl(form));

                    SoundSystem.SoundEngine.InitializeEngine(GM.AppWindow);
                    Application.Idle += new EventHandler(form.OnApplicationIdle);
                    Application.Run(form);
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
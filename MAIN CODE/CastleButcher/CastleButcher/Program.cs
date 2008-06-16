using System;
using System.Windows.Forms;
using Framework;
using Framework.GUI;
using System.Drawing;
using System.Threading;
using CastleButcher.GameEngine;
using Microsoft.DirectX;
using Framework.MyMath;

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

                    MyVector up = new MyVector(0, 1, 0);
                    MyVector v = new MyVector(-21, -0.3f, 1.2f);
                    MyVector v2 = new MyVector(v.X, 0, v.Z);
                    v.Normalize();
                    v2.Normalize();
                    MyVector right = v ^ up;
                    right.Normalize();

                    Matrix rot=Matrix.LookAtRH(new Vector3(0, 0, 0), new Vector3(v.X,v.Y,v.Z), new Vector3(0, 1, 0));
                    rot.Invert();
                    Matrix rot2=Matrix.LookAtRH(new Vector3(0, 0, 0), new Vector3(v2.X,v2.Y,v2.Z), new Vector3(0, 1, 0));
                    rot2.Invert();
                    Quaternion q = Quaternion.RotationMatrix(rot);
                    MyQuaternion lookOrientation = (MyQuaternion)q;
                    Quaternion q2 = Quaternion.RotationMatrix(rot2);
                    MyQuaternion walkOrientation = (MyQuaternion)q2;

                    MyVector vp = new MyVector(0, 0, -1);
                    vp.Rotate(lookOrientation);
                    MyVector v2p = new MyVector(0, 0, -1);
                    v2p.Rotate(walkOrientation);





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

                    SoundSystem.SoundEngine.InitializeEngine(GM.AppWindow, 0, 0.05f, 1);
                    SoundSystem.SoundEngine.NameFinder.MusicDirectoryPath = GameSettings.Default.MusicPath;
                    SoundSystem.SoundEngine.NameFinder.SoundDirectoryPath = GameSettings.Default.SoundPath;

                    //SoundSystem.SoundEngine.ListenerUpVector = new Vector3(0, 1, 0);
                    //SoundSystem.SoundEngine.Update(new Vector3(10, -15, 12), new Vector3(0, 0, -1),new Vector3(0,1,0));
                    //SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.fanfare1, new Vector3(10, -15, 12));
                    Application.Idle += new EventHandler(form.OnApplicationIdle);
                    Application.Run(form);
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}
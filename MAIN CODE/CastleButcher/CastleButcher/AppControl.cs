using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Framework;
using Microsoft.DirectX.Direct3D;
using Framework.Fonts;
using Framework.GUI;
using CastleButcher.GameEngine;
using CastleButcher.UI;
using CastleButcher.GameEngine.Resources;
using System.Threading;


namespace CastleButcher
{
    /// <summary>
    /// Przyk³adowa warstwa
    /// </summary>
    class AppControl : ControlContainer
    {
        FrameworkWindow frameworkWindow;

        UI.MainMenu mainMenu;
        MainView mainView;

        bool firstRun = true;

        public AppControl(FrameworkWindow window)
        {
            isTransparent = true;
            locksKeyboard = false;
            locksMouse = false;
            frameworkWindow = window;

            mainMenu = new CastleButcher.UI.MainMenu(false);
            mainMenu.OnExitGame += delegate { this.hasFinished = true; window.CloseWindow(); };
            mainMenu.OnNewGame+=new WindowDataHandler(this.StartGame);
            mainMenu.OnResumeGame+=new ButtonEventHandler(this.ResumeGame);

            window.PushLayer(mainMenu);
            

        }

        private void ResumeGame()
        {
            frameworkWindow.RemoveLayer(mainMenu);
            World.Instance.Paused = false;
            Cursor.Hide();
            SoundSystem.SoundEngine.StopMusic();
            SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round1Music);
        }

        public void StartGame(object d)
        {
            //World.Instance.
            NewGameData data = (NewGameData)d;
            ProgressReporter reporter = new ProgressReporter();
            Thread bkgLoader = new Thread(new ParameterizedThreadStart(delegate(object o)
            {
                object[] tab=(object[])o;
                World.FromFileBkg((string)tab[0], (ProgressReporter)tab[1]);
            }));
            bkgLoader.Start(new object[] { AppConfig.MapPath+"respawn_config.xml", reporter });
            //World.FromFileBkg("respawn_config.xml", reporter);
            //World.Instance=new
            //ShipClass shipClass=ObjectCache.Instance.GetShipClass(data.PlayerShip);
            frameworkWindow.RemoveLayer(mainView);
            mainView=new MainView(reporter,new Player(data.PlayerName,null));
            //mainView = new MainView(reporter, null);
            //mainView = new MainView(reporter, null);
            frameworkWindow.RemoveLayer(mainMenu);
            frameworkWindow.PushLayer(mainView);
            Cursor.Hide();
        }

        #region IDeviceRelated implementation
        public override void OnCreateDevice(Device device)
        {
            base.OnCreateDevice(device);
            ResourceCache.Instance.OnCreateDevice(device);
            ObjectCache.Instance.OnCreateDevice(device);
        }
        public override void OnResetDevice(Device device)
        {
            base.OnResetDevice(device);
            ResourceCache.Instance.OnResetDevice(device);
            ObjectCache.Instance.OnResetDevice(device);
        }
        public override void OnLostDevice(Device device)
        {
            base.OnLostDevice(device);
            ResourceCache.Instance.OnLostDevice(device);
            ObjectCache.Instance.OnLostDevice(device);
        }
        public override void OnDestroyDevice(Device device)
        {
            base.OnDestroyDevice(device);
            ResourceCache.Instance.OnDestroyDevice(device);
            ObjectCache.Instance.OnDestroyDevice(device);
        }
        public override void OnRenderFrame(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.OnRenderFrame(device, elapsedTime);

            
        }

        public override void OnUpdateFrame(Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);
            if (firstRun)
            {
                
                SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.welcomeMusic);
                firstRun = false;
            }
            //GM.AppWindow.Text = GM.AppWindow.FPS.ToString();
        }
        public override void OnKeyboard(List<Keys> pressedKeys, List<Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            foreach (Keys k in pressedKeys)
            {
                if(k == Keys.Escape)
                {

                    if (GM.AppWindow.GetLayer(1) != mainMenu)
                    {                        
                        
                        GM.AppWindow.PushLayer(mainMenu);
                        mainMenu.GameInProgress = true;
                        World.Instance.Paused = true;
                        Cursor.Show();

                        SoundSystem.SoundEngine.StopMusic();
                        SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.welcomeMusic);
                    }
                    else
                    {
                        hasFinished = true;
                        GM.AppWindow.CloseWindow();
                    }
                }
                else if(k == KeyMapping.Default.ToggleFullscreen)
                {
                    GM.AppWindow.LockKey(KeyMapping.Default.ToggleFullscreen);                        
                    GM.AppWindow.ToggleFullscreen();                        
                }
            }
        }
        #endregion
    }
}

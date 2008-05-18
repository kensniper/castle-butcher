using System;
using System.Collections.Generic;
using System.Text;
using Framework.GUI;
using Framework;
using Framework.MyMath;
using System.Drawing;

namespace CastleButcher.UI
{
    class MainMenu:ControlContainer
    {
        ///Powrot do gry
        ///
        ///Nowa gra
        ///Podlaczenie do serwera
        ///Wyjscie
        ///
        bool gameInProgress;

        
        bool rtgButtonAdded;


        //rozmiary i pozycje przycisków
        GuiButton resumeGame;
        GuiButton newGame;
        GuiButton joinGame;
        GuiButton exitGame;

        PointF orientationPoint;

        /// <summary>
        /// Zdarzenie sygnalizowane, gdy w menu zostanie wybrana opcja kontynuowania gry.
        /// </summary>
        public event ButtonEventHandler OnResumeGame;

        /// <summary>
        /// Zdarzenie sygbnalizowane, gdy w menu zostanie wybrana opcja wyjœcia z gry.
        /// </summary>
        public event ButtonEventHandler OnExitGame;

        /// <summary>
        /// Zdarzenie sygnalizowane, gdy w menu zostanie wybrana opcja rozpoczêcia nowej gry.
        /// </summary>
        public event WindowDataHandler OnNewGame;

        public event WindowDataHandler OnJoinServer;

        /// <summary>
        /// G³ówne menu gry.
        /// </summary>
        public MainMenu(bool gameInProgress)
            :base()
        {

            this.gameInProgress = gameInProgress;

            isTransparent = false;
            locksMouse = true;
            locksKeyboard = true;

            //AddControl(new NewGameDialog());


            InitializeButtons();

            
        }
        private void InitializeButtons()
        {
            //Usuwamy poprzednie przyciski jesli takie by³y
            if (resumeGame!=null)
                RemoveControl(resumeGame);
            if (newGame != null)
                RemoveControl(newGame);
            if (joinGame != null)
                RemoveControl(joinGame);
            if (exitGame != null)
                RemoveControl(exitGame);

            orientationPoint = new PointF(GM.AppWindow.GraphicsParameters.WindowSize.Width / 2 - 100, (int)(GM.AppWindow.GraphicsParameters.WindowSize.Height/2 -200));

            GM.GUIStyleManager.SetCurrentStyle("mystyle");
            resumeGame = new GuiButton("Wróæ do gry", new RectangleF(new PointF(orientationPoint.X, orientationPoint.Y - 50), new SizeF(200, 40)));
            resumeGame.OnClick += new ButtonEventHandler(this.ReturnToGame);
            //rtgButton = button;

            if (GameInProgress)
            {
                AddControl(resumeGame);
                rtgButtonAdded = true;
            }
            else
                rtgButtonAdded = false;

            newGame = new GuiButton("Nowa gra", new RectangleF(orientationPoint, new SizeF(200, 40)));
            newGame.OnClick += new ButtonEventHandler(this.NewGame);
            AddControl(newGame);

            joinGame = new GuiButton("Po³¹cz z serwerem", new RectangleF(new PointF(orientationPoint.X, orientationPoint.Y + 50), new SizeF(200, 40)));
            joinGame.OnClick += new ButtonEventHandler(this.Join);
            AddControl(joinGame);

            exitGame = new GuiButton("Wyjœcie", new RectangleF(new PointF(orientationPoint.X, orientationPoint.Y + 100), new SizeF(200, 40)));
            exitGame.OnClick += new ButtonEventHandler(this.ExitGame);
            AddControl(exitGame);
        }




        private void ExitGame()
        {
            //hasFinished = true;
            if(OnExitGame!=null)
                OnExitGame();
        }
        private void Join()
        {
            //
        }
        private void ReturnToGame()
        {
            //hasFinished = true;
            if (OnResumeGame != null)
                OnResumeGame();
        }
        private void NewGame()
        {
            
            NewGameDialog dialog = new NewGameDialog();
            AddControl(dialog);
            dialog.OnExit += OnNewGame;

        }
        public bool GameInProgress
        {
            get { return gameInProgress; }
            set 
            {
                if (value && !gameInProgress)
                {
                    AddControl(resumeGame);
                    rtgButtonAdded = true;
                }
                else if (!value && gameInProgress)
                {
                    this.RemoveControl(resumeGame);
                    rtgButtonAdded = false;
                }
                gameInProgress = value;

            }
        }


        public override void OnResetDevice(Microsoft.DirectX.Direct3D.Device device)
        {
            base.OnResetDevice(device);
            
            InitializeButtons();
        }
        public override void OnUpdateFrame(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            if (GameInProgress && !rtgButtonAdded)
            {
                AddControl(resumeGame);
                rtgButtonAdded=true;
            }

            if(!GameInProgress && rtgButtonAdded)
            {
                RemoveControl(resumeGame);
            }
            base.OnUpdateFrame(device, elapsedTime);
        }

        public override void OnMouse(Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

        }



    }
}

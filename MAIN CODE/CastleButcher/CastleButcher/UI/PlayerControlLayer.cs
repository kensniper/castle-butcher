using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Framework.Fonts;
using Framework.GUI;
using Microsoft.DirectX.Direct3D;
using CastleButcher.GameEngine;
using System.Drawing;
using Microsoft.DirectX;

namespace CastleButcher.UI
{
    class PlayerControlLayer : ControlContainer
    {
        GuiTextLabel velocity;
        GuiTextLabel shield;
        GuiTextLabel hp;
        GuiTextLabel slash;

        GuiTextLabel currentWeapon;
        GuiTextLabel currentRocket;
        GuiTextLabel weaponAmmo;

        GuiTextLabel weaponEnergy;
        GuiTextLabel numObjects;
        GuiTextLabel numDeaths;
        GuiTextLabel numFrags;
        GuiTextLabel statusInfo;

        //GuiTextLabel 

        HUD.CrosshairControl crosshair;
        HUD.CastleMap castleMap;

        Player player;

        PlayerList playerList;
        bool playerListAdded = false;
        bool mapAdded = false;
        SteeringLayer slay;
        GameController gameController;



        public Shaker CameraShaker
        {
            get { return slay.CameraShaker; }
        }

        

        public bool ShowPlayerList
        {
            get
            {
                return playerListAdded;
            }
            set
            {
                playerListAdded = false;
                RemoveControl(playerList);
            }
        }


        public PlayerControlLayer(Player player,GameController controller)
            : base()
        {
            this.isTransparent = true;
            this.RecievesEmptyMouse = true;
            RecievesEmptyMouse = true;
            

            gameController = controller;
            slay = new SteeringLayer(player);
            SizeF size = GM.AppWindow.GraphicsParameters.WindowSize;
            GM.GUIStyleManager.SetCurrentStyle("PlayerInfo");
            this.isTransparent = true;
            this.locksKeyboard = false;
            this.locksMouse = false;
            velocity = new GuiTextLabel("", new RectangleF(10, 40, 150, 18), 18);
            shield = new GuiTextLabel("", new RectangleF(10, 60, 70, 18), 18);
            slash = new GuiTextLabel("", new RectangleF(80, 60, 10, 18), 18);
            hp = new GuiTextLabel("100", new RectangleF(100, 60, 70, 18), 18);
            numObjects = new GuiTextLabel("", new RectangleF(10, 80, 200, 18), 18);

            int bottom = (int)GM.AppWindow.GraphicsParameters.WindowSize.Height;
            currentWeapon = new GuiTextLabel("", new RectangleF(10, bottom - 100, 250, 18), 18);
            currentRocket = new GuiTextLabel("", new RectangleF(10, bottom - 80, 150, 18), 18);
            weaponAmmo = new GuiTextLabel("", new RectangleF(10, bottom - 60, 100, 18), 18);
            weaponEnergy = new GuiTextLabel("", new RectangleF(10, bottom - 60, 150, 18), 18);

            numFrags = new GuiTextLabel("0", new RectangleF(10, bottom - 40, 150, 18), 18);
            numDeaths = new GuiTextLabel("0", new RectangleF(10, bottom - 20, 150, 18), 18);
            statusInfo = new GuiTextLabel("", new RectangleF(0, 100, size.Width, 40), 40, "Ghotic",
                Align.Center, Color.Black);
            AddControl(statusInfo);

            AddControl(velocity);
            AddControl(shield);
            AddControl(hp);
            AddControl(slash);
            AddControl(numObjects);

            AddControl(currentWeapon);
            //AddControl(currentRocket);
            AddControl(weaponAmmo);
            //AddControl(weaponEnergy);

            AddControl(numFrags);
            AddControl(numDeaths);

            crosshair = new CastleButcher.UI.HUD.CrosshairControl(new PointF(size.Width / 2, size.Height / 2));
            //AddControl(crosshair);
            castleMap = new CastleButcher.UI.HUD.CastleMap(new PointF(size.Width / 2, size.Height / 2));
            RenderCrosshair = false;

            //radar = new CastleButcher.UI.HUD.RadarControl(new PointF(size.Width - 256, size.Height - 256));
            ////AddControl(radar);

            //speedometer = new CastleButcher.UI.HUD.SpeedometerControl(player.CharacterClass.MovementParameters.MaxVelocity,
            //    new PointF(size.Width / 2, size.Height - 200));
            //AddControl(speedometer);

            playerList = new PlayerList(new RectangleF(100, 100, size.Width - 200, size.Height - 200));

            World.Instance.OnPlayerAdded += new PlayerEventHandler(playerList.AddPlayer);
            World.Instance.OnPlayerRemoved += new PlayerEventHandler(playerList.RemovePlayer);

            World.Instance.OnPlayerKilled += new PlayerEventHandler(delegate(Player pl)
            {
                if (pl == this.player)
                {
                    if (!playerListAdded)
                    {
                        AddControl(playerList);
                        playerListAdded = true;
                    }
                }
            });

            World.Instance.OnPlayerRespawned += new PlayerEventHandler(delegate(Player pl)
            {
                if (pl == this.player)
                {
                    if (playerListAdded)
                    {
                        RemoveControl(playerList);
                        playerListAdded = false;
                    }

                }
            });


            playerList.AddPlayer(player);

            this.player = player;
        }

        public bool RenderCrosshair
        {
            get
            {
                return !crosshair.IsDisabled;
            }
            set
            {
                crosshair.IsDisabled = !value;
                if (crosshair.IsDisabled)
                    RemoveControl(crosshair);
                else
                    AddControl(crosshair);
            }
        }
        public override void OnResetDevice(Device device)
        {
            base.OnResetDevice(device);
            SizeF size = GM.AppWindow.GraphicsParameters.WindowSize;

            playerList.ProcessRect(new RectangleF(100, 100, size.Width - 200, size.Height - 200));
            crosshair.Position = new PointF(size.Width / 2, size.Height / 2);
            castleMap.Position = new PointF(size.Width / 2, size.Height / 2);
            //radar.Position = new PointF(size.Width - 256, size.Height - 256);
            //speedometer.Position = new PointF(size.Width / 2, size.Height - 200);

            int bottom = (int)GM.AppWindow.GraphicsParameters.WindowSize.Height;



            currentWeapon.Position = new PointF(10, bottom - 100);
            //currentRocket.Position = new PointF(10, bottom - 80);
            weaponAmmo.Position = new PointF(10, bottom - 60);
            //weaponEnergy.Position = new PointF(10, bottom - 60);

        }

        public override void OnUpdateFrame(Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);
            
            if (gameController.GameStatus == GameStatus.WaitingForStart)
            {
                if (statusInfo.Text != "Oczekiwanie na innych graczy")
                    statusInfo.Text = "Oczekiwanie na innych graczy";
            }
            else
            {
                if (statusInfo.Text != "")
                    statusInfo.Text = "";
            }

            if (mapAdded)
            {
                castleMap.Update();
            }


            if (player.CurrentCharacter != null)
            {
                slay.OnUpdateFrame(device, elapsedTime);
                SoundSystem.SoundEngine.Update((Vector3)player.CurrentCharacter.Position, (Vector3)player.CurrentCharacter.LookDirection, new Vector3(0, 1, 0));
                SoundSystem.SoundEngine.UpdateSoundList();
                SoundSystem.SoundEngine.UpdateBackgroundMusic();


                velocity.Text = player.CurrentCharacter.CharacterController.SetVelocity.ToString() + ":" +
                    player.CurrentCharacter.Velocity.ToString();
                //velocity.Text = slay.CameraShaker.Position.ToString();
                //velocity.Text = player.CurrentShip.RigidBodyData.AngularVelocity.Length.ToString();
                shield.Text = "S:" + player.CurrentCharacter.ArmorState.Shield.ToString();
                hp.Text = "HP:" + player.CurrentCharacter.ArmorState.Hp.ToString();
                if (player.CurrentCharacter.Weapons.CurrentWeapon != null)
                    numObjects.Text = "WeaponReady:" + player.CurrentCharacter.Weapons.CurrentWeapon.Ready.ToString();
                else
                    numObjects.Text = "";

                numFrags.Text = "Frags: " + player.Frags.ToString();
                numDeaths.Text = "Deaths: " + player.Deaths.ToString();

                //weaponEnergy.Text = "E:" + player.CurrentShip.WeaponEnergy;
                currentWeapon.Text = (player.CurrentCharacter.Weapons.CurrentWeapon != null) ? "Weapon: " + player.CurrentCharacter.Weapons.CurrentRanged.WeaponClass.Name : "Weapon: None";

                //speedometer.SetSpeed = sdev.SetVelocity;
                //speedometer.CurrentSpeed = player.CurrentShip.Velocity.Length;

                if (player.CurrentCharacter.Weapons.CurrentWeapon != null && player.CurrentCharacter.Weapons.CurrentWeaponType ==
                    CastleButcher.GameEngine.Weapons.WeaponType.Ranged)
                {
                    weaponAmmo.Text = "Ammo:" + player.CurrentCharacter.Weapons.CurrentRanged.Ammo.ToString();
                }
                else
                    weaponAmmo.Text = "";

            }
            else
            {
                
            }
        }

        public override void OnRenderFrame(Device device, float elapsedTime)
        {
            base.OnRenderFrame(device, elapsedTime);
            StringBlock b = new StringBlock("FPS:" + ((int)GM.AppWindow.FPS).ToString(), new RectangleF(10, 10, 120, 50), Align.Left, 22, ColorValue.FromColor(Color.Red), false);
            BitmapFont font = GM.FontManager.GetFamily("Arial").GetFont(22);
            font.Render(device, font.GetProcessedQuads(b));
        }

        public override void OnMouse(Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

            if (true || player.IsAlive)
            {
                slay.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
                //sdev.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
            }
            else
            {

            }
            
            if (player.IsAlive == false)
            {
                //if (pressedButtons[0] == true)
                //{
                //    World.Instance.RespawnPlayer(player);
                //}
            }
            else
            {
                //if (pressedButtons[0] == true)
                //{
                //    player.CurrentShip.FireMainWeapon();
                //}

                if (pressedButtons[0] == true)
                {
                    player.CurrentCharacter.FireWeapon();
                }
            }
            GM.AppWindow.CenterCursor();
        }

        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            slay.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            if (player.IsAlive)
            {
                if (pressedKeys.Contains(System.Windows.Forms.Keys.F2) && playerListAdded == false)
                {
                    AddControl(playerList);
                    playerListAdded = true;
                }
                if (releasedKeys.Contains(System.Windows.Forms.Keys.F2) && playerListAdded == true)
                {
                    RemoveControl(playerList);
                    playerListAdded = false;
                }
                if (pressedKeys.Contains(System.Windows.Forms.Keys.Tab) && mapAdded == false)
                {
                    AddControl(castleMap);
                    mapAdded = true;
                }
                if (releasedKeys.Contains(System.Windows.Forms.Keys.Tab) && mapAdded == true)
                {
                    RemoveControl(castleMap);
                    mapAdded = false;
                }
            }
        }
    }
}





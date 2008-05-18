using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Framework.Fonts;
using Framework.GUI;
using Microsoft.DirectX.Direct3D;
using CastleButcher.GameEngine;
using System.Drawing;

namespace CastleButcher.UI
{
    class PlayerControl : ControlContainer
    {
        GuiTextLabel velocity;
        GuiTextLabel shield;
        GuiTextLabel hp;
        GuiTextLabel slash;

        GuiTextLabel currentLaser;
        GuiTextLabel currentRocket;
        GuiTextLabel rocketAmmo;

        GuiTextLabel weaponEnergy;
        GuiTextLabel numObjects;
        GuiTextLabel numDeaths;
        GuiTextLabel numFrags;

        //GuiTextLabel 

        HUD.CrosshairControl crosshair;
        HUD.RadarControl radar;
        HUD.SpeedometerControl speedometer;

        Player player;

        PlayerList playerList;
        bool playerListAdded = false;


        public PlayerControl(Player player)
            : base()
        {
            SizeF size = GM.AppWindow.GraphicsParameters.WindowSize;
            GM.GUIStyleManager.SetCurrentStyle("PlayerInfo");
            this.isTransparent = true;
            this.locksKeyboard = false;
            this.locksMouse = false;
            velocity = new GuiTextLabel("", new RectangleF(10, 40, 150, 18), 18);
            shield = new GuiTextLabel("", new RectangleF(10, 60, 70, 18), 18);
            slash = new GuiTextLabel("", new RectangleF(80, 60, 10, 18), 18);
            hp = new GuiTextLabel("100", new RectangleF(100, 60, 70, 18), 18);
            numObjects = new GuiTextLabel("", new RectangleF(10, 80, 70, 18), 18);

            int bottom = (int)GM.AppWindow.GraphicsParameters.WindowSize.Height;
            currentLaser = new GuiTextLabel("", new RectangleF(10, bottom - 100, 150, 18), 18);
            currentRocket = new GuiTextLabel("", new RectangleF(10, bottom - 80, 150, 18), 18);
            rocketAmmo = new GuiTextLabel("", new RectangleF(165, bottom - 80, 30, 18), 18);
            weaponEnergy = new GuiTextLabel("", new RectangleF(10, bottom - 60, 150, 18), 18);

            numFrags = new GuiTextLabel("0", new RectangleF(10, bottom - 40, 150, 18), 18);
            numDeaths = new GuiTextLabel("0", new RectangleF(10, bottom - 20, 150, 18), 18);


            //AddControl(velocity);
            //AddControl(shield);
            //AddControl(hp);
            //AddControl(slash);
            //AddControl(numObjects);

            //AddControl(currentLaser);
            //AddControl(currentRocket);
            //AddControl(rocketAmmo);
            //AddControl(weaponEnergy);

            //AddControl(numFrags);
            //AddControl(numDeaths);

            //crosshair = new CastleButcher.UI.HUD.CrosshairControl(new PointF(size.Width / 2, size.Height / 2));
            //AddControl(crosshair);

            //radar = new CastleButcher.UI.HUD.RadarControl(new PointF(size.Width - 256, size.Height - 256));
            ////AddControl(radar);

            //speedometer = new CastleButcher.UI.HUD.SpeedometerControl(player.CharacterClass.MovementParameters.MaxVelocity,
            //    new PointF(size.Width / 2, size.Height - 200));
            //AddControl(speedometer);

            playerList = new PlayerList(new RectangleF(100, 100, size.Width - 200, size.Height - 200));

            World.Instance.OnAddPlayer += new PlayerEventHandler(playerList.AddPlayer);
            World.Instance.OnRemovePlayer += new PlayerEventHandler(playerList.RemovePlayer);

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

        public override void OnResetDevice(Device device)
        {
            base.OnResetDevice(device);
            SizeF size = GM.AppWindow.GraphicsParameters.WindowSize;

            //playerList.ProcessRect(new RectangleF(100, 100, size.Width - 200, size.Height - 200));
            //crosshair.Position = new PointF(size.Width / 2, size.Height / 2);
            //radar.Position = new PointF(size.Width - 256, size.Height - 256);
            //speedometer.Position = new PointF(size.Width / 2, size.Height - 200);

            int bottom = (int)GM.AppWindow.GraphicsParameters.WindowSize.Height;



            //currentLaser.Position = new PointF(10, bottom - 100);
            //currentRocket.Position = new PointF(10, bottom - 80);
            //rocketAmmo.Position = new PointF(165, bottom - 80);
            //weaponEnergy.Position = new PointF(10, bottom - 60);

        }

        public override void OnUpdateFrame(Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);




            //velocity.Text = player.CurrentShip.AutoPilot.SetVelocity.ToString() + ":"+
            //    ((int)player.CurrentShip.Velocity.Length).ToString()+" - " + player.CurrentShip.Velocity.ToString();
            //velocity.Text = player.CurrentShip.RigidBodyData.AngularVelocity.Length.ToString();
            //shield.Text = "S:" + player.CurrentShip.ArmorState.Shield.ToString();
            //hp.Text = "HP:" + player.CurrentShip.ArmorState.Hull.ToString();
            //numObjects.Text = "nObj:" + World.Instance.GameObjects.Count.ToString();

            //numFrags.Text = "Frags: " +sdev.TurnX.ToString();//+ player.Frags.ToString();
            //numDeaths.Text = "Deaths: " + sdev.turnSpeedX.ToString();//+ player.Deaths.ToString();

            //weaponEnergy.Text = "E:" + player.CurrentShip.WeaponEnergy;
            //currentLaser.Text = (player.CurrentShip.Weapons.CurrentLaser != null) ? "Laser: " + player.CurrentShip.Weapons.CurrentLaser.WeaponClass.Name : "Laser: None";

            //speedometer.SetSpeed = sdev.SetVelocity;
            //speedometer.CurrentSpeed = player.CurrentShip.Velocity.Length;

            //if (player.CurrentShip.Weapons.CurrentRocket == null)
            //{
            //    currentRocket.Text = "Rocket: None";
            //    rocketAmmo.Text = "";
            //}
            //else
            //{
            //    currentRocket.Text = "Rocket: " + player.CurrentShip.Weapons.CurrentRocket.WeaponClass.Name;
            //    rocketAmmo.Text = player.CurrentShip.Weapons.CurrentRocket.Ammo.ToString();
            //}




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

            if (player.IsAlive == false)
            {
                if (pressedButtons[0] == true)
                {
                    World.Instance.RespawnPlayer(player);
                }
            }
            else
            {
                //if (pressedButtons[0] == true)
                //{
                //    player.CurrentShip.FireMainWeapon();
                //}

                //if (pressedButtons[1] == true)
                //{
                //    player.CurrentShip.FireRocket();
                //}
            }
        }

        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
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
            }
        }
    }
}





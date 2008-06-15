using System;
using System.Collections.Generic;
using System.Text;
using Framework.GUI;
using System.Drawing;
using Framework;
using System.IO;
using CastleButcher.GameEngine;
using UDPClientServerCommons;
using UDPClientServerCommons.Packets;
namespace CastleButcher.UI
{

    class JoinGameData
    {

        public string PlayerName;
        public UDPClientServerCommons.Client.ClientSide ClientSide;
        public UDPClientServerCommons.Packets.GameInfoPacket GameInfo;
    }

    /// <summary>
    /// Dialog tworzenia nowej gry. Pobiera od użytkownika dane o rozmiarze planszy i o graczach. Po kliknięciu OK
    /// uruchamiane jest zdarzenie OnExit do którego są przekazane zebrane dane.
    /// 
    /// </summary>
    class JoinGameDialog : GuiWindow
    {
        GuiButton ok;
        GuiButton cancel;



        GuiEditBox playerName;

        GuiListBox gameList;

        UDPClientServerCommons.Client.ClientSide clientSide;
        List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();




        public JoinGameDialog()
            : base("Dołącz się do gry", new RectangleF(GM.AppWindow.GraphicsParameters.WindowSize.Width / 2 - 190,
            GM.AppWindow.GraphicsParameters.WindowSize.Height / 2 - 200, 380, 400))
        {
            ok = new GuiButton("Start", new RectangleF(240, 300, 100, 34));
            cancel = new GuiButton("Anuluj", new RectangleF(155, 300, 80, 34));

            ok.OnClick += new ButtonEventHandler(OnOK);
            ok.Disable();
            cancel.OnClick += new ButtonEventHandler(Close);
            AddControl(ok);
            AddControl(cancel);

            AddControl(new GuiTextLabel("Nick:", new RectangleF(20, 10, 150, 22), 22));
            AddControl(new GuiTextLabel("Lista Gier:", new RectangleF(20, 35, 150, 22), 22));
            gameList = new GuiListBox(new RectangleF(20, 60, 320, 230), 8);
            gameList.OnSelectionChange += this.UpdateSelection;

            playerName = new GuiEditBox(Properties.Settings.Default.PlayerName, new RectangleF(190, 10, 150, 22), 18);

            AddControl(playerName);
            AddControl(gameList);



            //LoadShipData();
            //LoadMapData();

            clientSide = new UDPClientServerCommons.Client.ClientSide(4444);
            bool ret=clientSide.StartLookingForLANGames();

        }

        //private void LoadShipData()
        //{
        //    int selectedItem = -1;
        //    int index=0;
        //    foreach (KeyValuePair<string,string> shipClass in ObjectCache.Instance.ShipClasses)
        //    {
        //        this.shipSelection.AddItem(shipClass.Key,shipClass.Value);
        //        if (((string)shipClass.Value) == Properties.Settings.Default.ShipClass)
        //            selectedItem = index;

        //        index++;
        //    }
        //    if (selectedItem != -1)
        //    {
        //        shipSelection.SelectedItem = selectedItem;
        //    }


        //    //shipSelection.Visi
        //}
        private void UpdateSelection()
        {
            if (gameList.SelectedItemData != null)
            {
                ok.Enable();
            }          
            else
            {
                ok.Disable();
            }

        }

        private void OnOK()
        {
            Properties.Settings.Default.PlayerName = playerName.Text;

            JoinGameData data = new JoinGameData();
            data.PlayerName = playerName.Text;
            data.ClientSide = clientSide;
            data.GameInfo = (UDPClientServerCommons.Packets.GameInfoPacket)gameList.SelectedItemData;
            this.Close(data);

        }

        public override void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            UpdateList();
            base.Render(device, elapsedTime);

        }
        private void UpdateList()
        {
            gameList.RemoveAll();
            for (int i = 0; i < clientSide.CurrentLanGames.Count; i++)
            {
                gameList.AddItem(clientSide.CurrentLanGames[i].ServerAddress.ToString() +
                    " Graczy:" + clientSide.CurrentLanGames[i].PlayerStatusList.Count, clientSide.CurrentLanGames[i]);

            }
        }
    }
}

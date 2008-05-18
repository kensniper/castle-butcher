using System;
using System.Collections.Generic;
using System.Text;
using Framework.GUI;
using System.Drawing;
using Framework;
using System.IO;
using CastleButcher.GameEngine;
namespace CastleButcher.UI
{
    class NewGameData
    {
        public MapDescriptor MapDescriptor;
        public string PlayerName;
        public string PlayerShip;
    }

    /// <summary>
    /// Dialog tworzenia nowej gry. Pobiera od u¿ytkownika dane o rozmiarze planszy i o graczach. Po klikniêciu OK
    /// uruchamiane jest zdarzenie OnExit do którego s¹ przekazane zebrane dane.
    /// 
    /// </summary>
    class NewGameDialog:GuiWindow
    {
        GuiButton ok;
        GuiButton cancel;


        GuiEditBox playerName;
        GuiEditBox serverName;
        GuiNumberBox maxPlayers;


        List<MapDescriptor> mapDescriptors=new List<MapDescriptor>();



        public NewGameDialog()
            :base("Stwórz grê",new RectangleF(GM.AppWindow.GraphicsParameters.WindowSize.Width/2-190,
            GM.AppWindow.GraphicsParameters.WindowSize.Height/2-200, 380,190))
        {
            ok = new GuiButton("Start", new RectangleF(240, 100, 100, 34));
            cancel = new GuiButton("Anuluj", new RectangleF(155, 100, 80, 34));

            ok.OnClick+=new ButtonEventHandler(OnOK);
            //ok.Disable();
            cancel.OnClick+=new ButtonEventHandler(Close);
            AddControl(ok);
            AddControl(cancel);

            AddControl(new GuiTextLabel("Nick", new RectangleF(20, 10, 150, 22), 22));
            AddControl(new GuiTextLabel("Nazwa gry:",new RectangleF(20, 35, 150, 22),22));
            AddControl(new GuiTextLabel("Max. graczy", new RectangleF(20, 60, 150, 22),22));
            
            

            playerName = new GuiEditBox(Properties.Settings.Default.PlayerName, new RectangleF(190, 10, 150, 22), 18);
            serverName = new GuiEditBox("Nowa gra", new RectangleF(190, 35, 150, 22), 18);
            maxPlayers = new GuiNumberBox(4, new RectangleF(190, 60, 150, 22), 18);
            AddControl(playerName);
            AddControl(serverName);
            AddControl(maxPlayers);


            
            //LoadShipData();
            //LoadMapData();

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
            //MapDescriptor desc=(MapDescriptor)mapSelection.SelectedItemData;
            //if (desc != null)
            //{
            //    mapName.Text = desc.MapName;
            //    maxPlayers.Text = desc.MaxPlayers.ToString();
            //    if (shipSelection.SelectedItem >= 0)
            //    {
            //        ok.Enable();
            //        Properties.Settings.Default.ShipClass = (string)shipSelection.SelectedItemData;
            //    }
            //}
            //else
            //{
            //    mapName.Text = "";
            //    maxPlayers.Text = "";
            //    ok.Disable();
            //}

        }

        private void OnOK()
        {
            Properties.Settings.Default.PlayerName = playerName.Text;

            NewGameData data = new NewGameData();
            data.MapDescriptor = null;
            data.PlayerName = playerName.Text;
            this.Close(data);

        }

        //private void LoadMapData()
        //{
        //    DirectoryInfo dInfo = new DirectoryInfo(AppConfig.MapPath);
        //    FileInfo[] files = dInfo.GetFiles("*.xml");
        //    mapDescriptors.Clear();

        //    foreach (FileInfo fi in files)
        //    {
        //        MapDescriptor desc = MapDescriptor.FromFile(fi.FullName);
        //        if (desc != null)
        //            mapDescriptors.Add(desc);
        //    }
        //    //mapSelection.RemoveAll();
        //    foreach (MapDescriptor desc in mapDescriptors)
        //    {
        //        mapSelection.AddItem(desc.MapName, desc);
        //    }
        //}
    }
}

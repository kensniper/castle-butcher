using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Clutch.Net.UDP;
using System.Net;
using UDPClientServerCommons;

namespace HighPerformanceUDP
{
    public partial class MyUdpServerForm : Form
    {
        private ServerSide serverSide= null;

        public MyUdpServerForm()
        {
            InitializeComponent();
            //serverSide = new ServerSide(new IPEndPoint(IPAddress.Parse("10.0.0.3"),1234));
            serverSide = new ServerSide(1234);
            GameOptions gameOptions = new GameOptions("Poland","Niemcy", UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit,10);
            serverSide.StartLANServer(gameOptions);
}        
    }
}
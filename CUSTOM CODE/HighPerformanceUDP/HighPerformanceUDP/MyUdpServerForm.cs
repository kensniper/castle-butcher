using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Clutch.Net.UDP;

namespace HighPerformanceUDP
{
    public partial class MyUdpServerForm : Form
    {
        private MyUdpServer myUdpServer = null;

        public MyUdpServerForm()
        {
            myUdpServer = new MyUdpServer(1234);
            myUdpServer.MessageWasReceivedEvent += new EventHandler(myUdpServer_MessageWasReceivedEvent);
            InitializeComponent();
        }

        public delegate void UpdateStatus(string status);

        private void UpdateStatusFromUIThread(string status)
        {
            this.txtReceived.Text = status + System.Environment.NewLine + " -------- " + System.Environment.NewLine + txtReceived.Text;
        }

        void myUdpServer_MessageWasReceivedEvent(object sender, EventArgs e)
        {
            UDPPacketBuffer buf = sender as UDPPacketBuffer;
            if (buf != null)
            {
                txtReceived.Invoke(new UpdateStatus(UpdateStatusFromUIThread), Encoding.UTF8.GetString(buf.Data, 0, buf.DataLength));
            }
            else
            {
                string msg = sender as string;
                if(msg!=null)
                    txtReceived.Invoke(new UpdateStatus(UpdateStatusFromUIThread),msg);
            }

        }

        private void MyUdpServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            myUdpServer.close();
        }
    }
}
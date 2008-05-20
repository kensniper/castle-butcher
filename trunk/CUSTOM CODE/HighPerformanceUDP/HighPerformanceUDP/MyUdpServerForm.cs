using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Clutch.Net.UDP;
using System.Net;

namespace HighPerformanceUDP
{
    public partial class MyUdpServerForm : Form
    {
        private MyUdpServer myUdpServer = null;
        private UDPClientServerCommons.ServerPacket serverPacket = new UDPClientServerCommons.ServerPacket();
        private Dictionary<int, UDPClientServerCommons.ServerPacket> LastPackages = new Dictionary<int, UDPClientServerCommons.ServerPacket>();
        private UDPClientServerCommons.Last10 last10 = new UDPClientServerCommons.Last10();
        private List<EndPoint> cliendAdressList = new List<EndPoint>();
        private readonly object serverPacketLock = new object();

        private System.Threading.Timer timer;

        public MyUdpServerForm()
        {
            myUdpServer = new MyUdpServer(1234);
            myUdpServer.MessageWasReceivedEvent += new EventHandler(myUdpServer_MessageWasReceivedEvent);
            InitializeComponent();

            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null,300, 100);
        }

        public void timerCallbackMethod(object target)
        {
            lock (serverPacketLock)
            {
                UDPPacketBuffer buff = new UDPPacketBuffer();
                serverPacket.Timestamp = DateTime.Now;
                serverPacket.PacketId = (ushort)((serverPacket.PacketId + 1) % ushort.MaxValue);
                buff.Data = serverPacket.ToMinimalByte();
                buff.DataLength = serverPacket.ToMinimalByte().Length;

                UDPClientServerCommons.ServerPacket srv = new UDPClientServerCommons.ServerPacket(buff.Data);

                for (int i = 0; i < cliendAdressList.Count; i++)
                {
                    buff.RemoteEndPoint = cliendAdressList[i];
                    myUdpServer.AsyncBeginSend(buff);
                }
                if (cliendAdressList.Count > 0)
                {
                    if (LastPackages.ContainsKey(last10.Counter))
                        LastPackages[last10.Counter] = serverPacket;
                    else
                        LastPackages.Add(last10.Counter, serverPacket);
                    last10.Increase();
                }
            }
        }


        public delegate void UpdateStatus(string status);

        private void UpdateStatusFromUIThread(string status)
        {
            this.txtReceived.Text = status + System.Environment.NewLine + " -------- " + System.Environment.NewLine + txtReceived.Text;
        }

        void myUdpServer_MessageWasReceivedEvent(object sender, EventArgs e)
        {
            UDPPacketBuffer buff = sender as UDPPacketBuffer;

            if (buff != null)
            {
                try
                {
                    UDPClientServerCommons.PacketType packetType = UDPClientServerCommons.PacketTypeChecker.Check(buff.Data);
                    switch (packetType)
                    {
                        case(UDPClientServerCommons.PacketType.Join):
                            UDPClientServerCommons.JoinPacket joinPacket = new UDPClientServerCommons.JoinPacket(buff.Data);
                            cliendAdressList.Add(buff.RemoteEndPoint);

                            UDPClientServerCommons.AckPacket ackPacket = new UDPClientServerCommons.AckPacket();
                            ackPacket.PacketIdAck = joinPacket.PacketId;
                            ackPacket.PlayerId = joinPacket.PlayerId;

                            UDPPacketBuffer buffer = new UDPPacketBuffer();
                            buffer.RemoteEndPoint = buff.RemoteEndPoint;
                            buffer.Data = ackPacket.ToMinimalByte();
                            buffer.DataLength = ackPacket.ToMinimalByte().Length;
                            myUdpServer.AsyncBeginSend(buffer);

                            lock (serverPacketLock)
                            {
                                UDPClientServerCommons.PlayerInfo playerInfo = new UDPClientServerCommons.PlayerInfo();
                                playerInfo.PlayerId = joinPacket.PlayerId;
                                serverPacket.PlayerInfoList.Add(playerInfo);
                                serverPacket.NumberOfPlayers = (ushort)cliendAdressList.Count;
                            }
                            break;
                        case(UDPClientServerCommons.PacketType.Standard):
                            lock (serverPacketLock)
                            {
                                UDPClientServerCommons.ClientPacket clientPacket = new UDPClientServerCommons.ClientPacket(buff.Data);
                                for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                                {
                                    if (clientPacket.PlayerId == serverPacket.PlayerInfoList[i].PlayerId)
                                    {
                                        serverPacket.PlayerInfoList[i] = UDPClientServerCommons.Translator.TranslateBetweenClientPacketAndPlayerInfo(clientPacket);
                                    }
                                }
                            }
                            break;
                        case(UDPClientServerCommons.PacketType.ACK):
                            break;                            
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
            }
        }

        private void MyUdpServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            myUdpServer.close();
        }
    }
}
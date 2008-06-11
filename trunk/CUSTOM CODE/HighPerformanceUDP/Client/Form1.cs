using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using Clutch.Net.UDP;
using System.Threading;
using UDPClientServerCommons;
using UDPClientServerCommons.Packets;

namespace Client
{
    public partial class Form1 : Form
    {
        private UDPClientServerCommons.Client.ClientSide ClientSideNetworking;

        private ServerPacket serverPacket = null;
        private ClientPacket clientPacket = new ClientPacket();
        private readonly object serverPacketLock = new object(); 

        public Form1(int port)
        {
            IPEndPoint ServerIpep = new IPEndPoint(IPAddress.Parse("10.0.0.3"), 1234);

            InitializeComponent();
            ClientSideNetworking = new UDPClientServerCommons.Client.ClientSide(port);
            ClientSideNetworking.PacketReceivedEvent += new UDPClientServerCommons.Client.ClientSide.packetReceived(ClientSideNetworking_PacketReceivedEvent);
            ClientSideNetworking.JoinGame(ServerIpep, "Karp", 1,1);

            button1.Visible = false;
/*
            Random rand = new Random();
            S
            clientPacket.PacketId = 1;
            clientPacket.PlayerId = (ushort)rand.Next(1000, 9999);
            this.Name = "Player " + clientPacket.PlayerId.ToString();
            clientPacket.PlayerPosition = new UDPClientServerCommons.Vector(0f, 0f, 0f);
            clientPacket.PlayerMovementDirection = new UDPClientServerCommons.Vector(0f, 0f, 0f);

            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, 100);*/
        }
        delegate void RefreshEvent();

        void ClientSideNetworking_PacketReceivedEvent(UDPClientServerCommons.Interfaces.IPacket packet)
        {
            if (packet.PacketType == UDPClientServerCommons.Constants.PacketTypeEnumeration.StandardServerPacket)
            {
                lock (serverPacketLock)
                {
                    this.serverPacket = (UDPClientServerCommons.Packets.ServerPacket)packet;
                }
            }
            this.Invoke(new RefreshEvent(RefreshForm), null);
        }

 /*       public void timerCallbackMethod(object target)
        {
            lock (PackageLock)
            {
                clientPacket.PacketId = (ushort)((clientPacket.PacketId + 1) % ushort.MaxValue);
                udpSocket.SendTo(clientPacket.ToByte(), ServerIpep);
            }
        }*/

        void RefreshForm()
        {
            this.Refresh();
        }

        void DoOne()
        {
            Form1 formOne = new Form1(4443);
            formOne.Show();
        }
        /*
        public void Start()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, udpPort);
            udpSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            udpSocket.Bind(ipep);
            AsyncBeginReceive();
            
        }*/

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ClientSideNetworking.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /*
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (udpSocket != null)
                {
                    packetSendDate = DateTime.Now;
                    udpSocket.SendTo(Encoding.UTF8.GetBytes(txtSend.Text), ServerIpep);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

       /* private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }


        private void AsyncBeginReceive()
        {
            lock (typeof(Constants))
            {
                if (!Constants.isClosing)
                {
                    // allocate a packet buffer
                    UDPPacketBuffer buf = new UDPPacketBuffer();

                    try
                    {
                        // kick off an async read
                        udpSocket.BeginReceiveFrom(
                            buf.Data,
                            0,
                            UDPPacketBuffer.BUFFER_SIZE,
                            SocketFlags.None,
                            ref buf.RemoteEndPoint,
                            new AsyncCallback(AsyncEndReceive),
                            buf);
                    }
                    catch (SocketException se)
                    {
                        txtReceive.Text = se.Message + System.Environment.NewLine + txtReceive.Text;
                    }
                }
            }
        }

        private void AsyncEndReceive(IAsyncResult iar)
        {
            // Asynchronous receive operations will complete here through the call
            // to AsyncBeginReceive

            // start another receive - this keeps the server going!
            lock (typeof(Constants))
                if (!Constants.isClosing)
                {
                    AsyncBeginReceive();

                    // get the buffer that was created in AsyncBeginReceive
                    // this is the received data
                    UDPPacketBuffer buffer = (UDPPacketBuffer)iar.AsyncState;

                    try
                    {
                        // get the length of data actually read from the socket, store it with the
                        // buffer
                        buffer.DataLength = udpSocket.EndReceiveFrom(iar, ref buffer.RemoteEndPoint);

                        // call the abstract method PacketReceived(), passing the buffer that
                        // has just been filled from the socket read.
                        PacketReceived(buffer);
                    }
                    catch (SocketException se)
                    {
                        txtReceive.Text = se.Message + System.Environment.NewLine + txtReceive.Text;
                    }
                }
        }

        private void PacketReceived(UDPPacketBuffer buffer)
        {
            try
            {
                lock (ServerPackagesLock)
                {
                    if (LastPackages.ContainsKey(last10.Counter))
                        LastPackages[last10.Counter] = new UDPClientServerCommons.ServerPacket(buffer.Data);
                    else
                    LastPackages.Add(last10.Counter, new UDPClientServerCommons.ServerPacket(buffer.Data));
                    last10.Increase();                   
                }
                this.Invoke(new UpdateTextCallback(UpdateStatus), "some shit");
            }
            catch (Exception ex)
            {
                txtReceive.Text = ex.Message + System.Environment.NewLine + txtReceive.Text;
            }
        }

        public delegate void UpdateTextCallback(string text);

        private void UpdateStatus(string newStatus)
        {
           // txtReceive.Text = newStatus + System.Environment.NewLine + txtReceive.Text;
            this.Refresh();
        }

        private void UpdateStatusFromUIThread(string newStatus)
        {
            txtReceive.Invoke(new UpdateTextCallback(this.UpdateStatus), newStatus);
        }
        */

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {            
            switch (e.KeyCode)
            {
                case (Keys.Up):
                  //  lock (PackageLock)
                    {
                        clientPacket.TimeStamp = DateTime.Now;

                        clientPacket.PlayerPosition.Y--;
                        clientPacket.PlayerMovementDirection.X = clientPacket.PlayerPosition.X;
                        clientPacket.PlayerMovementDirection.Y = float.MinValue;
                    }
                    break;
                case (Keys.Down):
                    //lock (PackageLock)
                    {
                        clientPacket.TimeStamp = DateTime.Now;
                        
                        clientPacket.PlayerPosition.Y++;
                        clientPacket.PlayerMovementDirection.X = clientPacket.PlayerPosition.X;
                        clientPacket.PlayerMovementDirection.Y = float.MaxValue;
                    }
                    break;
                case (Keys.Left):
                    //lock (PackageLock)
                    {
                        clientPacket.TimeStamp = DateTime.Now;
                        
                        clientPacket.PlayerPosition.X--;
                        clientPacket.PlayerMovementDirection.Y = clientPacket.PlayerPosition.Y;
                        clientPacket.PlayerMovementDirection.X = float.MinValue;
                    }
                    break;
                case (Keys.Right):
                    //lock (PackageLock)
                    {
                        clientPacket.TimeStamp = DateTime.Now;
                        
                        clientPacket.PlayerPosition.X++;
                        clientPacket.PlayerMovementDirection.Y = clientPacket.PlayerPosition.Y;
                        clientPacket.PlayerMovementDirection.X = float.MaxValue;
                    }
                    break;

            }
            //ClientSideNetworking.UpdateDataToSend(clientPacket, false);
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                e.Graphics.DrawEllipse(new Pen(Color.Red, 2.0f), clientPacket.PlayerPosition.X, clientPacket.PlayerPosition.Y, 2.0f, 2.0f);
                //this.Refresh();
                lock (serverPacketLock)
                {
                    if (serverPacket == null)
                        return;
                    ServerPacket serv = serverPacket;
                    int number = serv.NumberOfPlayers;
                    for (int i = 0; i < number; i++)
                    {
                        if (serv.PlayerInfoList[i].PlayerId != clientPacket.PlayerId)
                        {
                            //Color color = Color.FromArgb((int)LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerId);
                            Color color = Color.Black;
                            e.Graphics.DrawEllipse(new Pen(color, 2.0f), serv.PlayerInfoList[i].PlayerPosition.X, serv.PlayerInfoList[i].PlayerPosition.Y, 2.0f, 2.0f);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            //object[] param = { e.Graphics };  
            //DoInvoke inv = new DoInvoke(DoStuff);
            //this.Invoke(inv, param);
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {/*
            ThreadStart startOne = new ThreadStart(DoOne);
            Thread threadOne = new Thread(startOne);
            threadOne.Start();
          */
            DoOne();
        }
        /*
        
        public delegate void DoInvoke(object[] param);

        private void DoStuff(object[] param)
        {
            if (param != null && param.Length != 0)
            {
                Graphics g = param[0] as Graphics;
                if (g != null)
                {
                    lock (ServerPackagesLock)
                    {
                        int number = LastPackages[last10.GetPrevoius(1)].NumberOfPlayers;
                        for (int i = 0; i < number; i++)
                        {
                            if (LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerId != clientPacket.PlayerId)
                            {
                                Color color = Color.FromArgb((int)LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerId);
                                g.DrawEllipse(new Pen(color, 2.0f), LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerPosition.X, LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerPosition.Y, 2.0f, 2.0f);
                            }
                        }
                    }
                    g.Dispose();
                    this.Refresh();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            //ThreadStart startOne = new ThreadStart(DoOne);
            //Thread threadOne = new Thread(startOne);
            //threadOne.Start();
            DoOne();
            Start();
            Join();
            Thread.Sleep(100);
            timer.Change(0, 100);
        }

        private void Join()
        {
            UDPClientServerCommons.JoinPacket joinPacket = new UDPClientServerCommons.JoinPacket();
            joinPacket.GameId = 1;
            joinPacket.PlayerId = clientPacket.PlayerId;
            joinPacket.PlayerName = "Karpik " + joinPacket.PlayerId.ToString();

            //UDPPacketBuffer buffer = new UDPPacketBuffer();
            //buffer.Data = joinPacket.ToByte();
            //buffer.DataLength=joinPacket.ToByte().Length;
            udpSocket.SendTo(joinPacket.ToByte(), ServerIpep);
        }
        */
    }
}
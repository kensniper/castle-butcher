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

namespace Client
{
    public partial class Form1 : Form
    {
        private UDPClientServerCommons.ClientPacket clientPacket = new UDPClientServerCommons.ClientPacket();
        private UDPClientServerCommons.ServerPacket serverPacket = new UDPClientServerCommons.ServerPacket();
        private Dictionary<int, UDPClientServerCommons.ServerPacket> LastPackages = new Dictionary<int, UDPClientServerCommons.ServerPacket>();
        private UDPClientServerCommons.Last10 last10 = new UDPClientServerCommons.Last10();

        private readonly object PackageLock = new object();
        private readonly object ServerPackagesLock = new object();
        // the port to listen on
        private int udpPort=4444;

        private DateTime packetSendDate;

        private System.Threading.Timer timer;

        // the UDP socket
        private Socket udpSocket;

        private IPEndPoint ServerIpep;

        public Form1(int port)
        {
            if (port == udpPort)
                udpPort++;
            else
                udpPort += Math.Abs(udpPort - port) + 1;


            InitializeComponent();
            Random rand = new Random();
            ServerIpep = new IPEndPoint(IPAddress.Parse("172.16.222.86"), 1234);
            clientPacket.PacketId = 1;
            clientPacket.PlayerId = (ushort)rand.Next(1000, 9999);
            this.Name = "Player " + clientPacket.PlayerId.ToString();
            clientPacket.PlayerPosition = new UDPClientServerCommons.Vector(0f, 0f, 0f);
            clientPacket.PlayerMovementDirection = new UDPClientServerCommons.Vector(0f, 0f, 0f);

            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, 100);
        }

        public void timerCallbackMethod(object target)
        {
            lock (PackageLock)
            {
                udpSocket.SendTo(clientPacket.ToByte(), ServerIpep);
            }
        }

        void DoOne()
        {
            Form1 formOne = new Form1(udpPort);
            formOne.Show();
        }

        public void Start()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, udpPort);
            udpSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            udpSocket.Bind(ipep);
            AsyncBeginReceive();
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Constants.isClosing = true;
                lock (typeof(Constants))
                {
                    Constants.isClosing = true;
                    if(udpSocket!=null)
                    udpSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

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

        private void btnStart_Click(object sender, EventArgs e)
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
                //TimeSpan span = DateTime.Now.Subtract(packetSendDate);
                //UpdateStatusFromUIThread("ping " + span.TotalMilliseconds.ToString() + " miliseconds");
                /*
                 UpdateStatusFromUIThread("----" +
                    Encoding.UTF8.GetString(buffer.Data, 0, buffer.DataLength));
                 */
                lock (ServerPackagesLock)
                {
                    if (LastPackages.ContainsKey(last10.Counter))
                        LastPackages[last10.Counter] = new UDPClientServerCommons.ServerPacket(buffer.Data);
                    else
                    LastPackages.Add(last10.Counter, new UDPClientServerCommons.ServerPacket(buffer.Data));
                    last10.Increase();
                }
            }
            catch (Exception ex)
            {
                txtReceive.Text = ex.Message + System.Environment.NewLine + txtReceive.Text;
            }
        }

        public delegate void UpdateTextCallback(string text);

        private void UpdateStatus(string newStatus)
        {
            txtReceive.Text = newStatus + System.Environment.NewLine + txtReceive.Text;
        }

        private void UpdateStatusFromUIThread(string newStatus)
        {
            txtReceive.Invoke(new UpdateTextCallback(this.UpdateStatus), newStatus);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Up):
                    lock (PackageLock)
                    {
                        clientPacket.PlayerPosition.Y--;
                        clientPacket.PlayerMovementDirection.X = clientPacket.PlayerPosition.X;
                        clientPacket.PlayerMovementDirection.Y = float.MinValue;
                    }
                    break;
                case (Keys.Down):
                    lock (PackageLock)
                    {
                        clientPacket.PlayerPosition.Y++;
                        clientPacket.PlayerMovementDirection.X = clientPacket.PlayerPosition.X;
                        clientPacket.PlayerMovementDirection.Y = float.MaxValue;
                    }
                    break;
                case (Keys.Left):
                    lock (PackageLock)
                    {
                        clientPacket.PlayerPosition.X--;
                        clientPacket.PlayerMovementDirection.Y = clientPacket.PlayerPosition.Y;
                        clientPacket.PlayerMovementDirection.X = float.MinValue;
                    }
                    break;
                case (Keys.Right):
                    lock (PackageLock)
                    {
                        clientPacket.PlayerPosition.X++;
                        clientPacket.PlayerMovementDirection.Y = clientPacket.PlayerPosition.Y;
                        clientPacket.PlayerMovementDirection.X = float.MaxValue;
                    }
                    break;

            }
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(new Pen(Color.Red, 2.0f), clientPacket.PlayerPosition.X, clientPacket.PlayerPosition.Y, 2.0f, 2.0f);
            //this.Refresh();
            lock (ServerPackagesLock)
            {
                int number = LastPackages[last10.GetPrevoius(1)].NumberOfPlayers;
                for (int i = 0; i < number; i++)
                {
                    Color color = Color.FromArgb((int)LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerId);
                    e.Graphics.DrawEllipse(new Pen(color, 2.0f), LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerPosition.X, LastPackages[last10.GetPrevoius(1)].PlayerInfoList[i].PlayerPosition.Y, 2.0f, 2.0f);
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
            timer.Change(0, 100);
        }

    }
}
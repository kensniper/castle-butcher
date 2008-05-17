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

namespace Client
{
    public partial class Form1 : Form
    {

        // the port to listen on
        private const int udpPort=4444;

        private DateTime packetSendDate;

        // the UDP socket
        private Socket udpSocket;

        private IPEndPoint ServerIpep;

        public Form1()
        {
            InitializeComponent();
            ServerIpep = new IPEndPoint(IPAddress.Parse("79.184.202.37"), 1234);
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
                TimeSpan span = DateTime.Now.Subtract(packetSendDate);
                UpdateStatusFromUIThread("ping " + span.TotalMilliseconds.ToString());
                /*
                 UpdateStatusFromUIThread("----" +
                    Encoding.UTF8.GetString(buffer.Data, 0, buffer.DataLength));
                 */
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

    }
}
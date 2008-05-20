using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Clutch.Net.UDP;
using System.Threading;

namespace UDPClientServerCommons.Client
{
    public class UDPLayer:IDisposable
    {
        /// <summary>
        /// Time in miliseconds
        /// </summary>
        public const int TimerTickPeriod = 100;

        public delegate ClientPacket GetDataToSend();

        public delegate void PacketReceived(UDPPacketBuffer udpPacketBuffer);

        /// <summary>
        /// Event that is used to get data to send to the server
        /// </summary>
        public event GetDataToSend GetData;

        public event PacketReceived PacketWasReceived;

        private readonly object isClosingLock = new object();
        private bool isClosing = false;

        /// <summary>
        ///  the port to listen on
        /// </summary>
        private int udpPort = 4444;

        // the UDP socket
        private Socket udpSocket;

        private IPEndPoint ServerIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

        /// <summary>
        /// Server adress
        /// </summary>
        public IPEndPoint ServerIp
        {
            get { return ServerIpep; }
            set { ServerIpep = value; }
        }

        /// <summary>
        /// timer
        /// </summary>
        private System.Threading.Timer timer;

        public UDPLayer(int port)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            udpPort = port + random.Next(1, 50);

            Start();
        }

        public UDPLayer()
        {
            Start();
        }

        private void Start()
        {
            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, TimerTickPeriod);
        }

        private void timerCallbackMethod(object target)
        {
                ClientPacket clientPacket = GetData();
                if (clientPacket != null)
                {
                    clientPacket.PacketId = (ushort)((clientPacket.PacketId + 1) % ushort.MaxValue);
                    //udpSocket.SendTo(clientPacket.ToByte(), ServerIpep);
                    SendPacket(clientPacket.ToByte());
                }
        }

        public void StartUdpSocket()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, udpPort);
            udpSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            udpSocket.Bind(ipep);
            AsyncBeginReceive();

            // start sending data
            timer.Change(0, TimerTickPeriod);
        }

        private void AsyncBeginReceive()
        {
            bool tmp = false;

            lock (isClosingLock)
            {
                tmp = isClosing;
            }
                if (!tmp)
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
                        //do something with exception (log it?)
                    }
                    catch (Exception ex)
                    {
                        //do something with exception (log it?)
                    }
                }
        }

        private void AsyncEndReceive(IAsyncResult iar)
        {
            // Asynchronous receive operations will complete here through the call
            // to AsyncBeginReceive

            // start another receive - this keeps the server going!
            bool tmp = false;
            lock (isClosingLock)
            {
                tmp = isClosing;
            }
                if (!tmp)
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
                        PacketWasReceived(buffer);
                    }
                    catch (SocketException se)
                    {
                        //do something with exception (log it?)
                    }
                    catch (Exception ex)
                    {
                        //do something with exception (log it?)
                    }
                }
        }

        private void AsyncBeginSend(UDPPacketBuffer buf)
        {
            bool tmp = false;
            
            lock (isClosingLock)
            {
                tmp=isClosing;
            }
                if (!tmp)
                {
                    try
                    {
                        udpSocket.BeginSendTo(
                            buf.Data,
                            0,
                            buf.DataLength,
                            SocketFlags.None,
                            buf.RemoteEndPoint,
                            new AsyncCallback(AsyncEndSend),
                            buf);
                    }
                    catch (SocketException se)
                    {
                        // do something
                    }
                    catch (Exception ex)
                    {
                        // do something
                    }
                }
        }

        private void AsyncEndSend(IAsyncResult iar)
        {
            bool tmp = false;

            lock (isClosingLock)
            {
                tmp = isClosing;
            }
                if (!tmp)
                {
                    UDPPacketBuffer buffer = (UDPPacketBuffer)iar.AsyncState;

                    try
                    {
                        int bytesSent = udpSocket.EndSendTo(iar);
                    }
                    catch (SocketException se)
                    {
                        // do something
                    }
                    catch (Exception ex)
                    {
                        // do something
                    }
                }
        }

        public void SendPacket(UDPPacketBuffer udpPacketBuffer)
        {
            AsyncBeginSend(udpPacketBuffer);
        }

        public void SendPacket(byte[] data)
        {
            UDPPacketBuffer buffer = new UDPPacketBuffer();
            buffer.Data = data;
            buffer.DataLength = data.Length;
            buffer.RemoteEndPoint = ServerIpep;

            AsyncBeginSend(buffer);
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                lock (isClosingLock)
                {
                    isClosing = true;
                    Thread.Sleep(200);
                    if (udpSocket != null)
                        udpSocket.Close();
                }
            }
            catch (Exception ex)
            {
                // Do something with exception (log it?)
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Clutch.Net.UDP;
using System.Threading;
using UDPClientServerCommons.Packets;

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

        // the ReaderWriterLock is used solely for the purposes of shutdown (Stop()).
        // since there are potentially many "reader" threads in the internal .NET IOCP
        // thread pool, this is a cheaper synchronization primitive than using
        // a Mutex object.  This allows many UDP socket "reads" concurrently - when
        // Stop() is called, it attempts to obtain a writer lock which will then
        // wait until all outstanding operations are completed before shutting down.
        // this avoids the problem of closing the socket with outstanding operations
        // and trying to catch the inevitable ObjectDisposedException.
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        // number of outstanding operations.  This is a reference count
        // which we use to ensure that the threads exit cleanly. Note that
        // we need this because the threads will potentially still need to process
        // data even after the socket is closed.
        private int rwOperationCount = 0;

        // the all important shutdownFlag.  This is synchronized through the ReaderWriterLock.
        private bool shutdownFlag = true;


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
                    clientPacket.PacketId.Next();
                    //udpSocket.SendTo(clientPacket.ToByte(), ServerIpep);
                    SendPacket(clientPacket.ToByte());
                }
        }

        public void StartUdpSocket()
        {
            if (shutdownFlag)
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, udpPort);
                udpSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Dgram,
                    ProtocolType.Udp);
                udpSocket.Bind(ipep);

                // we're not shutting down, we're starting up
                shutdownFlag = false;

                AsyncBeginReceive();


                // start sending data
                timer.Change(0, TimerTickPeriod);
            }
        }

        private void AsyncBeginReceive()
        {
            // this method actually kicks off the async read on the socket.
            // we aquire a reader lock here to ensure that no other thread
            // is trying to set shutdownFlag and close the socket.
            rwLock.AcquireReaderLock(-1);

            if (!shutdownFlag)
            {
                // increment the count of pending operations
                Interlocked.Increment(ref rwOperationCount);

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
                        Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginReceive", se);                        // an error occurred, therefore the operation is void.  Decrement the reference count.
                        Interlocked.Decrement(ref rwOperationCount);
                
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncBeginReceive", ex);                        // an error occurred, therefore the operation is void.  Decrement the reference count.
                        Interlocked.Decrement(ref rwOperationCount);
                
                    }
                }
            // we're done with the socket for now, release the reader lock.
            rwLock.ReleaseReaderLock();
        }

        private void AsyncEndReceive(IAsyncResult iar)
        {
            // Asynchronous receive operations will complete here through the call
            // to AsyncBeginReceive

            // aquire a reader lock
            rwLock.AcquireReaderLock(-1);

            if (!shutdownFlag)
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

                        // this operation is now complete, decrement the reference count
                        Interlocked.Decrement(ref rwOperationCount);

                        // we're done with the socket, release the reader lock
                        rwLock.ReleaseReaderLock();

                        // call the abstract method PacketReceived(), passing the buffer that
                        // has just been filled from the socket read.
                        PacketWasReceived(buffer);
                    }
                    catch (SocketException se)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncEndReceive", se); 
                        Interlocked.Decrement(ref rwOperationCount);

                        // we're done with the socket for now, release the reader lock.
                        rwLock.ReleaseReaderLock();
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncEndReceive", ex);
                        Interlocked.Decrement(ref rwOperationCount);

                        // we're done with the socket for now, release the reader lock.
                        rwLock.ReleaseReaderLock();
                    }
                }
                else
                {
                    // nothing bad happened, but we are done with the operation
                    // decrement the reference count and release the reader lock
                    Interlocked.Decrement(ref rwOperationCount);
                    rwLock.ReleaseReaderLock();
                }
        }

        private void AsyncBeginSend(UDPPacketBuffer buf)
        {
            rwLock.AcquireReaderLock(-1);
            if (!shutdownFlag)
            {
                    try
                    {
                        Interlocked.Increment(ref rwOperationCount);
                    
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
                        Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginSend", se);                        
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncBeginSend", ex);                        
                    }
                }
            rwLock.ReleaseReaderLock();
        }

        private void AsyncEndSend(IAsyncResult iar)
        {
            rwLock.AcquireReaderLock(-1);
            if (!shutdownFlag)
            {
                    UDPPacketBuffer buffer = (UDPPacketBuffer)iar.AsyncState;

                    try
                    {
                        int bytesSent = udpSocket.EndSendTo(iar);
                    }
                    catch (SocketException se)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncEndSend", se);                        
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncEndSend", ex);                        
                    }
                }
            Interlocked.Decrement(ref rwOperationCount);
            rwLock.ReleaseReaderLock();
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
           if (!shutdownFlag)
            {
                // wait indefinitely for a writer lock.  Once this is called, the .NET runtime
                // will deny any more reader locks, in effect blocking all other send/receive
                // threads.  Once we have the lock, we set shutdownFlag to inform the other
                // threads that the socket is closed.
                rwLock.AcquireWriterLock(-1);
                shutdownFlag = true;

                    if (udpSocket != null)
                        udpSocket.Close();
                  rwLock.ReleaseWriterLock();

                // wait for any pending operations to complete on other
                // threads before exiting.
                while (rwOperationCount > 0)
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("Dispose", ex);
            }
        }

        #endregion

        internal void StopSendingByTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}

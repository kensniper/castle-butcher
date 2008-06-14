using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Clutch.Net.UDP;
using System.Threading;

namespace UDPClientServerCommons.Client
{
    public class ClientLanBroadcast:IDisposable
    {
        public delegate void PacketReceived(UDPPacketBuffer udpPacketBuffer);

        /// <summary>
        /// Event fired when data is received
        /// </summary>
        public event PacketReceived PacketWasReceived;

        private static int rwOperationCount = 0;
        private static System.Threading.ReaderWriterLock rwLock = new System.Threading.ReaderWriterLock();
        private static bool shutdownFlag = false;
        private static Socket udpSocket = null;

        private bool isRunningField = false;

        /// <summary>
        /// is broadcast receiving service is running
        /// </summary>
        public bool IsRunning
        {
            get { return isRunningField; }
        }

        public ClientLanBroadcast()
        {        }

        /// <summary>
        /// Starts receiving broadcast packets
        /// </summary>
        /// <returns></returns>
        public bool StartBroadcastReceiving()
        {
            bool result = true;
            if (PacketWasReceived == null)
                return false;

            try
            {
                int GroupPort = 15000;
                IPEndPoint groupEP = null;

                groupEP = new IPEndPoint(MyIp, GroupPort);

                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                udpSocket.Bind(groupEP);
                udpSocket.EnableBroadcast = true;

                AsyncBeginReceive();
                isRunningField = true;
            }
            catch (Exception ex)
            {
                result = false;
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("Couldnt start broadcast receive", ex);
            }
            return result;
        }

        private void Close()
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
                    isRunningField = false;
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Error when closing lan broadcast", ex);
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
                     Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginReceive", se);
                    // an error occurred, therefore the operation is void.  Decrement the reference count.
                    Interlocked.Decrement(ref rwOperationCount);

                }
                catch (Exception ex)
                {
                     Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncBeginReceive", ex);
                    // an error occurred, therefore the operation is void.  Decrement the reference count.
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
                    if (isRunningField)
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

        private IPAddress MyIp
        {
            get
            {
                IPAddress result = null;
                string myHost = System.Net.Dns.GetHostName();
                System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);
                for (int i = 0; i < myIPs.AddressList.Length; i++)
                    //just LAN
                    if (myIPs.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        result = myIPs.AddressList[i];
                        break;
                    }
                return result;
            }
        }

        /// <summary>
        /// pauses receiving packets
        /// </summary>
        public void STOP()
        {
            isRunningField = false;
        }

        /// <summary>
        /// unpauses receiving packets
        /// </summary>
        public void START()
        {
            isRunningField = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}

//////////////////////////////////////////////////
// Copyright 2006, Clutch, Inc.
// Author: Jeff Cesnik
//////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.Sockets;
//using System.ServiceProcess;
using System.Threading;

namespace Clutch.Net.UDP
{
    public abstract class UDPServer
    {
        private const string ServiceName = "UDP_Server";

        private IPEndPoint ServerIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

        public IPEndPoint ServerIpAdress
        {
            get { return ServerIp; }
            set { ServerIp = value; }
        }

        // the port to listen on
        private int udpPort;

        // the UDP socket
        private Socket udpSocket;

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

        // these abstract methods must be implemented in a derived class to actually do
        // something with the packets that are sent and received.
        protected abstract void PacketReceived(UDPPacketBuffer buffer);
        protected abstract void PacketSent(UDPPacketBuffer buffer, int bytesSent);

        public UDPServer(int port)
        {
            this.udpPort = port;
            this.ServerIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }

        public UDPServer(IPEndPoint ServerIpAdress)
        {
            this.udpPort = ServerIp.Port;
            this.ServerIp = ServerIpAdress;
        }

        protected IPEndPoint Start()
        {
            if (shutdownFlag)
            {
                // create and bind the socket 
                IPEndPoint ipep = ServerIp;
                udpSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Dgram,
                    ProtocolType.Udp);
                udpSocket.Bind(ipep);

                // we're not shutting down, we're starting up
                shutdownFlag = false;

                udpSocket.DontFragment = true;
                udpSocket.EnableBroadcast = true;
                udpSocket.Ttl = 4;

                // kick off an async receive.  The Start() method will return, the
                // actual receives will occur asynchronously and will be caught in
                // AsyncEndRecieve().
                // I experimented with posting multiple AsyncBeginReceives() here in an attempt
                // to "queue up" reads, however I found that it negatively impacted performance.
                AsyncBeginReceive();
            }
            return ServerIp;
        }

        protected void Stop()
        {
            if (!shutdownFlag)
            {
                // wait indefinitely for a writer lock.  Once this is called, the .NET runtime
                // will deny any more reader locks, in effect blocking all other send/receive
                // threads.  Once we have the lock, we set shutdownFlag to inform the other
                // threads that the socket is closed.
                rwLock.AcquireWriterLock(-1);
                shutdownFlag = true;
                udpSocket.Close();
                rwLock.ReleaseWriterLock();

                // wait for any pending operations to complete on other
                // threads before exiting.
                while (rwOperationCount > 0)
                    Thread.Sleep(1);
            }
        }

        public bool IsRunning
        {
            // self-explanitory
            get { return !shutdownFlag; }
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
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginReceive", se); 
                    // something bad happened
                    //System.Diagnostics.EventLog.WriteEntry(ServiceName,
                    //    "A SocketException occurred in UDPServer.AsyncBeginReceive():\n\n" + se.Message,
                    //    System.Diagnostics.EventLogEntryType.Error);

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
                // start another receive - this keeps the server going!
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
                    PacketReceived(buffer);
                }
                catch (SocketException se)
                {
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncEndReceive", se); 
                    // something bad happened
                    //System.Diagnostics.EventLog.WriteEntry(ServiceName,
                    //    "A SocketException occurred in UDPServer.AsyncEndReceive():\n\n" + se.Message,
                    //    System.Diagnostics.EventLogEntryType.Error);

                    // an error occurred, therefore the operation is void.  Decrement the reference count.
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

        public void AsyncBeginSend(UDPPacketBuffer buf)
        {
            // by now you should you get the idea - no further explanation necessary

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
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginSend", se); 
                    //System.Diagnostics.EventLog.WriteEntry(ServiceName,
                    //    "A SocketException occurred in UDPServer.AsyncBeginSend():\n\n" + se.Message,
                    //    System.Diagnostics.EventLogEntryType.Error);
                }
            }

            rwLock.ReleaseReaderLock();
        }

        private void AsyncEndSend(IAsyncResult iar)
        {
            // by now you should you get the idea - no further explanation necessary

            rwLock.AcquireReaderLock(-1);

            if (!shutdownFlag)
            {
                UDPPacketBuffer buffer = (UDPPacketBuffer)iar.AsyncState;

                try
                {
                    int bytesSent = udpSocket.EndSendTo(iar);

                    // note that call to the abstract PacketSent() method - we are passing the number
                    // of bytes sent in a separate parameter, since we can't use buffer.DataLength which
                    // is the number of bytes to send (or bytes received depending upon whether this
                    // buffer was part of a send or a receive).
                    PacketSent(buffer, bytesSent);
                }
                catch (SocketException se)
                {
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncEndSend", se); 
                    //System.Diagnostics.EventLog.WriteEntry(ServiceName,
                    //    "A SocketException occurred in UDPServer.AsyncEndSend():\n\n" + se.Message,
                    //    System.Diagnostics.EventLogEntryType.Error);
                }
            }

            Interlocked.Decrement(ref rwOperationCount);
            rwLock.ReleaseReaderLock();
        }
    }
}


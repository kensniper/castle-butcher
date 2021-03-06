﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    class Program
    {
        private static int rwOperationCount =0;
        private static System.Threading.ReaderWriterLock rwLock = new System.Threading.ReaderWriterLock();
        private static bool shutdownFlag = false;
        private static Socket udpSocket = null;
        
        static void Main(string[] args)
        {
            try
            {
                int GroupPort = 15000;
                IPEndPoint groupEP = null;
                if (MessageBox.Show("Do you want to use .Any address?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    groupEP = new IPEndPoint(IPAddress.Any, GroupPort);
                else
                {
                    groupEP = new IPEndPoint(MyIp, GroupPort);
                }

                //udpSocket = new UdpClient(groupEP);
                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                udpSocket.Bind(groupEP);
                udpSocket.EnableBroadcast = true;

                AsyncBeginReceive();

                while (!shutdownFlag)
                {
                    Console.ReadLine();
                }
                //while (true)
                //{
                //    byte[] receiveBytes = udp.Receive(ref groupEP);

                //    string returnData = Encoding.ASCII.GetString(receiveBytes);

                //    Console.WriteLine(returnData);

                //    if (returnData.ToLower() == "q")
                //        break;
                //}
                //udpSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        private static void Close()
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
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        private static void AsyncBeginReceive()
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
                    // Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncBeginReceive", se);
                    // an error occurred, therefore the operation is void.  Decrement the reference count.
                    Interlocked.Decrement(ref rwOperationCount);

                }
                catch (Exception ex)
                {
                    // Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncBeginReceive", ex);
                    // an error occurred, therefore the operation is void.  Decrement the reference count.
                    Interlocked.Decrement(ref rwOperationCount);

                }
            }
            // we're done with the socket for now, release the reader lock.
            rwLock.ReleaseReaderLock();
        }

        private static void AsyncEndReceive(IAsyncResult iar)
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
                    //Diagnostic.NetworkingDiagnostics.Logging.Error("AsyncEndReceive", se);
                    Interlocked.Decrement(ref rwOperationCount);

                    // we're done with the socket for now, release the reader lock.
                    rwLock.ReleaseReaderLock();
                }
                catch (Exception ex)
                {
                    // Diagnostic.NetworkingDiagnostics.Logging.Fatal("AsyncEndReceive", ex);
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

        public static IPAddress MyIp
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

        private static void PacketWasReceived(UDPPacketBuffer buffer)
        {
            string msg = Encoding.UTF8.GetString(buffer.Data, 0, buffer.DataLength);
            if (msg.ToLower() == "q")
            {
                Close();
                Console.WriteLine("Press [ENTER] to quit");
            }
            Write(msg);
        }

        private static void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}

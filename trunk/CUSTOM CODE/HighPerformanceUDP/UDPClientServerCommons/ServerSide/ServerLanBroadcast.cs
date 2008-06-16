using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace UDPClientServerCommons.Server
{
    public class ServerLanBroadcast:IDisposable
    {
        private UdpClient udp = null;
        private IPEndPoint groupEP;
        private readonly object socketLock = new object();

        /// <summary>
        /// creates LanBroadcast class
        /// </summary>
        public ServerLanBroadcast()
        {
            try
            {
                int GroupPort = 15000;

                groupEP = new IPEndPoint(IPAddress.Broadcast, GroupPort);

                udp = new UdpClient(groupEP);
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Server lan broadcast", ex);
            }
        }

        /// <summary>
        /// Sends broadcast in the network
        /// </summary>
        /// <param name="binaryMsg">message to send</param>
        public void Send(byte[] binaryMsg)
        {
            lock (socketLock)
            {
                try
                {
                    if (udp != null)
                        udp.Send(binaryMsg, binaryMsg.Length, groupEP);
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Server lan broadcast Send byte", ex);
                }
            }
        }

        /// <summary>
        /// sends in another thread a broadcast
        /// </summary>
        /// <param name="binaryMsg">binnary message</param>
        public void SendAsync(byte[] binaryMsg)
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Send));
                thread.Start(binaryMsg);
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Server lan broadcast SendAsync", ex);
            }

        }

        private void Send(object obj)
        {
            lock (socketLock)
            {
                try
                {
                    byte[] binary = obj as byte[];
                    if (udp != null)
                        udp.Send(binary, binary.Length, groupEP);
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Server lan broadcast Send", ex);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (socketLock)
            {
                try
                {
                    if (udp != null)
                    {
                        udp.Close();
                        udp = null;
                    }
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Server lan broadcast Dispose", ex);
                }
            }
        }

        #endregion
    }
}

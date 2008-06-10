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

        public ServerLanBroadcast()
        {
            int GroupPort = 15000;

            groupEP = new IPEndPoint(IPAddress.Broadcast, GroupPort);

            udp = new UdpClient(groupEP);
        }


        public void Send(byte[] binaryMsg)
        {
            lock (socketLock)
            {
                if (udp != null)
                    udp.Send(binaryMsg, binaryMsg.Length, groupEP);
            }
        }

        public void SendAsync(byte[] binaryMsg)
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Send));
            thread.Start(binaryMsg);
        }

        public void Send(object obj)
        {
            lock (socketLock)
            {
                byte[] binary = obj as byte[];
                if(udp!=null)
                udp.Send(binary, binary.Length, groupEP);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (socketLock)
            {
                udp.Close();
                udp = null;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Clutch.Net.UDP
{
    public class MyUdpServer : Clutch.Net.UDP.UDPServer,IDisposable
    {
        public event EventHandler MessageWasReceivedEvent;

        protected override void PacketReceived(Clutch.Net.UDP.UDPPacketBuffer buffer)
        {            
            MessageWasReceivedEvent(buffer, new EventArgs());
        }

        protected override void PacketSent(Clutch.Net.UDP.UDPPacketBuffer buffer, int bytesSent)
        {
           // MessageWasReceivedEvent("++ Package was sent",new EventArgs());
        }

        public MyUdpServer(int port):base(port)
        {
        }

        public MyUdpServer(IPEndPoint ServerIp):base(ServerIp)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            base.Stop();
        }

        #endregion
    }
}
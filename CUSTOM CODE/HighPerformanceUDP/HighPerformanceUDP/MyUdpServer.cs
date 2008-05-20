using System;
using System.Collections.Generic;
using System.Text;

namespace HighPerformanceUDP
{
    class MyUdpServer : Clutch.Net.UDP.UDPServer
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
            base.Start();
        }


        internal void close()
        {
            base.Stop();
        }
    }
}

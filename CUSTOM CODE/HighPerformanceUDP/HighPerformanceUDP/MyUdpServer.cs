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
            /*
            MessageWasReceivedEvent(buffer, new EventArgs());
            Clutch.Net.UDP.UDPPacketBuffer bufferToSend = new Clutch.Net.UDP.UDPPacketBuffer();
            bufferToSend.Data = Encoding.UTF8.GetBytes("Hello this is Server, You Said=\""+Encoding.UTF8.GetString(buffer.Data,0,buffer.DataLength)+"\" and time: "+ DateTime.Now.ToLongTimeString());
            bufferToSend.DataLength = bufferToSend.Data.Length;
            bufferToSend.RemoteEndPoint = buffer.RemoteEndPoint;
            
            AsyncBeginSend(bufferToSend);
             */
            AsyncBeginSend(buffer);
        }

        protected override void PacketSent(Clutch.Net.UDP.UDPPacketBuffer buffer, int bytesSent)
        {
            MessageWasReceivedEvent("++ Package was sent",new EventArgs());
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

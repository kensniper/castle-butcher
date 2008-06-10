using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Client
{
    // this class encapsulates a single packet that
    // is either sent or received by a UDP socket
    public class UDPPacketBuffer
    {
        // size of the buffer
        public const int BUFFER_SIZE = 1400; //was 4096

        // the buffer itself
        public byte[] Data;

        // length of data to transmit
        public int DataLength;

        // the (IP)Endpoint of the remote host
        public EndPoint RemoteEndPoint;

        public UDPPacketBuffer()
        {
            this.Data = new byte[BUFFER_SIZE];

            // this will be filled in by the call to udpSocket.BeginReceiveFrom
            RemoteEndPoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
        }
    }
}
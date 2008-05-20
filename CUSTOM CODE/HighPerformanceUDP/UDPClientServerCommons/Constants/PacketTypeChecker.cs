using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public static class PacketTypeChecker
    {
        public static PacketType Check(byte[] binaryPacket)
        {
            ushort tmp = BitConverter.ToUInt16(binaryPacket, 0);
            return (PacketType)tmp;
        }
    }
}

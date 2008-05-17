using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public static class PacketTypeChecker
    {
        public static PacketType Check(byte[] binaryPacket)
        {
            return (PacketType)Enum.Parse(typeof(PacketType), BitConverter.ToUInt16(binaryPacket, 0).ToString());
        }
    }
}

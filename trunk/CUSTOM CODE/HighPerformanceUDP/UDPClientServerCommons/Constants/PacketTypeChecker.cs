using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Constants;

namespace UDPClientServerCommons
{
    public static class PacketTypeChecker
    {
        public static PacketTypeEnumeration Check(byte[] binaryPacket)
        {
            ushort tmp = BitConverter.ToUInt16(binaryPacket, 0);
            try
            {
                PacketTypeEnumeration PacketType = (PacketTypeEnumeration)tmp;
                return PacketType;
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Error when reading packet type", ex);
            }
            return PacketTypeEnumeration.Unknown;
        }

        public static Interfaces.IPacket GetPacket(byte[] binaryPacket)
        {
            switch (Check(binaryPacket))
            {
                case PacketTypeEnumeration.GameInfoPacket:
                    return new Packets.GameInfoPacket(binaryPacket);

                case PacketTypeEnumeration.JoinPacket:
                    return new Packets.JoinPacket(binaryPacket);

                case PacketTypeEnumeration.QuitPacket:
                    return new Packets.LeaveGamePacket(binaryPacket);

                case PacketTypeEnumeration.StandardClientPacket:
                    return new Packets.ClientPacket(binaryPacket);
                    
                case PacketTypeEnumeration.StandardServerPacket:
                    return new Packets.ServerPacket(binaryPacket);
                    
                case PacketTypeEnumeration.Unknown:
                    return null;

                default:
                    return null;
            }
        }
    }
}

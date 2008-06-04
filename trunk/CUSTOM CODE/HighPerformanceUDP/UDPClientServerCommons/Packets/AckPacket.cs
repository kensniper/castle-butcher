using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class AckPacket:IPacket,ICloneable
    {
        private const ushort _MTU_PacketSize = 1400;
        public const int _AckPacketMinimalSize = 6;

        private PacketType TypeOfPacketField = PacketType.JoinPacket;

        public PacketType TypeOfPacket
        {
            get { return TypeOfPacketField; }
        }

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort packetIdAckField;

        public ushort PacketIdAck
        {
            get { return packetIdAckField; }
            set { packetIdAckField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdAckField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;        
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(_AckPacketMinimalSize);
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdAckField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;        
        }

        public AckPacket()
        { }

        public AckPacket(byte[] binaryAckPacket)
        {
            this.TypeOfPacketField = (PacketType) BitConverter.ToUInt16(binaryAckPacket, 0);
            this.playerIdField = BitConverter.ToUInt16(binaryAckPacket, 2);
            this.packetIdAckField = BitConverter.ToUInt16(binaryAckPacket, 4);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            AckPacket copy = new AckPacket();
            copy.playerIdField = this.playerIdField;
            copy.packetIdAckField = this.packetIdAckField;
            return copy;
        }

        #endregion
    }
}

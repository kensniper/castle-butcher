using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class AckPacket:IPacket
    {
        private const ushort _MTU_PacketSize = 1400;

        private PacketType TypeOfPacketField = PacketType.Join;

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
            MemoryStream ms = new MemoryStream(6);
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
            this.TypeOfPacketField = (PacketType)Enum.Parse(typeof(PacketType), BitConverter.ToUInt16(binaryAckPacket, 0).ToString());
            this.playerIdField = BitConverter.ToUInt16(binaryAckPacket, 2);
            this.packetIdAckField = BitConverter.ToUInt16(binaryAckPacket, 4);
        }

        #endregion
    }
}

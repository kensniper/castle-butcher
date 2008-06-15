using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class PingResponsePacket : Interfaces.IPacket, Interfaces.ISerializablePacket
    {
        #region IPacket Members

        public UDPClientServerCommons.Constants.PacketTypeEnumeration PacketType
        {
            get { return UDPClientServerCommons.Constants.PacketTypeEnumeration.PingResponse; }
        }

        private Usefull.PacketIdCounter packetIdCounterField;

        public UDPClientServerCommons.Usefull.PacketIdCounter PacketId
        {
            get
            {
                return packetIdCounterField;
            }
            set
            {
                packetIdCounterField = value;
            }
        }

        private DateTime timestampField;

        public DateTime TimeStamp
        {
            get
            {
                return timestampField;
            }
            set
            {
                timestampField = value;
            }
        }

        public object Clone()
        {
            PingResponsePacket cpy = new PingResponsePacket();
            cpy.packetIdCounterField = new UDPClientServerCommons.Usefull.PacketIdCounter(this.packetIdCounterField.Value);
            cpy.timestampField = this.timestampField;
            cpy.playerIdField = this.playerIdField;

            return cpy;
        }

        #endregion

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }
        
        #region ISerializablePacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(Constants.Constant._MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(PacketId.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(ByteCount);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(PacketId.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public int ByteCount
        {
            get { return 14; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(PacketType);
            sb.Append("\n PlayerId ");
            sb.Append(playerIdField);
            sb.Append("\n timestamp ");
            sb.Append(timestampField);

            return sb.ToString();
        }

        public PingResponsePacket(byte[] binaryPingResponsePacket)
        {
            this.packetIdCounterField = new UDPClientServerCommons.Usefull.PacketIdCounter(BitConverter.ToUInt16(binaryPingResponsePacket, 2));
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryPingResponsePacket, 4));
            this.playerIdField = BitConverter.ToUInt16(binaryPingResponsePacket, 12);
        }

        public PingResponsePacket()
        { 
        }
    }
}

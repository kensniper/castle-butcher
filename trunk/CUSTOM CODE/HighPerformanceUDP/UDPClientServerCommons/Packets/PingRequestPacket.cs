using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class PingRequestPacket:Interfaces.IPacket,Interfaces.ISerializablePacket
    {
        #region IPacket Members

        public UDPClientServerCommons.Constants.PacketTypeEnumeration PacketType
        {
            get { return UDPClientServerCommons.Constants.PacketTypeEnumeration.PingRequest; }
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
            PingRequestPacket cpy = new PingRequestPacket();
            cpy.packetIdCounterField = new UDPClientServerCommons.Usefull.PacketIdCounter(this.packetIdCounterField.Value);
            cpy.timestampField = this.timestampField;

            return cpy;
        }

        #endregion

        #region ISerializablePacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(Constants.Constant._MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(PacketId.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

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

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public int ByteCount
        {
            get { return 12; }
        }

        #endregion
    }
}

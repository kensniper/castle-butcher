using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class MessagePacket:Interfaces.ISerializablePacket,Interfaces.IPacket,ICloneable
    {
        #region fields

        private Usefull.PacketIdCounter packetIdCounterField;

        private DateTime timestampField;

        private string messageField;

        public string Message
        {
            get { return messageField; }
            set { messageField = value; }
        }

        #endregion

        #region IPacket Members

        public UDPClientServerCommons.Constants.PacketTypeEnumeration PacketType
        {
            get { return UDPClientServerCommons.Constants.PacketTypeEnumeration.MessagePacket; }
        }

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

        #endregion

        #region ISerializablePacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(Constants.Constant._MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdCounterField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(messageField)), 0, 2);
            ms.Write(Encoding.UTF8.GetBytes(messageField), 0, Encoding.UTF8.GetByteCount(messageField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(ByteCount);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdCounterField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(messageField)), 0, 2);
            ms.Write(Encoding.UTF8.GetBytes(messageField), 0, Encoding.UTF8.GetByteCount(messageField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public int ByteCount
        {
            get
            {
                int pos = 0;
                pos += 2;
                pos += 2;
                pos += 8;
                pos += 2;
                pos += Encoding.UTF8.GetByteCount(messageField);

                return pos;
            }
        }

        #endregion

        #region Constructor

        public MessagePacket(byte[] binaryMessagePacket)
        {
            messageField = string.Empty;

            int pos = 2;
            this.packetIdCounterField = new UDPClientServerCommons.Usefull.PacketIdCounter(BitConverter.ToUInt16(binaryMessagePacket, pos));
            pos += 2;
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryMessagePacket, pos));
            pos += 8;
            int stringLength = (int)BitConverter.ToUInt16(binaryMessagePacket, pos);
            pos += 2;
            this.messageField = Encoding.UTF8.GetString(binaryMessagePacket, pos, stringLength);
        }

        public MessagePacket()
        {
            messageField = string.Empty;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nPacketId = ");
            sb.Append(packetIdCounterField.Value);
            sb.Append("\nTimestamp = ");
            sb.Append(timestampField);
            sb.Append("\nMessage = ");
            sb.Append(messageField);

            return sb.ToString();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            MessagePacket cpy = new MessagePacket();
            cpy.messageField = this.messageField;
            cpy.packetIdCounterField = new UDPClientServerCommons.Usefull.PacketIdCounter(this.packetIdCounterField.Value);
            cpy.timestampField = this.timestampField;

            return cpy;
        }

        #endregion
    }
}

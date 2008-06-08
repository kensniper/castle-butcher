using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Packets
{
    public class JoinPacket : Interfaces.ISerializablePacket, ICloneable, Interfaces.IPacket
    {
        #region fields

        private DateTime timestampField;

        private PacketIdCounter packetIdField;

        public const ushort _MTU_PacketSize = 1400;

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort gameIdField;

        public ushort GameId
        {
            get { return gameIdField; }
            set { gameIdField = value; }
        }

        private string playerNameField;

        public string PlayerName
        {
            get { return playerNameField; }
            set { playerNameField = value; }
        }

        #endregion

        #region ISerializablePacket Members

        public int ByteCount
        {
            get
            {
                int pos = 0;
                pos += 2;
                pos += 2;
                pos += 8;
                pos += 2;
                pos += 2;
                pos += 2;
                pos += Encoding.UTF8.GetByteCount(playerNameField);
                return pos;
            }
        }

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
         //   int pos = 0;
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
           // pos += 2;
            ms.Write(BitConverter.GetBytes(packetIdField.Value),0,2);

            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(gameIdField), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(Encoding.UTF8.GetByteCount(playerNameField))), 0, 2);
            //pos += 2;
            ms.Write(Encoding.UTF8.GetBytes(playerNameField), 0, Encoding.UTF8.GetByteCount(playerNameField));
            //pos += Encoding.UTF8.GetByteCount(PlayerNameField);
        
            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.ByteCount);
            //int pos = 0;
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(gameIdField), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(Encoding.UTF8.GetByteCount(playerNameField))), 0, 2);
            //pos += 2;
            ms.Write(Encoding.UTF8.GetBytes(playerNameField), 0, Encoding.UTF8.GetByteCount(playerNameField));
            //pos += Encoding.UTF8.GetByteCount(PlayerNameField);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        #region Constructors

        public JoinPacket()
        {
            packetIdField = new PacketIdCounter();
        }

        public JoinPacket(byte[] binaryJoinPacket)
        {

            this.packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryJoinPacket, 2));
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryJoinPacket, 4));

            this.playerIdField = BitConverter.ToUInt16(binaryJoinPacket, 12);
            this.gameIdField = BitConverter.ToUInt16(binaryJoinPacket, 14);

            ushort stringLength = BitConverter.ToUInt16(binaryJoinPacket, 16);
            this.playerNameField = Encoding.ASCII.GetString(binaryJoinPacket, 18, stringLength);
            
        }

        public JoinPacket(byte[] binaryJoinPacket,int index)
        {
            this.packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryJoinPacket, index+2));
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryJoinPacket, index + 4));

            this.playerIdField = BitConverter.ToUInt16(binaryJoinPacket, 12+index);
            this.gameIdField = BitConverter.ToUInt16(binaryJoinPacket, 14 + index);

            ushort stringLength = BitConverter.ToUInt16(binaryJoinPacket, 16 + index);
            this.playerNameField = Encoding.ASCII.GetString(binaryJoinPacket, 18 + index, stringLength);
           
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nPacketId = ");
            sb.Append(packetIdField);
            sb.Append("\nTimeStamp = ");
            sb.Append(timestampField);
            sb.Append("\nPlayerId = ");
            sb.Append(playerIdField);
            sb.Append("\nGameId = ");
            sb.Append(gameIdField);
            sb.Append("\nPlayerName = ");
            sb.Append(playerNameField);

            return sb.ToString();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            JoinPacket copy = new JoinPacket();
            copy.gameIdField = this.gameIdField;
            copy.playerIdField = this.playerIdField;
            copy.playerNameField = this.playerNameField;
            copy.timestampField = this.timestampField;
            copy.packetIdField = new PacketIdCounter(this.packetIdField.Value);
            return copy;
        }

        #endregion

        #region IPacket Members

        public PacketTypeEnumeration PacketType
        {
            get { return PacketTypeEnumeration.JoinPacket; }
        }

        #endregion

        #region IPacket Members


        public UDPClientServerCommons.Usefull.PacketIdCounter PacketId
        {
            get
            {
                return packetIdField;
            }
            set
            {
                packetIdField = value;
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
    }
}

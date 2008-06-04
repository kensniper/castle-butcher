using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class JoinPacket : IPacket,ICloneable
    {
        public const ushort _MTU_PacketSize = 1400;

        private PacketType TypeOfPacketField = PacketType.JoinPacket;

        public PacketType TypeOfPacket
        {
            get { return TypeOfPacketField; }
        }

        private ushort packetIdField;

        public ushort PacketId
        {
            get { return packetIdField; }
            set { packetIdField = value; }
        }

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort GameIdField;

        public ushort GameId
        {
            get { return GameIdField; }
            set { GameIdField = value; }
        }

        private string PlayerNameField;

        public string PlayerName
        {
            get { return PlayerNameField; }
            set { PlayerNameField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            ushort tmp = (ushort)TypeOfPacketField;

            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(GameIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetByteCount(PlayerNameField))), 0, 2);
            ms.Write(Encoding.ASCII.GetBytes(PlayerNameField), 0, Encoding.ASCII.GetByteCount(PlayerNameField));
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            ushort tmp = (ushort)TypeOfPacketField;

            MemoryStream ms = new MemoryStream(8 + Encoding.ASCII.GetByteCount(PlayerNameField));
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(GameIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetByteCount(PlayerNameField))), 0, 2);
            ms.Write(Encoding.ASCII.GetBytes(PlayerNameField), 0, Encoding.ASCII.GetByteCount(PlayerNameField));
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        public JoinPacket()
        { }

        public JoinPacket(byte[] binaryJoinPacket)
        {
            this.TypeOfPacketField = (PacketType)BitConverter.ToUInt16(binaryJoinPacket, 0);
            this.playerIdField = BitConverter.ToUInt16(binaryJoinPacket, 2);
            this.GameIdField = BitConverter.ToUInt16(binaryJoinPacket, 4);

            ushort stringLength = BitConverter.ToUInt16(binaryJoinPacket, 6);
            this.PlayerNameField = Encoding.ASCII.GetString(binaryJoinPacket, 8, stringLength);
            this.packetIdField = BitConverter.ToUInt16(binaryJoinPacket, 8 + stringLength);
        }

        #region ICloneable Members

        public object Clone()
        {
            JoinPacket copy = new JoinPacket();
            copy.GameIdField = this.GameIdField;
            copy.playerIdField = this.playerIdField;
            copy.PlayerNameField = this.PlayerNameField;
            copy.TypeOfPacketField = this.TypeOfPacketField;

            return copy;
        }

        #endregion
    }
}

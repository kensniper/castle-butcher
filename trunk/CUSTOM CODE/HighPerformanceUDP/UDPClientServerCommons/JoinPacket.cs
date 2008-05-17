using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class JoinPacket : IPacket
    {
        public const ushort _MTU_PacketSize = 1400;

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
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(GameIdField), 0, 2);
            //ms.Write(BitConverter.GetBytes(PlayerNameField, 0, Convert.ToUInt16(Encoding.ASCII.GetByteCount(PlayerNameField)));
            ms.Write(Encoding.ASCII.GetBytes(PlayerNameField), 0, Encoding.ASCII.GetByteCount(PlayerNameField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(6 + Encoding.ASCII.GetByteCount(PlayerNameField));
            ms.Write(BitConverter.GetBytes((ushort)TypeOfPacketField), 0, 2);
            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(GameIdField), 0, 2);
            //ms.Write(BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetByteCount(PlayerNameField), 0, 2));
            ms.Write(Encoding.ASCII.GetBytes(PlayerNameField), 0, Encoding.ASCII.GetByteCount(PlayerNameField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        public JoinPacket()
        { }

        public JoinPacket(byte[] binaryJoinPacket)
        {
            this.TypeOfPacketField = (PacketType)Enum.Parse(typeof(PacketType), BitConverter.ToUInt16(binaryJoinPacket, 0).ToString());
            this.playerIdField = BitConverter.ToUInt16(binaryJoinPacket, 2);
            this.GameIdField = BitConverter.ToUInt16(binaryJoinPacket, 4);

            ushort stringLength = BitConverter.ToUInt16(binaryJoinPacket, 6);
            this.PlayerNameField = Encoding.ASCII.GetString(binaryJoinPacket, 8, stringLength);
        }
    }
}

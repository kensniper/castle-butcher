using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class ServerPacket : IPacket
    {
        private const ushort _MTU_PacketSize = 1400;

        private PacketType packetTypeField;

        public PacketType TypeOfPacket
        {
            get { return packetTypeField; }
            set { packetTypeField = value; }
        }

        /// <summary>
        /// litle redundant info since playerInfoListField.Count has the same value
        /// </summary>
        private ushort numberOfPlayersField;
        
        public ushort NumberOfPlayers
        {
            get { return numberOfPlayersField; }
            set { numberOfPlayersField = value; }
        }

        private DateTime timestampField;

        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        private List<PlayerInfo> playerInfoListField;

        public List<PlayerInfo> PlayerInfoList
        {
            get { return playerInfoListField; }
            set { playerInfoListField = value; }
        }        

        private ushort packetIdField;

        public ushort PacketId
        {
            get { return packetIdField; }
            set { packetIdField = value; }
        }


        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);

            ms.Write(BitConverter.GetBytes(packetIdField), 0, 2);
            ms.Write(BitConverter.GetBytes((ushort)packetTypeField), 0, 2);
            ms.Write(BitConverter.GetBytes(numberOfPlayersField), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                byte[] info = playerInfoListField[i].ToByte();
                ms.Write(info, 0, info.Length);
            }

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            int size = 14;
            for(int i=0;i<playerInfoListField.Count;i++)
                size += 51 + playerInfoListField[i].AckIds.Count * 2;

            MemoryStream ms = new MemoryStream(size);

            ms.Write(BitConverter.GetBytes(packetIdField), 0, 2);
            ms.Write(BitConverter.GetBytes((ushort)packetTypeField), 0, 2);
            ms.Write(BitConverter.GetBytes(numberOfPlayersField), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                byte[] info = playerInfoListField[i].ToMinimalByte();
                ms.Write(info, 0, info.Length);
            }

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;

        }

        #endregion

        public ServerPacket()
        {
            playerInfoListField = new List<PlayerInfo>();
        }

        public ServerPacket(byte[] binaryServerPacket)
        {
            this.packetIdField = BitConverter.ToUInt16(binaryServerPacket, 0);
            this.packetTypeField = (PacketType)Enum.Parse(typeof(PacketType), BitConverter.ToUInt16(binaryServerPacket, 2).ToString());
            this.numberOfPlayersField = BitConverter.ToUInt16(binaryServerPacket, 4);
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryServerPacket, 6));
            this.playerInfoListField = new List<PlayerInfo>();
            int positionIn = 14;
            for (int i = 0; i < numberOfPlayersField; i++)
            {
                PlayerInfo info = new PlayerInfo(binaryServerPacket, positionIn);
                positionIn += info.PlayerInfoBinaryLength;
                playerInfoListField.Add(info);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n PacketId = \t");
            sb.Append(packetIdField);
            sb.Append("\n PacketType = \t");
            sb.Append(packetTypeField);
            sb.Append("\n numberOfPlayers = \t");
            sb.Append(numberOfPlayersField);
            sb.Append("\n timestamp = \t");
            sb.Append(timestampField);

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                sb.Append(playerInfoListField[i].ToString());
            }

            return sb.ToString();
        }
    }
}

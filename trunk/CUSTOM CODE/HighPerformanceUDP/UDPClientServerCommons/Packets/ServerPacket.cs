using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Packets
{
    public class ServerPacket : Interfaces.ISerializablePacket,Interfaces.IPacket
    {
        #region fields

        private const ushort _MTU_PacketSize = 1400;

        private DateTime timestampField;

        private PacketIdCounter packetIdField;
        
        public ushort NumberOfPlayers
        {
            get { return (ushort)playerInfoListField.Count; }
        }

        private List<PlayerInfo> playerInfoListField;

        public List<PlayerInfo> PlayerInfoList
        {
            get { return playerInfoListField; }
            set { playerInfoListField = value; }
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
                pos += 2;
                pos += 8;

                for (int i = 0; i < playerInfoListField.Count; i++)
                {
                    pos += playerInfoListField[i].ByteCount;
                }
                return pos;
            }
        }

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);

            //int pos = 0;
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(NumberOfPlayers), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            //pos += 8;

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                byte[] info = playerInfoListField[i].ToByte();
                ms.Write(info, 0, playerInfoListField[i].ByteCount);
                //pos += playerInfoListField[i].ByteCount;
            }

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
            //pos += 2;
            ms.Write(BitConverter.GetBytes(NumberOfPlayers), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            //pos += 8;

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                byte[] info = playerInfoListField[i].ToMinimalByte();
                ms.Write(info, 0, playerInfoListField[i].ByteCount);
                //pos += playerInfoListField[i].ByteCount;
            }

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;

        }

        #endregion

        #region Constructor

        public ServerPacket()
        {
            playerInfoListField = new List<PlayerInfo>();
            packetIdField = new PacketIdCounter();
        }

        public ServerPacket(byte[] binaryServerPacket)
        {
            this.packetIdField = new PacketIdCounter( BitConverter.ToUInt16(binaryServerPacket, 2));            
            int playerNumber = BitConverter.ToUInt16(binaryServerPacket, 4);
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryServerPacket, 6));
            this.playerInfoListField = new List<PlayerInfo>();
            int positionIn = 14;
            for (int i = 0; i < playerNumber; i++)
            {
                PlayerInfo info = new PlayerInfo(binaryServerPacket, positionIn);
                positionIn += info.ByteCount;
                playerInfoListField.Add(info);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n PacketId = \t");
            sb.Append(packetIdField.Value);
            sb.Append("\n PacketType = \t");
            sb.Append(PacketType);
            sb.Append("\n numberOfPlayers = \t");
            sb.Append(NumberOfPlayers);
            sb.Append("\n timestamp = \t");
            sb.Append(timestampField);

            for (int i = 0; i < playerInfoListField.Count; i++)
            {
                sb.Append(playerInfoListField[i].ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region IPacket Members

        public PacketTypeEnumeration PacketType
        {
            get { return PacketTypeEnumeration.StandardServerPacket; }
        }

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

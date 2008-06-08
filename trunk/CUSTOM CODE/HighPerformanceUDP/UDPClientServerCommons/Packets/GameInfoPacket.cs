using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Packets
{
    public class GameInfoPacket : ICloneable, Interfaces.ISerializablePacket, Interfaces.IPacket
    {
        #region fields

        public const int _MTU_PacketSize = 1400;

        private DateTime timestampField;

        private PacketIdCounter packetIdField;

        private List<PlayerStatus> playerStatusListField = null;

        public List<PlayerStatus> PlayerStatusList
        {
            get { return playerStatusListField; }
            set { playerStatusListField = value; }
        }

        private List<TeamScoreStruct> teamScoreListField = null;

        public List<TeamScoreStruct> TeamScoreList
        {
            get { return teamScoreListField; }
            set { teamScoreListField = value; }
        }

        private Constants.GameTypeEnumeration gameTypeField = UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit;

        public Constants.GameTypeEnumeration GameType
        {
            get { return gameTypeField; }
            set { gameTypeField = value; }
        }

        private ushort limitField = 0;

        public ushort Limit
        {
            get { return limitField; }
            set { limitField = value; }
        }

        #endregion

        #region ISerializablePacket Members

        public int ByteCount
        {
            get
            {
                int size = 2;
                size += 2;
                size += 8;

                if (playerStatusListField != null)
                {
                    size += 2;
                    for (int i = 0; i < playerStatusListField.Count; i++)
                    {
                        size += 2;
                        size += playerStatusListField[i].ByteCount;
                    }
                }
                else
                {
                    size += 2;
                }
                if (teamScoreListField != null)
                {
                    size += 2;
                    for (int i = 0; i < teamScoreListField.Count; i++)
                    {

                        size += 2;
                        size += 2;
                    }
                }
                else
                {
                    size += 2;
                }

                size += 2;
                size += 2;

                return size;
            }
        }

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);

           // int pos = 0;
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()),0,8);

            //pos += 2;

            if (playerStatusListField != null)
            {
                ms.Write(BitConverter.GetBytes(playerStatusListField.Count), 0, 2);
              //  pos += 2;

                for (int i = 0; i < playerStatusListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes((ushort)(playerStatusListField[i].ByteCount)), 0, 2);
                //    pos += 2;
                    ms.Write(playerStatusListField[i].ToByte(), 0, playerStatusListField[i].ByteCount);
                  //  pos += PlayerStatusListField[i].ByteCount;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), 0, 2);
               // pos += 2;
            }

            if (teamScoreListField != null)
            {
                ms.Write(BitConverter.GetBytes((ushort)teamScoreListField.Count), 0, 2);
                //pos += 2;

                for (int i = 0; i < teamScoreListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes(teamScoreListField[i].TeamId), 0, 2);
                  //  pos += 2;
                    ms.Write(BitConverter.GetBytes(teamScoreListField[i].TeamScore), 0, 2);
                    //pos += 2;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), 0, 2);
               // pos += 2;
            }

            ms.Write(BitConverter.GetBytes((ushort)gameTypeField), 0, 2);
           // pos += 2;
            ms.Write(BitConverter.GetBytes(limitField), 0, 2);
          //  pos += 2;

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.ByteCount);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
          //  int pos = 2;

            if (playerStatusListField != null)
            {
                ms.Write(BitConverter.GetBytes(playerStatusListField.Count), 0, 2);
            //    pos += 2;

                for (int i = 0; i < playerStatusListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes((ushort)(playerStatusListField[i].ByteCount)), 0, 2);
              //      pos += 2;
                    ms.Write(playerStatusListField[i].ToByte(), 0, playerStatusListField[i].ByteCount);
                //    pos += PlayerStatusListField[i].ByteCount;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), 0, 2);
                //pos += 2;
            }

            if (teamScoreListField != null)
            {
                ms.Write(BitConverter.GetBytes((ushort)teamScoreListField.Count), 0, 2);
                //pos += 2;

                for (int i = 0; i < teamScoreListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes(teamScoreListField[i].TeamId), 0, 2);
                  //  pos += 2;
                    ms.Write(BitConverter.GetBytes(teamScoreListField[i].TeamScore), 0, 2);
                    //pos += 2;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), 0, 2);
                //pos += 2;
            }

            ms.Write(BitConverter.GetBytes((ushort)gameTypeField), 0, 2);
            //pos += 2;
            ms.Write(BitConverter.GetBytes(limitField), 0, 2);
            //pos += 2;

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Constructor

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nGame Type =");
            sb.Append(gameTypeField);
            sb.Append("\nLimit = ");
            sb.Append(limitField);
            sb.Append("\nPlayer status list");
            for (int i = 0; i < playerStatusListField.Count; i++)
                sb.Append(playerStatusListField[i]);
            sb.Append("\nTeam Score list");
            for (int i = 0; i < teamScoreListField.Count; i++)
                sb.Append(teamScoreListField[i]);

            return sb.ToString();
        }

        public GameInfoPacket()
        {
            this.playerStatusListField = new List<PlayerStatus>();
            this.teamScoreListField = new List<TeamScoreStruct>();
        }

        public GameInfoPacket(byte[] binaryGameInfoPacket)
        {
            int pos = 2;
            packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryGameInfoPacket,pos));
            pos += 2;
            timestampField=DateTime.FromBinary(BitConverter.ToInt64(binaryGameInfoPacket,pos));
            pos += 8;

            playerStatusListField = new List<PlayerStatus>();

            int playerCount = (int)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;



            for (int i = 0; i < playerCount; i++)
            {
                int psLength = (int)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                PlayerStatus ps = new PlayerStatus(binaryGameInfoPacket, pos);
                pos += psLength;
                playerStatusListField.Add(ps);
            }

            int teamScoreCount = (int)BitConverter.ToUInt16(binaryGameInfoPacket, pos);

            pos += 2;
            teamScoreListField = new List<TeamScoreStruct>();

            for (int i = 0; i < teamScoreCount; i++)
            {
                TeamScoreStruct ts = new TeamScoreStruct();
                ts.TeamId = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                ts.TeamScore = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                teamScoreListField.Add(ts);
            }


            this.gameTypeField = (Constants.GameTypeEnumeration)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;
            this.limitField = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;
        }

        #endregion

        #region IPacket Members

        public PacketTypeEnumeration PacketType
        {
            get { return PacketTypeEnumeration.GameInfoPacket; }
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

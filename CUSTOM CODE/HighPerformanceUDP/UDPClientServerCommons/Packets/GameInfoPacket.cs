using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Usefull;
using System.Net;

namespace UDPClientServerCommons.Packets
{
    public class GameInfoPacket : ICloneable, Interfaces.ISerializablePacket, Interfaces.IPacket
    {
        #region fields

        private DateTime timestampField;

        private PacketIdCounter packetIdField;

        private List<PlayerStatus> playerStatusListField = null;

        /// <summary>
        /// list of players and their current states
        /// </summary>
        public List<PlayerStatus> PlayerStatusList
        {
            get { return playerStatusListField; }
            set { playerStatusListField = value; }
        }

        private List<TeamData> teamScoreListField = null;

        /// <summary>
        /// list of teams and their scores
        /// </summary>
        public List<TeamData> TeamScoreList
        {
            get { return teamScoreListField; }
            set { teamScoreListField = value; }
        }

        private Constants.GameTypeEnumeration gameTypeField = UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit;

        /// <summary>
        /// type of the game
        /// </summary>
        public Constants.GameTypeEnumeration GameType
        {
            get { return gameTypeField; }
            set { gameTypeField = value; }
        }

        private ushort limitField = 0;

        /// <summary>
        /// game limit (frags,time...)
        /// </summary>
        public ushort Limit
        {
            get { return limitField; }
            set { limitField = value; }
        }

        private ushort gameIdField = 0;

        /// <summary>
        /// game id
        /// </summary>
        public ushort GameId
        {
            get { return gameIdField; }
            set { gameIdField = value; }
        }

        private IPEndPoint serverAddressField;

        /// <summary>
        /// adress of the server the game is taking place
        /// </summary>
        public IPEndPoint ServerAddress
        {
            get { return serverAddressField; }
            set { serverAddressField = value; }
        }

        private ushort roundNumberField = 0;

        /// <summary>
        /// number of the current round, if 0 than game is ending
        /// </summary>
        public ushort RoundNumber
        {
            get { return roundNumberField; }
            set { roundNumberField = value; }
        }

        #endregion

        #region ISerializablePacket Members

        /// <summary>
        /// number of bytes in binary message
        /// </summary>
        public int ByteCount
        {
            get
            {
                int size = 2;
                size += 2;                
                size += 8;
                size += 2;
                size += 2;

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
                        size += 2;
                        size += Encoding.UTF8.GetByteCount(teamScoreListField[i].TeamName);
                    }
                }
                else
                {
                    size += 2;
                }

                size += 2;
                size += 2;

                size += 2;
                size += Encoding.UTF8.GetByteCount(serverAddressField.Address.ToString());
                size += 4;

                return size;
            }
        }

        /// <summary>
        /// converts packet to binary data of size _MTU_PacketSize
        /// </summary>
        /// <returns>packet in binary version</returns>
        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(Constant._MTU_PacketSize);

           // int pos = 0;
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()),0,8);
            ms.Write(BitConverter.GetBytes(gameIdField), 0, 2);

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
                    ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(teamScoreListField[i].TeamName)), 0, 2);

                    ms.Write(Encoding.UTF8.GetBytes(teamScoreListField[i].TeamName), 0, Encoding.UTF8.GetByteCount(teamScoreListField[i].TeamName));
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

            int addressLength = Encoding.UTF8.GetByteCount(serverAddressField.Address.ToString());
            ms.Write(BitConverter.GetBytes((ushort)addressLength), 0, 2);
            ms.Write(Encoding.UTF8.GetBytes(serverAddressField.Address.ToString()), 0, addressLength);
            ms.Write(BitConverter.GetBytes(serverAddressField.Port), 0, 4);
            ms.Write(BitConverter.GetBytes(roundNumberField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        /// <summary>
        /// converts packet to binary data of minimal size
        /// </summary>
        /// <returns>packet in binary version</returns>
        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.ByteCount);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdField.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);
            ms.Write(BitConverter.GetBytes(gameIdField), 0, 2);
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

                    ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(teamScoreListField[i].TeamName)), 0, 2);

                    ms.Write(Encoding.UTF8.GetBytes(teamScoreListField[i].TeamName), 0, Encoding.UTF8.GetByteCount(teamScoreListField[i].TeamName));
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

            int addressLength = Encoding.UTF8.GetByteCount(serverAddressField.Address.ToString());
            ms.Write(BitConverter.GetBytes((ushort)addressLength), 0, 2);
            ms.Write(Encoding.UTF8.GetBytes(serverAddressField.Address.ToString()), 0, addressLength);
            ms.Write(BitConverter.GetBytes(serverAddressField.Port), 0, 4);
            ms.Write(BitConverter.GetBytes(roundNumberField), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            GameInfoPacket cpy = new GameInfoPacket();
            cpy.gameIdField = this.gameIdField;
            cpy.gameTypeField = this.gameTypeField;
            cpy.limitField = this.limitField;
            cpy.packetIdField = this.packetIdField;
            cpy.playerStatusListField = new List<PlayerStatus>(this.playerStatusListField);
            cpy.serverAddressField = this.serverAddressField;
            cpy.teamScoreListField = new List<TeamData>(this.teamScoreListField);
            cpy.timestampField = this.timestampField;
            cpy.roundNumberField = this.roundNumberField;

            return cpy;
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
            sb.Append("\nGameId = ");
            sb.Append(gameIdField);
            sb.Append("\nServerAddress = ");
            sb.Append(serverAddressField);
            sb.Append("\nPlayer status list");
            for (int i = 0; i < playerStatusListField.Count; i++)
                sb.Append(playerStatusListField[i]);
            sb.Append("\nTeam Score list");
            for (int i = 0; i < teamScoreListField.Count; i++)
                sb.Append(teamScoreListField[i]);
            sb.Append("\nRoundId = ");
            sb.Append(roundNumberField);

            return sb.ToString();
        }

        public GameInfoPacket()
        {
            this.packetIdField = new PacketIdCounter();
            this.playerStatusListField = new List<PlayerStatus>();
            this.teamScoreListField = new List<TeamData>();
        }

        public GameInfoPacket(byte[] binaryGameInfoPacket)
        {
            int pos = 2;
            packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryGameInfoPacket,pos));
            pos += 2;
            timestampField=DateTime.FromBinary(BitConverter.ToInt64(binaryGameInfoPacket,pos));
            pos += 8;
            gameIdField = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;

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
            teamScoreListField = new List<TeamData>();

            for (int i = 0; i < teamScoreCount; i++)
            {
                TeamData ts = new TeamData();
                ts.TeamId = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                ts.TeamScore = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                int count = (int)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
                pos += 2;
                ts.TeamName = Encoding.UTF8.GetString(binaryGameInfoPacket, pos, count);
                pos += count;
                teamScoreListField.Add(ts);
            }


            this.gameTypeField = (Constants.GameTypeEnumeration)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;
            this.limitField = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;

            int addressLength = (int)BitConverter.ToUInt16(binaryGameInfoPacket, pos);
            pos += 2;
            string address = Encoding.UTF8.GetString(binaryGameInfoPacket, pos, addressLength);
            pos += addressLength;
            int port = BitConverter.ToInt32(binaryGameInfoPacket, pos);
            pos += 4;
            serverAddressField = new IPEndPoint(IPAddress.Parse(address), port);

            roundNumberField = BitConverter.ToUInt16(binaryGameInfoPacket, pos);
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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class PlayerStatus:Interfaces.ISerializablePacket
    {
        #region fields

        private ushort PlayerIdField;

        public ushort PlayerId
        {
            get { return PlayerIdField; }
            set { PlayerIdField = value; }
        }

        private ushort PlayerTeamField;

        public ushort PlayerTeam
        {
            get { return PlayerTeamField; }
            set { PlayerTeamField = value; }
        }

        private ushort PlayerScoreField;

        public ushort PlayerScore
        {
            get { return PlayerScoreField; }
            set { PlayerScoreField = value; }
        }

        private ushort PlayerPingField;

        public ushort PlayerPing
        {
            get { return PlayerPingField; }
            set { PlayerPingField = value; }
        }

        private string PlayerNameField="";

        public string PlayerName
        {
            get { return PlayerNameField; }
            set { PlayerNameField = value; }
        }

        private ushort PlayerHealthField;

        public ushort PlayerHealth
        {
            get { return PlayerHealthField; }
            set { PlayerHealthField = value; }
        }

        #endregion

        #region ISerializablePacket Members

        public int ByteCount
        {
            get
            {
                int count = 12;
                count += Encoding.UTF8.GetByteCount(PlayerNameField);
                return count;
            }
        }

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(ByteCount);

            ms.Write(BitConverter.GetBytes(PlayerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(PlayerTeamField), 0, 2);
            ms.Write(BitConverter.GetBytes(PlayerScoreField), 0, 2);
            ms.Write(BitConverter.GetBytes(PlayerPingField), 0, 2);
            ms.Write(BitConverter.GetBytes(PlayerHealthField), 0, 2);
            ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(PlayerNameField)), 0, 2);
            ms.Write(Encoding.UTF8.GetBytes(PlayerNameField), 0, Encoding.UTF8.GetByteCount(PlayerNameField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            return this.ToByte();
        }

        #endregion

        #region Constructor

        public PlayerStatus()
        {
        }

        public PlayerStatus(byte[] binaryPlayerStatus)
        {
            this.PlayerIdField = BitConverter.ToUInt16(binaryPlayerStatus, 0);
            this.PlayerTeamField = BitConverter.ToUInt16(binaryPlayerStatus, 2);
            this.PlayerScoreField = BitConverter.ToUInt16(binaryPlayerStatus, 4);
            this.PlayerPingField = BitConverter.ToUInt16(binaryPlayerStatus, 6);
            this.PlayerHealthField = BitConverter.ToUInt16(binaryPlayerStatus, 8);

            int length = (int)BitConverter.ToUInt16(binaryPlayerStatus, 10);

            this.PlayerNameField = Encoding.UTF8.GetString(binaryPlayerStatus, 12, length);
        }

        public PlayerStatus(byte[] binaryPlayerStatus,int index)
        {
            this.PlayerIdField = BitConverter.ToUInt16(binaryPlayerStatus, index);
            this.PlayerTeamField = BitConverter.ToUInt16(binaryPlayerStatus, 2+index);
            this.PlayerScoreField = BitConverter.ToUInt16(binaryPlayerStatus, 4 + index);
            this.PlayerPingField = BitConverter.ToUInt16(binaryPlayerStatus, 6 + index);
            this.PlayerHealthField = BitConverter.ToUInt16(binaryPlayerStatus, 8 + index);

            int length = (int)BitConverter.ToUInt16(binaryPlayerStatus, 10 + index);

            this.PlayerNameField = Encoding.UTF8.GetString(binaryPlayerStatus, 12 + index, length);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nPlayerId = ");
            sb.Append(PlayerIdField);
            sb.Append("\nPlayerTeam = ");
            sb.Append(PlayerTeamField);
            sb.Append("\nPlayerScore = ");
            sb.Append(PlayerScoreField);
            sb.Append("\nPlayerPing = ");
            sb.Append(PlayerPingField);
            sb.Append("\nPlayerHealth = ");
            sb.Append(PlayerHealthField);
            sb.Append("\nPlayerName = ");
            sb.Append(PlayerNameField);

            return sb.ToString();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class PlayerStatus:IPacket
    {
        public int PlayerStatusByteCount
        {
            get {
                int count = 12;
               count+= Encoding.UTF8.GetByteCount(PlayerNameField);
               return count;
            }
        }

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

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(PlayerStatusByteCount);

            ms.Write(BitConverter.GetBytes(PlayerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(PlayerTeamField), 2, 2);
            ms.Write(BitConverter.GetBytes(PlayerScoreField), 4, 2);
            ms.Write(BitConverter.GetBytes(PlayerPingField), 6, 2);
            ms.Write(BitConverter.GetBytes(PlayerHealthField), 8, 2);
            ms.Write(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(PlayerNameField)), 10, 2);
            ms.Write(Encoding.UTF8.GetBytes(PlayerNameField), 12, Encoding.UTF8.GetByteCount(PlayerNameField));

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            return this.ToByte();
        }

        #endregion

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
    }
}

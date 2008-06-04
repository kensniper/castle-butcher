using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    class LeaveGamePacket :IPacket,ICloneable
    {
        private ushort PlayerIdField;

        public ushort PlayerId
        {
            get { return PlayerIdField; }
            set { PlayerIdField = value; }
        }

        private ushort GameIdField;

        public ushort GameId
        {
            get { return GameIdField; }
            set { GameIdField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(4);

            ms.Write(BitConverter.GetBytes(PlayerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(GameIdField), 2, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            return this.ToByte();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            LeaveGamePacket cpy = new LeaveGamePacket();
            cpy.GameIdField = this.GameIdField;
            cpy.PlayerIdField = this.PlayerIdField;

            return cpy;
        }

        #endregion
    }
}

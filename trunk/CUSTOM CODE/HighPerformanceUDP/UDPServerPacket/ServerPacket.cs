using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UDPServerPacket
{
    public class ServerPacket : UDPClientServerCommons.PacketIdCommon,UDPClientServerCommons.IPacket
    {
        private UDPClientServerCommons.PacketType packetTypeField;

        public UDPClientServerCommons.PacketType TypeOfPacket
        {
            get { return packetTypeField; }
            set { packetTypeField = value; }
        }

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

        #region IPacket Members

        byte[] UDPClientServerCommons.IPacket.ToByte()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Packets
{
    public class GameInfoPacket:ICloneable,IPacket
    {
        private List<PlayerStatus> PlayerStatusListField = null;

        private List<TeamScoreStruct> TeamScoreList = null;

        private Constants.GameTypeEnumeration GameTypeField = UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit;

        private Nullable<ushort> LimitField = null;

        #region IPacket Members

        public byte[] ToByte()
        {
            throw new NotImplementedException();
        }

        public byte[] ToMinimalByte()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

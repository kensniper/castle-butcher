using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class NewRoundEvent :GameEventBase, Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public override UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.NewRound; }
        }

        #endregion

        private Packets.GameInfoPacket gameInfoPacketField;

        public Packets.GameInfoPacket GameInfo
        {
            get { return gameInfoPacketField; }
            set { gameInfoPacketField = value; }
        }

        public NewRoundEvent()
        { }

        public NewRoundEvent(Packets.GameInfoPacket gameInfo)
        {
            this.gameInfoPacketField = gameInfo;
        }
    }
}

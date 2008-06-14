using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class PlayerNickChangedEvent:Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerNickChanged; }
        }

        #endregion

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
        }

        private string newPlayerNickField;

        public string NewPlayerNick
        {
            get { return newPlayerNickField; }
            set { newPlayerNickField = value; }
        }
        private string oldPlayerNickField;

        public string OldPlayerNick
        {
            get { return oldPlayerNickField; }
            set { oldPlayerNickField = value; }
        }

        public PlayerNickChangedEvent(ushort playerId, string oldPlayerNick, string newPlayerNick)
        {
            this.playerIdField = playerId;
            this.oldPlayerNickField = oldPlayerNick;
            this.newPlayerNickField = newPlayerNick;
        }
    }
}

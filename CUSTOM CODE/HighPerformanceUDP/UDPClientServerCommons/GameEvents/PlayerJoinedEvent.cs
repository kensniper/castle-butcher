using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class PlayerJoinedEvent:Interfaces.IGameEvent
    {

        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerJoined; }
        }

        #endregion

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
        }
        private string playerNameField;

        public string PlayerName
        {
            get { return playerNameField; }
        }

        public PlayerJoinedEvent(ushort id, string name)
        {
            this.playerIdField = id;
            this.playerNameField = name;
        }
    }
}

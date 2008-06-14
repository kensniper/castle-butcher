using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class PlayerQuitEvent:Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerQuitted; }
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

        public PlayerQuitEvent(ushort id, string name)
        {
            this.playerIdField = id;
            this.playerNameField = name;
        }
    }
}

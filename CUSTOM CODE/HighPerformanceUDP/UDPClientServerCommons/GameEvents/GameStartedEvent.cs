using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class GameStartedEvent :GameEventBase,  Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public override UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.GameStared; }
        }

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class GameStartedEvent : Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.GameStared; }
        }

        #endregion
    }
}

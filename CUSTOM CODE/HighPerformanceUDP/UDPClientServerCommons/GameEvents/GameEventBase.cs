using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public abstract class GameEventBase
    {
        public abstract UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get;
        }

        public virtual new string ToString()
        {
            return GameEventType.ToString();
        }
    }
}

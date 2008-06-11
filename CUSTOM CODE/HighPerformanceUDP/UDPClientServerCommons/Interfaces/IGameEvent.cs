using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// game event not connected to gameplay
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// type of game event
        /// </summary>
        Constants.GameEventTypeEnumeration GameEventType
        {
            get;
        }
    }
}

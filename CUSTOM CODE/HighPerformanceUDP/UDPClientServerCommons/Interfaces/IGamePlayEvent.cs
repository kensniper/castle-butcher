using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// event which occures during gameplay
    /// </summary>
    public interface IGameplayEvent
    {
        /// <summary>
        /// type of gameplay event
        /// </summary>
        Constants.GamePlayEventTypeEnumeration GameplayEventType
        {
            get;
        }

        /// <summary>
        /// when event occured
        /// </summary>
        DateTime Timestamp
        {
            get;
        }

        /// <summary>
        /// id of player who made the event
        /// </summary>
        ushort PlayerId
        {
            get;
        }
    }
}

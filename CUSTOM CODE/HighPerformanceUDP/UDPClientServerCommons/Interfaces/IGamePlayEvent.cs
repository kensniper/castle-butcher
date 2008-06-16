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

        /// <summary>
        /// player position when event happend
        /// </summary>
        Microsoft.DirectX.Vector3 Position
        {
            get;
        }

        /// <summary>
        /// Point player is looking at when event happend
        /// </summary>
        Microsoft.DirectX.Vector3 LookingDirection
        {
            get;
        }

        /// <summary>
        /// Player Velocity when event happend
        /// </summary>
        Microsoft.DirectX.Vector3 Velocity
        {
            get;
        }
    }
}

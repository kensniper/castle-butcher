using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Interface describing data about Player
    /// </summary>
    public interface IPlayerDataRead
    {
        /// <summary>
        /// Player position
        /// </summary>
        Microsoft.DirectX.Vector3 Position
        {
            get;
        }

        /// <summary>
        /// Point player is looking at
        /// </summary>
        Microsoft.DirectX.Vector3 LookingDirection
        {
            get;
        }

        /// <summary>
        /// Player Velocity
        /// </summary>
        Microsoft.DirectX.Vector3 Velocity
        {
            get;
        }

        /// <summary>
        /// Weapon player is carrying
        /// </summary>
        Constants.WeaponEnumeration Weapon
        {
            get;
        }

        /// <summary>
        /// Is player jumping
        /// default false
        /// </summary>
        bool Jump
        {
            get;
        }

        /// <summary>
        /// Is player shooting
        /// default false
        /// </summary>
        bool Shoot
        {
            get;
        }

        /// <summary>
        /// Is player ducking
        /// default false
        /// </summary>
        bool Duck
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Interface describing data about Player
    /// </summary>
    public interface IPlayerDataWrite
    {
        /// <summary>
        /// Player position
        /// </summary>
        Microsoft.DirectX.Vector3 Position
        {
            set;
        }

        /// <summary>
        /// Point player is looking at
        /// </summary>
        Microsoft.DirectX.Vector3 LookingDirection
        {
            set;
        }

        /// <summary>
        /// Player Velocity
        /// </summary>
        Microsoft.DirectX.Vector3 Velocity
        {
            set;
        }

        /// <summary>
        /// Weapon player is carrying
        /// </summary>
        Constants.WeaponEnumeration Weapon
        {
            set;
        }

        /// <summary>
        /// Is player jumping
        /// default false
        /// </summary>
        bool Jump
        {
            set;
        }

        /// <summary>
        /// Is player shooting
        /// default false
        /// </summary>
        bool Shoot
        {
            set;
        }

        /// <summary>
        /// Is player ducking
        /// default false
        /// </summary>
        bool Duck
        {
            set;
        }
    }
}

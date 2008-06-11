using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Interface describing data about other Players
    /// </summary>
    public interface IOtherPlayerData
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

        ///// <summary>
        ///// Is player ducking
        ///// default false
        ///// </summary>
        //bool Duck
        //{
        //    get;
        //}
    }
}

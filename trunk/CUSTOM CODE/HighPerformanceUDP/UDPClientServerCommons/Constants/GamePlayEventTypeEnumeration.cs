using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    /// <summary>
    /// events that occur during gameplay
    /// </summary>
    public enum GamePlayEventTypeEnumeration : ushort
    {
        /// <summary>
        /// when player jumps
        /// </summary>
        JumpNow = 0,
        /// <summary>
        /// when player shoots/stabs/punches :)
        /// </summary>
        UseWeapon = 1,
        /// <summary>
        /// occures when player changes his weapon
        /// </summary>
        WeaponChange = 2,
        /// <summary>
        /// Occures when player dies
        /// </summary>
        PlayerDead = 3,
        /// <summary>
        /// Occures when player dies and borns :)
        /// </summary>
        PlayerRespawn = 4,
        /// <summary>
        /// Occures when player took some damage
        /// </summary>   
        PlayerWasHit = 5
    }
}

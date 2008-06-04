using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    public enum GameEventTypeEnumeration : ushort
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
        /// occures when player changes his weapon to Crossbow
        /// </summary>
        SwitchWeaponToCrossbow = 2,
        /// <summary>
        /// occures when player changes his weapon to Sword
        /// </summary>
        SwitchWeaponToSword = 3,
        /// <summary>
        /// occures when player starts walking
        /// </summary>
        WalkNow = 4,
        /// <summary>
        /// occures when player starts running
        /// </summary>
        RunNow = 5,
        /// <summary>
        /// occures when player starts standing
        /// </summary>
        StandNow = 6,
        /// <summary>
        /// occures when player starts ducking
        /// </summary>
        DuckNow = 7
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    /// <summary>
    /// event that happens during game like join,quit,etc...
    /// </summary>
    public enum GameEventTypeEnumeration : ushort
    {
        /// <summary>
        /// when player joins
        /// </summary>
        PlayerJoined = 0,
        /// <summary>
        /// when player quits game
        /// </summary>
        PlayerQuitted = 1,
        /// <summary>
        /// when game started
        /// </summary>
        GameStared =2,
        /// <summary>
        /// Round endded
        /// </summary>
        EndRound=3,
        /// <summary>
        /// New round started
        /// </summary>
        NewRound=4
    }
}

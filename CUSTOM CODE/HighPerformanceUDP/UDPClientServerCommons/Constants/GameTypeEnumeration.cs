using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    public enum GameTypeEnumeration:ushort
    {
        /// <summary>
        /// Game ends when time limit is reached
        /// </summary>
        TimeLimit=0,
        /// <summary>
        /// Game ends when frag limit is reached
        /// </summary>
        FragLimit=1,
        /// <summary>
        /// Game ends when objective is accomplished
        /// </summary>
        Objective=2
    }
}

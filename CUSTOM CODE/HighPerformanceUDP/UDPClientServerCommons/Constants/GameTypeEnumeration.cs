using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    public enum GameTypeEnumeration:ushort
    {
        /// <summary>
        /// Game ends when time limit is reached (in minutes)
        /// </summary>
        TimeLimit=0,
        /// <summary>
        /// Game ends when frag limit is reached (frag limit for the team)
        /// </summary>
        FragLimit=1,
        /// <summary>
        /// Game ends when objective is accomplished (map has the description of the objective)
        /// </summary>
        Objective=2
    }
}

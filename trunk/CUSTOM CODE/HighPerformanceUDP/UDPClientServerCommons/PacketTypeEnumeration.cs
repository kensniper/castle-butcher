using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public enum PacketType :ushort
    {
        /// <summary>
        /// Standard server comunication type, With player positions and stuff
        /// </summary>
        Standard,
        Other //(?)
    }
}

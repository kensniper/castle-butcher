using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public enum PacketType : ushort
    {
        /// <summary>
        /// Standard server comunication type, With player positions and stuff
        /// </summary>
        StandardPacket = 0,
        /// <summary>
        /// JoinPacket - when player is joining to the Game
        /// </summary>
        JoinPacket = 1,
        /// <summary>
        /// QuitPacket - when player quits game
        /// </summary>
        QuitPacket = 2,
        /// <summary>
        /// GameInfo - Server informs players about current game state - also answer to QuitPacket and JoinPacket
        /// </summary>
        GameInfoPacket = 3
    }
}

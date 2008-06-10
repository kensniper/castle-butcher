using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Constants
{
    public enum PacketTypeEnumeration : ushort
    {
        /// <summary>
        /// Standard server comunication type, With player positions and stuff
        /// </summary>
        StandardServerPacket = 0,
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
        GameInfoPacket = 3,
        /// <summary>
        /// Standard client comunication type, contains all player info
        /// </summary>
        StandardClientPacket = 4,
        /// <summary>
        /// when the packet is damaged or/and can't be read
        /// </summary>
        Unknown = 5,
        /// <summary>
        /// Packet with error message
        /// </summary>
        MessagePacket = 6,
        /// <summary>
        /// TEMP
        /// </summary>
        ACK = 7
    }
}

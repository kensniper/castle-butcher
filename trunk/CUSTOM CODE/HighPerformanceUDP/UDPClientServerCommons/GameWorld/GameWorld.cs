using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public class GameWorld
    {
        /// <summary>
        /// Aproximation of player positions
        /// </summary>
        /// <param name="previousPacket">last received packet</param>
        /// <returns></returns>
        public List<PlayerInfo> GetApproximatedPlayersInfos(ServerPacket previousPacket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Aproximation of player positions
        /// </summary>
        /// <param name="previousPacket">last packet</param>
        /// <param name="beforePreviousPacket">packet received before "last"</param>
        /// <returns></returns>
        public List<PlayerInfo> GetApproximatedPlayersInfos(ServerPacket previousPacket,ServerPacket beforePreviousPacket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Aproximation of player positions
        /// </summary>
        /// <param name="previousPacket">last packet</param>
        /// <param name="beforePreviousPacket">packet received before "last"</param>
        /// <param name="beforeBeforePreviousPacket">packet received before packet received before "last" :)</param>
        /// <returns></returns>
        public List<PlayerInfo> GetApproximatedPlayersInfos(ServerPacket previousPacket, ServerPacket beforePreviousPacket, ServerPacket beforeBeforePreviousPacket)
        {
            throw new NotImplementedException();
        }
    }
}

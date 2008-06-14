using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Interface describing all Client methods
    /// </summary>
    public interface IClient:IClientForServerBase
    {
        /// <summary> Method that joins Player to server
        /// 
        /// </summary>
        /// <param name="ServerIp">server ip adress</param>
        /// <param name="PlayerName">name of joining player</param>
        /// <param name="GameId">game id</param>
        /// <returns>Player Id</returns>
        void JoinGame(IPEndPoint ServerIp, string PlayerName, ushort GameId, ushort TeamId);

        /// <summary>
        /// tells if receiving broadcast about lan games is on
        /// </summary>
        bool IsLanBroadcastReceivingOn
        {
            get;
        }
        
        /// <summary>
        /// Starts looking for current lan games by receiving lan broadcasts
        /// </summary>
        void StartLookingForLANGames();

        /// <summary>
        /// Says when the game has started, (everyone joined and playing)
        /// player needs to have Id (from GameInfo)
        /// and server has to send first serverPacket
        /// </summary>
        bool GameStarted
        {
            get;
        }

        /// <summary>
        /// True if player has been successfully connected to the game
        /// </summary>
        bool PlayerHasSuccesfullyJoinedGame
        {
            get;
        }

        /// <summary>
        /// Current Information about lan games in progress
        /// </summary>
        List<UDPClientServerCommons.Packets.GameInfoPacket> CurrentLanGames
        {
            get;
        }
    }
}

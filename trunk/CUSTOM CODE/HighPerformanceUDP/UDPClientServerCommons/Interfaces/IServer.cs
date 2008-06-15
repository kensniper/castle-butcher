using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UDPClientServerCommons.Server;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Server methods
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Ip of the server, null if error
        /// </summary>
        IPAddress MyIp
        {
            get;
        }

        /// <summary>
        /// Close and cleans up the server
        /// </summary>
        void Dispose();

        /// <summary>
        /// All player methods needed for not-dedicated server
        /// </summary>
        Interfaces.IClientForServer Client
        {
            get;
        }

        /// <summary>
        /// Starts local game, it also starts broadcasting game info all over network
        /// </summary>
        /// <param name="gameOptions"> game parameters</param>
        /// <param name="dedicatedServer">is server going to be dedicated</param>
        /// <param name="Me">if server is not dedicated - playerData is needed</param>
        /// <returns>Server Adress or null if error occured</returns>
        IPEndPoint StartLANServer(GameOptions gameOptions, bool dedicatedServer, Interfaces.IPlayerMe Me);

        /// <summary>
        /// Starts Game - starts sending player info
        /// todo: ignore join packets after game starts?
        /// </summary>
        /// <returns>true if everything went ok</returns>
        bool StartGame();

        /// <summary>
        /// METHOD NOT IMPLEMENTED - DO NOT USE
        /// </summary>
        /// <param name="gameOptions"></param>
        /// <param name="dedicatedServer"></param>
        /// <param name="Me"></param>
        /// <returns></returns>
        IPEndPoint StartINTERNETServer(GameOptions gameOptions, bool dedicatedServer, Interfaces.IPlayerMe Me);
    }
}

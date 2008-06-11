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
    public interface IClient
    {
        /// <summary> Send with with this method if no event occured
        /// 
        /// </summary>
        /// <param name="position">player new position</param>
        /// <param name="lookDirection">player new looking/aiming direction</param>
        /// <param name="moventDirection">player new movement vector</param>
        void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection);
        
        /// <summary> Send with this method if important event occured
        /// 
        /// </summary>
        /// <param name="weaponAttacked"> weapon that was used to atack</param>
        /// <param name="playerAttacked"> true if player used his weapon </param>
        /// <param name="playerJumped">true if player jumped</param>
        /// <param name="weaponChanged"> true if weapon changed</param>
        /// <param name="weaponNew"> new weapon carried by the player</param>
        void UpdatePlayerData(WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew);
        
        /// <summary> Send with this method if event occured and position changed
        /// 
        /// </summary>
        /// <param name="position">player new position</param>
        /// <param name="lookDirection">player new looking/aiming direction</param>
        /// <param name="moventDirection">player new movement vector</param>
        /// <param name="weaponAttacked"> weapon that was used to atack</param>
        /// <param name="playerAttacked"> true if player used his weapon </param>
        /// <param name="playerJumped">true if player jumped</param>
        /// <param name="weaponChanged"> true if weapon changed</param>
        /// <param name="weaponNew"> new weapon carried by the player</param>
        void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection, WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew);

        /// <summary> Send new playerData
        /// 
        /// </summary>
        /// <param name="playerData">info about the current player state</param>
        void UpdatePlayerData(Interfaces.IPlayerDataWrite playerData);
        
        /// <summary>
        /// Leave current game
        /// </summary>
        void LeaveGame();

        /// <summary> Method that joins Player to server
        /// 
        /// </summary>
        /// <param name="ServerIp">server ip adress</param>
        /// <param name="PlayerName">name of joining player</param>
        /// <param name="GameId">game id</param>
        /// <returns>Player Id</returns>
        void JoinGame(IPEndPoint ServerIp, string PlayerName, ushort GameId,ushort TeamId);

        ///// <summary>
        ///// Neweest ServerPacket received by the client
        ///// </summary>
        ///// <returns></returns>
        //IPacket GetNewestDataFromServer();

        /// <summary>
        /// tells if receiving broadcast about lan games is on
        /// </summary>
        bool IsLanBroadcastReceivingOn
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

        /// <summary>
        /// True if player has been successfully connected to the game
        /// </summary>
        bool PlayerHasSuccesfullyJoinedGame
        {
            get;
        }

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
        /// Starts looking for current lan games by receiving lan broadcasts
        /// </summary>
        void StartLookingForLANGames();

        /// <summary>
        /// Change team method
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        bool ChangeTeam(ushort TeamId);

        /// <summary>
        /// Current info about game player is playing
        /// </summary>
        GameInfoPacket CurrentGameInfo
        {
            get;
        }

        /// <summary>
        /// Information about players
        /// </summary>
        List<Interfaces.IOtherPlayerData> PlayerDataList
        {
            get;
        }

        List<Interfaces.IGameplayEvent> GameplayEventList
        {
            get;
        }

        List<Interfaces.IGameEvent> GameEventList
        {
            get;
        }
    }
}

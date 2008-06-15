using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Packets;
using UDPClientServerCommons.Constants;

namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// interface for Client methods on server
    /// (not dedicated server)
    /// </summary>
   public interface IClientForServerBase
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
        /// <returns>true if operation was successfull</returns>
        bool LeaveGame();

        /// <summary>
        /// Change team method
        /// </summary>
        /// <param name="TeamId">new teamId</param>
        /// <returns>true if successfull</returns>
        bool ChangeTeam(ushort TeamId);

        /// <summary>
        /// Current info about game player is playing, null if no data avalible
        /// </summary>
        GameInfoPacket CurrentGameInfo
        {
            get;
        }

        /// <summary>
        /// Information about players, null if no data avalible
        /// </summary>
        List<Interfaces.IOtherPlayerData> PlayerDataList
        {
            get;
        }

       /// <summary>
       /// Events that happend during gameplay (someone jumped etc.)
       /// </summary>
        List<Interfaces.IGameplayEvent> GameplayEventList
        {
            get;
        }

       /// <summary>
       /// Events that happend during game (someone joined etc.)
       /// </summary>
        List<Interfaces.IGameEvent> GameEventList
        {
            get;
        }

        /// <summary>
        /// id of current player
        /// </summary>
        ushort? PlayerId
        {
            get;
        }

        /// <summary>
        /// Id of current game player is playing
        /// </summary>
        ushort? GameId
        {
            get;
        }
            
        /// <summary>
        /// name of current player
        /// </summary>
        string PlayerName
        {
            get;
        }

        /// <summary>
        /// id of player team
        /// </summary>
        ushort? TeamId
        {
            get;
        }
    }
}

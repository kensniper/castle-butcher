using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UDPClientServerCommons.Constants;

namespace UDPClientServerCommons.Interfaces
{
    public interface ISend
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

        void LeaveGame();
        /// <summary> Method that joins Player to server
        /// 
        /// </summary>
        /// <param name="ServerIp">server ip adress</param>
        /// <param name="PlayerName">name of joining player</param>
        /// <param name="GameId">game id</param>
        /// <returns>Player Id</returns>
        void JoinGame(IPEndPoint ServerIp, string PlayerName, ushort GameId,ushort TeamId);
    }
}

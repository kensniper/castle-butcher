using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Packets;
using UDPClientServerCommons.Constants;

namespace UDPClientServerCommons.Server
{
    public class ClientForServer:Interfaces.IClient
    {
        public delegate void AddMessageDelegate(Interfaces.IPacket packet);

        public AddMessageDelegate AddMessageToServer;

        private AddMessageDelegate GetMessageFromServer;

        private bool gameIsRunningAsDedicatedServer = true;

        /// <summary>
        /// Tells if game is running as dedicated server
        /// </summary>
        public bool GameIsRunningAsDedicatedServer
        {
            get { return gameIsRunningAsDedicatedServer; }
            set { gameIsRunningAsDedicatedServer = value; }
        }

        private ushort? playerIdField = null;
        private ushort? gameIdField = null;
        private string playerNameField = null;
        private ushort? teamIdField = null;

        /// <summary>
        /// ClientPacket stuff
        /// </summary>
        private ClientPacket clientPacket = new ClientPacket();
        private readonly object clientPacketLock = new object();

        /// <summary>
        /// Last 10 packages that came from server
        /// </summary>
        private Usefull.Last10Packages last10Packeges = new UDPClientServerCommons.Usefull.Last10Packages();

        /// <summary>
        /// Last 10 packeges that came from server about each game info
        /// key parameter is gameId
        /// value parameter is Last10Packages
        /// </summary>
        private Dictionary<ushort, Usefull.Last10Packages> gameInfoPackets = new Dictionary<ushort, UDPClientServerCommons.Usefull.Last10Packages>();
        private readonly object gameInfoPacketsLock = new object();

        public ClientForServer()
        {
            GetMessageFromServer = new AddMessageDelegate(AsyncReceiveMessage);
        }

        public void ReceiveMessage(Interfaces.IPacket packet)
        {
            GetMessageFromServer.Invoke(packet);
        }

        private void AsyncReceiveMessage(Interfaces.IPacket packet)
        {

        }

 
        #region IClient Members

        public void JoinGame(ushort playerId, ushort gameId, string playerName, ushort teamId)
        {
            this.playerIdField = playerId;
            this.gameIdField = gameId;
            this.playerNameField = playerName;
            this.teamIdField = teamId;

            gameIsRunningAsDedicatedServer = false;
        }

        public void ChangeTeam(ushort teamId)
        {

        }

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection)
        {
            lock (clientPacketLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }

        public void UpdatePlayerData(WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (clientPacketLock)
            {
                clientPacket.PlayerShooting = playerAttacked;
                clientPacket.PlayerJumping = playerJumped;
                if (playerAttacked)
                    if (weaponAttacked == WeaponEnumeration.CrossBow)
                    {
                        clientPacket.PlayerCarringWeponOne = false;
                        clientPacket.PlayerCarringWeponTwo = true;
                    }
                    else
                    {
                        clientPacket.PlayerCarringWeponTwo = false;
                        clientPacket.PlayerCarringWeponOne = true;
                    }
                else
                    if (weaponChanged)
                    {
                        if (weaponNew == WeaponEnumeration.CrossBow)
                        {
                            clientPacket.PlayerCarringWeponOne = false;
                            clientPacket.PlayerCarringWeponTwo = true;
                        }
                        else
                        {
                            clientPacket.PlayerCarringWeponTwo = false;
                            clientPacket.PlayerCarringWeponOne = true;
                        }
                    }
            }
        }

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection, WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (clientPacketLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }


        public void LeaveGame()
        {
            throw new NotImplementedException();
        }

        public void JoinGame(System.Net.IPEndPoint ServerIp, string PlayerName, ushort GameId, ushort TeamId)
        {
            throw new NotImplementedException();
        }

        public UDPClientServerCommons.Interfaces.IPacket GetNewestDataFromServer()
        {
            throw new NotImplementedException();
        }

        public bool IsLanBroadcastReceivingOn
        {
            get { throw new NotImplementedException(); }
        }

        public List<GameInfoPacket> CurrentLanGames
        {
            get { throw new NotImplementedException(); }
        }

        public bool PlayerHasSuccesfullyJoinedGame
        {
            get { throw new NotImplementedException(); }
        }

        public bool GameStarted
        {
            get { throw new NotImplementedException(); }
        }

        public void StartLookingForLANGames()
        {
            throw new NotImplementedException();
        }

        bool UDPClientServerCommons.Interfaces.IClient.ChangeTeam(ushort TeamId)
        {
            throw new NotImplementedException();
        }

        public GameInfoPacket CurrentGameInfo
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

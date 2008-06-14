using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Packets;
using UDPClientServerCommons.Constants;
using System.Threading;
using UDPClientServerCommons.Interfaces;

namespace UDPClientServerCommons.Server
{
    public class ClientForServer:Interfaces.IClientForServer
    {
        #region fields

        public delegate void AddMessageDelegate(Interfaces.IPacket packet);

        public event AddMessageDelegate AddMessageToServer;
        /// <summary>
        /// Time in miliseconds
        /// </summary>
        public const int TimerTickPeriod = 100;

        /// <summary>
        /// timer
        /// </summary>
        private System.Threading.Timer timer;

        //public AddMessageDelegate AddMessageToServer;

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

        /// <summary>
        /// List of Game events
        /// </summary>        
        private List<IGameEvent> gameEventList = new List<IGameEvent>();
        private readonly object gameEventListLock = new object();

        /// <summary>
        /// List of gameplay events
        /// </summary>
        private List<IGameplayEvent> gameplayEventList = new List<IGameplayEvent>();
        private readonly object gameplayEventListLock = new object();        

#endregion

        public ClientForServer()
        {
            GetMessageFromServer = new AddMessageDelegate(AsyncReceiveMessage);

            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, TimerTickPeriod);
        }

        private void timerCallbackMethod(object target)
        {
            lock (clientPacketLock)
            {
                AddMessageToServer(clientPacket);
            }
        }

        private void StopSendingByTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void ReceiveMessage(Interfaces.IPacket packet)
        {
            GetMessageFromServer.Invoke(packet);
        }

        private void AsyncReceiveMessage(Interfaces.IPacket packet)
        {
            switch (packet.PacketType)
            {
                case PacketTypeEnumeration.StandardServerPacket:
                    List<Interfaces.IGameplayEvent> gameplayEvents = GameEvents.GameEventExtractor.GetGameplayEvents((ServerPacket)packet, (ServerPacket)last10Packeges.LastPacket, playerIdField);
                    lock (gameplayEventListLock)
                    {
                        for (int i = 0; i < gameplayEvents.Count; i++)
                            gameplayEventList.Add(gameplayEvents[i]);
                    }
                    last10Packeges.AddPacket(packet);
                    break;

                case PacketTypeEnumeration.GameInfoPacket:
                    GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;
                    if (gameIdField.HasValue && gameInfoPacket.GameId == gameIdField.Value)
                    {
                        List<IGameEvent> gameEvents = GameEvents.GameEventExtractor.GetGameEvents(gameInfoPacket, (GameInfoPacket)gameInfoPackets[gameIdField.Value].LastPacket, playerIdField);
                        lock (gameInfoPacketsLock)
                        {
                            for (int i = 0; i < gameEvents.Count; i++)
                            {
                                gameEventList.Add(gameEvents[i]);
                            }
                        }
                    }
                    if (gameIdField.HasValue && gameInfoPacket.GameId == gameIdField.Value)
                    {
                        lock (gameInfoPacketsLock)
                        {
                            gameInfoPackets[gameIdField.Value].AddPacket(gameInfoPacket);
                        }
                    }
                    break;
            }
        }

 
        #region IClient Members

        public void JoinGame(ushort playerId, ushort gameId, string playerName, ushort teamId)
        {
            this.playerIdField = playerId;
            this.gameIdField = gameId;
            this.playerNameField = playerName;
            this.teamIdField = teamId;

            gameIsRunningAsDedicatedServer = false;

            // start sending data to server
            timer.Change(0,TimerTickPeriod);
        }

        public bool ChangeTeam(ushort TeamId)
        {
            bool result = true;
            if (playerIdField.HasValue && gameIdField.HasValue && teamIdField.HasValue)
            {
                GameInfoPacket gPack = (GameInfoPacket)gameInfoPackets[gameIdField.Value].LastPacket;
                if (gPack == null)
                    return false;

                List<ushort> teamIdList = new List<ushort>();
                List<ushort> playerIdList = new List<ushort>();

                for (int k = 0; k < gPack.TeamScoreList.Count; k++)
                    teamIdList.Add(gPack.TeamScoreList[k].TeamId);
                for (int k = 0; k < gPack.PlayerStatusList.Count; k++)
                    teamIdList.Add(gPack.PlayerStatusList[k].PlayerId);

                //todo: exception ?
                //maybe it would be better to throw exception ??
                if (!teamIdList.Contains(TeamId))
                    return false;
                if (!playerIdList.Contains(playerIdField.Value))
                    return false;

                teamIdField = TeamId;

                JoinPacket joinPacket = new JoinPacket();
                joinPacket.PlayerId = playerIdField.Value;
                joinPacket.TeamId = teamIdField.Value;
                joinPacket.TimeStamp = DateTime.Now;
                joinPacket.PlayerName = playerNameField;
                joinPacket.GameId = gameIdField.Value;

                AddMessageToServer(joinPacket);
            }
            else
                result = false;

            return result;
        }

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection)
        {
            lock (clientPacketLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(moventDirection);
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
                clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(moventDirection);
            }
        }

        public void LeaveGame()
        {
            if (playerIdField.HasValue && gameIdField.HasValue)
            {
                LeaveGamePacket leaveGamePacket = new LeaveGamePacket();
                leaveGamePacket.GameId = gameIdField.Value;
                leaveGamePacket.PlayerId = playerIdField.Value;
                leaveGamePacket.TimeStamp = DateTime.Now;

                AddMessageToServer(leaveGamePacket);
            }
        }

        public bool PlayerHasSuccesfullyJoinedGame
        {
            get
            {
                if (playerIdField.HasValue && gameIdField.HasValue && teamIdField.HasValue && gameInfoPackets.ContainsKey(gameIdField.Value))
                {
                    GameInfoPacket pck = (GameInfoPacket)((GameInfoPacket)gameInfoPackets[gameIdField.Value].LastPacket).Clone();

                    if (pck == null)
                        return false;
                    else
                    {
                        for (int i = 0; i < pck.PlayerStatusList.Count; i++)
                        {
                            if (pck.PlayerStatusList[i].PlayerId == playerIdField.Value)
                                return true;
                        }
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        public bool GameStarted
        {
            get
            {
                return (playerIdField.HasValue && (last10Packeges.LastPacket != null));
            }
        }

        public GameInfoPacket CurrentGameInfo
        {
            get
            {
                lock (gameInfoPacketsLock)
                {
                    if (gameIdField.HasValue && gameInfoPackets.ContainsKey(gameIdField.Value))
                        return (GameInfoPacket)gameInfoPackets[gameIdField.Value].LastPacket.Clone();
                    else
                        return null;
                }
            }
        }

        public void UpdatePlayerData(UDPClientServerCommons.Interfaces.IPlayerDataWrite playerData)
        {
            Usefull.PlayerData pdata = (Usefull.PlayerData)playerData;
            lock (clientPacket)
            {
                clientPacket.PlayerJumping = pdata.Jump;
                clientPacket.PlayerShooting = pdata.Shoot;
                clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(pdata.Position);
                clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(pdata.Velocity);
                clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(pdata.LookingDirection);
                switch (pdata.Weapon)
                {
                    case WeaponEnumeration.CrossBow:
                        clientPacket.PlayerCarringWeponOne = true;
                        clientPacket.PlayerCarringWeponTwo = false;
                        break;
                    case WeaponEnumeration.None:
                        clientPacket.PlayerCarringWeponOne = false;
                        clientPacket.PlayerCarringWeponTwo = false;
                        break;
                    case WeaponEnumeration.Sword:
                        clientPacket.PlayerCarringWeponOne = false;
                        clientPacket.PlayerCarringWeponTwo = true;
                        break;
                }
            }
        }

        public List<UDPClientServerCommons.Interfaces.IOtherPlayerData> PlayerDataList
        {
            get
            {
                Interfaces.IPacket packet = last10Packeges.LastPacket;
                if (packet == null)
                {
                    return null;
                }
                else if (packet.PacketType == PacketTypeEnumeration.StandardServerPacket && this.playerIdField.HasValue)
                {
                    ServerPacket spack = (ServerPacket)packet;
                    List<IOtherPlayerData> list = new List<IOtherPlayerData>();
                    for (int i = 0; i < spack.PlayerInfoList.Count; i++)
                    {
                        if (spack.PlayerInfoList[i].PlayerId != this.playerIdField.Value)
                            list.Add(Translator.TranslatePlayerInfoToPlayerOtherData(spack.PlayerInfoList[i]));
                    }
                    return list;
                }
                else
                    return null;
            }
        }

        public List<UDPClientServerCommons.Interfaces.IGameplayEvent> GameplayEventList
        {
            get
            {
                lock (gameplayEventListLock)
                {
                    List<IGameplayEvent> list = new List<IGameplayEvent>(gameplayEventList);
                    gameplayEventList.Clear();
                    return list;
                }
            }
        }

        public List<UDPClientServerCommons.Interfaces.IGameEvent> GameEventList
        {
            get
            {
                lock (gameEventListLock)
                {
                    List<IGameEvent> list = new List<IGameEvent>(gameEventList);
                    gameEventList.Clear();
                    return list;
                }
            }
        }

        #endregion
    }
}

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
        public event EventHandler EndGameEvent;

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

        public AddMessageDelegate GetMessageFromServer;

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
        private Usefull.Last10Packages gameInfoPackets = new UDPClientServerCommons.Usefull.Last10Packages();
        
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

        private Usefull.PacketIdCounter packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();
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
                clientPacket.PacketId = packetIdCounter.Next();

                    //Console.WriteLine(DateTime.Now.ToLongTimeString());
                    AddMessageToServer(clientPacket);
                    clientPacket.PlayerJumping = false;
                    clientPacket.PlayerShooting = false;
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
                    //try
                    //{
                    //    if (last10Packeges.LastPacket!=null && last10Packeges.LastPacket.PacketId.Value == packet.PacketId.Value)
                    //    {
                    //        //the same packet - ignore it
                    //        Diagnostic.NetworkingDiagnostics.Logging.Warn("Old ServerPacket was received and ignored id" + packet.PacketId);
                    //        return;
                    //    }
                    //    else if (last10Packeges.LastPacket != null && last10Packeges.LastPacket.PacketId > packet.PacketId)
                    //    {
                    //        // old packet received
                    //        Diagnostic.NetworkingDiagnostics.Logging.Warn("Old ServerPacket was received and ignored id" + packet.PacketId);
                    //        List<Interfaces.IGameplayEvent> gpEvents = GameEvents.GameEventExtractor.GetGameplayEvents((ServerPacket)packet, null, playerIdField);
                    //        lock (gameplayEventListLock)
                    //        {
                    //            for (int i = 0; i < gpEvents.Count; i++)
                    //                gameplayEventList.Add(gpEvents[i]);
                    //        }
                    //        return;
                    //    }
                    //}
                    //catch (Usefull.IdCompareException idEx)
                    //{
                    //    Diagnostic.NetworkingDiagnostics.Logging.Error("Error when comparing id " + packet.PacketId + " with id " + last10Packeges.LastPacket.PacketId, idEx);
                    //}

                    List<Interfaces.IGameplayEvent> gameplayEvents = GameEvents.GameEventExtractor.GetGameplayEvents((ServerPacket)packet, (ServerPacket)last10Packeges.LastPacket, playerIdField);                     
                    lock (gameplayEventListLock)
                    {                        
                        for (int i = 0; i < gameplayEvents.Count; i++)
                            gameplayEventList.Add(gameplayEvents[i]);
                    }
                    if (last10Packeges.LastPacket == null)
                    {
                        //first server packet - game hast just started
                        lock (gameEventListLock)
                        {
                            gameEventList.Add(new GameEvents.GameStartedEvent());
                        }
                    }
                    last10Packeges.AddPacket(packet);
                    break;

                case PacketTypeEnumeration.GameInfoPacket:
                    GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;
                    if (gameIdField.HasValue && gameInfoPacket.GameId == gameIdField.Value)
                    {
                        //try
                        //{
                        //    // old packet received
                        //    if (gameInfoPackets.LastPacket != null && (gameInfoPackets.LastPacket.PacketId > gameInfoPacket.PacketId || gameInfoPackets.LastPacket.PacketId == gameInfoPacket.PacketId))
                        //        return;
                        //}
                        //catch (Usefull.IdCompareException idEx)
                        //{
                        //    Diagnostic.NetworkingDiagnostics.Logging.Error("Error when comparing id " + gameInfoPacket.PacketId + " with id " + gameInfoPackets.LastPacket.PacketId, idEx);
                        //    return;
                        //}

                        List<IGameEvent> gameEvents = GameEvents.GameEventExtractor.GetGameEvents(gameInfoPacket, (GameInfoPacket)gameInfoPackets.LastPacket, playerIdField);

                        lock (gameEventListLock)
                        {
                            for (int i = 0; i < gameEvents.Count; i++)
                            {
                                gameEventList.Add(gameEvents[i]);
                            }
                        }

                        gameInfoPackets.AddPacket(gameInfoPacket);
                    }
                    break;
            }
        }

 
        #region IClient Members

        /// <summary>
        /// adds player to game which server is hosting (player-server game)
        /// </summary>
        /// <param name="playerId">player id</param>
        /// <param name="gameId">game id</param>
        /// <param name="playerName">player name</param>
        /// <param name="teamId">team id</param>
        public bool JoinGame(ushort playerId, ushort gameId, string playerName, ushort teamId)
        {
            bool result = true;
            try
            {
                this.playerIdField = playerId;
                this.gameIdField = gameId;
                this.playerNameField = playerName;
                this.teamIdField = teamId;

                gameIsRunningAsDedicatedServer = false;

                lock (clientPacketLock)
                {
                    clientPacket.PlayerId = playerId;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("join game", ex);
            }
            return result;
        }

        /// <summary>
        /// start adding data to server (after game is started)
        /// </summary>
        public void StartSendingDataToServer()
        {
            // start sending data to server
            timer.Change(0, TimerTickPeriod);
        }

        /// <summary>
        /// Change team method
        /// </summary>
        /// <param name="TeamId">new teamId</param>
        /// <returns>true if successfull</returns>
        public bool ChangeTeam(ushort TeamId)
        {
            bool result = true;
            try
            {
                if (playerIdField.HasValue && gameIdField.HasValue && teamIdField.HasValue)
                {
                    GameInfoPacket gPack = (GameInfoPacket)gameInfoPackets.LastPacket;
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
                    joinPacket.PacketId = packetIdCounter.Next();

                    AddMessageToServer(joinPacket);
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("Change team S", ex);
                result = false;
            }
            return result;
        }

        /// <summary> Send with with this method if no event occured
        /// 
        /// </summary>
        /// <param name="position">player new position</param>
        /// <param name="lookDirection">player new looking/aiming direction</param>
        /// <param name="moventDirection">player new movement vector</param>
        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection)
        {
            lock (clientPacketLock)
            {
                try
                {
                    clientPacket.TimeStamp = DateTime.Now;
                    clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(position);
                    clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(lookDirection);
                    clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(moventDirection);
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Update player data S", ex);
                }
            }
        }

        /// <summary> Send with this method if important event occured
        /// 
        /// </summary>
        /// <param name="weaponAttacked"> weapon that was used to atack</param>
        /// <param name="playerAttacked"> true if player used his weapon </param>
        /// <param name="playerJumped">true if player jumped</param>
        /// <param name="weaponChanged"> true if weapon changed</param>
        /// <param name="weaponNew"> new weapon carried by the player</param>
        public void UpdatePlayerData(WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (clientPacketLock)
            {
                try
                {
                    clientPacket.TimeStamp = DateTime.Now;
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
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Update player data S", ex);
                }
            }
        }

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
        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection, WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (clientPacketLock)
            {
                try
                {
                    clientPacket.TimeStamp = DateTime.Now;
                clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(moventDirection);
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Update player data S", ex);
                }
            }
        }

        /// <summary>
        /// End current game
        /// </summary>
        /// <returns>true if operation was successfull</returns>
        public bool LeaveGame()
        {
            bool result = true;
            if (playerIdField.HasValue && gameIdField.HasValue)
            {
                try
                {
                    //LeaveGamePacket leaveGamePacket = new LeaveGamePacket();
                    //leaveGamePacket.GameId = gameIdField.Value;
                    //leaveGamePacket.PlayerId = playerIdField.Value;
                    //leaveGamePacket.TimeStamp = DateTime.Now;

                    //AddMessageToServer(leaveGamePacket);
                    // stop sending
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    if (EndGameEvent != null)
                    EndGameEvent(null, new EventArgs());
                }
                catch (Exception ex)
                {
                    result = false;
                    Diagnostic.NetworkingDiagnostics.Logging.Fatal("LeaveGame S", ex);
                }
            }
            else
                result = false;

            return result;
        }

        /// <summary>
        /// Information about players, null if no data avalible
        /// </summary>
        public GameInfoPacket CurrentGameInfo
        {
            get
            {
                try
                {
                    if (gameIdField.HasValue)
                        return (GameInfoPacket)gameInfoPackets.LastPacket.Clone();
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Fatal("CurrentGameInfo S", ex);
                }
                return null;
            }
        }

        /// <summary> Send new playerData
        /// 
        /// </summary>
        /// <param name="playerData">info about the current player state</param>
        public void UpdatePlayerData(UDPClientServerCommons.Interfaces.IPlayerDataWrite playerData)
        {
            Usefull.PlayerData pdata = (Usefull.PlayerData)playerData;
            lock (clientPacketLock)
            {
                try
                {
                    clientPacket.TimeStamp = DateTime.Now;
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
                catch (Exception ex) 
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("UpdatePlayerData S", ex);
                }
            }
        }

        /// <summary>
        /// Information about players, null if no data avalible
        /// </summary>
        public List<UDPClientServerCommons.Interfaces.IOtherPlayerData> PlayerDataList
        {
            get
            {
                try
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
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("Player Data list", ex);
                }
                return null;
            }
        }

        /// <summary>
        /// list of gameplay events, its cleard after get !
        /// </summary>
        public List<UDPClientServerCommons.Interfaces.IGameplayEvent> GameplayEventList
        {
            get
            {
                lock (gameplayEventListLock)
                {
                    try
                    {
                        List<IGameplayEvent> list = new List<IGameplayEvent>(gameplayEventList);
                        gameplayEventList.Clear();
                        return list;
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("GameplayEventList S", ex);
                    }
                    return new List<IGameplayEvent>();
                }
            }
        }

        /// <summary>
        /// list of game events, it's cleard after get
        /// </summary>
        public List<UDPClientServerCommons.Interfaces.IGameEvent> GameEventList
        {
            get
            {
                lock (gameEventListLock)
                {
                    try
                    {
                    List<IGameEvent> list = new List<IGameEvent>(gameEventList);
                    gameEventList.Clear();
                    return list;
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("GameEventList S", ex);
                    }
                    return new List<IGameEvent>();
                }
            }
        }

        /// <summary>
        /// id of current player
        /// </summary>
        public ushort? PlayerId
        {
            get { return playerIdField; }
        }

        /// <summary>
        /// id of player team
        /// </summary>
        public ushort? TeamId
        {
            get { return teamIdField; }
        }

        /// <summary>
        /// name of current player
        /// </summary>
        public string PlayerName
        {
            get { return playerNameField; }
        }

        /// <summary>
        /// Id of current game player is playing
        /// </summary>
        public ushort? GameId
        {
            get { return gameIdField; }
        }

        #endregion
    }
}

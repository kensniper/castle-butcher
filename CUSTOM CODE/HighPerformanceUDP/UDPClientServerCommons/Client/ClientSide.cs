using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons;
using System.Net;
using Clutch.Net.UDP;
using UDPClientServerCommons.Interfaces;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons.Client
{
    /// <summary>
    /// Networking class for Client
    /// </summary>
    public class ClientSide:IDisposable,IClient
    {
        #region fields

        public delegate void packetReceived(IPacket serverPacket);

        /// <summary>
        /// temporary event, fired after packet is received (game data updated)
        /// </summary>
        public event packetReceived PacketReceivedEvent;

        /// <summary>
        /// UDP networking layer
        /// </summary>
        private UDPLayer udpNetworking;

        private AckOperating ackOperating;

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

        private ushort? playerIdField = null;
        private ushort? gameIdField = null;
        private string playerNameField = null;
        private ushort? teamIdField = null;

        /// <summary>
        /// class used to receive lan broadcasts about current games
        /// </summary>
        private ClientLanBroadcast lanBroadcast = null;

        private Usefull.PacketIdCounter packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();

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

        #region Constructors

        public ClientSide()
        {
            udpNetworking = new UDPLayer();
            udpNetworking.GetData += new UDPLayer.GetDataToSend(udpNetworking_GetData);
            udpNetworking.PacketWasReceived += new UDPLayer.PacketReceived(udpNetworking_PacketWasReceived);

            //prepare first packet
            clientPacket = new ClientPacket();

            ackOperating = new AckOperating();
            ackOperating.SendPacketEvent += new AckOperating.SendPacket(ackOperating_SendPacketEvent);

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();
        }

        public ClientSide(int port)
        {
            udpNetworking = new UDPLayer(port);
            udpNetworking.GetData += new UDPLayer.GetDataToSend(udpNetworking_GetData);
            udpNetworking.PacketWasReceived += new UDPLayer.PacketReceived(udpNetworking_PacketWasReceived);

            //prepare first packet
            clientPacket = new ClientPacket();

            ackOperating = new AckOperating();
            ackOperating.SendPacketEvent += new AckOperating.SendPacket(ackOperating_SendPacketEvent);

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();
        }

        #endregion

        #region NetworkingEvents

        private void lanBroadCast_PacketWasReceived(UDPPacketBuffer udpPacketBuffer)
        {
            try
            {
                IPacket packet = PacketTypeChecker.GetPacket(udpPacketBuffer.Data);
                if (packet != null && packet.PacketType == PacketTypeEnumeration.GameInfoPacket)
                {
                    GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;
                    lock (gameInfoPacketsLock)
                    {
                        if (gameInfoPackets.ContainsKey(gameInfoPacket.GameId))
                            gameInfoPackets[gameInfoPacket.GameId].AddPacket(gameInfoPacket);
                        else
                            gameInfoPackets.Add(gameInfoPacket.GameId, new UDPClientServerCommons.Usefull.Last10Packages());
                    }
                    if (PacketReceivedEvent != null)
                        PacketReceivedEvent(gameInfoPacket);
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("lanBroadCast_PacketWasReceived", ex);
            }

        }

        private void ackOperating_SendPacketEvent(Interfaces.IPacket Packet)
        {
            try
            {
            udpNetworking.SendPacket(((Interfaces.ISerializablePacket)Packet).ToByte());
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("ackOperating_SendPacketEvent", ex);
            }
        }

        private void udpNetworking_PacketWasReceived(Clutch.Net.UDP.UDPPacketBuffer udpPacketBuffer)
        {
            try
            {
                IPacket packet = PacketTypeChecker.GetPacket(udpPacketBuffer.Data);
                if (PacketReceivedEvent != null)
                    PacketReceivedEvent(packet);
                switch (packet.PacketType)
                {
                    case PacketTypeEnumeration.PingRequest:
                        PingResponsePacket pingResponsePacket = new PingResponsePacket();
                        if (playerIdField.HasValue)
                        {
                            pingResponsePacket.PlayerId = playerIdField.Value;
                            pingResponsePacket.PacketId = packetIdCounter.Next();
                            pingResponsePacket.TimeStamp = DateTime.Now;
                            udpNetworking.SendPacket(pingResponsePacket.ToByte());
                        }
                        break;
                    case PacketTypeEnumeration.StandardServerPacket:
                        bool start = false;
                        if (last10Packeges.LastPacket == null)
                            start = true;
                        else
                        {
                            try
                            {
                                if (last10Packeges.LastPacket.PacketId.Value == packet.PacketId.Value)
                                {
                                    //the same packet - ignore it
                                    Diagnostic.NetworkingDiagnostics.Logging.Warn("Old ServerPacket was received and ignored id" + packet.PacketId);
                                    return;
                                }
                                else if (last10Packeges.LastPacket.PacketId > packet.PacketId)
                                {
                                    // old packet received
                                    Diagnostic.NetworkingDiagnostics.Logging.Warn("Old ServerPacket was received and ignored id" + packet.PacketId);
                                    List<Interfaces.IGameplayEvent> gpEvents = GameEvents.GameEventExtractor.GetGameplayEvents((ServerPacket)packet, null, playerIdField);
                                    lock (gameplayEventListLock)
                                    {
                                        for (int i = 0; i < gpEvents.Count; i++)
                                            gameplayEventList.Add(gpEvents[i]);
                                    }
                                    return;
                                }
                            }
                            catch (Usefull.IdCompareException idEx)
                            {
                                Diagnostic.NetworkingDiagnostics.Logging.Error("Error when comparing id " + packet.PacketId + " with id " + last10Packeges.LastPacket.PacketId, idEx);
                                return;
                            }
                        }

                        List<Interfaces.IGameplayEvent> gameplayEvents = GameEvents.GameEventExtractor.GetGameplayEvents((ServerPacket)packet, (ServerPacket)last10Packeges.LastPacket, playerIdField);
                        lock (gameplayEventListLock)
                        {
                            for (int i = 0; i < gameplayEvents.Count; i++)
                                gameplayEventList.Add(gameplayEvents[i]);
                        }
                        if (start)
                        {
                            lock (gameEventListLock)
                            {
                                gameEventList.Add(new GameEvents.GameStartedEvent());
                            }
                        }
                        last10Packeges.AddPacket(packet);
                        break;
                    case PacketTypeEnumeration.GameInfoPacket:
                        GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;
                        lock (gameInfoPacketsLock)
                        {
                            if (gameInfoPackets.ContainsKey(gameInfoPacket.GameId))
                            {
                                if (gameInfoPackets[gameInfoPacket.GameId].LastPacket.PacketId > packet.PacketId || gameInfoPackets[gameInfoPacket.GameId].LastPacket.PacketId == packet.PacketId)
                                {
                                    Diagnostic.NetworkingDiagnostics.Logging.Warn("Old GameInfoPacket was received and ignored");
                                    return;
                                }
                                gameInfoPackets[gameInfoPacket.GameId].AddPacket(gameInfoPacket);
                            }
                            else
                            {
                                gameInfoPackets.Add(gameInfoPacket.GameId, new UDPClientServerCommons.Usefull.Last10Packages());
                                gameInfoPackets[gameInfoPacket.GameId].AddPacket(gameInfoPacket);
                            }

                            //if this is my game - get events
                            if (gameIdField.HasValue && gameInfoPacket.GameId == gameIdField.Value)
                            {
                                List<IGameEvent> gameEvents = GameEvents.GameEventExtractor.GetGameEvents(gameInfoPacket, (GameInfoPacket)gameInfoPackets[gameIdField.Value].GetPrevious(2), playerIdField);
                                lock (gameInfoPacketsLock)
                                {
                                    for (int i = 0; i < gameEvents.Count; i++)
                                        gameEventList.Add(gameEvents[i]);
                                }
                            }
                        }
                        //if this is my game - update player status'es
                        if (gameIdField.HasValue && gameInfoPacket.GameId == gameIdField)
                        {
                            for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                            {
                                if (gameInfoPacket.PlayerStatusList[i].PlayerName == playerNameField)
                                {
                                    lock (clientPacketLock)
                                    {
                                        playerIdField = gameInfoPacket.PlayerStatusList[i].PlayerId;
                                        teamIdField = gameInfoPacket.PlayerStatusList[i].PlayerTeam;
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Packet was received", ex);
            }
        }

        /// <summary>
        /// Method used by UDP network layer to get client network data
        /// </summary>
        /// <returns></returns>
        private ClientPacket udpNetworking_GetData()
        {
            lock (clientPacketLock)
            {
                if (GameStarted)
                {
                      try
                        {
                            clientPacket.PacketId = packetIdCounter.Next();
                            clientPacket.PlayerId = playerIdField.Value;

                                return (ClientPacket)clientPacket.Clone();
                          
                        }
                        catch (Exception ex)
                        {
                            Diagnostic.NetworkingDiagnostics.Logging.Fatal("udpNetworking_GetData", ex);
                        }
                        finally
                        {
                            clientPacket.PlayerJumping = false;
                            clientPacket.PlayerShooting = false;
                        }
                        return null;
                }
                else
                    return null;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Closes gently this instance of GameServer
        /// </summary>
        public void Dispose()
        {
            try
            {
                LeaveGame();
                System.Threading.Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Error on Dispose", ex);
            }
            finally
            {
                udpNetworking.Dispose();
              //  if (lanBroadcast!= null && lanBroadcast.IsRunning)
                // why isRunning is needed ??
                if(lanBroadcast!=null)
                    lanBroadcast.Dispose();
            }
        }

        #endregion

        #region IClient Members

        /// <summary>
        /// Curent Lan games found in broadcast, empty list if not found
        /// </summary>
        public List<GameInfoPacket> CurrentLanGames
        {
            get
            {
                List<GameInfoPacket> list = new List<GameInfoPacket>();
                lock (gameInfoPacketsLock)
                {
                    try
                    {
                        if (gameInfoPackets.Count > 0)
                            foreach (ushort key in gameInfoPackets.Keys)
                            {
                                GameInfoPacket gip = (GameInfoPacket)gameInfoPackets[key].LastPacket;
                                if (gip != null)
                                    list.Add(gip);
                            }
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("CurrentLanGames", ex);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// True if player has been successfully connected to the game
        /// </summary>
        public bool PlayerHasSuccesfullyJoinedGame
        {
            get
            {
                try
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
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("PlayerHasSuccesfullyJoinedGame", ex);
                }
                return false;
            }
        }

        /// <summary>
        /// Says when the game has started, (everyone joined and playing)
        /// player needs to have Id (from GameInfo)
        /// and server has to send first serverPacket
        /// </summary>
        public bool GameStarted
        {
            get
            {
                return (playerIdField.HasValue && (last10Packeges.LastPacket != null));
            }
        }

        /// <summary>
        /// tells if receiving broadcast about lan games is on
        /// </summary>
        public bool IsLanBroadcastReceivingOn
        {
            get { return lanBroadcast.IsRunning; }
        }

        /// <summary>
        /// Starts looking for current lan games by receiving lan broadcasts
        /// </summary>
        /// <returns>true if everything went ok</returns>
        public bool StartLookingForLANGames()
        {
            bool result = false;
            try
            {
                if (lanBroadcast == null)
                {
                    lanBroadcast = new ClientLanBroadcast();
                    lanBroadcast.PacketWasReceived += new ClientLanBroadcast.PacketReceived(lanBroadCast_PacketWasReceived);

                    // do something with socket error
                    result = lanBroadcast.StartBroadcastReceiving();
                }
                else
                {
                    if (!lanBroadcast.IsRunning)
                        lanBroadcast.START();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Diagnostic.NetworkingDiagnostics.Logging.Error("StartLookingForLanGames", ex);
            }
            return result;
        }

        /// <summary> Method that joins Player to server
        /// 
        /// </summary>
        /// <param name="ServerIp">server ip adress</param>
        /// <param name="PlayerName">name of joining player</param>
        /// <param name="GameId">game id</param>
        /// <returns>Player Id</returns>
        public bool JoinGame(IPEndPoint ServerIp, string PlayerName, ushort GameId, ushort TeamId)
        {
            bool result = true;
            try
            {
                if (lanBroadcast != null)
                    lanBroadcast.STOP();

                udpNetworking.ServerIp = ServerIp;
                udpNetworking.StartUdpSocket();
                JoinPacket joinPacket = new JoinPacket();

                joinPacket.PacketId = packetIdCounter.Next();
                // id <1000 means that id hasn't been selected
                joinPacket.PlayerId = 0;
                joinPacket.PlayerName = PlayerName;
                joinPacket.GameId = GameId;
                joinPacket.TeamId = TeamId;

                playerNameField = PlayerName;
                gameIdField = GameId;
                teamIdField = TeamId;

                // now we should inform player about other palyer that are in the game he wants to join
                GameInfoPacket gip = null;
                lock (gameInfoPacketsLock)
                {
                    gip = (GameInfoPacket) gameInfoPackets[GameId].LastPacket;
                }
                if (gip != null)
                {
                    lock(gameEventListLock)
                    {
                        for (int i = 0; i < gip.PlayerStatusList.Count; i++)
                            gameEventList.Add(new GameEvents.PlayerJoinedEvent(gip.PlayerStatusList[i].PlayerId, gip.PlayerStatusList[i].PlayerName, gip.PlayerStatusList[i].PlayerTeam));
                    }
                }

                udpNetworking.SendPacket(joinPacket.ToByte());
            }
            catch (Exception ex)
            {
                result = false;
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("Join", ex);
            }
            return result;
        }

        /// <summary>
        /// Change team method
        /// </summary>
        /// <param name="TeamId">new teamId</param>
        /// <returns>true if successfull</returns>
        public bool ChangeTeam(ushort TeamId)
        {
            bool result = true;
            if (playerIdField.HasValue && gameIdField.HasValue && teamIdField.HasValue)
            {
                try
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
                    joinPacket.PacketId = packetIdCounter.Next();

                    udpNetworking.SendPacket(joinPacket.ToByte());
                }
                catch (Exception ex)
                {
                    result = false;
                    Diagnostic.NetworkingDiagnostics.Logging.Fatal("change team", ex);
                }
            }
            else
                result = false;

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
                    Diagnostic.NetworkingDiagnostics.Logging.Error("UpdatePlayerData", ex);
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
                    Diagnostic.NetworkingDiagnostics.Logging.Error("UpdatePlayerData", ex);
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
                    Diagnostic.NetworkingDiagnostics.Logging.Error("UpdatePlayerData", ex);
                }
            }
        }

        /// <summary>
        /// tries to leave game
        /// </summary>
        /// <returns>true if operation was successfull</returns>
        public bool LeaveGame()
        {
            bool result = true;
            if (playerIdField.HasValue && gameIdField.HasValue)
            {
                try
                {
                    LeaveGamePacket leaveGamePacket = new LeaveGamePacket();
                    leaveGamePacket.GameId = gameIdField.Value;
                    leaveGamePacket.PlayerId = playerIdField.Value;
                    leaveGamePacket.TimeStamp = DateTime.Now;
                    leaveGamePacket.PacketId.Value = packetIdCounter.Next();

                    //ackOperating.SendPacketNeededAck(leaveGamePacket);
                    udpNetworking.SendPacket(leaveGamePacket.ToByte());

                    udpNetworking.StopSendingByTimer();
                    //udpNetworking.Dispose();

                    playerIdField = null;
                    gameIdField = null;
                    teamIdField = null;

                    //System.Threading.Thread.Sleep(10);
                    //udpNetworking.Dispose();
                    if (lanBroadcast.IsRunning)
                    {
                        lanBroadcast.STOP();
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    Diagnostic.NetworkingDiagnostics.Logging.Fatal("Leave error", ex);
                }                
            }
            else
                return false;
            return result;
        }

        /// <summary>
        /// information about current game, returns null if no info avalible
        /// </summary>
        public GameInfoPacket CurrentGameInfo
        {
            get
            {
                lock (gameInfoPacketsLock)
                {
                    try
                    {
                        if (gameIdField.HasValue && gameInfoPackets.ContainsKey(gameIdField.Value))
                            return (GameInfoPacket)gameInfoPackets[gameIdField.Value].LastPacket.Clone();
                        else
                            return null;
                    }

                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("CurrentGameInfo", ex);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// returns list of data about other players playing, returns null if no info avalible
        /// </summary>
        public List<IOtherPlayerData> PlayerDataList
        {
            get {
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
                    Diagnostic.NetworkingDiagnostics.Logging.Error("PlayerDataList", ex);
                }
                return null;
            }
        }

        /// <summary>
        /// updates client packet with new player data
        /// </summary>
        /// <param name="playerData">new player data</param>
        public void UpdatePlayerData(IPlayerDataWrite playerData)
        {
            Usefull.PlayerData pdata = (Usefull.PlayerData)playerData;
            lock (clientPacket)
            {
                try
                {
                    clientPacket.PlayerJumping = pdata.Jump;
                    clientPacket.PlayerShooting = pdata.Shoot;
                    clientPacket.PlayerPosition = Translator.TranslateBetweenVectorAndVector(pdata.Position);
                    clientPacket.PlayerMovementDirection = Translator.TranslateBetweenVectorAndVector(pdata.Velocity);
                    clientPacket.PlayerLookingDirection = Translator.TranslateBetweenVectorAndVector(pdata.LookingDirection);
                    clientPacket.TimeStamp = DateTime.Now;

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
                    Diagnostic.NetworkingDiagnostics.Logging.Error("UpdatePlayerData", ex);
                }
            }
        }

        /// <summary>
        /// list of gameplay events, its cleard after get !
        /// </summary>
        public List<IGameplayEvent> GameplayEventList
        {
            get {
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
                        Diagnostic.NetworkingDiagnostics.Logging.Error("GameplayEventList", ex);
                    }
                    return new List<IGameplayEvent>();
                }
            }
        }

        /// <summary>
        /// list of game events, it's cleard after get
        /// </summary>
        public List<IGameEvent> GameEventList
        {
            get {
                lock (gameEventListLock)
                {
                    try{
                    List<IGameEvent> list = new List<IGameEvent>(gameEventList);
                    gameEventList.Clear();
                    return list;
                    }
                    catch (Exception ex)
                    {
                        Diagnostic.NetworkingDiagnostics.Logging.Error("GameEventList", ex);
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

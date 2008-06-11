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

        /// <summary>
        /// constants about current game
        /// </summary>
        private ushort? playerIdField = null;
        private ushort? gameIdField = null;
        private string playerNameField = null;
        private ushort? teamIdField = null;

        /// <summary>
        /// class used to receive lan broadcasts about current games
        /// </summary>
        private ClientLanBroadcast lanBroadcast = null;

        private Usefull.PacketIdCounter packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();

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
            IPacket packet = PacketTypeChecker.GetPacket(udpPacketBuffer.Data);
            if (packet != null && packet.PacketType == PacketTypeEnumeration.GameInfoPacket)
            {
                GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;
                lock (gameInfoPacket)
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

        private void ackOperating_SendPacketEvent(Interfaces.IPacket Packet)
        {
            udpNetworking.SendPacket(((Interfaces.ISerializablePacket)Packet).ToByte());
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
                    case PacketTypeEnumeration.StandardServerPacket:
                        last10Packeges.AddPacket(packet);
                        break;
                    case PacketTypeEnumeration.GameInfoPacket:
                        GameInfoPacket gameInfoPacket = (GameInfoPacket)packet;

                        lock (gameInfoPacket)
                        {
                            if (gameInfoPackets.ContainsKey(gameInfoPacket.GameId))
                                gameInfoPackets[gameInfoPacket.GameId].AddPacket(gameInfoPacket);
                            else
                                gameInfoPackets.Add(gameInfoPacket.GameId, new UDPClientServerCommons.Usefull.Last10Packages());
                        }

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
                    lock (clientPacketLock)
                    {
                        clientPacket.PlayerId = playerIdField.Value;
                        return (ClientPacket)clientPacket.Clone();
                    }
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
                if (lanBroadcast.IsRunning)
                    lanBroadcast.Dispose();
            }
        }

        #endregion

        #region IClient Members

        public List<GameInfoPacket> CurrentLanGames
        {
            get
            {
                List<GameInfoPacket> list = new List<GameInfoPacket>();
                lock (gameInfoPacketsLock)
                {
                    if (gameInfoPackets.Count > 0)
                        foreach (ushort key in gameInfoPackets.Keys)
                        {
                            GameInfoPacket gip = (GameInfoPacket)gameInfoPackets[key].LastPacket;
                            if (gip != null)
                                list.Add(gip);
                        }
                }
                return list;
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

        public void StartLookingForLANGames()
        {
            if (lanBroadcast == null)
            {
                lanBroadcast = new ClientLanBroadcast();
                lanBroadcast.PacketWasReceived += new ClientLanBroadcast.PacketReceived(lanBroadCast_PacketWasReceived);
            }
            lanBroadcast.StartBroadcastReceiving();
        }

        public void JoinGame(IPEndPoint ServerIp, string PlayerName, ushort GameId, ushort TeamId)
        {
            if (lanBroadcast != null)
                lanBroadcast.Dispose();

            udpNetworking.ServerIp = ServerIp;
            udpNetworking.StartUdpSocket();
            JoinPacket joinPacket = new JoinPacket();

            // id <1000 means that id hasn't been selected
            joinPacket.PlayerId = 0;
            joinPacket.PlayerName = PlayerName;
            joinPacket.GameId = GameId;
            joinPacket.TeamId = TeamId;

            playerNameField = PlayerName;
            gameIdField = GameId;
            teamIdField = TeamId;

            udpNetworking.SendPacket(joinPacket.ToByte());
        }

        public bool ChangeTeam(ushort TeamId)
        {
            bool result = true;
            if (playerIdField.HasValue && gameIdField.HasValue && teamIdField.HasValue)
            {
                List<GameInfoPacket> gameInfoPacketList = new List<GameInfoPacket>(CurrentLanGames);
                if (gameInfoPacketList == null)
                    return false;

                bool gameOk = false;
                List<ushort> teamIdList = new List<ushort>();
                List<ushort> playerIdList = new List<ushort>();

                for (int i = 0; i < gameInfoPacketList.Count; i++)
                {
                    if (gameInfoPacketList[i].GameId == gameIdField)
                    {
                        for (int k = 0; k < gameInfoPacketList[i].TeamScoreList.Count; k++)
                            teamIdList.Add(gameInfoPacketList[i].TeamScoreList[k].TeamId);
                        for (int k = 0; k < gameInfoPacketList[i].PlayerStatusList.Count; k++)
                            teamIdList.Add(gameInfoPacketList[i].PlayerStatusList[k].PlayerId);

                        gameOk = true;
                        break;
                    }
                }

                if (!gameOk)
                    return false;
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
            else
                result = false;

            return result;
        }

        //public IPacket GetNewestDataFromServer()
        //{
        //    return last10Packeges.LastPacket;
        //}

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
            if (!playerIdField.HasValue)
                throw new Usefull.PlayerIdNullException();
            if (!gameIdField.HasValue)
                throw new NullReferenceException("Game id was null");

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

            System.Threading.Thread.Sleep(10);
            udpNetworking.Dispose();
            if (lanBroadcast.IsRunning)
            {
                lanBroadcast.Dispose();
                lanBroadcast = null;
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

        public List<IOtherPlayerData> PlayerDataList
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdatePlayerData(IPlayerDataWrite playerData)
        {
            throw new NotImplementedException();
        }

        public List<IGameplayEvent> GameplayEventList
        {
            get { throw new NotImplementedException(); }
        }

        public List<IGameEvent> GameEventList
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

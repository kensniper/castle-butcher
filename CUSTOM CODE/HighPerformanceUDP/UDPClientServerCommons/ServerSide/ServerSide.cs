using System;
using System.Collections.Generic;
using System.Text;
using Clutch.Net.UDP;
using System.Net;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons.Server
{
    /// <summary>
    /// Server Side networking of Castle Strike
    /// </summary>
    public class ServerSide:MyUdpServer,IDisposable,Interfaces.IServer
    {
        #region fields

        private const int _TimerTickPeriod = 100;

        /// <summary>
        /// Server packet parts
        /// </summary>
        private readonly object serverPacketLock = new object();
        private ServerPacket serverPacket = new ServerPacket();
        private Dictionary<int, Interfaces.IPacket> lastPackages = new Dictionary<int, Interfaces.IPacket>();
        private Last10 last10 = new Last10();
        private List<EndPoint> cliendAdressList = new List<EndPoint>();
        
        /// <summary>
        /// Not used but may be needed in the future
        /// </summary>
        //private AckOperating ackOperating;

        /// <summary>
        /// Game info parts
        /// </summary>
        private readonly object gameInfoPacketLock = new object();
        private GameInfoPacket gameInfoPacket = new GameInfoPacket();
        private int gameInfoPacketSendCounter = 0;
        private bool gameStarted = false;
        private ServerLanBroadcast lanBroadcast = null;

        private System.Threading.Timer timer;
        private Usefull.PacketIdCounter packetIdCounter;

        /// <summary>
        /// when server is not dedicated
        /// </summary>
        private ClientForServer clientForServer = null;

        private Dictionary<ushort, Usefull.Last10Packages> clientPackagesDictionary = new Dictionary<ushort, UDPClientServerCommons.Usefull.Last10Packages>();
        private readonly object clientPackagesDictionaryLock = new object();

        /// <summary>
        /// method for client stuff when server is not dedicated
        /// </summary>
        public Interfaces.IClientForServer Client
        {
            get { return clientForServer; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Starts local game, it also starts broadcasting game info all over network
        /// </summary>
        /// <param name="gameOptions"> game parameters</param>
        /// <param name="dedicatedServer">is server going to be dedicated</param>
        /// <param name="Me">if server is not dedicated - playerData is needed</param>
        /// <returns>Server Adress or null if error occured</returns>
        public IPEndPoint StartLANServer(GameOptions gameOptions,bool dedicatedServer,Interfaces.IPlayerMe Me)
        {
            try
            {
                if(lanBroadcast == null)
                lanBroadcast = new ServerLanBroadcast();
                Usefull.PlayerMe me = (Usefull.PlayerMe)Me;
                PlayerStatus ps = new PlayerStatus();
                lock (gameInfoPacketLock)
                {
                    Random rand = new Random(gameOptions.GetHashCode());
                    gameInfoPacket.GameId = (ushort)rand.Next(1000, 9999);
                    gameInfoPacket.GameType = gameOptions.GameType;
                    gameInfoPacket.Limit = gameOptions.GameLimit;
                    gameInfoPacket.PlayerStatusList = new List<PlayerStatus>();
                    gameInfoPacket.RoundNumber = 1;
                    if (!dedicatedServer && me != null)
                    {
                        ps.PlayerId = (ushort)rand.Next(1000, 9999);
                        ps.PlayerName = me.PlayerName;
                        if (me.PlayerTeam == gameOptions.TeamOneName)
                            ps.PlayerTeam = 13;
                        else
                            ps.PlayerTeam = 39;

                        if (clientForServer == null)
                        {
                            clientForServer = new ClientForServer();
                            clientForServer.AddMessageToServer += new ClientForServer.AddMessageDelegate(clientForServer_AddMessageToServer);
                            clientForServer.EndGameEvent += new EventHandler(clientForServer_EndGameEvent);
                        }
                        clientForServer.JoinGame(ps.PlayerId, gameInfoPacket.GameId, ps.PlayerName, ps.PlayerTeam);
                    }

                    gameInfoPacket.TeamScoreList = new List<TeamData>();
                    TeamData ts1 = new TeamData(13, gameOptions.TeamOneName);
                    TeamData ts2 = new TeamData(39, gameOptions.TeamTwoName);
                    gameInfoPacket.ServerAddress = base.ServerIpAdress;
                    gameInfoPacket.TeamScoreList.Add(ts1);
                    gameInfoPacket.TeamScoreList.Add(ts2);
                    gameInfoPacketSendCounter = 0;
                }
                if (!dedicatedServer && me != null)
                {
                    lock (serverPacketLock)
                    {
                        PlayerInfo pi = new PlayerInfo();
                        pi.PlayerId = ps.PlayerId;
                        serverPacket.PlayerInfoList.Add(pi);
                    }
                }

                timer.Change(0, _TimerTickPeriod);
                return base.Start();
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("StartLanServer S", ex);
            }
            return null;
        }
        
        private void clientForServer_EndGameEvent(object sender, EventArgs e)
        {
            lock (gameInfoPacketLock)
            {
                try
                {
                    UDPPacketBuffer buff = new UDPPacketBuffer();
                    buff.Data = gameInfoPacket.ToMinimalByte();
                    buff.DataLength = gameInfoPacket.ByteCount;
                    gameInfoPacket.RoundNumber = 0;
                    for (int i = 0; i < cliendAdressList.Count; i++)
                    {
                        buff.RemoteEndPoint = cliendAdressList[i];
                        base.AsyncBeginSend(buff);
                    }
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("EndGameEvent", ex);
                }
            }
            System.Threading.Thread.Sleep(200);
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            cliendAdressList.Clear();
            lock (clientPackagesDictionaryLock)
            {
                clientPackagesDictionary = new Dictionary<ushort, UDPClientServerCommons.Usefull.Last10Packages>();
            }
            lock (gameInfoPacketLock)
            {
                gameInfoPacket = new GameInfoPacket();
            }
            last10 = new Last10();
            lock (serverPacketLock)
            {
                serverPacket = new ServerPacket();
            }
            
        }

        /// <summary>
        /// Starts Game - starts sending player info
        /// todo: ignore join packets after game starts?
        /// </summary>
        /// <returns>true if everything went ok</returns>
        public bool StartGame()
        {
            bool ret = true;
            lock (gameInfoPacketLock)
            {
                try
                {
                    if (gameInfoPacket.TeamScoreList.Count == 2 &&
                        cliendAdressList.Count > 1 &&
                        gameInfoPacket.PlayerStatusList.Count > 1)
                    {
                        int teamOne = 0;
                        int teamTwo = 0;
                        for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                        {
                            if (gameInfoPacket.PlayerStatusList[i].PlayerTeam == gameInfoPacket.TeamScoreList[0].TeamId)
                                teamOne++;
                            if (gameInfoPacket.PlayerStatusList[i].PlayerTeam == gameInfoPacket.TeamScoreList[1].TeamId)
                                teamTwo++;
                        }
                        if (teamOne == 0 || teamTwo == 0)
                            ret = false;
                    }
                    else
                        ret = false;

                }
                catch (Exception ex)
                {
                    ret = false;
                    Diagnostic.NetworkingDiagnostics.Logging.Fatal("Couldnt start game!!", ex);
                }
                if (ret)
                {
                    // everything went OK so we can start
                    gameStarted = true;
                    if (Client != null && !Client.GameIsRunningAsDedicatedServer)
                        clientForServer.StartSendingDataToServer();
                }
            }
            return ret;
        }

        /// <summary>
        /// Method not implemented !!!
        /// </summary>
        /// <param name="gameOptions"></param>
        /// <returns></returns>
        public IPEndPoint StartINTERNETServer(GameOptions gameOptions,bool dedicatedServer,Interfaces.IPlayerMe Me)
        {
            throw new NotImplementedException("Option for internet gaming has not been implemented");
        }

        public ServerSide(int port)
            : base( port)
        {
            if (MyIp != null)
                ServerIpAdress = new IPEndPoint(MyIp,port);
            base.MessageWasReceivedEvent += new EventHandler(myUdpServer_MessageWasReceivedEvent);
            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite,_TimerTickPeriod );
           // ackOperating = new AckOperating();

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();

            //counter
            packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();
        }

        public ServerSide(IPEndPoint ServerIp)
            : base(ServerIp)
        {
            base.MessageWasReceivedEvent += new EventHandler(myUdpServer_MessageWasReceivedEvent);
            System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(timerCallbackMethod);
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, _TimerTickPeriod);
            //ackOperating = new AckOperating();

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();

            //counter
            packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();
        }

        #endregion

        private void clientForServer_AddMessageToServer(UDPClientServerCommons.Interfaces.IPacket packet)
        {
            if (packet.PacketType == PacketTypeEnumeration.StandardClientPacket)
            {
                // adding client info to server packet
                ClientPacket cp = (ClientPacket)packet;
                lock (serverPacketLock)
                {
                    for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                        if (serverPacket.PlayerInfoList[i].PlayerId == cp.PlayerId)
                        {
                            serverPacket.PlayerInfoList[i] = Translator.TranslateBetweenClientPacketAndPlayerInfo(cp);
                            break;
                        }
                }
            }
            if (packet.PacketType == PacketTypeEnumeration.JoinPacket)
            {
                JoinPacket joinPacket = (JoinPacket)packet;
                lock (serverPacketLock)
                {
                    for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                    {
                        if (gameInfoPacket.PlayerStatusList[i].PlayerId == joinPacket.PlayerId)
                        {
                            // we update playerName
                            gameInfoPacket.PlayerStatusList[i].PlayerName = joinPacket.PlayerName;
                            // change team request
                            gameInfoPacket.PlayerStatusList[i].PlayerTeam = joinPacket.TeamId;
                            break;
                        }
                    }
                }
            }
        }

        private void timerCallbackMethod(object target)
        {
            bool gameHasStarted = false;
            lock (gameInfoPacketLock)
            {
                try
                {
                    gameHasStarted = gameStarted;
                    if (!gameStarted)
                    {
                        if (gameInfoPacketSendCounter == 0)
                        {
                            // game hasnt started yet so sent game info more often
                            UDPPacketBuffer buff = new UDPPacketBuffer();
                            gameInfoPacket.TimeStamp = DateTime.Now;
                            gameInfoPacket.PacketId.Value = packetIdCounter.Next();

                            buff.Data = gameInfoPacket.ToMinimalByte();
                            buff.DataLength = buff.Data.Length;

                            // broadcasting GameInfo to everyone
                            lanBroadcast.SendAsync(buff.Data);
                            Console.WriteLine("Sending broadcast");
                            for (int i = 0; i < cliendAdressList.Count; i++)
                            {
                                Console.WriteLine("Sending gameInfo to : {0}", cliendAdressList[i]);
                                buff.RemoteEndPoint = cliendAdressList[i];
                                base.AsyncBeginSend(buff);
                            }
                            if(clientForServer!=null && !clientForServer.GameIsRunningAsDedicatedServer)
                            clientForServer.GetMessageFromServer(gameInfoPacket);
                            // we send every 10 ticks = every second
                            gameInfoPacketSendCounter = 10;
                        }
                        else
                            gameInfoPacketSendCounter--;
                    }
                    else
                    {
                        // game has started so we dont have to send game info so often
                        if (gameInfoPacketSendCounter == 0)
                        {
                            UDPPacketBuffer buff = new UDPPacketBuffer();
                            gameInfoPacket.TimeStamp = DateTime.Now;
                            gameInfoPacket.PacketId.Value = packetIdCounter.Next();

                            buff.Data = gameInfoPacket.ToMinimalByte();
                            buff.DataLength = gameInfoPacket.ByteCount;

                            for (int i = 0; i < cliendAdressList.Count; i++)
                            {
                                Console.WriteLine("Sending gameInfo to : {0}", cliendAdressList[i]);
                                buff.RemoteEndPoint = cliendAdressList[i];
                                base.AsyncBeginSend(buff);
                            }
                            if (clientForServer != null && !clientForServer.GameIsRunningAsDedicatedServer)
                            clientForServer.GetMessageFromServer(gameInfoPacket);
                            if (cliendAdressList.Count > 0)
                            {
                                if (lastPackages.ContainsKey(last10.Counter))
                                    lastPackages[last10.Counter] = gameInfoPacket;
                                else
                                    lastPackages.Add(last10.Counter, gameInfoPacket);
                                last10.Increase();
                            }
                            // we need to send GameInfoPaket every 30 ticks - 3 seconds
                            gameInfoPacketSendCounter = 30;
                        }
                        else
                            gameInfoPacketSendCounter--;
                    }
                }
                catch (Exception ex)
                { Diagnostic.NetworkingDiagnostics.Logging.Fatal("Server Timer Error !!! ", ex); }
            }

            if(gameHasStarted)
            lock (serverPacketLock)
            {
                try
                {
                UDPPacketBuffer buff = new UDPPacketBuffer();
                serverPacket.TimeStamp = DateTime.Now;
                serverPacket.PacketId.Value = packetIdCounter.Next();
                buff.Data = serverPacket.ToMinimalByte();
                buff.DataLength = serverPacket.ToMinimalByte().Length;

                for (int i = 0; i < cliendAdressList.Count; i++)
                {
                    Console.WriteLine("Sending serverPacket to : {0}", cliendAdressList[i]);
                    buff.RemoteEndPoint = cliendAdressList[i];
                    base.AsyncBeginSend(buff);
                }
                if (clientForServer != null && !clientForServer.GameIsRunningAsDedicatedServer)
                clientForServer.GetMessageFromServer(serverPacket);
                    //clear client events ??
                for (int k = 0; k < serverPacket.PlayerInfoList.Count; k++)
                {
                    serverPacket.PlayerInfoList[k].PlayerJumping = false;
                    serverPacket.PlayerInfoList[k].PlayerShooting = false;
                }

                if (cliendAdressList.Count > 0)
                {
                    if (lastPackages.ContainsKey(last10.Counter))
                        lastPackages[last10.Counter] = serverPacket;
                    else
                        lastPackages.Add(last10.Counter, serverPacket);
                    last10.Increase();
                }
                }
                catch (Exception ex)
                { Diagnostic.NetworkingDiagnostics.Logging.Fatal("Server Timer Error !!! ", ex); }
            }
        }

        private void myUdpServer_MessageWasReceivedEvent(object sender, EventArgs e)
        {
            UDPPacketBuffer buff = sender as UDPPacketBuffer;

            if (buff != null)
            {
                try
                {
                    Interfaces.IPacket packet = PacketTypeChecker.GetPacket(buff.Data);
                    if (packet != null)
                    {
                        switch (packet.PacketType)
                        {
                            case PacketTypeEnumeration.PingResponse :
                                DateTime received = DateTime.Now;
                                TimeSpan span = received.Subtract(packet.TimeStamp);
                                Packets.PingResponsePacket pingResponsePacket = (Packets.PingResponsePacket)packet;
                                lock (gameInfoPacketLock)
                                {
                                    for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                                    {
                                        if (gameInfoPacket.PlayerStatusList[i].PlayerId == pingResponsePacket.PlayerId)
                                        {
                                            gameInfoPacket.PlayerStatusList[i].PlayerPing = (ushort)span.TotalMilliseconds;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case (PacketTypeEnumeration.JoinPacket):
                                JoinPacket joinPacket = (JoinPacket)packet;
                                ushort id = 0;
                                lock (gameInfoPacketLock)
                                {
                                    // quit if this is join for other game
                                    if (gameInfoPacket.GameId != joinPacket.GameId)
                                        return;

                                    bool newPlayer = true;
                                    // id smaller then 1000 means that new player without id
                                    if (joinPacket.PlayerId < 1000)
                                        for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                                        {
                                            if (gameInfoPacket.PlayerStatusList[i].PlayerId == joinPacket.PlayerId)
                                            {
                                                // we already have this player
                                                newPlayer = false;
                                                // we update playerName
                                                gameInfoPacket.PlayerStatusList[i].PlayerName = joinPacket.PlayerName;
                                                // change team request
                                                gameInfoPacket.PlayerStatusList[i].PlayerTeam = joinPacket.TeamId;
                                                break;
                                            }
                                        }
                                    // we need to add new player to gameInformation
                                    if (newPlayer)
                                    {
                                        PlayerStatus ps = new PlayerStatus();
                                        Random rand = new Random(joinPacket.PlayerName.GetHashCode());
                                        id = (ushort)rand.Next(1000, 9999);

                                        bool newPlayerOk = true;
                                        List<ushort> idList = new List<ushort>();

                                        for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                                        {
                                            idList.Add(gameInfoPacket.PlayerStatusList[i].PlayerId);
                                            if (gameInfoPacket.PlayerStatusList[i].PlayerName == joinPacket.PlayerName)
                                                newPlayerOk = false;
                                        }
                                        if (!newPlayerOk)
                                            return;

                                        // ensure that id is uniqe
                                        while (idList.Contains(id))
                                        {
                                            id = (ushort)rand.Next(1000, 9999);
                                        }
                                        ps.PlayerId = id;
                                        ps.PlayerName = joinPacket.PlayerName;
                                        ps.PlayerTeam = joinPacket.TeamId;
                                        gameInfoPacket.PlayerStatusList.Add(ps);
                                    }
                                }
                                lock (serverPacketLock)
                                {
                                    if (!cliendAdressList.Contains(buff.RemoteEndPoint))
                                    {
                                        cliendAdressList.Add(buff.RemoteEndPoint);

                                        PlayerInfo playerInfo = new PlayerInfo();
                                        playerInfo.PlayerId = id;
                                        serverPacket.PlayerInfoList.Add(playerInfo);
                                    }
                                }
                                break;
                            case (PacketTypeEnumeration.StandardClientPacket):
                                ClientPacket clientPacket = (ClientPacket)packet;
                                bool packetOk = false;
                                lock (clientPackagesDictionaryLock)
                                {
                                    if (!clientPackagesDictionary.ContainsKey(clientPacket.PlayerId))
                                    {
                                        clientPackagesDictionary.Add(clientPacket.PlayerId, new UDPClientServerCommons.Usefull.Last10Packages());
                                        clientPackagesDictionary[clientPacket.PlayerId].AddPacket(clientPacket);
                                    }
                                    else
                                        if (clientPackagesDictionary[clientPacket.PlayerId].LastPacket != null &&
                                            clientPacket.PacketId.Value > clientPackagesDictionary[clientPacket.PlayerId].LastPacket.PacketId.Value)
                                        {
                                            clientPackagesDictionary[clientPacket.PlayerId].AddPacket(clientPacket);
                                            packetOk = true;
                                        }
                                        else
                                        {
                                            Diagnostic.NetworkingDiagnostics.Logging.Error("Old ClientPacket was received");
                                        }
                                }
                                if(packetOk)
                                lock (serverPacketLock)
                                {
                                    for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                                    {
                                        if (clientPacket.PlayerId == serverPacket.PlayerInfoList[i].PlayerId)
                                        {
                                            if (clientPacket.PlayerJumping || clientPacket.PlayerShooting)
                                                Diagnostic.NetworkingDiagnostics.Logging.Warn(" Packet with events received " + clientPacket.ToString());
                                            // here we need to validate what Client send us !!!!!!
                                            // todo: validation
                                            serverPacket.PlayerInfoList[i] = UDPClientServerCommons.Translator.TranslateBetweenClientPacketAndPlayerInfo(clientPacket);
                                        }
                                    }
                                }
                                break;
                            case (PacketTypeEnumeration.QuitPacket):
                                LeaveGamePacket leaveGamePacket = (LeaveGamePacket)packet;
                                lock (gameInfoPacketLock)
                                {
                                    // do nothing if its not this game
                                    if (gameInfoPacket.GameId != leaveGamePacket.GameId)
                                        return;

                                    for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                                    {
                                        if (leaveGamePacket.PlayerId == gameInfoPacket.PlayerStatusList[i].PlayerId)
                                        {
                                            gameInfoPacket.PlayerStatusList.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                                lock (serverPacketLock)
                                {
                                    cliendAdressList.Remove(buff.RemoteEndPoint);

                                    for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                                    {
                                        if (serverPacket.PlayerInfoList[i].PlayerId == leaveGamePacket.PlayerId)
                                        {
                                            serverPacket.PlayerInfoList.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Fatal("myUdpServer_MessageWasReceivedEvent", ex);
                }
            }
        }

        /// <summary>
        /// Ip of the server, null if error
        /// </summary>
        public IPAddress MyIp
        {
            get
            {
                try
                {
                    IPAddress result = null;
                    string myHost = System.Net.Dns.GetHostName();
                    System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);
                    for (int i = 0; i < myIPs.AddressList.Length; i++)
                        //just LAN
                        if (myIPs.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            result = myIPs.AddressList[i];
                            break;
                        }
                    return result;
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("ip get error", ex);
                }
                return null;
            }
        }

        /// <summary>
        /// Close and cleans up the server
        /// </summary>
        public override void Dispose()
        {
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            base.Dispose();
            if (lanBroadcast != null)
                lanBroadcast.Dispose();
        }

        #region IServer Members

        /// <summary>
        /// starts new round , after a while it clears player scores (game engine should wait for 500 ms)
        /// and save plyaer scores if needed
        /// </summary>
        public void NewRound()
        {
            lock (gameInfoPacketLock)
            {
                gameInfoPacket.RoundNumber += 1;
            }

            System.Threading.Thread.Sleep(500);
            lock (gameInfoPacketLock)
            {
                for (int i = 0; i<gameInfoPacket.PlayerStatusList.Count; i++)
                {
                    gameInfoPacket.PlayerStatusList[i].PlayerScore = 0;
                    gameInfoPacket.PlayerStatusList[i].PlayerHealth = 100;
                }
            }
        }

        /// <summary>
        /// ends game
        /// </summary>
        public void EndGame()
        {
            if (Client!= null && !Client.GameIsRunningAsDedicatedServer)
            {
                Client.LeaveGame();
            }
            else
            {
                clientForServer_EndGameEvent(null, new EventArgs());
            }
        }

        public ServerPacket NeewestServerData
        {
            get {
                lock (serverPacketLock)
                {
                    return (ServerPacket) serverPacket.Clone();
                }
            }
        }

        public void UpdatePlayerHealth(List<UDPClientServerCommons.Usefull.PlayerHealthData> playerHealthList)
        {
            try
            {
                Dictionary<ushort, ushort> health = new Dictionary<ushort, ushort>();
                for (int i = 0; i < playerHealthList.Count; i++)
                {
                    health.Add(playerHealthList[i].PlayerId, playerHealthList[i].PlayerHealth);
                }

                lock (serverPacketLock)
                {
                    for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                    {
                        serverPacket.PlayerInfoList[i].Health = health[serverPacket.PlayerInfoList[i].PlayerId];
                    }
                }
                lock (gameInfoPacketLock)
                {
                    for (int i = 0; i < gameInfoPacket.PlayerStatusList.Count; i++)
                    {
                        gameInfoPacket.PlayerStatusList[i].PlayerHealth = health[gameInfoPacket.PlayerStatusList[i].PlayerId];
                    }
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Fatal("UpdatePlayerHealth", ex);
            }
        }

        #endregion
    }
}

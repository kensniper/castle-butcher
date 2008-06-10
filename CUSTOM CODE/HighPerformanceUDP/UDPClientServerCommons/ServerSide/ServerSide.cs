using System;
using System.Collections.Generic;
using System.Text;
using Clutch.Net.UDP;
using System.Net;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons
{
    public class ServerSide:MyUdpServer,IDisposable
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
        private AckOperating ackOperating;

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

        #endregion

        #region Constructor

        /// <summary> Starts new LAN Game
        /// 
        /// </summary>
        /// <param name="gameOptions">options for new game</param>
        /// <returns></returns>
        public IPEndPoint StartLANServer(GameOptions gameOptions)
        {
            lanBroadcast = new ServerLanBroadcast();

            lock (gameInfoPacketLock)
            {
                Random rand = new Random(gameOptions.GetHashCode());
                gameInfoPacket.GameId = (ushort)rand.Next(1000, 9999);
                gameInfoPacket.GameType = gameOptions.GameType;
                gameInfoPacket.Limit = gameOptions.GameLimit;
                gameInfoPacket.PlayerStatusList = new List<PlayerStatus>();
                gameInfoPacket.TeamScoreList = new List<TeamData>();
                TeamData ts1 = new TeamData(13, gameOptions.TeamOneName);
                TeamData ts2 = new TeamData(39, gameOptions.TeamTwoName);
                gameInfoPacket.ServerAddress = base.ServerIpAdress;
                gameInfoPacket.TeamScoreList.Add(ts1);
                gameInfoPacket.TeamScoreList.Add(ts2);
                gameInfoPacketSendCounter = 0;
            }

            timer.Change(0, _TimerTickPeriod);
            return base.Start();
        }

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
                }
            }
            return ret;
        }

        /// <summary>
        /// Method not implemented !!!
        /// </summary>
        /// <param name="gameOptions"></param>
        /// <returns></returns>
        public IPEndPoint StartINTERNETServer(GameOptions gameOptions)
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
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, 100);
            ackOperating = new AckOperating();

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
            timer = new System.Threading.Timer(timerCallback, null, System.Threading.Timeout.Infinite, 100);
            ackOperating = new AckOperating();

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();

            //counter
            packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter();
        }

        #endregion

        private void timerCallbackMethod(object target)
        {
            bool gameHasStarted = false;
            lock (gameInfoPacketLock)
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
                            buff.RemoteEndPoint = cliendAdressList[i];
                            base.AsyncBeginSend(buff);
                        }
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
                            buff.RemoteEndPoint = cliendAdressList[i];
                            base.AsyncBeginSend(buff);
                        }

                        if (cliendAdressList.Count > 0)
                        {
                            if (lastPackages.ContainsKey(last10.Counter))
                                lastPackages[last10.Counter] = gameInfoPacket;
                            else
                                lastPackages.Add(last10.Counter, gameInfoPacket);
                            last10.Increase();
                        }
                        // we need to send GameInfoPaket every 30 ticks - 30 seconds
                        gameInfoPacketSendCounter = 30;
                    }
                    else
                        gameInfoPacketSendCounter--;
                }
            }

            if(gameHasStarted)
            lock (serverPacketLock)
            {
                UDPPacketBuffer buff = new UDPPacketBuffer();
                serverPacket.TimeStamp = DateTime.Now;
                serverPacket.PacketId.Value = packetIdCounter.Next();
                buff.Data = serverPacket.ToMinimalByte();
                buff.DataLength = serverPacket.ToMinimalByte().Length;

                for (int i = 0; i < cliendAdressList.Count; i++)
                {
                    buff.RemoteEndPoint = cliendAdressList[i];
                    base.AsyncBeginSend(buff);
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
                                lock (serverPacketLock)
                                {
                                    ClientPacket clientPacket = (ClientPacket)packet;

                                    for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                                    {
                                        if (clientPacket.PlayerId == serverPacket.PlayerInfoList[i].PlayerId)
                                        {
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

        public IPAddress MyIp
        {
            get
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
        }

        public override void Dispose()
        {
            base.Dispose();
            lanBroadcast.Dispose();
        }
    }
}

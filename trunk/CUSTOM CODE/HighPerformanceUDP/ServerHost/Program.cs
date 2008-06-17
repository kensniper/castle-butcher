using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using System.Net;
using UDPClientServerCommons.Server;

namespace ServerHost
{
    class Program
    {
        private static ushort teamScore = 1;
        private static System.Threading.Timer timer = null;
        private static ServerSide CS_Server = null;
        static void Main(string[] args)
        {
            //Console.WriteLine("How do you want to start Server?");
            // Console.WriteLine("[1]\t by passing port number (ex. 1234)");
            // Console.WriteLine("[2]\t by passing Ip Adress and port number (ex. 127.0.0.1:1234)");
            // Console.WriteLine("[3]\t Default");
            // string input = Console.ReadLine();
            try
            {
                timer = new System.Threading.Timer(new System.Threading.TimerCallback(timerCallback), null, 1000, 1000);
                CS_Server = new ServerSide(1234);
                CS_Server.MessageWasReceivedEvent += new EventHandler(CS_Server_MessageWasReceivedEvent);
                GameOptions gameOptions = new GameOptions("Poland", "Niemcy", UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit, 10);
                //IPEndPoint adress = CS_Server.StartLANServer(gameOptions,true,null);

                IPEndPoint adress = CS_Server.StartLANServer(gameOptions, false, new UDPClientServerCommons.Usefull.PlayerMe("Poland", "ServerPlayer"));

                Console.WriteLine("Server started at adress : {0}", adress.ToString());
                Console.WriteLine("Press [ENTER] to termiante...");
                Console.Title = "Castle Strike Server - Press [ENTER] to termiante...";
                while (true)
                {
                    string input = Console.ReadLine();
                    if (input.ToLower() == "start")
                        Console.WriteLine(" Trying to start game .... Started? {0}", CS_Server.StartGame());
                    if (input.ToLower() == "info")
                    {
                        if (CS_Server.Client.CurrentGameInfo != null)
                            Console.WriteLine(CS_Server.Client.CurrentGameInfo.ToString());
                    }
                    if (input.ToLower() == "pl")
                    {
                        List<UDPClientServerCommons.Interfaces.IOtherPlayerData> list = new List<UDPClientServerCommons.Interfaces.IOtherPlayerData>(CS_Server.Client.PlayerDataList);
                        for (int i = 0; i < list.Count; i++)
                            Console.WriteLine("\t {0}", list[i]);
                    }
                    if (input.ToLower() == "jump")
                    {
                        UDPClientServerCommons.Interfaces.IPlayerDataWrite pd = new UDPClientServerCommons.Usefull.PlayerData();
                        pd.Jump = true;
                        pd.Weapon = UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow;

                        CS_Server.Client.UpdatePlayerData(pd);
                    }
                    if (input.ToLower() == "shoot")
                    {
                        UDPClientServerCommons.Interfaces.IPlayerDataWrite pd = new UDPClientServerCommons.Usefull.PlayerData();
                        pd.Shoot = true;
                        pd.Weapon = UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow;

                        CS_Server.Client.UpdatePlayerData(pd);
                    }
                    if (input.ToLower() == "team")
                    {
                        List<UDPClientServerCommons.Usefull.TeamData> tdList = new List<UDPClientServerCommons.Usefull.TeamData>();
                        tdList.Add(new UDPClientServerCommons.Usefull.TeamData(13,null));
                        tdList[0].TeamScore= teamScore;
                        teamScore++;
                        CS_Server.UpdatePlayerHealthAndTeamScore(null, tdList);
                    }
                    if (input.Length == 0)
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (CS_Server != null)
                CS_Server.Dispose();
            }
        }

        private static Int64 packetNumber = 0;

        private static void timerCallback(object obj)
        {
            try
            {
                List<UDPClientServerCommons.Interfaces.IGameEvent> gevents = new List<UDPClientServerCommons.Interfaces.IGameEvent>();
                gevents = CS_Server.Client.GameEventList;
                List<UDPClientServerCommons.Interfaces.IGameplayEvent> gpevents = new List<UDPClientServerCommons.Interfaces.IGameplayEvent>();
                gpevents = CS_Server.Client.GameplayEventList;

                if (gevents != null)
                {
                    for (int i = 0; i < gevents.Count; i++)
                    {
                        if (gevents[i].GameEventType == UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerJoined)
                        {
                            UDPClientServerCommons.GameEvents.PlayerJoinedEvent pje = (UDPClientServerCommons.GameEvents.PlayerJoinedEvent)gevents[i];
                            Console.WriteLine(pje.ToString());
                        }
                        else
                        Console.WriteLine("\tGE : {0}", gevents[i].ToString());
                    }
                }
                if (gpevents != null)
                {
                    for (int i = 0; i < gpevents.Count; i++)
                        Console.Write("\t\tGP : {0}", gpevents[i].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void CS_Server_MessageWasReceivedEvent(object sender, EventArgs e)
        {
            try
            {
                packetNumber=(packetNumber+1)%Int64.MaxValue;
                //Console.WriteLine("Received packet {0}", packetNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

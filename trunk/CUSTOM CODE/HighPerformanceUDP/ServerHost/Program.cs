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

        static void CS_Server_MessageWasReceivedEvent(object sender, EventArgs e)
        {
            try
            {
                packetNumber=(packetNumber+1)%Int64.MaxValue;
                Console.WriteLine("Received packet {0}", packetNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

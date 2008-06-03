using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using System.Net;

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
                IPEndPoint adress = CS_Server.StartServer();

                Console.WriteLine("Server started at adress : {0}", adress.ToString());
                Console.WriteLine("Press [ENTER] to termiante...");
                Console.Title = "Castle Strike Server - Press [ENTER] to termiante...";
                while (true)
                {
                    string input = Console.ReadLine();
                    //if (input.Length > 0)
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
            catch (Exception)
            {
                //ignore :)
            }
        }
    }
}

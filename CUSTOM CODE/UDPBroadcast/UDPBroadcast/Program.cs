using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace UDPBroadcast
{
    class Program
    {
        private static UdpClient udp;
        private static IPEndPoint groupEP;

        static void Main(string[] args)
        {
            try
            {
                int GroupPort = 15000;

                groupEP = new IPEndPoint(IPAddress.Broadcast, GroupPort);

                udp = new UdpClient(groupEP);

                while (true)
                {
                    string msg = Console.ReadLine();
                    if (msg != "")
                        Send(msg);
                    else
                        break;
                }

                udp.Close();
       
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                Console.ReadLine();                   
            }
        }

        private static void Send(string msg)
        {
            Console.WriteLine("Sending ... " + msg);

            byte[] sendBytes4 = Encoding.ASCII.GetBytes(msg);

            udp.Send(sendBytes4, sendBytes4.Length, groupEP);
            //udp.Send(sendBytes4, sendBytes4.Length);
        }
    }
}

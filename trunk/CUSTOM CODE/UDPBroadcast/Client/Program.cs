using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int GroupPort = 15000;
                IPEndPoint groupEP = null;
                if (MessageBox.Show("Do you want to use .Any address?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    groupEP = new IPEndPoint(IPAddress.Any, GroupPort);
                else
                {
                    groupEP = new IPEndPoint(MyIp, GroupPort);
                }

                UdpClient udp = new UdpClient(groupEP);

                udp.EnableBroadcast = true;


                while (true)
                {
                    byte[] receiveBytes = udp.Receive(ref groupEP);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine(returnData);

                    if (returnData.ToLower() == "q")
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

        public static IPAddress MyIp
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
    }
}

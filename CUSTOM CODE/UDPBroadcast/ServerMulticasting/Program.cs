using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerMulticasting
{
    public static class StockPriceMulticaster
    {
        static string[] symbols = { "ABCD", "EFGH", "IJKL", "MNOP" };
        public static void Start()
        {
            UdpClient publisher = new UdpClient("230.0.0.1", 8899);
            Console.WriteLine("Publishing stock prices to 230.0.0.1:8899");
            Random gen = new Random();
            while (true)
            {
                int i = gen.Next(0, symbols.Length);
                double price = 400 * gen.NextDouble() + 100;
                string msg = String.Format("{0} {1:#.00}", symbols[i], price);
                byte[] sdata = Encoding.ASCII.GetBytes(msg);
                publisher.Send(sdata, sdata.Length);
                System.Threading.Thread.Sleep(5000);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StockPriceMulticaster.Start();
        }
    }
}




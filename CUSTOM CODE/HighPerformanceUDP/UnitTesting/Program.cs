using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using System.Collections;

namespace UnitTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector vector = new Vector(34.35f, 12.89f, 31.98f);
            Console.WriteLine(vector);
            byte[] byteVector = vector.ToByte();
            Console.WriteLine("            byteVector.Length = {0}", byteVector.Length);

            Vector newVector = new Vector(byteVector, 0);

            Console.WriteLine(newVector);

            ushort sampleUshort = 25;
            byteVector = BitConverter.GetBytes(sampleUshort);

            Console.WriteLine("length od ushort is {0}, and byte data {1}", byteVector.Length, byteVector);

            bool sampleBool = true;

            BitArray bitArray = new BitArray(8, false);
            bitArray.CopyTo(byteVector, 0);

            byteVector = BitConverter.GetBytes(sampleBool);

            byte tmp = byteVector[0];

            bool tmp2 = BitConverter.ToBoolean(byteVector, 0);

            Console.WriteLine("length od bool is {0}, and bool value is {1}", byteVector.Length, sampleBool);

            DateTime now = DateTime.Now;

            byteVector = BitConverter.GetBytes(now.ToBinary());

            Console.WriteLine("length of datetime is {0}, and its value {1}",byteVector.Length,now);

            Console.ReadLine();

           // ClientPacket testClientPacket = new ClientPacket();

            PlayerInfo testClientPacket = new PlayerInfo();

            //testClientPacket.PacketId = 10;
            testClientPacket.PlayerCarringWeponOne = true;
            testClientPacket.PlayerCarringWeponTwo = false;
            testClientPacket.PlayerDucking = true;
            testClientPacket.PlayerId = 67;
            testClientPacket.PlayerJumping = true;
            testClientPacket.PlayerLookingDirection = new Vector(45.56f, 34.67f, 87.45f);
            testClientPacket.PlayerMovementDirection = new Vector(43.54f, 87.34f, 56.21f);
            testClientPacket.PlayerPosition = new Vector(1f, 5f, 67.65f);
            testClientPacket.PlayerRunning = true;
            testClientPacket.PlayerShooting = true;
            testClientPacket.PlayerWalking = false;
            testClientPacket.Timestamp = DateTime.Now;
            testClientPacket.DamageTaken = 45;
            testClientPacket.AckIds.Add(2);
            testClientPacket.AckIds.Add(67);


            PlayerInfo testClientPacket2 = new PlayerInfo();

            //testClientPacket.PacketId = 10;
            testClientPacket2.PlayerCarringWeponOne = false;
            testClientPacket2.PlayerCarringWeponTwo = true;
            testClientPacket2.PlayerDucking = true;
            testClientPacket2.PlayerId = 647;
            testClientPacket2.PlayerJumping = true;
            testClientPacket2.PlayerLookingDirection = new Vector(45.4f, 334.617f, 187.425f);
            testClientPacket2.PlayerMovementDirection = new Vector(473.547f, 837.343f, 546.231f);
            testClientPacket2.PlayerPosition = new Vector(17f, 57f, 677.675f);
            testClientPacket2.PlayerRunning = true;
            testClientPacket2.PlayerShooting = true;
            testClientPacket2.PlayerWalking = false;
            testClientPacket2.Timestamp = DateTime.Now;
            testClientPacket2.DamageTaken = 100;
            testClientPacket2.AckIds.Add(412);
            testClientPacket2.AckIds.Add(456);

            byte[] whatever = testClientPacket.ToByte();

            Console.WriteLine("Client packet length is {0}", whatever.Length);

            PlayerInfo retrived = new PlayerInfo(whatever);

            Console.WriteLine(retrived);

            Console.ReadLine();
            ServerPacket server = new ServerPacket();

            server.NumberOfPlayers = 2;
            server.PacketId = 346;
            server.PlayerInfoList.Add(testClientPacket);
            server.PlayerInfoList.Add(testClientPacket2);
            server.Timestamp = DateTime.Now;
            server.TypeOfPacket = PacketType.Standard;
            whatever = server.ToMinimalByte();

            Console.WriteLine(server);
            Console.WriteLine("\nServer length = {0}", whatever.Length);

            ServerPacket server2 = new ServerPacket(whatever);

            Console.WriteLine(server2);
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using System.Collections;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;
using System.Net;

namespace UnitTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPClientServerCommons.Usefull.PlayerData pd = new UDPClientServerCommons.Usefull.PlayerData();
            pd.Duck = true;
            bool tst = pd.Duck;
            byte[] binary = null;
           
            Vector vector = new Vector(34.35f, 12.89f, 31.98f);
            Console.WriteLine(vector);
            binary = vector.ToMinimalByte();

            Vector newVector = new Vector(binary,0);

            Console.WriteLine(newVector);
            Console.ReadLine();
            Console.WriteLine("========================================");
           // ClientPacket testClientPacket = new ClientPacket();

            ClientPacket testClientPacket = new ClientPacket();

            //testClientPacket.PacketId = 10;
            testClientPacket.PlayerCarringWeponOne = true;
            testClientPacket.PlayerCarringWeponTwo = false;
            testClientPacket.PlayerDucking = true;
            testClientPacket.PlayerId = 67;
            testClientPacket.PlayerJumping = true;
            testClientPacket.PlayerLookingDirection = new Vector(45.56f, 34.67f, 87.45f);
            testClientPacket.PlayerMovementDirection = new Vector(43.54f, 87.34f, 56.21f);
            testClientPacket.PlayerPosition = new Vector(1f, 5f, 67.65f);
            //testClientPacket.PlayerRunning = true;
            testClientPacket.PlayerShooting = true;
            //testClientPacket.PlayerWalking = false;
            testClientPacket.TimeStamp = DateTime.Now;
            testClientPacket.PacketId = 10;

            Console.WriteLine(testClientPacket);

            ClientPacket tmp = new ClientPacket(testClientPacket.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp);
            Console.WriteLine("========================================");
            Console.ReadLine();

            GameInfoPacket gameInfoPacket = new GameInfoPacket();
            gameInfoPacket.GameType = GameTypeEnumeration.Objective;
            gameInfoPacket.Limit = 10;
            gameInfoPacket.PacketId = 6889;
            gameInfoPacket.GameId = 10;
            gameInfoPacket.ServerAddress = new IPEndPoint(IPAddress.Parse("90.156.78.90"), 1234);
            PlayerStatus ps1 = new PlayerStatus();
            ps1.PlayerHealth = 100;
            ps1.PlayerId = 8;
            ps1.PlayerName = "Karpik";
            ps1.PlayerPing = 23;
            ps1.PlayerScore = 3;
            ps1.PlayerTeam = 1;
            PlayerStatus ps2 = new PlayerStatus();
            ps2.PlayerHealth = 78;
            ps2.PlayerId = 2;
            ps2.PlayerName = "Ziomek";
            ps2.PlayerPing = 34;
            ps2.PlayerScore = 7;
            ps2.PlayerTeam = 2;
            gameInfoPacket.PlayerStatusList.Add(ps1);
            gameInfoPacket.PlayerStatusList.Add(ps2);
            TeamData ts1 = new TeamData();
            ts1.TeamId = 1;
            ts1.TeamScore = 89;
            ts1.TeamName = "the crazy killers";
            TeamData ts2 = new TeamData();
            ts2.TeamId = 2;
            ts2.TeamScore = 45;
            ts2.TeamName = "what the fuckers";
            gameInfoPacket.TeamScoreList.Add(ts1);
            gameInfoPacket.TeamScoreList.Add(ts2);
            gameInfoPacket.TimeStamp = DateTime.Now;

            Console.WriteLine(gameInfoPacket);

            GameInfoPacket tmp2 = new GameInfoPacket(gameInfoPacket.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp2);
            Console.WriteLine("========================================");
            Console.ReadLine();

            JoinPacket joinPacket = new JoinPacket();
            joinPacket.GameId = 1;
            joinPacket.PacketId = 6543;
            joinPacket.PlayerId = 34;
            joinPacket.PlayerName = "Ziomek2";
            joinPacket.TimeStamp = DateTime.Now;
            joinPacket.TeamId = 67;

            Console.WriteLine(joinPacket);
            JoinPacket tmp3 = new JoinPacket(joinPacket.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp3);
            Console.WriteLine("========================================");
            Console.ReadLine();


            LeaveGamePacket leaveGamePacket = new LeaveGamePacket();
            leaveGamePacket.GameId = 56;
            leaveGamePacket.PacketId = 876;
            leaveGamePacket.PlayerId = 89;
            leaveGamePacket.TimeStamp = DateTime.Now;

            Console.WriteLine(leaveGamePacket);
            LeaveGamePacket tmp4 = new LeaveGamePacket(leaveGamePacket.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp4);
            Console.WriteLine("========================================");
            Console.ReadLine();

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
            //testClientPacket2.PlayerRunning = true;
            testClientPacket2.PlayerShooting = true;
            //testClientPacket2.PlayerWalking = false;
            
            testClientPacket2.Health = 100;
            testClientPacket2.AckIds.Add(412);
            testClientPacket2.AckIds.Add(456);
            testClientPacket2.Timestamp = DateTime.Now;
            Console.WriteLine(testClientPacket2);
            PlayerInfo tmp5 = new PlayerInfo(testClientPacket2.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp5);
            Console.WriteLine("========================================");
            Console.ReadLine();

            
            ServerPacket server = new ServerPacket();

            //server.NumberOfPlayers = 2;
            server.PacketId = 346;
            server.PlayerInfoList.Add(testClientPacket2);
            server.PlayerInfoList.Add(testClientPacket2);
            server.TimeStamp = DateTime.Now;

            Console.WriteLine(server);
            ServerPacket tmp6 = new ServerPacket(server.ToByte());
            Console.WriteLine("========================================");
            Console.WriteLine(tmp6);
            Console.WriteLine("========================================");
            Console.ReadLine();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using Clutch.Net.UDP;
using System.Net;

namespace UDPClientServerCommons
{
    public class ServerSide:MyUdpServer,IDisposable
    {
        private const int _TimerTickPeriod = 100;

        private ServerPacket serverPacket = new ServerPacket();
        private Dictionary<int, ServerPacket> LastPackages = new Dictionary<int, ServerPacket>();
        private Last10 last10 = new Last10();
        private List<EndPoint> cliendAdressList = new List<EndPoint>();
        private readonly object serverPacketLock = new object();
        private AckOperating ackOperating;

        private System.Threading.Timer timer;

        public IPEndPoint StartServer()
        {            
            timer.Change(0, _TimerTickPeriod);
            return base.Start();
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
        }

        private void timerCallbackMethod(object target)
        {
            lock (serverPacketLock)
            {
                UDPPacketBuffer buff = new UDPPacketBuffer();
                serverPacket.Timestamp = DateTime.Now;
                serverPacket.PacketId = (ushort)((serverPacket.PacketId + 1) % ushort.MaxValue);
                buff.Data = serverPacket.ToMinimalByte();
                buff.DataLength = serverPacket.ToMinimalByte().Length;

                UDPClientServerCommons.ServerPacket srv = new UDPClientServerCommons.ServerPacket(buff.Data);

                for (int i = 0; i < cliendAdressList.Count; i++)
                {
                    buff.RemoteEndPoint = cliendAdressList[i];
                    base.AsyncBeginSend(buff);
                }
                if (cliendAdressList.Count > 0)
                {
                    if (LastPackages.ContainsKey(last10.Counter))
                        LastPackages[last10.Counter] = serverPacket;
                    else
                        LastPackages.Add(last10.Counter, serverPacket);
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
                    UDPClientServerCommons.PacketType packetType = UDPClientServerCommons.PacketTypeChecker.Check(buff.Data);
                    switch (packetType)
                    {
                        case (UDPClientServerCommons.PacketType.JoinPacket):
                            UDPClientServerCommons.JoinPacket joinPacket = new UDPClientServerCommons.JoinPacket(buff.Data);
                            cliendAdressList.Add(buff.RemoteEndPoint);

                            SendAck(joinPacket.PacketId, joinPacket.PlayerId, buff.RemoteEndPoint);

                            lock (serverPacketLock)
                            {
                                UDPClientServerCommons.PlayerInfo playerInfo = new UDPClientServerCommons.PlayerInfo();
                                playerInfo.PlayerId = joinPacket.PlayerId;
                                serverPacket.PlayerInfoList.Add(playerInfo);
                                serverPacket.NumberOfPlayers = (ushort)cliendAdressList.Count;
                            }
                            break;
                        case (UDPClientServerCommons.PacketType.StandardPacket):
                            lock (serverPacketLock)
                            {
                                UDPClientServerCommons.ClientPacket clientPacket = new UDPClientServerCommons.ClientPacket(buff.Data);
                                if (clientPacket.AckRequired)
                                    SendAck(clientPacket, buff.RemoteEndPoint);

                                for (int i = 0; i < serverPacket.PlayerInfoList.Count; i++)
                                {
                                    if (clientPacket.PlayerId == serverPacket.PlayerInfoList[i].PlayerId)
                                    {
                                        serverPacket.PlayerInfoList[i] = UDPClientServerCommons.Translator.TranslateBetweenClientPacketAndPlayerInfo(clientPacket);
                                    }
                                }
                            }
                            break;
                        case (UDPClientServerCommons.PacketType.ACK):
                            // i just received an ack :)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UDPClientServerCommons.Diagnostic.NetworkingDiagnostics.Logging.Fatal("myUdpServer_MessageWasReceivedEvent", ex); 
                }
            }
        }

        private void SendAck(ushort packetId,ushort playerId,EndPoint adress)
        {
            AckPacket ackPacket = new AckPacket();
            ackPacket.PacketIdAck = packetId;
            ackPacket.PlayerId = playerId;

            UDPPacketBuffer buffer = new UDPPacketBuffer();
            buffer.RemoteEndPoint = adress;
            buffer.Data = ackPacket.ToMinimalByte();
            buffer.DataLength = ackPacket.ToMinimalByte().Length;
            base.AsyncBeginSend(buffer);
        }

        private void SendAck(ClientPacket packet, EndPoint adress)
        {
            AckPacket ackPacket = new AckPacket();
            ackPacket.PacketIdAck = packet.PacketId;
            ackPacket.PlayerId = packet.PlayerId;

            UDPPacketBuffer buffer = new UDPPacketBuffer();
            buffer.RemoteEndPoint = adress;
            buffer.Data = ackPacket.ToMinimalByte();
            buffer.DataLength = ackPacket.ToMinimalByte().Length;
            base.AsyncBeginSend(buffer);
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
    }
}

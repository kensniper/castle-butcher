using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons;
using System.Net;
using Clutch.Net.UDP;

namespace UDPClientServerCommons.Client
{
    public class ClientSide:IDisposable
    {
        public delegate void packetReceived(ServerPacket serverPacket);

        /// <summary>
        /// temporary event, fired after packet is received (game data updated)
        /// </summary>
        public event packetReceived PacketReceivedEvent;

        /// <summary>
        /// UDP networking layer
        /// </summary>
        private UDPLayer udpNetworking;
        /// <summary>
        /// Game world methods (physics)
        /// </summary>
        private GameWorld gameWorld;

        private AckOperating ackOperating;

        private ClientPacket clientPacket = new ClientPacket();
        private Dictionary<int, ServerPacket> LastPackages = new Dictionary<int, ServerPacket>();
        private Last10 last10 = new Last10();

        private readonly object ClientPackageLock = new object();
        private readonly object ServerPackagesLock = new object();

        public ClientSide()
        {
            udpNetworking = new UDPLayer();
            udpNetworking.GetData += new UDPLayer.GetDataToSend(udpNetworking_GetData);
            udpNetworking.PacketWasReceived += new UDPLayer.PacketReceived(udpNetworking_PacketWasReceived);

            //prepare first packet
            clientPacket = new ClientPacket();

            ackOperating = new AckOperating();
            ackOperating.SendPacketEvent += new AckOperating.SendPacket(ackOperating_SendPacketEvent);

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();
        }

        public ClientSide(int port)
        {
            udpNetworking = new UDPLayer(port);
            udpNetworking.GetData += new UDPLayer.GetDataToSend(udpNetworking_GetData);
            udpNetworking.PacketWasReceived += new UDPLayer.PacketReceived(udpNetworking_PacketWasReceived);

            //prepare first packet
            clientPacket = new ClientPacket();

            ackOperating = new AckOperating();
            ackOperating.SendPacketEvent += new AckOperating.SendPacket(ackOperating_SendPacketEvent);

            //diagnostics
            Diagnostic.NetworkingDiagnostics.Configure();
        }

        void ackOperating_SendPacketEvent(object Packet)
        {
            udpNetworking.SendPacket(((ClientPacket)Packet).ToMinimalByte());
        }

        public ushort JoinGame(IPEndPoint ServerIp, string PlayerName,ushort GameId)
        {
            udpNetworking.ServerIp = ServerIp;
            udpNetworking.StartUdpSocket();
            JoinPacket joinPacket = new JoinPacket();

            Random random = new Random();
            //CASTLE: asdlkasdlk
            //todo: game selection
            //todo: server should give id
            //temporary solution id selected by the user
            joinPacket.PlayerId = (ushort)random.Next(1000, 9999);
            joinPacket.PlayerName = PlayerName;
            joinPacket.GameId = GameId;

            udpNetworking.SendPacket(joinPacket.ToMinimalByte());

            return joinPacket.PlayerId;
        }

        private void udpNetworking_PacketWasReceived(Clutch.Net.UDP.UDPPacketBuffer udpPacketBuffer)
        {
            try
            {
                if (PacketTypeChecker.Check(udpPacketBuffer.Data) == PacketType.ACK)
                    ackOperating.AckReceived(new AckPacket(udpPacketBuffer.Data).PacketIdAck);
                else
                {
                    lock (ServerPackagesLock)
                    {
                        if (LastPackages.ContainsKey(last10.Counter))
                            LastPackages[last10.Counter] = new UDPClientServerCommons.ServerPacket(udpPacketBuffer.Data);
                        else
                            LastPackages.Add(last10.Counter, new UDPClientServerCommons.ServerPacket(udpPacketBuffer.Data));
                        last10.Increase();
                    }
                    PacketReceivedEvent(new UDPClientServerCommons.ServerPacket(udpPacketBuffer.Data));
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Packet was received", ex);
            }
        }

        public void ChangeDataToSend(ClientPacket clientPacket,bool needsAck)
        {
            lock (ClientPackageLock)
            {
                this.clientPacket = clientPacket;
                this.clientPacket.AckRequired = needsAck;
            }
        }

        public void ChangeDataToSend(Vector playerPosition,Vector playerMovementDirection)
        {
            lock (ClientPackageLock)
            {
                clientPacket.Timestamp = DateTime.Now;
                clientPacket.PlayerMovementDirection = playerMovementDirection;
                clientPacket.PlayerPosition = playerPosition;
            }
        }

        /// <summary>
        /// Method used by UDP network layer to get client network data
        /// </summary>
        /// <returns></returns>
        private ClientPacket udpNetworking_GetData()
        {
            lock (ClientPackageLock)
            {
                if (clientPacket.AckRequired == true)
                {
                    ackOperating.SendPacketNeededAck(clientPacket.Clone());
                    return null;
                }
                else
                    return (ClientPacket)clientPacket.Clone();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Closes gently this instance of GameServer
        /// </summary>
        public void Dispose()
        {
            udpNetworking.Dispose();
        }

        #endregion
    }
}

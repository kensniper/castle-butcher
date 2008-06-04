using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons;
using System.Net;
using Clutch.Net.UDP;
using UDPClientServerCommons.Interfaces;

namespace UDPClientServerCommons.Client
{
    public class ClientSide:IDisposable,ISend,IServerData
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

        private AckOperating ackOperating;

        private ClientPacket clientPacket = new ClientPacket();
        private Dictionary<int, ServerPacket> LastPackages = new Dictionary<int, ServerPacket>();
        private Last10 last10 = new Last10();

        private readonly object ClientPackageLock = new object();
        private readonly object ServerPackagesLock = new object();

        private ushort? PlayerId = null;

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

            PlayerId = joinPacket.PlayerId;
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
              
        #region IServerData Members

        public ServerPacket GetNeewestDataFromServer()
        {
            lock (ServerPackagesLock)
            {
                ServerPacket tmp = null;
                if (LastPackages.Count < 10 && LastPackages.Count > 0)
                    return LastPackages[LastPackages.Count - 1];
                else
                {
                    LastPackages.TryGetValue(last10.GetPrevoius(1), out tmp);
                    return tmp;
                }
            }
        }

        public ushort GetPlayerId()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISend Members

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection)
        {
            lock (ClientPackageLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }

        public void UpdatePlayerData(WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (ClientPackageLock)
            {
                clientPacket.PlayerShooting = playerAttacked;
                clientPacket.PlayerJumping = playerJumped;
                if (playerAttacked)
                    if (weaponAttacked == WeaponEnumeration.CrossBow)
                    {
                        clientPacket.PlayerCarringWeponOne = false;
                        clientPacket.PlayerCarringWeponTwo = true;
                    }
                    else
                    {
                        clientPacket.PlayerCarringWeponTwo = false;
                        clientPacket.PlayerCarringWeponOne = true;
                    }
                else
                    if (weaponChanged)
                    {
                        if (weaponNew == WeaponEnumeration.CrossBow)
                        {
                            clientPacket.PlayerCarringWeponOne = false;
                            clientPacket.PlayerCarringWeponTwo = true;
                        }
                        else
                        {
                            clientPacket.PlayerCarringWeponTwo = false;
                            clientPacket.PlayerCarringWeponOne = true;
                        }
                    }
            }
        }

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection, WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (ClientPackageLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }

        public void LeaveGame()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

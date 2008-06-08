using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons;
using System.Net;
using Clutch.Net.UDP;
using UDPClientServerCommons.Interfaces;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons.Client
{
    public class ClientSide:IDisposable,ISend,IServerData
    {
        public delegate void packetReceived(IPacket serverPacket);

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
        private Dictionary<int, Interfaces.IPacket> lastPackages = new Dictionary<int, Interfaces.IPacket>();
        private Last10 last10 = new Last10();

        private readonly object clientPacketLock = new object();
        private readonly object serverPacketLock = new object();

        private ushort? playerId = null;
        private ushort? gameId = null;

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

       private void ackOperating_SendPacketEvent(Interfaces.IPacket Packet)
        {
            udpNetworking.SendPacket(((Interfaces.ISerializablePacket)Packet).ToByte());
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

            udpNetworking.SendPacket(joinPacket.ToByte());

            playerId = joinPacket.PlayerId;
            return joinPacket.PlayerId;
        }

        private void udpNetworking_PacketWasReceived(Clutch.Net.UDP.UDPPacketBuffer udpPacketBuffer)
        {
            try
            {
                    IPacket packet = PacketTypeChecker.GetPacket(udpPacketBuffer.Data);
                    lock (serverPacketLock)
                    {                        
                        if (lastPackages.ContainsKey(last10.Counter))
                            lastPackages[last10.Counter] = packet;
                        else
                            lastPackages.Add(last10.Counter, packet);
                        last10.Increase();
                    }
                    PacketReceivedEvent(packet);

                //bool ackNeeded = false;
                //if (packet.PacketType == PacketTypeEnumeration.GameInfoPacket)
                //    ackNeeded = true;
                //if(packet.PacketId==ServerPacket && ! ackNeeded)
                //    for (int i = 0; i < ((ServerPacket)(packet)).PlayerInfoList.Count; i++)
                //        if (((ServerPacket)(packet)).PlayerInfoList[i].AckRequired)
                //        {
                //            ackNeeded = true;
                //            break;
                //        }

            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Packet was received", ex);
            }
        }

        public void UpdateDataToSend(ClientPacket clientPacket,bool needsAck)
        {
            lock (clientPacketLock)
            {
                this.clientPacket = clientPacket;
                this.clientPacket.AckRequired = needsAck;
            }
        }

        public void UpdateDataToSend(Vector playerPosition,Vector playerMovementDirection)
        {
            lock (clientPacketLock)
            {
                clientPacket.TimeStamp = DateTime.Now;
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
            lock (clientPacketLock)
            {
                if (clientPacket.AckRequired == true)
                {
                    ackOperating.SendPacketNeededAck(clientPacket);
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
            try
            {
                LeaveGame();
                System.Threading.Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("Error on Dispose", ex);
            }
            finally
            {
                udpNetworking.Dispose();
            }
        }

        #endregion
              
        #region IServerData Members

        public IPacket GetNewestDataFromServer()
        {
            lock (serverPacketLock)
            {
                Interfaces.IPacket tmp = null;
                if (lastPackages.Count < 10 && lastPackages.Count > 0)
                    return lastPackages[lastPackages.Count - 1];
                else
                {
                    lastPackages.TryGetValue(last10.GetPrevoius(1), out tmp);
                    return tmp;
                }
            }
        }

        public ushort GetPlayerId()
        {
            if (playerId.HasValue)
                return this.playerId.Value;
            else
                throw new Usefull.PlayerIdNullException();
        }

        #endregion

        #region ISend Members

        public void UpdatePlayerData(Microsoft.DirectX.Vector3 position, Microsoft.DirectX.Vector3 lookDirection, Microsoft.DirectX.Vector3 moventDirection)
        {
            lock (clientPacketLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }

        public void UpdatePlayerData(WeaponEnumeration weaponAttacked, bool playerAttacked, bool playerJumped, bool weaponChanged, WeaponEnumeration weaponNew)
        {
            lock (clientPacketLock)
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
            lock (clientPacketLock)
            {
                clientPacket.PlayerPosition = Translator.TranslateVector3toVector(position);
                clientPacket.PlayerLookingDirection = Translator.TranslateVector3toVector(lookDirection);
                clientPacket.PlayerMovementDirection = Translator.TranslateVector3toVector(moventDirection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LeaveGame()
        {
            if (!playerId.HasValue)
                throw new Usefull.PlayerIdNullException();

            LeaveGamePacket leaveGamePacket = new LeaveGamePacket();
            leaveGamePacket.GameId = gameId ?? 0;
            leaveGamePacket.PlayerId = playerId.Value;

            ackOperating.SendPacketNeededAck(leaveGamePacket);

            udpNetworking.StopSendingByTimer();
        }

        #endregion
    }
}

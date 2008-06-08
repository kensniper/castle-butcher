using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UDPClientServerCommons
{
    /// <summary>
    /// class used to send Acknowledge packages
    /// </summary>
    public class AckOperating
    {
        #region fields

        private const int _SENDING_DELAY = 5;

        private Usefull.PacketIdCounter packetIdCounter;

        private Dictionary<ushort, Interfaces.IPacket> packetsWaitingForAck;

        public delegate void SendPacket(Interfaces.IPacket Packet);

        private Timer AckTimer;

        public event SendPacket SendPacketEvent;

        private readonly object DictionaryLock = new object();

        #endregion

        public AckOperating()
        {
            packetIdCounter = new UDPClientServerCommons.Usefull.PacketIdCounter(1);
            packetsWaitingForAck = new Dictionary<ushort, Interfaces.IPacket>();
            TimerCallback callBack = new TimerCallback(TimerCallbackEvent);

            AckTimer = new Timer(callBack, null, System.Threading.Timeout.Infinite, 2*_SENDING_DELAY);
        }

        private void TimerCallbackEvent(object args)
        {
            lock (DictionaryLock)
            {
                foreach (Interfaces.IPacket o in packetsWaitingForAck.Values)
                {
                    SendPacketEvent(o);
                    Thread.Sleep(_SENDING_DELAY);
                }
                if (packetsWaitingForAck.Count > 10)
                    throw new ToManyAcksWaitingException(10);
            }
          }

        public void SendPacketNeededAck(Interfaces.IPacket Packet)
        {
            lock (DictionaryLock)
            {
                packetIdCounter.Next();
                Packet.PacketId = new UDPClientServerCommons.Usefull.PacketIdCounter(packetIdCounter.Value);
                packetsWaitingForAck.Add(packetIdCounter.Value, Packet);
            }
            if (packetsWaitingForAck.Count == 0)
            {
                //theres no packages that needed ack
                //so the timer has stoped
                //it needs to be started again when needed
                AckTimer.Change(0, 10);
            }
            else
                switch (packetsWaitingForAck.Count)
                {
                    case (1):
                        AckTimer.Change(0, 2*_SENDING_DELAY);
                        break;
                    default:
                        AckTimer.Change(0, _SENDING_DELAY);
                        break;
                }
        }

        public void AckReceived(ushort packageId)
        {
            lock(DictionaryLock)
            {
                if (packetsWaitingForAck.ContainsKey(packageId))
                    packetsWaitingForAck.Remove(packageId);
                else
                    throw new BadPackageIdException(packageId);
            }
        }

        public void AckReceived(List<ushort> packageIdList)
        {
            lock (DictionaryLock)
            {
                for(int i=0;i<packageIdList.Count;i++)
                if (packetsWaitingForAck.ContainsKey(packageIdList[i]))
                    packetsWaitingForAck.Remove(packageIdList[i]);
                else
                    throw new BadPackageIdException(packageIdList[i]);
            }
        }
    }
}

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
        private Dictionary<ushort, object> packetsWaitingForAck;

        public delegate void SendPacket(object Packet);

        private Timer AckTimer;

        public event SendPacket SendPacketEvent;

        private readonly object DictionaryLock = new object();
      
        public AckOperating()
        {
            packetsWaitingForAck = new Dictionary<ushort, object>();
            TimerCallback callBack = new TimerCallback(TimerCallbackEvent);

            AckTimer = new Timer(callBack, null, System.Threading.Timeout.Infinite, 10);
        }

        private void TimerCallbackEvent(object args)
        {
            lock (DictionaryLock)
            {
                foreach (object o in packetsWaitingForAck.Values)
                {
                    SendPacketEvent(o);
                    Thread.Sleep(1);
                }
                if (packetsWaitingForAck.Count > 10)
                    throw new ToManyAcksWaitingException(10);
            }
          }

        public void SendPacketNeededAck(object Packet)
        {
            lock (DictionaryLock)
            {
                ClientPacket CPacket = Packet as ClientPacket;
                ServerPacket SPacket = Packet as ServerPacket;
                if(CPacket!=null)
                packetsWaitingForAck.Add(CPacket.PacketId, Packet);
                if(SPacket!=null)
                    packetsWaitingForAck.Add(SPacket.PacketId, Packet);
            }
            if (packetsWaitingForAck.Count == 0)
                //theres no packages that needed ack
                //so the timer has stoped
                //it needs to be started again
                AckTimer.Change(0, 10);
            else
                switch (packetsWaitingForAck.Count)
                {
                    case (1):
                        AckTimer.Change(0, 10);
                        break;
                    default:
                        AckTimer.Change(0, 5);
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
    }
}

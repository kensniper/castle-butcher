using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class Last10Packages
    {
        private Last10 last10;
        private Dictionary<int, Interfaces.IPacket> packets;
        private readonly object packetLock = new object();

        public Last10Packages()
        {
            last10 = new Last10();
            packets = new Dictionary<int, UDPClientServerCommons.Interfaces.IPacket>();
        }

        public Interfaces.IPacket GetPacket(int which)
        {
            if (which > 9 || which < 0)
                return null;

            lock (packetLock)
            {
                if (!packets.ContainsKey(which))
                    return null;
                return packets[which];
            }
        }

        public Interfaces.IPacket LastPacket
        {
            get {
                lock (packetLock)
                {
                    if (!packets.ContainsKey(last10.GetPrevoius(1)))
                        return null;
                    return (Interfaces.IPacket)packets[last10.GetPrevoius(1)].Clone();
                }
            }
        }

        public void AddPacket(Interfaces.IPacket packet)
        {
            lock (packetLock)
            {
                if (!packets.ContainsKey(last10.Counter))
                    packets.Add(last10.Counter, packet);
                else
                    packets[last10.Counter] = packet;

                    last10.Increase();
            }
        }
    }
}

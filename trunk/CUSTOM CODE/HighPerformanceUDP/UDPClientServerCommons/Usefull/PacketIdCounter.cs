using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class PacketIdCounter
    {
        private ushort counter = 0;

        public ushort Value
        {
            get { return counter; }
            set { counter = value; }
        }

        public PacketIdCounter()
        {
        }

        public PacketIdCounter(ushort val)
        {
            counter = val;
        }

        public PacketIdCounter(int val)
        {
            counter = (ushort)(val % ushort.MaxValue);
        }

        public static PacketIdCounter operator +(PacketIdCounter a, ushort b)
        {
            return new PacketIdCounter((ushort)((a.counter + b) % ushort.MaxValue));
        }

        public static implicit operator PacketIdCounter(int val)
        {
            return new PacketIdCounter(val);
        }

        public static implicit operator PacketIdCounter(ushort val)
        {
            return new PacketIdCounter(val);
        }

        public ushort Next()
        {
            counter = (ushort)((counter + 1) % ushort.MaxValue);
            return counter;
        }

        public bool isNext(PacketIdCounter newVal)
        {
            if (newVal.counter == 1)
                if (this.counter == ushort.MaxValue)
                    return true;
                else
                    return false;
            else
                return ((newVal.counter - this.counter) == 1);
        }

        public override string ToString()
        {
            return counter.ToString();
        }
    }
}

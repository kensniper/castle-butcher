using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class PacketIdCounter
    {
        private const ushort DELTA = 200;

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

        public static bool operator ==(PacketIdCounter a, PacketIdCounter b)
        {
            return (a.Value == b.Value);
        }

        public static bool operator !=(PacketIdCounter a, PacketIdCounter b)
        {
            return (a.Value != b.Value);
        }

        public static bool operator >(PacketIdCounter a, PacketIdCounter b)
        {
            int diff = Math.Abs(a.Value - b.Value);

            if (diff > (ushort)(ushort.MaxValue / 2))
            {
                if ((ushort.MaxValue - a.Value) < DELTA && b.Value < DELTA)
                    return false;
                if ((ushort.MaxValue - b.Value) < DELTA && a.Value < DELTA)
                    return true;
                throw new Usefull.IdCompareException("to big ids in compare");
            }
            else
                return (a.Value > b.Value);
        }

        public static bool operator <(PacketIdCounter a, PacketIdCounter b)
        {
            return !(a > b);
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

        public override int GetHashCode()
        {
            return counter.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PacketIdCounter p = obj as PacketIdCounter;
            if (p == null)
                return false;
            else
                return this.counter.Equals(p.Value);
        }
    }
}

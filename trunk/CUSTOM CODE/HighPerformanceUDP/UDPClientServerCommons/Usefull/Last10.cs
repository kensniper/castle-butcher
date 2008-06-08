using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public class Last10
    {
        private int counter = 0;

        public int Counter
        {
            get { return counter; }
        }

        public void Increase()
        {
            counter = (counter + 1) % 10;
        }

        public int GetPrevoius(int howManyBack)
        {
            int k = counter - howManyBack;
            if (k >= 0)
                return k;
            else
                return 10 + k;
        }

        public Last10()
        { }

        public Last10(int val)
        {
            this.counter = val % 10;
        }

        public static Last10 operator +(Last10 a, int b)
        {
            return new Last10((a.counter + b) % 10);
        }

        public static implicit operator Last10(int val)
        {
            return new Last10(val);
        }
    }
}

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

    }
}

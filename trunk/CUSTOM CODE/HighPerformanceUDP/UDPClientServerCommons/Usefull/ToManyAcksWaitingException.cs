using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public class ToManyAcksWaitingException:ApplicationException
    {
        public ToManyAcksWaitingException(int howMany)
            : base("Thera are to many acks in dictionary list!!, number of waiting acks : " + howMany.ToString())
        {
            this.NumberOfAckWaitingField = howMany;
        }

        private int NumberOfAckWaitingField;

        public int NumberOfAckWaiting
        {
            get { return NumberOfAckWaitingField; }
            set { NumberOfAckWaitingField = value; }
        }
    }
}

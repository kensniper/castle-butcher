using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class IdCompareException:ApplicationException
    {
        public IdCompareException(string message)
            : base(message)
        {
        }

        public IdCompareException()
            : base()
        { }
    }
}

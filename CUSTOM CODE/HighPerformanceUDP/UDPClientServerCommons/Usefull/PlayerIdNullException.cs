using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class PlayerIdNullException:ApplicationException
    {
        public PlayerIdNullException()
            : base("Player id is unknown !!!!!!!")
        {
            
        }
    }
}

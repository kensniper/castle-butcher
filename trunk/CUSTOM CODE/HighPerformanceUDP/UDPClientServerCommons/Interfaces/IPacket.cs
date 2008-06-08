using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Interfaces
{
    public interface IPacket
    {
        PacketTypeEnumeration PacketType
        {
            get;
        }

        PacketIdCounter PacketId
        {
            get;
            set;
        }

        DateTime TimeStamp
        {
            get;
            set;
        }
    }
}

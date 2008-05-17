using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    public abstract class PacketIdCommon
    {
        private ushort packetIdField;

        public ushort PacketId
        {
            get { return packetIdField; }
            set { packetIdField = value; }
        }
    }
}

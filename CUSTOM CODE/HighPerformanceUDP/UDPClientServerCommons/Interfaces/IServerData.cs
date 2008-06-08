using System;
using System.Collections.Generic;
using System.Text;
using UDPClientServerCommons.Packets;

namespace UDPClientServerCommons.Interfaces
{
    public interface IServerData
    {
        IPacket GetNewestDataFromServer();
        ushort GetPlayerId();
    }
}

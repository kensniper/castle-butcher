using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    public interface IServerData
    {
        ServerPacket GetNeewestDataFromServer();
        ushort GetPlayerId();
    }
}

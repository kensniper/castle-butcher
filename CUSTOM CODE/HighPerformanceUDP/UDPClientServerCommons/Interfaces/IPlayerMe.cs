using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    public interface IPlayerMe
    {
        string PlayerTeam
        {
            set;
        }

        string PlayerName
        {
            set;
        }
    }
}

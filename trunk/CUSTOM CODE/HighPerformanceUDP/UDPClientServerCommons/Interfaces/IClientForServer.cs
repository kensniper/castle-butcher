using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Interfaces
{
    public interface IClientForServer:IClientForServerBase
    {
        /// <summary>
        /// Tells if game is running as dedicated server
        /// </summary>
        bool GameIsRunningAsDedicatedServer
        {
            get;
        }
    }
}

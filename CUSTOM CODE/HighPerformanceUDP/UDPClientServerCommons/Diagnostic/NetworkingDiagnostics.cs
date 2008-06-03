using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace UDPClientServerCommons.Diagnostic
{
    public static class NetworkingDiagnostics
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkingDiagnostics));

        public static ILog Logging
        {
            get { return NetworkingDiagnostics.log; }
        }
        
        public static void Configure()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}

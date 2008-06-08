using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Packets
{
    public struct TeamScoreStruct
    {
        public ushort TeamId;
        public ushort TeamScore;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nTeamId = ");
            sb.Append(TeamId);
            sb.Append("\nTeamScore = ");
            sb.Append(TeamScore);

            return sb.ToString();
        }
    }
}

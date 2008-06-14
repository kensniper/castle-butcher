using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class PlayerMe:Interfaces.IPlayerMe
    {
        private string playerTeamField;
        private string playerNameField;

        #region IPlayerMe Members

        public string PlayerTeam
        {
            set { playerTeamField = value; }
            get { return playerTeamField; }
        }

        public string PlayerName
        {
            set { playerNameField = value; }
            get { return playerNameField; }
        }

        #endregion

        public PlayerMe(string playerTeam, string playerName)
        {
            this.playerNameField = playerName;
            this.playerTeamField = playerTeam;
        }
    }
}

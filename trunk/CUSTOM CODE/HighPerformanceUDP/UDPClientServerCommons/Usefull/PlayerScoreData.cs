using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    /// <summary>
    /// player id, player name, player score and player Team
    /// </summary>
    public class PlayerScoreData
    {
        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort playerScoreField;

        public ushort PlayerScore
        {
            get { return playerScoreField; }
            set { playerScoreField = value; }
        }

        private ushort playerTeamField;

        public ushort PlayerTeam
        {
            get { return playerTeamField; }
            set { playerTeamField = value; }
        }

        private string playerNameField;

        public string PlayerName
        {
            get { return playerNameField; }
            set { playerNameField = value; }
        }

        public PlayerScoreData()
        { }

        public PlayerScoreData(ushort playerId, string playerName, ushort playerTeam, ushort score)
        {
            this.playerNameField = playerName;
            this.playerScoreField = score;
            this.playerIdField = playerId;
            this.playerTeamField = playerTeam;
        }
    }
}

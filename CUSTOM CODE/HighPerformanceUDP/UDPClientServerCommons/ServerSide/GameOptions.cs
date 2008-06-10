using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace UDPClientServerCommons
{
    public class GameOptions
    {
        private string teamOneNameField;

        /// <summary>
        /// Name of the first team
        /// </summary>
        public string TeamOneName
        {
            set { teamOneNameField = value; }
            get { return teamOneNameField; }
        }
        private string teamTwoNameField;

        /// <summary>
        /// Name of the second team
        /// </summary>
        public string TeamTwoName
        {
            set { teamTwoNameField = value; }
            get { return teamTwoNameField; }
        }

        private Constants.GameTypeEnumeration gameTypeField;

        /// <summary>
        /// Type of the game
        /// </summary>
        public Constants.GameTypeEnumeration GameType
        {
            set { gameTypeField = value; }
            get { return gameTypeField; }
        }
        private ushort gameLimitField;

        /// <summary>
        /// Game limit - its time in minutes for GameType = TimeLimit,
        /// or frags (for the team) for GameType = FragLimit
        /// </summary>
        public ushort GameLimit
        {
            set { gameLimitField = value; }
            get { return gameLimitField; }
        }

        public GameOptions()
        {
            teamOneNameField = "TeamOne";
            teamTwoNameField = "TeamTwo";
            gameTypeField = UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit;
            gameLimitField= 10;
        }

        public GameOptions(string TeamOneName, string TeamTwoName, Constants.GameTypeEnumeration GameType, ushort GameLimit)
        {
            this.teamOneNameField = TeamOneName;
            this.teamTwoNameField = TeamTwoName;
            this.gameLimitField = GameLimit;
            this.gameTypeField = GameType;
        }

        public override int GetHashCode()
        {
            return teamTwoNameField.GetHashCode() + teamTwoNameField.GetHashCode() + gameLimitField.GetHashCode() + gameTypeField.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nTeamOne = ");
            sb.Append(teamOneNameField);
            sb.Append("\nTeamTwo = ");
            sb.Append(teamTwoNameField);
            sb.Append("\nGameType = ");
            sb.Append(gameTypeField);
            sb.Append("\nGameLimit = ");
            sb.Append(gameLimitField);

            return sb.ToString();
        }
    }
}

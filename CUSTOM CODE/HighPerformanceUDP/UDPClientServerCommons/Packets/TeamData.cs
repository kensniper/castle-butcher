using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.Usefull
{
    public class TeamData:ICloneable
    {
        private ushort teamIdField;

        public ushort TeamId
        {
            get { return teamIdField; }
            set { teamIdField = value; }
        }
        private ushort teamScoreField;

        public ushort TeamScore
        {
            get { return teamScoreField; }
            set { teamScoreField = value; }
        }
        private string teamNameField;

        public string TeamName
        {
            get { return teamNameField; }
            set { teamNameField = value; }
        }

        public TeamData(ushort id, string name)
        {
            teamIdField = id;
            teamNameField = name;
            teamScoreField = 0;
        }

        public TeamData()
        {  }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nTeamId = ");
            sb.Append(teamIdField);
            sb.Append("\nTeamScore = ");
            sb.Append(teamScoreField);
            sb.Append("\nTeamName = ");
            sb.Append(teamNameField);

            return sb.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
            TeamData cpy = new TeamData();
            cpy.teamIdField = this.teamIdField;
            cpy.teamNameField = (string)this.teamNameField.Clone();
            cpy.teamScoreField = this.teamScoreField;

            return cpy;
        }

        #endregion
    }
}

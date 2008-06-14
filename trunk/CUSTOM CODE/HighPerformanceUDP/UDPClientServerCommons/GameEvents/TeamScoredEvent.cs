using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class TeamScoredEvent:Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerScored; }
        }

        #endregion

        private ushort teamIdField;

        public ushort TeamId
        {
            get { return teamIdField; }
        }

        private string teamNameField;

        public string TeamName
        {
            get { return teamNameField; }
        }

        private ushort currentScoreField;

        public ushort CurrentScoreField
        {
            get { return currentScoreField; }
        }

        private ushort oldScoreField;

        public ushort OldScore
        {
            get { return oldScoreField; }
        }

        public TeamScoredEvent(ushort teamId, string teamName, ushort currentScore, ushort oldScore)
        {
            this.teamIdField = teamId;
            this.teamNameField = teamName;
            this.currentScoreField = currentScore;
            this.oldScoreField = oldScore;
        }
    }
}

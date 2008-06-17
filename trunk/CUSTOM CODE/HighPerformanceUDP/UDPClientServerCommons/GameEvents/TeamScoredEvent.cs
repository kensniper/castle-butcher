﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class TeamScoredEvent:GameEventBase, Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public override UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.TeamScored; }
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

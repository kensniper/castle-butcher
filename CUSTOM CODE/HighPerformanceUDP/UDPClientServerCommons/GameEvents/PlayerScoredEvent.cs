using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class PlayerScoredEvent:Interfaces.IGameEvent
    {
        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerScored; }
        }

        #endregion

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
        }
        private string playerNameField;

        public string PlayerName
        {
            get { return playerNameField; }
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

        public PlayerScoredEvent(ushort playerId, string playerName, ushort currentScore, ushort oldScore)
        {
            this.playerIdField = playerId;
            this.playerNameField = playerName;
            this.currentScoreField = currentScore;
            this.oldScoreField = oldScore;
        }
    }
}

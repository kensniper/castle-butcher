using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public class PlayerChangedTeamEvent:Interfaces.IGameEvent
    {

        #region IGameEvent Members

        public UDPClientServerCommons.Constants.GameEventTypeEnumeration GameEventType
        {
            get { return UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerChangedTeam; }
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

        private string newTeamNameField;

        public string NewTeamName
        {
            get { return newTeamNameField; }
        }

        private ushort newTeamIdField;

        public ushort NewTeamId
        {
            get { return newTeamIdField; }
            set { newTeamIdField = value; }
        }

        public PlayerChangedTeamEvent(ushort playerId, string playerName,ushort teamId,string teamName)
        {
            this.playerIdField = playerId;
            this.playerNameField = playerName;
            this.newTeamIdField = teamId;
            this.newTeamNameField = teamName;
        }
    }
}

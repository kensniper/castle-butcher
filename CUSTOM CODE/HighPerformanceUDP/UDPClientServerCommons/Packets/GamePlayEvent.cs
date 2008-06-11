using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class GamePlayEvent:ICloneable,Interfaces.IGameplayEvent
    {
        #region fields

        private Constants.GamePlayEventTypeEnumeration gameplayEventTypeField;

        public Constants.GamePlayEventTypeEnumeration GameplayEventType
        {
            get { return gameplayEventTypeField; }
            set { gameplayEventTypeField = value; }
        }

        private DateTime timestampField;

        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            GamePlayEvent cpy = new GamePlayEvent();
            cpy.playerIdField = this.playerIdField;
            cpy.timestampField = this.timestampField;
            cpy.gameplayEventTypeField = this.gameplayEventTypeField;

            return cpy;
        }

        #endregion

        #region Constructor

        public GamePlayEvent()
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nGameplayEventType = ");
            sb.Append(gameplayEventTypeField);
            sb.Append("\nTimestamp = ");
            sb.Append(timestampField);
            sb.Append("\nPlayerId = ");
            sb.Append(playerIdField);

            return sb.ToString();
        }

        #endregion
    }
}

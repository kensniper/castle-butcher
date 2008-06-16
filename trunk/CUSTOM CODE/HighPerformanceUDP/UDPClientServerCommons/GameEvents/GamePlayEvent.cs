using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.GameEvents
{
    public class GamePlayEvent:ICloneable,Interfaces.IGameplayEvent
    {
        #region IGameplayEvent Members

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

        private Microsoft.DirectX.Vector3 positionField;

        public Microsoft.DirectX.Vector3 Position
        {
            get { return positionField; }
            set { positionField = value; }
        }

        private Microsoft.DirectX.Vector3 lookingDirectionField;

        public Microsoft.DirectX.Vector3 LookingDirection
        {
            get { return lookingDirectionField; }
            set { lookingDirectionField = value; }
        }

        private Microsoft.DirectX.Vector3 velocityField;

        public Microsoft.DirectX.Vector3 Velocity
        {
            get { return velocityField; }
            set { velocityField = value; }
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

        public GamePlayEvent(ushort playerId,Constants.GamePlayEventTypeEnumeration eventType,DateTime timestamp)
        {
            this.playerIdField = playerId;
            this.gameplayEventTypeField = eventType;
            this.timestampField = timestamp;
        }

        public GamePlayEvent(ushort playerId, Constants.GamePlayEventTypeEnumeration eventType, 
            DateTime timestamp,Microsoft.DirectX.Vector3 position,Microsoft.DirectX.Vector3 lookingDirection,
            Microsoft.DirectX.Vector3 velocity)
        {
            this.playerIdField = playerId;
            this.gameplayEventTypeField = eventType;
            this.timestampField = timestamp;
            this.lookingDirectionField = lookingDirection;
            this.positionField = position;
            this.velocityField = velocity;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nGameplayEventType = ");
            sb.Append(gameplayEventTypeField);
            sb.Append("\nTimestamp = ");
            sb.Append(timestampField.ToLongTimeString());
            sb.Append("\nPlayerId = ");
            sb.Append(playerIdField);

            return sb.ToString();
        }

        #endregion
    }
}

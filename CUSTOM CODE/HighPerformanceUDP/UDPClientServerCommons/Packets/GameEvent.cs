using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class GameEvent:IPacket,ICloneable
    {
        private Constants.GameEventTypeEnumeration GameEventTypeField;

        public Constants.GameEventTypeEnumeration GameEventType
        {
            get { return GameEventTypeField; }
            set { GameEventTypeField = value; }
        }

        private UInt16 GameEventAttributeField=0;

        /// <summary>
        /// Zero means NULL !!!!!!
        /// </summary>
        public UInt16 GameEventAttribute
        {
            get { return GameEventAttributeField; }
            set { GameEventAttributeField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(4);
            ms.Write(BitConverter.GetBytes((ushort)GameEventTypeField), 0, 2);
            ms.Write(BitConverter.GetBytes((ushort)GameEventAttributeField), 2, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            return this.ToByte();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            GameEvent cpy = new GameEvent();
            cpy.GameEventAttributeField = this.GameEventAttributeField;
            cpy.GameEventTypeField = this.GameEventTypeField;

            return cpy;
        }

        #endregion

        public GameEvent()
        { }

        public GameEvent(byte[] binaryGameEvent)
        {
            this.GameEventTypeField = (Constants.GameEventTypeEnumeration)(ushort)BitConverter.ToInt16(binaryGameEvent, 0);
            this.GameEventAttributeField = (ushort)BitConverter.ToInt16(binaryGameEvent, 2);
        }

        public GameEvent(byte[] binaryGameEvent, int index)
        {
            this.GameEventTypeField = (Constants.GameEventTypeEnumeration)(ushort)BitConverter.ToInt16(binaryGameEvent, index);
            this.GameEventAttributeField = (ushort)BitConverter.ToInt16(binaryGameEvent, 2 + index);
        }
    }
}

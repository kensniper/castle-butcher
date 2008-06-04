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

        private Nullable<Int16> GameEventAttributeField;

        public Nullable<Int16> GameEventAttribute
        {
            get { return GameEventAttributeField; }
            set { GameEventAttributeField = value; }
        }

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = null;
            if (! GameEventAttributeField.HasValue)
            {
                ms = new MemoryStream(2);
                ms.Write(BitConverter.GetBytes((ushort) GameEventTypeField), 0, 2);
            }
            else
            {
                ms = new MemoryStream(4);
                ms.Write(BitConverter.GetBytes((ushort)GameEventTypeField), 0, 2);
                ms.Write(BitConverter.GetBytes((ushort)GameEventAttributeField.Value), 2, 2);
            }

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
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class Vector : Interfaces.ISerializablePacket,ICloneable
    {
        private float xField;

        /// <summary>
        /// X coordinate
        /// </summary>
        public float X
        {
            get { return xField; }
            set { xField = value; }
        }

        private float yField;

        /// <summary>
        /// Y coordinate
        /// </summary>
        public float Y
        {
            get { return yField; }
            set { yField = value; }
        }

        private float zField;

        /// <summary>
        /// Z coordinate
        /// </summary>
        public float Z
        {
            get { return zField; }
            set { zField = value; }
        }

        public Vector(float x, float y, float z)
        {
            this.xField = x;
            this.yField = y;
            this.zField = z;
        }

        public Vector(Vector vector)
        {
            this.xField = vector.X;
            this.yField = vector.Y;
            this.zField = vector.Z;
        }

        public Vector(byte[] packet, int position)
        {
            this.xField = BitConverter.ToSingle(packet, position);
            this.yField = BitConverter.ToSingle(packet, position+4);
            this.zField = BitConverter.ToSingle(packet, position+8);
        }

        public Vector()
        {        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            sb.Append(xField);
            sb.Append(", ");
            sb.Append(yField);
            sb.Append(", ");
            sb.Append(zField);
            sb.Append(")");

            return sb.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
            Vector copy = new Vector(this.xField, this.yField, this.zField);

            return copy;
        }

        #endregion

        #region ISerializablePacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(12);

            ms.Write(BitConverter.GetBytes(xField), 0, 4);
            ms.Write(BitConverter.GetBytes(yField), 0, 4);
            ms.Write(BitConverter.GetBytes(zField), 0, 4);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public byte[] ToMinimalByte()
        {
            return ToByte();
        }

        public int ByteCount
        {
            get { return 12; }
        }

        #endregion
    }
}

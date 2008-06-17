using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Constants;

namespace UDPClientServerCommons.Packets
{
    /// <summary>
    /// Information Send by the server about player
    /// </summary>
    public class PlayerInfo:BasePlayerInfo,ICloneable,Interfaces.ISerializablePacket
    {
        private ushort healthField;

        /// <summary>
        /// Health field, 100 mean that player is healthy
        /// </summary>
        public ushort Health
        {
            get { return healthField; }
            set { healthField = value; }
        }

        new public int ByteCount
        {
            get {return base.ByteCount + 2+2 + ackIdsField.Count*2 + 8; }
        }

        private List<ushort> ackIdsField;

        public List<ushort> AckIds
        {
            get { return ackIdsField; }
            set { ackIdsField = value; }
        }

        private DateTime timestampField;

        public DateTime Timestamp
        {
            get { return timestampField; }
            set { timestampField = value; }
        }

        #region IPacket Members

        public override byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(Constant._MTU_PacketSize);
            ms.Write(base.ToByte(), 0, base.ByteCount);
            ms.Write(BitConverter.GetBytes(healthField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(ackIdsField.Count)), 0, 2);
            for (int i = 0; i < ackIdsField.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(ackIdsField[i]), 0, 2);
            }
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()),0,8);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public override byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.ByteCount);
            ms.Write(base.ToMinimalByte(), 0, base.ByteCount);
            ms.Write(BitConverter.GetBytes(healthField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(ackIdsField.Count)), 0, 2);
            for (int i = 0; i < ackIdsField.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(ackIdsField[i]), 0, 2);
            }
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public PlayerInfo()
            : base()
        {
            ackIdsField = new List<ushort>();
            healthField = 100;
        }

        public PlayerInfo(byte[] binaryPlayerInfo)
            : base(binaryPlayerInfo)
        {
            int pos = base.ByteCount;
            this.healthField = (ushort)BitConverter.ToInt16(binaryPlayerInfo, pos);
            pos += 2;
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, pos);
            pos += 2;
            ackIdsField = new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, pos));
                pos += 2;
            }
            timestampField = DateTime.FromBinary( BitConverter.ToInt64(binaryPlayerInfo, pos));
        }

        public PlayerInfo(byte[] binaryPlayerInfo,int index)
            : base(binaryPlayerInfo,index)
        {
            int pos = index + base.ByteCount;
            this.healthField = (ushort)BitConverter.ToInt16(binaryPlayerInfo, pos);
            pos += 2;
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, pos);
            pos += 2;
            ackIdsField=new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, pos));
                pos += 2;
            }
            timestampField = DateTime.FromBinary( BitConverter.ToInt64(binaryPlayerInfo, pos));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("\n DamageTaken = \t");
            sb.Append(healthField);
            sb.Append("\n Number od acks = \t");
            sb.Append(ackIdsField.Count);

            for (int i = 0; i < ackIdsField.Count; i++)
            {
                sb.Append("\n\t ack id = \t");
                sb.Append(ackIdsField[i]);
            }

            sb.Append("\n Timestamp = ");
            sb.Append(timestampField);

            return sb.ToString();
        }

        #endregion

        #region ICloneable Members

        new object Clone()
        {
            PlayerInfo copy = (PlayerInfo)base.Clone();
            copy.healthField = this.healthField;
            copy.ackIdsField = new List<ushort>(this.ackIdsField);
            copy.timestampField = this.timestampField;
            return copy;
        }

        #endregion
    }
}

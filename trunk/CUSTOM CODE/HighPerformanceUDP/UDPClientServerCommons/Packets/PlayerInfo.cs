using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    /// <summary>
    /// Client Packet is formmated this way:
    /// PlayerPosition 12
    /// PlayerMovementDirection 12
    /// PlayerLookingDirection 12
    /// PlayerId 2
    ///     {PlayerCarringWeponOne,
    ///     PlayerCarringWeponTwo,
    ///     PlayerJumping,
    ///     PlayerShooting,
    ///     PlayerWalking,
    ///     PlayerRunning,
    ///     PlayerDucking} 1
    /// Timestamp 8
    /// DamageTaken 2
    /// Number of Acks 2
    /// Acks (Number of Acks) * 2
    /// Packet length = 51 + (Number of Acks) * 2 bytes
    /// </summary>
    public class PlayerInfo:BasePlayerInfo,ICloneable,Interfaces.ISerializablePacket
    {
        private ushort damageTakenField;

        /// <summary>
        /// Damage taken by player, if 0 then no damage was taken :)
        /// </summary>
        public ushort DamageTaken
        {
            get { return damageTakenField; }
            set { damageTakenField = value; }
        }

        new public int ByteCount
        {
            get {return base.ByteCount + 2+2 + ackIdsField.Count*2; }
        }

        private List<ushort> ackIdsField;

        public List<ushort> AckIds
        {
            get { return ackIdsField; }
            set { ackIdsField = value; }
        }

        #region IPacket Members

        public override byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(base.ToByte(), 0, base.ByteCount);
            ms.Write(BitConverter.GetBytes(damageTakenField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(ackIdsField.Count)), 0, 2);
            for (int i = 0; i < ackIdsField.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(ackIdsField[i]), 0, 2);
            }

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public override byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.ByteCount);
            ms.Write(base.ToMinimalByte(), 0, base.ByteCount);
            ms.Write(BitConverter.GetBytes(damageTakenField), 0, 2);
            ms.Write(BitConverter.GetBytes(Convert.ToUInt16(ackIdsField.Count)), 0, 2);
            for (int i = 0; i < ackIdsField.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(ackIdsField[i]), 0, 2);
            }

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public PlayerInfo()
            : base()
        {
            ackIdsField = new List<ushort>(); 
        }

        public PlayerInfo(byte[] binaryPlayerInfo)
            : base(binaryPlayerInfo)
        {
            this.damageTakenField = (ushort)BitConverter.ToInt16(binaryPlayerInfo, base.ByteCount);
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, base.ByteCount+2);
            ackIdsField = new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, base.ByteCount + 2+2 + i * 2));
            }
        }

        public PlayerInfo(byte[] binaryPlayerInfo,int index)
            : base(binaryPlayerInfo,index)
        {
            this.damageTakenField = (ushort)BitConverter.ToInt16(binaryPlayerInfo, index + base.ByteCount);
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, index + base.ByteCount+2);
            ackIdsField=new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, index + base.ByteCount +2+2+ i * 2));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("\n DamageTaken = \t");
            sb.Append(damageTakenField);
            sb.Append("\n Number od acks = \t");
            sb.Append(ackIdsField.Count);

            for (int i = 0; i < ackIdsField.Count; i++)
            {
                sb.Append("\n\t ack id = \t");
                sb.Append(ackIdsField[i]);
            }

            return sb.ToString();
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            PlayerInfo copy = (PlayerInfo)base.Clone();
            copy.damageTakenField = this.damageTakenField;
            copy.ackIdsField = new List<ushort>(this.ackIdsField);

            return copy;
        }

        #endregion
    }
}

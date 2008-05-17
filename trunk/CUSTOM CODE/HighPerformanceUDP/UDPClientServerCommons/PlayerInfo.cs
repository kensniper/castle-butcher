using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
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
    public class PlayerInfo:BasePlayerInfo
    {
        private ushort damageTakenField;

        private int PlayerInfoBinaryLengthField;

        public int PlayerInfoBinaryLength
        {
            get { return PlayerInfoBinaryLengthField; }
        }

        /// <summary>
        /// Damage taken by player, if 0 then no damage was taken :)
        /// </summary>
        public ushort DamageTaken
        {
            get { return damageTakenField; }
            set { damageTakenField = value; }
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
            ms.Write(base.ToByte(), 0, 47);
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
            int size = 47 + 4 + ackIdsField.Count * 2;

            MemoryStream ms = new MemoryStream(size);
            ms.Write(base.ToMinimalByte(), 0, 47);
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
            this.damageTakenField = (ushort)BitConverter.ToInt16(binaryPlayerInfo, 47);
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, 49);
            ackIdsField = new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, 51 + i * 2));
            }

            PlayerInfoBinaryLengthField = 51 + ackIdsField.Count * 2;
        }

        public PlayerInfo(byte[] binaryPlayerInfo,int index)
            : base(binaryPlayerInfo,index)
        {
            this.damageTakenField = (ushort)BitConverter.ToInt16(binaryPlayerInfo,index+ 47);
            ushort ackNumber = (ushort)BitConverter.ToInt16(binaryPlayerInfo, index + 49);
            ackIdsField=new List<ushort>();
            for (ushort i = 0; i < ackNumber; i++)
            {
                ackIdsField.Add((ushort)BitConverter.ToInt16(binaryPlayerInfo, index + 51+i*2));
            }

            PlayerInfoBinaryLengthField = 51 + ackIdsField.Count * 2;
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
    }
}

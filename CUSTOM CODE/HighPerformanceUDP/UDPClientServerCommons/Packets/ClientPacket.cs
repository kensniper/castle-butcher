using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Packets
{
    public class ClientPacket:BasePlayerInfo,ICloneable,Interfaces.IPacket,Interfaces.ISerializablePacket
    {
        #region Fields

        private PacketIdCounter packetIdField;

        private DateTime timestampField;

        #endregion

        public ClientPacket(byte[] binaryClientPacket)
            : base(binaryClientPacket, 2)
        {
            this.packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryClientPacket, base.ByteCount + 2));
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryClientPacket, base.ByteCount + 4));
        }

        public ClientPacket()
            : base()
        {
            this.packetIdField = new PacketIdCounter();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("\n Packetid = \t");
            sb.Append(packetIdField);
            sb.Append("\n TimeStamp = \t");
            sb.Append(timestampField);

            return sb.ToString();
        }

        #region ICloneable Members

        new public object Clone()
        {
            ClientPacket copy = new ClientPacket();
            copy.PlayerCarringWeponOne = this.PlayerCarringWeponOne;
            copy.PlayerCarringWeponTwo = this.PlayerCarringWeponTwo;
            copy.PlayerDucking = this.PlayerDucking;
            copy.PlayerId = this.PlayerId;
            copy.PlayerJumping = this.PlayerJumping;
            copy.PlayerLookingDirection = (Vector)this.PlayerLookingDirection.Clone();
            copy.PlayerMovementDirection = (Vector)this.PlayerMovementDirection.Clone();
            copy.PlayerPosition = (Vector)this.PlayerPosition.Clone();
            copy.PlayerShooting = this.PlayerShooting;
            copy.TimeStamp = this.TimeStamp;
            copy.PlayerWalking = this.PlayerWalking;
            copy.PlayerRunning = this.PlayerRunning;
            copy.AckRequired = this.AckRequired;
            copy.packetIdField = new PacketIdCounter(this.packetIdField.Value);
            copy.timestampField = this.timestampField;
            return copy;
        }

        #endregion

        #region ISerializablePacket Members

        public override byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            int pos = base.ByteCount;
            ms.Write(base.ToByte(), 0, pos);
            ms.Write(BitConverter.GetBytes(PacketId.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()),0,8);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public override byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(ByteCount);
            ms.Write(BitConverter.GetBytes((ushort)PacketType), 0, 2);
            int pos = base.ByteCount;
            ms.Write(base.ToByte(), 0, pos);
            ms.Write(BitConverter.GetBytes(PacketId.Value), 0, 2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()), 0, 8);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        new int ByteCount
        {
            get {return base.ByteCount + 4+8; }
        }

        #endregion

        #region IPacket Members

        public UDPClientServerCommons.Constants.PacketTypeEnumeration PacketType
        {
            get { return UDPClientServerCommons.Constants.PacketTypeEnumeration.StandardClientPacket; }
        }

        public PacketIdCounter PacketId
        {
            get
            {
                return packetIdField;
            }
            set
            {
                packetIdField = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return timestampField;
            }
            set
            {
                timestampField = value;
            }
        }

        #endregion
    }
}

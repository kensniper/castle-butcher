using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UDPClientServerCommons.Usefull;

namespace UDPClientServerCommons.Packets
{
    public class LeaveGamePacket :Interfaces.ISerializablePacket,ICloneable,Interfaces.IPacket
    {
        #region fields

        private DateTime timestampField;

        private PacketIdCounter packetIdField;

        private ushort playerIdField;

        public ushort PlayerId
        {
            get { return playerIdField; }
            set { playerIdField = value; }
        }

        private ushort gameIdField;

        public ushort GameId
        {
            get { return gameIdField; }
            set { gameIdField = value; }
        }

        #endregion
        
        #region ICloneable Members

        public object Clone()
        {
            LeaveGamePacket cpy = new LeaveGamePacket();
            cpy.gameIdField = this.gameIdField;
            cpy.playerIdField = this.playerIdField;
            cpy.timestampField = this.timestampField;
            cpy.packetIdField = new PacketIdCounter(this.packetIdField.Value);

            return cpy;
        }

        #endregion

        #region IPacket Members

        public UDPClientServerCommons.Constants.PacketTypeEnumeration PacketType
        {
            get { return UDPClientServerCommons.Constants.PacketTypeEnumeration.QuitPacket; }
        }

        public UDPClientServerCommons.Usefull.PacketIdCounter PacketId
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

        #region ISerializablePacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(14);

            ms.Write(BitConverter.GetBytes(playerIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(gameIdField), 0, 2);
            ms.Write(BitConverter.GetBytes(packetIdField.Value),0,2);
            ms.Write(BitConverter.GetBytes(timestampField.ToBinary()),0,8);

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        public byte[] ToMinimalByte()
        {
            return this.ToByte();
        }

        public int ByteCount
        {
            get { return 14; }
        }

        #endregion

        #region Constructor

        public LeaveGamePacket()
        {
            this.packetIdField = new PacketIdCounter();
        }

        public LeaveGamePacket(byte[] binaryLeaveGamePacket)
        {
            this.playerIdField = BitConverter.ToUInt16(binaryLeaveGamePacket, 0);
            this.gameIdField = BitConverter.ToUInt16(binaryLeaveGamePacket, 2);
            this.packetIdField = new PacketIdCounter(BitConverter.ToUInt16(binaryLeaveGamePacket, 4));
            this.timestampField = DateTime.FromBinary(BitConverter.ToInt64(binaryLeaveGamePacket, 6));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nPlayerId = ");
            sb.Append(playerIdField);
            sb.Append("\nGameId = ");
            sb.Append(gameIdField);
            sb.Append("\nPacketId = ");
            sb.Append(packetIdField);
            sb.Append("\nTimeStamp = ");
            sb.Append(timestampField);

            return sb.ToString();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class ClientPacket:BasePlayerInfo,ICloneable
    {
        private ushort packetIdField;
        public const int _MinimalSize = 51;

        public ushort PacketId
        {
            get { return packetIdField; }
            set { packetIdField = value; }
        }

        public override byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(base.ToByte(), 0, 49);
            ms.Write(BitConverter.GetBytes(PacketId), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public override byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(_MinimalSize);
            ms.Write(base.ToByte(), 0, 49);
            ms.Write(BitConverter.GetBytes(PacketId), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public ClientPacket(byte[] binaryClientPacket):base(binaryClientPacket)
        {
            this.packetIdField = (ushort)BitConverter.ToInt16(binaryClientPacket, 49);
        }

        public ClientPacket()
            : base()
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("\n Packetid = \t");
            sb.Append(packetIdField);

            return sb.ToString();
        }

        #region ICloneable Members

        public object Clone()
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
            copy.Timestamp = this.Timestamp;
            copy.PlayerWalking = this.PlayerWalking;
            copy.PlayerRunning = this.PlayerRunning;
            copy.AckRequired = this.AckRequired;
            copy.packetIdField = this.packetIdField;
            return copy;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons
{
    public class ClientPacket:BasePlayerInfo
    {
        private ushort packetIdField;

        public ushort PacketId
        {
            get { return packetIdField; }
            set { packetIdField = value; }
        }

        public override byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            ms.Write(base.ToByte(), 0, 47);
            ms.Write(BitConverter.GetBytes(PacketId), 0, 2);

            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }

        public ClientPacket(byte[] binaryClientPacket):base(binaryClientPacket)
        {
            this.packetIdField = (ushort)BitConverter.ToInt16(binaryClientPacket, 47);
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
    }
}

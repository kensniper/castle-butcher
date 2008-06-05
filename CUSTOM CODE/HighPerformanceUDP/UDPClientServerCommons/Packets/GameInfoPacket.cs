using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    public class GameInfoPacket:ICloneable,IPacket
    {
        public const int _MTU_PacketSize = 1400;

        private List<PlayerStatus> PlayerStatusListField = null;

        private List<TeamScoreStruct> TeamScoreList = null;

        private Constants.GameTypeEnumeration GameTypeField = UDPClientServerCommons.Constants.GameTypeEnumeration.FragLimit;

        private ushort LimitField = 0;

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = new MemoryStream(_MTU_PacketSize);
            int pos = 0;

            if (PlayerStatusListField != null)
            {
                ms.Write(BitConverter.GetBytes(PlayerStatusListField.Count), pos, 2);
                pos += 2;

                for (int i = 0; i < PlayerStatusListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes((ushort)(PlayerStatusListField[i].PlayerStatusByteCount)), pos, 2);
                    pos += 2;
                    ms.Write(PlayerStatusListField[i].ToByte(), pos, PlayerStatusListField[i].PlayerStatusByteCount);
                    pos += PlayerStatusListField[i].PlayerStatusByteCount;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), pos, 2);
                pos += 2;
            }

            if (TeamScoreList != null)
            {
                ms.Write(BitConverter.GetBytes((ushort)TeamScoreList.Count), pos, 2);
                pos += 2;

                for (int i = 0; i < TeamScoreList.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes(TeamScoreList[i].TeamId), pos, 2);
                    pos += 2;
                    ms.Write(BitConverter.GetBytes(TeamScoreList[i].TeamScore), pos, 2);
                    pos += 2;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), pos, 2);
                pos += 2;
            }

            ms.Write(BitConverter.GetBytes((ushort)GameTypeField), pos, 2);
            pos += 2;
            ms.Write(BitConverter.GetBytes(LimitField), pos, 2);
            pos += 2;

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        private int GetMinimalSize()
        {
            int size = 0;

            if (PlayerStatusListField != null)
            {

                size += 2;

                for (int i = 0; i < PlayerStatusListField.Count; i++)
                {
                    size += 2;
                    size += PlayerStatusListField[i].PlayerStatusByteCount;
                }
            }
            else
            {
                size += 2;
            }



            if (TeamScoreList != null)
            {

                size += 2;

                for (int i = 0; i < TeamScoreList.Count; i++)
                {

                    size += 2;
                    size += 2;
                }
            }
            else
            {
                size += 2;
            }

            size += 2;
            size += 2;

            return size;
        }

        public byte[] ToMinimalByte()
        {
            MemoryStream ms = new MemoryStream(this.GetMinimalSize());
            int pos = 0;

            if (PlayerStatusListField != null)
            {
                ms.Write(BitConverter.GetBytes(PlayerStatusListField.Count), pos, 2);
                pos += 2;

                for (int i = 0; i < PlayerStatusListField.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes((ushort)(PlayerStatusListField[i].PlayerStatusByteCount)), pos, 2);
                    pos += 2;
                    ms.Write(PlayerStatusListField[i].ToByte(), pos, PlayerStatusListField[i].PlayerStatusByteCount);
                    pos += PlayerStatusListField[i].PlayerStatusByteCount;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), pos, 2);
                pos += 2;
            }

            if (TeamScoreList != null)
            {
                ms.Write(BitConverter.GetBytes((ushort)TeamScoreList.Count), pos, 2);
                pos += 2;

                for (int i = 0; i < TeamScoreList.Count; i++)
                {
                    ms.Write(BitConverter.GetBytes(TeamScoreList[i].TeamId), pos, 2);
                    pos += 2;
                    ms.Write(BitConverter.GetBytes(TeamScoreList[i].TeamScore), pos, 2);
                    pos += 2;
                }
            }
            else
            {
                ms.Write(BitConverter.GetBytes((ushort)0), pos, 2);
                pos += 2;
            }

            ms.Write(BitConverter.GetBytes((ushort)GameTypeField), pos, 2);
            pos += 2;
            ms.Write(BitConverter.GetBytes(LimitField), pos, 2);
            pos += 2;

            byte[] result = ms.GetBuffer();
            ms.Close();

            return result;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

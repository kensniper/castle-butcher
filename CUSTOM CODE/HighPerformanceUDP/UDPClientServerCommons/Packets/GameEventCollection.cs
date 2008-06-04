using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    class GameEventCollection : ICollection<GameEvent>,IPacket,ICloneable
    {
        private readonly object ListLock = new object();

        private List<GameEvent> GameEventList = null;

        private ushort PlayerIdField;

        public ushort PlayerId
        {
            get { return PlayerIdField; }
            set { PlayerIdField = value; }
        }

        private DateTime GameEventTimeField;

        public DateTime GameEventTime
        {
            get { return GameEventTimeField; }
            set { GameEventTimeField = value; }
        }

        public int GameEventCollectionByteCount
        {
            get {
                int count = 0;
                for (int i = 0; i < GameEventList.Count; i++)
                {
                    if (GameEventList[i].GameEventAttribute.HasValue)
                        count += 4;
                    else
                        count += 2;
                }
                return count;
            }
        }

        public GameEventCollection()
            : base()
        {
            GameEventList = new List<GameEvent>();
        }

        #region ICollection<GameEvent> Members

        public void Add(GameEvent item)
        {
            lock (ListLock)
            {
                GameEventList.Add(item);
            }
        }

        public void Clear()
        {
            lock (ListLock)
            {
                GameEventList.Clear();
            }
        }

        public bool Contains(GameEvent item)
        {
            bool ret = false;
            lock (ListLock)
            {
                ret = GameEventList.Contains(item);
            }
            return ret;
        }

        public void CopyTo(GameEvent[] array, int arrayIndex)
        {
            lock (ListLock)
            {
                GameEventList.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                int tmp = -1;
                lock (ListLock)
                {
                    tmp = GameEventList.Count;
                }
                return tmp;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(GameEvent item)
        {
            bool tmp = false;
            lock (ListLock)
            {
                tmp = GameEventList.Remove(item);
            }
            return tmp;
        }

        #endregion

        #region IEnumerable<GameEvent> Members

        public IEnumerator<GameEvent> GetEnumerator()
        {
            return GameEventList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GameEventList.GetEnumerator();
        }

        #endregion

        #region IPacket Members

        public byte[] ToByte()
        {
            MemoryStream ms = null;
            lock (ListLock)
            {
                ms = new MemoryStream(GameEventCollectionByteCount + 2);

                ms.Write(BitConverter.GetBytes((ushort)GameEventList.Count), 0, 2);
                int pos = 2;
                for (int i = 0; i < GameEventList.Count; i++)
                {
                    if (GameEventList[i].GameEventAttribute.HasValue)
                    {
                        pos += 4;
                        ms.Write(GameEventList[i].ToByte(), pos, 4);
                    }
                    else
                    {
                        pos += 2;
                        ms.Write(GameEventList[i].ToByte(), pos, 2);
                    }
                }
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
            GameEventCollection cpy = new GameEventCollection();
            lock(ListLock)
            {
                cpy.GameEventList = new List<GameEvent>(this.GameEventList);
            }
            return cpy;
        }

        #endregion
    }
}

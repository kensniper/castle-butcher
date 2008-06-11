using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UDPClientServerCommons.Packets
{
    /// <summary>
    /// Collection of game events
    /// </summary>
    public class GameEventCollection : ICollection<GameEvent>,Interfaces.ISerializablePacket,ICloneable
    {
        #region fields

        private readonly object ListLock = new object();

        private List<GameEvent> gameEventList = null;

        private DateTime gameEventTimeField;

        public DateTime GameEventTime
        {
            get { return gameEventTimeField; }
            set { gameEventTimeField = value; }
        }

        #endregion

        #region ICollection<GameEvent> Members

        public void Add(GameEvent item)
        {
            lock (ListLock)
            {
                gameEventList.Add(item);
            }
        }

        public void Clear()
        {
            lock (ListLock)
            {
                gameEventList.Clear();
            }
        }

        public bool Contains(GameEvent item)
        {
            bool ret = false;
            lock (ListLock)
            {
                ret = gameEventList.Contains(item);
            }
            return ret;
        }

        public void CopyTo(GameEvent[] array, int arrayIndex)
        {
            lock (ListLock)
            {
                gameEventList.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                int tmp = -1;
                lock (ListLock)
                {
                    tmp = gameEventList.Count;
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
                tmp = gameEventList.Remove(item);
            }
            return tmp;
        }

        #endregion

        #region IEnumerable<GameEvent> Members

        public IEnumerator<GameEvent> GetEnumerator()
        {
            return gameEventList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return gameEventList.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            GameEventCollection cpy = new GameEventCollection();
            lock(ListLock)
            {
                cpy.gameEventList = new List<GameEvent>(this.gameEventList);
            }
            return cpy;
        }

        #endregion

        #region Constructor

        public GameEventCollection()
            : base()
        {
            gameEventList = new List<GameEvent>();
        }

        public GameEventCollection(byte[] binaryGameEventCollection)
        {
            lock (ListLock)
            {
                this.gameEventList = new List<GameEvent>();
                int count = (int)BitConverter.ToInt16(binaryGameEventCollection, 0);
                this.gameEventTimeField = DateTime.FromBinary(BitConverter.ToInt64(binaryGameEventCollection, 2));
                for (int i = 0; i < count; i++)
                {
                    GameEvent ge = new GameEvent(binaryGameEventCollection, 10 + 4 * i);
                    this.gameEventList.Add(ge);
                }
            }
        }

        public GameEventCollection(byte[] binaryGameEventCollection,int index)
        {
            lock (ListLock)
            {
                this.gameEventList = new List<GameEvent>();
                int count = (int)BitConverter.ToInt16(binaryGameEventCollection, index);
                this.gameEventTimeField = DateTime.FromBinary(BitConverter.ToInt64(binaryGameEventCollection, 2 + index));
                for (int i = 0; i < count; i++)
                {
                    GameEvent ge = new GameEvent(binaryGameEventCollection, 10 + index + 4 * i);
                    this.gameEventList.Add(ge);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\nGameEventTime = ");
            sb.Append(gameEventTimeField);
            sb.Append("\nGameEventCollection");
            for (int i = 0; i < this.gameEventList.Count; i++)
                sb.Append(gameEventList[i]);

            return sb.ToString();
        }

        #endregion

        #region ISerializablePacket Members

        public int ByteCount
        {
            get
            {
                // because listCount first
                int count = 2;
                count += 8; //datetime
                for (int i = 0; i < gameEventList.Count; i++)
                {
                    count += gameEventList[i].ByteCount;
                }
                return count;
            }
        }

        public byte[] ToByte()
        {
            MemoryStream ms = null;
            lock (ListLock)
            {
                ms = new MemoryStream(ByteCount);

                ms.Write(BitConverter.GetBytes((ushort)gameEventList.Count), 0, 2);
               // int pos = 2;
                ms.Write(BitConverter.GetBytes(gameEventTimeField.ToBinary()), 0, 8);
                for (int i = 0; i < gameEventList.Count; i++)
                {
                    ms.Write(gameEventList[i].ToByte(), 0, 4);
                    //pos += GameEventList[i].ByteCount;
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
    }
}

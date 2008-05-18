using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Physics
{
    //public class PriorityQueue
    //{
    //    IComparable[] m_data = null;
    //    int m_size = 0;

    //    public PriorityQueue()
    //    {
    //        m_data = new IComparable[10];
    //    }
    //    public PriorityQueue(int size)
    //    {
    //        m_data = new IComparable[size];
    //    }


    //    private void ChangeSize(int newSize)
    //    {
    //        IComparable[] tab = new IComparable[newSize];
    //        for (int i = 0; i < m_size && i < newSize; i++)
    //        {
    //            tab[i] = m_data[i];
    //        }
    //        m_data = tab;
    //    }
    //    private void Xchg(IComparable a, IComparable b)
    //    {
    //        IComparable temp = a;
    //        a = b;
    //        b = temp;
    //    }
    //    private void Heapify(int i)
    //    {
    //        int l = 2 * i;
    //        int r = 2 * i + 1;
    //        int max = 0;
    //        if (l <= m_size && m_data[l].CompareTo(m_data[i])>0)
    //            max = l;
    //        else
    //            max = i;
    //        if (r <= m_size && m_data[r].CompareTo(m_data[max])>0)
    //            max = r;
    //        if (max != i)
    //        {
    //            Xchg(m_data[i], m_data[max]);
    //            Heapify(max);
    //        }
    //    }

    //    public void Insert(IComparable item)
    //    {
    //        if (m_size >= m_data.Length)
    //        {
    //            ChangeSize(m_size * 2);
    //        }
    //        m_size++;
    //        int i = m_size - 1;
    //        while (i > 0 && m_data[i/2].CompareTo(item) < 0)
    //        {
    //            m_data[i] = m_data[i/2];
    //            i = i/2;
    //        }
    //        m_data[i] = item;
    //    }
    //    public IComparable PopHead()
    //    {
    //        if (m_size < 1)
    //        {
    //            return null;
    //        }
    //        IComparable max = m_data[0];
    //        m_data[0] = m_data[m_size - 1];
    //        m_size--;
    //        Heapify( 0);
    //        if (m_size * 2 < m_data.Length)
    //        {
    //            ChangeSize(m_size/2);
    //        }
    //        return max;
    //    }

    //    public IComparable Head
    //    {
    //        get
    //        {
    //            return m_data[0];
    //        }
    //    }
    //    public IComparable Tail
    //    {
    //        get
    //        {
    //            return m_data[m_size - 1];
    //        }
    //    }


    //}

    public class IntersectionQueue
    {
        Intersection[] m_data = null;
        int m_size = 0;
        int m_maxSize = 0;

        public IntersectionQueue()
        {
            m_data = new Intersection[10];
        }
        public IntersectionQueue(int size)
        {
            m_data = new Intersection[size];
        }


        private void ChangeSize(int newSize)
        {
            Intersection[] tab = new Intersection[newSize];
            for (int i = 0; i < m_size && i < newSize; i++)
            {
                tab[i] = m_data[i];
            }
            m_data = tab;
        }
        //private void Xchg(Intersection a, Intersection b)
        //{
        //    Intersection temp = a;
        //    a = b;
        //    b = temp;
        //}
        private void Heapify(int i)
        {
            int l = 2 * i + 1;
            int r = 2 * i + 2;
            int min = 0;
            if (l <= m_size && m_data[l].CompareTo(m_data[i]) < 0)
                min = l;
            else
                min = i;
            if (r <= m_size && m_data[r].CompareTo(m_data[min]) < 0)
                min = r;
            if (min != i)
            {
                //Xchg(m_data[i], m_data[min]);
                Intersection temp = m_data[i];
                m_data[i] = m_data[min];
                m_data[min] = temp;
                Heapify(min);
            }
        }

        public void Insert(Intersection item)
        {
            if (m_size >= m_data.Length)
            {
                ChangeSize(m_size * 2);
            }
            m_size++;
            if (m_size > m_maxSize)
                m_maxSize = m_size;
            int i = m_size - 1;
            while (i > 0 && m_data[(i - 1) / 2].CompareTo(item) > 0)
            {
                m_data[i] = m_data[(i - 1) / 2];
                i = (i - 1) / 2;
            }
            m_data[i] = (Intersection)item.Clone();
        }
        public Intersection PopHead()
        {
            if (m_size < 1)
            {
                return null;
            }
            Intersection max = m_data[0];
            m_data[0] = m_data[m_size - 1];
            m_size--;
            Heapify(0);

            return max;
        }
        public void Remove(int index)
        {
            if (index < 0 || index >= m_size)
                return;

            int l = index * 2 + 1;
            int r = index * 2 + 2;
            if (l < m_size && r < m_size)
            {
                if (m_data[l].CompareTo(m_data[r]) < 0)
                {
                    m_data[index] = m_data[l];
                    Remove(l);
                    return;
                }
                else
                {
                    m_data[index] = m_data[r];
                    Remove(r);
                    return;
                }
            }
            else if (l < m_size && r >= m_size)
            {
                m_data[index] = m_data[l];

            }
            else
            {
                m_data[index] = m_data[m_size - 1];
            }
            m_size--;
        }

        public Intersection this[int index]
        {
            get
            {
                return m_data[index];
            }
        }
        public Intersection Head
        {
            get
            {
                return m_data[0];
            }
        }
        public Intersection Tail
        {
            get
            {
                return m_data[m_size - 1];
            }
        }
        public int Count
        {
            get
            {
                return m_size;
            }
        }
        public void Compact()
        {

            ChangeSize(m_maxSize);
            m_maxSize = m_size;
        }
    }
    public class Intersection : IComparable, ICloneable
    {
        public float T0;
        public float T1;
        public IPhysicalObject A;
        public IPhysicalObject B;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (((Intersection)obj).T0 < this.T0)
                return 1;
            else if (((Intersection)obj).T0 > this.T0)
                return -1;
            else
                return 0;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            Intersection i = new Intersection();
            i.A = A;
            i.B = B;
            i.T0 = T0;
            i.T1 = T1;
            return i;
        }

        #endregion
    }


    //
    //
    internal class CollisionQueue
    {
        Collision[] m_data = null;
        int m_size = 0;
        int m_maxSize = 0;

        public CollisionQueue()
        {
            m_data = new Collision[10];
        }
        public CollisionQueue(int size)
        {
            m_data = new Collision[size];
        }


        private void ChangeSize(int newSize)
        {
            Collision[] tab = new Collision[newSize];
            for (int i = 0; i < m_size && i < newSize; i++)
            {
                tab[i] = m_data[i];
            }
            m_data = tab;

        }
        //private void Xchg(Intersection a, Intersection b)
        //{
        //    Intersection temp = a;
        //    a = b;
        //    b = temp;
        //}
        private void Heapify(int i)
        {
            int l = 2 * i + 1;
            int r = 2 * i + 2;
            int min = 0;
            if (l < m_size && m_data[l].CompareTo(m_data[i]) < 0)
                min = l;
            else
                min = i;
            if (r < m_size && m_data[r].CompareTo(m_data[min]) < 0)
                min = r;
            if (min != i)
            {
                //Xchg(m_data[i], m_data[min]);
                Collision temp = m_data[i];
                m_data[i] = m_data[min];
                m_data[min] = temp;
                Heapify(min);
            }
        }

        public void Insert(Collision item)
        {
            if (m_size >= m_data.Length)
            {
                ChangeSize(m_size * 2);
            }
            m_size++;
            if (m_size > m_maxSize)
                m_maxSize = m_size;
            int i = m_size - 1;
            while (i > 0 && m_data[(i - 1) / 2].CompareTo(item) > 0)
            {
                m_data[i] = m_data[(i - 1) / 2];
                i = (i - 1) / 2;
            }
            m_data[i] = item;
        }
        public Collision PopHead()
        {
            if (m_size < 1)
            {
                return null;
            }
            Collision max = m_data[0];
            m_data[0] = m_data[m_size - 1];
            m_data[m_size - 1] = null;
            m_size--;
            Heapify(0);

            return max;
        }
        public void Remove(int index)
        {
            if (index < 0 || index >= m_size)
                return;

            int l = index * 2 + 1;
            int r = index * 2 + 2;
            if (l < m_size && r < m_size)
            {
                if (m_data[l].CompareTo(m_data[r]) < 0)
                {
                    m_data[index] = m_data[l];
                    Remove(l);
                    return;
                }
                else
                {
                    m_data[index] = m_data[r];
                    Remove(r);
                    return;
                }
            }
            else if (l < m_size && r >= m_size)
            {
                m_data[index] = m_data[l];
                m_data[l] = null;
            }
            else
            {
                m_data[index] = m_data[m_size - 1];
                m_data[m_size - 1] = null;
            }
            m_size--;
        }

        public Collision this[int index]
        {
            get
            {
                return m_data[index];
            }
        }
        public Collision Head
        {
            get
            {
                return m_data[0];
            }
        }
        public Collision Tail
        {
            get
            {
                return m_data[m_size - 1];
            }
        }
        public int Count
        {
            get
            {
                return m_size;
            }
        }
        public void Compact()
        {

            ChangeSize(m_maxSize);
            m_maxSize = m_size;
        }
    }
    internal class Collision : IComparable
    {
        public float T;

        public IPhysicalObject A;
        public IPhysicalObject B;

        public CollisionParameters Params;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (((Collision)obj).T < this.T)
                return 1;
            else if (((Collision)obj).T > this.T)
                return -1;
            else
                return 0;
        }

        #endregion
    }
    //internal class CollisionQueue
    //{
    //    //Collision[] m_data = null;
    //    //int m_size = 0;
    //    //int m_maxSize = 0;

    //    SortedList<Collision, Collision> data;

    //    public CollisionQueue()
    //    {
    //        data = new SortedList<Collision, Collision>();
            
    //    }


    //    //private void Xchg(Intersection a, Intersection b)
    //    //{
    //    //    Intersection temp = a;
    //    //    a = b;
    //    //    b = temp;
    //    //}
      

    //    public void Insert(Collision item)
    //    {
    //        data.Add(item, item);
    //    }
    //    public Collision PopHead()
    //    {
    //        Collision c = data.Values[0];
    //        data.RemoveAt(0);

    //        return c;
    //    }
    //    public void Remove(int index)
    //    {
    //        data.RemoveAt(index);
    //    }

    //    public Collision this[int index]
    //    {
    //        get
    //        {
    //            return data.Values[index];
    //        }
    //    }
      
    //    public int Count
    //    {
    //        get
    //        {
    //            return data.Count;
    //        }
    //    }
       
    //}
    //internal class Collision : IComparable
    //{
    //    public float T;

    //    public IPhysicalObject A;
    //    public IPhysicalObject B;

    //    public CollisionParameters Params;

    //    #region IComparable Members

    //    public int CompareTo(object obj)
    //    {
    //        if (((Collision)obj).T < this.T)
    //            return 1;
    //        else if (((Collision)obj).T > this.T)
    //            return -1;
    //        else
    //            return -1;
    //    }

    //    #endregion
    //}
}

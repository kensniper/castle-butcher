using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.MyMath
{
    public class MyMatrix : ICloneable
    {
        float[] m = new float[9];

        public MyMatrix()
        {
            m[0] = 0;
            m[1] = 0;
            m[2] = 0;
            m[3] = 0;
            m[4] = 0;
            m[5] = 0;
            m[6] = 0;
            m[7] = 0;
            m[8] = 0;

        }
        public MyMatrix(float m11, float m12, float m13, float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            m[0] = m11;
            m[1] = m12;
            m[2] = m13;
            m[3] = m21;
            m[4] = m22;
            m[5] = m23;
            m[6] = m31;
            m[7] = m32;
            m[8] = m33;
        }
        public MyMatrix(MyVector col1, MyVector col2, MyVector col3)
        {
            m[0] = col1.X;
            m[1] = col2.X;
            m[2] = col3.X;
            m[3] = col1.Y;
            m[4] = col2.Y;
            m[5] = col3.Y;
            m[6] = col1.Z;
            m[7] = col2.Z;
            m[8] = col3.Z;
        }
        public MyMatrix(MyMatrix m)
        {
            this.m[0] = m.m[0];
            this.m[1] = m.m[1];
            this.m[2] = m.m[2];
            this.m[3] = m.m[3];
            this.m[4] = m.m[4];
            this.m[5] = m.m[5];
            this.m[6] = m.m[6];
            this.m[7] = m.m[7];
            this.m[8] = m.m[8];
        }

        public float Det
        {
            get
            {
                return m[0] * m[4] * m[8] + m[1] * m[5] * m[6] + m[2] * m[3] * m[7] -
                    m[2] * m[4] * m[6] - m[1] * m[3] * m[8] - m[0] * m[5] * m[7];
            }
        }
        public MyMatrix Transpose
        {
            get
            {
                return new MyMatrix(m[0], m[3], m[6], m[1], m[4], m[7], m[2], m[5], m[8]);
            }
        }
        public MyMatrix Inverse
        {
            get
            {
                float d = Det;
                if (d == 0) d = 1;
                return new MyMatrix((m[4] * m[8] - m[5] * m[7]) / d,
                    -(m[1] * m[8] - m[2] * m[7]) / d,
                    (m[1] * m[5] - m[2] * m[4]) / d,
                    -(m[3] * m[8] - m[5] * m[6]) / d,
                    (m[0] * m[8] - m[2] * m[6]) / d,
                    -(m[0] * m[5] - m[2] * m[4]) / d,
                    (m[3] * m[7] - m[4] * m[6]) / d,
                    -(m[0] * m[7] - m[1] * m[6]) / d,
                    (m[0] * m[4] - m[1] * m[3]) / d);
            }
        }

        public MyMatrix Add(MyMatrix m)
        {
            this.m[0] += m.m[0];
            this.m[1] += m.m[1];
            this.m[2] += m.m[2];
            this.m[3] += m.m[3];
            this.m[4] += m.m[4];
            this.m[5] += m.m[5];
            this.m[6] += m.m[6];
            this.m[7] += m.m[7];
            this.m[8] += m.m[8];
            return this;
        }
        public MyMatrix Subtract(MyMatrix m)
        {
            this.m[0] -= m.m[0];
            this.m[1] -= m.m[1];
            this.m[2] -= m.m[2];
            this.m[3] -= m.m[3];
            this.m[4] -= m.m[4];
            this.m[5] -= m.m[5];
            this.m[6] -= m.m[6];
            this.m[7] -= m.m[7];
            this.m[8] -= m.m[8];
            return this;
        }
        public static MyMatrix operator *(MyMatrix m1, MyMatrix m2)
        {
            return new MyMatrix(m1.m[0] * m2.m[0] + m1.m[3] * m2.m[1] + m1.m[6] * m2.m[2],
                m1.m[1] * m2.m[0] + m1.m[4] * m2.m[1] + m1.m[7] * m2.m[2],
                m1.m[2] * m2.m[0] + m1.m[5] * m2.m[1] + m1.m[8] * m2.m[2],
                m1.m[0] * m2.m[3] + m1.m[3] * m2.m[4] + m1.m[6] * m2.m[5],
                m1.m[1] * m2.m[3] + m1.m[4] * m2.m[4] + m1.m[7] * m2.m[5],
                m1.m[2] * m2.m[3] + m1.m[5] * m2.m[4] + m1.m[8] * m2.m[5],
                m1.m[0] * m2.m[6] + m1.m[3] * m2.m[7] + m1.m[6] * m2.m[8],
                m1.m[1] * m2.m[6] + m1.m[4] * m2.m[7] + m1.m[7] * m2.m[8],
                m1.m[2] * m2.m[6] + m1.m[5] * m2.m[7] + m1.m[8] * m2.m[8]);

        }
        public static MyMatrix operator *(MyMatrix m, float s)
        {
            return new MyMatrix(m.m[0] * s, m.m[1] * s, m.m[2] * s, m.m[3] * s, m.m[4] * s, m.m[5] * s,
                m.m[6] * s, m.m[7] * s, m.m[8] * s);
        }
        public static MyMatrix operator *(float s, MyMatrix m)
        {
            return new MyMatrix(m.m[0] * s, m.m[1] * s, m.m[2] * s, m.m[3] * s, m.m[4] * s, m.m[5] * s,
                m.m[6] * s, m.m[7] * s, m.m[8] * s);
        }
        public static MyMatrix operator /(MyMatrix m, float s)
        {
            return new MyMatrix(m.m[0] / s, m.m[1] / s, m.m[2] / s, m.m[3] / s, m.m[4] / s, m.m[5] / s,
                m.m[6] / s, m.m[7] / s, m.m[8] / s);
        }

        public static MyMatrix operator +(MyMatrix m1, MyMatrix m2)
        {
            return new MyMatrix(m1.m[0] + m2.m[0], m1.m[1] + m2.m[1], m1.m[2] + m2.m[2],
                m1.m[3] + m2.m[3], m1.m[4] + m2.m[4], m1.m[5] + m2.m[5],
                m1.m[6] + m2.m[6], m1.m[7] + m2.m[7], m1.m[8] + m2.m[8]);
        }
        public static MyMatrix operator -(MyMatrix m1, MyMatrix m2)
        {
            return new MyMatrix(m1.m[0] - m2.m[0], m1.m[1] - m2.m[1], m1.m[2] - m2.m[2],
                m1.m[3] - m2.m[3], m1.m[4] - m2.m[4], m1.m[5] - m2.m[5],
                m1.m[6] - m2.m[6], m1.m[7] - m2.m[7], m1.m[8] - m2.m[8]);
        }

        public static MyVector operator *(MyMatrix m, MyVector v)
        {
            return new MyVector(m.m[0] * v.X + m.m[1] * v.Y + m.m[2] * v.Z,
                m.m[3] * v.X + m.m[4] * v.Y + m.m[5] * v.Z,
                m.m[6] * v.X + m.m[7] * v.Y + m.m[8] * v.Z);
        }
        public static MyVector operator *(MyVector v, MyMatrix m)
        {
            return new MyVector(m.m[0] * v.X + m.m[3] * v.Y + m.m[6] * v.Z,
                m.m[1] * v.X + m.m[4] * v.Y + m.m[7] * v.Z,
                m.m[2] * v.X + m.m[5] * v.Y + m.m[8] * v.Z);
        }

        public static MyMatrix FromString(string s)
        {
            MyMatrix m = new MyMatrix();

            if(s[0]!='[' || s[s.Length-1]!=']')
                return null;
            string[] tokens = s.Substring(1, s.Length - 2).Split(',', ';');
            if (tokens.Length != 9)
                return null;
            m.M11 = float.Parse(tokens[0]);
            m.M12 = float.Parse(tokens[1]);
            m.M13 = float.Parse(tokens[2]);
            m.M21 = float.Parse(tokens[3]);
            m.M22 = float.Parse(tokens[4]);
            m.M23 = float.Parse(tokens[5]);
            m.M31 = float.Parse(tokens[6]);
            m.M32 = float.Parse(tokens[7]);
            m.M33 = float.Parse(tokens[8]);
            return m;

        }
        public float M11
        {
            get
            {
                return m[0];
            }
            set
            {
                m[0] = value;
            }
        }
        public float M12
        {
            get
            {
                return m[1];
            }
            set
            {
                m[1] = value;
            }
        }
        public float M13
        {
            get
            {
                return m[2];
            }
            set
            {
                m[2] = value;
            }
        }
        public float M21
        {
            get
            {
                return m[3];
            }
            set
            {
                m[3] = value;
            }
        }
        public float M22
        {
            get
            {
                return m[4];
            }
            set
            {
                m[4] = value;
            }
        }
        public float M23
        {
            get
            {
                return m[5];
            }
            set
            {
                m[5] = value;
            }
        }
        public float M31
        {
            get
            {
                return m[6];
            }
            set
            {
                m[6] = value;
            }
        }
        public float M32
        {
            get
            {
                return m[7];
            }
            set
            {
                m[7] = value;
            }
        }
        public float M33
        {
            get
            {
                return m[8];
            }
            set
            {
                m[8] = value;
            }
        }

        public static MyMatrix Identity
        {
            get
            {
                return new MyMatrix(1, 0, 0, 0, 1, 0, 0, 0, 1);
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            return new MyMatrix(this);
        }

        #endregion
    }
}

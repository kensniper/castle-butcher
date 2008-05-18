using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using System.Globalization;
using System.Diagnostics;

namespace Framework.MyMath
{
    [DebuggerDisplay("({x};{y};{z})")]
    public struct MyVector
    {
        float x, y, z;


        public MyVector(MyVector v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public MyVector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public MyVector Add(MyVector v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
            return this;
        }
        public MyVector Subtract(MyVector v)
        {
            x -= v.x;
            y -= v.y;
            z -= v.z;
            return this;
        }
        public MyVector Multiply(float s)
        {
            x *= s;
            y *= s;
            z *= s;
            return this;
        }
        public MyVector Divide(float s)
        {
            x /= s;
            y /= s;
            z /= s;
            return this;
        }
        public MyVector Rotate(MyQuaternion q)
        {
            MyQuaternion t = (q) * (this) * (~q);
            x = t.Vector.X;
            y = t.Vector.Y;
            z = t.Vector.Z;
            return this;
        }
        public static MyVector operator +(MyVector v1, MyVector v2)
        {
            return new MyVector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static MyVector operator -(MyVector v1, MyVector v2)
        {
            return new MyVector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static MyVector operator -(MyVector v)
        {
            return new MyVector(-v.x, -v.y, -v.z);
        }
        public static float operator *(MyVector v1, MyVector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        public float Dot(MyVector v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public static MyVector operator ^(MyVector v1, MyVector v2)
        {
            return new MyVector(v1.y * v2.z - v1.z * v2.y, -v1.x * v2.z + v1.z * v2.x, v1.x * v2.y - v1.y * v2.x);
        }
        public MyVector Cross(MyVector v)
        {
            return new MyVector(y * v.z - z * v.y, -x * v.z + z * v.x, x * v.y - y * v.x);
        }

        public MyVector Normalize()
        {
            float mag = Length;
            if (mag != 0 && mag != 1)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
            return this;
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public float LengthSq
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        static NumberFormatInfo nfi = new NumberFormatInfo();
        public static bool FromString(string s,out MyVector v)
        {
            v = new MyVector();
            nfi.NumberDecimalSeparator = ".";

            if (s[0] != '[' || s[s.Length - 1] != ']')
                return false;
            s = s.Substring(1, s.Length - 2);
            string[] tokens = s.Split(',');
            if (tokens.Length != 3)
                return false;
            try
            {
                v.X = float.Parse(tokens[0], nfi);
                v.Y = float.Parse(tokens[1], nfi);
                v.Z = float.Parse(tokens[2], nfi);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static MyVector operator *(MyVector v, float s)
        {
            return new MyVector(v.x * s, v.y * s, v.z * s);
        }
        public static MyVector operator *(float s, MyVector v)
        {
            return new MyVector(v.x * s, v.y * s, v.z * s);
        }
        public static MyVector operator /(MyVector v, float s)
        {
            return new MyVector(v.x / s, v.y / s, v.z / s);
        }

        public static explicit operator Vector3(MyVector v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static explicit operator MyVector(Vector3 v)
        {
            return new MyVector(v.X, v.Y, v.Z);
        }

        public override string ToString()
        {
            string s = x.ToString();
            return "[" + ((int)x).ToString() + ";" + ((int)y).ToString() + ";" + ((int)z).ToString() + "]";
        }
    }
}

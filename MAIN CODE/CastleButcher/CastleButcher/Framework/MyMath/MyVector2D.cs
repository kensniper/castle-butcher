using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Framework.MyMath
{
    [DebuggerDisplay("X={x} ; Y={y}")]
    public struct MyVector2D
    {
        int x, y;


        public MyVector2D(MyVector2D v)
        {
            x = v.x;
            y = v.y;
            
        }
        public MyVector2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
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
        public int Y
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
        

        public MyVector2D Add(MyVector2D v)
        {
            x += v.x;
            y += v.y;
            return this;
        }
        public MyVector2D Subtract(MyVector2D v)
        {
            x -= v.x;
            y -= v.y;
            return this;
        }
        public MyVector2D Multiply(int s)
        {
            x *= s;
            y *= s;
            return this;
        }
        public MyVector2D Divide(int s)
        {
            x /= s;
            y /= s;
            return this;
        }
     
        public static MyVector2D operator +(MyVector2D v1, MyVector2D v2)
        {
            return new MyVector2D(v1.x + v2.x, v1.y + v2.y);
        }
        public static MyVector2D operator -(MyVector2D v1, MyVector2D v2)
        {
            return new MyVector2D(v1.x - v2.x, v1.y - v2.y);
        }
        public static MyVector2D operator -(MyVector2D v)
        {
            return new MyVector2D(-v.x, -v.y);
        }
        public static bool operator ==(MyVector2D v1, MyVector2D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(MyVector2D v1, MyVector2D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }
     
        public MyVector2D Normalize()
        {
            int mag = Length;
            if (mag != 0 && mag != 1)
            {
                x /= mag;
                y /= mag;
            }
            return this;
        }
        public int Length
        {
            get
            {
                return (int)Math.Sqrt(x * x + y * y);
            }
        }
        public int LengthSq
        {
            get
            {
                return x * x + y * y;
            }
        }

        public static MyVector2D operator *(MyVector2D v, int s)
        {
            return new MyVector2D(v.x * s, v.y * s);
        }
        public static MyVector2D operator *(int s, MyVector2D v)
        {
            return new MyVector2D(v.x * s, v.y * s);
        }
        public static MyVector2D operator /(MyVector2D v, int s)
        {
            return new MyVector2D(v.x / s, v.y / s);
        }

        
        
        public static implicit operator MyVector(MyVector2D v)
        {
            return new MyVector(v.x, v.y, 0);
        }
        public static explicit operator MyVector2D(MyVector v)
        {
            return new MyVector2D((int)v.X, (int)v.Y);
        }

        public override bool Equals(object obj)
        {
            return ((MyVector2D)obj) == (this);
        }
    }
}

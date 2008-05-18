using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
namespace Framework.MyMath
{
    public struct MyQuaternion
    {
        float n;
        MyVector v;


        public MyQuaternion(float n, MyVector v)
        {
            this.n = n;
            this.v = v;
        }
        public MyQuaternion(float n, float x, float y, float z)
        {
            this.n = n;
            this.v = new MyVector(x, y, z);
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(n * n + v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            }
        }
        public MyVector Vector
        {
            get
            {
                return v;
            }
        }
        public float Scalar
        {
            get
            {
                return n;
            }
        }

        public static MyQuaternion operator +(MyQuaternion q1, MyQuaternion q2)
        {
            return new MyQuaternion(q1.n + q2.n, q1.v.X + q2.v.X, q1.v.Y + q2.v.Y, q1.v.Z + q2.v.Z);
        }
        public static MyQuaternion operator -(MyQuaternion q1, MyQuaternion q2)
        {
            return new MyQuaternion(q1.n - q2.n, q1.v.X - q2.v.X, q1.v.Y - q2.v.Y, q1.v.Z - q2.v.Z);
        }

        public MyQuaternion Multiply(float s)
        {
            n *= s;
            v.Multiply(s);
            return this;
        }
        public MyQuaternion Divide(float s)
        {
            n /= s;
            v.Divide(s);
            return this;
        }

        public static MyQuaternion operator *(MyQuaternion q1, MyQuaternion q2)
        {
            return new MyQuaternion(q1.n * q2.n - q1.v.X * q2.v.X - q1.v.Y * q2.v.Y - q1.v.Z * q2.v.Z,
                q1.n * q2.v.X + q1.v.X * q2.n + q1.v.Y * q2.v.Z - q1.v.Z * q2.v.Y,
                q1.n * q2.v.Y + q1.v.Y * q2.n + q1.v.Z * q2.v.X - q1.v.X * q2.v.Z,
                q1.n * q2.v.Z + q1.v.Z * q2.n + q1.v.X * q2.v.Y - q1.v.Y * q2.v.X);

        }
        public static MyQuaternion operator *(MyQuaternion q, float s)
        {
            MyQuaternion nq = new MyQuaternion(q.n, q.v);
            nq.Multiply(s);
            return nq;
        }
        public static MyQuaternion operator *(float s, MyQuaternion q)
        {
            MyQuaternion nq = new MyQuaternion(q.n, q.v);
            nq.Multiply(s);
            return nq;
        }
        public static MyQuaternion operator /(MyQuaternion q, float s)
        {
            MyQuaternion nq = new MyQuaternion(q.n, q.v);
            nq.Divide(s);
            return nq;
        }

        public static MyQuaternion operator *(MyQuaternion q1, MyVector q2)
        {
            return new MyQuaternion(-q1.v.X * q2.X - q1.v.Y * q2.Y - q1.v.Z * q2.Z,
                q1.n * q2.X + q1.v.Y * q2.Z - q1.v.Z * q2.Y,
                q1.n * q2.Y + q1.v.Z * q2.X - q1.v.X * q2.Z,
                q1.n * q2.Z + q1.v.X * q2.Y - q1.v.Y * q2.X);

        }
        public static MyQuaternion operator *(MyVector q1, MyQuaternion q2)
        {
            return new MyQuaternion(-q1.X * q2.v.X - q1.Y * q2.v.Y - q1.Z * q2.v.Z,
                q1.X * q2.n + q1.Y * q2.v.Z - q1.Z * q2.v.Y,
                q1.Y * q2.n + q1.Z * q2.v.X - q1.X * q2.v.Z,
                q1.Z * q2.n + q1.X * q2.v.Y - q1.Y * q2.v.X);

        }
        public static MyQuaternion operator ~(MyQuaternion q)
        {
            return new MyQuaternion(q.n, -q.v);
        }

        public float Angle
        {
            get
            {
                return (float)(2 * Math.Acos(n));
            }
        }
        public MyVector Axis
        {
            get
            {

                return v / Length;
            }
        }

        public MyQuaternion Rotate(MyQuaternion q)
        {
            MyQuaternion t = (q) * (this) * (~q);
            n = t.n;
            v = t.v;
            return this;
        }

        public MyMatrix RotationMatrix
        {
            get
            {
                MyMatrix m = new MyMatrix();
                float x = v.X, y = v.Y, z = v.Z;
                m.M11 = n * n + x * x - y * y - z * z;
                m.M12 = 2 * x * y - 2 * z * n;
                m.M13 = 2 * x * z + 2 * y * n;
                m.M21 = 2 * x * y + 2 * z * n;
                m.M22 = n * n - x * x + y * y - z * z;
                m.M23 = 2 * y * z - 2 * x * n;
                m.M31 = 2 * x * z - 2 * y * n;
                m.M32 = 2 * y * z + 2 * x * n;
                m.M33 = n * n - x * x - y * y + z * z;
                return m;
            }
        }
        public MyVector EulerAngles()
        {
            MyMatrix m = RotationMatrix;
            MyVector v = new MyVector();
            float tmp = Math.Abs(m.M31);
            if (tmp > 0.999999)
            {
                v.X = 0.0f; //roll
                v.Y = (float)(-(Math.PI / 2) * m.M31 / tmp);
                v.Z = (float)Math.Atan2(-m.M12, -m.M31 * m.M13);
            }
            else
            {
                v.X = (float)Math.Atan2(m.M32, m.M33);
                v.Y = (float)(-m.M31);
                v.Z = (float)Math.Atan2(m.M21, m.M11);
            }
            return v;
        }

        public static MyQuaternion FromEulerAngles(float roll, float pitch, float yaw)
        {
            double cyaw, cpitch, croll, syaw, spitch, sroll;
            double cyawcpitch, syawspitch, cyawspitch, syawcpitch;

            cyaw = Math.Cos(yaw / 2);
            cpitch = Math.Cos(pitch / 2);
            croll = Math.Cos(roll / 2);
            syaw = Math.Sin(yaw / 2);
            spitch = Math.Sin(pitch / 2);
            sroll = Math.Sin(roll / 2);

            cyawcpitch = cyaw * cpitch;
            syawspitch = syaw * spitch;
            cyawspitch = cyaw * spitch;
            syawcpitch = syaw * cpitch;

            MyQuaternion q = new MyQuaternion();
            q.n = (float)(cyawcpitch * croll + syawspitch * sroll);
            q.v.X = (float)(cyawcpitch * sroll - syawspitch * croll);
            q.v.Y = (float)(cyawspitch * croll + syawcpitch * sroll);
            q.v.Z = (float)(syawcpitch * croll - cyawspitch * sroll);
            return q;
        }
        public static MyQuaternion FromAxisAngle(float angle, MyVector axis)
        {
            MyQuaternion q = new MyQuaternion();
            q.n = (float)Math.Cos(angle / 2);
            q.v.X = axis.X * ((float)Math.Sin(angle / 2));
            q.v.Y = axis.Y * ((float)Math.Sin(angle / 2));
            q.v.Z = axis.Z * ((float)Math.Sin(angle / 2));
            return q;
        }

        public static explicit operator Quaternion(MyQuaternion mq)
        {
            Quaternion q = new Quaternion(mq.v.X, mq.v.Y, mq.v.Z, mq.n);
            return q;
        }

        public MyQuaternion Normalize()
        {
            float mag = Length;
            if (mag != 0 && mag != 1)
                Divide(mag);

            return this;
        }
    }
}

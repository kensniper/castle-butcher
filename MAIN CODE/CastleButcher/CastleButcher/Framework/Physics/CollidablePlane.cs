using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{

    public class CollisionPlane : ICollisionData
    {
        public readonly MyVector Normal;
        public readonly float D;

        public CollisionPlane(MyVector normal, float D)
        {
            this.Normal = normal;
            this.D = D;
        }
        public static CollisionPlane From3Points(MyVector p1, MyVector p2, MyVector p3)
        {
            MyVector normal = (p2 - p1) ^ (p3 - p1).Normalize();
            CollisionPlane plane = new CollisionPlane(normal, -normal * p1);

            return plane;
        }
        public static CollisionPlane FromNormal(MyVector p, MyVector normal)
        {
            CollisionPlane plane = new CollisionPlane(normal, -normal * p);
            return plane;
        }


        #region ICollisionData Members

        public float BoundingSphereRadius
        {
            get
            {
                return float.PositiveInfinity;
            }
        }
        public bool IntersectPoint(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime,
            MyVector startPoint, MyVector endPoint, float dt)
        {

            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            MyVector rVelocity = (endPoint - startPoint) / dt;
            collisionTime = 0;
            if (startPoint * Normal >= 0 && endPoint * Normal <= 0)
            {
                intersectionNormal = Normal;
                float dist = Normal * (startPoint);
                collisionTime = -dist / (Normal * rVelocity);
                intersectionPoint = startPoint + collisionTime * rVelocity;
                return true;

            }
            return false;
        }
        public bool IntersectSphere(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime,
            MyVector startPoint, MyVector endPoint, float dt, CollisionSphere sphere)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            MyVector rVelocity = (endPoint - startPoint) / dt;

            collisionTime = 0;
            MyVector sPoint = startPoint - Normal * sphere.Radius;
            MyVector ePoint = endPoint - Normal * sphere.Radius;
            if (sPoint * Normal >= 0 && ePoint * Normal <= 0)
            {
                intersectionNormal = Normal;
                float dist = Normal * sPoint;
                collisionTime = -dist / (Normal * rVelocity);
                intersectionPoint = sPoint + collisionTime * rVelocity;

                return true;
            }
            return false;
        }




        public bool IntersectPlane(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint,
            MyVector endPoint, float dt, CollisionPlane plane)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            collisionTime = 0;
            return false;

        }

        public bool IntersectMesh(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint,
            MyVector endPoint, float dt, CollisionMesh mesh)
        {
            bool status = mesh.IntersectPlane(out intersectionPoint, out intersectionNormal, out collisionTime, -startPoint, -endPoint, dt, this);
            intersectionPoint.Add(startPoint);
            intersectionNormal = -intersectionNormal;
            return status;
        }

        public ICollisionData Rotate(MyQuaternion orientation)
        {
            this.Normal.Rotate(orientation);
            return this;
        }

        public object Clone()
        {
            CollisionPlane p = new CollisionPlane(this.Normal, this.D);
            return p;
        }

        #endregion
    }


    //class CollidablePlane : StaticBody
    //{
    //    CollisionPlane m_plane;

    //    public CollidablePlane(MyVector p, MyVector normal)
    //    {
    //        m_plane = CollisionPlane.FromNormal(p, normal);
    //        Position = p;
    //    }
    //    public CollidablePlane(MyVector p1, MyVector p2, MyVector p3)
    //    {
    //        m_plane = CollisionPlane.From3Points(p1, p2, p3);
    //        Position = p1;
    //    }


    //    public override float BoundingSphereRadius
    //    {
    //        get
    //        {
    //            return 10000f;
    //        }
    //    }

    //    public override CollisionDataType CollisionDataType
    //    {
    //        get
    //        {
    //            return CollisionDataType.CollisionPlane;
    //        }
    //    }

    //    public override ICollisionData CollisionData
    //    {
    //        get
    //        {
    //            return m_plane;
    //        }
    //    }

    //    public override float CoefficientOfRestitution
    //    {
    //        get
    //        {
    //            return 1f;
    //        }
    //        set
    //        {

    //        }
    //    }

    //    public override float FrictionCoefficient
    //    {
    //        get
    //        {
    //            return 0.1f;
    //        }
    //        set
    //        {

    //        }

    //    }
    //}
}

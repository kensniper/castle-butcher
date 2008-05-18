using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    public class CollisionSphere : ICollisionData
    {
        public readonly float Radius;

        public CollisionSphere()
        {
        }
        public CollisionSphere(float radius)
        {
            this.Radius = radius;
        }


        #region ICollisionData Members
        public float BoundingSphereRadius
        {
            get
            {
                return this.Radius;
            }
        }

        public bool IntersectPoint(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt)
        {
            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            collisionTime = 0;
            MyVector rVelocity = (endPoint - startPoint) / dt;

            float dx, dy, dz, dVx, dVy, dVz;
            dx = startPoint.X;
            dy = startPoint.Y;
            dz = startPoint.Z;


            dVx = rVelocity.X;
            dVy = rVelocity.Y;
            dVz = rVelocity.Z;


            float a, b, c;
            a = dVx * dVx + dVy * dVy + dVz * dVz;
            b = 2 * (dx * dVx + dy * dVy + dz * dVz);
            c = dx * dx + dy * dy + dz * dz - Radius * Radius;
            float delta = b * b - 4 * a * c;

            if (delta < 0)
            {
                return false;
            }
            else if (delta > 0)
            {
                float sq = (float)Math.Sqrt(delta);
                collisionTime = (-b - sq) / (2 * a);
            }
            else if (delta == 0)
            {
                collisionTime = (-b) / (2 * a);
            }
            float maxt = (endPoint - startPoint).LengthSq / rVelocity.LengthSq;
            if (collisionTime > 0 && collisionTime * collisionTime < maxt)
            {
                intersectionPoint = startPoint + rVelocity * collisionTime;
                intersectionNormal = intersectionPoint;
                intersectionNormal.Normalize();
                return true;
            }
            return false;

        }

        public bool IntersectSphere(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionSphere sphere)
        {

            intersectionPoint = new MyVector();
            intersectionNormal = new MyVector();
            collisionTime = 0;

            MyVector rVelocity = (endPoint - startPoint) / dt;

            float dx, dy, dz, dVx, dVy, dVz;
            dx = startPoint.X;
            dy = startPoint.Y;
            dz = startPoint.Z;


            dVx = rVelocity.X;
            dVy = rVelocity.Y;
            dVz = rVelocity.Z;


            float R = this.Radius + sphere.Radius;
            float a, b, c;
            a = dVx * dVx + dVy * dVy + dVz * dVz;
            b = 2 * (dx * dVx + dy * dVy + dz * dVz);
            c = dx * dx + dy * dy + dz * dz - R * R;
            float delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                return false;
            }
            else if (delta > 0)
            {
                float sq = (float)Math.Sqrt(delta);
                collisionTime = (-b - sq) / (2 * a);
            }
            else if (delta == 0)
            {
                collisionTime = (-b) / (2 * a);
            }
            float maxt = dt;
            if (collisionTime > 0 && collisionTime * collisionTime < maxt)
            {
                intersectionPoint = this.Radius * (startPoint + rVelocity * collisionTime) / R;
                intersectionNormal = intersectionPoint;
                intersectionNormal.Normalize();
                return true;
            }
            return false;
        }

        public bool IntersectPlane(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionPlane plane)
        {
            bool status = plane.IntersectSphere(out intersectionPoint, out intersectionNormal, out collisionTime, -startPoint, -endPoint, dt, this);
            intersectionPoint.Add(startPoint);
            intersectionNormal = -intersectionNormal;
            return status;
        }

        public bool IntersectMesh(out MyVector intersectionPoint, out MyVector intersectionNormal, out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionMesh mesh)
        {
            throw new Exception("powinno byc na odwrot, nie sphere z mesh!!");
            bool status = mesh.IntersectSphere(out intersectionPoint, out intersectionNormal, out collisionTime, -startPoint, -endPoint, dt, this);
            intersectionPoint.Add(startPoint);
            intersectionNormal = -intersectionNormal;
            return status;
        }

        public ICollisionData Rotate(MyQuaternion orientation)
        {
            return this;
        }

        public object Clone()
        {
            CollisionSphere s = new CollisionSphere(this.Radius);
            return s;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Framework.MyMath;

namespace Framework.Physics
{

    public enum ObjectType { Static, PointMass, RigidBody }



    public struct CollisionParameters
    {
        public IPhysicalObject A;
        public IPhysicalObject B;

        public MyVector CollisionPoint;

        public MyVector RelativeVelocity;   //at the point of collision
        public MyVector RelativeAcceleration;   //at the point of collision
        public MyVector CollisionNormal;
        public MyVector CollisionTangent;   //In Friction's direction
    }

    public enum CollisionDataType { None,CollisionPoint, CollisionSphere, CollisionMesh, CollisionPlane, CollisionOctree };
    public interface ICollisionData : ICloneable
    {
        float BoundingSphereRadius
        {
            get;
        }
        ICollisionData Rotate(MyQuaternion orientation);
        bool IntersectPoint(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt);
        bool IntersectSphere(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionSphere sphere);

        bool IntersectPlane(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionPlane plane);
        bool IntersectMesh(out MyVector intersectionPoint, out MyVector intersectionNormal,
            out float collisionTime, MyVector startPoint, MyVector endPoint, float dt, CollisionMesh mesh);

    }

    public class BoundingSphere
    {

        public BoundingSphere()
        {
            Radius = 0;
            //Position = new MyVector();
        }
        public BoundingSphere(float r, MyVector v)
        {
            Radius = r;
            Position = v;
        }
        public float Radius;
        public MyVector Position;
    }
    public struct StaticBodyData
    {
        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Position;

        /// <summary>
        /// Local to Global rotation
        /// </summary>
        public MyQuaternion Orientation;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Velocity;

        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector VelocityBody;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector AngularVelocity;
    }
    public struct PointMassData
    {
        public float Mass;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Position;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Velocity;

        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector VelocityBody;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector AngularVelocity;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Acceleration;

        /// <summary>
        /// Local to Global rotation
        /// </summary>
        public MyQuaternion Orientation;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Forces;

        public static implicit operator StaticBodyData(PointMassData data)
        {
            StaticBodyData d = new StaticBodyData();
            d.Orientation = data.Orientation;
            d.Position = data.Position;
            d.Velocity = data.Velocity;
            d.VelocityBody = data.VelocityBody;
            d.AngularVelocity = data.AngularVelocity;
            return d;
        }

    }

    public struct RigidBodyData
    {
        public float Mass;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyMatrix Inertia;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyMatrix InertiaInverse;
        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyMatrix InertiaInverseGlobal;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Position;
        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Velocity;

        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector VelocityBody;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector AngularVelocity;
        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Acceleration;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector AngularAcceleration;

        /// <summary>
        /// Local to Global rotation
        /// </summary>
        public MyQuaternion Orientation;

        /// <summary>
        /// In Global coordinates
        /// </summary>
        public MyVector Forces;
        /// <summary>
        /// In Local coordinates
        /// </summary>
        public MyVector Moments;

        public static implicit operator PointMassData(RigidBodyData body)
        {
            PointMassData m = new PointMassData();
            m.Acceleration = body.Acceleration;
            m.Forces = body.Forces;
            m.Mass = body.Mass;
            m.Orientation = body.Orientation;
            m.Position = body.Position;
            m.Velocity = body.Velocity;
            m.VelocityBody = body.VelocityBody;
            m.AngularVelocity = body.AngularVelocity;
            return m;
        }

    }
    public struct PhysicalProperties
    {
        public float LinearDragCoefficient;
        public float AngularDragCoefficient;
        public float FrictionCoefficient;
        public float RestitutionCoefficient;

        public PhysicalProperties(float linear, float angular, float friction, float restitution)
        {
            LinearDragCoefficient = linear;
            AngularDragCoefficient = angular;
            FrictionCoefficient = friction;
            RestitutionCoefficient = restitution;
        }
        public PhysicalProperties(float linear, float friction, float restitution)
        {
            LinearDragCoefficient = linear;
            AngularDragCoefficient = 0;
            FrictionCoefficient = friction;
            RestitutionCoefficient = restitution;
        }
        public PhysicalProperties(float friction, float restitution)
        {
            LinearDragCoefficient = 0;
            AngularDragCoefficient = 0;
            FrictionCoefficient = friction;
            RestitutionCoefficient = restitution;
        }
    }

    


    public interface IPhysicalObject
    {
        //event CollisionHandler OnCollision;

        ObjectType Type
        {
            get;
            set;
        }
        MyVector Velocity
        {
            get;
            set;
        }

        MyVector AngularVelocity
        {
            get;
            set;
        }
        MyVector Position
        {
            get;
            set;
        }
        MyQuaternion Orientation
        {
            get;
            set;
        }

        float CurrentTime
        {
            get;
            set;
        }

        float BoundingSphereRadius
        {
            get;
        }
        void Advance(float elapsedTime);
        void MoveToTime(float time);

        CollisionDataType CollisionDataType
        {
            get;
        }
        ICollisionData CollisionData
        {
            get;
        }


        float CoefficientOfRestitution
        {
            get;
        }
        float FrictionCoefficient
        {
            get;
        }

        PhysicalProperties PhysicalProperties
        {
            get;
        }

        MyVector PointToLocalCoords(MyVector v);
        MyVector PointToGlobalCoords(MyVector v);

        MyVector NormalToLocalCoords(MyVector v);
        MyVector NormalToGlobalCoords(MyVector v);

    }
    public interface IDynamicPhysicalObject : IPhysicalObject
    {
        float Mass
        {
            get;
        }

        MyVector Forces
        {
            get;
            set;
        }
    }
}
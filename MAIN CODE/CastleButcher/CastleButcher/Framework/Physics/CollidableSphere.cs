using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;
using Framework.Physics;

namespace Framework.Physics
{
    public class CollidableSphere : RigidBody
    {
        CollisionSphere m_boundingSphere;

        public CollidableSphere(MyVector pos, float radius):base(new PhysicalProperties(0.2f,40,0.3f,1))
        {
            m_boundingSphere = new CollisionSphere(radius);
            m_data.Position = pos;
            m_data.Acceleration = new MyVector(0, 0, 0);
            m_data.AngularAcceleration = new MyVector(0, 0, 0);
            m_data.AngularVelocity = new MyVector(0, 0, 0);
            m_data.Velocity = new MyVector(0, 0, 0);
            m_data.VelocityBody = new MyVector(0, 0, 0);
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            m_data.Mass = radius * radius * radius;
            m_data.Forces = new MyVector(0, 0, 0);
            m_data.Moments = new MyVector(0, 0, 0);

            float moment = 0.4f * m_data.Mass * m_boundingSphere.Radius * m_boundingSphere.Radius;
            m_data.Inertia = new MyMatrix(moment, 0, 0, 0, moment, 0, 0, 0, moment);
            m_data.InertiaInverse = m_data.Inertia.Inverse;

        }



        public override float BoundingSphereRadius
        {
            get
            {
                return m_boundingSphere.Radius;
            }
        }

        public override CollisionDataType CollisionDataType
        {
            get
            {
                return CollisionDataType.CollisionSphere;
            }
        }
        public override ICollisionData CollisionData
        {
            get
            {
                return m_boundingSphere;
            }
        }

       
    }

    public class CollidableSpherePM : PointMass
    {
        CollisionSphere m_boundingSphere;

        public CollidableSpherePM(MyVector pos, float radius):base(new PhysicalProperties(0.2f,0.3f,1))
        {
            m_boundingSphere = new CollisionSphere(radius);
            m_data.Position = pos;
            m_data.Acceleration = new MyVector(0, 0, 0);
            //m_data.AngularAcceleration = new MyVector(0, 0, 0);
            //m_data.AngularVelocity = new MyVector(0, 0, 0);
            m_data.Velocity = new MyVector(0, 0, 0);
            m_data.VelocityBody = new MyVector(0, 0, 0);
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            m_data.Mass = radius * radius * radius;
            m_data.Forces = new MyVector(0, 0, 0);
            //m_data.Moments = new MyVector(0, 0, 0);

            //float moment = 0.4f * m_data.Mass * m_boundingSphere.Radius * m_boundingSphere.Radius;
            //m_data.Inertia = new MyMatrix(moment, 0, 0, 0, moment, 0, 0, 0, moment);
            //m_data.InertiaInverse = m_data.Inertia.Inverse;

        }



        public override float BoundingSphereRadius
        {
            get
            {
                return m_boundingSphere.Radius;
            }
        }

        public override CollisionDataType CollisionDataType
        {
            get
            {
                return CollisionDataType.CollisionSphere;
            }
        }
        public override ICollisionData CollisionData
        {
            get
            {
                return m_boundingSphere;
            }
        }

    }
}

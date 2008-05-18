using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    public abstract class PointMass : IDynamicPhysicalObject
    {
        protected PointMassData m_data;


        protected float m_currentTime;
        protected ObjectType m_type = ObjectType.PointMass;
        protected PhysicalProperties physicalProperties;

        public PointMass()
        {
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
        }
        public PointMass(PhysicalProperties physProp)
        {
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            physicalProperties = physProp;
        }



        public PointMassData PointMassData
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        #region IPhysicalObject Members

        public ObjectType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        public MyVector Velocity
        {
            get
            {
                return m_data.Velocity;
            }
            set
            {
                m_data.Velocity = value;
                m_data.VelocityBody = value;
                m_data.VelocityBody.Rotate(~m_data.Orientation);
            }
        }
        public virtual MyVector AngularVelocity
        {
            get
            {
                return m_data.AngularVelocity;
            }
            set
            {
                m_data.AngularVelocity = value;
            }
        }
        public MyVector Position
        {
            get
            {
                return m_data.Position;
            }
            set
            {
                m_data.Position = value;
            }
        }

        public float CurrentTime
        {
            get
            {
                return m_currentTime;
            }
            set
            {
                m_currentTime = value;
            }
        }


        public abstract float BoundingSphereRadius
        {
            get;
        }

        public virtual MyVector PointToLocalCoords(MyVector v)
        {
            MyVector w;
            w = v - m_data.Position;

            w.Rotate(~m_data.Orientation);
            return w;
        }
        public virtual MyVector PointToGlobalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(m_data.Orientation);
            w.Add(m_data.Position);
            return w;
        }

        public virtual MyVector NormalToLocalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(~m_data.Orientation);
            return w;
        }
        public virtual MyVector NormalToGlobalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(m_data.Orientation);

            return w;
        }

        /// <summary>
        /// Advances in time using current velocity and position
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void Advance(float elapsedTime)
        {

            m_data.Position.Add(m_data.Velocity * elapsedTime);
            m_data.Orientation += m_data.Orientation * (m_data.AngularVelocity * (elapsedTime / 2));
            m_data.Orientation.Normalize();
            m_currentTime += elapsedTime;
        }

        public void MoveToTime(float time)
        {
            Advance(time - m_currentTime);
            m_currentTime = time;
        }

        

        public abstract CollisionDataType CollisionDataType
        {
            get;
        }
        public abstract ICollisionData CollisionData
        {
            get;
        }

        public float CoefficientOfRestitution
        {
            get
            {
                return physicalProperties.RestitutionCoefficient;
            }
        }

        public float FrictionCoefficient
        {
            get
            {
                return physicalProperties.FrictionCoefficient;
            }
        }

        public PhysicalProperties PhysicalProperties
        {
            get
            {
                return physicalProperties;
            }
        }
        public float LinearDragCoefficient
        {
            get
            {
                return physicalProperties.LinearDragCoefficient;
            }
        }



        public MyQuaternion Orientation
        {
            get
            {
                return m_data.Orientation;
            }
            set
            {
                m_data.Orientation = value;
            }
        }

        #endregion

        #region IDynamicPhysicalObject Members

        public float Mass
        {
            get
            {
                return m_data.Mass;
            }
        }

        public MyVector Forces
        {
            get
            {
                return m_data.Forces;
            }
            set
            {
                m_data.Forces = value;
            }
        }

        #endregion
    }
}

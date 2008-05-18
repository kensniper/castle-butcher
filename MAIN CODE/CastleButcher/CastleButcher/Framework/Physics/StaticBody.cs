using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;

namespace Framework.Physics
{
    public abstract class StaticBody : IPhysicalObject
    {
        //protected MyVector m_velocity;
        //protected MyVector m_position;
        //protected MyQuaternion m_orientation;
        protected StaticBodyData m_data;



        protected float m_currentTime;
        protected ObjectType m_type = ObjectType.Static;
        protected PhysicalProperties physicalProperties;

        public StaticBody()
        {
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
        }
        public StaticBody(PhysicalProperties physProp)
        {
            m_data.Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
            physicalProperties = physProp;
        }


        public virtual StaticBodyData StaticBodyData
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
        #region ICollidable Members
        //public event CollisionHandler OnCollision;

        public virtual ObjectType Type
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

        public virtual MyVector Velocity
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

        public virtual MyVector Position
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
            w = v - Position;

            w.Rotate(~Orientation);
            return w;
        }
        public virtual MyVector PointToGlobalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(Orientation);
            w.Add(Position);
            return w;
        }

        public virtual MyVector NormalToLocalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(~Orientation);
            return w;
        }
        public virtual MyVector NormalToGlobalCoords(MyVector v)
        {
            MyVector w = v;
            w.Rotate(Orientation);

            return w;
        }

        /// <summary>
        /// Advances in time using current velocity and position
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void Advance(float elapsedTime)
        {
            Position.Add(Velocity * elapsedTime);
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
    }
}

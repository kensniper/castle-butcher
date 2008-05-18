using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine.Objects
{
    public abstract class TemporaryObject:ITemporaryObject
    {
        #region ITemporaryObject Members

        private float remainingTime;
        public float RemainingTime
        {
            get { return remainingTime; }
            protected set
            {
                remainingTime = value;
            }
        }

        #endregion

        #region IGameObject Members

        public abstract string Name
        {
            get ; 
        }

        public abstract Framework.Physics.IPhysicalObject PhysicalData
        {
            get;
        }

        public abstract CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get;
        }

        public abstract Microsoft.DirectX.Matrix Transform
        {
            get;
        }

        #endregion

        #region IPhysicalObject Members

        public abstract Framework.Physics.ObjectType Type
        {
            get;
            set;
        }

        public abstract Framework.MyMath.MyVector Velocity
        {
            get;
            set;
        }

        public abstract Framework.MyMath.MyVector Position
        {
            get;
            set;
        }

        public abstract Framework.MyMath.MyQuaternion Orientation
        {
            get;
            set;
        }

        public abstract float CurrentTime
        {
            get;
            set;
        }

        public abstract float BoundingSphereRadius
        {
            get;
        }

        public abstract void Advance(float elapsedTime);

        public abstract void MoveToTime(float time);

        public abstract Framework.Physics.CollisionDataType CollisionDataType
        {
            get;
        }

        public abstract Framework.Physics.ICollisionData CollisionData
        {
            get;
        }

        public abstract float CoefficientOfRestitution
        {
            get;
            set;
        }

        public abstract float FrictionCoefficient
        {
            get;
            set;
        }

        public abstract Framework.MyMath.MyVector PointToLocalCoords(Framework.MyMath.MyVector v);

        public abstract Framework.MyMath.MyVector PointToGlobalCoords(Framework.MyMath.MyVector v);

        public abstract Framework.MyMath.MyVector NormalToLocalCoords(Framework.MyMath.MyVector v);

        public abstract Framework.MyMath.MyVector NormalToGlobalCoords(Framework.MyMath.MyVector v);

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            RemainingTime -= timeElapsed;
            return true;
        }

        #endregion
    }
}

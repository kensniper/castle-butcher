using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Framework.MyMath;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Framework;

namespace CastleButcher.GameEngine.Weapons
{
    class Arrow : PointMass, IMissile, ITemporaryObject, IGameObject
    {
        object owner;
        WeaponClass weaponClass;

        float remainingTime;

        public Arrow(object owner, WeaponClass weaponClass, MyVector position, MyQuaternion orientation, MyVector velocity)
            : base(new PhysicalProperties(0, 0, 0))
        {
            this.owner = owner;
            this.weaponClass = weaponClass;

            PointMassData data = this.PointMassData;

            data.Position = position;
            data.Orientation = orientation;
            data.Velocity = velocity;
            data.Mass = 0;
            this.PointMassData = data;
            remainingTime = weaponClass.WeaponParameters.ParticleLifetime;
        }

        #region IMissile Members

        public object Owner
        {
            get { return owner; }
        }

        public int ImpactDamage
        {
            get { return weaponClass.WeaponParameters.HitDamage; }
        }

        #endregion

        public override float BoundingSphereRadius
        {
            get { return 0; }
        }

        public override CollisionDataType CollisionDataType
        {
            get { return CollisionDataType.CollisionPoint; }
        }

        public override ICollisionData CollisionData
        {
            get { return null; }
        }



        #region GameObject Members

        public string Name
        {
            get { return "Arrow from " + weaponClass.Name; }
        }

        public IPhysicalObject PhysicalData
        {
            get { return this; }
        }

        public CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get { return weaponClass.MissileRenderingData; }
        }

        public Matrix Transform
        {
            get { return Matrix.RotationQuaternion((Quaternion)this.Orientation) * Matrix.Translation((Vector3)Position); }
        }

        #endregion

        #region ITemporaryObject Members

        public float RemainingTime
        {
            get { return remainingTime; }
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            remainingTime -= timeElapsed;
            return true;
        }

        #endregion

        #region IMissile Members


        public WeaponClass WeaponClass
        {
            get { return weaponClass; }
        }

        #endregion
    }
}

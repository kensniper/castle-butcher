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
    class SwordBlade : PointMass, IMissile, ITemporaryObject, IGameObject
    {


        public SwordBlade(object owner, WeaponClass weaponClass, MyVector position, MyQuaternion orientation, MyVector velocity)
            : base(new PhysicalProperties(0, 0, 0))
        {
        }
        public override float BoundingSphereRadius
        {
            get { throw new NotImplementedException(); }
        }

        public override CollisionDataType CollisionDataType
        {
            get { throw new NotImplementedException(); }
        }

        public override ICollisionData CollisionData
        {
            get { throw new NotImplementedException(); }
        }

        #region IMissile Members

        public object Owner
        {
            get { throw new NotImplementedException(); }
        }

        public int ImpactDamage
        {
            get { throw new NotImplementedException(); }
        }

        public WeaponClass WeaponClass
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITemporaryObject Members

        public float RemainingTime
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGameObject Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IPhysicalObject PhysicalData
        {
            get { throw new NotImplementedException(); }
        }

        public CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get { throw new NotImplementedException(); }
        }

        public Matrix Transform
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

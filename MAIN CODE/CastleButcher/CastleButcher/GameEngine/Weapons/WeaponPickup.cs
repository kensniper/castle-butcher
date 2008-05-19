using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;
using Framework.MyMath;
using Framework;

namespace CastleButcher.GameEngine.Weapons
{
    public class WeaponPickup : StaticBody,IGameObject,IUpdateable
    {
        WeaponClass weaponClass;

        public WeaponClass WeaponClass
        {
            get { return weaponClass; }
        }
        public WeaponPickup(WeaponClass w)
            : base(new PhysicalProperties(0, 0))
        {
            weaponClass = w;
        }
        public WeaponPickup(WeaponClass w,MyVector pos,MyQuaternion orientation):base(new PhysicalProperties(0,0))
        {
            weaponClass = w;
            StaticBodyData data = this.StaticBodyData;
            data.Position = pos;
            data.Orientation = orientation;
            this.StaticBodyData = data;
            timeSinceLastUse = weaponClass.PickupReuseTime;
        }
        float timeSinceLastUse;

        public bool Ready
        {
            get
            {
                return timeSinceLastUse > weaponClass.PickupReuseTime;
            }
        }

        public void Use()
        {
            timeSinceLastUse = 0;
        }
        public void Reset()
        {
            timeSinceLastUse = weaponClass.PickupReuseTime;
        }

        #region GameObject Members

        public string Name
        {
            get
            {
                return weaponClass.Name;
            }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.RotationQuaternion((Quaternion)this.Orientation)*Matrix.RotationY(rotation) * Matrix.Translation(Position.X, Position.Y, Position.Z);
            }
        }

        public Framework.Physics.IPhysicalObject PhysicalData
        {
            get
            {
                return null;
            }
        }


        public RenderingData RenderingData
        {
            get { return weaponClass.PickupRenderingData; }
        }

        #endregion

        #region StaticBodyMembers

        public override float BoundingSphereRadius
        {
            get { return weaponClass.PickupRenderingData.BoundingSphereRadius; }
        }

        public override CollisionDataType CollisionDataType
        {
            get { return weaponClass.CollisionDataType; }
        }

        public override ICollisionData CollisionData
        {
            get { return weaponClass.CollisionData; }
        }

        
        #endregion

        #region IUpdateable Members

        float rotation = 0;
        public bool Update(float timeElapsed)
        {
            rotation += timeElapsed * 2;
            if (rotation > 6.2832f)
                rotation -= 6.2832f;

            timeSinceLastUse += timeElapsed;
            return true;
        }

        #endregion
    }
}

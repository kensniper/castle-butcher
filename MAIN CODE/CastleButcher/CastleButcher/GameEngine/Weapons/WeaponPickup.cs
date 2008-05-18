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
            

            
        }

        #region GameObject Members

        public string Name
        {
            get
            {
                return weaponClass.Name;
            }
        }

        //public new Framework.MyMath.MyVector Position
        //{
        //    get
        //    {
        //        return this.StaticBodyData.Position;
        //    }
        //    set
        //    {
        //        throw new Exception("The method or operation is not implemented.");
        //    }
        //}
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
            get { return weaponClass.FloatingRenderingData; }
        }

        #endregion

        #region StaticBodyMembers

        public override float BoundingSphereRadius
        {
            get { return weaponClass.FloatingRenderingData.BoundingSphereRadius; }
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
            return true;
        }

        #endregion
    }
}

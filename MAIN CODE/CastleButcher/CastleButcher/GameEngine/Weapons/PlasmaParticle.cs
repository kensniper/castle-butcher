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
    //class PlasmaParticle:PointMass,IMissile,ITemporaryObject,IGameObject
    //{
    //    Ship owner;
    //    WeaponClass weaponClass;

    //    float remainingTime;

    //    public PlasmaParticle(Ship owner, WeaponClass weaponClass, MyVector position, MyQuaternion orientation, MyVector velocity)
    //    {
    //        this.owner = owner;
    //        this.weaponClass = weaponClass;

    //        this.Position = position;
    //        this.Orientation = orientation;
    //        this.Velocity = velocity;
    //        remainingTime = weaponClass.WeaponParameters.ParticleLifetime;
    //    }

    //    #region IMissile Members

    //    public Ship Owner
    //    {
    //        get { return owner; }
    //    }

    //    public int ImpactDamage
    //    {
    //        get { return weaponClass.WeaponParameters.HitDamage; }
    //    }

    //    #endregion

    //    public override float BoundingSphereRadius
    //    {
    //        get { return 0; }
    //    }

    //    public override CollisionDataType CollisionDataType
    //    {
    //        get { return CollisionDataType.Point; }
    //    }

    //    public override ICollisionData CollisionData
    //    {
    //        get { return null; }
    //    }

    //    public override float CoefficientOfRestitution
    //    {
    //        get
    //        {
    //            return 0;
    //        }
    //        set
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }

    //    public override float FrictionCoefficient
    //    {
    //        get
    //        {
    //            return 0;
    //        }
    //        set
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }

    //    public override float LinearDragCoefficient
    //    {
    //        get
    //        {
    //            return 0;
    //        }
    //        set
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }

    //    #region GameObject Members

    //    public string Name
    //    {
    //        get { return "Plasma particle from "+weaponClass.Name; }
    //    }

    //    public IPhysicalObject PhysicalData
    //    {
    //        get { return null; }
    //    }

    //    public CastleButcher.GameEngine.Resources.RenderingData RenderingData
    //    {
    //        get { return weaponClass.MissileRenderingData; }
    //    }

    //    public Matrix Transform
    //    {
    //        get { return Matrix.RotationQuaternion((Quaternion)this.Orientation)*Matrix.Translation((Vector3)Position); }
    //    }

    //    #endregion

    //    #region ITemporaryObject Members

    //    public float RemainingTime
    //    {
    //        get { return remainingTime; }
    //    }

    //    #endregion

    //    #region IUpdateable Members

    //    public bool Update(float timeElapsed)
    //    {
    //        remainingTime -= timeElapsed;
    //        return true;
    //    }

    //    #endregion
    //}
}

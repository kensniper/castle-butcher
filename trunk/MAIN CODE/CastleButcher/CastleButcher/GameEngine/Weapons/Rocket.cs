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
    ///// <summary>
    ///// Klasa pocisków rakietowych
    ///// </summary>
    ///// <remarks>Pociski rakietowe mo¿na zniszczyæ!!</remarks>
    //public class Rocket : PointMass, DestroyableObj, IMissile
    //{
    //    #region GameObject Members

    //    public string Name
    //    {
    //        get
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }

       
    //    public Matrix Transform
    //    {
    //        get
    //        {
    //            return Matrix.Translation(Position.X, Position.Y, Position.Z);
    //        }
    //        //set
    //        //{
    //        //    throw new Exception("The method or operation is not implemented.");
    //        //}
    //    }
    //    public Framework.Physics.IPhysicalObject PhysicalData
    //    {
    //        get
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }


    //    public RenderingData RenderingData
    //    {
    //        get { throw new Exception("The method or operation is not implemented."); }
    //    }


    //    #endregion

    //    #region DestroyableObj Members

    //    public event ObjectDestructionHandler OnDestroyed;

    //    public event HitDataHandler OnHit;

    //    public DestroyableObjState ArmorState
    //    {
    //        get
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //        set
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }


    //    public void TakeDamage(int damage)
    //    {
    //        throw new Exception("The method or operation is not implemented.");
    //    }

    //    #endregion

    //    public CastleButcher.GameEngine.AutoPilot AutoPilot
    //    {
    //        get
    //        {
    //            throw new System.NotImplementedException();
    //        }
    //    }

    //    /// <summary>
    //    /// Obiekt AutoPilot u¿ywany do kierowania pociskiem
    //    /// </summary>
    //    private AutoPilot pilot;

    //    #region IMissile Members

    //    public int ImpactDamage
    //    {
    //        get
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }
    //    }
    //    public Ship Owner
    //    {
    //        get { throw new Exception("The method or operation is not implemented."); }
    //    }

    //    #endregion

    //    public Rocket(int damage, EngineParameters parameters)
    //    {
    //        throw new System.NotImplementedException();
    //    }


    //    #region PointMass members

    //    public override float BoundingSphereRadius
    //    {
    //        get { throw new Exception("The method or operation is not implemented."); }
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
    //    #endregion
    //}

    //class Rocket : PointMass, IMissile, DestroyableObj,IFlyingObject,IUpdateable,ITemporaryObject
    //{
    //    Ship owner;
    //    WeaponClass weaponClass;
    //    DestroyableObjState armorState;
    //    Ship target;

    //    public Rocket(Ship owner,Ship target, WeaponClass weaponClass, MyVector position, MyQuaternion orientation, MyVector velocity)
    //    {
    //        this.owner = owner;
    //        this.target = target;
    //        this.weaponClass = weaponClass;

    //        this.Position = position;
    //        this.Orientation = orientation;
    //        this.Velocity = velocity;

    //        this.sdev = new SteeringDevice2(this);
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
    //            return 1;
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



    //    #region DestroyableObj Members

    //    public event ObjectDestructionHandler OnDestroyed;

    //    public event HitDataHandler OnHit;

    //    public DestroyableObjState ArmorState
    //    {
    //        get { return armorState; }
    //    }

    //    public void TakeDamage(int damage)
    //    {
    //        armorState.Shield -= damage;
    //        if (armorState.Shield < 0)
    //        {
    //            armorState.Hull += armorState.Shield;
    //            armorState.Shield = 0;
    //        }
    //        if (armorState.Hull <= 0 && OnDestroyed != null)
    //            OnDestroyed(this);
    //    }

    //    #endregion

    //    #region GameObject Members

    //    public string Name
    //    {
    //        get { return "Rocket from " + weaponClass.Name; }
    //    }

    //    public IPhysicalObject PhysicalData
    //    {
    //        get { return this; }
    //    }

    //    public CastleButcher.GameEngine.Resources.RenderingData RenderingData
    //    {
    //        get { return weaponClass.MissileRenderingData; }
    //    }

    //    public Microsoft.DirectX.Matrix Transform
    //    {
    //        get { return Matrix.RotationQuaternion((Quaternion)this.Orientation) * Matrix.Translation((Vector3)Position); }
    //    }

    //    #endregion

    //    #region IFlyingObject Members


    //    public PlayerMovementParameters EngineParameters
    //    {
    //        get { return weaponClass.FlyingObjectParameters; }
    //    }

    //    SteeringDevice2 sdev;
    //    public SteeringDevice2 SteeringDevice
    //    {
    //        get { return sdev; }
    //    }

    //    #endregion

    //    #region IUpdateable Members

    //    float lifetime = 0;
    //    public bool Update(float timeElapsed)
    //    {
    //        lifetime += timeElapsed;
    //        return true;
    //    }

    //    #endregion

    //    #region ITemporaryObject Members

    //    public float RemainingTime
    //    {
    //        get { return Math.Max(0, weaponClass.WeaponParameters.ParticleLifetime - lifetime); }
    //    }

    //    #endregion
    //}

}
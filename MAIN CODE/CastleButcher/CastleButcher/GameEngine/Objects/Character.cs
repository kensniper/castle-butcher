using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Framework.Physics;
using Framework.MyMath;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public class Character : PointMass, DestroyableObj, IUpdateable, IMovingObject
    {
        protected Player player;
        protected CharacterClass characterClass;

        public virtual CharacterClass CharacterClass
        {
            get { return characterClass; }
        }
        protected bool hasGroundContact = false;

        public bool HasGroundContact
        {
            get { return hasGroundContact; }
            set { hasGroundContact = value; }
        }
        public Player Player
        {
            get { return player; }
            set { player = value; }
        }
        CharacterController characterController;

        public CharacterController CharacterController
        {
            get { return characterController; }
        }

        
        public Character(Player player, CharacterClass characterClass, MyVector position, MyQuaternion orientation)
        {
            this.player = player;
            this.characterClass = characterClass;

            characterController = new CharacterController(this);
            

            Reset(position, orientation);

        }

        public void Reset(MyVector position, MyQuaternion orientation)
        {
            PointMassData data = this.PointMassData;
            data.Position = position;
            data.Orientation = orientation;
            //this.armorState.Shield = shipClass.ArmorParameters.ShieldAmount;
            //this.armorState.Hull = shipClass.ArmorParameters.HullAmount;

            data.Velocity = new MyVector(0, 0, 0);
            //this.AutoPilot.SetVelocity = 0;

            data.Acceleration = new MyVector(0, 0, 0);
            data.AngularVelocity = new MyVector(0, 0, 0);

            data.Mass = 1000;


            this.PointMassData = data;
            characterController.Reset(this);
            DestroyableObjState hp = ArmorState;
            hp.Hp = 100;
            hp.Shield = 0;
            armorState = hp;

            //this.Weapons.Reset();

            //this.weaponEnergy = this.ShipClass.ArmorParameters.WeaponEnergy;
        }

        public virtual ICollisionData WalkingCollisionData
        {
            get
            {
                if (player.IsAlive)
                {
                    return characterClass.WalkingCollisionData;
                }
                throw new Exception();
            }
        }
        public MyVector LookDirection
        {
            get { return (new MyVector(0, 0, -1).Rotate(this.Orientation)); }
        }

        public override float BoundingSphereRadius
        {
            get { return CollisionData.BoundingSphereRadius; }
        }

        public override CollisionDataType CollisionDataType
        {
            get
            {
                //spectating
                if (player.IsAlive)
                {
                    return characterClass.CollisionDataType;
                }
                throw new Exception();
            }
        }

        public override ICollisionData CollisionData
        {
            get
            {
                //spectating
                if (player.IsAlive)
                {
                    return characterClass.CollisionData;
                }
                throw new Exception();
            }
        }

        #region DestroyableObj Members

        public event ObjectDestructionHandler OnDestroyed;

        public event HitDataHandler OnHit;

        DestroyableObjState armorState;
        public DestroyableObjState ArmorState
        {
            get { return armorState; }
        }

        public void TakeDamage(int damage)
        {


            if (armorState.Shield > 0)
                armorState.Shield -= damage;
            if (armorState.Shield < 0)
            {
                armorState.Hp += armorState.Shield;
                armorState.Shield = 0;
            }
            if (OnHit != null)
            {
                OnHit(this, damage);
            }

            if (armorState.Hp < 0)
            {

                armorState.Hp = 0;
                if (OnDestroyed != null)
                {
                    OnDestroyed(this);
                }
            }

            

        }

        #endregion

        #region IGameObject Members

        public string Name
        {
            get { return player.Name; }
        }

        public Framework.Physics.IPhysicalObject PhysicalData
        {
            get
            {
                return this;
            }
        }

        public virtual CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get
            {
                if (player.IsAlive)
                {
                    return characterClass.RenderingData;
                }
                //else
                return null;
            }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.RotationQuaternion((Quaternion)this.Orientation) * Matrix.Translation(Position.X, Position.Y, Position.Z);
            }
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            characterController.Update(timeElapsed);
            return true;
        }

        #endregion

        #region IFlyingObject Members


        public virtual PlayerMovementParameters MovementParameters
        {
            get 
            { 
                if(player.IsAlive)
                    return characterClass.MovementParameters;
                throw new Exception();
            }
        }


        #endregion
    }

    public class SpectatingCharacter : Character
    {
        CollisionMesh spectatingCollisionData;
        CollisionMesh walkingCollisionData;
        RenderingData spectatingRenderingData;
        static PlayerMovementParameters SpectatorMovementParameters = 
            new PlayerMovementParameters(0, GameSettings.Default.SpectatorSpeed);

        public SpectatingCharacter(Player player,CharacterClass characterClass, MyVector position, MyQuaternion orientation)
            : base(player,characterClass, position, orientation)
        {
            spectatingCollisionData = Resources.ResourceCache.Instance.GetCollisionMesh("walkingMesh.cm");
            spectatingRenderingData = ResourceCache.Instance.GetRenderingData("walkingMesh.x");
            walkingCollisionData = ResourceCache.Instance.GetCollisionMesh("walkingPoint.cm");

            PointMassData data = this.PointMassData;
            data.Mass = 0;
            this.PointMassData = data;
            //this.Cha
        }
        public override CharacterClass CharacterClass
        {
            get
            {
                return null;
            }
        }
        public override CollisionDataType CollisionDataType
        {
            get
            {
                return CollisionDataType.CollisionMesh;
            }
        }
        public override ICollisionData CollisionData
        {
            get
            {
                return spectatingCollisionData;
            }
        }
        public override RenderingData RenderingData
        {
            get
            {
                return spectatingRenderingData;
            }
        }

        public override PlayerMovementParameters MovementParameters
        {
            get
            {
                return SpectatorMovementParameters;
            }
        }

        public override ICollisionData WalkingCollisionData
        {
            get
            {
                return walkingCollisionData;
            }
        }

    }
}

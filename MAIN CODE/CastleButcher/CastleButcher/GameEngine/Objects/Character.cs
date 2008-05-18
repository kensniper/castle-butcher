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
        //CollisionMesh spectatingCollisionData;
        protected CollisionMesh walkingCollisionData;
        //RenderingData spectatingRenderingData;
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
        public Character(CharacterClass characterClass, MyVector position, MyQuaternion orientation)
        {
            this.characterClass = characterClass;
            //spectatingCollisionData = new CollisionSphere(3);

            //this.steeringDevice = new SteeringDevice2(this);
            //this.autoPilot = new AutoPilot();



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

            //this.Weapons.Reset();

            //this.weaponEnergy = this.ShipClass.ArmorParameters.WeaponEnergy;
        }

        public MyVector LookDirection
        {
            get { return (new MyVector(0, 0, -1).Rotate(this.Orientation)); }
        }

        public override float BoundingSphereRadius
        {
            get { return 0; }
        }

        public override CollisionDataType CollisionDataType
        {
            get
            {
                //spectating
                if (!player.IsAlive)
                {
                    //return CollisionDataType.CollisionMesh;
                }
                throw new Exception();
            }
        }

        public override ICollisionData CollisionData
        {
            get
            {
                //spectating
                if (!player.IsAlive)
                {
                    //return spectatingCollisionData;
                }
                throw new Exception();
            }
        }

        #region DestroyableObj Members

        public event ObjectDestructionHandler OnDestroyed;

        public event HitDataHandler OnHit;

        public DestroyableObjState ArmorState
        {
            get { throw new NotImplementedException(); }
        }

        public void TakeDamage(int damage)
        {
            throw new NotImplementedException();
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
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public virtual CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get
            {
                if (!player.IsAlive)
                {
                    //return spectatingRenderingData;
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
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            return true;
        }

        #endregion

        #region IFlyingObject Members


        public virtual PlayerMovementParameters MovementParameters
        {
            get { return characterClass.MovementParameters; }
        }

        //public SteeringDevice2 SteeringDevice
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion
    }

    public class SpectatingCharacter : Character
    {
        CollisionMesh spectatingCollisionData;
        CollisionMesh walkingCollisionData;
        RenderingData spectatingRenderingData;
        static PlayerMovementParameters SpectatorMovementParameters = 
            new PlayerMovementParameters(0, GameSettings.Default.SpectatorSpeed);

        public SpectatingCharacter(CharacterClass characterClass, MyVector position, MyQuaternion orientation)
            : base(characterClass, position, orientation)
        {
            spectatingCollisionData = Resources.ResourceCache.Instance.GetCollisionMesh("walkingMesh.cm");
            spectatingRenderingData = ResourceCache.Instance.GetRenderingData("walkingMesh.x");

            //this.Cha
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

    }
}

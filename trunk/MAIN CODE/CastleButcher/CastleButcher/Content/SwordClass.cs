using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine.Weapons;
using CastleButcher.GameEngine.Resources;
using CastleButcher.GameEngine;
using Framework.MyMath;
using Framework.Physics;


namespace CastleButcher.Content
{
    class SwordClass:WeaponClass
    {
        public SwordClass()
        {
            name = "Sword";
            this.gameTeam = GameTeam.Knights;
            this.pickupRD = null;
            this.missileRD = null;
            this.weaponType = WeaponType.Melee;
            this.weaponParams = new WeaponInfo(30, 0.5f, 100);
            this.flyingObjectParameters = new CastleButcher.GameEngine.PlayerMovementParameters(0, 500);
            this.collisionDataType = Framework.Physics.CollisionDataType.CollisionSphere;
            this.collisionData = new CollisionSphere(4);
            this.ammoInBox = 1;
            this.pickupReuseTime=2;

        }

        public override CastleButcher.GameEngine.IMissile GetMissile(object owner, Framework.MyMath.MyVector position, Framework.MyMath.MyQuaternion orientation)
        {
            IMissile missile = new Arrow(owner, this, position, orientation, (new MyVector(0, 0, -1)).Rotate(orientation) * this.flyingObjectParameters.MaxVelocity);
            
            return missile;
        }
    }
}

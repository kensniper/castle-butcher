using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine.Weapons;
using CastleButcher.GameEngine.Resources;
using CastleButcher.GameEngine;
using Framework.MyMath;
using Framework.Physics;
using Framework;


namespace CastleButcher.Content
{
    class FireballClass : WeaponClass
    {
        AnimatedMapData animatedMapData;
        public FireballClass()
        {
            animatedMapData = ResourceCache.Instance.GetAnimatedTexture("fireball.amd");
            name = "Fireball";
            this.gameTeam = GameTeam.Assassins;
            this.pickupRD = ResourceCache.Instance.GetRenderingData("crossbowContainer.x");
            this.missileRD = ResourceCache.Instance.GetRenderingData("fireball.x");
            this.cameraRD = ResourceCache.Instance.GetRenderingData("handWithFireball.x");

            this.weaponType = WeaponType.Ranged;
            this.weaponParams = new WeaponInfo(100, 0.5f, 100);
            this.flyingObjectParameters = new CastleButcher.GameEngine.PlayerMovementParameters(0, 200);
            this.collisionDataType = Framework.Physics.CollisionDataType.CollisionSphere;
            this.collisionData = new CollisionSphere(4);
            this.ammoInBox = 1;
            this.pickupReuseTime = 2;

            MapAnimation anim = animatedMapData.GetAnimationInstance();
            anim.Start();
            anim.Loop = true;
            missileRD.MeshMaterials[0] = new AnimatedMaterialData(null, null, null, anim, new Microsoft.DirectX.Direct3D.Material());
            cameraRD.MeshMaterials[0] = missileRD.MeshMaterials[0];
            GM.AppWindow.AddUpdateableItem(anim as IUpdateable);

        }

        public override CastleButcher.GameEngine.IMissile GetMissile(object owner, Framework.MyMath.MyVector position, Framework.MyMath.MyQuaternion orientation)
        {
            IMissile missile = new Grenade(0, owner, this, position, orientation, (new MyVector(0, 0, -1)).Rotate(orientation) * this.flyingObjectParameters.MaxVelocity);

            return missile;
        }
    }
}

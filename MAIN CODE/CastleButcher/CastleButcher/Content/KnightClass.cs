using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;
using Framework.MyMath;
using Microsoft.DirectX;

namespace CastleButcher.Content
{
    public class KnightClass : CharacterClass
    {
        PlayerMovementParameters movementParameters = new PlayerMovementParameters(35, GameSettings.Default.KnightSpeed);
        CollisionMesh collisionData;
        CollisionMesh walkingCollisionData;
        //CollisionMesh hitMesh;
        RenderingData renderingData;
        RenderingData rdWithCrossbow;
        RenderingData rdWithGrenade;
        RenderingData deadRd;

        public override RenderingData DeadRd
        {
            get { return deadRd; }

        }

        public KnightClass()
        {
            collisionData = ResourceCache.Instance.GetCollisionMesh("knightHitMesh2.cm");
            List<MyVector> tempPoints = new List<MyVector>();
            for (int i = 0; i < collisionData.m_hardPoints.Length; i++)
            {
                if (collisionData.m_hardPoints[i].Y < 0)
                    tempPoints.Add(collisionData.m_hardPoints[i]);
            }
            collisionData.m_hardPoints = tempPoints.ToArray();
            walkingCollisionData = ResourceCache.Instance.GetCollisionMesh("knightWalkingPoint.cm");
            //hitMesh = ResourceCache.Instance.GetCollisionMesh("knightHitMesh.cm");
            //            renderingData = ResourceCache.Instance.GetRenderingData("knightStanding.x");
            Matrix rot = Matrix.RotationY((float)Math.PI);
            renderingData = ResourceCache.Instance.GetRenderingData("knightStanding.x");
            //renderingData.CustomTransform = rot;
            rdWithCrossbow = ResourceCache.Instance.GetRenderingData("knightWithCrossbow.x");
            rdWithGrenade = ResourceCache.Instance.GetRenderingData("knightWithGrenade.x");
            deadRd = ResourceCache.Instance.GetRenderingData("knightDead.x");

            float r = renderingData.BoundingSphereRadius;


            Matrix tr = Matrix.Translation(0, -3, 0);
            deadRd.CustomTransform = tr;

        }
        public override PlayerMovementParameters MovementParameters
        {
            get { return movementParameters; }
        }

        public override Framework.Physics.ICollisionData CollisionData
        {
            get { return collisionData; }
        }

        public override Framework.Physics.CollisionDataType CollisionDataType
        {
            get { return CollisionDataType.CollisionMesh; }
        }

        public override Framework.Physics.ICollisionData WalkingCollisionData
        {
            get { return walkingCollisionData; }
        }

        public override CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get { return renderingData; }
        }

        public override GameTeam GameTeam
        {
            get { return GameTeam.Knights; }
        }

        public override RenderingData CharRenderingData(Character character)
        {
            if (character.Player.IsAlive)
            {
                if (character.Weapons.CurrentWeapon != null)
                {
                    if (character.Weapons.CurrentWeapon.WeaponClass is CrossbowClass)
                    {
                        return rdWithCrossbow;
                    }
                    else if (character.Weapons.CurrentWeapon.WeaponClass is GrenadeClass)
                        return rdWithGrenade;
                }
                return RenderingData;
            }
            else
                return deadRd;



        }
    }
}

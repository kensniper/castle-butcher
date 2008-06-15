using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;
using Framework.MyMath;

namespace CastleButcher.Content
{
    public class AssassinClass:CharacterClass
    {
        PlayerMovementParameters movementParameters=new PlayerMovementParameters(50,GameSettings.Default.AssassinSpeed);
        CollisionMesh collisionData;
        CollisionMesh walkingCollisionData;
        //CollisionMesh hitMesh;
        RenderingData renderingData;

        public AssassinClass()
        {
            collisionData = ResourceCache.Instance.GetCollisionMesh("assassinHitMesh.cm");
            List<MyVector> tempPoints = new List<MyVector>();
            for (int i = 0; i < collisionData.m_hardPoints.Length; i++)
            {
                if (collisionData.m_hardPoints[i].Y < 0)
                    tempPoints.Add(collisionData.m_hardPoints[i]);
            }
            collisionData.m_hardPoints = tempPoints.ToArray();
            walkingCollisionData = ResourceCache.Instance.GetCollisionMesh("assassinWalkingPoint.cm");
            //hitMesh = ResourceCache.Instance.GetCollisionMesh("assassinHitMesh.cm");
            
            renderingData = ResourceCache.Instance.GetRenderingData("assassinWalkingMesh2.x");
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
            get { return GameTeam.Assassins; }
        }
    }
}

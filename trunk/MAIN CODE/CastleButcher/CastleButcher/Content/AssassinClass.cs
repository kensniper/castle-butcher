using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.Content
{
    public class AssassinClass:CharacterClass
    {
        PlayerMovementParameters movementParameters=new PlayerMovementParameters(50,GameSettings.Default.AssassinSpeed);
        ICollisionData collisionData;
        ICollisionData walkingCollisionData;
        RenderingData renderingData;

        public AssassinClass()
        {
            collisionData = ResourceCache.Instance.GetCollisionMesh("walkingMesh.cm");
            walkingCollisionData = ResourceCache.Instance.GetCollisionMesh("walkingPoint.cm");
            renderingData = ResourceCache.Instance.GetRenderingData("walkingMesh.x");
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

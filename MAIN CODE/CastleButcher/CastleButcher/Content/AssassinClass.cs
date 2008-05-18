using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;

namespace CastleButcher.Content
{
    public class AssassinClass:CharacterClass
    {
        PlayerMovementParameters movementParameters=new PlayerMovementParameters(35,50);
        public override PlayerMovementParameters MovementParameters
        {
            get { return movementParameters; }
        }

        public override Framework.Physics.ICollisionData CollisionData
        {
            get { throw new NotImplementedException(); }
        }

        public override Framework.Physics.CollisionDataType CollisionDataType
        {
            get { throw new NotImplementedException(); }
        }

        public override Framework.Physics.ICollisionData WalkingCollisionData
        {
            get { throw new NotImplementedException(); }
        }

        public override CastleButcher.GameEngine.Resources.RenderingData RenderingData
        {
            get { throw new NotImplementedException(); }
        }
    }
}

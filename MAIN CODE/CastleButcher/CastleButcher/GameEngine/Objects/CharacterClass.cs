using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public abstract class CharacterClass
    {
        ushort networkId;

        public ushort NetworkId
        {
            get { return networkId; }
            set { networkId = value; }
        }


        public abstract PlayerMovementParameters MovementParameters
        {
            get;
        }
        public abstract ICollisionData CollisionData
        {
            get;
        }
        public abstract CollisionDataType CollisionDataType
        {
            get;
        }
        public abstract ICollisionData WalkingCollisionData
        {
            get;
        }
        public abstract RenderingData RenderingData
        {
            get;
        }

        public abstract GameTeam GameTeam
        {
            get;
        }

        public abstract RenderingData CharRenderingData(Character character);
    }
}

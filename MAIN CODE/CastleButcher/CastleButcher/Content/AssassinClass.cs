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
    }
}

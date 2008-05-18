using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastleButcher.GameEngine
{
    public abstract class CharacterClass
    {

        public abstract PlayerMovementParameters MovementParameters
        {
            get;
        }
    }
}

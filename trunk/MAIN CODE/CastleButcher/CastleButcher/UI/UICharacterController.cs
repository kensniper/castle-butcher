using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;

namespace CastleButcher.UI
{
    class UICharacterController:CharacterController
    {
        UIPlayer player;
        public UICharacterController(Character character)
            : base(character)
        {
            player = (UIPlayer)character.Player;
            updateDirections = true;
        }

        public override float Velocity
        {
            get
            {
                return base.Velocity;
            }
            set
            {
                base.Velocity = value;
                if (value == 1)
                    player.PlayerControl.CameraShaker.StartWalking();
                if(value==0)
                    player.PlayerControl.CameraShaker.StopWalking();
            }
        }
    }
}

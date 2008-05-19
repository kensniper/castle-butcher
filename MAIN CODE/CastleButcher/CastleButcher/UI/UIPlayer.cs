using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;

namespace CastleButcher.UI
{
    class UIPlayer : Player
    {
        PlayerControlLayer playerControl;

        internal PlayerControlLayer PlayerControl
        {
            get { return playerControl; }
            set { playerControl = value; }
        }
        public UIPlayer(string name, CharacterClass characterClass)
            : base(name, characterClass)
        {
            playerControl = null;
        }
        public UIPlayer(PlayerControlLayer control, string name, CharacterClass characterClass)
            : base(name, characterClass)
        {
            playerControl = control;
        }

        public override void OnWeaponPickup(CastleButcher.GameEngine.Weapons.WeaponPickup weapon)
        {
            base.OnWeaponPickup(weapon);
            if (weapon.WeaponClass.WeaponType == CastleButcher.GameEngine.Weapons.WeaponType.Ranged)
            {
                playerControl.RenderCrosshair = true;
            }
        }
        public override void OnDestroyed(DestroyableObj destroyedObj)
        {
            base.OnDestroyed(destroyedObj);
            Spectating = true;
        }
        public override void OnRespawned()
        {
            base.OnRespawned();

            Spectating = false;
        }
    }
}

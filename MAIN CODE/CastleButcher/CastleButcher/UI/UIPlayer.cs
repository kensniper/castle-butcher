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

        public PlayerControlLayer PlayerControl
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



        }

        public void OnWeaponChanged(CastleButcher.GameEngine.Weapons.WeaponClass newWeapon)
        {
            if (newWeapon == null || newWeapon.WeaponType != CastleButcher.GameEngine.Weapons.WeaponType.Ranged)
            {
                playerControl.RenderCrosshair = false;
            }
            else
            {
                playerControl.RenderCrosshair = true;
            }
        }

        public override void OnWeaponPickup(CastleButcher.GameEngine.Weapons.WeaponPickup weapon)
        {
            base.OnWeaponPickup(weapon);
            //if (weapon.WeaponClass.WeaponType == CastleButcher.GameEngine.Weapons.WeaponType.Ranged)
            //{
            //    playerControl.RenderCrosshair = true;
            //}
        }
        public override void OnDestroyed(DestroyableObj destroyedObj)
        {
            base.OnDestroyed(destroyedObj);
            Spectating = true;
            playerControl.RenderCrosshair = false;
        }
        public override void OnRespawned()
        {
            base.OnRespawned();

            Spectating = false;

            playerControl.ShowPlayerList = false;

            currentChar.Weapons.OnWeaponChanged += new CastleButcher.GameEngine.Weapons.WeaponChangedEvent(OnWeaponChanged);
        }
    }
}

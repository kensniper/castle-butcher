using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine.Weapons
{
    public class WeaponState:IUpdateable
    {
        WeaponClass weaponClass;

        public WeaponClass WeaponClass
        {
            get { return weaponClass; }
        }

        bool ready;
        float timeSinceLastUse;
        int ammo;

        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }


        public WeaponState(WeaponClass weaponClass)
        {
            this.weaponClass = weaponClass;
            ammo = weaponClass.AmmoInBox;
        }


        public bool Ready
        {
            get
            {
                if (weaponClass.WeaponType == WeaponType.Rocket)
                {
                    return ready && Ammo > 0;
                }
                else
                    return ready;
            }
        }


        public bool Use()
        {
            if (ready)
            {
                timeSinceLastUse = 0;
                ready = false;
                return true;
            }
            return false;
        }



        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            timeSinceLastUse += timeElapsed;
            if (ready == false && timeSinceLastUse>=weaponClass.WeaponParameters.FireRate)
            {
                ready = true;
            }
            return true;
        }

        #endregion
    }
}

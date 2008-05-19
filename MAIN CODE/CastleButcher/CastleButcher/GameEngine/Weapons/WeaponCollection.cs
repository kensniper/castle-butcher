using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine.Weapons
{

    public class WeaponWithAmmo
    {
        public WeaponWithAmmo(WeaponClass weaponClass, int ammo)
        {
            WeaponClass = weaponClass;
            Ammo = ammo;
        }
        public WeaponClass WeaponClass;
        public int Ammo;
    }

    public class WeaponCollection:IUpdateable
    {
        List<WeaponState> melee = new List<WeaponState>();
        List<WeaponState> ranged = new List<WeaponState>();

        bool autoChangeWeapons = true;

        public bool AutoChangeWeapons
        {
            get { return autoChangeWeapons; }
            set { autoChangeWeapons = value; }
        }

        int currentMelee=-1;

        public WeaponState CurrentMelee
        {
            get
            {
                if (currentMelee >= 0 && currentMelee < melee.Count)
                    return melee[currentMelee];
                else
                    return null;
            }
        }
        int currentRanged=-1;

        public WeaponState CurrentRanged
        {
            get 
            {
                if (currentRanged >= 0 && currentRanged < ranged.Count)
                    return ranged[currentRanged];
                else
                    return null;
            }
        }


        public void SelectNextMelee()
        {
            if (melee.Count == 0) return;
            currentMelee++;
            if (currentMelee >= melee.Count)
            {
                currentMelee = 0;
            }
        }
        public void SelectPreviousMelee()
        {
            if (melee.Count == 0) return;
            currentMelee--;
            if (currentMelee <=0)
            {
                currentMelee = melee.Count-1;
            }
        }

        public void SelectNextRanged()
        {
            if (ranged.Count == 0) return;
            currentRanged++;
            if (currentRanged >= ranged.Count)
            {
                currentRanged = 0;
            }
        }
        public void SelectPreviousRanged()
        {
            if (ranged.Count == 0) return;
            currentRanged--;
            if (currentRanged <= 0)
            {
                currentRanged = ranged.Count - 1;
            }
        }

        public WeaponCollection()
        {

        }

        public void AddWeapon(WeaponClass weaponClass)
        {
            if (weaponClass.WeaponType == WeaponType.Melee)
            {
                
                for (int i = 0; i < melee.Count; i++)
                {
                    if (melee[i].WeaponClass == weaponClass)
                        return;
                }
                melee.Add(new WeaponState(weaponClass));
                if (autoChangeWeapons || currentMelee == -1)
                {
                    currentMelee = melee.Count - 1;
                }

            }
            else if (weaponClass.WeaponType == WeaponType.Ranged)
            {
                int index=-1;
                for(int i=0;i<ranged.Count;i++)
                {
                    if(ranged[i].WeaponClass==weaponClass)
                    {
                        index=i;
                        break;
                    }
                }
                if (index == -1)
                {
                    ranged.Add(new WeaponState(weaponClass));
                    if (autoChangeWeapons || currentMelee == -1)
                        currentRanged = ranged.Count - 1;
                }
                else
                {
                    ranged[index].Ammo += ranged[index].WeaponClass.AmmoInBox;
                }
            }
        }

        public void RemoveWeapon(WeaponClass weaponClass)
        {
            if (weaponClass.WeaponType == WeaponType.Melee)
            {
                for (int i = 0; i < melee.Count; i++)
                {
                    if (melee[i].WeaponClass == weaponClass)
                    {
                        
                        if (CurrentMelee.WeaponClass == weaponClass)
                        {
                            SelectPreviousMelee();
                        }
                        melee.RemoveAt(i);
                    }
                }
               
            }
            else if (weaponClass.WeaponType == WeaponType.Ranged)
            {
                for (int i = 0; i < ranged.Count; i++)
                {
                    if (ranged[i].WeaponClass == weaponClass)
                    {
                        
                        if (CurrentRanged.WeaponClass == weaponClass)
                        {
                            SelectPreviousRanged();
                        }
                        ranged.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Reset()
        {
            melee.Clear();
            ranged.Clear();
            currentMelee = -1;
            currentRanged = -1;
        }

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            if (CurrentMelee != null)
            {
                CurrentMelee.Update(timeElapsed);
            }
            if (CurrentRanged != null)
            {
                CurrentRanged.Update(timeElapsed);
            }
            return true;
        }

        #endregion
    }
}

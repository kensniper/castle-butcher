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

    public class WeaponCollection : IUpdateable
    {
        List<WeaponState> melee = new List<WeaponState>();
        List<WeaponState> ranged = new List<WeaponState>();
        WeaponType currentType = WeaponType.Melee;

        public WeaponType CurrentWeaponType
        {
            get { return currentType; }
            set { currentType = value; }
        }

        public WeaponState CurrentWeapon
        {
            get
            {
                if (currentType == WeaponType.Melee)
                {
                    return CurrentMelee;
                }
                else
                    return CurrentRanged;
            }
        }
        bool autoChangeWeapons = true;

        public bool AutoChangeWeapons
        {
            get { return autoChangeWeapons; }
            set { autoChangeWeapons = value; }
        }

        int currentMelee = -1;

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
        int currentRanged = -1;

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
            currentType = WeaponType.Melee;
            if (melee.Count == 0)
            {
                SelectNextRanged();
            }
            currentMelee++;
            if (currentMelee >= melee.Count)
            {
                currentMelee = 0;
            }
        }
        public void SelectPreviousMelee()
        {
            currentType = WeaponType.Melee;
            if (melee.Count == 0)
            {
                SelectNextRanged();
            }
            currentMelee--;
            if (currentMelee <= 0)
            {
                currentMelee = melee.Count - 1;
            }
        }

        public void SelectNextRanged()
        {
            currentType = WeaponType.Ranged;
            if (ranged.Count == 0)
            {
                SelectNextMelee();
            }
            currentRanged++;
            if (currentRanged >= ranged.Count)
            {
                currentRanged = 0;
            }
        }
        public void SelectPreviousRanged()
        {
            currentType = WeaponType.Ranged;
            if (ranged.Count == 0)
            {
                SelectPreviousMelee();
            }
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
                if (currentType == WeaponType.Melee)
                {
                    if (autoChangeWeapons || currentMelee == -1)
                    {
                        currentMelee = melee.Count - 1;
                    }
                }
                else
                {
                    if (autoChangeWeapons || currentRanged == -1)
                    {
                        currentMelee = melee.Count - 1;
                    }
                    currentType = WeaponType.Melee;

                }

            }
            else if (weaponClass.WeaponType == WeaponType.Ranged)
            {
                int index = -1;
                for (int i = 0; i < ranged.Count; i++)
                {
                    if (ranged[i].WeaponClass == weaponClass)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                {
                    ranged.Add(new WeaponState(weaponClass));

                    if (currentType == WeaponType.Ranged)
                    {
                        if (autoChangeWeapons || currentMelee == -1)
                            currentRanged = ranged.Count - 1;
                    }
                    else
                    {
                        if (autoChangeWeapons || currentMelee == -1)
                        {
                            currentRanged = ranged.Count - 1;
                        }
                        currentType = WeaponType.Ranged;

                    }
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
using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine.Weapons
{

    //public class WeaponWithAmmo
    //{
    //    public WeaponWithAmmo(WeaponClass weaponClass, int ammo)
    //    {
    //        WeaponClass = weaponClass;
    //        Ammo = ammo;
    //    }
    //    public WeaponClass WeaponClass;
    //    public int Ammo;
    //}

    public class WeaponCollection:IUpdateable
    {
        List<WeaponState> lasers = new List<WeaponState>();
        List<WeaponState> rockets = new List<WeaponState>();

        bool autoChangeWeapons = true;

        public bool AutoChangeWeapons
        {
            get { return autoChangeWeapons; }
            set { autoChangeWeapons = value; }
        }

        int currentLaser=-1;

        public WeaponState CurrentLaser
        {
            get
            {
                if (currentLaser >= 0 && currentLaser < lasers.Count)
                    return lasers[currentLaser];
                else
                    return null;
            }
        }
        int currentRocket=-1;

        public WeaponState CurrentRocket
        {
            get 
            {
                if (currentRocket >= 0 && currentRocket < rockets.Count)
                    return rockets[currentRocket];
                else
                    return null;
            }
        }


        public void SelectNextLaser()
        {
            if (lasers.Count == 0) return;
            currentLaser++;
            if (currentLaser >= lasers.Count)
            {
                currentLaser = 0;
            }
        }
        public void SelectPreviousLaser()
        {
            if (lasers.Count == 0) return;
            currentLaser--;
            if (currentLaser <=0)
            {
                currentLaser = lasers.Count-1;
            }
        }

        public void SelectNextRocket()
        {
            if (rockets.Count == 0) return;
            currentRocket++;
            if (currentRocket >= rockets.Count)
            {
                currentRocket = 0;
            }
        }
        public void SelectPreviousRocket()
        {
            if (rockets.Count == 0) return;
            currentRocket--;
            if (currentRocket <= 0)
            {
                currentRocket = rockets.Count - 1;
            }
        }

        public WeaponCollection()
        {

        }

        public void AddWeapon(WeaponClass weaponClass)
        {
            if (weaponClass.WeaponType == WeaponType.Laser)
            {
                
                for (int i = 0; i < lasers.Count; i++)
                {
                    if (lasers[i].WeaponClass == weaponClass)
                        return;
                }
                lasers.Add(new WeaponState(weaponClass));
                if (autoChangeWeapons || currentLaser == -1)
                {
                    currentLaser = lasers.Count - 1;
                }

            }
            else if (weaponClass.WeaponType == WeaponType.Rocket)
            {
                int index=-1;
                for(int i=0;i<rockets.Count;i++)
                {
                    if(rockets[i].WeaponClass==weaponClass)
                    {
                        index=i;
                        break;
                    }
                }
                if (index == -1)
                {
                    rockets.Add(new WeaponState(weaponClass));
                    if (autoChangeWeapons || currentLaser == -1)
                        currentRocket = rockets.Count - 1;
                }
                else
                {
                    rockets[index].Ammo += rockets[index].WeaponClass.AmmoInBox;
                }
            }
        }

        public void RemoveWeapon(WeaponClass weaponClass)
        {
            if (weaponClass.WeaponType == WeaponType.Laser)
            {
                for (int i = 0; i < lasers.Count; i++)
                {
                    if (lasers[i].WeaponClass == weaponClass)
                    {
                        
                        if (CurrentLaser.WeaponClass == weaponClass)
                        {
                            SelectPreviousLaser();
                        }
                        lasers.RemoveAt(i);
                    }
                }
               
            }
            else if (weaponClass.WeaponType == WeaponType.Rocket)
            {
                for (int i = 0; i < rockets.Count; i++)
                {
                    if (rockets[i].WeaponClass == weaponClass)
                    {
                        
                        if (CurrentRocket.WeaponClass == weaponClass)
                        {
                            SelectPreviousRocket();
                        }
                        rockets.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Reset()
        {
            lasers.Clear();
            rockets.Clear();
            currentLaser = -1;
            currentRocket = -1;
        }

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            if (CurrentLaser != null)
            {
                CurrentLaser.Update(timeElapsed);
            }
            if (CurrentRocket != null)
            {
                CurrentRocket.Update(timeElapsed);
            }
            return true;
        }

        #endregion
    }
}

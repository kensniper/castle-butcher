using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine.Weapons
{
    /// <summary>
    /// Informacje o parametrach broni
    /// </summary>
    public struct WeaponInfo
    {
        public WeaponInfo(int hitDamage, float fireRate,float lifetime)
        {
            HitDamage = hitDamage;
            FireRate = fireRate;
            ParticleLifetime = lifetime;
        }
        /// <summary>
        /// Obra¿enia od jednego pocisku
        /// </summary>
        public int HitDamage;
        /// <summary>
        /// [Hz]maksymalna czêstotliwoœæ strza³ów
        /// </summary>
        public float FireRate;

        public float ParticleLifetime;

        /// <summary>
        /// Obra¿enia w ci¹gu sekundy
        /// </summary>
        public int DamagePerSecond
        {
            get
            {
                return (int)(HitDamage * FireRate);
            }
        }

      
    }
}

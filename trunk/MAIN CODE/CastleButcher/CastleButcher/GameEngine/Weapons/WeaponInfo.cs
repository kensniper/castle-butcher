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
        public WeaponInfo(int hitDamage, int energyCost, float fireRate,float lifetime)
        {
            HitDamage = hitDamage;
            EnergyCost = energyCost;
            FireRate = fireRate;
            ParticleLifetime = lifetime;
        }
        /// <summary>
        /// [J]Obra¿enia od jednego pocisku
        /// </summary>
        public int HitDamage;
        /// <summary>
        /// [J]Ile energii potrzeba na wystrzelenie jednego pocisku
        /// </summary>
        public int EnergyCost;
        /// <summary>
        /// [Hz]maksymalna czêstotliwoœæ strza³ów
        /// </summary>
        public float FireRate;

        public float ParticleLifetime;

        /// <summary>
        /// [J]Obra¿enia w ci¹gu sekundy
        /// </summary>
        public int DamagePerSecond
        {
            get
            {
                return (int)(HitDamage * FireRate);
            }
        }

        /// <summary>
        /// [J]Ile zjada energii w ci¹gu sekundy
        /// </summary>
        public int EnergyPerSecond
        {
            get
            {
                return (int)(EnergyCost * FireRate);
            }
        }

        /// <summary>
        /// Stosunek obrazen do szkotu energetycznego
        /// </summary>
        public int DmgToEnergyRatio
        {
            get
            {
                return HitDamage / EnergyCost;
            }
        }
    }
}

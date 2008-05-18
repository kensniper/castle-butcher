using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{

    /// <summary>
    /// Podstawowe parametry ogólne(nie chwilowe) lataj¹cego obiektu, takie jak maksymalna prêdkoœæ, przyspieszeie itp
    /// </summary>
    public struct PlayerMovementParameters
    {
        public PlayerMovementParameters(float acc, float maxSpeed)
        {
            Acceleration = acc;
            MaxVelocity = maxSpeed;
        }
        /// <summary>
        /// [m/s*s]Przyspieszenie, z jakim odbywa siê zmiana prêdkoœci(zarówno podczas rozpêdzania jak i hamowania)
        /// </summary>
        public float Acceleration;
        /// <summary>
        /// [m/s]Maksymalna prêdkoœæ obiektu
        /// </summary>
        public float MaxVelocity;
    }
}

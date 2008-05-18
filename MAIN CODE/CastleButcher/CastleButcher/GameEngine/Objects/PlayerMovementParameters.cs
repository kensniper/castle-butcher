using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{

    /// <summary>
    /// Podstawowe parametry og�lne(nie chwilowe) lataj�cego obiektu, takie jak maksymalna pr�dko��, przyspieszeie itp
    /// </summary>
    public struct PlayerMovementParameters
    {
        public PlayerMovementParameters(float acc, float maxSpeed)
        {
            Acceleration = acc;
            MaxVelocity = maxSpeed;
        }
        /// <summary>
        /// [m/s*s]Przyspieszenie, z jakim odbywa si� zmiana pr�dko�ci(zar�wno podczas rozp�dzania jak i hamowania)
        /// </summary>
        public float Acceleration;
        /// <summary>
        /// [m/s]Maksymalna pr�dko�� obiektu
        /// </summary>
        public float MaxVelocity;
    }
}

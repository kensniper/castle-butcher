using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Struktura opisująca podstawowe parametry statku związane z wytrzymałościa i szybkością regeneracji tarcz
    /// </summary>
    public struct CharacterParameters
    {
        /// <summary>
        /// Punkty tarczy
        /// </summary>
        public int ShieldAmount;
        /// <summary>
        /// Punkty życia
        /// </summary>
        public int HitPoints;
    }
}

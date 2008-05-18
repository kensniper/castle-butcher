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
        /// [J]Wytrzymałość tarczy
        /// </summary>
        public int ShieldAmount;
        /// <summary>
        /// [J]Wytrzymałość kadłuba
        /// </summary>
        public int HitPoints;
    }
}

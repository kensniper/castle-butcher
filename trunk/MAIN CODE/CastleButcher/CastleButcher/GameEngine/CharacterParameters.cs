using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Struktura opisuj�ca podstawowe parametry statku zwi�zane z wytrzyma�o�cia i szybko�ci� regeneracji tarcz
    /// </summary>
    public struct CharacterParameters
    {
        /// <summary>
        /// [J]Wytrzyma�o�� tarczy
        /// </summary>
        public int ShieldAmount;
        /// <summary>
        /// [J]Wytrzyma�o�� kad�uba
        /// </summary>
        public int HitPoints;
    }
}

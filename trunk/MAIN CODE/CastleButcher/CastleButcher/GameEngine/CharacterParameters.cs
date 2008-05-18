using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Struktura opisuj¹ca podstawowe parametry statku zwi¹zane z wytrzyma³oœcia i szybkoœci¹ regeneracji tarcz
    /// </summary>
    public struct CharacterParameters
    {
        /// <summary>
        /// [J]Wytrzyma³oœæ tarczy
        /// </summary>
        public int ShieldAmount;
        /// <summary>
        /// [J]Wytrzyma³oœæ kad³uba
        /// </summary>
        public int HitPoints;
    }
}

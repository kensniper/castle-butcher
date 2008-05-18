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
        /// Punkty tarczy
        /// </summary>
        public int ShieldAmount;
        /// <summary>
        /// Punkty ¿ycia
        /// </summary>
        public int HitPoints;
    }
}

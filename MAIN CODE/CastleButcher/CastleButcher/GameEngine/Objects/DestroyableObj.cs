using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Stan obiektu, który mo¿na zniszczyæ. Ka¿dy obiekt ma 2 parametry, Hull i Shield, oba wyra¿one w J. Oznaczaj¹ one,
    /// ile energii dana sk³adowa mo¿e zaabsorbowaæ. Tarcza(Shield) jest sk³adow¹ odnawialn¹, choæ mo¿e byæ stale równa 0(np asteroida itp.).
    /// Kiedy obiekt zostaje uszkodzony (np pociskiem) od jego stanu odejmowana jest odpowiednia iloœæ energii,
    /// przy czym sk³adowa kad³uba(Hull) zostaje zmniejszana dopiero, gdy tarcza ma wartoœæ 0. Gdy Hull osi¹gnie 0 obiekt zostaje zniszczony.
    /// </summary>
    public struct DestroyableObjState
    {
        /// <summary>
        /// Punkty tarczy danego obiektu
        /// </summary>
        public int Shield;
        /// <summary>
        /// Punty ¿ycia
        /// </summary>
        public int Hp;
    }

    /// <summary>
    /// Interfejs bazowy dla wszystkich obiektów, które mo¿na zniszczyæ(statki, rakiety, asteroidy itp)
    /// </summary>
    public interface DestroyableObj : IGameObject
    {
        /// <summary>
        /// Event wywo³ywany, gdy obiekt ulegnie zniszczeniu
        /// </summary>
        event ObjectDestructionHandler OnDestroyed;

        /// <summary>
        /// Event wywo³ywany, gdy obiekt odniesie jakieœ obra¿enia
        /// </summary>
        event HitDataHandler OnHit;
    
        /// <summary>
        /// Stan obiektu, tzn obiekt typu DestroyableObjState opisuj¹cy, ile jeszcze obra¿eñ mo¿e
        /// wytrzymaæ zanim ulegnie zniszczeniu
        /// </summary>
        DestroyableObjState ArmorState
        {
            get;
        }

        /// <summary>
        /// Uszkadza obiekt
        /// </summary>
        /// <param name="damage">Iloœæ obra¿eñ zadanych obiektowi</param>
        void TakeDamage(int damage);
    }
}

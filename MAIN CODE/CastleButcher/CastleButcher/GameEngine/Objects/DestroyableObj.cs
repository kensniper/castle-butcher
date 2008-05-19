using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Stan obiektu, kt�ry mo�na zniszczy�. Ka�dy obiekt ma 2 parametry, Hull i Shield, oba wyra�one w J. Oznaczaj� one,
    /// ile energii dana sk�adowa mo�e zaabsorbowa�. Tarcza(Shield) jest sk�adow� odnawialn�, cho� mo�e by� stale r�wna 0(np asteroida itp.).
    /// Kiedy obiekt zostaje uszkodzony (np pociskiem) od jego stanu odejmowana jest odpowiednia ilo�� energii,
    /// przy czym sk�adowa kad�uba(Hull) zostaje zmniejszana dopiero, gdy tarcza ma warto�� 0. Gdy Hull osi�gnie 0 obiekt zostaje zniszczony.
    /// </summary>
    public struct DestroyableObjState
    {
        /// <summary>
        /// Punkty tarczy danego obiektu
        /// </summary>
        public int Shield;
        /// <summary>
        /// Punty �ycia
        /// </summary>
        public int Hp;
    }

    /// <summary>
    /// Interfejs bazowy dla wszystkich obiekt�w, kt�re mo�na zniszczy�(statki, rakiety, asteroidy itp)
    /// </summary>
    public interface DestroyableObj : IGameObject
    {
        /// <summary>
        /// Event wywo�ywany, gdy obiekt ulegnie zniszczeniu
        /// </summary>
        event ObjectDestructionHandler OnDestroyed;

        /// <summary>
        /// Event wywo�ywany, gdy obiekt odniesie jakie� obra�enia
        /// </summary>
        event HitDataHandler OnHit;
    
        /// <summary>
        /// Stan obiektu, tzn obiekt typu DestroyableObjState opisuj�cy, ile jeszcze obra�e� mo�e
        /// wytrzyma� zanim ulegnie zniszczeniu
        /// </summary>
        DestroyableObjState ArmorState
        {
            get;
        }

        /// <summary>
        /// Uszkadza obiekt
        /// </summary>
        /// <param name="damage">Ilo�� obra�e� zadanych obiektowi</param>
        void TakeDamage(int damage);
    }
}

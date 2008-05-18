using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using CastleButcher.GameEngine.Resources;
using Framework;
using Framework.MyMath;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Wyj�tek zg�aszany g��wnie przy wczytywaniu danych, gdy format jakiego� pliku jest niepoprawny
    /// </summary>
    class FileFormatException : Exception
    {
        public FileFormatException(string ex)
            : base(ex)
        {
        }
    }
    /// <summary>
    /// Delegat do eventa informuj�cego o odniesieniu obra�e� przez jaki� obiekt
    /// </summary>
    /// <param name="hitObject">Obiekt, kt�ry odni�s� obra�enia</param>
    /// <param name="damage">[J]Ilo�� obra�e�</param>
    public delegate void HitDataHandler(DestroyableObj hitObject, int damage);

    /// <summary>
    /// Delegat do eventa, kt�ry informuje o zniszczeniu obiektu
    /// </summary>
    /// <param name="destroyedObj">Obiekt, kt�ry zosta� zniszczony</param>
    public delegate void ObjectDestructionHandler(DestroyableObj destroyedObj);

    public enum ObjectType
    {
        Weapon,
        Player,
        Mesh,
        Respawn,
        Powerup
    }
    public enum GameTeam { Assassins, Knights };


    public interface IGameObject
    {
        /// <summary>
        /// Nazwa obiektu
        /// </summary>
        string Name
        {
            get;
            //set;
        }

        ///// <summary>
        ///// Pozycja Obiektu
        ///// </summary>
        //new Framework.MyMath.MyVector Position
        //{
        //    get;
        //    //set;
        //}

        /// <summary>
        /// Dane fizyczne obiektu uzywane przez podsystem fizyki
        /// </summary>
        /// <remarks>Mo�e by� null, wtedy obiekt jest "wirtualny", tzn tylko sie pokazuje ale nei da sie z nim np zderzy�</remarks>
        IPhysicalObject PhysicalData
        {
            get;
            //set;
        }

        /// <summary>
        /// "Grafika" obiektu, je�li null to obiekt nie jest renderowany
        /// </summary>
        RenderingData RenderingData
        {
            get;
        }

        /// <summary>
        /// Macierz przekszta�caj�ca uk�ad do lokalnego uk�adu obiektu(chyba w 2 strone:P)
        /// </summary>
        Microsoft.DirectX.Matrix Transform
        {
            get;
            //set;
        }
    }

    /// <summary>
    /// Interfejs dla szeroko rozumianego pocisku(rakieta, cz�tka wystrzelona przez dzialo itp)
    /// </summary>
    public interface IMissile
    {
        /// <summary>
        /// Obiekt, kt�ry wystrzeli� pocisk(u�ywany przy zderzeniach, �eby przez przypadek nie strzeli� sam w siebie)
        /// </summary>
        object Owner
        {
            get;
        }

        /// <summary>
        /// Ilo�� obra�e� jakie wyrz�dza pocisk przy uderzeniu
        /// </summary>
        int ImpactDamage
        {
            get;
        }
    }

    public interface ITemporaryObject:IUpdateable
    {
        float RemainingTime
        {
            get;
        }
    }


    public interface IMovingObject
    {
        MyVector Position
        {
            get;
            set;
        }
        MyVector Velocity
        {
            get;
            set;
        }
        MyQuaternion Orientation
        {
            get;
            set;
        }

        PlayerMovementParameters MovementParameters
        {
            get;
        }


    }


}

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
    /// Wyj¹tek zg³aszany g³ównie przy wczytywaniu danych, gdy format jakiegoœ pliku jest niepoprawny
    /// </summary>
    class FileFormatException : Exception
    {
        public FileFormatException(string ex)
            : base(ex)
        {
        }
    }
    /// <summary>
    /// Delegat do eventa informuj¹cego o odniesieniu obra¿eñ przez jakiœ obiekt
    /// </summary>
    /// <param name="hitObject">Obiekt, który odniós³ obra¿enia</param>
    /// <param name="damage">[J]Iloœæ obra¿eñ</param>
    public delegate void HitDataHandler(DestroyableObj hitObject, int damage);

    /// <summary>
    /// Delegat do eventa, który informuje o zniszczeniu obiektu
    /// </summary>
    /// <param name="destroyedObj">Obiekt, który zosta³ zniszczony</param>
    public delegate void ObjectDestructionHandler(DestroyableObj destroyedObj);

    public enum ObjectType
    {
        Weapon,
        Player,
        Mesh,
        Respawn,
        Powerup
    }
    public enum GameTeam { Assassins, Knights, Both };


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
        /// <remarks>Mo¿e byæ null, wtedy obiekt jest "wirtualny", tzn tylko sie pokazuje ale nei da sie z nim np zderzyæ</remarks>
        IPhysicalObject PhysicalData
        {
            get;
            //set;
        }

        /// <summary>
        /// "Grafika" obiektu, jeœli null to obiekt nie jest renderowany
        /// </summary>
        RenderingData RenderingData
        {
            get;
        }

        /// <summary>
        /// Macierz przekszta³caj¹ca uk³ad do lokalnego uk³adu obiektu(chyba w 2 strone:P)
        /// </summary>
        Microsoft.DirectX.Matrix Transform
        {
            get;
            //set;
        }
    }

    /// <summary>
    /// Interfejs dla szeroko rozumianego pocisku(rakieta, cz¹tka wystrzelona przez dzialo itp)
    /// </summary>
    public interface IMissile
    {
        /// <summary>
        /// Obiekt, który wystrzeli³ pocisk(u¿ywany przy zderzeniach, ¿eby przez przypadek nie strzeli³ sam w siebie)
        /// </summary>
        object Owner
        {
            get;
        }

        /// <summary>
        /// Iloœæ obra¿eñ jakie wyrz¹dza pocisk przy uderzeniu
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

using System;
using System.Collections.Generic;
using System.Text;
using CastleButcher.GameEngine.Weapons;
using Framework.Physics;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Klasa pilota(gracza), zawiera imie typ(komputer-cz³owiek) itp
    /// </summary>
    public class Player
    {
        protected string name;
        protected Character currentChar;
        protected CharacterClass characterClass;
        protected bool isAlive;

        public CharacterController CharacterController
        {
            get { return CurrentCharacter.CharacterController; }
        }

        private int frags = 0;

        public int Frags
        {
            get { return frags; }
        }
        private int deaths = 0;

        public int Deaths
        {
            get { return deaths; }
        }

        public Player()
        {
            name = "Noname";
            currentChar = null;
            isAlive = false;
            characterClass = null;
        }

        public Player(string name, CharacterClass characterClass)
            : base()
        {
            this.name = name;
            this.characterClass = characterClass;
        }

        public CharacterClass CharacterClass
        {
            get { return characterClass; }
            set { characterClass = value; }
        }

        public Character CurrentCharacter
        {
            get { return currentChar; }
            set
            {
                currentChar = value;
                if (value != null)
                {
                    currentChar.OnDestroyed += new ObjectDestructionHandler(OnDestroyed);
                    currentChar.Player = this;
                }
            }
        }

        public bool Spectating
        {
            get
            {
                return IsAlive;
            }
            set
            {
                if (value)
                {
                    this.currentChar = new SpectatingCharacter(this,this.CharacterClass, this.CurrentCharacter.Position,
                        this.CurrentCharacter.Orientation);
                    this.CharacterController.Flying = true;

                }
                else
                {
                    //this.currentChar = new Character(this,this.CharacterClass, this.CurrentCharacter.Position,
                    //    this.CurrentCharacter.Orientation);
                    this.CharacterController.Flying = false;
                }
            }
        }

        
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public virtual PlayerType Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        //events
        public virtual void OnDestroyed(DestroyableObj destroyedObj)
        {
            Spectating = true;
            IsAlive = false;
            deaths++;
            //World.Instance.RespawnPilot(this);
        }
        public virtual void OnRespawned()
        {
            isAlive = true;
            Spectating = false;
        }
        public virtual void OnStaticCollision(IGameObject staticObject, CollisionParameters parameters)
        {
            if(parameters.CollisionNormal.Y>0.5)
                CurrentCharacter.HasGroundContact = true;
        }

        public virtual void OnCharacterCollision(Character character, CollisionParameters parameters)
        {

        }

        public virtual void OnMissileHit(IMissile missile, CollisionParameters parameters)
        {

        }

        public virtual void OnWeaponPickup(WeaponPickup weapon)
        {

        }

        public virtual void OnEnemyDestroyed(Player enemy)
        {
            frags++;
        }

        
    }

    /// <summary>
    /// Rodzaj gracza
    /// </summary>
    public enum PlayerType
    {
        Local,
        Remote,
        CPU,
    }
}

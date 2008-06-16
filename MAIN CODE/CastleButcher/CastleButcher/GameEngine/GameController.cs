using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Framework.MyMath;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Weapons;
using CastleButcher.Content;

namespace CastleButcher.GameEngine
{
    public enum GameStatus { WaitingToConnect, WaitingForStart, InProgress };
    public abstract class GameController : IUpdateable
    {
        protected GameStatus gameStatus;
        protected int knightsScore;
        protected int assassinsScore;

        protected ushort assassinsID = 13;
        protected ushort knightsID = 39;

        protected Player player;

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        public GameTeam GetTeamByID(ushort id)
        {
            if (id == assassinsID)
                return GameTeam.Assassins;
            if (id == knightsID)
                return GameTeam.Knights;
            else
                return GameTeam.Both;
        }

        public Player GetPlayerByID(ushort id)
        {
            for (int i = 0; i < World.Instance.Players.Count; i++)
            {
                if (World.Instance.Players[i].NetworkId == id)
                    return World.Instance.Players[i];
            }
            return null;
        }

        public WeaponClass GetWeaponClassByID(UDPClientServerCommons.Constants.WeaponEnumeration weapon)
        {
            if (player.CharacterClass.GameTeam == GameTeam.Assassins)
            {
                if (weapon == UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow)
                    return ObjectCache.Instance.GetWeapon("Pipe");
                if (weapon == UDPClientServerCommons.Constants.WeaponEnumeration.Sword)
                    return ObjectCache.Instance.GetWeapon("Fireball");

            }
            else if (player.CharacterClass.GameTeam == GameTeam.Knights)
            {
                if (weapon == UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow)
                    return ObjectCache.Instance.GetWeapon("Crossbow");
                if (weapon == UDPClientServerCommons.Constants.WeaponEnumeration.Sword)
                    return ObjectCache.Instance.GetWeapon("Grenade");

            }
            return null;
        }
        public UDPClientServerCommons.Constants.WeaponEnumeration GetWeaponIDByClass(WeaponClass wc)
        {
            if (player.CharacterClass.GameTeam == GameTeam.Assassins)
            {
                if (wc is PipeClass)
                    return  UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow;
                if (wc is FireballClass)
                    return UDPClientServerCommons.Constants.WeaponEnumeration.Sword;

            }
            else if (player.CharacterClass.GameTeam == GameTeam.Knights)
            {
                if (wc is CrossbowClass)
                    return UDPClientServerCommons.Constants.WeaponEnumeration.CrossBow;
                if (wc is GrenadeClass)
                    return UDPClientServerCommons.Constants.WeaponEnumeration.Sword;

            }
            return  UDPClientServerCommons.Constants.WeaponEnumeration.None;
        }

        public MyQuaternion LookOrientationFromDirection(MyVector direction)
        {
            Matrix rot = Matrix.LookAtRH(new Vector3(0, 0, 0), new Vector3(direction.X, direction.Y, direction.Z), new Vector3(0, 1, 0));
            rot.Invert();
            Quaternion q = Quaternion.RotationMatrix(rot);
            return (MyQuaternion)q;
        }
        public MyQuaternion WalkOrientationFromDirection(MyVector direction)
        {
            Matrix rot = Matrix.LookAtRH(new Vector3(0, 0, 0), new Vector3(direction.X, 0, direction.Z), new Vector3(0, 1, 0));
            rot.Invert();
            Quaternion q = Quaternion.RotationMatrix(rot);
            return (MyQuaternion)q;
        }
        public GameStatus GameStatus
        {
            get
            {
                return gameStatus;
            }
        }


        public abstract bool IsLocal
        {
            get;
        }

        public virtual void Init()
        {
            World.Instance.OnObjectAdded += new ObjectEventHandler(OnObjectAdded);
            World.Instance.OnObjectRemoved += new ObjectEventHandler(OnObjectRemoved);
            World.Instance.OnPlayerKilled += new PlayerEventHandler(OnPlayerKilled);
            World.Instance.OnPlayerRespawned += new PlayerEventHandler(OnPlayerRespawned);
            World.Instance.OnPlayerRemoved += new PlayerEventHandler(OnPlayerRemoved);

            knightsScore = 0;
            assassinsScore = 0;
        }

        public abstract void AddPlayer(Player player);
        public abstract void RemovePlayer(Player player);

        public abstract void ChangePlayerTeam(Player player, GameTeam team);
        public int AssassinsScore
        {
            get
            {
                return assassinsScore;
            }
        }
        public int KnightsScore
        {
            get
            {
                return knightsScore;
            }
        }

        public abstract void StartGame();
        public abstract void BeginRound();
        public abstract void EndRound();
        public virtual void EndRound(GameTeam defeatedTeam)
        {
            if (defeatedTeam == GameTeam.Assassins)
                knightsScore++;
            else if (defeatedTeam == GameTeam.Knights)
                assassinsScore++;
        }
        public abstract void EndGame();


        protected abstract void OnPlayerAdded(Player player);
        protected abstract void OnPlayerRemoved(Player player);
        protected abstract void OnPlayerRespawned(Player player);
        protected abstract void OnPlayerKilled(Player player);
        protected abstract void OnCharacterAdded(Character character);
        protected abstract void OnCharacterRemoved(Character character);
        protected abstract void OnObjectAdded(IGameObject obj);
        protected abstract void OnObjectRemoved(IGameObject obj);



        public abstract bool Update(float timeElapsed);

    }
}

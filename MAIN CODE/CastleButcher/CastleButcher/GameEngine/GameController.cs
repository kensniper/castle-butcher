using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastleButcher.GameEngine
{
    public enum GameStatus { WaitingToConnect, WaitingForStart, InProgress };
    public abstract class GameController
    {
        protected GameStatus gameStatus;
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
        }

        public abstract void AddPlayer(Player player);
        public abstract void RemovePlayer(Player player);

        public abstract void ChangePlayerTeam(Player player,GameTeam team);

        public abstract void BeginRound();
        public abstract void EndRound();
        public abstract void EndRound(GameTeam defeatedTeam);


        protected abstract void OnPlayerAdded(Player player);
        protected abstract void OnPlayerRemoved(Player player);
        protected abstract void OnPlayerRespawned(Player player);
        protected abstract void OnPlayerKilled(Player player);
        protected abstract void OnCharacterAdded(Character character);
        protected abstract void OnCharacterRemoved(Character character);
        protected abstract void OnObjectAdded(IGameObject obj);
        protected abstract void OnObjectRemoved(IGameObject obj);



    }
}

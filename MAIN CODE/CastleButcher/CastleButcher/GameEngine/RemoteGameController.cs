using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastleButcher.GameEngine
{
    public class RemoteGameController:GameController
    {
        public override bool IsLocal
        {
            get { return false; }
        }

        public override void AddPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public override void RemovePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public override void ChangePlayerTeam(Player player, GameTeam team)
        {
            throw new NotImplementedException();
        }

        public override void BeginRound()
        {
            throw new NotImplementedException();
        }

        public override void EndRound()
        {
            throw new NotImplementedException();
        }

        public override void EndRound(GameTeam defeatedTeam)
        {
            throw new NotImplementedException();
        }

        protected override void OnPlayerAdded(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void OnPlayerRemoved(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void OnPlayerRespawned(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void OnPlayerKilled(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void OnCharacterAdded(Character character)
        {
            throw new NotImplementedException();
        }

        protected override void OnCharacterRemoved(Character character)
        {
            throw new NotImplementedException();
        }

        protected override void OnObjectAdded(IGameObject obj)
        {
            throw new NotImplementedException();
        }

        protected override void OnObjectRemoved(IGameObject obj)
        {
            throw new NotImplementedException();
        }
    }
}

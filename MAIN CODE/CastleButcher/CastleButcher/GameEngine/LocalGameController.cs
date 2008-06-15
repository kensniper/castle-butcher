using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using UDPClientServerCommons.Client;
using UDPClientServerCommons.Interfaces;
using UDPClientServerCommons.Server;
namespace CastleButcher.GameEngine
{
    public class LocalGameController : GameController
    {
        ServerSide serverNetworkLayer;
        Player player;

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }
        public LocalGameController(Player player)
        {
            gameStatus = GameStatus.WaitingForStart;
            this.player = player;
            serverNetworkLayer = new ServerSide(4444);
            GameOptions options = new GameOptions("Zabójcy", "Rycerze", UDPClientServerCommons.Constants.GameTypeEnumeration.TimeLimit, 10);
            UDPClientServerCommons.Usefull.PlayerMe pl = new UDPClientServerCommons.Usefull.PlayerMe("Zabójcy", player.Name);

            serverNetworkLayer.StartLANServer(options, false, pl);       
        }

        

        
        public override void AddPlayer(Player player)
        {
            World.Instance.AddPlayer(player);
            if (player == this.player)
                ChangePlayerTeam(player, player.CharacterClass.GameTeam);
        }

        public override void RemovePlayer(Player player)
        {
            World.Instance.RemovePlayer(player);
        }

        public override void ChangePlayerTeam(Player player, GameTeam newTeam)
        {
            World.Instance.ChangeTeam(player, newTeam);
            if (player == this.player)
            {
                //if(newTeam== GameTeam.Assassins)
                //    serverNetworkLayer.Client.ChangeTeam(serverNetworkLayer.Client.CurrentGameInfo.TeamScoreList[0].TeamId);
                //else
                //    serverNetworkLayer.Client.ChangeTeam(serverNetworkLayer.Client.CurrentGameInfo.TeamScoreList[1].TeamId);
                //if (newTeam == GameTeam.Assassins)
                //    serverNetworkLayer.Client.ChangeTeam(13);
                //else
                //    serverNetworkLayer.Client.ChangeTeam(39);
            }
            
        }

        public override void BeginRound()
        {
            World.Instance.Start();
            gameStatus = GameStatus.InProgress;            
        }

        public override void EndRound()
        {
            World.Instance.Paused = true;
            gameStatus = GameStatus.WaitingForStart;
            
        }
        public override void EndRound(GameTeam defeatedTeam)
        {
            base.EndRound(defeatedTeam);
            World.Instance.Paused = true;
            gameStatus = GameStatus.WaitingForStart;
        }

        protected override void OnPlayerAdded(Player player)
        {

        }

        protected override void OnPlayerRemoved(Player player)
        {
            if (World.Instance.Players.Count == 0)
            {
                EndRound();
            }
        }

        protected override void OnPlayerRespawned(Player player)
        {
            
        }

        protected override void OnPlayerKilled(Player player)
        {
            bool allDead = true;
            if (player.CharacterClass.GameTeam == GameTeam.Assassins)
            {
                for (int i = 0; i < World.Instance.AssassinPlayers.Count; i++)
                {
                    if (World.Instance.AssassinPlayers[i].IsAlive)
                        allDead = false;
                }
            }
            else
            {
                for (int i = 0; i < World.Instance.KnightPlayers.Count; i++)
                {
                    if (World.Instance.KnightPlayers[i].IsAlive)
                        allDead = false;
                }
            }

            if (allDead)
            {
                EndRound(player.CharacterClass.GameTeam);
                BeginRound();
            }
        }

        protected override void OnCharacterAdded(Character character)
        {
        }

        protected override void OnCharacterRemoved(Character character)
        {
        }
        protected override void OnObjectAdded(IGameObject obj)
        {
        }
        protected override void OnObjectRemoved(IGameObject obj)
        {
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public override bool Update(float timeElapsed)
        {
            World.Instance.Update(timeElapsed);
            return true;
        }

        public override void EndGame()
        {
            serverNetworkLayer.Client.LeaveGame();
        }

        public override void StartGame()
        {
            serverNetworkLayer.StartGame();
            BeginRound();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons;
using UDPClientServerCommons.Client;
using UDPClientServerCommons.Interfaces;
using UDPClientServerCommons.Server;
using Microsoft.DirectX;
using Framework.MyMath;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.GameEvents;
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
                if (newTeam == GameTeam.Assassins)
                    serverNetworkLayer.Client.ChangeTeam(serverNetworkLayer.Client.CurrentGameInfo.TeamScoreList[0].TeamId);
                else
                    serverNetworkLayer.Client.ChangeTeam(serverNetworkLayer.Client.CurrentGameInfo.TeamScoreList[1].TeamId);

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

            //siec?
        }

        public override void EndRound()
        {
            World.Instance.Paused = true;
            gameStatus = GameStatus.WaitingForStart;

            //siec?

        }
        public override void EndRound(GameTeam defeatedTeam)
        {
            base.EndRound(defeatedTeam);
            World.Instance.Paused = true;
            gameStatus = GameStatus.WaitingForStart;
            if (defeatedTeam == player.CharacterClass.GameTeam)
            {
                SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.fanfare1, (Vector3)player.CurrentCharacter.Position);
            }
            else
            {
                SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.defeat, (Vector3)player.CurrentCharacter.Position);
            }

            //siec?
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
            List<IOtherPlayerData> data = serverNetworkLayer.Client.PlayerDataList;

            List<IGameEvent> gameEvents = serverNetworkLayer.Client.GameEventList;
            List<IGameplayEvent> gameplayEvents = serverNetworkLayer.Client.GameplayEventList;
            for (int i = 0; i < gameEvents.Count; i++)
            {
                if (gameEvents[i].GameEventType == GameEventTypeEnumeration.GameStared)
                {
                    //gameEvents[i].
                    //StartGame();
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.PlayerJoined)
                {
                    PlayerJoinedEvent ev = (PlayerJoinedEvent)gameEvents[i];
                    Player p = new Player(ev.PlayerName, null);
                    p.NetworkId = ev.PlayerId;
                    AddPlayer(p);
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.PlayerChangedTeam)
                {
                    PlayerChangedTeamEvent ev = (PlayerChangedTeamEvent)gameEvents[i];

                    Player p = GetPlayerByID(ev.PlayerId);
                    ChangePlayerTeam(p, GetTeamByID(ev.NewTeamId));
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.PlayerQuitted)
                {
                    PlayerQuitEvent ev = (PlayerQuitEvent)gameEvents[i];

                    Player p = GetPlayerByID(ev.PlayerId);
                    RemovePlayer(p);
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.NewRound)
                {
                    BeginRound();
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.TeamScored)
                {
                    EndRound();
                    TeamScoredEvent ev = (TeamScoredEvent)gameEvents[i];
                    GameTeam team = GetTeamByID(ev.TeamId);
                    if (team == GameTeam.Assassins)
                        assassinsScore++;
                    else
                        knightsScore++;
                }

            }

            //if (data != null)
            //{
            //    for (int i = 0; i < World.Instance.Players.Count; i++)
            //    {
            //        Player p = World.Instance.Players[i];

            //        for (int j = 0; j < data.Count; j++)
            //        {
            //            if (p.NetworkId == /*data[i].ID*/0)
            //            {
            //                p.CurrentCharacter.Position = (MyVector)data[i].Position;

            //                p.CurrentCharacter.Velocity = (MyVector)data[i].Velocity;

            //                //p.CurrentCharacter.
            //            }
            //        }
            //    }
            //}
            World.Instance.Update(timeElapsed);
            //serverNetworkLayer.Client.UpdatePlayerData(
            return true;
        }

        public override void EndGame()
        {
            serverNetworkLayer.Client.LeaveGame();
        }

        public override void StartGame()
        {
            serverNetworkLayer.StartGame();
            World.Instance.Remote = false;
            BeginRound();
        }
    }
}

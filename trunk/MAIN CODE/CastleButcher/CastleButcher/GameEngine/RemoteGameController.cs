using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons.Client;
using System.Net;
using CastleButcher.UI;
using Microsoft.DirectX;
using Framework.MyMath;
using UDPClientServerCommons.Interfaces;
using UDPClientServerCommons.Constants;
using UDPClientServerCommons.GameEvents;

namespace CastleButcher.GameEngine
{
    public class RemoteGameController : GameController
    {

        ClientSide clientNetworkLayer;
        Player player;

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }
        public RemoteGameController(Player player, ClientSide clientSide, UDPClientServerCommons.Packets.GameInfoPacket gameInfo)
        {
            gameStatus = GameStatus.WaitingForStart;
            clientNetworkLayer = clientSide;
            this.player = player;
            clientNetworkLayer.JoinGame(gameInfo.ServerAddress, player.Name, gameInfo.GameId, gameInfo.TeamScoreList[0].TeamId);


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
                    clientNetworkLayer.ChangeTeam(clientNetworkLayer.CurrentGameInfo.TeamScoreList[0].TeamId);
                else
                    clientNetworkLayer.ChangeTeam(clientNetworkLayer.CurrentGameInfo.TeamScoreList[1].TeamId);
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
            if (defeatedTeam == player.CharacterClass.GameTeam)
            {
                SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.fanfare1, (Vector3)player.CurrentCharacter.Position);
            }
            else
            {
                SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.defeat, (Vector3)player.CurrentCharacter.Position);
            }
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

            //if (allDead)
            //{
            //    EndRound(player.CharacterClass.GameTeam);
            //    BeginRound();
            //}
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
            get { return false; }
        }

        public override bool Update(float timeElapsed)
        {
            List<IOtherPlayerData> data = clientNetworkLayer.PlayerDataList;

            List<IGameEvent> gameEvents = clientNetworkLayer.GameEventList;
            List<IGameplayEvent> gameplayEvents = clientNetworkLayer.GameplayEventList;
            for (int i = 0; i < gameEvents.Count; i++)
            {

                if (gameEvents[i].GameEventType == GameEventTypeEnumeration.GameStared)
                {
                    //gameEvents[i].
                    StartGame();
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
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.EndRound)
                {
                    EndRound();
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.TeamScored)
                {
                    TeamScoredEvent ev = (TeamScoredEvent)gameEvents[i];
                    GameTeam team = GetTeamByID(ev.TeamId);
                    if (team == GameTeam.Assassins)
                        assassinsScore++;
                    else
                        knightsScore++;
                }

            }
          

            World.Instance.Update(timeElapsed);
            return true;
        }

        public override void EndGame()
        {
            clientNetworkLayer.LeaveGame();
        }

        public override void StartGame()
        {

        }
    }
}

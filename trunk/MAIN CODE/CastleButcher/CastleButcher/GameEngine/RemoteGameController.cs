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
        //Player player;
        UDPClientServerCommons.Packets.GameInfoPacket gameInfo;

        //public Player Player
        //{
        //    get { return player; }
        //    set { player = value; }
        //}
        public RemoteGameController(Player player, ClientSide clientSide, UDPClientServerCommons.Packets.GameInfoPacket gameInfo)
        {
            gameStatus = GameStatus.WaitingForStart;
            clientNetworkLayer = clientSide;
            this.player = player;
            this.gameInfo = gameInfo;


        }




        public override void AddPlayer(Player player)
        {
            World.Instance.AddPlayer(player);
            if (player == this.player)
            {
                ushort id = (Player.CharacterClass.GameTeam == GameTeam.Assassins) ? gameInfo.TeamScoreList[0].TeamId : gameInfo.TeamScoreList[1].TeamId;
                bool ret = clientNetworkLayer.JoinGame(gameInfo.ServerAddress, player.Name, gameInfo.GameId, id);

                //ChangePlayerTeam(player, player.CharacterClass.GameTeam);
            }
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
            World.Instance.Remote = true;
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
            //if (World.Instance.Players.Count == 0)
            //{
            //    EndRound();
            //}
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
                    player.NetworkId = clientNetworkLayer.PlayerId ?? 0;
                    StartGame();

                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.PlayerJoined)
                {
                    PlayerJoinedEvent ev = (PlayerJoinedEvent)gameEvents[i];
                    if (ev.PlayerName != player.Name)
                    {
                        GameTeam team = GetTeamByID(ev.TeamId);
                        Player p;
                        if (team == GameTeam.Assassins)
                        {
                            p = new Player(ev.PlayerName, ObjectCache.Instance.GetAssassinClass());
                        }
                        else
                        {
                            p = new Player(ev.PlayerName, ObjectCache.Instance.GetKnightClass());
                        }
                        p.NetworkId = ev.PlayerId;
                        AddPlayer(p);
                    }
                    else
                    {
                        player.NetworkId = ev.PlayerId;
                    }
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
                    EndRound();
                    BeginRound();
                }
                else if (gameEvents[i].GameEventType == GameEventTypeEnumeration.TeamScored)
                {

                    TeamScoredEvent ev = (TeamScoredEvent)gameEvents[i];
                    GameTeam team = GetTeamByID(ev.TeamId);
                    if (team == GameTeam.Assassins)
                        team = GameTeam.Knights;
                    else
                        team = GameTeam.Assassins;
                    EndRound(team);

                }

            }

            if (World.Instance.Started)
            {
                for (int i = 0; i < gameplayEvents.Count; i++)
                {
                    if (gameplayEvents[i].GameplayEventType == GamePlayEventTypeEnumeration.PlayerDead)
                    {
                        Player p = GetPlayerByID(gameplayEvents[i].PlayerId);
                        World.Instance.PlayerKilled(p);
                    }
                    else if (gameplayEvents[i].GameplayEventType == GamePlayEventTypeEnumeration.JumpNow)
                    {
                        Player p = GetPlayerByID(gameplayEvents[i].PlayerId);
                        p.CharacterController.Jump();
                    }
                    else if (gameplayEvents[i].GameplayEventType == GamePlayEventTypeEnumeration.WeaponChange)
                    {
                        Player p = GetPlayerByID(gameplayEvents[i].PlayerId);
                        if (p == player) continue;
                        //gameplayEvents[i].
                        for (int j = 0; j < clientNetworkLayer.PlayerDataList.Count; j++)
                        {
                            if (clientNetworkLayer.PlayerDataList[j].PlayerId == p.NetworkId)
                            {
                                WeaponEnumeration we = clientNetworkLayer.PlayerDataList[j].Weapon;
                                p.CurrentCharacter.Weapons.SelectWeapon(GetWeaponClassByID(we));
                            }
                        }
                    }
                    else if (gameplayEvents[i].GameplayEventType == GamePlayEventTypeEnumeration.UseWeapon)
                    {
                        Player p = GetPlayerByID(gameplayEvents[i].PlayerId);
                        if (p == player) continue;
                        //gameplayEvents[i].
                        for (int j = 0; j < clientNetworkLayer.PlayerDataList.Count; j++)
                        {
                            if (clientNetworkLayer.PlayerDataList[j].PlayerId == p.NetworkId)
                            {
                                WeaponEnumeration we = clientNetworkLayer.PlayerDataList[j].Weapon;
                                p.CurrentCharacter.Weapons.SelectWeapon(GetWeaponClassByID(we));
                            }
                        }
                        p.CurrentCharacter.FireWeapon();
                    }
                }

                if (data != null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        Player p = GetPlayerByID(data[i].PlayerId);
                        if (p.LastTimeStamp < data[i].Timestamp && p.IsAlive)
                        {
                            p.CurrentCharacter.Position = (MyVector)data[i].Position;
                            p.CurrentCharacter.Velocity = (MyVector)data[i].Velocity;
                            p.CurrentCharacter.LookOrientation = this.LookOrientationFromDirection((MyVector)data[i].LookingDirection);
                            p.CurrentCharacter.WalkOrientation = this.WalkOrientationFromDirection((MyVector)data[i].LookingDirection);
                            p.LastTimeStamp = data[i].Timestamp;

                        }
                        //p.CurrentCharacter.

                    }
                }
                World.Instance.Update(timeElapsed);
                if (player.IsAlive)
                {
                    clientNetworkLayer.UpdatePlayerData((Vector3)player.CurrentCharacter.Position, (Vector3)
                        player.CurrentCharacter.CharacterController.LookVector, (Vector3)player.CurrentCharacter.Velocity);

                    WeaponEnumeration we;
                    if (player.CurrentCharacter.Weapons.CurrentWeapon != null)
                        we = GetWeaponIDByClass(player.CurrentCharacter.Weapons.CurrentWeapon.WeaponClass);
                    else
                        we = GetWeaponIDByClass(null);
                    clientNetworkLayer.UpdatePlayerData(we, player.FiredWeapon, player.Jumped, player.ChangedWeapon, we);
                    
                    player.ChangedWeapon = false;
                    player.FiredWeapon = false;
                    player.Jumped = false;

                }
            }
            return true;
        }

        public override void EndGame()
        {
            clientNetworkLayer.LeaveGame();
        }

        public override void StartGame()
        {
            World.Instance.Remote = true;
        }
    }
}

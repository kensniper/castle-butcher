using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPClientServerCommons.Client;
using System.Net;
using CastleButcher.UI;
using Microsoft.DirectX;
using Framework.MyMath;

namespace CastleButcher.GameEngine
{
    public class RemoteGameController : GameController
    {

        ClientSide clientNetworkLayer;
        Player player;
        public RemoteGameController(Player player)
        {
            gameStatus = GameStatus.WaitingForStart;
            clientNetworkLayer = new ClientSide(4444);
            this.player = player;

        }




        public override void AddPlayer(Player player)
        {
            World.Instance.AddPlayer(player);
        }

        public override void RemovePlayer(Player player)
        {
            World.Instance.RemovePlayer(player);
        }

        public override void ChangePlayerTeam(Player player, GameTeam newTeam)
        {
            World.Instance.ChangeTeam(player, newTeam);
        }

        public override void BeginRound()
        {
            World.Instance.Start();
            gameStatus = GameStatus.InProgress;
            player.NetworkId=clientNetworkLayer.JoinGame(new System.Net.IPEndPoint(IPAddress.Parse("10.0.0.2"), 1234), "kkk", 0);

        }

        public override void EndRound()
        {
            World.Instance.Paused = true;
            gameStatus = GameStatus.WaitingForStart;
        }
        public override void EndRound(GameTeam defeatedTeam)
        {
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
            get { return false; }
        }

        public override bool Update(float timeElapsed)
        {
            if (player.IsAlive && player.CurrentCharacter != null)
            {
                clientNetworkLayer.UpdatePlayerData((Vector3)player.CurrentCharacter.Position,
                    (Vector3)player.CurrentCharacter.LookDirection,
                    (Vector3)player.CurrentCharacter.Velocity);
            }
            if (player.IsAlive)
            {
                UDPClientServerCommons.ServerPacket packet = ((UDPClientServerCommons.Interfaces.IServerData)clientNetworkLayer).GetNeewestDataFromServer();
                if (packet != null)
                {
                    for (int i = 0; i < packet.PlayerInfoList.Count; i++)
                    {
                        UDPClientServerCommons.PlayerInfo pInfo = packet.PlayerInfoList[i];

                        bool newPlayer = false;
                        foreach (Player pp in World.Instance.Players)
                        {
                            if (pInfo.PlayerId == pp.NetworkId)
                            {
                                newPlayer = false;
                                break;
                            }
                        }
                        if (newPlayer)
                        {
                            AddPlayer(new Player(pInfo.PlayerId.ToString(), ObjectCache.Instance.GetKnightClass()));

                        }

                    }
                    for (int i = 0; i < packet.PlayerInfoList.Count; i++)
                    {
                        UDPClientServerCommons.PlayerInfo pInfo = packet.PlayerInfoList[i];

                        foreach (Player p in World.Instance.Players)
                        {

                            if (p.NetworkId == pInfo.PlayerId)
                            {
                                MyVector v = new MyVector();
                                v.X = pInfo.PlayerPosition.X;
                                v.Y = pInfo.PlayerPosition.Y;
                                v.Z = pInfo.PlayerPosition.Z;
                                p.CurrentCharacter.Position = v;
                                v.X = pInfo.PlayerMovementDirection.X;
                                v.Y = pInfo.PlayerMovementDirection.Y;
                                v.Z = pInfo.PlayerMovementDirection.Z;
                                p.CurrentCharacter.Velocity = v;

                            }
                        }

                    }
                }
            }

            World.Instance.Update(timeElapsed);
            return true;
        }
    }
}

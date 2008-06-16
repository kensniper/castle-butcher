using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons.GameEvents
{
    public static class GameEventExtractor
    {
        public static List<Interfaces.IGameEvent> GetGameEvents(Packets.GameInfoPacket newPacket, Packets.GameInfoPacket oldPacket, ushort? PlayerId)
        {
            Dictionary<ushort, string> teamNameDictionary = new Dictionary<ushort, string>();
            List<Interfaces.IGameEvent> gameEvents = new List<UDPClientServerCommons.Interfaces.IGameEvent>();

            if (newPacket != null && oldPacket != null)// && PlayerId.HasValue)
            {
                try
                {
                    for (int i = 0; i < newPacket.TeamScoreList.Count; i++)
                    {
                        teamNameDictionary.Add(newPacket.TeamScoreList[i].TeamId, newPacket.TeamScoreList[i].TeamName);
                        for (int j = 0; j < oldPacket.TeamScoreList.Count; j++)
                        {
                            if (newPacket.TeamScoreList[i].TeamId == oldPacket.TeamScoreList[j].TeamId)
                            {
                                if (newPacket.TeamScoreList[i].TeamScore != oldPacket.TeamScoreList[j].TeamScore)
                                {
                                    // team score
                                    gameEvents.Add(new GameEvents.TeamScoredEvent(newPacket.TeamScoreList[i].TeamId, newPacket.TeamScoreList[i].TeamName, newPacket.TeamScoreList[i].TeamScore, oldPacket.TeamScoreList[i].TeamScore));
                                }
                                break;
                            }
                        }
                    }

                    if (newPacket.RoundNumber == 0)
                    {
                        // game over
                        gameEvents.Add(new GameEvents.GameEndEvent());
                    }

                    if (newPacket.RoundNumber > oldPacket.RoundNumber)
                    {
                        //new round
                        gameEvents.Add(new GameEvents.NewRoundEvent((Packets.GameInfoPacket) newPacket.Clone()));
                    }

                    if (newPacket.PlayerStatusList.Count < oldPacket.PlayerStatusList.Count)
                    {
                        // player quitted
                        for (int i = 0; i < oldPacket.PlayerStatusList.Count; i++)
                        {
                            bool found = false;
                            ushort id = oldPacket.PlayerStatusList[i].PlayerId;
                            for (int j = 0; j < newPacket.PlayerStatusList.Count; j++)
                            {
                                if (newPacket.PlayerStatusList[j].PlayerId == id)
                                    found = true;
                            }
                            if (!found)
                            {
                                gameEvents.Add(new GameEvents.PlayerQuitEvent(id, oldPacket.PlayerStatusList[i].PlayerName));
                            }
                            found = false;
                        }
                    }
                    if (newPacket.PlayerStatusList.Count > oldPacket.PlayerStatusList.Count)
                    {
                        // player joined
                        for (int i = 0; i < newPacket.PlayerStatusList.Count; i++)
                        {
                            bool found = false;
                            ushort id = newPacket.PlayerStatusList[i].PlayerId;
                            for (int j = 0; j < oldPacket.PlayerStatusList.Count; j++)
                            {
                                if (oldPacket.PlayerStatusList[j].PlayerId == id)
                                    found = true;
                            }
                            if (!found)
                            {
                                gameEvents.Add(new GameEvents.PlayerJoinedEvent(id, newPacket.PlayerStatusList[i].PlayerName,newPacket.PlayerStatusList[i].PlayerTeam));
                            }
                            found = false;
                        }

                    }
                    for (int i = 0; i < newPacket.PlayerStatusList.Count; i++)
                        for (int j = 0; j < oldPacket.PlayerStatusList.Count; j++)
                        {
                            if (newPacket.PlayerStatusList[i].PlayerId == oldPacket.PlayerStatusList[j].PlayerId)
                            {
                                if (newPacket.PlayerStatusList[i].PlayerTeam != oldPacket.PlayerStatusList[j].PlayerTeam)
                                {
                                    // player changed team
                                    gameEvents.Add(new GameEvents.PlayerChangedTeamEvent(newPacket.PlayerStatusList[i].PlayerId, newPacket.PlayerStatusList[i].PlayerName, newPacket.PlayerStatusList[i].PlayerTeam, teamNameDictionary[newPacket.PlayerStatusList[i].PlayerTeam]));
                                }
                                if (newPacket.PlayerStatusList[i].PlayerScore != oldPacket.PlayerStatusList[j].PlayerScore)
                                {
                                    // player scored
                                    gameEvents.Add(new GameEvents.PlayerScoredEvent(newPacket.PlayerStatusList[i].PlayerId, newPacket.PlayerStatusList[i].PlayerName, newPacket.PlayerStatusList[i].PlayerScore, oldPacket.PlayerStatusList[i].PlayerScore));
                                }
                                if (newPacket.PlayerStatusList[i].PlayerName != oldPacket.PlayerStatusList[j].PlayerName)
                                {
                                    // player nick changed
                                    gameEvents.Add(new GameEvents.PlayerNickChangedEvent(newPacket.PlayerStatusList[i].PlayerId, oldPacket.PlayerStatusList[i].PlayerName, newPacket.PlayerStatusList[i].PlayerName));
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("GetGameEvents Error", ex);
                }
            }
            else if (oldPacket == null && newPacket != null)
            {
                // first gameInfoPacket - if game is not on a dedicated server than there will be a server - player
                if (newPacket.PlayerStatusList.Count == 1)
                {
                    gameEvents.Add(new GameEvents.PlayerJoinedEvent(newPacket.PlayerStatusList[0].PlayerId, newPacket.PlayerStatusList[0].PlayerName, newPacket.PlayerStatusList[0].PlayerTeam));
                }
            }

            return gameEvents;
        }

        public static List<Interfaces.IGameplayEvent> GetGameplayEvents(Packets.ServerPacket newPacket, Packets.ServerPacket oldPacket, ushort? PlayerId)
        {

            List<Interfaces.IGameplayEvent> gameplayEvents = new List<UDPClientServerCommons.Interfaces.IGameplayEvent>();
            if (newPacket != null && oldPacket != null && PlayerId.HasValue)
            {
                try
                {
                    if (newPacket.NumberOfPlayers == oldPacket.NumberOfPlayers)
                        for (int i = 0; i < newPacket.PlayerInfoList.Count; i++)
                        {
                            for (int j = 0; j < oldPacket.PlayerInfoList.Count; j++)
                            {
                                Microsoft.DirectX.Vector3 position = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerPosition);
                                Microsoft.DirectX.Vector3 velocity = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerMovementDirection);
                                Microsoft.DirectX.Vector3 lookingDirection = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerLookingDirection);

                                if (newPacket.PlayerInfoList[i].PlayerId == oldPacket.PlayerInfoList[j].PlayerId && newPacket.PlayerInfoList[i].PlayerId == PlayerId.Value)
                                {
                                    // check current player health
                                    if (newPacket.PlayerInfoList[i].PlayerId == PlayerId.Value && newPacket.PlayerInfoList[i].Health != oldPacket.PlayerInfoList[j].Health)
                                    {
                                        // damage was taken
                                        // damage was taken
                                        if (newPacket.PlayerInfoList[i].Health <= 0)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerDead, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                        else if (newPacket.PlayerInfoList[i].Health < oldPacket.PlayerInfoList[j].Health)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerWasHit, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                        else if (newPacket.PlayerInfoList[i].Health == 100 && newPacket.PlayerInfoList[i].Health > oldPacket.PlayerInfoList[j].Health)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerRespawn, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                    }
                                    break;
                                }

                                // first find player to compare
                                if (newPacket.PlayerInfoList[i].PlayerId == oldPacket.PlayerInfoList[j].PlayerId && newPacket.PlayerInfoList[i].PlayerId != PlayerId.Value)
                                {
                                    // compare player data
                                    if (newPacket.PlayerInfoList[i].PlayerCarringWeponOne != oldPacket.PlayerInfoList[j].PlayerCarringWeponOne ||
                                        newPacket.PlayerInfoList[i].PlayerCarringWeponTwo != oldPacket.PlayerInfoList[j].PlayerCarringWeponTwo)
                                    {
                                        gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.WeaponChange, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                    }
                                    if (newPacket.PlayerInfoList[i].Health != oldPacket.PlayerInfoList[j].Health)
                                    {
                                        // damage was taken
                                        if (newPacket.PlayerInfoList[i].Health <= 0)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerDead, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                        else if (newPacket.PlayerInfoList[i].Health < oldPacket.PlayerInfoList[j].Health)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerWasHit, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                        else if (newPacket.PlayerInfoList[i].Health == 100 && newPacket.PlayerInfoList[i].Health > oldPacket.PlayerInfoList[j].Health)
                                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.PlayerRespawn, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                                    }
                                    break;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    Diagnostic.NetworkingDiagnostics.Logging.Error("GetGameplayEvents Exception", ex);
                }
            }
            try
            {
                if (newPacket != null && PlayerId.HasValue)
                {
                    for (int i = 0; i < newPacket.PlayerInfoList.Count; i++)
                    {
                        Microsoft.DirectX.Vector3 position = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerPosition);
                        Microsoft.DirectX.Vector3 velocity = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerMovementDirection);
                        Microsoft.DirectX.Vector3 lookingDirection = Translator.TranslateBetweenVectorAndVector(newPacket.PlayerInfoList[i].PlayerLookingDirection);

                        // get events that don't need comapare 
                        if (newPacket.PlayerInfoList[i].PlayerJumping && newPacket.PlayerInfoList[i].PlayerId!=PlayerId.Value)
                        {
                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.JumpNow, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                        }
                        if (newPacket.PlayerInfoList[i].PlayerShooting && newPacket.PlayerInfoList[i].PlayerId != PlayerId.Value)
                        {
                            gameplayEvents.Add(new GamePlayEvent(newPacket.PlayerInfoList[i].PlayerId, UDPClientServerCommons.Constants.GamePlayEventTypeEnumeration.UseWeapon, newPacket.PlayerInfoList[i].Timestamp, position, lookingDirection, velocity));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Diagnostic.NetworkingDiagnostics.Logging.Error("GetGameplayEvents Exception", ex);
            }

            return gameplayEvents;
        }
    }
}

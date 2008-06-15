﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class GameplayForm : Form
    {
        private UDPClientServerCommons.Interfaces.IClient network = null;

        private delegate void RefreshStuff(object obj);

        private System.Threading.Timer gameLoopTimer = null;

        private readonly object cordsLock = new object();
        private bool jumpField = false;
        private bool shootField = false;


        public GameplayForm(UDPClientServerCommons.Client.ClientSide clientSide)
        {
            InitializeComponent();

            System.Threading.TimerCallback tc = new System.Threading.TimerCallback(timerCallback);
            gameLoopTimer = new System.Threading.Timer(tc);
            gameLoopTimer.Change(1000, 3000);
            
            network = clientSide;
            //gameLoopTimer = new System.Threading.Timer(new System.Threading.TimerCallback(timerCallback), null, 0, 1000);
        }

        private void timerCallback(object obj)
        {
            try
            {
                lock (cordsLock)
                {
                    UDPClientServerCommons.Interfaces.IPlayerDataWrite pdata = new UDPClientServerCommons.Usefull.PlayerData();
                    pdata.Position = new Microsoft.DirectX.Vector3(float.Parse(txtX.Text), float.Parse(txtY.Text), 0.0f);
                    pdata.LookingDirection = new Microsoft.DirectX.Vector3(float.Parse(txtX.Text), float.Parse(txtY.Text), 0.0f);
                    pdata.Velocity = new Microsoft.DirectX.Vector3(float.Parse(txtX.Text), float.Parse(txtY.Text), 0.0f);
                    pdata.Jump = jumpField;
                    pdata.Shoot = shootField;

                    if (jumpField)
                        jumpField = false;
                    if (shootField)
                        shootField = false;
                    network.UpdatePlayerData(pdata);
                    string blabla = jumpField.ToString() + shootField.ToString();
                }
                dgData.Invoke(new RefreshStuff(RefreshGrid), network.PlayerDataList);
                txtEvents.Invoke(new RefreshStuff(RefreshGameEvents), network.GameEventList);
                txtEvents.Invoke(new RefreshStuff(RefreshGameplayEvents), network.GameplayEventList);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void AddTime(object obj)
        {
            txtEvents.Text = txtEvents.Text + System.Environment.NewLine + DateTime.Now.ToLongTimeString();
            txtEvents.ScrollToCaret();
            txtEvents.Select(txtEvents.Text.Length, 0);
        }

        private void RefreshGrid(object obj)
        {
            List<UDPClientServerCommons.Interfaces.IOtherPlayerData> data = obj as List<UDPClientServerCommons.Interfaces.IOtherPlayerData>;
            if (data != null)
            {
                dgData.DataSource = data;
                dgData.Refresh();
            }
        }

        private void RefreshGameEvents(object obj)
        {
            List<UDPClientServerCommons.Interfaces.IGameEvent> events = obj as List<UDPClientServerCommons.Interfaces.IGameEvent>;
            if (events != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    string info = "";
                    switch (events[i].GameEventType)
                    {
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.GameStared:
                            UDPClientServerCommons.GameEvents.GameStartedEvent gse = (UDPClientServerCommons.GameEvents.GameStartedEvent)events[i];
                            info = gse.GameEventType.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerChangedTeam:
                            UDPClientServerCommons.GameEvents.PlayerChangedTeamEvent pcte = (UDPClientServerCommons.GameEvents.PlayerChangedTeamEvent)events[i];
                            info = pcte.GameEventType.ToString() + " NewTeam " + pcte.NewTeamName + " Player Name " + pcte.PlayerName + " playerID " + pcte.PlayerId.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerJoined:
                            UDPClientServerCommons.GameEvents.PlayerJoinedEvent pje = (UDPClientServerCommons.GameEvents.PlayerJoinedEvent)events[i];
                            info = pje.GameEventType.ToString() + " playerName = " + pje.PlayerName + " playerId " + pje.PlayerId.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerNickChanged:
                            UDPClientServerCommons.GameEvents.PlayerNickChangedEvent pnce = (UDPClientServerCommons.GameEvents.PlayerNickChangedEvent)events[i];
                            info = pnce.GameEventType.ToString() + " oldNick " + pnce.OldPlayerNick + " newNick " + pnce.NewPlayerNick + " playerid " + pnce.PlayerId.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerQuitted:
                            UDPClientServerCommons.GameEvents.PlayerQuitEvent pqe = (UDPClientServerCommons.GameEvents.PlayerQuitEvent)events[i];
                            info = pqe.GameEventType.ToString() + " playerName " + pqe.PlayerName + " playeid " + pqe.PlayerId.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.PlayerScored:
                            UDPClientServerCommons.GameEvents.PlayerScoredEvent pse = (UDPClientServerCommons.GameEvents.PlayerScoredEvent)events[i];
                            info = pse.GameEventType.ToString() + " playerName " + pse.PlayerName + " scored " + (-pse.OldScore + pse.CurrentScoreField).ToString() + " playerId " + pse.PlayerId.ToString();
                            break;
                        case UDPClientServerCommons.Constants.GameEventTypeEnumeration.TeamScored:
                            UDPClientServerCommons.GameEvents.TeamScoredEvent tse = (UDPClientServerCommons.GameEvents.TeamScoredEvent)events[i];
                            info = tse.GameEventType.ToString() + "teamName " + tse.TeamName + " id " + tse.TeamId.ToString() + " scored " + (tse.CurrentScoreField - tse.OldScore).ToString();
                            break;
                    }
                }
            }
        }

        private void RefreshGameplayEvents(object obj)
        {
            List<UDPClientServerCommons.Interfaces.IGameplayEvent> events = obj as List<UDPClientServerCommons.Interfaces.IGameplayEvent>;
            if (events != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    txtEvents.Text += txtEvents.Text + System.Environment.NewLine +
                        "\t GameplayEvent " + events[i].GameplayEventType.ToString() + " player + " + events[i].PlayerId.ToString() + " timestamp " + events[i].Timestamp.ToShortTimeString();
                }
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                txtY.Text = (int.Parse(txtY.Text) + 1).ToString();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                txtY.Text = (int.Parse(txtY.Text) - 1).ToString();
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                txtX.Text = (int.Parse(txtX.Text) - 1).ToString();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                txtX.Text = (int.Parse(txtX.Text) + 1).ToString();
            }
        }

        private void btnShoot_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                shootField = true;
            }
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            lock (cordsLock)
            {
                jumpField = true;
            }
        }

        private void GameplayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameLoopTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
    }
}

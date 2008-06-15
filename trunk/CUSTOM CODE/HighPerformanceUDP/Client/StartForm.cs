using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class StartForm : Form
    {
        private UDPClientServerCommons.Client.ClientSide clientSideNetworking;
        private List<UDPClientServerCommons.Packets.GameInfoPacket> gameInfoPackets;
        private delegate void RefreshDelegate();
        private bool joinedGame = false;

        private GameplayForm gamePlayForm;

        public StartForm()
        {
            InitializeComponent();
            btnRefresh.Enabled = false;
            btnJoin.Enabled = false;
            btnLeave.Enabled = false;
            gameInfoPackets = new List<UDPClientServerCommons.Packets.GameInfoPacket>();
        }

        void clientSideNetworking_PacketReceivedEvent(UDPClientServerCommons.Interfaces.IPacket serverPacket)
        {
            if (serverPacket.PacketType == UDPClientServerCommons.Constants.PacketTypeEnumeration.GameInfoPacket)
            {
                dataGridView1.Invoke(new RefreshDelegate(RefreshGrid));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            clientSideNetworking = new UDPClientServerCommons.Client.ClientSide(Int32.Parse(txtPort.Text));
            txtPort.ReadOnly = true;
            if (clientSideNetworking.StartLookingForLANGames()==true)
            {
                btnStart.Enabled = false;
                btnRefresh.Enabled = true;
                clientSideNetworking.PacketReceivedEvent += new UDPClientServerCommons.Client.ClientSide.packetReceived(clientSideNetworking_PacketReceivedEvent);
            }
        }

        private void RefreshGrid()
        {
            gameInfoPackets = new List<UDPClientServerCommons.Packets.GameInfoPacket>(clientSideNetworking.CurrentLanGames);
            dataGridView1.DataSource = gameInfoPackets;
            if (gameInfoPackets.Count > 0)
                dataGridView1.InvalidateRow(0);
            if (gameInfoPackets.Count > 0 && !btnLeave.Enabled)
                btnJoin.Enabled = true;

            if (!joinedGame && clientSideNetworking.PlayerHasSuccesfullyJoinedGame)
            {
                joinedGame = true;
                btnJoin.Enabled = false;
                btnLeave.Enabled = true;
                btnJoin.Refresh();
                btnLeave.Refresh();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gameInfoPackets.Count > 0)
            {
                UDPClientServerCommons.Packets.GameInfoPacket gameInfoPacket = gameInfoPackets[e.RowIndex];

                ChooseTeamForm chooseTeam = new ChooseTeamForm(gameInfoPacket);
                if (chooseTeam.ShowDialog(this) == DialogResult.OK)
                {
                    clientSideNetworking.JoinGame(gameInfoPacket.ServerAddress, chooseTeam.PlayerName, gameInfoPacket.GameId, chooseTeam.TeamId.Value);
                }
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int index = dataGridView1.SelectedRows[0].Index;

                UDPClientServerCommons.Packets.GameInfoPacket gameInfoPacket = gameInfoPackets[index];

                ChooseTeamForm chooseTeam = new ChooseTeamForm(gameInfoPacket);
                if (chooseTeam.ShowDialog(this) == DialogResult.OK)
                {
                    clientSideNetworking.JoinGame(gameInfoPacket.ServerAddress, chooseTeam.PlayerName, gameInfoPacket.GameId, chooseTeam.TeamId.Value);
                    gamePlayForm = new GameplayForm(clientSideNetworking);
                    gamePlayForm.Show();
                }
            }
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(clientSideNetworking!=null)
            clientSideNetworking.Dispose();
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            try
            {
                if (clientSideNetworking != null)
                    clientSideNetworking.LeaveGame();
                clientSideNetworking = null;
                btnLeave.Enabled = false;
                btnJoin.Enabled = false;
                btnStart.Enabled = true;
                txtPort.ReadOnly = false;
                btnRefresh.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());   
            }
        }

        private void splitContainer1_Panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StartForm st = new StartForm();
            st.Show();
        }
    }
}

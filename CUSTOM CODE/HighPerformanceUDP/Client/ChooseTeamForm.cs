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
    public partial class ChooseTeamForm : Form
    {
        private UDPClientServerCommons.Packets.GameInfoPacket packet = null;

        private ushort? teamIdField = null;
        
        public ushort? TeamId
        {
            get { return teamIdField; }
        }

        private string playerNameField = null;

        public string PlayerName
        {
            get { return playerNameField; }
        }

        public ChooseTeamForm(UDPClientServerCommons.Packets.GameInfoPacket gameInfo)
        {
            InitializeComponent();
            packet = (UDPClientServerCommons.Packets.GameInfoPacket)gameInfo.Clone();

            dataGridView1.DataSource = packet.TeamScoreList;
            dataGridView2.DataSource = packet.PlayerStatusList;
            dataGridView2.Enabled = false;

            this.DialogResult = DialogResult.Cancel;
            this.textBox1.Text += textBox1.Text.GetHashCode().ToString();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            teamIdField = packet.TeamScoreList[e.RowIndex].TeamId;
            playerNameField = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = packet.TeamScoreList;
            dataGridView1.Refresh();
            dataGridView2.DataSource = packet.PlayerStatusList;
            dataGridView2.Enabled = false;
            dataGridView2.Refresh();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int index = dataGridView1.SelectedRows[0].Index;

                teamIdField = packet.TeamScoreList[index].TeamId;
                playerNameField = textBox1.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

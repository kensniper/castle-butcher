using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;

namespace FormsTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            SoundSystem.SoundEngine.InitializeEngine(this);
            //foreach (Type ee in SoundSystem.Enums.MusicTypes)
            //{
            //    ee.ToString();
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float x = (float)Convert.ToDouble(xTxtBx.Text.ToString());
            float y = (float)Convert.ToDouble(yTxtBx.Text.ToString());
            float z = (float)Convert.ToDouble(zTxtBx.Text.ToString());
            switch (soundsListBx.Text)
            {
                case "swordPure":
                    SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.swordPure, new Vector3(x, y, z));
                    break;
                case "swordFight1":
                    SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.swordFight1, new Vector3(x, y, z));
                    break;
                default:
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (musicLstBx.Text)
            {
                case "welcomeMusic":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.welcomeMusic);
                    break;
                case "round1Music":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round1Music);
                    break;
                default:
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SoundSystem.SoundEngine.StopMusic();
        }
    }
}

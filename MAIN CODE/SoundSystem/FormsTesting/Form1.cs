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
            SoundSystem.SoundEngine.NameFinder.SoundDirectoryPath = "SoundSamples/";
            //foreach (Type ee in SoundSystem.Enums.MusicTypes)
            //{
            //    ee.ToString();
            //}
            //float x = (float)Convert.ToDouble(xTxtBx.Text.ToString());
            //float y = (float)Convert.ToDouble(yTxtBx.Text.ToString());
            //float z = (float)Convert.ToDouble(zTxtBx.Text.ToString());
                        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float x = (float)Convert.ToDouble(xTxtBx.Text.ToString());
            float y = (float)Convert.ToDouble(yTxtBx.Text.ToString());
            float z = (float)Convert.ToDouble(zTxtBx.Text.ToString());
            switch (soundsListBx.Text)
            {
                case "swordFight1":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.swordFight1);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.swordFight1, new Vector3(x, y, z));
                    break;
                case "swordFight2":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.swordFight2);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.swordFight2, new Vector3(x, y, z));
                    break;
                case "swordFight3":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.swordFight3);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.swordFight3, new Vector3(x, y, z));
                    break;
                case "swordFight4":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.swordFight4);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.swordFight4, new Vector3(x, y, z));
                    break;
                case "swordFight5":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.swordFight5);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.swordFight5, new Vector3(x, y, z));
                    break;
                case "daggerFight1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight1, new Vector3(x, y, z));
                    break;
                case "daggerFight2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight2, new Vector3(x, y, z));
                    break;
                case "daggerFight3":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight3, new Vector3(x, y, z));
                    break;
                case "daggerFight4":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight4, new Vector3(x, y, z));
                    break;
                case "daggerFight5":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight5, new Vector3(x, y, z));
                    break;
                case "stepsInside1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsInside1, new Vector3(x, y, z));
                    break;
                case "stepsInside2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsInside2, new Vector3(x, y, z));
                    break;
                case "stepsInside3":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsInside3, new Vector3(x, y, z));
                    break;
                case "stepsSlower":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsSlower, new Vector3(x, y, z));
                    break;
                case "stepsStone":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsStone, new Vector3(x, y, z));
                    break;
                case "stepsStairs":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsStairs, new Vector3(x, y, z));
                    break;
                case "stepsGrass1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsGrass1, new Vector3(x, y, z));
                    break;
                case "stepsGrass2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsGrass2, new Vector3(x, y, z));
                    break;
                case "stepsPath1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsPath1, new Vector3(x, y, z));
                    break;
                case "stepsPath2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.stepsPath2, new Vector3(x, y, z));
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

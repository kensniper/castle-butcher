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
        private Boolean isWalking = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            SoundSystem.SoundEngine.InitializeEngine(this, -900, 100.0f, 1.0f);
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
                case "explode":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.explode);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.explode, new Vector3(x, y, z));
                    break;
                case "defeat":
                    soundSourceTxtBx.Text =
                        SoundSystem.SoundEngine.NameFinder.FindSound(
                        SoundSystem.Enums.SoundTypes.defeat);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.defeat, new Vector3(x, y, z));
                    break;
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
                    soundSourceTxtBx.Text =
                      SoundSystem.SoundEngine.NameFinder.FindSound(
                      SoundSystem.Enums.SoundTypes.daggerFight1);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight1, new Vector3(x, y, z));
                    break;
                case "daggerFight2":
                    soundSourceTxtBx.Text =
                    SoundSystem.SoundEngine.NameFinder.FindSound(
                    SoundSystem.Enums.SoundTypes.daggerFight2);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight2, new Vector3(x, y, z));
                    break;
                case "daggerFight3":
                    soundSourceTxtBx.Text =
                      SoundSystem.SoundEngine.NameFinder.FindSound(
                      SoundSystem.Enums.SoundTypes.daggerFight3);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight3, new Vector3(x, y, z));
                    break;
                case "daggerFight4":
                    soundSourceTxtBx.Text =
                      SoundSystem.SoundEngine.NameFinder.FindSound(
                      SoundSystem.Enums.SoundTypes.daggerFight4);
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.daggerFight4, new Vector3(x, y, z));
                    break;
                case "daggerFight5":
                    soundSourceTxtBx.Text =
                      SoundSystem.SoundEngine.NameFinder.FindSound(
                      SoundSystem.Enums.SoundTypes.daggerFight5);
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
                case "arrowOff1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.arrowOff1, new Vector3(x, y, z));
                    break;
                case "arrowOff2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.arrowOff2, new Vector3(x, y, z));
                    break;
                case "crossbowOff":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.crossbowOff, new Vector3(x, y, z));
                    break;
                case "fanfare1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.fanfare1, new Vector3(x, y, z));
                    break;
                case "fanfare2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.fanfare2, new Vector3(x, y, z));
                    break;
                case "fanfare3":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.fanfare3, new Vector3(x, y, z));
                    break;
                case "fanfare4":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.fanfare4, new Vector3(x, y, z));
                    break;
                case "knightAaa":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightAaa, new Vector3(x, y, z));
                    break;
                case "assassinCought":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinCought, new Vector3(x, y, z));
                    break;
                case "manShotByArrow":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.manShotByArrow, new Vector3(x, y, z));
                    break;
                case "shotByArrow":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.shotByArrow, new Vector3(x, y, z));
                    break;
                case "manShotByCrossbow":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.manShotByCrossbow, new Vector3(x, y, z));
                    break;
                case "shotByCrossbow":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.shotByCrossbow, new Vector3(x, y, z));
                    break;
                case "knightHou":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightHou, new Vector3(x, y, z));
                    break;
                case "knightHouu":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightHouu, new Vector3(x, y, z));
                    break;
                case "knightHruuhb":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightHruuhb, new Vector3(x, y, z));
                    break;
                case "assassinJah":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinJah, new Vector3(x, y, z));
                    break;
                case "assassinJhuee":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinJhuee, new Vector3(x, y, z));
                    break;
                case "assassinJooaah":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinJooaah, new Vector3(x, y, z));
                    break;
                case "knightLongDeath":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightLongDeath, new Vector3(x, y, z));
                    break;
                case "assassinLongDeath":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinLongDeath, new Vector3(x, y, z));
                    break;
                case "knightNey":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightNey, new Vector3(x, y, z));
                    break;
                case "assassinOhm":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinOhm, new Vector3(x, y, z));
                    break;
                case "knightScream1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightScream1, new Vector3(x, y, z));
                    break;
                case "knightScream2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightScream2, new Vector3(x, y, z));
                    break;
                case "assassinScream1":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinScream1, new Vector3(x, y, z));
                    break;
                case "assassinScream2":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinScream2, new Vector3(x, y, z));
                    break;
                case "knightShortDeath":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightShortDeath, new Vector3(x, y, z));
                    break;
                case "assassinShortDeath":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinShortDeath, new Vector3(x, y, z));
                    break;
                case "knightUhraa":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightUhraa, new Vector3(x, y, z));
                    break;
                case "knightUueh":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.knightUueh, new Vector3(x, y, z));
                    break;
                case "assassinYeam":
                    SoundSystem.SoundEngine.PlaySound(
                        SoundSystem.Enums.SoundTypes.assassinYeam, new Vector3(x, y, z));
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
                case "menuMusic":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.menuMusic);
                    break;
                case "round1Music":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round1Music);
                    break;
                case "round2Music":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round2Music);
                    break;
                case "endingMusic":
                    SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.endingMusic);
                    break;
                default:
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SoundSystem.SoundEngine.StopMusic();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float x = (float)Convert.ToDouble(x2TxtBx.Text.ToString());
            float y = (float)Convert.ToDouble(y2TxtBx.Text.ToString());
            float z = (float)Convert.ToDouble(z2TxtBx.Text.ToString());

            switch (stepsListBx.Text)
            {             
                case "stepsInside1":
                   // isWalking = true;
                    SoundSystem.SoundDescriptor sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsInside1, new Vector3(x, y, z));
                    while (isWalking)
                    {
                        SoundSystem.SoundEngine.UpdateSoundList();
                    }
                    break;
                case "stepsInside2":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsInside2, new Vector3(x, y, z));
                    break;
                case "stepsInside3":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsInside3, new Vector3(x, y, z));
                    break;
                case "stepsSlower":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsSlower, new Vector3(x, y, z));
                    break;
                case "stepsStone":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsStone, new Vector3(x, y, z));
                    break;
                case "stepsStairs":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsStairs, new Vector3(x, y, z));
                    break;
                case "stepsGrass1":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsGrass1, new Vector3(x, y, z));
                    break;
                case "stepsGrass2":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsGrass2, new Vector3(x, y, z));
                    break;
                case "stepsPath1":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsPath1, new Vector3(x, y, z));
                    break;
                case "stepsPath2":
                    sd = SoundSystem.SoundEngine.StartSteps(
                            SoundSystem.Enums.SoundTypes.stepsPath2, new Vector3(x, y, z));
                    break;
                default:
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            isWalking = false;
        }

    }
}

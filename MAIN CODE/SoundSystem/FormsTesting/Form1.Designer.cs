namespace FormsTesting
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.soundsGrpBx = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.selectGrpBx = new System.Windows.Forms.GroupBox();
            this.soundsListBx = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.zTxtBx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.yTxtBx = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.xTxtBx = new System.Windows.Forms.TextBox();
            this.musicGrpBx = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.musicLstBx = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.soundSourceTxtBx = new System.Windows.Forms.TextBox();
            this.musicSourceTxtBx = new System.Windows.Forms.TextBox();
            this.soundsGrpBx.SuspendLayout();
            this.selectGrpBx.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.musicGrpBx.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // soundsGrpBx
            // 
            this.soundsGrpBx.Controls.Add(this.soundSourceTxtBx);
            this.soundsGrpBx.Controls.Add(this.button1);
            this.soundsGrpBx.Controls.Add(this.selectGrpBx);
            this.soundsGrpBx.Controls.Add(this.groupBox1);
            this.soundsGrpBx.Location = new System.Drawing.Point(12, 12);
            this.soundsGrpBx.Name = "soundsGrpBx";
            this.soundsGrpBx.Size = new System.Drawing.Size(467, 200);
            this.soundsGrpBx.TabIndex = 0;
            this.soundsGrpBx.TabStop = false;
            this.soundsGrpBx.Text = "Sound testing";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(384, 171);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // selectGrpBx
            // 
            this.selectGrpBx.Controls.Add(this.soundsListBx);
            this.selectGrpBx.Controls.Add(this.label1);
            this.selectGrpBx.Location = new System.Drawing.Point(6, 19);
            this.selectGrpBx.Name = "selectGrpBx";
            this.selectGrpBx.Size = new System.Drawing.Size(269, 175);
            this.selectGrpBx.TabIndex = 1;
            this.selectGrpBx.TabStop = false;
            this.selectGrpBx.Text = "select sound";
            // 
            // soundsListBx
            // 
            this.soundsListBx.FormattingEnabled = true;
            this.soundsListBx.Items.AddRange(new object[] {
            "swordFight1",
            "swordFight2",
            "swordFight3",
            "swordFight4",
            "swordFight5",
            "stepsInside1",
            "stepsInside2",
            "stepsInside3",
            "stepsSlower",
            "stepsStone",
            "stepsStairs",
            "stepsGrass1",
            "stepsGrass2",
            "stepsPath1",
            "stepsPath2",
            "daggerFight1",
            "daggerFigth2",
            "daggerFight3",
            "daggerFigth4",
            "daggerFight5",
            "arrowOff1",
            "arrowOff2",
            "manShotByArrow",
            "shotByArrow, //!",
            "crossbowOff",
            "manShotByCrossbow",
            "shotByCrossbow, //!",
            "mansShortDeath1",
            "mansShortDeath2",
            "mansLongDeath1",
            "mansLongDeath2",
            "mansAaa",
            "mansCought",
            "mansHou",
            "mansHouu",
            "mansHruuhb",
            "mansJah",
            "mansJhuee",
            "mansJooaah",
            "mansNey",
            "mansOhm",
            "mansScream1",
            "mansScream2",
            "mansScream3",
            "mansScream4",
            "mansUhraa",
            "mansUueh",
            "mansYeam",
            "manSaysHello, //!",
            "manSaysWassup, //!",
            "manSaysKidding, //!",
            "manSaysKillem, //!",
            "fanfare1",
            "fanfare2",
            "fanfare3",
            "fanfare4"});
            this.soundsListBx.Location = new System.Drawing.Point(86, 18);
            this.soundsListBx.Name = "soundsListBx";
            this.soundsListBx.Size = new System.Drawing.Size(177, 147);
            this.soundsListBx.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "type of sound:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.zTxtBx);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.yTxtBx);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.xTxtBx);
            this.groupBox1.Location = new System.Drawing.Point(281, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(97, 98);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "position";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Z:";
            // 
            // zTxtBx
            // 
            this.zTxtBx.Location = new System.Drawing.Point(29, 71);
            this.zTxtBx.Name = "zTxtBx";
            this.zTxtBx.Size = new System.Drawing.Size(49, 20);
            this.zTxtBx.TabIndex = 8;
            this.zTxtBx.Text = "3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Y:";
            // 
            // yTxtBx
            // 
            this.yTxtBx.Location = new System.Drawing.Point(29, 45);
            this.yTxtBx.Name = "yTxtBx";
            this.yTxtBx.Size = new System.Drawing.Size(49, 20);
            this.yTxtBx.TabIndex = 6;
            this.yTxtBx.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "X:";
            // 
            // xTxtBx
            // 
            this.xTxtBx.Location = new System.Drawing.Point(29, 19);
            this.xTxtBx.Name = "xTxtBx";
            this.xTxtBx.Size = new System.Drawing.Size(49, 20);
            this.xTxtBx.TabIndex = 0;
            this.xTxtBx.Text = "1";
            // 
            // musicGrpBx
            // 
            this.musicGrpBx.Controls.Add(this.musicSourceTxtBx);
            this.musicGrpBx.Controls.Add(this.button3);
            this.musicGrpBx.Controls.Add(this.button2);
            this.musicGrpBx.Controls.Add(this.groupBox2);
            this.musicGrpBx.Location = new System.Drawing.Point(12, 218);
            this.musicGrpBx.Name = "musicGrpBx";
            this.musicGrpBx.Size = new System.Drawing.Size(469, 129);
            this.musicGrpBx.TabIndex = 1;
            this.musicGrpBx.TabStop = false;
            this.musicGrpBx.Text = "Music testing";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(384, 100);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(290, 100);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Play";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.musicLstBx);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(8, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(269, 105);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "select music";
            // 
            // musicLstBx
            // 
            this.musicLstBx.FormattingEnabled = true;
            this.musicLstBx.Items.AddRange(new object[] {
            "welcomeMusic",
            "round1Music",
            "round2Music",
            "victoryMusic"});
            this.musicLstBx.Location = new System.Drawing.Point(86, 18);
            this.musicLstBx.Name = "musicLstBx";
            this.musicLstBx.Size = new System.Drawing.Size(177, 69);
            this.musicLstBx.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "type of music:";
            // 
            // soundSourceTxtBx
            // 
            this.soundSourceTxtBx.Location = new System.Drawing.Point(281, 145);
            this.soundSourceTxtBx.Name = "soundSourceTxtBx";
            this.soundSourceTxtBx.Size = new System.Drawing.Size(177, 20);
            this.soundSourceTxtBx.TabIndex = 7;
            // 
            // musicSourceTxtBx
            // 
            this.musicSourceTxtBx.Location = new System.Drawing.Point(286, 74);
            this.musicSourceTxtBx.Name = "musicSourceTxtBx";
            this.musicSourceTxtBx.Size = new System.Drawing.Size(177, 20);
            this.musicSourceTxtBx.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 359);
            this.Controls.Add(this.musicGrpBx);
            this.Controls.Add(this.soundsGrpBx);
            this.Name = "Form1";
            this.Text = "Testing SoundSystem";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.soundsGrpBx.ResumeLayout(false);
            this.soundsGrpBx.PerformLayout();
            this.selectGrpBx.ResumeLayout(false);
            this.selectGrpBx.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.musicGrpBx.ResumeLayout(false);
            this.musicGrpBx.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox soundsGrpBx;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox xTxtBx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox yTxtBx;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox zTxtBx;
        private System.Windows.Forms.GroupBox selectGrpBx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox soundsListBx;
        private System.Windows.Forms.GroupBox musicGrpBx;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox musicLstBx;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox soundSourceTxtBx;
        private System.Windows.Forms.TextBox musicSourceTxtBx;
    }
}


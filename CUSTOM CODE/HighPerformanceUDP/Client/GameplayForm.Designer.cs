namespace Client
{
    partial class GameplayForm
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
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnJump = new System.Windows.Forms.Button();
            this.btnShoot = new System.Windows.Forms.Button();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerUp = new System.Windows.Forms.SplitContainer();
            this.dgData = new System.Windows.Forms.DataGridView();
            this.txtEvents = new System.Windows.Forms.TextBox();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.splitContainerUp.Panel1.SuspendLayout();
            this.splitContainerUp.Panel2.SuspendLayout();
            this.splitContainerUp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(295, 14);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(45, 23);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(295, 43);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(45, 23);
            this.btnDown.TabIndex = 0;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(342, 43);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(45, 23);
            this.btnRight.TabIndex = 0;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(248, 43);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(45, 23);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnJump
            // 
            this.btnJump.Location = new System.Drawing.Point(46, 43);
            this.btnJump.Name = "btnJump";
            this.btnJump.Size = new System.Drawing.Size(61, 23);
            this.btnJump.TabIndex = 1;
            this.btnJump.Text = "Jump";
            this.btnJump.UseVisualStyleBackColor = true;
            this.btnJump.Click += new System.EventHandler(this.btnJump_Click);
            // 
            // btnShoot
            // 
            this.btnShoot.Location = new System.Drawing.Point(48, 14);
            this.btnShoot.Name = "btnShoot";
            this.btnShoot.Size = new System.Drawing.Size(59, 23);
            this.btnShoot.TabIndex = 2;
            this.btnShoot.Text = "Shoot";
            this.btnShoot.UseVisualStyleBackColor = true;
            this.btnShoot.Click += new System.EventHandler(this.btnShoot_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerUp);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.txtSend);
            this.splitContainerMain.Panel2.Controls.Add(this.txtX);
            this.splitContainerMain.Panel2.Controls.Add(this.txtY);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRight);
            this.splitContainerMain.Panel2.Controls.Add(this.btnShoot);
            this.splitContainerMain.Panel2.Controls.Add(this.btnUp);
            this.splitContainerMain.Panel2.Controls.Add(this.btnJump);
            this.splitContainerMain.Panel2.Controls.Add(this.btnDown);
            this.splitContainerMain.Panel2.Controls.Add(this.btnLeft);
            this.splitContainerMain.Size = new System.Drawing.Size(550, 369);
            this.splitContainerMain.SplitterDistance = 233;
            this.splitContainerMain.TabIndex = 3;
            // 
            // splitContainerUp
            // 
            this.splitContainerUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUp.Location = new System.Drawing.Point(0, 0);
            this.splitContainerUp.Name = "splitContainerUp";
            // 
            // splitContainerUp.Panel1
            // 
            this.splitContainerUp.Panel1.Controls.Add(this.dgData);
            // 
            // splitContainerUp.Panel2
            // 
            this.splitContainerUp.Panel2.Controls.Add(this.txtEvents);
            this.splitContainerUp.Size = new System.Drawing.Size(550, 233);
            this.splitContainerUp.SplitterDistance = 270;
            this.splitContainerUp.TabIndex = 0;
            // 
            // dgData
            // 
            this.dgData.AllowUserToAddRows = false;
            this.dgData.AllowUserToDeleteRows = false;
            this.dgData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgData.Location = new System.Drawing.Point(0, 0);
            this.dgData.Name = "dgData";
            this.dgData.ReadOnly = true;
            this.dgData.Size = new System.Drawing.Size(270, 233);
            this.dgData.TabIndex = 0;
            // 
            // txtEvents
            // 
            this.txtEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEvents.Location = new System.Drawing.Point(0, 0);
            this.txtEvents.Multiline = true;
            this.txtEvents.Name = "txtEvents";
            this.txtEvents.ReadOnly = true;
            this.txtEvents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEvents.Size = new System.Drawing.Size(276, 233);
            this.txtEvents.TabIndex = 0;
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(158, 45);
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            this.txtX.Size = new System.Drawing.Size(39, 20);
            this.txtX.TabIndex = 3;
            this.txtX.Text = "0";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(203, 45);
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            this.txtY.Size = new System.Drawing.Size(39, 20);
            this.txtY.TabIndex = 3;
            this.txtY.Text = "0";
            // 
            // txtSend
            // 
            this.txtSend.Location = new System.Drawing.Point(3, 72);
            this.txtSend.Multiline = true;
            this.txtSend.Name = "txtSend";
            this.txtSend.ReadOnly = true;
            this.txtSend.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSend.Size = new System.Drawing.Size(544, 57);
            this.txtSend.TabIndex = 4;
            // 
            // GameplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 369);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "GameplayForm";
            this.Text = "GameplayForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameplayForm_FormClosing);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerUp.Panel1.ResumeLayout(false);
            this.splitContainerUp.Panel2.ResumeLayout(false);
            this.splitContainerUp.Panel2.PerformLayout();
            this.splitContainerUp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnJump;
        private System.Windows.Forms.Button btnShoot;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerUp;
        private System.Windows.Forms.DataGridView dgData;
        private System.Windows.Forms.TextBox txtEvents;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.TextBox txtSend;
    }
}
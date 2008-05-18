namespace Framework
{
    partial class FrameworkWindow
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
            if (this.IsDisposed)
                return;

            GM.GeneralLog.BeginBlock("Window started disposing");
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                if (m_device != null && m_device.Disposed == false)
                    this.m_device.Dispose();
            }
            base.Dispose(disposing);
            GM.GeneralLog.EndBlock("Window finished disposing");
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Kulki";
        }

        #endregion
    }
}


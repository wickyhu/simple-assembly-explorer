namespace SimpleAssemblyExplorer
{
    partial class frmVerify
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
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.txtAdditionalOptions = new System.Windows.Forms.TextBox();
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkHResult = new System.Windows.Forms.CheckBox();
            this.chkMD = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkIL = new System.Windows.Forms.CheckBox();
            this.chkClock = new System.Windows.Forms.CheckBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.chkNoLogo = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(169, 61);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(281, 61);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 91);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 396);
            this.txtInfo.TabIndex = 11;
            this.txtInfo.WordWrap = false;
            // 
            // txtAdditionalOptions
            // 
            this.txtAdditionalOptions.Location = new System.Drawing.Point(87, 34);
            this.txtAdditionalOptions.Name = "txtAdditionalOptions";
            this.txtAdditionalOptions.Size = new System.Drawing.Size(537, 21);
            this.txtAdditionalOptions.TabIndex = 7;
            this.toolTip1.SetToolTip(this.txtAdditionalOptions, "Enter additional command line arguments here");
            // 
            // chkUnique
            // 
            this.chkUnique.AutoSize = true;
            this.chkUnique.Location = new System.Drawing.Point(212, 12);
            this.chkUnique.Name = "chkUnique";
            this.chkUnique.Size = new System.Drawing.Size(59, 17);
            this.chkUnique.TabIndex = 3;
            this.chkUnique.Text = "Unique";
            this.chkUnique.UseVisualStyleBackColor = true;
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Checked = true;
            this.chkVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVerbose.Location = new System.Drawing.Point(366, 12);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 5;
            this.chkVerbose.Text = "Verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // chkHResult
            // 
            this.chkHResult.AutoSize = true;
            this.chkHResult.Checked = true;
            this.chkHResult.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHResult.Location = new System.Drawing.Point(291, 12);
            this.chkHResult.Name = "chkHResult";
            this.chkHResult.Size = new System.Drawing.Size(63, 17);
            this.chkHResult.TabIndex = 4;
            this.chkHResult.Text = "HResult";
            this.chkHResult.UseVisualStyleBackColor = true;
            // 
            // chkMD
            // 
            this.chkMD.AutoSize = true;
            this.chkMD.Location = new System.Drawing.Point(146, 12);
            this.chkMD.Name = "chkMD";
            this.chkMD.Size = new System.Drawing.Size(41, 17);
            this.chkMD.TabIndex = 2;
            this.chkMD.Text = "MD";
            this.chkMD.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Options";
            // 
            // chkIL
            // 
            this.chkIL.AutoSize = true;
            this.chkIL.Location = new System.Drawing.Point(87, 12);
            this.chkIL.Name = "chkIL";
            this.chkIL.Size = new System.Drawing.Size(35, 17);
            this.chkIL.TabIndex = 1;
            this.chkIL.Text = "IL";
            this.chkIL.UseVisualStyleBackColor = true;
            // 
            // chkClock
            // 
            this.chkClock.AutoSize = true;
            this.chkClock.Location = new System.Drawing.Point(437, 12);
            this.chkClock.Name = "chkClock";
            this.chkClock.Size = new System.Drawing.Size(51, 17);
            this.chkClock.TabIndex = 6;
            this.chkClock.Text = "Clock";
            this.chkClock.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(389, 61);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 24);
            this.btnHelp.TabIndex = 10;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // chkNoLogo
            // 
            this.chkNoLogo.AutoSize = true;
            this.chkNoLogo.Checked = true;
            this.chkNoLogo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoLogo.Location = new System.Drawing.Point(494, 12);
            this.chkNoLogo.Name = "chkNoLogo";
            this.chkNoLogo.Size = new System.Drawing.Size(65, 17);
            this.chkNoLogo.TabIndex = 12;
            this.chkNoLogo.Text = "No Logo";
            this.chkNoLogo.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // frmVerify
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(671, 496);
            this.Controls.Add(this.chkNoLogo);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.chkClock);
            this.Controls.Add(this.txtAdditionalOptions);
            this.Controls.Add(this.chkUnique);
            this.Controls.Add(this.chkVerbose);
            this.Controls.Add(this.chkHResult);
            this.Controls.Add(this.chkMD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkIL);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmVerify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PE Verify";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.TextBox txtAdditionalOptions;
        private System.Windows.Forms.CheckBox chkUnique;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.CheckBox chkHResult;
        private System.Windows.Forms.CheckBox chkMD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkIL;
        private System.Windows.Forms.CheckBox chkClock;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.CheckBox chkNoLogo;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
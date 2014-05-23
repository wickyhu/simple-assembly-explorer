namespace SimpleAssemblyExplorer
{
    partial class frmStrongName
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
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.rbRemoveSN = new System.Windows.Forms.RadioButton();
            this.rbSign = new System.Windows.Forms.RadioButton();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.txtKeyFile = new System.Windows.Forms.TextBox();
            this.rbVr = new System.Windows.Forms.RadioButton();
            this.rbVu = new System.Windows.Forms.RadioButton();
            this.rbvf = new System.Windows.Forms.RadioButton();
            this.rbGacInstall = new System.Windows.Forms.RadioButton();
            this.rbGacRemove = new System.Windows.Forms.RadioButton();
            this.btnSelectOutput = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.rbVl = new System.Windows.Forms.RadioButton();
            this.btnHelp = new System.Windows.Forms.Button();
            this.txtAdditionalOptions = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rbVx = new System.Windows.Forms.RadioButton();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.chkQuiet = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(145, 125);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(256, 125);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 155);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 367);
            this.txtInfo.TabIndex = 21;
            this.txtInfo.WordWrap = false;
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.Location = new System.Drawing.Point(46, 33);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(132, 17);
            this.chkOverwrite.TabIndex = 4;
            this.chkOverwrite.Text = "Overwrite Original File";
            this.chkOverwrite.UseVisualStyleBackColor = true;
            this.chkOverwrite.CheckedChanged += new System.EventHandler(this.chkOverwrite_CheckedChanged);
            // 
            // rbRemoveSN
            // 
            this.rbRemoveSN.AutoSize = true;
            this.rbRemoveSN.Checked = true;
            this.rbRemoveSN.Location = new System.Drawing.Point(27, 10);
            this.rbRemoveSN.Name = "rbRemoveSN";
            this.rbRemoveSN.Size = new System.Drawing.Size(129, 17);
            this.rbRemoveSN.TabIndex = 0;
            this.rbRemoveSN.TabStop = true;
            this.rbRemoveSN.Text = "Remove Strong Name";
            this.rbRemoveSN.UseVisualStyleBackColor = true;
            this.rbRemoveSN.CheckedChanged += new System.EventHandler(this.rbRemoveSN_CheckedChanged);
            // 
            // rbSign
            // 
            this.rbSign.AutoSize = true;
            this.rbSign.Location = new System.Drawing.Point(211, 10);
            this.rbSign.Name = "rbSign";
            this.rbSign.Size = new System.Drawing.Size(93, 17);
            this.rbSign.TabIndex = 1;
            this.rbSign.Text = "Sign Assembly";
            this.rbSign.UseVisualStyleBackColor = true;
            this.rbSign.CheckedChanged += new System.EventHandler(this.rbSign_CheckedChanged);
            // 
            // btnSelFile
            // 
            this.btnSelFile.Enabled = false;
            this.btnSelFile.Location = new System.Drawing.Point(617, 9);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(27, 21);
            this.btnSelFile.TabIndex = 3;
            this.btnSelFile.Text = "...";
            this.btnSelFile.UseVisualStyleBackColor = true;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // txtKeyFile
            // 
            this.txtKeyFile.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtKeyFile.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtKeyFile.Enabled = false;
            this.txtKeyFile.Location = new System.Drawing.Point(310, 9);
            this.txtKeyFile.Name = "txtKeyFile";
            this.txtKeyFile.Size = new System.Drawing.Size(301, 21);
            this.txtKeyFile.TabIndex = 2;
            // 
            // rbVr
            // 
            this.rbVr.AutoSize = true;
            this.rbVr.Location = new System.Drawing.Point(27, 56);
            this.rbVr.Name = "rbVr";
            this.rbVr.Size = new System.Drawing.Size(35, 17);
            this.rbVr.TabIndex = 8;
            this.rbVr.Text = "Vr";
            this.toolTip1.SetToolTip(this.rbVr, "Register <assembly> for verification skipping");
            this.rbVr.UseVisualStyleBackColor = true;
            this.rbVr.CheckedChanged += new System.EventHandler(this.rbVr_CheckedChanged);
            // 
            // rbVu
            // 
            this.rbVu.AutoSize = true;
            this.rbVu.Location = new System.Drawing.Point(68, 56);
            this.rbVu.Name = "rbVu";
            this.rbVu.Size = new System.Drawing.Size(37, 17);
            this.rbVu.TabIndex = 9;
            this.rbVu.Text = "Vu";
            this.toolTip1.SetToolTip(this.rbVu, "Unregister <assembly> for verification skipping");
            this.rbVu.UseVisualStyleBackColor = true;
            this.rbVu.CheckedChanged += new System.EventHandler(this.rbVu_CheckedChanged);
            // 
            // rbvf
            // 
            this.rbvf.AutoSize = true;
            this.rbvf.Location = new System.Drawing.Point(150, 56);
            this.rbvf.Name = "rbvf";
            this.rbvf.Size = new System.Drawing.Size(35, 17);
            this.rbvf.TabIndex = 11;
            this.rbvf.Text = "vf";
            this.toolTip1.SetToolTip(this.rbvf, "Verify <assembly> for strong name signature self consistency");
            this.rbvf.UseVisualStyleBackColor = true;
            // 
            // rbGacInstall
            // 
            this.rbGacInstall.AutoSize = true;
            this.rbGacInstall.Location = new System.Drawing.Point(27, 102);
            this.rbGacInstall.Name = "rbGacInstall";
            this.rbGacInstall.Size = new System.Drawing.Size(91, 17);
            this.rbGacInstall.TabIndex = 16;
            this.rbGacInstall.Text = "Install to GAC";
            this.rbGacInstall.UseVisualStyleBackColor = true;
            // 
            // rbGacRemove
            // 
            this.rbGacRemove.AutoSize = true;
            this.rbGacRemove.Location = new System.Drawing.Point(211, 102);
            this.rbGacRemove.Name = "rbGacRemove";
            this.rbGacRemove.Size = new System.Drawing.Size(113, 17);
            this.rbGacRemove.TabIndex = 17;
            this.rbGacRemove.Text = "Remove from GAC";
            this.rbGacRemove.UseVisualStyleBackColor = true;
            // 
            // btnSelectOutput
            // 
            this.btnSelectOutput.Location = new System.Drawing.Point(617, 31);
            this.btnSelectOutput.Name = "btnSelectOutput";
            this.btnSelectOutput.Size = new System.Drawing.Size(27, 21);
            this.btnSelectOutput.TabIndex = 7;
            this.btnSelectOutput.Text = "...";
            this.btnSelectOutput.UseVisualStyleBackColor = true;
            this.btnSelectOutput.Click += new System.EventHandler(this.btnSelectOutput_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(310, 31);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(301, 21);
            this.txtOutputDir.TabIndex = 6;
            this.txtOutputDir.Leave += new System.EventHandler(this.txtOutputDir_Leave);
            // 
            // lblOutputDir
            // 
            this.lblOutputDir.AutoSize = true;
            this.lblOutputDir.Location = new System.Drawing.Point(208, 34);
            this.lblOutputDir.Name = "lblOutputDir";
            this.lblOutputDir.Size = new System.Drawing.Size(88, 13);
            this.lblOutputDir.TabIndex = 5;
            this.lblOutputDir.Text = "Output Directory";
            // 
            // rbVl
            // 
            this.rbVl.AutoSize = true;
            this.rbVl.Location = new System.Drawing.Point(111, 56);
            this.rbVl.Name = "rbVl";
            this.rbVl.Size = new System.Drawing.Size(33, 17);
            this.rbVl.TabIndex = 10;
            this.rbVl.Text = "Vl";
            this.toolTip1.SetToolTip(this.rbVl, "List current settings for strong name verification on this machine");
            this.rbVl.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(363, 125);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 24);
            this.btnHelp.TabIndex = 20;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // txtAdditionalOptions
            // 
            this.txtAdditionalOptions.Location = new System.Drawing.Point(94, 79);
            this.txtAdditionalOptions.Name = "txtAdditionalOptions";
            this.txtAdditionalOptions.Size = new System.Drawing.Size(517, 21);
            this.txtAdditionalOptions.TabIndex = 15;
            this.toolTip1.SetToolTip(this.txtAdditionalOptions, "Enter custom command line arguments here");
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // rbVx
            // 
            this.rbVx.AutoSize = true;
            this.rbVx.Location = new System.Drawing.Point(191, 56);
            this.rbVx.Name = "rbVx";
            this.rbVx.Size = new System.Drawing.Size(37, 17);
            this.rbVx.TabIndex = 12;
            this.rbVx.Text = "Vx";
            this.toolTip1.SetToolTip(this.rbVx, "Remove all verification skipping entries");
            this.rbVx.UseVisualStyleBackColor = true;
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(27, 79);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(61, 17);
            this.rbCustom.TabIndex = 14;
            this.rbCustom.Text = "Custom";
            this.rbCustom.UseVisualStyleBackColor = true;
            this.rbCustom.CheckedChanged += new System.EventHandler(this.rbCustom_CheckedChanged);
            // 
            // chkQuiet
            // 
            this.chkQuiet.AutoSize = true;
            this.chkQuiet.Checked = true;
            this.chkQuiet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuiet.Location = new System.Drawing.Point(310, 58);
            this.chkQuiet.Name = "chkQuiet";
            this.chkQuiet.Size = new System.Drawing.Size(52, 17);
            this.chkQuiet.TabIndex = 13;
            this.chkQuiet.Text = "Quiet";
            this.chkQuiet.UseVisualStyleBackColor = true;
            // 
            // frmStrongName
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(674, 534);
            this.Controls.Add(this.chkQuiet);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.rbVx);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.txtAdditionalOptions);
            this.Controls.Add(this.rbVl);
            this.Controls.Add(this.btnSelectOutput);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.lblOutputDir);
            this.Controls.Add(this.rbGacRemove);
            this.Controls.Add(this.rbGacInstall);
            this.Controls.Add(this.rbvf);
            this.Controls.Add(this.rbVu);
            this.Controls.Add(this.rbVr);
            this.Controls.Add(this.btnSelFile);
            this.Controls.Add(this.txtKeyFile);
            this.Controls.Add(this.rbSign);
            this.Controls.Add(this.rbRemoveSN);
            this.Controls.Add(this.chkOverwrite);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmStrongName";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Strong Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.RadioButton rbRemoveSN;
        private System.Windows.Forms.RadioButton rbSign;
        private System.Windows.Forms.Button btnSelFile;
        private System.Windows.Forms.TextBox txtKeyFile;
        private System.Windows.Forms.RadioButton rbVr;
        private System.Windows.Forms.RadioButton rbVu;
        private System.Windows.Forms.RadioButton rbvf;
        private System.Windows.Forms.RadioButton rbGacInstall;
        private System.Windows.Forms.RadioButton rbGacRemove;
        private System.Windows.Forms.Button btnSelectOutput;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Label lblOutputDir;
        private System.Windows.Forms.RadioButton rbVl;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TextBox txtAdditionalOptions;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton rbVx;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.CheckBox chkQuiet;
    }
}
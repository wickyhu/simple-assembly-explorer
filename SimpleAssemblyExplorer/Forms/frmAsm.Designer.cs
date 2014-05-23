namespace SimpleAssemblyExplorer
{
    partial class frmAsm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnSelectOutput = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.txtKeyFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rbDbg = new System.Windows.Forms.RadioButton();
            this.rbDbgImpl = new System.Windows.Forms.RadioButton();
            this.rbDbgOpt = new System.Windows.Forms.RadioButton();
            this.rbDbgNo = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.chkReplaceToken = new System.Windows.Forms.CheckBox();
            this.txtOldToken = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNewToken = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtAdditionalOptions = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbSNSign = new System.Windows.Forms.RadioButton();
            this.rbSNNo = new System.Windows.Forms.RadioButton();
            this.rbSNRemove = new System.Windows.Forms.RadioButton();
            this.btnSelReplFile = new System.Windows.Forms.Button();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.chkRemoveLicenseProvider = new System.Windows.Forms.CheckBox();
            this.chkQuiet = new System.Windows.Forms.CheckBox();
            this.chkNoLogo = new System.Windows.Forms.CheckBox();
            this.chkClock = new System.Windows.Forms.CheckBox();
            this.chkItanium = new System.Windows.Forms.CheckBox();
            this.chkX64 = new System.Windows.Forms.CheckBox();
            this.chkOptimize = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbTypeAuto = new System.Windows.Forms.RadioButton();
            this.rbTypeDll = new System.Windows.Forms.RadioButton();
            this.rbTypeExe = new System.Windows.Forms.RadioButton();
            this.btnHelp = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Output Directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(117, 12);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(502, 21);
            this.txtOutputDir.TabIndex = 0;
            this.txtOutputDir.Leave += new System.EventHandler(this.txtOutputDir_Leave);
            // 
            // btnSelectOutput
            // 
            this.btnSelectOutput.Location = new System.Drawing.Point(624, 12);
            this.btnSelectOutput.Name = "btnSelectOutput";
            this.btnSelectOutput.Size = new System.Drawing.Size(27, 21);
            this.btnSelectOutput.TabIndex = 1;
            this.btnSelectOutput.Text = "...";
            this.btnSelectOutput.UseVisualStyleBackColor = true;
            this.btnSelectOutput.Click += new System.EventHandler(this.btnSelectOutputDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(171, 188);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(290, 188);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 218);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 299);
            this.txtInfo.TabIndex = 23;
            // 
            // txtKeyFile
            // 
            this.txtKeyFile.Enabled = false;
            this.txtKeyFile.Location = new System.Drawing.Point(297, 88);
            this.txtKeyFile.Name = "txtKeyFile";
            this.txtKeyFile.Size = new System.Drawing.Size(322, 21);
            this.txtKeyFile.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Strong Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Options";
            // 
            // rbDbg
            // 
            this.rbDbg.AutoSize = true;
            this.rbDbg.Location = new System.Drawing.Point(52, 0);
            this.rbDbg.Name = "rbDbg";
            this.rbDbg.Size = new System.Drawing.Size(62, 17);
            this.rbDbg.TabIndex = 1;
            this.rbDbg.Text = "/DEBUG";
            this.rbDbg.UseVisualStyleBackColor = true;
            // 
            // rbDbgImpl
            // 
            this.rbDbgImpl.AutoSize = true;
            this.rbDbgImpl.Location = new System.Drawing.Point(120, 0);
            this.rbDbgImpl.Name = "rbDbgImpl";
            this.rbDbgImpl.Size = new System.Drawing.Size(93, 17);
            this.rbDbgImpl.TabIndex = 2;
            this.rbDbgImpl.Text = "/DEBUG=IMPL";
            this.rbDbgImpl.UseVisualStyleBackColor = true;
            // 
            // rbDbgOpt
            // 
            this.rbDbgOpt.AutoSize = true;
            this.rbDbgOpt.Location = new System.Drawing.Point(219, 0);
            this.rbDbgOpt.Name = "rbDbgOpt";
            this.rbDbgOpt.Size = new System.Drawing.Size(90, 17);
            this.rbDbgOpt.TabIndex = 3;
            this.rbDbgOpt.Text = "/DEBUG=OPT";
            this.rbDbgOpt.UseVisualStyleBackColor = true;
            // 
            // rbDbgNo
            // 
            this.rbDbgNo.AutoSize = true;
            this.rbDbgNo.Checked = true;
            this.rbDbgNo.Location = new System.Drawing.Point(3, 0);
            this.rbDbgNo.Name = "rbDbgNo";
            this.rbDbgNo.Size = new System.Drawing.Size(43, 17);
            this.rbDbgNo.TabIndex = 0;
            this.rbDbgNo.TabStop = true;
            this.rbDbgNo.Text = "N/A";
            this.rbDbgNo.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Other Options";
            // 
            // chkReplaceToken
            // 
            this.chkReplaceToken.AutoSize = true;
            this.chkReplaceToken.Location = new System.Drawing.Point(117, 143);
            this.chkReplaceToken.Name = "chkReplaceToken";
            this.chkReplaceToken.Size = new System.Drawing.Size(207, 17);
            this.chkReplaceToken.TabIndex = 14;
            this.chkReplaceToken.Text = "Replace References\' Public Key Token";
            this.toolTip1.SetToolTip(this.chkReplaceToken, ".Net 2.0: Better not to use disassembler \"CA Verbal\" option with this");
            this.chkReplaceToken.UseVisualStyleBackColor = true;
            this.chkReplaceToken.CheckedChanged += new System.EventHandler(this.chkReplaceToken_CheckedChanged);
            // 
            // txtOldToken
            // 
            this.txtOldToken.Enabled = false;
            this.txtOldToken.Location = new System.Drawing.Point(336, 141);
            this.txtOldToken.Name = "txtOldToken";
            this.txtOldToken.Size = new System.Drawing.Size(122, 21);
            this.txtOldToken.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(464, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "with";
            // 
            // txtNewToken
            // 
            this.txtNewToken.Enabled = false;
            this.txtNewToken.Location = new System.Drawing.Point(497, 141);
            this.txtNewToken.Name = "txtNewToken";
            this.txtNewToken.Size = new System.Drawing.Size(122, 21);
            this.txtNewToken.TabIndex = 17;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // txtAdditionalOptions
            // 
            this.txtAdditionalOptions.Location = new System.Drawing.Point(117, 114);
            this.txtAdditionalOptions.Name = "txtAdditionalOptions";
            this.txtAdditionalOptions.Size = new System.Drawing.Size(502, 21);
            this.txtAdditionalOptions.TabIndex = 13;
            this.toolTip1.SetToolTip(this.txtAdditionalOptions, "Enter additional command line arguments here");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbDbgNo);
            this.panel1.Controls.Add(this.rbDbg);
            this.panel1.Controls.Add(this.rbDbgImpl);
            this.panel1.Controls.Add(this.rbDbgOpt);
            this.panel1.Location = new System.Drawing.Point(295, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 20);
            this.panel1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbSNSign);
            this.panel2.Controls.Add(this.rbSNNo);
            this.panel2.Controls.Add(this.rbSNRemove);
            this.panel2.Location = new System.Drawing.Point(116, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(174, 20);
            this.panel2.TabIndex = 10;
            // 
            // rbSNSign
            // 
            this.rbSNSign.AutoSize = true;
            this.rbSNSign.Location = new System.Drawing.Point(120, 0);
            this.rbSNSign.Name = "rbSNSign";
            this.rbSNSign.Size = new System.Drawing.Size(45, 17);
            this.rbSNSign.TabIndex = 2;
            this.rbSNSign.Text = "Sign";
            this.rbSNSign.UseVisualStyleBackColor = true;
            this.rbSNSign.CheckedChanged += new System.EventHandler(this.rbSNSign_CheckedChanged);
            // 
            // rbSNNo
            // 
            this.rbSNNo.AutoSize = true;
            this.rbSNNo.Checked = true;
            this.rbSNNo.Location = new System.Drawing.Point(3, 0);
            this.rbSNNo.Name = "rbSNNo";
            this.rbSNNo.Size = new System.Drawing.Size(43, 17);
            this.rbSNNo.TabIndex = 0;
            this.rbSNNo.TabStop = true;
            this.rbSNNo.Text = "N/A";
            this.rbSNNo.UseVisualStyleBackColor = true;
            // 
            // rbSNRemove
            // 
            this.rbSNRemove.AutoSize = true;
            this.rbSNRemove.Location = new System.Drawing.Point(52, 0);
            this.rbSNRemove.Name = "rbSNRemove";
            this.rbSNRemove.Size = new System.Drawing.Size(64, 17);
            this.rbSNRemove.TabIndex = 1;
            this.rbSNRemove.Text = "Remove";
            this.rbSNRemove.UseVisualStyleBackColor = true;
            this.rbSNRemove.CheckedChanged += new System.EventHandler(this.rbSNRemove_CheckedChanged);
            // 
            // btnSelReplFile
            // 
            this.btnSelReplFile.Enabled = false;
            this.btnSelReplFile.Location = new System.Drawing.Point(624, 141);
            this.btnSelReplFile.Name = "btnSelReplFile";
            this.btnSelReplFile.Size = new System.Drawing.Size(27, 21);
            this.btnSelReplFile.TabIndex = 18;
            this.btnSelReplFile.Text = "...";
            this.btnSelReplFile.UseVisualStyleBackColor = true;
            this.btnSelReplFile.Click += new System.EventHandler(this.btnSelReplFile_Click);
            // 
            // btnSelFile
            // 
            this.btnSelFile.Enabled = false;
            this.btnSelFile.Location = new System.Drawing.Point(624, 88);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(27, 21);
            this.btnSelFile.TabIndex = 12;
            this.btnSelFile.Text = "...";
            this.btnSelFile.UseVisualStyleBackColor = true;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // chkRemoveLicenseProvider
            // 
            this.chkRemoveLicenseProvider.AutoSize = true;
            this.chkRemoveLicenseProvider.Location = new System.Drawing.Point(117, 165);
            this.chkRemoveLicenseProvider.Name = "chkRemoveLicenseProvider";
            this.chkRemoveLicenseProvider.Size = new System.Drawing.Size(186, 17);
            this.chkRemoveLicenseProvider.TabIndex = 19;
            this.chkRemoveLicenseProvider.Text = "Remove LicenseProviderAttribute";
            this.chkRemoveLicenseProvider.UseVisualStyleBackColor = true;
            // 
            // chkQuiet
            // 
            this.chkQuiet.AutoSize = true;
            this.chkQuiet.Checked = true;
            this.chkQuiet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQuiet.Location = new System.Drawing.Point(309, 40);
            this.chkQuiet.Name = "chkQuiet";
            this.chkQuiet.Size = new System.Drawing.Size(52, 17);
            this.chkQuiet.TabIndex = 5;
            this.chkQuiet.Text = "Quiet";
            this.chkQuiet.UseVisualStyleBackColor = true;
            // 
            // chkNoLogo
            // 
            this.chkNoLogo.AutoSize = true;
            this.chkNoLogo.Checked = true;
            this.chkNoLogo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoLogo.Location = new System.Drawing.Point(424, 40);
            this.chkNoLogo.Name = "chkNoLogo";
            this.chkNoLogo.Size = new System.Drawing.Size(65, 17);
            this.chkNoLogo.TabIndex = 7;
            this.chkNoLogo.Text = "No Logo";
            this.chkNoLogo.UseVisualStyleBackColor = true;
            // 
            // chkClock
            // 
            this.chkClock.AutoSize = true;
            this.chkClock.Location = new System.Drawing.Point(367, 40);
            this.chkClock.Name = "chkClock";
            this.chkClock.Size = new System.Drawing.Size(51, 17);
            this.chkClock.TabIndex = 6;
            this.chkClock.Text = "Clock";
            this.chkClock.UseVisualStyleBackColor = true;
            // 
            // chkItanium
            // 
            this.chkItanium.AutoSize = true;
            this.chkItanium.Location = new System.Drawing.Point(240, 40);
            this.chkItanium.Name = "chkItanium";
            this.chkItanium.Size = new System.Drawing.Size(62, 17);
            this.chkItanium.TabIndex = 4;
            this.chkItanium.Text = "Itanium";
            this.chkItanium.UseVisualStyleBackColor = true;
            this.chkItanium.CheckedChanged += new System.EventHandler(this.chkItanium_CheckedChanged);
            // 
            // chkX64
            // 
            this.chkX64.AutoSize = true;
            this.chkX64.Location = new System.Drawing.Point(190, 40);
            this.chkX64.Name = "chkX64";
            this.chkX64.Size = new System.Drawing.Size(44, 17);
            this.chkX64.TabIndex = 3;
            this.chkX64.Text = "X64";
            this.chkX64.UseVisualStyleBackColor = true;
            this.chkX64.CheckedChanged += new System.EventHandler(this.chkX64_CheckedChanged);
            // 
            // chkOptimize
            // 
            this.chkOptimize.AutoSize = true;
            this.chkOptimize.Location = new System.Drawing.Point(117, 40);
            this.chkOptimize.Name = "chkOptimize";
            this.chkOptimize.Size = new System.Drawing.Size(67, 17);
            this.chkOptimize.TabIndex = 2;
            this.chkOptimize.Text = "Optimize";
            this.chkOptimize.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbTypeAuto);
            this.panel3.Controls.Add(this.rbTypeDll);
            this.panel3.Controls.Add(this.rbTypeExe);
            this.panel3.Location = new System.Drawing.Point(116, 62);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(147, 20);
            this.panel3.TabIndex = 8;
            // 
            // rbTypeAuto
            // 
            this.rbTypeAuto.AutoSize = true;
            this.rbTypeAuto.Checked = true;
            this.rbTypeAuto.Location = new System.Drawing.Point(3, 0);
            this.rbTypeAuto.Name = "rbTypeAuto";
            this.rbTypeAuto.Size = new System.Drawing.Size(48, 17);
            this.rbTypeAuto.TabIndex = 0;
            this.rbTypeAuto.TabStop = true;
            this.rbTypeAuto.Text = "Auto";
            this.rbTypeAuto.UseVisualStyleBackColor = true;
            // 
            // rbTypeDll
            // 
            this.rbTypeDll.AutoSize = true;
            this.rbTypeDll.Location = new System.Drawing.Point(52, 0);
            this.rbTypeDll.Name = "rbTypeDll";
            this.rbTypeDll.Size = new System.Drawing.Size(36, 17);
            this.rbTypeDll.TabIndex = 1;
            this.rbTypeDll.Text = "Dll";
            this.rbTypeDll.UseVisualStyleBackColor = true;
            // 
            // rbTypeExe
            // 
            this.rbTypeExe.AutoSize = true;
            this.rbTypeExe.Location = new System.Drawing.Point(94, 0);
            this.rbTypeExe.Name = "rbTypeExe";
            this.rbTypeExe.Size = new System.Drawing.Size(43, 17);
            this.rbTypeExe.TabIndex = 2;
            this.rbTypeExe.Text = "Exe";
            this.rbTypeExe.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(408, 188);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 24);
            this.btnHelp.TabIndex = 22;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // frmAsm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(671, 527);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.chkNoLogo);
            this.Controls.Add(this.chkClock);
            this.Controls.Add(this.txtAdditionalOptions);
            this.Controls.Add(this.chkItanium);
            this.Controls.Add(this.chkX64);
            this.Controls.Add(this.chkOptimize);
            this.Controls.Add(this.chkQuiet);
            this.Controls.Add(this.chkRemoveLicenseProvider);
            this.Controls.Add(this.btnSelReplFile);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtNewToken);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOldToken);
            this.Controls.Add(this.chkReplaceToken);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSelFile);
            this.Controls.Add(this.txtKeyFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectOutput);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmAsm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assembler";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnSelectOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.TextBox txtKeyFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbDbg;
        private System.Windows.Forms.RadioButton rbDbgImpl;
        private System.Windows.Forms.RadioButton rbDbgOpt;
        private System.Windows.Forms.RadioButton rbDbgNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkReplaceToken;
        private System.Windows.Forms.TextBox txtOldToken;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNewToken;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbSNSign;
        private System.Windows.Forms.RadioButton rbSNNo;
        private System.Windows.Forms.RadioButton rbSNRemove;
        private System.Windows.Forms.Button btnSelReplFile;
        private System.Windows.Forms.Button btnSelFile;
        private System.Windows.Forms.CheckBox chkRemoveLicenseProvider;
        private System.Windows.Forms.CheckBox chkQuiet;
        private System.Windows.Forms.CheckBox chkNoLogo;
        private System.Windows.Forms.CheckBox chkClock;
        private System.Windows.Forms.TextBox txtAdditionalOptions;
        private System.Windows.Forms.CheckBox chkItanium;
        private System.Windows.Forms.CheckBox chkX64;
        private System.Windows.Forms.CheckBox chkOptimize;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbTypeAuto;
        private System.Windows.Forms.RadioButton rbTypeDll;
        private System.Windows.Forms.RadioButton rbTypeExe;
        private System.Windows.Forms.Button btnHelp;
    }
}
namespace SAE.de4dot
{
    partial class frmDe4dot
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboExe = new System.Windows.Forms.ComboBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.txtAdditional = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rbDeobf = new System.Windows.Forms.RadioButton();
            this.rbDetect = new System.Windows.Forms.RadioButton();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkCreateOutputDir = new System.Windows.Forms.CheckBox();
            this.chkScanDir = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkPreserveTokens = new System.Windows.Forms.CheckBox();
            this.chkDontRename = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboStringDecrypter = new System.Windows.Forms.ComboBox();
            this.txtStringDecrypterMethod = new System.Windows.Forms.TextBox();
            this.btnSelectMethod = new System.Windows.Forms.Button();
            this.chkKeepTypes = new System.Windows.Forms.CheckBox();
            this.chkIgnoreUnsupported = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(155, 155);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(267, 155);
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
            this.txtInfo.Location = new System.Drawing.Point(12, 185);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 361);
            this.txtInfo.TabIndex = 21;
            this.txtInfo.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Executable";
            // 
            // cboExe
            // 
            this.cboExe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExe.FormattingEnabled = true;
            this.cboExe.Location = new System.Drawing.Point(83, 13);
            this.cboExe.Name = "cboExe";
            this.cboExe.Size = new System.Drawing.Size(126, 21);
            this.cboExe.TabIndex = 1;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(380, 155);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 24);
            this.btnHelp.TabIndex = 20;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // txtAdditional
            // 
            this.txtAdditional.Location = new System.Drawing.Point(83, 89);
            this.txtAdditional.Multiline = true;
            this.txtAdditional.Name = "txtAdditional";
            this.txtAdditional.Size = new System.Drawing.Size(561, 60);
            this.txtAdditional.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Additional";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Action";
            // 
            // rbDeobf
            // 
            this.rbDeobf.AutoSize = true;
            this.rbDeobf.Checked = true;
            this.rbDeobf.Location = new System.Drawing.Point(83, 41);
            this.rbDeobf.Name = "rbDeobf";
            this.rbDeobf.Size = new System.Drawing.Size(86, 17);
            this.rbDeobf.TabIndex = 6;
            this.rbDeobf.TabStop = true;
            this.rbDeobf.Text = "Deobfuscate";
            this.rbDeobf.UseVisualStyleBackColor = true;
            // 
            // rbDetect
            // 
            this.rbDetect.AutoSize = true;
            this.rbDetect.Location = new System.Drawing.Point(175, 41);
            this.rbDetect.Name = "rbDetect";
            this.rbDetect.Size = new System.Drawing.Size(57, 17);
            this.rbDetect.TabIndex = 7;
            this.rbDetect.Text = "Detect";
            this.rbDetect.UseVisualStyleBackColor = true;
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Location = new System.Drawing.Point(222, 17);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 2;
            this.chkVerbose.Text = "Verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // chkCreateOutputDir
            // 
            this.chkCreateOutputDir.AutoSize = true;
            this.chkCreateOutputDir.Checked = true;
            this.chkCreateOutputDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateOutputDir.Location = new System.Drawing.Point(289, 17);
            this.chkCreateOutputDir.Name = "chkCreateOutputDir";
            this.chkCreateOutputDir.Size = new System.Drawing.Size(143, 17);
            this.chkCreateOutputDir.TabIndex = 3;
            this.chkCreateOutputDir.Text = "Create Output Directory";
            this.chkCreateOutputDir.UseVisualStyleBackColor = true;
            // 
            // chkScanDir
            // 
            this.chkScanDir.AutoSize = true;
            this.chkScanDir.Location = new System.Drawing.Point(435, 17);
            this.chkScanDir.Name = "chkScanDir";
            this.chkScanDir.Size = new System.Drawing.Size(96, 17);
            this.chkScanDir.TabIndex = 4;
            this.chkScanDir.Text = "Scan Directory";
            this.chkScanDir.UseVisualStyleBackColor = true;
            this.chkScanDir.CheckedChanged += new System.EventHandler(this.chkScanDir_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Options";
            // 
            // chkPreserveTokens
            // 
            this.chkPreserveTokens.AutoSize = true;
            this.chkPreserveTokens.Location = new System.Drawing.Point(83, 66);
            this.chkPreserveTokens.Name = "chkPreserveTokens";
            this.chkPreserveTokens.Size = new System.Drawing.Size(106, 17);
            this.chkPreserveTokens.TabIndex = 13;
            this.chkPreserveTokens.Text = "Preserve Tokens";
            this.chkPreserveTokens.UseVisualStyleBackColor = true;
            // 
            // chkDontRename
            // 
            this.chkDontRename.AutoSize = true;
            this.chkDontRename.Location = new System.Drawing.Point(195, 66);
            this.chkDontRename.Name = "chkDontRename";
            this.chkDontRename.Size = new System.Drawing.Size(90, 17);
            this.chkDontRename.TabIndex = 14;
            this.chkDontRename.Text = "Don\'t rename";
            this.chkDontRename.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(290, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "String Decrypter";
            // 
            // cboStringDecrypter
            // 
            this.cboStringDecrypter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStringDecrypter.FormattingEnabled = true;
            this.cboStringDecrypter.Location = new System.Drawing.Point(382, 40);
            this.cboStringDecrypter.Name = "cboStringDecrypter";
            this.cboStringDecrypter.Size = new System.Drawing.Size(99, 21);
            this.cboStringDecrypter.TabIndex = 9;
            // 
            // txtStringDecrypterMethod
            // 
            this.txtStringDecrypterMethod.Location = new System.Drawing.Point(487, 40);
            this.txtStringDecrypterMethod.Name = "txtStringDecrypterMethod";
            this.txtStringDecrypterMethod.Size = new System.Drawing.Size(124, 21);
            this.txtStringDecrypterMethod.TabIndex = 10;
            // 
            // btnSelectMethod
            // 
            this.btnSelectMethod.Location = new System.Drawing.Point(617, 39);
            this.btnSelectMethod.Name = "btnSelectMethod";
            this.btnSelectMethod.Size = new System.Drawing.Size(27, 21);
            this.btnSelectMethod.TabIndex = 11;
            this.btnSelectMethod.Text = "...";
            this.btnSelectMethod.UseVisualStyleBackColor = true;
            this.btnSelectMethod.Click += new System.EventHandler(this.btnSelectMethod_Click);
            // 
            // chkKeepTypes
            // 
            this.chkKeepTypes.AutoSize = true;
            this.chkKeepTypes.Location = new System.Drawing.Point(286, 66);
            this.chkKeepTypes.Name = "chkKeepTypes";
            this.chkKeepTypes.Size = new System.Drawing.Size(82, 17);
            this.chkKeepTypes.TabIndex = 15;
            this.chkKeepTypes.Text = "Keep Types";
            this.chkKeepTypes.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreUnsupported
            // 
            this.chkIgnoreUnsupported.AutoSize = true;
            this.chkIgnoreUnsupported.Location = new System.Drawing.Point(534, 17);
            this.chkIgnoreUnsupported.Name = "chkIgnoreUnsupported";
            this.chkIgnoreUnsupported.Size = new System.Drawing.Size(123, 17);
            this.chkIgnoreUnsupported.TabIndex = 5;
            this.chkIgnoreUnsupported.Text = "Ignore Unsupported";
            this.chkIgnoreUnsupported.UseVisualStyleBackColor = true;
            // 
            // frmDe4dot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(672, 556);
            this.Controls.Add(this.chkIgnoreUnsupported);
            this.Controls.Add(this.chkKeepTypes);
            this.Controls.Add(this.txtStringDecrypterMethod);
            this.Controls.Add(this.btnSelectMethod);
            this.Controls.Add(this.cboStringDecrypter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkDontRename);
            this.Controls.Add(this.chkPreserveTokens);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkScanDir);
            this.Controls.Add(this.chkCreateOutputDir);
            this.Controls.Add(this.chkVerbose);
            this.Controls.Add(this.rbDetect);
            this.Controls.Add(this.rbDeobf);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAdditional);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboExe);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmDe4dot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "de4dot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboExe;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TextBox txtAdditional;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbDeobf;
        private System.Windows.Forms.RadioButton rbDetect;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.CheckBox chkCreateOutputDir;
        private System.Windows.Forms.CheckBox chkScanDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkPreserveTokens;
        private System.Windows.Forms.CheckBox chkDontRename;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboStringDecrypter;
        private System.Windows.Forms.TextBox txtStringDecrypterMethod;
        private System.Windows.Forms.Button btnSelectMethod;
        private System.Windows.Forms.CheckBox chkKeepTypes;
        private System.Windows.Forms.CheckBox chkIgnoreUnsupported;
    }
}
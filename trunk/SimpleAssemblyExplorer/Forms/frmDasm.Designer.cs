namespace SimpleAssemblyExplorer
{
    partial class frmDasm
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
            this.btnSelectDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.chkBytes = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTokens = new System.Windows.Forms.CheckBox();
            this.chkTypeList = new System.Windows.Forms.CheckBox();
            this.chkClassList = new System.Windows.Forms.CheckBox();
            this.chkCAVerbal = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtAdditionalOptions = new System.Windows.Forms.TextBox();
            this.chkUnicode = new System.Windows.Forms.CheckBox();
            this.chkUTF8 = new System.Windows.Forms.CheckBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Output Directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(106, 9);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(447, 21);
            this.txtOutputDir.TabIndex = 1;
            this.txtOutputDir.Leave += new System.EventHandler(this.txtOutputDir_Leave);
            // 
            // btnSelectDir
            // 
            this.btnSelectDir.Location = new System.Drawing.Point(559, 9);
            this.btnSelectDir.Name = "btnSelectDir";
            this.btnSelectDir.Size = new System.Drawing.Size(28, 21);
            this.btnSelectDir.TabIndex = 2;
            this.btnSelectDir.Text = "...";
            this.btnSelectDir.UseVisualStyleBackColor = true;
            this.btnSelectDir.Click += new System.EventHandler(this.btnSelectOutputDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 86);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(271, 86);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 116);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(577, 266);
            this.txtInfo.TabIndex = 15;
            this.txtInfo.WordWrap = false;
            // 
            // chkBytes
            // 
            this.chkBytes.AutoSize = true;
            this.chkBytes.Checked = true;
            this.chkBytes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBytes.Location = new System.Drawing.Point(106, 36);
            this.chkBytes.Name = "chkBytes";
            this.chkBytes.Size = new System.Drawing.Size(53, 17);
            this.chkBytes.TabIndex = 4;
            this.chkBytes.Text = "Bytes";
            this.chkBytes.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Options";
            // 
            // chkTokens
            // 
            this.chkTokens.AutoSize = true;
            this.chkTokens.Checked = true;
            this.chkTokens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTokens.Location = new System.Drawing.Point(165, 37);
            this.chkTokens.Name = "chkTokens";
            this.chkTokens.Size = new System.Drawing.Size(60, 17);
            this.chkTokens.TabIndex = 5;
            this.chkTokens.Text = "Tokens";
            this.chkTokens.UseVisualStyleBackColor = true;
            // 
            // chkTypeList
            // 
            this.chkTypeList.AutoSize = true;
            this.chkTypeList.Checked = true;
            this.chkTypeList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTypeList.Location = new System.Drawing.Point(310, 37);
            this.chkTypeList.Name = "chkTypeList";
            this.chkTypeList.Size = new System.Drawing.Size(69, 17);
            this.chkTypeList.TabIndex = 7;
            this.chkTypeList.Text = "Type List";
            this.chkTypeList.UseVisualStyleBackColor = true;
            // 
            // chkClassList
            // 
            this.chkClassList.AutoSize = true;
            this.chkClassList.Location = new System.Drawing.Point(385, 37);
            this.chkClassList.Name = "chkClassList";
            this.chkClassList.Size = new System.Drawing.Size(70, 17);
            this.chkClassList.TabIndex = 8;
            this.chkClassList.Text = "Class List";
            this.chkClassList.UseVisualStyleBackColor = true;
            // 
            // chkCAVerbal
            // 
            this.chkCAVerbal.AutoSize = true;
            this.chkCAVerbal.Location = new System.Drawing.Point(231, 37);
            this.chkCAVerbal.Name = "chkCAVerbal";
            this.chkCAVerbal.Size = new System.Drawing.Size(73, 17);
            this.chkCAVerbal.TabIndex = 6;
            this.chkCAVerbal.Text = "CA Verbal";
            this.toolTip1.SetToolTip(this.chkCAVerbal, "Custom attribute type information may lost with /CAVERBAL option");
            this.chkCAVerbal.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // txtAdditionalOptions
            // 
            this.txtAdditionalOptions.Location = new System.Drawing.Point(106, 59);
            this.txtAdditionalOptions.Name = "txtAdditionalOptions";
            this.txtAdditionalOptions.Size = new System.Drawing.Size(447, 21);
            this.txtAdditionalOptions.TabIndex = 11;
            this.toolTip1.SetToolTip(this.txtAdditionalOptions, "Enter additional command line arguments here");
            // 
            // chkUnicode
            // 
            this.chkUnicode.AutoSize = true;
            this.chkUnicode.Location = new System.Drawing.Point(461, 37);
            this.chkUnicode.Name = "chkUnicode";
            this.chkUnicode.Size = new System.Drawing.Size(64, 17);
            this.chkUnicode.TabIndex = 9;
            this.chkUnicode.Text = "Unicode";
            this.chkUnicode.UseVisualStyleBackColor = true;
            this.chkUnicode.CheckedChanged += new System.EventHandler(this.chkUnicode_CheckedChanged);
            // 
            // chkUTF8
            // 
            this.chkUTF8.AutoSize = true;
            this.chkUTF8.Checked = true;
            this.chkUTF8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUTF8.Location = new System.Drawing.Point(531, 37);
            this.chkUTF8.Name = "chkUTF8";
            this.chkUTF8.Size = new System.Drawing.Size(51, 17);
            this.chkUTF8.TabIndex = 10;
            this.chkUTF8.Text = "UTF8";
            this.chkUTF8.UseVisualStyleBackColor = true;
            this.chkUTF8.CheckedChanged += new System.EventHandler(this.chkUTF8_CheckedChanged);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(382, 86);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 24);
            this.btnHelp.TabIndex = 14;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // frmDasm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(610, 392);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.txtAdditionalOptions);
            this.Controls.Add(this.chkUTF8);
            this.Controls.Add(this.chkUnicode);
            this.Controls.Add(this.chkCAVerbal);
            this.Controls.Add(this.chkClassList);
            this.Controls.Add(this.chkTypeList);
            this.Controls.Add(this.chkTokens);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkBytes);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectDir);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmDasm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Disassembler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnSelectDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.CheckBox chkBytes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkTokens;
        private System.Windows.Forms.CheckBox chkTypeList;
        private System.Windows.Forms.CheckBox chkClassList;
        private System.Windows.Forms.CheckBox chkCAVerbal;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkUnicode;
        private System.Windows.Forms.CheckBox chkUTF8;
        private System.Windows.Forms.TextBox txtAdditionalOptions;
        private System.Windows.Forms.Button btnHelp;
    }
}
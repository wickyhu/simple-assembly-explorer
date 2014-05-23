namespace SAE.FileDisassembler
{
    partial class frmFileDisassembler
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
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnSelectOutput = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.lbAssemblies = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboOptimization = new System.Windows.Forms.ComboBox();
            this.lblOptimization = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblOutputDir
            // 
            this.lblOutputDir.AutoSize = true;
            this.lblOutputDir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblOutputDir.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputDir.Location = new System.Drawing.Point(12, 74);
            this.lblOutputDir.Name = "lblOutputDir";
            this.lblOutputDir.Size = new System.Drawing.Size(88, 13);
            this.lblOutputDir.TabIndex = 2;
            this.lblOutputDir.Text = "Output Directory";
            this.lblOutputDir.Click += new System.EventHandler(this.lblOutputDir_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(118, 71);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(502, 21);
            this.txtOutputDir.TabIndex = 3;
            // 
            // btnSelectOutput
            // 
            this.btnSelectOutput.Location = new System.Drawing.Point(625, 71);
            this.btnSelectOutput.Name = "btnSelectOutput";
            this.btnSelectOutput.Size = new System.Drawing.Size(27, 21);
            this.btnSelectOutput.TabIndex = 4;
            this.btnSelectOutput.Text = "...";
            this.btnSelectOutput.UseVisualStyleBackColor = true;
            this.btnSelectOutput.Click += new System.EventHandler(this.btnSelectOutputDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(235, 131);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(353, 131);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 161);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 326);
            this.txtInfo.TabIndex = 12;
            this.txtInfo.WordWrap = false;
            // 
            // lbAssemblies
            // 
            this.lbAssemblies.FormattingEnabled = true;
            this.lbAssemblies.Location = new System.Drawing.Point(118, 9);
            this.lbAssemblies.Name = "lbAssemblies";
            this.lbAssemblies.Size = new System.Drawing.Size(534, 56);
            this.lbAssemblies.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Assemblies";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "C#",
            "Visual Basic",
            "MC++"});
            this.cboLanguage.Location = new System.Drawing.Point(118, 99);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(121, 21);
            this.cboLanguage.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Language";
            // 
            // cboOptimization
            // 
            this.cboOptimization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOptimization.FormattingEnabled = true;
            this.cboOptimization.Location = new System.Drawing.Point(353, 98);
            this.cboOptimization.Name = "cboOptimization";
            this.cboOptimization.Size = new System.Drawing.Size(62, 21);
            this.cboOptimization.TabIndex = 8;
            // 
            // lblOptimization
            // 
            this.lblOptimization.AutoSize = true;
            this.lblOptimization.Location = new System.Drawing.Point(281, 101);
            this.lblOptimization.Name = "lblOptimization";
            this.lblOptimization.Size = new System.Drawing.Size(66, 13);
            this.lblOptimization.TabIndex = 7;
            this.lblOptimization.Text = "Optimization";
            // 
            // frmFileDisassembler
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(671, 496);
            this.Controls.Add(this.cboOptimization);
            this.Controls.Add(this.lblOptimization);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbAssemblies);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectOutput);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.lblOutputDir);
            this.Controls.Add(this.txtInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmFileDisassembler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File Disassembler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFileDisassembler_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOutputDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnSelectOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.ListBox lbAssemblies;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboOptimization;
        private System.Windows.Forms.Label lblOptimization;
    }
}
namespace SAE.ILMerge
{
    partial class frmILMerge
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnSelectOutput = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.txtKeyFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.lbAssemblies = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkClosed = new System.Windows.Forms.CheckBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkDuplicate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Output Directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(118, 122);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(502, 21);
            this.txtOutputDir.TabIndex = 1;
            this.txtOutputDir.Leave += new System.EventHandler(this.txtOutputDir_Leave);
            // 
            // btnSelectOutput
            // 
            this.btnSelectOutput.Location = new System.Drawing.Point(625, 122);
            this.btnSelectOutput.Name = "btnSelectOutput";
            this.btnSelectOutput.Size = new System.Drawing.Size(27, 21);
            this.btnSelectOutput.TabIndex = 2;
            this.btnSelectOutput.Text = "...";
            this.btnSelectOutput.UseVisualStyleBackColor = true;
            this.btnSelectOutput.Click += new System.EventHandler(this.btnSelectOutputDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(229, 203);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(340, 203);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 238);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 249);
            this.txtInfo.TabIndex = 8;
            this.txtInfo.WordWrap = false;
            // 
            // txtKeyFile
            // 
            this.txtKeyFile.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtKeyFile.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtKeyFile.Location = new System.Drawing.Point(118, 148);
            this.txtKeyFile.Name = "txtKeyFile";
            this.txtKeyFile.Size = new System.Drawing.Size(502, 21);
            this.txtKeyFile.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Key File";
            // 
            // btnSelFile
            // 
            this.btnSelFile.Location = new System.Drawing.Point(625, 148);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(27, 21);
            this.btnSelFile.TabIndex = 4;
            this.btnSelFile.Text = "...";
            this.btnSelFile.UseVisualStyleBackColor = true;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // lbAssemblies
            // 
            this.lbAssemblies.FormattingEnabled = true;
            this.lbAssemblies.Location = new System.Drawing.Point(118, 9);
            this.lbAssemblies.Name = "lbAssemblies";
            this.lbAssemblies.Size = new System.Drawing.Size(534, 108);
            this.lbAssemblies.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Assemblies";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(28, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 86);
            this.label4.TabIndex = 10;
            this.label4.Text = "(Please choose primary assembly)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Options";
            // 
            // chkClosed
            // 
            this.chkClosed.AutoSize = true;
            this.chkClosed.Checked = true;
            this.chkClosed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkClosed.Location = new System.Drawing.Point(118, 175);
            this.chkClosed.Name = "chkClosed";
            this.chkClosed.Size = new System.Drawing.Size(58, 17);
            this.chkClosed.TabIndex = 5;
            this.chkClosed.Text = "Closed";
            this.chkClosed.UseVisualStyleBackColor = true;
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(195, 176);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(80, 17);
            this.chkDebug.TabIndex = 14;
            this.chkDebug.Text = "Debug Info";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // chkDuplicate
            // 
            this.chkDuplicate.AutoSize = true;
            this.chkDuplicate.Location = new System.Drawing.Point(292, 175);
            this.chkDuplicate.Name = "chkDuplicate";
            this.chkDuplicate.Size = new System.Drawing.Size(125, 17);
            this.chkDuplicate.TabIndex = 15;
            this.chkDuplicate.Text = "Allow Duplicate Type";
            this.chkDuplicate.UseVisualStyleBackColor = true;
            // 
            // frmILMerge
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(671, 496);
            this.Controls.Add(this.chkDuplicate);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.chkClosed);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbAssemblies);
            this.Controls.Add(this.btnSelFile);
            this.Controls.Add(this.txtKeyFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectOutput);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmILMerge";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ILMerge";
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
        private System.Windows.Forms.Button btnSelFile;
        private System.Windows.Forms.ListBox lbAssemblies;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkClosed;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.CheckBox chkDuplicate;
    }
}
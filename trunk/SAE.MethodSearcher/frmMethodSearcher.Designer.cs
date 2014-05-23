namespace SAE.MethodSearcher
{
    partial class frmMethodSearcher
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
            this.label3 = new System.Windows.Forms.Label();
            this.lblLog = new System.Windows.Forms.Label();
            this.cboSearchFor = new System.Windows.Forms.ComboBox();
            this.rbToScreen = new System.Windows.Forms.RadioButton();
            this.rbToFile = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(233, 71);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(344, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Location = new System.Drawing.Point(12, 101);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(647, 386);
            this.txtInfo.TabIndex = 9;
            this.txtInfo.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Search For";
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLog.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLog.Location = new System.Drawing.Point(385, 39);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(24, 13);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Log";
            this.lblLog.Click += new System.EventHandler(this.lblLog_Click);
            // 
            // cboSearchFor
            // 
            this.cboSearchFor.FormattingEnabled = true;
            this.cboSearchFor.Location = new System.Drawing.Point(118, 6);
            this.cboSearchFor.MaxDropDownItems = 20;
            this.cboSearchFor.Name = "cboSearchFor";
            this.cboSearchFor.Size = new System.Drawing.Size(502, 21);
            this.cboSearchFor.TabIndex = 1;
            // 
            // rbToScreen
            // 
            this.rbToScreen.AutoSize = true;
            this.rbToScreen.Location = new System.Drawing.Point(474, 37);
            this.rbToScreen.Name = "rbToScreen";
            this.rbToScreen.Size = new System.Drawing.Size(73, 17);
            this.rbToScreen.TabIndex = 5;
            this.rbToScreen.Text = "To Screen";
            this.rbToScreen.UseVisualStyleBackColor = true;
            // 
            // rbToFile
            // 
            this.rbToFile.AutoSize = true;
            this.rbToFile.Checked = true;
            this.rbToFile.Location = new System.Drawing.Point(564, 37);
            this.rbToFile.Name = "rbToFile";
            this.rbToFile.Size = new System.Drawing.Size(56, 17);
            this.rbToFile.TabIndex = 6;
            this.rbToFile.TabStop = true;
            this.rbToFile.Text = "To File";
            this.rbToFile.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Language";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(118, 33);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(121, 21);
            this.cboLanguage.TabIndex = 3;
            // 
            // frmMethodSearcher
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(671, 496);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.rbToFile);
            this.Controls.Add(this.rbToScreen);
            this.Controls.Add(this.cboSearchFor);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMethodSearcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Method Searcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.ComboBox cboSearchFor;
        private System.Windows.Forms.RadioButton rbToScreen;
        private System.Windows.Forms.RadioButton rbToFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboLanguage;
    }
}
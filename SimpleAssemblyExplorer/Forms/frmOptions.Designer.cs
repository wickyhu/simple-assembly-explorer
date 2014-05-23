namespace SimpleAssemblyExplorer
{
    partial class frmOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboCheckUpdatePeriod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCheckUpdateEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nudRecentPluginList = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboBamlTranslator = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblRtbFontReset = new System.Windows.Forms.Label();
            this.lblRtbFont = new System.Windows.Forms.Label();
            this.lblRtbFontSelect = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.chkAutoOpenDroppedAssembly = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAutoSaveBookmarkEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkIntegrateWithExplorer = new System.Windows.Forms.CheckBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentPluginList)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(120, 363);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(226, 363);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboCheckUpdatePeriod);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkCheckUpdateEnabled);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Auto Update";
            // 
            // cboCheckUpdatePeriod
            // 
            this.cboCheckUpdatePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCheckUpdatePeriod.FormattingEnabled = true;
            this.cboCheckUpdatePeriod.Items.AddRange(new object[] {
            "1 Week",
            "2 Weeks",
            "3 Weeks",
            "4 Weeks",
            "5 Weeks",
            "6 Weeks",
            "7 Weeks",
            "8 Weeks"});
            this.cboCheckUpdatePeriod.Location = new System.Drawing.Point(214, 18);
            this.cboCheckUpdatePeriod.Name = "cboCheckUpdatePeriod";
            this.cboCheckUpdatePeriod.Size = new System.Drawing.Size(95, 21);
            this.cboCheckUpdatePeriod.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Auto Check for Update";
            // 
            // chkCheckUpdateEnabled
            // 
            this.chkCheckUpdateEnabled.AutoSize = true;
            this.chkCheckUpdateEnabled.Checked = true;
            this.chkCheckUpdateEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCheckUpdateEnabled.Location = new System.Drawing.Point(144, 20);
            this.chkCheckUpdateEnabled.Name = "chkCheckUpdateEnabled";
            this.chkCheckUpdateEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkCheckUpdateEnabled.TabIndex = 1;
            this.chkCheckUpdateEnabled.Text = "Enabled";
            this.chkCheckUpdateEnabled.UseVisualStyleBackColor = true;
            this.chkCheckUpdateEnabled.CheckedChanged += new System.EventHandler(this.chkCheckUpdateEnabled_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nudRecentPluginList);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(12, 62);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(392, 49);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Plugin";
            // 
            // nudRecentPluginList
            // 
            this.nudRecentPluginList.Location = new System.Drawing.Point(144, 17);
            this.nudRecentPluginList.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRecentPluginList.Name = "nudRecentPluginList";
            this.nudRecentPluginList.Size = new System.Drawing.Size(49, 21);
            this.nudRecentPluginList.TabIndex = 1;
            this.nudRecentPluginList.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Recent Plugin List";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboBamlTranslator);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblRtbFontReset);
            this.groupBox2.Controls.Add(this.lblRtbFont);
            this.groupBox2.Controls.Add(this.lblRtbFontSelect);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.chkAutoOpenDroppedAssembly);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkAutoSaveBookmarkEnabled);
            this.groupBox2.Location = new System.Drawing.Point(12, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 181);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Class Editor";
            // 
            // cboBamlTranslator
            // 
            this.cboBamlTranslator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBamlTranslator.FormattingEnabled = true;
            this.cboBamlTranslator.Items.AddRange(new object[] {
            "Reflector.BamlViewer",
            "ILSpy.BamlDecompiler"});
            this.cboBamlTranslator.Location = new System.Drawing.Point(174, 64);
            this.cboBamlTranslator.Name = "cboBamlTranslator";
            this.cboBamlTranslator.Size = new System.Drawing.Size(135, 21);
            this.cboBamlTranslator.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Baml Translator";
            // 
            // lblRtbFontReset
            // 
            this.lblRtbFontReset.AutoSize = true;
            this.lblRtbFontReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRtbFontReset.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRtbFontReset.Location = new System.Drawing.Point(113, 91);
            this.lblRtbFontReset.Name = "lblRtbFontReset";
            this.lblRtbFontReset.Size = new System.Drawing.Size(35, 13);
            this.lblRtbFontReset.TabIndex = 7;
            this.lblRtbFontReset.Text = "Reset";
            this.lblRtbFontReset.Click += new System.EventHandler(this.lblRtbFontReset_Click);
            // 
            // lblRtbFont
            // 
            this.lblRtbFont.AutoSize = true;
            this.lblRtbFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRtbFont.Location = new System.Drawing.Point(173, 91);
            this.lblRtbFont.Name = "lblRtbFont";
            this.lblRtbFont.Size = new System.Drawing.Size(102, 15);
            this.lblRtbFont.TabIndex = 8;
            this.lblRtbFont.Text = "public class Test { }";
            // 
            // lblRtbFontSelect
            // 
            this.lblRtbFontSelect.AutoSize = true;
            this.lblRtbFontSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRtbFontSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRtbFontSelect.Location = new System.Drawing.Point(9, 91);
            this.lblRtbFontSelect.Name = "lblRtbFontSelect";
            this.lblRtbFontSelect.Size = new System.Drawing.Size(98, 13);
            this.lblRtbFontSelect.TabIndex = 6;
            this.lblRtbFontSelect.Text = "Rich Text Box Font";
            this.lblRtbFontSelect.Click += new System.EventHandler(this.lblRtbFontSelect_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(151, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Auto Open Dropped Assembly";
            // 
            // chkAutoOpenDroppedAssembly
            // 
            this.chkAutoOpenDroppedAssembly.AutoSize = true;
            this.chkAutoOpenDroppedAssembly.Checked = true;
            this.chkAutoOpenDroppedAssembly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoOpenDroppedAssembly.Location = new System.Drawing.Point(174, 43);
            this.chkAutoOpenDroppedAssembly.Name = "chkAutoOpenDroppedAssembly";
            this.chkAutoOpenDroppedAssembly.Size = new System.Drawing.Size(64, 17);
            this.chkAutoOpenDroppedAssembly.TabIndex = 3;
            this.chkAutoOpenDroppedAssembly.Text = "Enabled";
            this.chkAutoOpenDroppedAssembly.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Auto Save Bookmark";
            // 
            // chkAutoSaveBookmarkEnabled
            // 
            this.chkAutoSaveBookmarkEnabled.AutoSize = true;
            this.chkAutoSaveBookmarkEnabled.Checked = true;
            this.chkAutoSaveBookmarkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSaveBookmarkEnabled.Location = new System.Drawing.Point(174, 20);
            this.chkAutoSaveBookmarkEnabled.Name = "chkAutoSaveBookmarkEnabled";
            this.chkAutoSaveBookmarkEnabled.Size = new System.Drawing.Size(64, 17);
            this.chkAutoSaveBookmarkEnabled.TabIndex = 1;
            this.chkAutoSaveBookmarkEnabled.Text = "Enabled";
            this.chkAutoSaveBookmarkEnabled.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.chkIntegrateWithExplorer);
            this.groupBox4.Location = new System.Drawing.Point(12, 117);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(392, 53);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Shell Integration";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "(Administrator permission required)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Integrate with Explorer";
            // 
            // chkIntegrateWithExplorer
            // 
            this.chkIntegrateWithExplorer.AutoSize = true;
            this.chkIntegrateWithExplorer.Location = new System.Drawing.Point(144, 20);
            this.chkIntegrateWithExplorer.Name = "chkIntegrateWithExplorer";
            this.chkIntegrateWithExplorer.Size = new System.Drawing.Size(65, 17);
            this.chkIntegrateWithExplorer.TabIndex = 1;
            this.chkIntegrateWithExplorer.Text = "Enabled";
            this.chkIntegrateWithExplorer.UseVisualStyleBackColor = true;
            // 
            // frmOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(419, 404);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentPluginList)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkCheckUpdateEnabled;
        private System.Windows.Forms.ComboBox cboCheckUpdatePeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudRecentPluginList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAutoSaveBookmarkEnabled;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkIntegrateWithExplorer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkAutoOpenDroppedAssembly;
		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.Label lblRtbFont;
		private System.Windows.Forms.Label lblRtbFontSelect;
        private System.Windows.Forms.Label lblRtbFontReset;
        private System.Windows.Forms.ComboBox cboBamlTranslator;
        private System.Windows.Forms.Label label7;
    }
}
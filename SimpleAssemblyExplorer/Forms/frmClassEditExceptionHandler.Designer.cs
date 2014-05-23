namespace SimpleAssemblyExplorer
{
    partial class frmClassEditExceptionHandler
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
            this.label3 = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.cboTryStart = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTryEnd = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboHandlerStart = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboHandlerEnd = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboFilterStart = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCatchType = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(126, 206);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(237, 206);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Try Start";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(97, 8);
            this.cboType.MaxDropDownItems = 20;
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(130, 21);
            this.cboType.TabIndex = 0;
            // 
            // cboTryStart
            // 
            this.cboTryStart.DropDownWidth = 450;
            this.cboTryStart.FormattingEnabled = true;
            this.cboTryStart.Location = new System.Drawing.Point(97, 62);
            this.cboTryStart.MaxDropDownItems = 20;
            this.cboTryStart.Name = "cboTryStart";
            this.cboTryStart.Size = new System.Drawing.Size(335, 21);
            this.cboTryStart.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Type";
            // 
            // cboTryEnd
            // 
            this.cboTryEnd.DropDownWidth = 450;
            this.cboTryEnd.FormattingEnabled = true;
            this.cboTryEnd.Location = new System.Drawing.Point(97, 89);
            this.cboTryEnd.MaxDropDownItems = 20;
            this.cboTryEnd.Name = "cboTryEnd";
            this.cboTryEnd.Size = new System.Drawing.Size(335, 21);
            this.cboTryEnd.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Try End";
            // 
            // cboHandlerStart
            // 
            this.cboHandlerStart.DropDownWidth = 450;
            this.cboHandlerStart.FormattingEnabled = true;
            this.cboHandlerStart.Location = new System.Drawing.Point(97, 116);
            this.cboHandlerStart.MaxDropDownItems = 20;
            this.cboHandlerStart.Name = "cboHandlerStart";
            this.cboHandlerStart.Size = new System.Drawing.Size(335, 21);
            this.cboHandlerStart.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Handler Start";
            // 
            // cboHandlerEnd
            // 
            this.cboHandlerEnd.DropDownWidth = 450;
            this.cboHandlerEnd.FormattingEnabled = true;
            this.cboHandlerEnd.Location = new System.Drawing.Point(97, 143);
            this.cboHandlerEnd.MaxDropDownItems = 20;
            this.cboHandlerEnd.Name = "cboHandlerEnd";
            this.cboHandlerEnd.Size = new System.Drawing.Size(335, 21);
            this.cboHandlerEnd.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Handler End";
            // 
            // cboFilterStart
            // 
            this.cboFilterStart.DropDownWidth = 450;
            this.cboFilterStart.FormattingEnabled = true;
            this.cboFilterStart.Location = new System.Drawing.Point(97, 170);
            this.cboFilterStart.MaxDropDownItems = 20;
            this.cboFilterStart.Name = "cboFilterStart";
            this.cboFilterStart.Size = new System.Drawing.Size(335, 21);
            this.cboFilterStart.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Filterr Start";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Catch Type";
            // 
            // txtCatchType
            // 
            this.txtCatchType.Enabled = false;
            this.txtCatchType.Location = new System.Drawing.Point(97, 35);
            this.txtCatchType.Name = "txtCatchType";
            this.txtCatchType.Size = new System.Drawing.Size(335, 21);
            this.txtCatchType.TabIndex = 1;
            // 
            // frmClassEditExceptionHandler
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(456, 246);
            this.Controls.Add(this.txtCatchType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cboFilterStart);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboHandlerEnd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboHandlerStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboTryEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTryStart);
            this.Controls.Add(this.cboType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmClassEditExceptionHandler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception Handler Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.ComboBox cboTryStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTryEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboHandlerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboHandlerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboFilterStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCatchType;
    }
}
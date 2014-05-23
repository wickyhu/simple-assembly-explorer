namespace SimpleAssemblyExplorer
{
    partial class frmClassEditMoveInstructions
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
            this.cboStart = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboEnd = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboTo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rbBefore = new System.Windows.Forms.RadioButton();
            this.rbAfter = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(121, 141);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(232, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Start";
            // 
            // cboStart
            // 
            this.cboStart.DropDownWidth = 450;
            this.cboStart.FormattingEnabled = true;
            this.cboStart.Location = new System.Drawing.Point(97, 19);
            this.cboStart.MaxDropDownItems = 20;
            this.cboStart.Name = "cboStart";
            this.cboStart.Size = new System.Drawing.Size(335, 21);
            this.cboStart.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Type";
            // 
            // cboEnd
            // 
            this.cboEnd.DropDownWidth = 450;
            this.cboEnd.FormattingEnabled = true;
            this.cboEnd.Location = new System.Drawing.Point(97, 46);
            this.cboEnd.MaxDropDownItems = 20;
            this.cboEnd.Name = "cboEnd";
            this.cboEnd.Size = new System.Drawing.Size(335, 21);
            this.cboEnd.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "End";
            // 
            // cboTo
            // 
            this.cboTo.DropDownWidth = 450;
            this.cboTo.FormattingEnabled = true;
            this.cboTo.Location = new System.Drawing.Point(97, 105);
            this.cboTo.MaxDropDownItems = 20;
            this.cboTo.Name = "cboTo";
            this.cboTo.Size = new System.Drawing.Size(335, 21);
            this.cboTo.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "To";
            // 
            // rbBefore
            // 
            this.rbBefore.AutoSize = true;
            this.rbBefore.Location = new System.Drawing.Point(170, 77);
            this.rbBefore.Name = "rbBefore";
            this.rbBefore.Size = new System.Drawing.Size(57, 17);
            this.rbBefore.TabIndex = 2;
            this.rbBefore.Text = "Before";
            this.rbBefore.UseVisualStyleBackColor = true;
            // 
            // rbAfter
            // 
            this.rbAfter.AutoSize = true;
            this.rbAfter.Checked = true;
            this.rbAfter.Location = new System.Drawing.Point(97, 77);
            this.rbAfter.Name = "rbAfter";
            this.rbAfter.Size = new System.Drawing.Size(50, 17);
            this.rbAfter.TabIndex = 3;
            this.rbAfter.TabStop = true;
            this.rbAfter.Text = "After";
            this.rbAfter.UseVisualStyleBackColor = true;
            // 
            // frmClassEditMoveInstructions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(456, 186);
            this.Controls.Add(this.rbAfter);
            this.Controls.Add(this.rbBefore);
            this.Controls.Add(this.cboTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmClassEditMoveInstructions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move Instructions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbBefore;
        private System.Windows.Forms.RadioButton rbAfter;
    }
}
namespace SimpleAssemblyExplorer
{
    partial class frmClassEditInstruction
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
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboOpCode = new System.Windows.Forms.ComboBox();
            this.cboOperand = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPrevIns = new System.Windows.Forms.TextBox();
            this.txtNextIns = new System.Windows.Forms.TextBox();
            this.btnSelectMethod = new System.Windows.Forms.Button();
            this.lblOpCodeInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(142, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(253, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "OpCode";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Operand";
            // 
            // cboOpCode
            // 
            this.cboOpCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboOpCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOpCode.FormattingEnabled = true;
            this.cboOpCode.Location = new System.Drawing.Point(83, 36);
            this.cboOpCode.MaxDropDownItems = 20;
            this.cboOpCode.Name = "cboOpCode";
            this.cboOpCode.Size = new System.Drawing.Size(166, 21);
            this.cboOpCode.TabIndex = 1;
            this.cboOpCode.SelectedIndexChanged += new System.EventHandler(this.cboOpCode_SelectedIndexChanged);
            this.cboOpCode.TextChanged += new System.EventHandler(this.cboOpCode_TextChanged);
            // 
            // cboOperand
            // 
            this.cboOperand.DropDownWidth = 450;
            this.cboOperand.FormattingEnabled = true;
            this.cboOperand.Location = new System.Drawing.Point(83, 105);
            this.cboOperand.MaxDropDownItems = 20;
            this.cboOperand.Name = "cboOperand";
            this.cboOperand.Size = new System.Drawing.Size(335, 21);
            this.cboOperand.TabIndex = 2;
            this.cboOperand.SelectedIndexChanged += new System.EventHandler(this.cboOperand_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Previous";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Next";
            // 
            // txtPrevIns
            // 
            this.txtPrevIns.Enabled = false;
            this.txtPrevIns.Location = new System.Drawing.Point(83, 9);
            this.txtPrevIns.Name = "txtPrevIns";
            this.txtPrevIns.Size = new System.Drawing.Size(335, 21);
            this.txtPrevIns.TabIndex = 0;
            // 
            // txtNextIns
            // 
            this.txtNextIns.Enabled = false;
            this.txtNextIns.Location = new System.Drawing.Point(83, 132);
            this.txtNextIns.Name = "txtNextIns";
            this.txtNextIns.Size = new System.Drawing.Size(335, 21);
            this.txtNextIns.TabIndex = 3;
            // 
            // btnSelectMethod
            // 
            this.btnSelectMethod.Location = new System.Drawing.Point(424, 105);
            this.btnSelectMethod.Name = "btnSelectMethod";
            this.btnSelectMethod.Size = new System.Drawing.Size(27, 21);
            this.btnSelectMethod.TabIndex = 10;
            this.btnSelectMethod.Text = "...";
            this.btnSelectMethod.UseVisualStyleBackColor = true;
            this.btnSelectMethod.Click += new System.EventHandler(this.btnSelectMethod_Click);
            // 
            // lblOpCodeInfo
            // 
            this.lblOpCodeInfo.AutoSize = true;
            this.lblOpCodeInfo.Location = new System.Drawing.Point(81, 59);
            this.lblOpCodeInfo.Name = "lblOpCodeInfo";
            this.lblOpCodeInfo.Size = new System.Drawing.Size(66, 13);
            this.lblOpCodeInfo.TabIndex = 11;
            this.lblOpCodeInfo.Text = "OpCodeInfo";
            // 
            // frmClassEditInstruction
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(469, 201);
            this.Controls.Add(this.btnSelectMethod);
            this.Controls.Add(this.txtNextIns);
            this.Controls.Add(this.txtPrevIns);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboOperand);
            this.Controls.Add(this.cboOpCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblOpCodeInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmClassEditInstruction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Instruction Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboOpCode;
        private System.Windows.Forms.ComboBox cboOperand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPrevIns;
        private System.Windows.Forms.TextBox txtNextIns;
        private System.Windows.Forms.Button btnSelectMethod;
        private System.Windows.Forms.Label lblOpCodeInfo;
    }
}
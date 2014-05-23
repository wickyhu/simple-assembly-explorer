namespace SimpleAssemblyExplorer
{
    partial class frmClassEditRename
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
            this.txtOriginalName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCurrentName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNewName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtOriginalNameHex = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCurrentNameHex = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtOriginalName
            // 
            this.txtOriginalName.Location = new System.Drawing.Point(121, 12);
            this.txtOriginalName.Name = "txtOriginalName";
            this.txtOriginalName.ReadOnly = true;
            this.txtOriginalName.Size = new System.Drawing.Size(256, 21);
            this.txtOriginalName.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Original Name";
            // 
            // txtCurrentName
            // 
            this.txtCurrentName.Location = new System.Drawing.Point(121, 66);
            this.txtCurrentName.Name = "txtCurrentName";
            this.txtCurrentName.ReadOnly = true;
            this.txtCurrentName.Size = new System.Drawing.Size(256, 21);
            this.txtCurrentName.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Current Name";
            // 
            // txtNewName
            // 
            this.txtNewName.Location = new System.Drawing.Point(121, 120);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Size = new System.Drawing.Size(256, 21);
            this.txtNewName.TabIndex = 0;
            this.txtNewName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtNewName_KeyUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "New Name";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(210, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(99, 155);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtOriginalNameHex
            // 
            this.txtOriginalNameHex.Location = new System.Drawing.Point(121, 39);
            this.txtOriginalNameHex.Name = "txtOriginalNameHex";
            this.txtOriginalNameHex.ReadOnly = true;
            this.txtOriginalNameHex.Size = new System.Drawing.Size(256, 21);
            this.txtOriginalNameHex.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Original Name (Hex)";
            // 
            // txtCurrentNameHex
            // 
            this.txtCurrentNameHex.Location = new System.Drawing.Point(121, 93);
            this.txtCurrentNameHex.Name = "txtCurrentNameHex";
            this.txtCurrentNameHex.ReadOnly = true;
            this.txtCurrentNameHex.Size = new System.Drawing.Size(256, 21);
            this.txtCurrentNameHex.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Current Name (Hex)";
            // 
            // frmClassEditRename
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 198);
            this.Controls.Add(this.txtCurrentNameHex);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOriginalNameHex);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCurrentName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOriginalName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmClassEditRename";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Class Editor - Rename";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOriginalName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCurrentName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNewName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtOriginalNameHex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCurrentNameHex;
        private System.Windows.Forms.Label label5;
    }
}
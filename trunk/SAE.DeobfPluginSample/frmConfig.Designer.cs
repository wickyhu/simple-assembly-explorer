
namespace SAE.DeobfPluginSample
{
    partial class frmConfig
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
            this.chkAssemblyEvent = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkModuleEvent = new System.Windows.Forms.CheckBox();
            this.chkTypeEvent = new System.Windows.Forms.CheckBox();
            this.chkFieldEvent = new System.Windows.Forms.CheckBox();
            this.chkPropertyEvent = new System.Windows.Forms.CheckBox();
            this.chkMethodEvent = new System.Windows.Forms.CheckBox();
            this.chkEventEvent = new System.Windows.Forms.CheckBox();
            this.chkRenameList = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkNameList = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(86, 139);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(197, 139);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkAssemblyEvent
            // 
            this.chkAssemblyEvent.AutoSize = true;
            this.chkAssemblyEvent.Checked = true;
            this.chkAssemblyEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAssemblyEvent.Location = new System.Drawing.Point(72, 22);
            this.chkAssemblyEvent.Name = "chkAssemblyEvent";
            this.chkAssemblyEvent.Size = new System.Drawing.Size(71, 17);
            this.chkAssemblyEvent.TabIndex = 1;
            this.chkAssemblyEvent.Text = "Assembly";
            this.chkAssemblyEvent.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Events";
            // 
            // chkModuleEvent
            // 
            this.chkModuleEvent.AutoSize = true;
            this.chkModuleEvent.Checked = true;
            this.chkModuleEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkModuleEvent.Location = new System.Drawing.Point(155, 22);
            this.chkModuleEvent.Name = "chkModuleEvent";
            this.chkModuleEvent.Size = new System.Drawing.Size(60, 17);
            this.chkModuleEvent.TabIndex = 2;
            this.chkModuleEvent.Text = "Module";
            this.chkModuleEvent.UseVisualStyleBackColor = true;
            // 
            // chkTypeEvent
            // 
            this.chkTypeEvent.AutoSize = true;
            this.chkTypeEvent.Checked = true;
            this.chkTypeEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTypeEvent.Location = new System.Drawing.Point(234, 22);
            this.chkTypeEvent.Name = "chkTypeEvent";
            this.chkTypeEvent.Size = new System.Drawing.Size(50, 17);
            this.chkTypeEvent.TabIndex = 3;
            this.chkTypeEvent.Text = "Type";
            this.chkTypeEvent.UseVisualStyleBackColor = true;
            // 
            // chkFieldEvent
            // 
            this.chkFieldEvent.AutoSize = true;
            this.chkFieldEvent.Checked = true;
            this.chkFieldEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFieldEvent.Location = new System.Drawing.Point(234, 45);
            this.chkFieldEvent.Name = "chkFieldEvent";
            this.chkFieldEvent.Size = new System.Drawing.Size(48, 17);
            this.chkFieldEvent.TabIndex = 6;
            this.chkFieldEvent.Text = "Field";
            this.chkFieldEvent.UseVisualStyleBackColor = true;
            // 
            // chkPropertyEvent
            // 
            this.chkPropertyEvent.AutoSize = true;
            this.chkPropertyEvent.Checked = true;
            this.chkPropertyEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPropertyEvent.Location = new System.Drawing.Point(155, 45);
            this.chkPropertyEvent.Name = "chkPropertyEvent";
            this.chkPropertyEvent.Size = new System.Drawing.Size(68, 17);
            this.chkPropertyEvent.TabIndex = 5;
            this.chkPropertyEvent.Text = "Property";
            this.chkPropertyEvent.UseVisualStyleBackColor = true;
            // 
            // chkMethodEvent
            // 
            this.chkMethodEvent.AutoSize = true;
            this.chkMethodEvent.Checked = true;
            this.chkMethodEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMethodEvent.Location = new System.Drawing.Point(72, 45);
            this.chkMethodEvent.Name = "chkMethodEvent";
            this.chkMethodEvent.Size = new System.Drawing.Size(62, 17);
            this.chkMethodEvent.TabIndex = 4;
            this.chkMethodEvent.Text = "Method";
            this.chkMethodEvent.UseVisualStyleBackColor = true;
            // 
            // chkEventEvent
            // 
            this.chkEventEvent.AutoSize = true;
            this.chkEventEvent.Checked = true;
            this.chkEventEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEventEvent.Location = new System.Drawing.Point(294, 45);
            this.chkEventEvent.Name = "chkEventEvent";
            this.chkEventEvent.Size = new System.Drawing.Size(54, 17);
            this.chkEventEvent.TabIndex = 7;
            this.chkEventEvent.Text = "Event";
            this.chkEventEvent.UseVisualStyleBackColor = true;
            // 
            // chkRenameList
            // 
            this.chkRenameList.AutoSize = true;
            this.chkRenameList.Location = new System.Drawing.Point(72, 100);
            this.chkRenameList.Name = "chkRenameList";
            this.chkRenameList.Size = new System.Drawing.Size(84, 17);
            this.chkRenameList.TabIndex = 12;
            this.chkRenameList.Text = "Rename List";
            this.chkRenameList.UseVisualStyleBackColor = true;
            this.chkRenameList.CheckedChanged += new System.EventHandler(this.chkRenameList_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Special";
            // 
            // chkNameList
            // 
            this.chkNameList.AutoSize = true;
            this.chkNameList.Location = new System.Drawing.Point(72, 77);
            this.chkNameList.Name = "chkNameList";
            this.chkNameList.Size = new System.Drawing.Size(72, 17);
            this.chkNameList.TabIndex = 11;
            this.chkNameList.Text = "Name List";
            this.chkNameList.UseVisualStyleBackColor = true;
            this.chkNameList.CheckedChanged += new System.EventHandler(this.chkNameList_CheckedChanged);
            // 
            // frmConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(366, 175);
            this.Controls.Add(this.chkRenameList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkNameList);
            this.Controls.Add(this.chkEventEvent);
            this.Controls.Add(this.chkFieldEvent);
            this.Controls.Add(this.chkPropertyEvent);
            this.Controls.Add(this.chkMethodEvent);
            this.Controls.Add(this.chkTypeEvent);
            this.Controls.Add(this.chkModuleEvent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAssemblyEvent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkAssemblyEvent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkModuleEvent;
        private System.Windows.Forms.CheckBox chkTypeEvent;
        private System.Windows.Forms.CheckBox chkFieldEvent;
        private System.Windows.Forms.CheckBox chkPropertyEvent;
        private System.Windows.Forms.CheckBox chkMethodEvent;
        private System.Windows.Forms.CheckBox chkEventEvent;
        private System.Windows.Forms.CheckBox chkRenameList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkNameList;
    }
}
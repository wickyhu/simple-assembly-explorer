namespace SimpleAssemblyExplorer
{
    partial class frmPluginList
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
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.dgcTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcPluginType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcContact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcTitle,
            this.dgcPluginType,
            this.dgcVersion,
            this.dgcAuthor,
            this.dgcContact,
            this.dgcUrl});
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 0);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersWidth = 21;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(751, 309);
            this.dgvData.TabIndex = 1;
            this.dgvData.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellDoubleClick);
            // 
            // dgcTitle
            // 
            this.dgcTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcTitle.DataPropertyName = "Title";
            this.dgcTitle.HeaderText = "Title";
            this.dgcTitle.MinimumWidth = 150;
            this.dgcTitle.Name = "dgcTitle";
            this.dgcTitle.ReadOnly = true;
            this.dgcTitle.Width = 150;
            // 
            // dgcPluginType
            // 
            this.dgcPluginType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcPluginType.DataPropertyName = "PluginType";
            this.dgcPluginType.HeaderText = "Plugin Type";
            this.dgcPluginType.MinimumWidth = 90;
            this.dgcPluginType.Name = "dgcPluginType";
            this.dgcPluginType.ReadOnly = true;
            this.dgcPluginType.Width = 90;
            // 
            // dgcVersion
            // 
            this.dgcVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcVersion.DataPropertyName = "Version";
            this.dgcVersion.HeaderText = "Version";
            this.dgcVersion.MinimumWidth = 80;
            this.dgcVersion.Name = "dgcVersion";
            this.dgcVersion.ReadOnly = true;
            this.dgcVersion.Width = 80;
            // 
            // dgcAuthor
            // 
            this.dgcAuthor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcAuthor.DataPropertyName = "Author";
            this.dgcAuthor.HeaderText = "Author";
            this.dgcAuthor.MinimumWidth = 90;
            this.dgcAuthor.Name = "dgcAuthor";
            this.dgcAuthor.ReadOnly = true;
            this.dgcAuthor.Width = 90;
            // 
            // dgcContact
            // 
            this.dgcContact.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcContact.DataPropertyName = "Contact";
            this.dgcContact.HeaderText = "Contact";
            this.dgcContact.MinimumWidth = 150;
            this.dgcContact.Name = "dgcContact";
            this.dgcContact.ReadOnly = true;
            this.dgcContact.Width = 150;
            // 
            // dgcUrl
            // 
            this.dgcUrl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcUrl.DataPropertyName = "Url";
            this.dgcUrl.HeaderText = "Url";
            this.dgcUrl.MinimumWidth = 100;
            this.dgcUrl.Name = "dgcUrl";
            this.dgcUrl.ReadOnly = true;
            // 
            // frmPluginList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(751, 309);
            this.Controls.Add(this.dgvData);
            this.Name = "frmPluginList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Plugin List";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcPluginType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcContact;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcUrl;
    }
}
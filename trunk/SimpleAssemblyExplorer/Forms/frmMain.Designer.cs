namespace SimpleAssemblyExplorer
{
    public partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPluginList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuProfileApp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProfileASPNet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuGenStrongKey = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHome = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEmailAuthor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCheckForUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSDK20 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDK35 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDK4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDK45 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.statusVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.dgcFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmAssembly = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmAsm = new System.Windows.Forms.ToolStripMenuItem();
            this.cmDasm = new System.Windows.Forms.ToolStripMenuItem();
            this.cmDeobf = new System.Windows.Forms.ToolStripMenuItem();
            this.cmStrongName = new System.Windows.Forms.ToolStripMenuItem();
            this.cmVerify = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmClassEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRunMethod = new System.Windows.Forms.ToolStripMenuItem();
            this.cmProfiler = new System.Windows.Forms.ToolStripMenuItem();
            this.cmPluginSepStart = new System.Windows.Forms.ToolStripSeparator();
            this.cmMore = new System.Windows.Forms.ToolStripMenuItem();
            this.cmPluginSepEnd = new System.Windows.Forms.ToolStripSeparator();
            this.cmCopyInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.cmOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.cmDeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.dgcAssemblyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcProcessorArchitecture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcTargetRuntime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.cmFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmFolderAsm = new System.Windows.Forms.ToolStripMenuItem();
            this.cmFolderDasm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.cmFolderOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.cmFolderDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.cmRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblPath = new System.Windows.Forms.ToolStripLabel();
            this.txtPath = new System.Windows.Forms.ToolStripTextBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.cmAssembly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.cmFolder.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuTools,
            this.mnuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOptions,
            this.toolStripMenuItem7,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(35, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuOptions
            // 
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.Size = new System.Drawing.Size(111, 22);
            this.mnuOptions.Text = "Options";
            this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(108, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(111, 22);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPluginList,
            this.toolStripMenuItem11,
            this.mnuProfileApp,
            this.mnuProfileASPNet,
            this.toolStripMenuItem10,
            this.mnuGenStrongKey});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(44, 20);
            this.mnuTools.Text = "&Tools";
            // 
            // mnuPluginList
            // 
            this.mnuPluginList.Name = "mnuPluginList";
            this.mnuPluginList.Size = new System.Drawing.Size(175, 22);
            this.mnuPluginList.Text = "Plugin List";
            this.mnuPluginList.Click += new System.EventHandler(this.mnuPluginList_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(172, 6);
            // 
            // mnuProfileApp
            // 
            this.mnuProfileApp.Name = "mnuProfileApp";
            this.mnuProfileApp.Size = new System.Drawing.Size(175, 22);
            this.mnuProfileApp.Text = "Profile Application";
            this.mnuProfileApp.Click += new System.EventHandler(this.mnuProfileApp_Click);
            // 
            // mnuProfileASPNet
            // 
            this.mnuProfileASPNet.Name = "mnuProfileASPNet";
            this.mnuProfileASPNet.Size = new System.Drawing.Size(175, 22);
            this.mnuProfileASPNet.Text = "Profile ASP.Net";
            this.mnuProfileASPNet.Click += new System.EventHandler(this.mnuProfileASPNet_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(172, 6);
            // 
            // mnuGenStrongKey
            // 
            this.mnuGenStrongKey.Name = "mnuGenStrongKey";
            this.mnuGenStrongKey.Size = new System.Drawing.Size(175, 22);
            this.mnuGenStrongKey.Text = "Generate Strong Key";
            this.mnuGenStrongKey.Click += new System.EventHandler(this.mnuGenStrongKey_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHome,
            this.mnuEmailAuthor,
            this.mnuCheckForUpdate,
            this.toolStripMenuItem6,
            this.mnuSDK20,
            this.mnuSDK35,
            this.mnuSDK4,
            this.mnuSDK45,
            this.toolStripMenuItem5,
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(40, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHome
            // 
            this.mnuHome.Name = "mnuHome";
            this.mnuHome.Size = new System.Drawing.Size(331, 22);
            this.mnuHome.Text = "Home Page";
            this.mnuHome.Click += new System.EventHandler(this.mnuHome_Click);
            // 
            // mnuEmailAuthor
            // 
            this.mnuEmailAuthor.Name = "mnuEmailAuthor";
            this.mnuEmailAuthor.Size = new System.Drawing.Size(331, 22);
            this.mnuEmailAuthor.Text = "Email Author";
            this.mnuEmailAuthor.Click += new System.EventHandler(this.mnuEmailAuthor_Click);
            // 
            // mnuCheckForUpdate
            // 
            this.mnuCheckForUpdate.Name = "mnuCheckForUpdate";
            this.mnuCheckForUpdate.Size = new System.Drawing.Size(331, 22);
            this.mnuCheckForUpdate.Text = "Check for Update";
            this.mnuCheckForUpdate.Click += new System.EventHandler(this.mnuCheckForUpdate_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(328, 6);
            // 
            // mnuSDK20
            // 
            this.mnuSDK20.Name = "mnuSDK20";
            this.mnuSDK20.Size = new System.Drawing.Size(331, 22);
            this.mnuSDK20.Text = ".Net Framework SDK 2.0";
            this.mnuSDK20.Click += new System.EventHandler(this.mnuSDK20_Click);
            // 
            // mnuSDK35
            // 
            this.mnuSDK35.Name = "mnuSDK35";
            this.mnuSDK35.Size = new System.Drawing.Size(331, 22);
            this.mnuSDK35.Text = "Windows SDK for .Net Framework 3.5";
            this.mnuSDK35.Click += new System.EventHandler(this.mnuSDK35_Click);
            // 
            // mnuSDK4
            // 
            this.mnuSDK4.Name = "mnuSDK4";
            this.mnuSDK4.Size = new System.Drawing.Size(331, 22);
            this.mnuSDK4.Text = "Windows SDK for Windows 7 and .Net Framework 4";
            this.mnuSDK4.Click += new System.EventHandler(this.mnuSDK4_Click);
            // 
            // mnuSDK45
            // 
            this.mnuSDK45.Name = "mnuSDK45";
            this.mnuSDK45.Size = new System.Drawing.Size(331, 22);
            this.mnuSDK45.Text = "Windows SDK for Windows 8 and .Net Framework 4.5";
            this.mnuSDK45.Click += new System.EventHandler(this.mnuSDK45_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(328, 6);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(331, 22);
            this.mnuAbout.Text = "&About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusInfo,
            this.statusProgress,
            this.statusVersion});
            this.statusStrip1.Location = new System.Drawing.Point(0, 408);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusInfo
            // 
            this.statusInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusInfo.Name = "statusInfo";
            this.statusInfo.Size = new System.Drawing.Size(731, 17);
            this.statusInfo.Spring = true;
            this.statusInfo.Text = "Ready";
            this.statusInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusProgress
            // 
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(100, 18);
            this.statusProgress.Visible = false;
            // 
            // statusVersion
            // 
            this.statusVersion.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusVersion.Name = "statusVersion";
            this.statusVersion.Size = new System.Drawing.Size(46, 17);
            this.statusVersion.Text = "Version";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 49);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(191, 359);
            this.treeView1.TabIndex = 2;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(191, 49);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 359);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.AutoGenerateColumns = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcFileName,
            this.dgcAssemblyName,
            this.dgcVersion,
            this.dgcToken,
            this.dgcProcessorArchitecture,
            this.dgcTargetRuntime,
            this.dgcFullName});
            this.dgvData.DataSource = this.bindingSource1;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(194, 49);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersWidth = 21;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(598, 359);
            this.dgvData.TabIndex = 4;
            this.dgvData.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellDoubleClick);
            this.dgvData.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_CellMouseDown);
            this.dgvData.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvData_KeyUp);
            // 
            // dgcFileName
            // 
            this.dgcFileName.ContextMenuStrip = this.cmAssembly;
            this.dgcFileName.DataPropertyName = "file_name";
            this.dgcFileName.HeaderText = "File Name";
            this.dgcFileName.MinimumWidth = 100;
            this.dgcFileName.Name = "dgcFileName";
            this.dgcFileName.ReadOnly = true;
            this.dgcFileName.ToolTipText = "Right click to filter content";
            // 
            // cmAssembly
            // 
            this.cmAssembly.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmAsm,
            this.cmDasm,
            this.cmDeobf,
            this.cmStrongName,
            this.cmVerify,
            this.toolStripMenuItem3,
            this.cmClassEditor,
            this.cmRunMethod,
            this.cmProfiler,
            this.cmPluginSepStart,
            this.cmMore,
            this.cmPluginSepEnd,
            this.cmCopyInfo,
            this.cmOpenFolder,
            this.cmDeleteFile});
            this.cmAssembly.Name = "cmAssembly";
            this.cmAssembly.Size = new System.Drawing.Size(140, 286);
            this.cmAssembly.Opening += new System.ComponentModel.CancelEventHandler(this.cmAssembly_Opening);
            // 
            // cmAsm
            // 
            this.cmAsm.Name = "cmAsm";
            this.cmAsm.Size = new System.Drawing.Size(139, 22);
            this.cmAsm.Text = "Assembler";
            this.cmAsm.Click += new System.EventHandler(this.cmAsm_Click);
            // 
            // cmDasm
            // 
            this.cmDasm.Image = global::SimpleAssemblyExplorer.Properties.Resources.ildasm;
            this.cmDasm.Name = "cmDasm";
            this.cmDasm.Size = new System.Drawing.Size(139, 22);
            this.cmDasm.Text = "Disassembler";
            this.cmDasm.Click += new System.EventHandler(this.cmDasm_Click);
            // 
            // cmDeobf
            // 
            this.cmDeobf.Name = "cmDeobf";
            this.cmDeobf.Size = new System.Drawing.Size(139, 22);
            this.cmDeobf.Text = "Deobfuscator";
            this.cmDeobf.Click += new System.EventHandler(this.cmDeobf_Click);
            // 
            // cmStrongName
            // 
            this.cmStrongName.Name = "cmStrongName";
            this.cmStrongName.Size = new System.Drawing.Size(139, 22);
            this.cmStrongName.Text = "Strong Name";
            this.cmStrongName.Click += new System.EventHandler(this.cmStrongName_Click);
            // 
            // cmVerify
            // 
            this.cmVerify.Name = "cmVerify";
            this.cmVerify.Size = new System.Drawing.Size(139, 22);
            this.cmVerify.Text = "PE Verify";
            this.cmVerify.Click += new System.EventHandler(this.cmVerify_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(136, 6);
            // 
            // cmClassEditor
            // 
            this.cmClassEditor.Image = global::SimpleAssemblyExplorer.Properties.Resources.classbrowser;
            this.cmClassEditor.ImageTransparentColor = System.Drawing.Color.Green;
            this.cmClassEditor.Name = "cmClassEditor";
            this.cmClassEditor.Size = new System.Drawing.Size(139, 22);
            this.cmClassEditor.Text = "Class Editor";
            this.cmClassEditor.Click += new System.EventHandler(this.cmClassEditor_Click);
            // 
            // cmRunMethod
            // 
            this.cmRunMethod.Image = global::SimpleAssemblyExplorer.Properties.Resources.runmethod;
            this.cmRunMethod.ImageTransparentColor = System.Drawing.Color.Green;
            this.cmRunMethod.Name = "cmRunMethod";
            this.cmRunMethod.Size = new System.Drawing.Size(139, 22);
            this.cmRunMethod.Text = "Run Method";
            this.cmRunMethod.Click += new System.EventHandler(this.cmRunMethod_Click);
            // 
            // cmProfiler
            // 
            this.cmProfiler.Name = "cmProfiler";
            this.cmProfiler.Size = new System.Drawing.Size(139, 22);
            this.cmProfiler.Text = "Profiler";
            this.cmProfiler.Click += new System.EventHandler(this.cmProfiler_Click);
            // 
            // cmPluginSepStart
            // 
            this.cmPluginSepStart.Name = "cmPluginSepStart";
            this.cmPluginSepStart.Size = new System.Drawing.Size(136, 6);
            // 
            // cmMore
            // 
            this.cmMore.Name = "cmMore";
            this.cmMore.Size = new System.Drawing.Size(139, 22);
            this.cmMore.Text = "More ...";
            this.cmMore.Visible = false;
            // 
            // cmPluginSepEnd
            // 
            this.cmPluginSepEnd.Name = "cmPluginSepEnd";
            this.cmPluginSepEnd.Size = new System.Drawing.Size(136, 6);
            // 
            // cmCopyInfo
            // 
            this.cmCopyInfo.Image = global::SimpleAssemblyExplorer.Properties.Resources.copy;
            this.cmCopyInfo.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.cmCopyInfo.Name = "cmCopyInfo";
            this.cmCopyInfo.Size = new System.Drawing.Size(139, 22);
            this.cmCopyInfo.Text = "Copy Info";
            this.cmCopyInfo.Click += new System.EventHandler(this.cmCopyInfo_Click);
            // 
            // cmOpenFolder
            // 
            this.cmOpenFolder.Image = global::SimpleAssemblyExplorer.Properties.Resources.openfolder;
            this.cmOpenFolder.ImageTransparentColor = System.Drawing.Color.Green;
            this.cmOpenFolder.Name = "cmOpenFolder";
            this.cmOpenFolder.Size = new System.Drawing.Size(139, 22);
            this.cmOpenFolder.Text = "Open Folder";
            this.cmOpenFolder.Click += new System.EventHandler(this.cmOpenFolder_Click);
            // 
            // cmDeleteFile
            // 
            this.cmDeleteFile.Image = global::SimpleAssemblyExplorer.Properties.Resources.delete;
            this.cmDeleteFile.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.cmDeleteFile.Name = "cmDeleteFile";
            this.cmDeleteFile.Size = new System.Drawing.Size(139, 22);
            this.cmDeleteFile.Text = "Delete File";
            this.cmDeleteFile.Click += new System.EventHandler(this.cmDeleteFile_Click);
            // 
            // dgcAssemblyName
            // 
            this.dgcAssemblyName.ContextMenuStrip = this.cmAssembly;
            this.dgcAssemblyName.DataPropertyName = "assembly_name";
            this.dgcAssemblyName.HeaderText = "Assembly Name";
            this.dgcAssemblyName.MinimumWidth = 100;
            this.dgcAssemblyName.Name = "dgcAssemblyName";
            this.dgcAssemblyName.ReadOnly = true;
            this.dgcAssemblyName.ToolTipText = "Right click to filter content";
            // 
            // dgcVersion
            // 
            this.dgcVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcVersion.ContextMenuStrip = this.cmAssembly;
            this.dgcVersion.DataPropertyName = "version";
            this.dgcVersion.HeaderText = "Version";
            this.dgcVersion.Name = "dgcVersion";
            this.dgcVersion.ReadOnly = true;
            this.dgcVersion.ToolTipText = "Right click to filter content";
            this.dgcVersion.Width = 150;
            // 
            // dgcToken
            // 
            this.dgcToken.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcToken.ContextMenuStrip = this.cmAssembly;
            this.dgcToken.DataPropertyName = "public_token";
            this.dgcToken.HeaderText = "Public Key Token";
            this.dgcToken.Name = "dgcToken";
            this.dgcToken.ReadOnly = true;
            this.dgcToken.ToolTipText = "Right click to filter content";
            this.dgcToken.Width = 125;
            // 
            // dgcProcessorArchitecture
            // 
            this.dgcProcessorArchitecture.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcProcessorArchitecture.ContextMenuStrip = this.cmAssembly;
            this.dgcProcessorArchitecture.DataPropertyName = "processor_architecture";
            this.dgcProcessorArchitecture.HeaderText = "Processor Architecture";
            this.dgcProcessorArchitecture.Name = "dgcProcessorArchitecture";
            this.dgcProcessorArchitecture.ReadOnly = true;
            this.dgcProcessorArchitecture.ToolTipText = "Right click to filter content";
            this.dgcProcessorArchitecture.Width = 80;
            // 
            // dgcTargetRuntime
            // 
            this.dgcTargetRuntime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcTargetRuntime.ContextMenuStrip = this.cmAssembly;
            this.dgcTargetRuntime.DataPropertyName = "target_runtime";
            this.dgcTargetRuntime.HeaderText = "Target Runtime";
            this.dgcTargetRuntime.Name = "dgcTargetRuntime";
            this.dgcTargetRuntime.ReadOnly = true;
            this.dgcTargetRuntime.Width = 50;
            // 
            // dgcFullName
            // 
            this.dgcFullName.DataPropertyName = "full_name";
            this.dgcFullName.HeaderText = "Full Name";
            this.dgcFullName.Name = "dgcFullName";
            this.dgcFullName.ReadOnly = true;
            this.dgcFullName.Visible = false;
            // 
            // cmFolder
            // 
            this.cmFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmFolderAsm,
            this.cmFolderDasm,
            this.toolStripMenuItem8,
            this.cmFolderOpen,
            this.cmFolderDelete,
            this.toolStripMenuItem9,
            this.cmRefresh});
            this.cmFolder.Name = "cmAssembly";
            this.cmFolder.Size = new System.Drawing.Size(139, 126);
            // 
            // cmFolderAsm
            // 
            this.cmFolderAsm.Name = "cmFolderAsm";
            this.cmFolderAsm.Size = new System.Drawing.Size(138, 22);
            this.cmFolderAsm.Text = "Assembler";
            this.cmFolderAsm.Click += new System.EventHandler(this.cmFolderAsm_Click);
            // 
            // cmFolderDasm
            // 
            this.cmFolderDasm.Image = global::SimpleAssemblyExplorer.Properties.Resources.ildasm;
            this.cmFolderDasm.Name = "cmFolderDasm";
            this.cmFolderDasm.Size = new System.Drawing.Size(138, 22);
            this.cmFolderDasm.Text = "Disassembler";
            this.cmFolderDasm.Click += new System.EventHandler(this.cmFolderDasm_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(135, 6);
            // 
            // cmFolderOpen
            // 
            this.cmFolderOpen.Image = global::SimpleAssemblyExplorer.Properties.Resources.openfolder;
            this.cmFolderOpen.ImageTransparentColor = System.Drawing.Color.Green;
            this.cmFolderOpen.Name = "cmFolderOpen";
            this.cmFolderOpen.Size = new System.Drawing.Size(138, 22);
            this.cmFolderOpen.Text = "Open Folder";
            this.cmFolderOpen.Click += new System.EventHandler(this.cmFolderOpen_Click);
            // 
            // cmFolderDelete
            // 
            this.cmFolderDelete.Image = global::SimpleAssemblyExplorer.Properties.Resources.delete;
            this.cmFolderDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.cmFolderDelete.Name = "cmFolderDelete";
            this.cmFolderDelete.Size = new System.Drawing.Size(138, 22);
            this.cmFolderDelete.Text = "Delete Folder";
            this.cmFolderDelete.Click += new System.EventHandler(this.cmFolderDelete_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(135, 6);
            // 
            // cmRefresh
            // 
            this.cmRefresh.Name = "cmRefresh";
            this.cmRefresh.ShortcutKeyDisplayString = "F5";
            this.cmRefresh.Size = new System.Drawing.Size(138, 22);
            this.cmRefresh.Text = "Refresh";
            this.cmRefresh.Click += new System.EventHandler(this.cmRefresh_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblPath,
            this.txtPath});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(792, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.SizeChanged += new System.EventHandler(this.toolStrip1_SizeChanged);
            // 
            // lblPath
            // 
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(29, 22);
            this.lblPath.Text = "Path";
            // 
            // txtPath
            // 
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtPath.AutoSize = false;
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(100, 25);
            this.txtPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyUp);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.Filter = "*.txt";
            this.fileSystemWatcher1.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fileSystemWatcher1.SynchronizingObject = this;
            this.fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Changed);
            this.fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Created);
            this.fileSystemWatcher1.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Deleted);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(792, 430);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.ShowIcon = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.cmAssembly.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.cmFolder.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusInfo;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripStatusLabel statusVersion;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ContextMenuStrip cmAssembly;
        private System.Windows.Forms.ToolStripMenuItem cmCopyInfo;
        private System.Windows.Forms.ToolStripSeparator cmPluginSepEnd;
        private System.Windows.Forms.ToolStripMenuItem cmDasm;
        private System.Windows.Forms.ToolStripMenuItem cmAsm;
        private System.Windows.Forms.ToolStripMenuItem cmDeobf;
        private System.Windows.Forms.ToolStripMenuItem cmDeleteFile;
        private System.Windows.Forms.ToolStripMenuItem cmOpenFolder;
        private System.Windows.Forms.ToolStripSeparator cmPluginSepStart;
        private System.Windows.Forms.ToolStripMenuItem cmClassEditor;
        private System.Windows.Forms.ToolStripMenuItem cmRunMethod;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.ToolStripMenuItem cmVerify;
        private System.Windows.Forms.ContextMenuStrip cmFolder;
        private System.Windows.Forms.ToolStripMenuItem cmFolderOpen;
        private System.Windows.Forms.ToolStripMenuItem cmFolderDelete;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuGenStrongKey;
        private System.Windows.Forms.ToolStripMenuItem cmStrongName;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuHome;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem mnuSDK20;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem mnuCheckForUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem cmFolderAsm;
        private System.Windows.Forms.ToolStripMenuItem cmFolderDasm;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem mnuEmailAuthor;
        private System.Windows.Forms.ToolStripMenuItem mnuSDK35;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblPath;
        private System.Windows.Forms.ToolStripTextBox txtPath;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem cmRefresh;
        private System.Windows.Forms.ToolStripMenuItem mnuProfileApp;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem cmProfiler;
        private System.Windows.Forms.ToolStripMenuItem mnuProfileASPNet;
        private System.Windows.Forms.ToolStripMenuItem cmMore;
        private System.Windows.Forms.ToolStripMenuItem mnuPluginList;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ToolStripMenuItem mnuSDK4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAssemblyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcToken;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcProcessorArchitecture;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTargetRuntime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcFullName;
        private System.Windows.Forms.ToolStripMenuItem mnuSDK45;
    }
}


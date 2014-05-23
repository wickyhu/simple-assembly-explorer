namespace SimpleAssemblyExplorer
{
    partial class frmClassEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClassEdit));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmAssembly = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmExpand = new System.Windows.Forms.ToolStripMenuItem();
            this.cmCollapse = new System.Windows.Forms.ToolStripMenuItem();
            this.cmExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cmCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.cmBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRename = new System.Windows.Forms.ToolStripMenuItem();
            this.cmCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.cmCopyNameAsHex = new System.Windows.Forms.ToolStripMenuItem();
            this.cmGotoEntryPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.cmCustomAttributes = new System.Windows.Forms.ToolStripMenuItem();
            this.cmViewInReflector = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.cmImportMethodFromAssembly = new System.Windows.Forms.ToolStripMenuItem();
            this.cmReadMethodFromAssembly = new System.Windows.Forms.ToolStripMenuItem();
            this.cmReadMethodFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRestoreMethodFromImage = new System.Windows.Forms.ToolStripMenuItem();
            this.cmWriteMethodToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmReadMethodsFromFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cmSaveResourceAs = new System.Windows.Forms.ToolStripMenuItem();
            this.cmViewResourceAsBinary = new System.Windows.Forms.ToolStripMenuItem();
            this.cmViewResourceAsNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.cmImportResourceFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRemoveResource = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ddbOpen = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenGAC = new System.Windows.Forms.ToolStripMenuItem();
            this.tbSave = new System.Windows.Forms.ToolStripButton();
            this.tbSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbBack = new System.Windows.Forms.ToolStripButton();
            this.tbForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lblSearch = new System.Windows.Forms.ToolStripLabel();
            this.cboSearchType = new System.Windows.Forms.ToolStripComboBox();
            this.cboSearch = new System.Windows.Forms.ToolStripComboBox();
            this.tbSearchNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cboBookmark = new System.Windows.Forms.ToolStripComboBox();
            this.tbSaveBookmark = new System.Windows.Forms.ToolStripButton();
            this.dgBody = new System.Windows.Forms.DataGridView();
            this.dgcIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmOp = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmInsertBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.cmInsertAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.cmDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.cmHighlight = new System.Windows.Forms.ToolStripMenuItem();
            this.cmDeobfBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmMakeBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmNop = new System.Windows.Forms.ToolStripMenuItem();
            this.cmCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmMove = new System.Windows.Forms.ToolStripMenuItem();
            this.cmRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.cmNewExceptionHandler = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmDeobf = new System.Windows.Forms.ToolStripMenuItem();
            this.cmSaveDetailsAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmMarkBlocks = new System.Windows.Forms.ToolStripMenuItem();
            this.dgcAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcOperand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcOperandType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDetails = new System.Windows.Forms.TabPage();
            this.dgResource = new System.Windows.Forms.DataGridView();
            this.dgvcResourceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcResourceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcResourceValue = new SimpleAssemblyExplorer.DataGridViewTextAndImageColumn();
            this.dgvcValueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmResource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmResourceSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceViewAsBinary = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceViewAsNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceImportFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.cmResourceBamlTranslator = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceReflectorBamlViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.cmResourceILSpyBamlDecompiler = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panelResource = new System.Windows.Forms.Panel();
            this.lvResource = new System.Windows.Forms.ListView();
            this.txtResource = new System.Windows.Forms.TextBox();
            this.hbResource = new Be.Windows.Forms.HexBox();
            this.pbResource = new System.Windows.Forms.PictureBox();
            this.tabReflector = new System.Windows.Forms.TabPage();
            this.cboOptimization = new System.Windows.Forms.ComboBox();
            this.lblOptimization = new System.Windows.Forms.Label();
            this.lblLoadedFile = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.rtbText = new System.Windows.Forms.RichTextBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.tabILSpy = new System.Windows.Forms.TabPage();
            this.lblILSpyLoadedFile = new System.Windows.Forms.Label();
            this.btnILSpyLoad = new System.Windows.Forms.Button();
            this.btnILSpyReload = new System.Windows.Forms.Button();
            this.rtbILSpyText = new System.Windows.Forms.RichTextBox();
            this.cboILSpyLanguage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.dgVariable = new System.Windows.Forms.DataGridView();
            this.dgvcVarIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmVariable = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmVarAppend = new System.Windows.Forms.ToolStripMenuItem();
            this.cmVarInsertBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.cmVarInsertAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.cmVarDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.cmVarEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmVarRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvcVarName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcVarType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcVarIsPinned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgGeneral = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmAssembly.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBody)).BeginInit();
            this.cmOp.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResource)).BeginInit();
            this.cmResource.SuspendLayout();
            this.panelResource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResource)).BeginInit();
            this.tabReflector.SuspendLayout();
            this.tabILSpy.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVariable)).BeginInit();
            this.cmVariable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgGeneral)).BeginInit();
            this.tabLog.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(272, 412);
            this.treeView1.TabIndex = 1;
            this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Green;
            this.imageList1.Images.SetKeyName(0, "Class.Browser16.1.bmp");
            this.imageList1.Images.SetKeyName(1, "Class.Browser16.2.bmp");
            this.imageList1.Images.SetKeyName(2, "Class.Browser16.3.bmp");
            this.imageList1.Images.SetKeyName(3, "Class.Browser16.4.bmp");
            this.imageList1.Images.SetKeyName(4, "Class.Browser16.5.bmp");
            this.imageList1.Images.SetKeyName(5, "Class.Browser16.6.bmp");
            this.imageList1.Images.SetKeyName(6, "Class.Browser16.7.bmp");
            this.imageList1.Images.SetKeyName(7, "Class.Browser16.8.bmp");
            this.imageList1.Images.SetKeyName(8, "Class.Browser16.9.bmp");
            this.imageList1.Images.SetKeyName(9, "Class.Browser16.10.bmp");
            this.imageList1.Images.SetKeyName(10, "Class.Browser16.11.bmp");
            this.imageList1.Images.SetKeyName(11, "Class.Browser16.12.bmp");
            this.imageList1.Images.SetKeyName(12, "Class.Browser16.13.bmp");
            this.imageList1.Images.SetKeyName(13, "Class.Browser16.14.bmp");
            this.imageList1.Images.SetKeyName(14, "Class.Browser16.15.bmp");
            this.imageList1.Images.SetKeyName(15, "Class.Browser16.16.bmp");
            this.imageList1.Images.SetKeyName(16, "Class.Browser16.17.bmp");
            this.imageList1.Images.SetKeyName(17, "Class.Browser16.18.bmp");
            this.imageList1.Images.SetKeyName(18, "Class.Browser16.19.bmp");
            this.imageList1.Images.SetKeyName(19, "Class.Browser16.20.bmp");
            this.imageList1.Images.SetKeyName(20, "Class.Browser16.21.bmp");
            this.imageList1.Images.SetKeyName(21, "Class.Browser16.22.bmp");
            this.imageList1.Images.SetKeyName(22, "Class.Browser16.23.bmp");
            this.imageList1.Images.SetKeyName(23, "Class.Browser16.24.bmp");
            this.imageList1.Images.SetKeyName(24, "Class.Browser16.25.bmp");
            this.imageList1.Images.SetKeyName(25, "Class.Browser16.26.bmp");
            this.imageList1.Images.SetKeyName(26, "Class.Browser16.27.bmp");
            this.imageList1.Images.SetKeyName(27, "Class.Browser16.28.bmp");
            this.imageList1.Images.SetKeyName(28, "Class.Browser16.29.bmp");
            this.imageList1.Images.SetKeyName(29, "Class.Browser16.30.bmp");
            this.imageList1.Images.SetKeyName(30, "Class.Browser16.31.bmp");
            this.imageList1.Images.SetKeyName(31, "Class.Browser16.32.bmp");
            this.imageList1.Images.SetKeyName(32, "Class.Browser16.33.bmp");
            this.imageList1.Images.SetKeyName(33, "Class.Browser16.34.bmp");
            this.imageList1.Images.SetKeyName(34, "Class.Browser16.35.bmp");
            this.imageList1.Images.SetKeyName(35, "Class.Browser16.36.bmp");
            this.imageList1.Images.SetKeyName(36, "Class.Browser16.37.bmp");
            this.imageList1.Images.SetKeyName(37, "Class.Browser16.38.bmp");
            this.imageList1.Images.SetKeyName(38, "Class.Browser16.39.bmp");
            this.imageList1.Images.SetKeyName(39, "Class.Browser16.40.bmp");
            this.imageList1.Images.SetKeyName(40, "Class.Browser16.41.bmp");
            this.imageList1.Images.SetKeyName(41, "Class.Browser16.42.bmp");
            this.imageList1.Images.SetKeyName(42, "Class.Browser16.43.bmp");
            this.imageList1.Images.SetKeyName(43, "Class.Browser16.44.bmp");
            this.imageList1.Images.SetKeyName(44, "Class.Browser16.45.bmp");
            this.imageList1.Images.SetKeyName(45, "Class.Browser16.46.bmp");
            this.imageList1.Images.SetKeyName(46, "Class.Browser16.47.bmp");
            this.imageList1.Images.SetKeyName(47, "Class.Browser16.48.bmp");
            this.imageList1.Images.SetKeyName(48, "Class.Browser16.49.bmp");
            this.imageList1.Images.SetKeyName(49, "Class.Browser16.50.bmp");
            this.imageList1.Images.SetKeyName(50, "Class.Browser16.51.bmp");
            this.imageList1.Images.SetKeyName(51, "Class.Browser16.52.bmp");
            this.imageList1.Images.SetKeyName(52, "Class.Browser16.53.bmp");
            this.imageList1.Images.SetKeyName(53, "Class.Browser16.54.bmp");
            this.imageList1.Images.SetKeyName(54, "Class.Browser16.55.bmp");
            this.imageList1.Images.SetKeyName(55, "Class.Browser16.56.bmp");
            this.imageList1.Images.SetKeyName(56, "Class.Browser16.57.bmp");
            this.imageList1.Images.SetKeyName(57, "Class.Browser16.58.bmp");
            this.imageList1.Images.SetKeyName(58, "Class.Browser16.59.bmp");
            this.imageList1.Images.SetKeyName(59, "Class.Browser16.60.bmp");
            this.imageList1.Images.SetKeyName(60, "Class.Browser16.61.bmp");
            this.imageList1.Images.SetKeyName(61, "Class.Browser16.62.bmp");
            this.imageList1.Images.SetKeyName(62, "Class.Browser16.63.bmp");
            this.imageList1.Images.SetKeyName(63, "Class.Browser16.64.bmp");
            this.imageList1.Images.SetKeyName(64, "Class.Browser16.65.bmp");
            this.imageList1.Images.SetKeyName(65, "Class.Browser16.66.bmp");
            this.imageList1.Images.SetKeyName(66, "Class.Browser16.67.bmp");
            this.imageList1.Images.SetKeyName(67, "Class.Browser16.68.bmp");
            this.imageList1.Images.SetKeyName(68, "Class.Browser16.69.bmp");
            this.imageList1.Images.SetKeyName(69, "Class.Browser16.70.bmp");
            this.imageList1.Images.SetKeyName(70, "Class.Browser16.71.bmp");
            this.imageList1.Images.SetKeyName(71, "Class.Browser16.72.bmp");
            this.imageList1.Images.SetKeyName(72, "Class.Browser16.73.bmp");
            this.imageList1.Images.SetKeyName(73, "Class.Browser16.74.bmp");
            this.imageList1.Images.SetKeyName(74, "Class.Browser16.75.bmp");
            this.imageList1.Images.SetKeyName(75, "Class.Browser16.76.bmp");
            this.imageList1.Images.SetKeyName(76, "Class.Browser16.77.bmp");
            this.imageList1.Images.SetKeyName(77, "Class.Browser16.78.bmp");
            this.imageList1.Images.SetKeyName(78, "Class.Browser16.79.bmp");
            this.imageList1.Images.SetKeyName(79, "Class.Browser16.80.bmp");
            this.imageList1.Images.SetKeyName(80, "Class.Browser16.81.bmp");
            this.imageList1.Images.SetKeyName(81, "Class.Browser16.82.bmp");
            this.imageList1.Images.SetKeyName(82, "Class.Browser16.83.bmp");
            this.imageList1.Images.SetKeyName(83, "Class.Browser16.84.bmp");
            this.imageList1.Images.SetKeyName(84, "Class.Browser16.85.bmp");
            this.imageList1.Images.SetKeyName(85, "Class.Browser16.86.bmp");
            this.imageList1.Images.SetKeyName(86, "Class.Browser16.87.bmp");
            this.imageList1.Images.SetKeyName(87, "Class.Browser16.88.bmp");
            this.imageList1.Images.SetKeyName(88, "Class.Browser16.89.bmp");
            this.imageList1.Images.SetKeyName(89, "Class.Browser16.90.bmp");
            this.imageList1.Images.SetKeyName(90, "Class.Browser16.91.bmp");
            this.imageList1.Images.SetKeyName(91, "Class.Browser16.92.bmp");
            this.imageList1.Images.SetKeyName(92, "Class.Browser16.93.bmp");
            this.imageList1.Images.SetKeyName(93, "Class.Browser16.94.bmp");
            this.imageList1.Images.SetKeyName(94, "Class.Browser16.95.bmp");
            this.imageList1.Images.SetKeyName(95, "Class.Browser16.96.bmp");
            this.imageList1.Images.SetKeyName(96, "Class.Browser16.97.bmp");
            this.imageList1.Images.SetKeyName(97, "Class.Browser16.98.bmp");
            this.imageList1.Images.SetKeyName(98, "Class.Browser16.99.bmp");
            this.imageList1.Images.SetKeyName(99, "Class.Browser16.100.bmp");
            this.imageList1.Images.SetKeyName(100, "Class.Browser16.101.bmp");
            this.imageList1.Images.SetKeyName(101, "Class.Browser16.102.bmp");
            this.imageList1.Images.SetKeyName(102, "Class.Browser16.103.bmp");
            this.imageList1.Images.SetKeyName(103, "Class.Browser16.104.bmp");
            this.imageList1.Images.SetKeyName(104, "Class.Browser16.105.bmp");
            this.imageList1.Images.SetKeyName(105, "Class.Browser16.106.bmp");
            this.imageList1.Images.SetKeyName(106, "Class.Browser16.107.bmp");
            this.imageList1.Images.SetKeyName(107, "Class.Browser16.108.bmp");
            this.imageList1.Images.SetKeyName(108, "Class.Browser16.109.bmp");
            this.imageList1.Images.SetKeyName(109, "Class.Browser16.110.bmp");
            this.imageList1.Images.SetKeyName(110, "Class.Browser16.111.bmp");
            this.imageList1.Images.SetKeyName(111, "Class.Browser16.112.bmp");
            this.imageList1.Images.SetKeyName(112, "Class.Browser16.113.bmp");
            this.imageList1.Images.SetKeyName(113, "Class.Browser16.114.bmp");
            this.imageList1.Images.SetKeyName(114, "Class.Browser16.115.bmp");
            this.imageList1.Images.SetKeyName(115, "Class.Browser16.116.bmp");
            this.imageList1.Images.SetKeyName(116, "Class.Browser16.117.bmp");
            this.imageList1.Images.SetKeyName(117, "Class.Browser16.118.bmp");
            this.imageList1.Images.SetKeyName(118, "Class.Browser16.119.bmp");
            this.imageList1.Images.SetKeyName(119, "Class.Browser16.120.bmp");
            this.imageList1.Images.SetKeyName(120, "Class.Browser16.121.bmp");
            this.imageList1.Images.SetKeyName(121, "Class.Browser16.122.bmp");
            this.imageList1.Images.SetKeyName(122, "Class.Browser16.123.bmp");
            this.imageList1.Images.SetKeyName(123, "Class.Browser16.124.bmp");
            this.imageList1.Images.SetKeyName(124, "Class.Browser16.125.bmp");
            this.imageList1.Images.SetKeyName(125, "Class.Browser16.126.bmp");
            this.imageList1.Images.SetKeyName(126, "Class.Browser16.127.bmp");
            this.imageList1.Images.SetKeyName(127, "Class.Browser16.128.bmp");
            this.imageList1.Images.SetKeyName(128, "Class.Browser16.129.bmp");
            this.imageList1.Images.SetKeyName(129, "Class.Browser16.130.bmp");
            this.imageList1.Images.SetKeyName(130, "Class.Browser16.131.bmp");
            this.imageList1.Images.SetKeyName(131, "Class.Browser16.132.bmp");
            this.imageList1.Images.SetKeyName(132, "Class.Browser16.133.bmp");
            this.imageList1.Images.SetKeyName(133, "Class.Browser16.134.bmp");
            this.imageList1.Images.SetKeyName(134, "Class.Browser16.135.bmp");
            this.imageList1.Images.SetKeyName(135, "Class.Browser16.136.bmp");
            this.imageList1.Images.SetKeyName(136, "Class.Browser16.137.bmp");
            this.imageList1.Images.SetKeyName(137, "Class.Browser16.138.bmp");
            this.imageList1.Images.SetKeyName(138, "Class.Browser16.139.bmp");
            this.imageList1.Images.SetKeyName(139, "Class.Browser16.140.bmp");
            this.imageList1.Images.SetKeyName(140, "Class.Browser16.141.bmp");
            this.imageList1.Images.SetKeyName(141, "Class.Browser16.142.bmp");
            this.imageList1.Images.SetKeyName(142, "Class.Browser16.143.bmp");
            this.imageList1.Images.SetKeyName(143, "Class.Browser16.144.bmp");
            this.imageList1.Images.SetKeyName(144, "Class.Browser16.145.bmp");
            this.imageList1.Images.SetKeyName(145, "Class.Browser16.146.bmp");
            this.imageList1.Images.SetKeyName(146, "Class.Browser16.147.bmp");
            this.imageList1.Images.SetKeyName(147, "Class.Browser16.148.bmp");
            this.imageList1.Images.SetKeyName(148, "Class.Browser16.149.bmp");
            this.imageList1.Images.SetKeyName(149, "Class.Browser16.150.bmp");
            this.imageList1.Images.SetKeyName(150, "Class.Browser16.151.bmp");
            this.imageList1.Images.SetKeyName(151, "Class.Browser16.152.bmp");
            this.imageList1.Images.SetKeyName(152, "Class.Browser16.153.bmp");
            this.imageList1.Images.SetKeyName(153, "Class.Browser16.154.bmp");
            this.imageList1.Images.SetKeyName(154, "Class.Browser16.155.bmp");
            this.imageList1.Images.SetKeyName(155, "Class.Browser16.156.bmp");
            this.imageList1.Images.SetKeyName(156, "Class.Browser16.157.bmp");
            this.imageList1.Images.SetKeyName(157, "Class.Browser16.158.bmp");
            this.imageList1.Images.SetKeyName(158, "Class.Browser16.159.bmp");
            this.imageList1.Images.SetKeyName(159, "Class.Browser16.160.bmp");
            this.imageList1.Images.SetKeyName(160, "Class.Browser16.161.bmp");
            this.imageList1.Images.SetKeyName(161, "Class.Browser16.162.bmp");
            this.imageList1.Images.SetKeyName(162, "Class.Browser16.163.bmp");
            this.imageList1.Images.SetKeyName(163, "Class.Browser16.164.bmp");
            this.imageList1.Images.SetKeyName(164, "Class.Browser16.165.bmp");
            this.imageList1.Images.SetKeyName(165, "Class.Browser16.166.bmp");
            this.imageList1.Images.SetKeyName(166, "Class.Browser16.167.bmp");
            this.imageList1.Images.SetKeyName(167, "Class.Browser16.168.bmp");
            // 
            // cmAssembly
            // 
            this.cmAssembly.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmExpand,
            this.cmCollapse,
            this.cmExpandAll,
            this.cmCollapseAll,
            this.toolStripMenuItem7,
            this.cmBookmark,
            this.cmRename,
            this.cmCopyName,
            this.cmCopyNameAsHex,
            this.cmGotoEntryPoint,
            this.toolStripMenuItem6,
            this.cmCustomAttributes,
            this.cmViewInReflector,
            this.toolStripMenuItem8,
            this.cmImportMethodFromAssembly,
            this.cmReadMethodFromAssembly,
            this.cmReadMethodFromFile,
            this.cmRestoreMethodFromImage,
            this.cmWriteMethodToFile,
            this.cmReadMethodsFromFolder,
            this.toolStripMenuItem4,
            this.cmSaveResourceAs,
            this.cmViewResourceAsBinary,
            this.cmViewResourceAsNormal,
            this.cmImportResourceFromFile,
            this.cmRemoveResource});
            this.cmAssembly.Name = "cmAssembly";
            this.cmAssembly.Size = new System.Drawing.Size(241, 512);
            this.cmAssembly.Opening += new System.ComponentModel.CancelEventHandler(this.cmAssembly_Opening);
            // 
            // cmExpand
            // 
            this.cmExpand.Name = "cmExpand";
            this.cmExpand.Size = new System.Drawing.Size(240, 22);
            this.cmExpand.Text = "Expand";
            this.cmExpand.Click += new System.EventHandler(this.cmExpand_Click);
            // 
            // cmCollapse
            // 
            this.cmCollapse.Name = "cmCollapse";
            this.cmCollapse.Size = new System.Drawing.Size(240, 22);
            this.cmCollapse.Text = "Collapse";
            this.cmCollapse.Click += new System.EventHandler(this.cmCollapse_Click);
            // 
            // cmExpandAll
            // 
            this.cmExpandAll.Name = "cmExpandAll";
            this.cmExpandAll.Size = new System.Drawing.Size(240, 22);
            this.cmExpandAll.Text = "Expand All";
            this.cmExpandAll.Click += new System.EventHandler(this.cmExpandAll_Click);
            // 
            // cmCollapseAll
            // 
            this.cmCollapseAll.Name = "cmCollapseAll";
            this.cmCollapseAll.Size = new System.Drawing.Size(240, 22);
            this.cmCollapseAll.Text = "Collapse All";
            this.cmCollapseAll.Click += new System.EventHandler(this.cmCollapseAll_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(237, 6);
            // 
            // cmBookmark
            // 
            this.cmBookmark.Name = "cmBookmark";
            this.cmBookmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.cmBookmark.Size = new System.Drawing.Size(240, 22);
            this.cmBookmark.Text = "Bookmark";
            this.cmBookmark.Click += new System.EventHandler(this.cmBookmark_Click);
            // 
            // cmRename
            // 
            this.cmRename.Name = "cmRename";
            this.cmRename.Size = new System.Drawing.Size(240, 22);
            this.cmRename.Text = "Rename";
            this.cmRename.Click += new System.EventHandler(this.cmRename_Click);
            // 
            // cmCopyName
            // 
            this.cmCopyName.Name = "cmCopyName";
            this.cmCopyName.Size = new System.Drawing.Size(240, 22);
            this.cmCopyName.Text = "Copy Name";
            this.cmCopyName.Click += new System.EventHandler(this.cmCopyName_Click);
            // 
            // cmCopyNameAsHex
            // 
            this.cmCopyNameAsHex.Name = "cmCopyNameAsHex";
            this.cmCopyNameAsHex.Size = new System.Drawing.Size(240, 22);
            this.cmCopyNameAsHex.Text = "Copy Name as Hex";
            this.cmCopyNameAsHex.Click += new System.EventHandler(this.cmCopyNameAsHex_Click);
            // 
            // cmGotoEntryPoint
            // 
            this.cmGotoEntryPoint.Name = "cmGotoEntryPoint";
            this.cmGotoEntryPoint.Size = new System.Drawing.Size(240, 22);
            this.cmGotoEntryPoint.Text = "Go to Entry Point";
            this.cmGotoEntryPoint.Click += new System.EventHandler(this.cmGotoEntryPoint_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(237, 6);
            // 
            // cmCustomAttributes
            // 
            this.cmCustomAttributes.Name = "cmCustomAttributes";
            this.cmCustomAttributes.Size = new System.Drawing.Size(240, 22);
            this.cmCustomAttributes.Text = "Custom Attributes";
            this.cmCustomAttributes.Click += new System.EventHandler(this.cmCustomAttributes_Click);
            // 
            // cmViewInReflector
            // 
            this.cmViewInReflector.Name = "cmViewInReflector";
            this.cmViewInReflector.Size = new System.Drawing.Size(240, 22);
            this.cmViewInReflector.Text = "View in Reflector";
            this.cmViewInReflector.Click += new System.EventHandler(this.cmViewInReflector_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(237, 6);
            // 
            // cmImportMethodFromAssembly
            // 
            this.cmImportMethodFromAssembly.Name = "cmImportMethodFromAssembly";
            this.cmImportMethodFromAssembly.Size = new System.Drawing.Size(240, 22);
            this.cmImportMethodFromAssembly.Text = "Import Method From Assembly";
            this.cmImportMethodFromAssembly.Click += new System.EventHandler(this.cmImportMethodFromAssembly_Click);
            // 
            // cmReadMethodFromAssembly
            // 
            this.cmReadMethodFromAssembly.Name = "cmReadMethodFromAssembly";
            this.cmReadMethodFromAssembly.Size = new System.Drawing.Size(240, 22);
            this.cmReadMethodFromAssembly.Text = "Read Method From Assembly";
            this.cmReadMethodFromAssembly.Click += new System.EventHandler(this.cmReadMethodFromAssembly_Click);
            // 
            // cmReadMethodFromFile
            // 
            this.cmReadMethodFromFile.Name = "cmReadMethodFromFile";
            this.cmReadMethodFromFile.Size = new System.Drawing.Size(240, 22);
            this.cmReadMethodFromFile.Text = "Read Method From File";
            this.cmReadMethodFromFile.Click += new System.EventHandler(this.cmReadMethodFromFile_Click);
            // 
            // cmRestoreMethodFromImage
            // 
            this.cmRestoreMethodFromImage.Name = "cmRestoreMethodFromImage";
            this.cmRestoreMethodFromImage.Size = new System.Drawing.Size(240, 22);
            this.cmRestoreMethodFromImage.Text = "Restore Method From Image";
            this.cmRestoreMethodFromImage.Click += new System.EventHandler(this.cmRestoreMethodFromImage_Click);
            // 
            // cmWriteMethodToFile
            // 
            this.cmWriteMethodToFile.Name = "cmWriteMethodToFile";
            this.cmWriteMethodToFile.Size = new System.Drawing.Size(240, 22);
            this.cmWriteMethodToFile.Text = "Write Method To File";
            this.cmWriteMethodToFile.Click += new System.EventHandler(this.cmWriteMethodToFile_Click);
            // 
            // cmReadMethodsFromFolder
            // 
            this.cmReadMethodsFromFolder.Name = "cmReadMethodsFromFolder";
            this.cmReadMethodsFromFolder.Size = new System.Drawing.Size(240, 22);
            this.cmReadMethodsFromFolder.Text = "Read Methods From Folder";
            this.cmReadMethodsFromFolder.Click += new System.EventHandler(this.cmReadMethodsFromFolder_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(237, 6);
            // 
            // cmSaveResourceAs
            // 
            this.cmSaveResourceAs.Name = "cmSaveResourceAs";
            this.cmSaveResourceAs.Size = new System.Drawing.Size(240, 22);
            this.cmSaveResourceAs.Text = "Save Resource As";
            this.cmSaveResourceAs.Click += new System.EventHandler(this.cmSaveResourceAs_Click);
            // 
            // cmViewResourceAsBinary
            // 
            this.cmViewResourceAsBinary.Name = "cmViewResourceAsBinary";
            this.cmViewResourceAsBinary.Size = new System.Drawing.Size(240, 22);
            this.cmViewResourceAsBinary.Text = "View as Binary";
            this.cmViewResourceAsBinary.Click += new System.EventHandler(this.cmViewResourceAsBinary_Click);
            // 
            // cmViewResourceAsNormal
            // 
            this.cmViewResourceAsNormal.Name = "cmViewResourceAsNormal";
            this.cmViewResourceAsNormal.Size = new System.Drawing.Size(240, 22);
            this.cmViewResourceAsNormal.Text = "View as Normal";
            this.cmViewResourceAsNormal.Click += new System.EventHandler(this.cmViewResourceAsNormal_Click);
            // 
            // cmImportResourceFromFile
            // 
            this.cmImportResourceFromFile.Name = "cmImportResourceFromFile";
            this.cmImportResourceFromFile.Size = new System.Drawing.Size(240, 22);
            this.cmImportResourceFromFile.Text = "Import Resource From File";
            this.cmImportResourceFromFile.Click += new System.EventHandler(this.cmImportResourceFromFile_Click);
            // 
            // cmRemoveResource
            // 
            this.cmRemoveResource.Name = "cmRemoveResource";
            this.cmRemoveResource.Size = new System.Drawing.Size(240, 22);
            this.cmRemoveResource.Text = "Remove Resource";
            this.cmRemoveResource.Click += new System.EventHandler(this.cmRemoveResource_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddbOpen,
            this.tbSave,
            this.tbSelect,
            this.toolStripSeparator2,
            this.tbBack,
            this.tbForward,
            this.toolStripSeparator3,
            this.lblSearch,
            this.cboSearchType,
            this.cboSearch,
            this.tbSearchNext,
            this.toolStripSeparator4,
            this.toolStripLabel1,
            this.cboBookmark,
            this.tbSaveBookmark});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(870, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ddbOpen
            // 
            this.ddbOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpenFile,
            this.mnuOpenGAC});
            this.ddbOpen.Image = global::SimpleAssemblyExplorer.Properties.Resources.openfolder;
            this.ddbOpen.ImageTransparentColor = System.Drawing.Color.Green;
            this.ddbOpen.Name = "ddbOpen";
            this.ddbOpen.Size = new System.Drawing.Size(65, 22);
            this.ddbOpen.Text = "Open";
            // 
            // mnuOpenFile
            // 
            this.mnuOpenFile.Name = "mnuOpenFile";
            this.mnuOpenFile.Size = new System.Drawing.Size(130, 22);
            this.mnuOpenFile.Text = "Open File";
            this.mnuOpenFile.Click += new System.EventHandler(this.mnuOpenFile_Click);
            // 
            // mnuOpenGAC
            // 
            this.mnuOpenGAC.Name = "mnuOpenGAC";
            this.mnuOpenGAC.Size = new System.Drawing.Size(130, 22);
            this.mnuOpenGAC.Text = "Open GAC";
            this.mnuOpenGAC.Click += new System.EventHandler(this.mnuOpenGAC_Click);
            // 
            // tbSave
            // 
            this.tbSave.Image = global::SimpleAssemblyExplorer.Properties.Resources.Save;
            this.tbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSave.Name = "tbSave";
            this.tbSave.Size = new System.Drawing.Size(51, 23);
            this.tbSave.Text = "Save";
            this.tbSave.ToolTipText = "Save Assembly";
            this.tbSave.Visible = false;
            this.tbSave.Click += new System.EventHandler(this.tbSave_Click);
            // 
            // tbSelect
            // 
            this.tbSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSelect.Name = "tbSelect";
            this.tbSelect.Size = new System.Drawing.Size(42, 23);
            this.tbSelect.Text = "Select";
            this.tbSelect.Visible = false;
            this.tbSelect.Click += new System.EventHandler(this.tbSelect_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbBack
            // 
            this.tbBack.Image = global::SimpleAssemblyExplorer.Properties.Resources.NavBack;
            this.tbBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbBack.Name = "tbBack";
            this.tbBack.Size = new System.Drawing.Size(52, 22);
            this.tbBack.Text = "Back";
            this.tbBack.Click += new System.EventHandler(this.tbBack_Click);
            // 
            // tbForward
            // 
            this.tbForward.Image = global::SimpleAssemblyExplorer.Properties.Resources.NavForward;
            this.tbForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbForward.Name = "tbForward";
            this.tbForward.Size = new System.Drawing.Size(70, 22);
            this.tbForward.Text = "Forward";
            this.tbForward.Click += new System.EventHandler(this.tbForward_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // lblSearch
            // 
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(42, 22);
            this.lblSearch.Text = "Search";
            // 
            // cboSearchType
            // 
            this.cboSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSearchType.DropDownWidth = 115;
            this.cboSearchType.IntegralHeight = false;
            this.cboSearchType.MaxDropDownItems = 20;
            this.cboSearchType.Name = "cboSearchType";
            this.cboSearchType.Size = new System.Drawing.Size(105, 25);
            this.cboSearchType.SelectedIndexChanged += new System.EventHandler(this.cboSearchType_SelectedIndexChanged);
            // 
            // cboSearch
            // 
            this.cboSearch.DropDownWidth = 300;
            this.cboSearch.MaxDropDownItems = 16;
            this.cboSearch.Name = "cboSearch";
            this.cboSearch.Size = new System.Drawing.Size(170, 25);
            this.cboSearch.SelectedIndexChanged += new System.EventHandler(this.cboSearch_SelectedIndexChanged);
            this.cboSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboSearch_KeyUp);
            // 
            // tbSearchNext
            // 
            this.tbSearchNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbSearchNext.Image = ((System.Drawing.Image)(resources.GetObject("tbSearchNext.Image")));
            this.tbSearchNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSearchNext.Name = "tbSearchNext";
            this.tbSearchNext.Size = new System.Drawing.Size(35, 22);
            this.tbSearchNext.Text = "Next";
            this.tbSearchNext.ToolTipText = "Search Next";
            this.tbSearchNext.Click += new System.EventHandler(this.tbSearchNext_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(61, 22);
            this.toolStripLabel1.Text = "Bookmark";
            // 
            // cboBookmark
            // 
            this.cboBookmark.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBookmark.DropDownWidth = 350;
            this.cboBookmark.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBookmark.MaxDropDownItems = 50;
            this.cboBookmark.Name = "cboBookmark";
            this.cboBookmark.Size = new System.Drawing.Size(200, 25);
            this.cboBookmark.SelectedIndexChanged += new System.EventHandler(this.cboBookmark_SelectedIndexChanged);
            // 
            // tbSaveBookmark
            // 
            this.tbSaveBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSaveBookmark.Image = global::SimpleAssemblyExplorer.Properties.Resources.Save;
            this.tbSaveBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSaveBookmark.Name = "tbSaveBookmark";
            this.tbSaveBookmark.Size = new System.Drawing.Size(23, 22);
            this.tbSaveBookmark.Text = "Save Bookmark";
            this.tbSaveBookmark.Click += new System.EventHandler(this.tbSaveBookmark_Click);
            // 
            // dgBody
            // 
            this.dgBody.AllowDrop = true;
            this.dgBody.AllowUserToAddRows = false;
            this.dgBody.AllowUserToDeleteRows = false;
            this.dgBody.AllowUserToResizeColumns = false;
            this.dgBody.AllowUserToResizeRows = false;
            this.dgBody.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgBody.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgBody.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgBody.ColumnHeadersVisible = false;
            this.dgBody.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcIndex,
            this.dgcAddress,
            this.dgcOpCode,
            this.dgcOperand,
            this.dgcOperandType});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgBody.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgBody.Location = new System.Drawing.Point(3, 3);
            this.dgBody.Name = "dgBody";
            this.dgBody.ReadOnly = true;
            this.dgBody.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgBody.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgBody.RowTemplate.Height = 16;
            this.dgBody.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgBody.Size = new System.Drawing.Size(581, 56);
            this.dgBody.TabIndex = 0;
            this.dgBody.Visible = false;
            this.dgBody.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgBody_CellFormatting);
            this.dgBody.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgBody_CellMouseDoubleClick);
            this.dgBody.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgBody_CellMouseDown);
            this.dgBody.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgBody_CellToolTipTextNeeded);
            this.dgBody.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgBody_DragDrop);
            this.dgBody.DragOver += new System.Windows.Forms.DragEventHandler(this.dgBody_DragOver);
            this.dgBody.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgBody_KeyUp);
            this.dgBody.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgBody_MouseDown);
            this.dgBody.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgBody_MouseMove);
            // 
            // dgcIndex
            // 
            this.dgcIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.dgcIndex.ContextMenuStrip = this.cmOp;
            this.dgcIndex.DataPropertyName = "index";
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.dgcIndex.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgcIndex.HeaderText = "Index";
            this.dgcIndex.Name = "dgcIndex";
            this.dgcIndex.ReadOnly = true;
            this.dgcIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgcIndex.Width = 5;
            // 
            // cmOp
            // 
            this.cmOp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmEdit,
            this.cmInsertBefore,
            this.cmInsertAfter,
            this.cmDuplicate,
            this.cmHighlight,
            this.cmDeobfBranch,
            this.cmMakeBranch,
            this.toolStripMenuItem1,
            this.cmNop,
            this.cmCopy,
            this.cmPaste,
            this.cmMove,
            this.cmRemove,
            this.cmNewExceptionHandler,
            this.toolStripMenuItem2,
            this.cmDeobf,
            this.cmSaveDetailsAs,
            this.toolStripMenuItem3,
            this.cmMarkBlocks});
            this.cmOp.Name = "mnuOp";
            this.cmOp.Size = new System.Drawing.Size(198, 374);
            this.cmOp.Opening += new System.ComponentModel.CancelEventHandler(this.cmOp_Opening);
            // 
            // cmEdit
            // 
            this.cmEdit.Name = "cmEdit";
            this.cmEdit.Size = new System.Drawing.Size(197, 22);
            this.cmEdit.Text = "Edit";
            this.cmEdit.Click += new System.EventHandler(this.cmEdit_Click);
            // 
            // cmInsertBefore
            // 
            this.cmInsertBefore.Name = "cmInsertBefore";
            this.cmInsertBefore.Size = new System.Drawing.Size(197, 22);
            this.cmInsertBefore.Text = "Insert Before";
            this.cmInsertBefore.Click += new System.EventHandler(this.cmInsertBefore_Click);
            // 
            // cmInsertAfter
            // 
            this.cmInsertAfter.Name = "cmInsertAfter";
            this.cmInsertAfter.Size = new System.Drawing.Size(197, 22);
            this.cmInsertAfter.Text = "Insert After";
            this.cmInsertAfter.Click += new System.EventHandler(this.cmInsertAfter_Click);
            // 
            // cmDuplicate
            // 
            this.cmDuplicate.Name = "cmDuplicate";
            this.cmDuplicate.Size = new System.Drawing.Size(197, 22);
            this.cmDuplicate.Text = "Duplicate";
            this.cmDuplicate.Click += new System.EventHandler(this.cmDuplicate_Click);
            // 
            // cmHighlight
            // 
            this.cmHighlight.Name = "cmHighlight";
            this.cmHighlight.Size = new System.Drawing.Size(197, 22);
            this.cmHighlight.Text = "Highlight";
            // 
            // cmDeobfBranch
            // 
            this.cmDeobfBranch.Name = "cmDeobfBranch";
            this.cmDeobfBranch.Size = new System.Drawing.Size(197, 22);
            this.cmDeobfBranch.Text = "Deobfuscate Branch";
            this.cmDeobfBranch.Click += new System.EventHandler(this.cmDeobfBranch_Click);
            // 
            // cmMakeBranch
            // 
            this.cmMakeBranch.Name = "cmMakeBranch";
            this.cmMakeBranch.Size = new System.Drawing.Size(197, 22);
            this.cmMakeBranch.Text = "Make Branch";
            this.cmMakeBranch.Click += new System.EventHandler(this.cmMakeBranch_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 6);
            // 
            // cmNop
            // 
            this.cmNop.Name = "cmNop";
            this.cmNop.Size = new System.Drawing.Size(197, 22);
            this.cmNop.Text = "Nop";
            this.cmNop.Click += new System.EventHandler(this.cmNop_Click);
            // 
            // cmCopy
            // 
            this.cmCopy.Name = "cmCopy";
            this.cmCopy.Size = new System.Drawing.Size(197, 22);
            this.cmCopy.Text = "Copy";
            this.cmCopy.Click += new System.EventHandler(this.cmCopy_Click);
            // 
            // cmPaste
            // 
            this.cmPaste.Name = "cmPaste";
            this.cmPaste.Size = new System.Drawing.Size(197, 22);
            this.cmPaste.Text = "Paste";
            this.cmPaste.Click += new System.EventHandler(this.cmPaste_Click);
            // 
            // cmMove
            // 
            this.cmMove.Name = "cmMove";
            this.cmMove.Size = new System.Drawing.Size(197, 22);
            this.cmMove.Text = "Move";
            this.cmMove.Click += new System.EventHandler(this.cmMove_Click);
            // 
            // cmRemove
            // 
            this.cmRemove.Name = "cmRemove";
            this.cmRemove.Size = new System.Drawing.Size(197, 22);
            this.cmRemove.Text = "Remove";
            this.cmRemove.Click += new System.EventHandler(this.cmRemove_Click);
            // 
            // cmNewExceptionHandler
            // 
            this.cmNewExceptionHandler.Name = "cmNewExceptionHandler";
            this.cmNewExceptionHandler.Size = new System.Drawing.Size(197, 22);
            this.cmNewExceptionHandler.Text = "New Exception Handler";
            this.cmNewExceptionHandler.Click += new System.EventHandler(this.cmNewExceptionHandler_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(194, 6);
            // 
            // cmDeobf
            // 
            this.cmDeobf.Name = "cmDeobf";
            this.cmDeobf.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.cmDeobf.Size = new System.Drawing.Size(197, 22);
            this.cmDeobf.Text = "&Deobfuscator";
            this.cmDeobf.Click += new System.EventHandler(this.cmDeobf_Click);
            // 
            // cmSaveDetailsAs
            // 
            this.cmSaveDetailsAs.Name = "cmSaveDetailsAs";
            this.cmSaveDetailsAs.Size = new System.Drawing.Size(197, 22);
            this.cmSaveDetailsAs.Text = "Save As";
            this.cmSaveDetailsAs.Click += new System.EventHandler(this.cmSaveDetailsAs_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(194, 6);
            // 
            // cmMarkBlocks
            // 
            this.cmMarkBlocks.Name = "cmMarkBlocks";
            this.cmMarkBlocks.Size = new System.Drawing.Size(197, 22);
            this.cmMarkBlocks.Text = "Mark Blocks";
            this.cmMarkBlocks.Click += new System.EventHandler(this.cmMarkBlocks_Click);
            // 
            // dgcAddress
            // 
            this.dgcAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.dgcAddress.ContextMenuStrip = this.cmOp;
            this.dgcAddress.DataPropertyName = "address";
            this.dgcAddress.HeaderText = "Address";
            this.dgcAddress.Name = "dgcAddress";
            this.dgcAddress.ReadOnly = true;
            this.dgcAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgcAddress.Width = 5;
            // 
            // dgcOpCode
            // 
            this.dgcOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.dgcOpCode.ContextMenuStrip = this.cmOp;
            this.dgcOpCode.DataPropertyName = "opcode";
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Green;
            this.dgcOpCode.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgcOpCode.HeaderText = "OpCode";
            this.dgcOpCode.Name = "dgcOpCode";
            this.dgcOpCode.ReadOnly = true;
            this.dgcOpCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgcOpCode.Width = 5;
            // 
            // dgcOperand
            // 
            this.dgcOperand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcOperand.ContextMenuStrip = this.cmOp;
            this.dgcOperand.DataPropertyName = "operand";
            this.dgcOperand.HeaderText = "Operand";
            this.dgcOperand.Name = "dgcOperand";
            this.dgcOperand.ReadOnly = true;
            this.dgcOperand.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgcOperandType
            // 
            this.dgcOperandType.DataPropertyName = "operandtype";
            this.dgcOperandType.HeaderText = "Operand Type";
            this.dgcOperandType.Name = "dgcOperandType";
            this.dgcOperandType.ReadOnly = true;
            this.dgcOperandType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgcOperandType.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(272, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 412);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDetails);
            this.tabControl1.Controls.Add(this.tabReflector);
            this.tabControl1.Controls.Add(this.tabILSpy);
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(275, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(595, 412);
            this.tabControl1.TabIndex = 2;
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.dgBody);
            this.tabDetails.Controls.Add(this.dgResource);
            this.tabDetails.Controls.Add(this.splitter3);
            this.tabDetails.Controls.Add(this.panelResource);
            this.tabDetails.Location = new System.Drawing.Point(4, 22);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabDetails.Size = new System.Drawing.Size(587, 386);
            this.tabDetails.TabIndex = 0;
            this.tabDetails.Text = "Details";
            this.tabDetails.UseVisualStyleBackColor = true;
            // 
            // dgResource
            // 
            this.dgResource.AllowDrop = true;
            this.dgResource.AllowUserToAddRows = false;
            this.dgResource.AllowUserToDeleteRows = false;
            this.dgResource.AllowUserToResizeRows = false;
            this.dgResource.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgResource.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgResource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResource.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcResourceNo,
            this.dgvcResourceName,
            this.dgvcResourceValue,
            this.dgvcValueType});
            this.dgResource.ContextMenuStrip = this.cmResource;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgResource.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgResource.Location = new System.Drawing.Point(0, 65);
            this.dgResource.Name = "dgResource";
            this.dgResource.ReadOnly = true;
            this.dgResource.RowHeadersVisible = false;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgResource.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgResource.RowTemplate.Height = 18;
            this.dgResource.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgResource.Size = new System.Drawing.Size(581, 60);
            this.dgResource.TabIndex = 1;
            this.dgResource.Visible = false;
            this.dgResource.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgResource_CellMouseDown);
            this.dgResource.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResource_RowEnter);
            this.dgResource.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgResource_KeyUp);
            // 
            // dgvcResourceNo
            // 
            this.dgvcResourceNo.DataPropertyName = "no";
            this.dgvcResourceNo.FillWeight = 30F;
            this.dgvcResourceNo.HeaderText = "#";
            this.dgvcResourceNo.Name = "dgvcResourceNo";
            this.dgvcResourceNo.ReadOnly = true;
            this.dgvcResourceNo.Width = 30;
            // 
            // dgvcResourceName
            // 
            this.dgvcResourceName.DataPropertyName = "name";
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.dgvcResourceName.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvcResourceName.HeaderText = "Resource Name";
            this.dgvcResourceName.MinimumWidth = 250;
            this.dgvcResourceName.Name = "dgvcResourceName";
            this.dgvcResourceName.ReadOnly = true;
            this.dgvcResourceName.Width = 300;
            // 
            // dgvcResourceValue
            // 
            this.dgvcResourceValue.DataPropertyName = "value";
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Green;
            this.dgvcResourceValue.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvcResourceValue.HeaderText = "Value";
            this.dgvcResourceValue.MinimumWidth = 400;
            this.dgvcResourceValue.Name = "dgvcResourceValue";
            this.dgvcResourceValue.ReadOnly = true;
            this.dgvcResourceValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcResourceValue.Width = 400;
            // 
            // dgvcValueType
            // 
            this.dgvcValueType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcValueType.DataPropertyName = "type";
            this.dgvcValueType.HeaderText = "Value Type";
            this.dgvcValueType.MinimumWidth = 100;
            this.dgvcValueType.Name = "dgvcValueType";
            this.dgvcValueType.ReadOnly = true;
            // 
            // cmResource
            // 
            this.cmResource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmResourceSaveAs,
            this.cmResourceViewAsBinary,
            this.cmResourceViewAsNormal,
            this.cmResourceImportFromFile,
            this.cmResourceRemove,
            this.toolStripMenuItem9,
            this.cmResourceBamlTranslator});
            this.cmResource.Name = "cmAssembly";
            this.cmResource.Size = new System.Drawing.Size(214, 142);
            this.cmResource.Opening += new System.ComponentModel.CancelEventHandler(this.cmResource_Opening);
            // 
            // cmResourceSaveAs
            // 
            this.cmResourceSaveAs.Name = "cmResourceSaveAs";
            this.cmResourceSaveAs.Size = new System.Drawing.Size(213, 22);
            this.cmResourceSaveAs.Text = "Save Resource As";
            this.cmResourceSaveAs.Click += new System.EventHandler(this.cmResourceSaveAs_Click);
            // 
            // cmResourceViewAsBinary
            // 
            this.cmResourceViewAsBinary.Name = "cmResourceViewAsBinary";
            this.cmResourceViewAsBinary.Size = new System.Drawing.Size(213, 22);
            this.cmResourceViewAsBinary.Text = "View as Binary";
            this.cmResourceViewAsBinary.Click += new System.EventHandler(this.cmResourceViewAsBinary_Click);
            // 
            // cmResourceViewAsNormal
            // 
            this.cmResourceViewAsNormal.Name = "cmResourceViewAsNormal";
            this.cmResourceViewAsNormal.Size = new System.Drawing.Size(213, 22);
            this.cmResourceViewAsNormal.Text = "View as Normal";
            this.cmResourceViewAsNormal.Click += new System.EventHandler(this.cmResourceViewAsNormal_Click);
            // 
            // cmResourceImportFromFile
            // 
            this.cmResourceImportFromFile.Name = "cmResourceImportFromFile";
            this.cmResourceImportFromFile.Size = new System.Drawing.Size(213, 22);
            this.cmResourceImportFromFile.Text = "Import Resource From File";
            this.cmResourceImportFromFile.Click += new System.EventHandler(this.cmResourceImportFromFile_Click);
            // 
            // cmResourceRemove
            // 
            this.cmResourceRemove.Name = "cmResourceRemove";
            this.cmResourceRemove.Size = new System.Drawing.Size(213, 22);
            this.cmResourceRemove.Text = "Remove Resource";
            this.cmResourceRemove.Click += new System.EventHandler(this.cmResourceRemove_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(210, 6);
            // 
            // cmResourceBamlTranslator
            // 
            this.cmResourceBamlTranslator.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmResourceReflectorBamlViewer,
            this.cmResourceILSpyBamlDecompiler});
            this.cmResourceBamlTranslator.Name = "cmResourceBamlTranslator";
            this.cmResourceBamlTranslator.Size = new System.Drawing.Size(213, 22);
            this.cmResourceBamlTranslator.Text = "Baml Translator";
            // 
            // cmResourceReflectorBamlViewer
            // 
            this.cmResourceReflectorBamlViewer.Name = "cmResourceReflectorBamlViewer";
            this.cmResourceReflectorBamlViewer.Size = new System.Drawing.Size(193, 22);
            this.cmResourceReflectorBamlViewer.Text = "Reflector.BamlViewer";
            this.cmResourceReflectorBamlViewer.Click += new System.EventHandler(this.cmResourceReflectorBamlViewer_Click);
            // 
            // cmResourceILSpyBamlDecompiler
            // 
            this.cmResourceILSpyBamlDecompiler.Name = "cmResourceILSpyBamlDecompiler";
            this.cmResourceILSpyBamlDecompiler.Size = new System.Drawing.Size(193, 22);
            this.cmResourceILSpyBamlDecompiler.Text = "ILSpy.BamlDecompiler";
            this.cmResourceILSpyBamlDecompiler.Click += new System.EventHandler(this.cmResourceILSpyBamlDecompiler_Click);
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(3, 256);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(581, 3);
            this.splitter3.TabIndex = 2;
            this.splitter3.TabStop = false;
            // 
            // panelResource
            // 
            this.panelResource.Controls.Add(this.lvResource);
            this.panelResource.Controls.Add(this.txtResource);
            this.panelResource.Controls.Add(this.hbResource);
            this.panelResource.Controls.Add(this.pbResource);
            this.panelResource.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelResource.Location = new System.Drawing.Point(3, 259);
            this.panelResource.Name = "panelResource";
            this.panelResource.Size = new System.Drawing.Size(581, 124);
            this.panelResource.TabIndex = 3;
            this.panelResource.Visible = false;
            this.panelResource.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelResource_MouseClick);
            // 
            // lvResource
            // 
            this.lvResource.Location = new System.Drawing.Point(409, 46);
            this.lvResource.Name = "lvResource";
            this.lvResource.Size = new System.Drawing.Size(97, 50);
            this.lvResource.TabIndex = 2;
            this.lvResource.UseCompatibleStateImageBehavior = false;
            this.lvResource.Visible = false;
            // 
            // txtResource
            // 
            this.txtResource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.txtResource.Location = new System.Drawing.Point(143, 14);
            this.txtResource.Multiline = true;
            this.txtResource.Name = "txtResource";
            this.txtResource.ReadOnly = true;
            this.txtResource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResource.Size = new System.Drawing.Size(60, 82);
            this.txtResource.TabIndex = 0;
            this.txtResource.Visible = false;
            this.txtResource.WordWrap = false;
            // 
            // hbResource
            // 
            this.hbResource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.hbResource.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hbResource.InfoForeColor = System.Drawing.Color.Empty;
            this.hbResource.LineInfoVisible = true;
            this.hbResource.Location = new System.Drawing.Point(316, 55);
            this.hbResource.Name = "hbResource";
            this.hbResource.ReadOnly = true;
            this.hbResource.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hbResource.Size = new System.Drawing.Size(63, 41);
            this.hbResource.StringViewVisible = true;
            this.hbResource.TabIndex = 1;
            this.hbResource.UseFixedBytesPerLine = true;
            this.hbResource.Visible = false;
            this.hbResource.VScrollBarVisible = true;
            // 
            // pbResource
            // 
            this.pbResource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResource.Location = new System.Drawing.Point(219, 46);
            this.pbResource.Name = "pbResource";
            this.pbResource.Size = new System.Drawing.Size(70, 50);
            this.pbResource.TabIndex = 8;
            this.pbResource.TabStop = false;
            this.pbResource.Visible = false;
            // 
            // tabReflector
            // 
            this.tabReflector.Controls.Add(this.cboOptimization);
            this.tabReflector.Controls.Add(this.lblOptimization);
            this.tabReflector.Controls.Add(this.lblLoadedFile);
            this.tabReflector.Controls.Add(this.btnLoad);
            this.tabReflector.Controls.Add(this.btnReload);
            this.tabReflector.Controls.Add(this.rtbText);
            this.tabReflector.Controls.Add(this.cboLanguage);
            this.tabReflector.Controls.Add(this.lblLanguage);
            this.tabReflector.Location = new System.Drawing.Point(4, 22);
            this.tabReflector.Name = "tabReflector";
            this.tabReflector.Padding = new System.Windows.Forms.Padding(3);
            this.tabReflector.Size = new System.Drawing.Size(587, 386);
            this.tabReflector.TabIndex = 2;
            this.tabReflector.Text = "Reflector";
            this.tabReflector.UseVisualStyleBackColor = true;
            this.tabReflector.Enter += new System.EventHandler(this.tabReflector_Enter);
            this.tabReflector.Resize += new System.EventHandler(this.tabReflector_Resize);
            // 
            // cboOptimization
            // 
            this.cboOptimization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOptimization.FormattingEnabled = true;
            this.cboOptimization.Location = new System.Drawing.Point(247, 6);
            this.cboOptimization.Name = "cboOptimization";
            this.cboOptimization.Size = new System.Drawing.Size(62, 21);
            this.cboOptimization.TabIndex = 3;
            // 
            // lblOptimization
            // 
            this.lblOptimization.AutoSize = true;
            this.lblOptimization.Location = new System.Drawing.Point(175, 9);
            this.lblOptimization.Name = "lblOptimization";
            this.lblOptimization.Size = new System.Drawing.Size(66, 13);
            this.lblOptimization.TabIndex = 2;
            this.lblOptimization.Text = "Optimization";
            // 
            // lblLoadedFile
            // 
            this.lblLoadedFile.AutoSize = true;
            this.lblLoadedFile.Location = new System.Drawing.Point(466, 9);
            this.lblLoadedFile.Name = "lblLoadedFile";
            this.lblLoadedFile.Size = new System.Drawing.Size(10, 13);
            this.lblLoadedFile.TabIndex = 6;
            this.lblLoadedFile.Text = " ";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(391, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(65, 23);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(320, 5);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(65, 23);
            this.btnReload.TabIndex = 4;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // rtbText
            // 
            this.rtbText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbText.Location = new System.Drawing.Point(3, 33);
            this.rtbText.Name = "rtbText";
            this.rtbText.ReadOnly = true;
            this.rtbText.Size = new System.Drawing.Size(581, 371);
            this.rtbText.TabIndex = 7;
            this.rtbText.Text = "";
            this.rtbText.WordWrap = false;
            this.rtbText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbText_LinkClicked);
            this.rtbText.DoubleClick += new System.EventHandler(this.rtbText_DoubleClick);
            this.rtbText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbText_KeyDown);
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(66, 6);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(97, 21);
            this.cboLanguage.TabIndex = 1;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(6, 9);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(54, 13);
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Language";
            // 
            // tabILSpy
            // 
            this.tabILSpy.Controls.Add(this.lblILSpyLoadedFile);
            this.tabILSpy.Controls.Add(this.btnILSpyLoad);
            this.tabILSpy.Controls.Add(this.btnILSpyReload);
            this.tabILSpy.Controls.Add(this.rtbILSpyText);
            this.tabILSpy.Controls.Add(this.cboILSpyLanguage);
            this.tabILSpy.Controls.Add(this.label1);
            this.tabILSpy.Location = new System.Drawing.Point(4, 22);
            this.tabILSpy.Name = "tabILSpy";
            this.tabILSpy.Padding = new System.Windows.Forms.Padding(3);
            this.tabILSpy.Size = new System.Drawing.Size(587, 386);
            this.tabILSpy.TabIndex = 4;
            this.tabILSpy.Text = "ILSpy";
            this.tabILSpy.UseVisualStyleBackColor = true;
            this.tabILSpy.Enter += new System.EventHandler(this.tabILSpy_Enter);
            // 
            // lblILSpyLoadedFile
            // 
            this.lblILSpyLoadedFile.AutoSize = true;
            this.lblILSpyLoadedFile.Location = new System.Drawing.Point(322, 9);
            this.lblILSpyLoadedFile.Name = "lblILSpyLoadedFile";
            this.lblILSpyLoadedFile.Size = new System.Drawing.Size(10, 13);
            this.lblILSpyLoadedFile.TabIndex = 4;
            this.lblILSpyLoadedFile.Text = " ";
            // 
            // btnILSpyLoad
            // 
            this.btnILSpyLoad.Location = new System.Drawing.Point(251, 5);
            this.btnILSpyLoad.Name = "btnILSpyLoad";
            this.btnILSpyLoad.Size = new System.Drawing.Size(65, 22);
            this.btnILSpyLoad.TabIndex = 3;
            this.btnILSpyLoad.Text = "Load";
            this.btnILSpyLoad.UseVisualStyleBackColor = true;
            this.btnILSpyLoad.Click += new System.EventHandler(this.btnILSpyLoad_Click);
            // 
            // btnILSpyReload
            // 
            this.btnILSpyReload.Location = new System.Drawing.Point(180, 5);
            this.btnILSpyReload.Name = "btnILSpyReload";
            this.btnILSpyReload.Size = new System.Drawing.Size(65, 22);
            this.btnILSpyReload.TabIndex = 2;
            this.btnILSpyReload.Text = "Reload";
            this.btnILSpyReload.UseVisualStyleBackColor = true;
            this.btnILSpyReload.Click += new System.EventHandler(this.btnILSpyReload_Click);
            // 
            // rtbILSpyText
            // 
            this.rtbILSpyText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.rtbILSpyText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbILSpyText.Location = new System.Drawing.Point(3, 33);
            this.rtbILSpyText.Name = "rtbILSpyText";
            this.rtbILSpyText.ReadOnly = true;
            this.rtbILSpyText.Size = new System.Drawing.Size(581, 360);
            this.rtbILSpyText.TabIndex = 5;
            this.rtbILSpyText.Text = "";
            this.rtbILSpyText.WordWrap = false;
            this.rtbILSpyText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbILSpyText_LinkClicked);
            this.rtbILSpyText.DoubleClick += new System.EventHandler(this.rtbILSpyText_DoubleClick);
            this.rtbILSpyText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbILSpyText_KeyDown);
            // 
            // cboILSpyLanguage
            // 
            this.cboILSpyLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboILSpyLanguage.FormattingEnabled = true;
            this.cboILSpyLanguage.Location = new System.Drawing.Point(66, 6);
            this.cboILSpyLanguage.Name = "cboILSpyLanguage";
            this.cboILSpyLanguage.Size = new System.Drawing.Size(97, 21);
            this.cboILSpyLanguage.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Language";
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.splitter2);
            this.tabGeneral.Controls.Add(this.dgVariable);
            this.tabGeneral.Controls.Add(this.dgGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(587, 386);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // splitter2
            // 
            this.splitter2.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(3, 116);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(581, 3);
            this.splitter2.TabIndex = 0;
            this.splitter2.TabStop = false;
            // 
            // dgVariable
            // 
            this.dgVariable.AllowUserToAddRows = false;
            this.dgVariable.AllowUserToDeleteRows = false;
            this.dgVariable.AllowUserToResizeColumns = false;
            this.dgVariable.AllowUserToResizeRows = false;
            this.dgVariable.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgVariable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgVariable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcVarIndex,
            this.dgvcVarName,
            this.dgvcVarType,
            this.dgvcVarIsPinned});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgVariable.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgVariable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgVariable.Location = new System.Drawing.Point(3, 116);
            this.dgVariable.Name = "dgVariable";
            this.dgVariable.ReadOnly = true;
            this.dgVariable.RowHeadersVisible = false;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgVariable.RowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dgVariable.RowTemplate.Height = 16;
            this.dgVariable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgVariable.Size = new System.Drawing.Size(581, 267);
            this.dgVariable.TabIndex = 7;
            this.dgVariable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgVariable_CellDoubleClick);
            this.dgVariable.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgVariable_CellMouseDown);
            this.dgVariable.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgVariable_KeyUp);
            this.dgVariable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgVariable_MouseDown);
            // 
            // dgvcVarIndex
            // 
            this.dgvcVarIndex.ContextMenuStrip = this.cmVariable;
            this.dgvcVarIndex.DataPropertyName = "index";
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.dgvcVarIndex.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvcVarIndex.HeaderText = "Index";
            this.dgvcVarIndex.Name = "dgvcVarIndex";
            this.dgvcVarIndex.ReadOnly = true;
            this.dgvcVarIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvcVarIndex.Width = 60;
            // 
            // cmVariable
            // 
            this.cmVariable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmVarAppend,
            this.cmVarInsertBefore,
            this.cmVarInsertAfter,
            this.cmVarDuplicate,
            this.toolStripMenuItem5,
            this.cmVarEdit,
            this.cmVarRemove});
            this.cmVariable.Name = "mnuOp";
            this.cmVariable.Size = new System.Drawing.Size(141, 142);
            this.cmVariable.Opening += new System.ComponentModel.CancelEventHandler(this.cmVariable_Opening);
            // 
            // cmVarAppend
            // 
            this.cmVarAppend.Name = "cmVarAppend";
            this.cmVarAppend.Size = new System.Drawing.Size(140, 22);
            this.cmVarAppend.Text = "Append";
            this.cmVarAppend.Click += new System.EventHandler(this.cmVarAppend_Click);
            // 
            // cmVarInsertBefore
            // 
            this.cmVarInsertBefore.Name = "cmVarInsertBefore";
            this.cmVarInsertBefore.Size = new System.Drawing.Size(140, 22);
            this.cmVarInsertBefore.Text = "Insert Before";
            this.cmVarInsertBefore.Click += new System.EventHandler(this.cmVarInsertBefore_Click);
            // 
            // cmVarInsertAfter
            // 
            this.cmVarInsertAfter.Name = "cmVarInsertAfter";
            this.cmVarInsertAfter.Size = new System.Drawing.Size(140, 22);
            this.cmVarInsertAfter.Text = "Insert After";
            this.cmVarInsertAfter.Click += new System.EventHandler(this.cmVarInsertAfter_Click);
            // 
            // cmVarDuplicate
            // 
            this.cmVarDuplicate.Name = "cmVarDuplicate";
            this.cmVarDuplicate.Size = new System.Drawing.Size(140, 22);
            this.cmVarDuplicate.Text = "Duplicate";
            this.cmVarDuplicate.Click += new System.EventHandler(this.cmVarDuplicate_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(137, 6);
            // 
            // cmVarEdit
            // 
            this.cmVarEdit.Name = "cmVarEdit";
            this.cmVarEdit.Size = new System.Drawing.Size(140, 22);
            this.cmVarEdit.Text = "Edit";
            this.cmVarEdit.Click += new System.EventHandler(this.cmVarEdit_Click);
            // 
            // cmVarRemove
            // 
            this.cmVarRemove.Name = "cmVarRemove";
            this.cmVarRemove.Size = new System.Drawing.Size(140, 22);
            this.cmVarRemove.Text = "Remove";
            this.cmVarRemove.Click += new System.EventHandler(this.cmVarRemove_Click);
            // 
            // dgvcVarName
            // 
            this.dgvcVarName.ContextMenuStrip = this.cmVariable;
            this.dgvcVarName.DataPropertyName = "name";
            this.dgvcVarName.HeaderText = "Variable Name";
            this.dgvcVarName.Name = "dgvcVarName";
            this.dgvcVarName.ReadOnly = true;
            this.dgvcVarName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvcVarName.Width = 200;
            // 
            // dgvcVarType
            // 
            this.dgvcVarType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcVarType.ContextMenuStrip = this.cmVariable;
            this.dgvcVarType.DataPropertyName = "type";
            this.dgvcVarType.HeaderText = "Variable Type";
            this.dgvcVarType.MinimumWidth = 400;
            this.dgvcVarType.Name = "dgvcVarType";
            this.dgvcVarType.ReadOnly = true;
            this.dgvcVarType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvcVarIsPinned
            // 
            this.dgvcVarIsPinned.ContextMenuStrip = this.cmVariable;
            this.dgvcVarIsPinned.DataPropertyName = "ispinned";
            this.dgvcVarIsPinned.HeaderText = "Is Pinned";
            this.dgvcVarIsPinned.Name = "dgvcVarIsPinned";
            this.dgvcVarIsPinned.ReadOnly = true;
            this.dgvcVarIsPinned.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcVarIsPinned.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvcVarIsPinned.Width = 60;
            // 
            // dgGeneral
            // 
            this.dgGeneral.AllowUserToAddRows = false;
            this.dgGeneral.AllowUserToDeleteRows = false;
            this.dgGeneral.AllowUserToResizeColumns = false;
            this.dgGeneral.AllowUserToResizeRows = false;
            this.dgGeneral.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgGeneral.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgGeneral.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgGeneral.ColumnHeadersVisible = false;
            this.dgGeneral.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgGeneral.DefaultCellStyle = dataGridViewCellStyle13;
            this.dgGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgGeneral.Location = new System.Drawing.Point(3, 3);
            this.dgGeneral.Name = "dgGeneral";
            this.dgGeneral.ReadOnly = true;
            this.dgGeneral.RowHeadersVisible = false;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgGeneral.RowsDefaultCellStyle = dataGridViewCellStyle14;
            this.dgGeneral.RowTemplate.Height = 16;
            this.dgGeneral.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgGeneral.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgGeneral.Size = new System.Drawing.Size(581, 113);
            this.dgGeneral.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "content";
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn1.HeaderText = "Content";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.btnSaveLog);
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(587, 386);
            this.tabLog.TabIndex = 3;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Location = new System.Drawing.Point(6, 6);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(65, 23);
            this.btnSaveLog.TabIndex = 0;
            this.btnSaveLog.Text = "Save";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Location = new System.Drawing.Point(3, 11);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(581, 372);
            this.txtLog.TabIndex = 1;
            this.txtLog.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusInfo,
            this.statusCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(870, 24);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusInfo
            // 
            this.statusInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusInfo.Name = "statusInfo";
            this.statusInfo.Size = new System.Drawing.Size(725, 19);
            this.statusInfo.Spring = true;
            this.statusInfo.Text = "Ready";
            this.statusInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusCount
            // 
            this.statusCount.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusCount.Name = "statusCount";
            this.statusCount.Size = new System.Drawing.Size(130, 19);
            this.statusCount.Text = "Deobfuscator Count: 0";
            this.statusCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmClassEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(870, 461);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.KeyPreview = true;
            this.Name = "frmClassEdit";
            this.ShowIcon = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Class Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmClassEdit_FormClosing);
            this.Shown += new System.EventHandler(this.frmClassEdit_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmClassEdit_KeyUp);
            this.cmAssembly.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBody)).EndInit();
            this.cmOp.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResource)).EndInit();
            this.cmResource.ResumeLayout(false);
            this.panelResource.ResumeLayout(false);
            this.panelResource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResource)).EndInit();
            this.tabReflector.ResumeLayout(false);
            this.tabReflector.PerformLayout();
            this.tabILSpy.ResumeLayout(false);
            this.tabILSpy.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgVariable)).EndInit();
            this.cmVariable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgGeneral)).EndInit();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbSave;
        private System.Windows.Forms.ToolStripButton tbSelect;
        private System.Windows.Forms.DataGridView dgBody;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabDetails;
        private System.Windows.Forms.DataGridView dgGeneral;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbBack;
        private System.Windows.Forms.ToolStripButton tbForward;
        private System.Windows.Forms.ContextMenuStrip cmOp;
        private System.Windows.Forms.ToolStripMenuItem cmNop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cmDeobf;
        private System.Windows.Forms.ToolStripMenuItem cmEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel lblSearch;
        private System.Windows.Forms.ToolStripComboBox cboSearch;
        private System.Windows.Forms.ToolStripMenuItem cmInsertBefore;
        private System.Windows.Forms.ToolStripMenuItem cmInsertAfter;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cmRemove;
        private System.Windows.Forms.ToolStripMenuItem cmDuplicate;
        private System.Windows.Forms.ToolStripMenuItem cmMarkBlocks;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripButton tbSearchNext;
        private System.Windows.Forms.ContextMenuStrip cmAssembly;
        private System.Windows.Forms.ToolStripMenuItem cmRename;
        private System.Windows.Forms.ToolStripMenuItem cmCustomAttributes;
        private System.Windows.Forms.TabPage tabReflector;
        private System.Windows.Forms.RichTextBox rtbText;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ToolStripMenuItem cmViewInReflector;
        private System.Windows.Forms.ToolStripMenuItem cmCopy;
        private System.Windows.Forms.ToolStripMenuItem cmPaste;
        private System.Windows.Forms.ToolStripComboBox cboSearchType;
        private System.Windows.Forms.Label lblLoadedFile;
        private System.Windows.Forms.ToolStripMenuItem cmSaveDetailsAs;
        private System.Windows.Forms.ToolStripMenuItem cmDeobfBranch;
        private System.Windows.Forms.ToolStripMenuItem cmMove;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.ComboBox cboOptimization;
        private System.Windows.Forms.Label lblOptimization;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusInfo;
        private System.Windows.Forms.ToolStripStatusLabel statusCount;
        private System.Windows.Forms.ToolStripMenuItem cmGotoEntryPoint;
        private System.Windows.Forms.ToolStripMenuItem cmHighlight;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcOperand;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcOperandType;
        private System.Windows.Forms.DataGridView dgVariable;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ContextMenuStrip cmVariable;
        private System.Windows.Forms.ToolStripMenuItem cmVarEdit;
        private System.Windows.Forms.ToolStripMenuItem cmVarInsertBefore;
        private System.Windows.Forms.ToolStripMenuItem cmVarInsertAfter;
        private System.Windows.Forms.ToolStripMenuItem cmVarDuplicate;
        private System.Windows.Forms.ToolStripMenuItem cmVarRemove;
        private System.Windows.Forms.ToolStripMenuItem cmVarAppend;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem cmBookmark;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cboBookmark;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripButton tbSaveBookmark;
        private System.Windows.Forms.DataGridView dgResource;
        private System.Windows.Forms.TextBox txtResource;
        private System.Windows.Forms.PictureBox pbResource;
        private Be.Windows.Forms.HexBox hbResource;
        private System.Windows.Forms.ToolStripMenuItem cmSaveResourceAs;
        private System.Windows.Forms.ContextMenuStrip cmResource;
        private System.Windows.Forms.ToolStripMenuItem cmResourceSaveAs;
        private System.Windows.Forms.Panel panelResource;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.ListView lvResource;
        private System.Windows.Forms.ToolStripMenuItem cmMakeBranch;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cmViewResourceAsBinary;
        private System.Windows.Forms.ToolStripMenuItem cmResourceViewAsBinary;
        private System.Windows.Forms.ToolStripMenuItem cmResourceViewAsNormal;
        private System.Windows.Forms.ToolStripMenuItem cmViewResourceAsNormal;
        private System.Windows.Forms.ToolStripMenuItem cmCopyName;
        private System.Windows.Forms.ToolStripMenuItem cmExpandAll;
        private System.Windows.Forms.ToolStripMenuItem cmCollapseAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripDropDownButton ddbOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenFile;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenGAC;
        private System.Windows.Forms.ToolStripMenuItem cmExpand;
        private System.Windows.Forms.ToolStripMenuItem cmCollapse;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcVarIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcVarName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcVarType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcVarIsPinned;
        private System.Windows.Forms.TabPage tabILSpy;
        private System.Windows.Forms.Button btnILSpyLoad;
        private System.Windows.Forms.Button btnILSpyReload;
        private System.Windows.Forms.RichTextBox rtbILSpyText;
        private System.Windows.Forms.ComboBox cboILSpyLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblILSpyLoadedFile;
        private System.Windows.Forms.ToolStripMenuItem cmNewExceptionHandler;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem cmReadMethodFromFile;
        private System.Windows.Forms.ToolStripMenuItem cmRestoreMethodFromImage;
        private System.Windows.Forms.ToolStripMenuItem cmReadMethodsFromFolder;
        private System.Windows.Forms.ToolStripMenuItem cmReadMethodFromAssembly;
        private System.Windows.Forms.ToolStripMenuItem cmImportMethodFromAssembly;
        private System.Windows.Forms.ToolStripMenuItem cmWriteMethodToFile;
        private System.Windows.Forms.ToolStripMenuItem cmCopyNameAsHex;
        private System.Windows.Forms.ToolStripMenuItem cmImportResourceFromFile;
        private System.Windows.Forms.ToolStripMenuItem cmRemoveResource;
        private System.Windows.Forms.ToolStripMenuItem cmResourceImportFromFile;
        private System.Windows.Forms.ToolStripMenuItem cmResourceRemove;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcResourceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcResourceName;
        private DataGridViewTextAndImageColumn dgvcResourceValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcValueType;
        private System.Windows.Forms.ToolStripMenuItem cmResourceBamlTranslator;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem cmResourceReflectorBamlViewer;
        private System.Windows.Forms.ToolStripMenuItem cmResourceILSpyBamlDecompiler;

    }
}
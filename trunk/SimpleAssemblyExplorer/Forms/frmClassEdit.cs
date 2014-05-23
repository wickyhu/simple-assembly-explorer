using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;

namespace SimpleAssemblyExplorer
{
    public partial class frmClassEdit : frmBase
    {
        public IHost Host { get; private set; }
        public string[] Rows { get; private set; }
        public string SourceDir { get; private set; }
        public bool ShowStaticOnly { get; private set; }
        public bool ShowSelectButton { get; private set; }

        //public string Text { get { return this.Text; } set { this.Text = value; } }
        public ToolStripComboBox BookmarkComboBox { get { return this.cboBookmark; } }
        public TreeView TreeView { get { return this.treeView1; } }
        public TextBox LogText { get { return txtLog; } }
        public ToolStripComboBox SearchTypeComboBox { get { return cboSearchType; } }
        public ToolStripComboBox SearchTextComboBox { get { return cboSearch; } }
        public ComboBox LanguageComboBox { get { return cboLanguage; } }
        public ComboBox OptimizationComboBox { get { return cboOptimization; } }
        public ComboBox ILSpyLanguageComboBox { get { return cboILSpyLanguage; } }
        public TabControl TabControl { get { return tabControl1; } }
        public TabPage DetailsTabPage { get { return tabDetails; } }
        public TabPage ReflectorTabPage { get { return tabReflector; } }
        public TabPage ILSpyTabPage { get { return tabILSpy; } }
        public RichTextBox ReflectorTextBox { get { return rtbText; } }
        public RichTextBox ILSpyTextBox { get { return rtbILSpyText; } }        

        public DataGridView BodyDataGrid { get { return dgBody; } }
        public DataGridView GeneralDataGrid { get { return dgGeneral; } }
        public DataGridView VariableDataGrid { get { return dgVariable; } }
        public DataGridView ResourceDataGrid { get { return dgResource; } }
        public ContextMenuStrip VariableContextMenuStrip { get { return cmVariable; } }
        public ContextMenuStrip BodyContextMenuStrip { get { return cmOp; } }
        public ContextMenuStrip ResourceContextMenuStrip { get { return cmResource; } }
        public ContextMenuStrip TreeViewContextMenuStrip { get { return cmAssembly; } }
        public ToolStripButton SaveAssemblyButton { get { return tbSave; } }
        
        public Label ReflectorLoadedFile { get { return lblLoadedFile; } }
        public Label ILSpyLoadedFile { get { return lblILSpyLoadedFile; } }        

        public TextBox ResourceText { get { return txtResource; } }
        public PictureBox ResourceImage { get { return pbResource; } }
        public Be.Windows.Forms.HexBox ResourceBinary { get { return hbResource; } }
        public ListView ResourceListView { get { return lvResource; } }
        public Panel ResourcePanel { get { return panelResource; } }

        public ClassEditBookmarkHandler BookmarkHandler { get; private set; }
        public ClassEditTreeViewHandler TreeViewHandler { get; private set; }
        public ClassEditLogHandler LogHandler { get; private set; }
        public ClassEditBodyGridHandler BodyGridHandler { get; private set; }
        public ClassEditVariableGridHandler VariableGridHandler { get; private set; }
        public ClassEditResourceHandler ResourceHandler { get; private set; }
        public ClassEditSearchHandler SearchHandler { get; private set; }
        public ClassEditReflectorHandler ReflectorHandler { get; private set; }
        public ClassEditILSpyHandler ILSpyHandler { get; private set; }
        public ClassEditBinaryViewHandler BinaryViewHandler { get; private set; }
        public ClassEditTextViewHandler TextViewHandler { get; private set; }

        //used in frmClassEditInstruction only
        public List<MethodReference> SelectedMethodHistory = new List<MethodReference>();

        private void InitForm(ClassEditParams p)
        {
            try
            {
                //this.SuspendLayout();

                Host = p.Host;
                Rows = p.Rows;
                SourceDir = SimplePath.GetFullPath(p.SourceDir);
                ShowStaticOnly = p.ShowStaticOnly;
                ShowSelectButton = p.ShowSelectButton;

                tbSelect.Visible = ShowSelectButton;
                tbSave.Visible = !ShowSelectButton;

                //dgBody.Visible = false;
                //dgResource.Visible = false;
                //panelResource.Visible = false;

                dgBody.Dock = DockStyle.Fill;
                dgResource.Dock = DockStyle.Fill;

                panelResource.Dock = DockStyle.Fill;
                txtResource.Dock = DockStyle.Fill;
                //pbResource.Dock = DockStyle.Fill;
                pbResource.Left = 0; pbResource.Top = 0;
                lvResource.Dock = DockStyle.Fill;
                hbResource.Dock = DockStyle.Fill;                

                rtbText.Font = Config.ClassEditorRichTextBoxFont;
                rtbILSpyText.Font = Config.ClassEditorRichTextBoxFont;

                LogHandler = new ClassEditLogHandler(this);
                BodyGridHandler = new ClassEditBodyGridHandler(this);
                VariableGridHandler = new ClassEditVariableGridHandler(this);
                ResourceHandler = new ClassEditResourceHandler(this);
                SearchHandler = new ClassEditSearchHandler(this);
                ReflectorHandler = new ClassEditReflectorHandler(this);
                ILSpyHandler = new ClassEditILSpyHandler(this);
                BookmarkHandler = new ClassEditBookmarkHandler(this);
                TreeViewHandler = new ClassEditTreeViewHandler(this);
                BinaryViewHandler = new ClassEditBinaryViewHandler(this);
                TextViewHandler = new ClassEditTextViewHandler(this);

                TreeViewHandler.ObjectType = p.ObjectType;
            }
            catch
            {
                throw;
            }
            finally
            {
                //this.ResumeLayout();
            }
        }

        public frmClassEdit(ClassEditParams p)
        {
            InitializeComponent();

            InitForm(p);            

        }

        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);
        //    this.WindowState = FormWindowState.Maximized;
        //}

        //private void tbClose_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        MethodDefinition _selectedMethod;
        string _selectedPath;
        public MethodDefinition SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
        }
        public string SelectedPath
        {
            get
            {
                return _selectedPath;
            }
        }

        private void GetSelectedMethod() 
        {
            _selectedMethod = TreeViewHandler.SelectedMethod;
            _selectedPath = null;
            if (_selectedMethod != null)
            {
                string path = null;
                TreeNode node = TreeViewHandler.SelectedNode;
                path = TreeViewHandler.GetTreeNodeText(node);
                int i = path.IndexOf("(");
                if (i > 0) path = path.Substring(0, i);

                while (node.Level > 2)
                {
                    node = node.Parent;
                    path = String.Format("{0}.{1}", TreeViewHandler.GetTreeNodeText(node), path);
                }
                if (path.StartsWith(".")) path = path.Substring(1);
                _selectedPath = path;
            }
        }

        private void tbSelect_Click(object sender, EventArgs e)
        {
            GetSelectedMethod();
            this.Close();
        }

        string _lastSaveDir = null;
        string _lastSaveFileName = null;
        string _lastOpenDir = null;
        private void SaveAssembly()
        {
            AssemblyDefinition ad = TreeViewHandler.GetCurrentAssembly();
            if (ad == null)
            {
                SimpleMessage.ShowInfo("Cannot determine current assembly.");
                return;
            }

            string initDir = null;
            if (!String.IsNullOrEmpty(_lastSaveDir))
            {
                initDir = _lastSaveDir;
            }
            else
            {
                initDir = SourceDir;
            }

            string initFileName = _lastSaveFileName;
            string origFileName = Path.GetFileName(ad.MainModule.FullyQualifiedName);
            if (String.IsNullOrEmpty(initFileName) ||
                !Path.GetFileNameWithoutExtension(initFileName).StartsWith(Path.GetFileNameWithoutExtension(origFileName)))
            {
                initFileName = origFileName;
            }

            string file = SimpleDialog.OpenFile("Save As", Consts.FilterAssemblyFile, Path.GetExtension(initFileName), false, initDir, initFileName);
            if (!String.IsNullOrEmpty(file))
            {
                if (File.Exists(file)) File.Delete(file);

                string adPath = Path.GetDirectoryName(ad.MainModule.FullyQualifiedName);
                bool resolveDirAdded = Host.AddAssemblyResolveDir(adPath);

                try
                {
                    ad.Write(file);
                }
                finally
                {
                    if (resolveDirAdded)
                    {
                        Host.RemoveAssemblyResolveDir(adPath);
                    }
                }

                _lastSaveDir = Path.GetDirectoryName(file);
                _lastSaveFileName = Path.GetFileName(file);
                //Config.LastSaveDir = _lastSaveDir;

                SimpleMessage.ShowInfo(String.Format("Assembly saved to {0}", file));

                if (file.Equals(initFileName, StringComparison.OrdinalIgnoreCase))
                {
                    TreeViewHandler.ClearRenamedObjects();
                }
            }
        }
        private void tbSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (new SimpleWaitCursor())
                {
                    SaveAssembly();
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void tbBack_Click(object sender, EventArgs e)
        {
            TreeViewHandler.NavigateBack();
        }

        private void tbForward_Click(object sender, EventArgs e)
        {
            TreeViewHandler.NavigateForward();
        }      

        private void frmClassEdit_KeyUp(object sender, KeyEventArgs e)
        {
            //Deobfuscator: CTRL+D
            //if (e.Control && e.KeyCode == Keys.D && tabControl1.SelectedTab == tabDetails)
            //{
            //    cmDeobf_Click(sender, e);
            //    e.Handled = true;
            //    return;
            //}

            //Search Next: F3
            if (e.KeyCode == Keys.F3)
            {
                tbSearchNext_Click(sender, e);
                e.Handled = true;
                return;
            }

            //Bookmark: CTRL+F4, F4, SHIFT+F4
            if (e.KeyCode == Keys.F4)
            {
                if (e.Control)
                {
                    cmBookmark_Click(sender, e);
                    return;
                }
                if (e.Shift)
                {
                    BookmarkHandler.NavigatePrevious();
                    e.Handled = true;
                    return;
                }
                else
                {
                    BookmarkHandler.NavigateNext();
                    e.Handled = true;
                    return;
                }
            }

        }       

        private void ResizeControls()
        {
            rtbText.Height = tabReflector.Height - cboLanguage.Top - cboLanguage.Height - rtbText.Margin.Top - rtbText.Margin.Bottom;
            rtbText.Width = tabReflector.Width - rtbText.Margin.Left - rtbText.Margin.Right;

            rtbILSpyText.Height = tabILSpy.Height - cboILSpyLanguage.Top - cboILSpyLanguage.Height - rtbILSpyText.Margin.Top - rtbILSpyText.Margin.Bottom;
            rtbILSpyText.Width = tabILSpy.Width - rtbILSpyText.Margin.Left - rtbILSpyText.Margin.Right;

            txtLog.Height = tabReflector.Height - btnSaveLog.Top - btnSaveLog.Height - txtLog.Margin.Top - txtLog.Margin.Bottom;
            txtLog.Width = tabReflector.Width - txtLog.Margin.Left - txtLog.Margin.Right;
        }

        private void frmClassEdit_Shown(object sender, EventArgs e)
        {
            ResizeControls();
            if (this.Rows.Length == 0)
            {
                mnuOpenFile_Click(this, null);
            }
        }

        private void tabReflector_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void frmClassEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ReflectorHandler.Initializing)
            {
                //SetStatusText("Reflector is initialing, please try again later.");
                e.Cancel = true;
                return;
            }
            if (ReflectorHandler.Unloading)
            {
                //SetStatusText("Reflector is unloading, please try again later.");
                e.Cancel = true;
                return;
            }

            try
            {
                this.treeView1.SuspendLayout();
                this.treeView1.BeginUpdate();
                this.treeView1.DrawMode = TreeViewDrawMode.Normal;

                if (Config.ClassEditorAutoSaveBookmarkEnabled)
                {
                    BookmarkHandler.SaveBookmark();
                }

                ReflectorHandler.Unload();

            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            using (new SimpleWaitCursor())
            {
                ReflectorHandler.UnloadAssemblies();
                ReflectorHandler.LoadAssemblies();                
            }
        }

        private void btnILSpyReload_Click(object sender, EventArgs e)
        {
            using (new SimpleWaitCursor())
            {
                ILSpyHandler.UnloadAssemblies();
                ILSpyHandler.LoadAssemblies();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            ReflectorHandler.btnLoad_Click(sender, e);
        }

        private void btnILSpyLoad_Click(object sender, EventArgs e)
        {
            ILSpyHandler.btnLoad_Click(sender, e);
        }

        private void cmViewInReflector_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmViewInReflector_Click(sender, e);
        }

        public void SetStatusText(string info)
        {
            if (String.IsNullOrEmpty(info)) info = "Ready.";
            statusInfo.Text = info.Replace('\r', ' ').Replace('\n', ' ');
        }

        public void SetStatusCount(int deobfCount)
        {
            statusCount.Text = String.Format("Deobfuscator Count: {0}", deobfCount);
        }

        private void cmOp_Opening(object sender, CancelEventArgs e)
        {
            BodyGridHandler.cmOp_Opening(sender, e);
        }

        private void cmNop_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmNop_Click(sender, e);
        }

        private void cmDeobfBranch_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmDeobfBranch_Click(sender, e);
        }

        private void cmMakeBranch_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmMakeBranch_Click(sender, e);
        }     

        private void dgBody_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            BodyGridHandler.dgBody_CellFormatting(sender, e);
        }

        private void dgBody_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            BodyGridHandler.dgBody_CellToolTipTextNeeded(sender, e);
        }

        private void cmDeobf_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmDeobf_Click(sender, e);
        }       

        private void cmEdit_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmEdit_Click(sender, e);
        }

        private void cmInsertAfter_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmInsertAfter_Click(sender, e);
        }

        private void cmInsertBefore_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmInsertBefore_Click(sender, e);
        }

        private void cmRemove_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmRemove_Click(sender, e);
        }

        private void cmNewExceptionHandler_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmNewExceptionHandler_Click(sender, e);
        }    

        private void cmDuplicate_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmDuplicate_Click(sender, e);
        }

        private void cmAssembly_Opening(object sender, CancelEventArgs e)
        {
            TreeViewHandler.cmAssembly_Opening(sender, e);
        }

        private void cmResource_Opening(object sender, CancelEventArgs e)
        {
            ResourceHandler.cmResource_Opening(sender, e);
        }

        private void cmExpand_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmExpand_Click(sender, e);
        }

        private void cmCollapse_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmCollapse_Click(sender, e);
        }

        private void cmExpandAll_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmExpandAll_Click(sender, e);
        }

        private void cmCollapseAll_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmCollapseAll_Click(sender, e);
        }

        private void cmBookmark_Click(object sender, EventArgs e)
        {
            BookmarkHandler.cmBookmark_Click(sender, e);
        }

        private void cmRename_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmRename_Click(sender, e);
        }

        private void cmCopyName_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmCopyName_Click(sender, e);
        }

        private void cmCustomAttributes_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmCustomAttributes_Click(sender, e);
        }
        
        private void cmGotoEntryPoint_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmGotoEntryPoint_Click(sender, e);
        }

        private void cmMove_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmMove_Click(sender, e);
        }

       
        private void cmHighlightOpCode_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmHighlightOpCode_Click(sender, e);
        }

        private void cmHighlightReference_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmHighlightReference_Click(sender, e);
        }

        private void cmSaveDetailsAs_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmSaveDetailsAs_Click(sender, e);
        }

        private void cmCopy_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmCopy_Click(sender, e);
        }

        private void cmPaste_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmPaste_Click(sender, e);
        }

        private void cmVarEdit_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarEdit_Click(sender, e);
        }

        private void cmVarInsertBefore_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarInsertBefore_Click(sender, e);
        }

        private void cmVarInsertAfter_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarInsertAfter_Click(sender, e);
        }

        private void cmVarDuplicate_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarDuplicate_Click(sender, e);
        }

        private void cmVarRemove_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarRemove_Click(sender, e);
        }

        private void cmVarAppend_Click(object sender, EventArgs e)
        {
            VariableGridHandler.cmVarAppend_Click(sender, e);
        }

        private void cmVariable_Opening(object sender, CancelEventArgs e)
        {
            VariableGridHandler.cmVariable_Opening(sender, e);
        }

       
        private void cmSaveResourceAs_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmSaveResourceAs_Click(sender, e);
        }

        private void cmViewResourceAsBinary_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmViewResourceAsBinary_Click(sender, e);
        }

        private void cmViewResourceAsNormal_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmViewResourceAsNormal_Click(sender, e);
        }

        private void cmImportResourceFromFile_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmImportResourceFromFile_Click(sender, e);
        }

        private void cmRemoveResource_Click(object sender, EventArgs e)
        {
            TreeViewHandler.cmRemoveResource_Click(sender, e);
        }

        private void cmResourceSaveAs_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceSaveAs_Click(sender, e);
        }

        private void cmResourceViewAsBinary_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceViewAsBinary_Click(sender, e);
        }

        private void cmResourceViewAsNormal_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceViewAsNormal_Click(sender, e);
        }

        private void cmResourceImportFromFile_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceImportFromFile_Click(sender, e);
        }

        private void cmResourceRemove_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceRemove_Click(sender, e);
        }

        private void cmResourceReflectorBamlViewer_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceReflectorBamlViewer_Click(sender, e);
        }

        private void cmResourceILSpyBamlDecompiler_Click(object sender, EventArgs e)
        {
            ResourceHandler.cmResourceILSpyBamlDecompiler_Click(sender, e);
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(Brushes.DodgerBlue, e.Node.Bounds);
                //ControlPaint.DrawFocusRectangle(e.Graphics, e.Node.Bounds, Color.White, Color.DodgerBlue);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, e.Node.Bounds, Color.White, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void dgVariable_KeyUp(object sender, KeyEventArgs e)
        {
            VariableGridHandler.dgVariable_KeyUp(sender, e);
        }

        private void dgVariable_MouseDown(object sender, MouseEventArgs e)
        {
            VariableGridHandler.dgVariable_MouseDown(sender, e);
        }

        private void dgVariable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            VariableGridHandler.dgVariable_CellDoubleClick(sender, e);
        }

        private void dgVariable_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            VariableGridHandler.dgVariable_CellMouseDown(sender, e);
        }

        private void dgBody_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            BodyGridHandler.dgBody_CellMouseDoubleClick(sender, e);
        }

        private void dgBody_KeyUp(object sender, KeyEventArgs e)
        {
            BodyGridHandler.dgBody_KeyUp(sender, e);
        }

        private void dgBody_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            BodyGridHandler.dgBody_CellMouseDown(sender, e);
        }

        private void cmMarkBlocks_Click(object sender, EventArgs e)
        {
            BodyGridHandler.cmMarkBlocks_Click(sender, e);
        }

        private void dgBody_MouseMove(object sender, MouseEventArgs e)
        {
            BodyGridHandler.dgBody_MouseMove(sender, e);
        }

        private void dgBody_MouseDown(object sender, MouseEventArgs e)
        {
            BodyGridHandler.dgBody_MouseDown(sender, e);
        }

         private void dgBody_DragOver(object sender, DragEventArgs e)
        {
            BodyGridHandler.dgBody_DragOver(sender, e);
        }

         private void dgBody_DragDrop(object sender, DragEventArgs e)
         {
             BodyGridHandler.dgBody_DragDrop(sender, e);
         }

         private void btnSaveLog_Click(object sender, EventArgs e)
         {
             LogHandler.btnSaveLog_Click(sender, e);
         }

         private void cboSearch_KeyUp(object sender, KeyEventArgs e)
         {
             SearchHandler.cboSearch_KeyUp(sender, e);
         }

        private void tbSearchNext_Click(object sender, EventArgs e)
        {
            SearchHandler.tbSearchNext_Click(sender,e);
        }

        private void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchHandler == null) return;
            SearchHandler.cboSearchType_SelectedIndexChanged(sender,e);
        }

        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReflectorHandler == null) return;
            ReflectorHandler.cboLanguage_SelectedIndexChanged(sender, e);
        }

        private void cboOptimization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReflectorHandler == null) return;
            ReflectorHandler.cboOptimization_SelectedIndexChanged(sender, e);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeViewHandler.treeView1_AfterSelect(sender, e);
        }      

        private void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            TreeViewHandler.treeView1_KeyUp(sender, e);
        }

        private void cboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchHandler == null) return;
            SearchHandler.cboSearch_SelectedIndexChanged(sender, e);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewHandler.treeView1_NodeMouseClick(sender, e);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            TreeViewHandler.treeView1_DoubleClick(sender, e);
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeViewHandler.treeView1_BeforeLabelEdit(sender, e);
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeViewHandler.treeView1_AfterLabelEdit(sender, e);
        }

        private void cboBookmark_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BookmarkHandler == null) return;
            BookmarkHandler.cboBookmark_SelectedIndexChanged(sender, e);
        }

        private void tbSaveBookmark_Click(object sender, EventArgs e)
        {
            BookmarkHandler.tbSaveBookmark_Click(sender, e);
        }

        private void dgResource_KeyUp(object sender, KeyEventArgs e)
        {
            ResourceHandler.dgResource_KeyUp(sender, e);
        }

        private void dgResource_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            ResourceHandler.dgResource_CellMouseDown(sender, e);
        }

        private void dgResource_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            ResourceHandler.dgResource_RowEnter(sender, e);
        }

        private void tabReflector_Enter(object sender, EventArgs e)
        {
            ReflectorHandler.tabPage_Enter(sender, e);
        }

        private void tabILSpy_Enter(object sender, EventArgs e)
        {
            ILSpyHandler.tabPage_Enter(sender, e);
        }      

        private void panelResource_MouseClick(object sender, MouseEventArgs e)
        {
            panelResource.Focus();
        }

        private void rtbText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            SearchHandler.rtbText_LinkClicked(sender, e);
        }

        private void rtbILSpyText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            SearchHandler.rtbText_LinkClicked(sender, e);
        }

        private void OpenFile(string newFile)
        {
            foreach (string file in this.Rows)
            {
                if (file.Equals(newFile, StringComparison.OrdinalIgnoreCase))
                    return;
            }
            
            string[] tmp = this.Rows;
            this.Rows = new string[this.Rows.Length + 1];
            Array.Copy(tmp, this.Rows, tmp.Length);
            this.Rows[this.Rows.Length - 1] = newFile;
            
            //mh...sequence is important here
            if(this.ReflectorHandler.IsReady)
                this.ReflectorHandler.InitAssemblyPath();
            if (this.ILSpyHandler.IsReady)
                this.ILSpyHandler.InitAssemblyPath();


            TreeNode node = this.TreeViewHandler.AddFileNode(newFile);
            if (node != null)
            {
                this.TreeViewHandler.ExpandFileNode(node);
                this.TreeViewHandler.SelectedNode = node;
            }
        }

        private void mnuOpenFile_Click(object sender, EventArgs e)
        {
            string initDir = null;
            if (!String.IsNullOrEmpty(_lastOpenDir))
            {
                initDir = _lastOpenDir;
            }
            else
            {
                initDir = SourceDir;
            }
            string file = SimpleDialog.OpenFile("Open File", Consts.FilterAssemblyFile, "", false, initDir);
            if (File.Exists(file))
            {
                _lastOpenDir = Path.GetDirectoryName(file);
                OpenFile(file);
            }            
        }

        private void mnuOpenGAC_Click(object sender, EventArgs e)
        {
            frmClassEditOpenGAC f = new frmClassEditOpenGAC();
            if (f.ShowDialog() == DialogResult.OK)
            {                
                foreach (string fullName in f.SelectedAssemblies)
                {
                    AssemblyNameReference anr = AssemblyNameReference.Parse(fullName);
                    AssemblyDefinition ad = GlobalAssemblyResolver.Instance.Resolve(anr);
                    string file = ad.MainModule.FullyQualifiedName;
                    OpenFile(file);
                }
            }
        }

        private void rtbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                new RichTextFormatterHelper(rtbText).CopySelectedTextToClipboard();
                e.Handled = true;
            }
        }

        private void rtbILSpyText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                new RichTextFormatterHelper(rtbILSpyText).CopySelectedTextToClipboard();
                e.Handled = true;
            }
        }

        private void cmReadMethodFromFile_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmReadMethodFromFile_Click(sender, e);
        }

        private void cmRestoreMethodFromImage_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmRestoreMethodFromImage_Click(sender, e);
        }

        private void cmReadMethodsFromFolder_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmReadMethodsFromFolder_Click(sender, e);
        }

        private void cmReadMethodFromAssembly_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmReadMethodFromAssembly_Click(sender, e);
        }

        private void cmImportMethodFromAssembly_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmImportMethodFromAssembly_Click(sender, e);
        }

        private void cmWriteMethodToFile_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmWriteMethodToFile_Click(sender, e);
        }

        private void cmCopyNameAsHex_Click(object sender, EventArgs e)
        {
            this.TreeViewHandler.cmCopyNameAsHex_Click(sender, e);
        }

        private void rtbText_DoubleClick(object sender, EventArgs e)
        {
            this.ReflectorHandler.rtbText_Hightlight(sender, e);
        }

        private void rtbILSpyText_DoubleClick(object sender, EventArgs e)
        {
            this.ILSpyHandler.rtbText_Hightlight(sender, e);
        }
      

    } // end of class

}
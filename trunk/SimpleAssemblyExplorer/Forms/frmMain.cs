using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using Mono.Cecil;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using DgvFilterPopup;

namespace SimpleAssemblyExplorer
{

    public partial class frmMain : frmBase, IHost
    {
        public TreeView TreeView { get { return this.treeView1; } }
        public DataGridView AssemblyDataGrid { get { return this.dgvData; } }
        public BindingSource AssemblyDataGridBindingSource { get { return this.bindingSource1; } }        
        
        public ToolStripStatusLabel StatusVersion { get { return this.statusVersion; } }
        public ToolStripStatusLabel StatusInfo { get { return this.statusInfo; } }
        public ToolStripProgressBar StatusProgressBar { get { return this.statusProgress; } }                
        public ToolStripTextBox PathTextBox { get { return txtPath; } }
        public ToolStripMenuItem ToolMenu { get { return this.mnuTools; } }
        public ContextMenuStrip AssemblyContextMenuStrip { get { return this.cmAssembly; } }
        public ContextMenuStrip FolderContextMenuStrip { get { return this.cmFolder; } }
        public ToolStripMenuItem AssemblyMoreMenu { get { return this.cmMore; } }        
        
        public MainHostHandler HostHandler { get; private set; }
        public MainPluginHandler PluginHandler { get; private set; }
        public MainToolHandler ToolHandler { get; private set; }
        public MainTreeViewHandler TreeViewHandler { get; private set; }

        public string[] Arguments { get; private set; }

        public frmMain(string[] args)
        {
            InitializeComponent();

            this.Arguments = args;
            InitForm();
        }

        private void InitForm()
        {
            this.HostHandler = new MainHostHandler(this);
            this.TreeViewHandler = new MainTreeViewHandler(this);
            this.PluginHandler = new MainPluginHandler(this);
            this.ToolHandler = new MainToolHandler(this);

            SetupGridViewFilter();
            SetupFileWatcher();
        }


        #region Form event
        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.F5:
                    TreeViewHandler.RefreshNode(treeView1.SelectedNode, true);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    TreeViewHandler.EscPressed = true;
                    e.Handled = true;
                    break;
            }
        }

        private void toolStrip1_SizeChanged(object sender, EventArgs e)
        {
            txtPath.Width = toolStrip1.Width - lblPath.Width - 20;
        }

        private void txtPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string path = txtPath.Text.Trim();
                TreeViewHandler.FindPath(path);
                e.Handled = true;
            }
        }

        private void CheckUpdate() 
        {
            if (Config.CheckUpdateEnabled)
            {
                TimeSpan ts = DateTime.Now - Config.CheckUpdateLastDate;
                if (ts.Days >= Config.CheckUpdatePeriod)
                {
                    CheckForUpdate(true);
                    Config.CheckUpdateLastDate = DateTime.Now;
                }
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (OpenAssemblyWithClassEditor(this.Arguments) > 0)
            {
                //argument loaded
            }
            else
            {
                //set last path, need to be after settings upgraded
                if (!String.IsNullOrEmpty(Config.LastPath) && Directory.Exists(Config.LastPath))
                {
                    TreeViewHandler.SetPath(Config.LastPath, true);
                }
            }
        }

         private void frmMain_Load(object sender, EventArgs e)
        {           

            PluginHandler.LoadPlugins();

            ToolHandler.LoadTools();

            //put this after loadplugins to avoid "where's reflector" dialog
            HostHandler.SetupAssemblyResolver();

            #region About & Update
            try
            {
                ShowAbout(false);
                CheckUpdate();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
            #endregion About & Update
            
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //treeView1.SuspendLayout();
                //treeView1.BeginUpdate();
                //treeView1.DrawMode = TreeViewDrawMode.Normal;

                Config.LastPath = txtPath.Text;
                HostHandler.ReleaseAssemblyResolver();

            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        const string DATA_FORMAT = "FileDrop";
        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DATA_FORMAT))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;

        }

        private int OpenAssemblyWithClassEditor(string[] files)
        {
            int count = 0;
            if (files == null || files.Length == 0)
                return count;

            string path = files[0];
            if (Directory.Exists(path))
            {
                TreeViewHandler.SetPath(path, true);
                count++;
            }
            else
            {
                if (File.Exists(path))
                {
                    string filePath = Path.GetDirectoryName(path);
                    TreeViewHandler.SetPath(filePath, true);                    
                    TreeViewHandler.CheckDropFile(path);
                    count++;
                }
            }
            for (int i = 1; i < files.Length; i++)
            {
                path = files[i];
                if (File.Exists(path))
                {
                    TreeViewHandler.CheckDropFile(path);
                    count++;
                }
            }
            return count;
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = e.Data.GetData(DATA_FORMAT) as string[];
            OpenAssemblyWithClassEditor(s);
        }

        public void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        //TreeNode _lastSelectedNode = null;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //if (_lastSelectedNode != null)
            //{
            //    this.RemoveAssemblyResolveDir(_lastSelectedNode.FullPath);
            //}
            
            TreeViewHandler.RefreshNode(e.Node);
            
            //_lastSelectedNode = e.Node;
            //if (_lastSelectedNode != null)
            //{
            //    this.AddAssemblyResolveDir(_lastSelectedNode.FullPath);
            //}
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
        #endregion Form event

        #region Menu
        private void mnuProfileApp_Click(object sender, EventArgs e)
        {
            frmProfilerApp f = new frmProfilerApp(null);
            f.ShowDialog();
        }

        private void mnuProfileASPNet_Click(object sender, EventArgs e)
        {
            frmProfilerASPNet f = new frmProfilerASPNet(this);
            f.ShowDialog();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            ShowAbout(true);
        }

        private void mnuEmailAuthor_Click(object sender, EventArgs e)
        {
            EmailAuthor();
        }

        private void mnuGenStrongKey_Click(object sender, EventArgs e)
        {
            string initDir = null;
            if (!String.IsNullOrEmpty(Config.StrongKeyFile))
            {
                initDir = Path.GetDirectoryName(Config.StrongKeyFile);
            }

            string file = SimpleDialog.OpenFile(null, "Key Files(*.snk)|*.snk|All Files(*.*)|*.*", ".snk", false, initDir);
            if (!String.IsNullOrEmpty(file))
            {
                if (File.Exists(file)) File.Delete(file);

                SN sn = new SN();
                sn.Options.Host = this;
                sn.Options.SourceDir = Path.GetDirectoryName(file);
                sn.Options.rbCustomChecked = true;
                sn.Options.AdditionalOptions = String.Format(" -k \"{0}\" ", file);
                sn.Options.TextInfoBox = new TextBox();
                sn.Go();
                if (File.Exists(file))
                {
                    SimpleMessage.ShowInfo(String.Format("Key pair written to {0}", file));
                }
                else
                {
                    SimpleMessage.ShowInfo(String.Format("Failed to generate key file.\n\n{0}", sn.Options.TextInfoBox.Text));
                }
            }
        }

        private void mnuHome_Click(object sender, EventArgs e)
        {
            SimpleProcess.OpenWebSite(HomeUrl);
        }

        private void mnuSDK20_Click(object sender, EventArgs e)
        {
            //SimpleProcess.OpenWebSite(@"http://www.microsoft.com/downloads/results.aspx?pocId=&freetext=.Net%20Framework%202.0%20SDK&DisplayLang=en");
            SimpleProcess.OpenWebSite(@"http://www.google.com.hk/#hl=en&q=windows+sdk+for+.net+framework+2.0+site:www.microsoft.com");
        }

        private void mnuSDK35_Click(object sender, EventArgs e)
        {
            //SimpleProcess.OpenWebSite(@"http://www.microsoft.com/downloads/results.aspx?pocId=&freetext=.Net%20Framework%203.5%20SDK&DisplayLang=en");
            SimpleProcess.OpenWebSite(@"http://www.google.com.hk/#hl=en&q=windows+sdk+for+.net+framework+3.5+site:www.microsoft.com");
        }

        private void mnuSDK4_Click(object sender, EventArgs e)
        {
            SimpleProcess.OpenWebSite(@"http://www.google.com.hk/#hl=en&q=microsoft+windows+SDK+for+windows+7+and+.net+framework+4.0+site:www.microsoft.com");
        }

        private void mnuSDK45_Click(object sender, EventArgs e)
        {
            SimpleProcess.OpenWebSite(@"http://msdn.microsoft.com/en-US/windows/hardware/hh852363");            
        }

        private void mnuCheckForUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate(false);
        }

        private void mnuOptions_Click(object sender, EventArgs e)
        {
            frmOptions frm = new frmOptions(this);
            frm.ShowDialog();
        }

        private void mnuPluginList_Click(object sender, EventArgs e)
        {
            frmPluginList frm = new frmPluginList();
            frm.ShowDialog();
        }
        #endregion Menu        


        #region Context Menu

        private void CopyCellsToClipboard()
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < dgvData.SelectedRows.Count; i++)
                    {
                        DataRowView drv = dgvData.SelectedRows[i].DataBoundItem as DataRowView;
                        if (drv != null)
                        {
                            sb.AppendFormat("{0}\r\n", drv["full_name"]);
                        }
                    }
                    // Add the selection to the clipboard.
                    Clipboard.SetDataObject(sb.ToString());
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    SetStatusText("The Clipboard could not be accessed. Please try again.");
                }
            }
        }

        private void cmCopyInfo_Click(object sender, EventArgs e)
        {
            CopyCellsToClipboard();
        }

        private void cmFolderDasm_Click(object sender, EventArgs e)
        {
            try
            {
                frmDasm frm = new frmDasm(this, PathUtils.GetFullFileNames(dgvData.Rows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode, true);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmFolderAsm_Click(object sender, EventArgs e)
        {
            try
            {
                string path = treeView1.SelectedNode.FullPath;
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*.il", SearchOption.AllDirectories);

                frmAsm frm = new frmAsm(this, PathUtils.GetFullFileNames(files, path), path);
                frm.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmDasm_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly.");
                return;
            }

            try
            {
                frmDasm frm = new frmDasm(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode, true);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmAsm_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select IL file.");
                return;
            }

            try
            {
                frmAsm frm = new frmAsm(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private frmDeobf _frmDeobf = null;
        private void cmDeobf_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly.");
                return;
            }

            try
            {
                if (_frmDeobf == null)
                {
                    _frmDeobf = new frmDeobf(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                }
                else
                {
                    _frmDeobf.InitForm(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                }

                _frmDeobf.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select file.");
                return;
            }

            try
            {
                if (SimpleMessage.Confirm("Are you sure to delete selected file(s)?") == DialogResult.Yes)
                {
                    string path = SimplePath.GetFullPath(treeView1.SelectedNode.FullPath);
                    for (int i = 0; i < dgvData.SelectedRows.Count; i++)
                    {
                        string file = PathUtils.GetFileName(dgvData.SelectedRows[i], path);
                        File.Delete(file);
                    }

                    TreeViewHandler.RefreshNode(treeView1.SelectedNode);
                }

            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmOpenFolder_Click(object sender, EventArgs e)
        {
            Process.Start(SimplePath.GetFullPath(treeView1.SelectedNode.FullPath));
        }

        public void cmClassEditor_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly to edit.");
                return;
            }

            try
            {
                frmClassEdit frm = new frmClassEdit(
                    new ClassEditParams() {
                        Host = this,
                        Rows = PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath),
                        SourceDir = treeView1.SelectedNode.FullPath,
                        ObjectType = ObjectTypes.All,
                        ShowStaticOnly = false,
                        ShowSelectButton = false
                    }
                    );
                frm.Show();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmProfiler_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly to profile.");
                return;
            }

            try
            {
                string appFile = PathUtils.GetFullFileName(dgvData.SelectedRows, 0, treeView1.SelectedNode.FullPath);
                frmProfilerApp frm = new frmProfilerApp(appFile);
                frm.Show();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmRunMethod_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count != 1)
            {
                SimpleMessage.ShowInfo("Please select one assembly.");
                return;
            }

            try
            {
                frmRunMethod frm = new frmRunMethod(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmVerify_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly to verify.");
                return;
            }

            try
            {
                frmVerify frm = new frmVerify(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmAssembly_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;

            //nothing selected
            if (dgvData.SelectedRows.Count < 1)
            {
                for (int i = 0; i < cms.Items.Count; i++)
                {
                    cms.Items[i].Enabled = false;
                }
                return;
            }

            //common menus
            cms.Items["cmCopyInfo"].Enabled = true;
            cms.Items["cmOpenFolder"].Enabled = true;
            cms.Items["cmDeleteFile"].Enabled = true;

            //checking selected rows
            bool oneSelected = dgvData.SelectedRows.Count == 1;
            bool isAllExe = true;
            bool isAllAssembly = true;
            bool hasNetModule = false;
            for (int i = 0; i < dgvData.SelectedRows.Count; i++)
            {
                string fileName = PathUtils.GetFileName(dgvData.SelectedRows[i]);
                isAllExe = isAllExe && PathUtils.IsExe(fileName);
                isAllAssembly = isAllAssembly && PathUtils.IsAssembly(fileName);
                hasNetModule = hasNetModule || PathUtils.IsNetModule(fileName);
            }            
            bool isAllIL = !isAllAssembly;

            //standard menus
            if (isAllAssembly)
            {
                cms.Items["cmDasm"].Enabled = true;
                if (hasNetModule)
                {
                    cms.Items["cmDeobf"].Enabled = false;
                    cms.Items["cmVerify"].Enabled = false;
                    cms.Items["cmStrongName"].Enabled = false;
                }
                else
                {
                    cms.Items["cmDeobf"].Enabled = true;
                    cms.Items["cmVerify"].Enabled = true;
                    cms.Items["cmStrongName"].Enabled = true;
                }
                cms.Items["cmClassEditor"].Enabled = true;  //disable this just because Reflector cannot load *same* assembly files 
                if (oneSelected && !hasNetModule)
                {
                    cms.Items["cmRunMethod"].Enabled = true;
                }
                else
                {
                    cms.Items["cmRunMethod"].Enabled = false;
                }
            }
            else
            {
                cms.Items["cmDasm"].Enabled = false;
                cms.Items["cmDeobf"].Enabled = false;
                cms.Items["cmVerify"].Enabled = false;
                cms.Items["cmStrongName"].Enabled = false;
                cms.Items["cmClassEditor"].Enabled = false;
                cms.Items["cmRunMethod"].Enabled = false;
            }

            if (isAllIL)
            {
                cms.Items["cmAsm"].Enabled = true;
            }
            else
            {
                cms.Items["cmAsm"].Enabled = false;
            }

            if (isAllExe && oneSelected)
            {
                cms.Items["cmProfiler"].Enabled = true;
            }
            else
            {
                cms.Items["cmProfiler"].Enabled = false;
            }

            foreach (ToolStripItem item in cms.Items)
            {
                ToolStripMenuItem mi = item as ToolStripMenuItem;
                if (mi == null || mi.Tag == null) continue;
                IMainPlugin plugin = mi.Tag as IMainPlugin;
                if (plugin == null) continue;

                MainPluginInfo pi = plugin.PluginInfo;
                if (pi.RowType == RowTypes.One && !oneSelected)
                {
                    mi.Enabled = false;
                }
                else
                {
                    if (pi.SourceType == SourceTypes.Any) mi.Enabled = true;
                    else if (pi.SourceType == SourceTypes.Assembly && isAllAssembly) mi.Enabled = true;
                    else if (pi.SourceType == SourceTypes.Executable && isAllExe) mi.Enabled = true;
                    else if (pi.SourceType == SourceTypes.ILFile && isAllIL) mi.Enabled = true;
                    else mi.Enabled = false;
                }
            }

            e.Cancel = false;
        }

        private void cmFolderOpen_Click(object sender, EventArgs e)
        {
            cmOpenFolder_Click(sender, e);
        }

        private void cmFolderDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string path = SimplePath.GetFullPath(treeView1.SelectedNode.FullPath);
                if (SimpleMessage.Confirm(String.Format("Are you sure to delete selected folder:\n{0}?", path)) == DialogResult.Yes)
                {
                    Directory.Delete(path, true);
                    treeView1.SelectedNode.Remove();
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmStrongName_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count < 1)
            {
                SimpleMessage.ShowInfo("Please select assembly.");
                return;
            }

            try
            {
                frmStrongName frm = new frmStrongName(this, PathUtils.GetFullFileNames(dgvData.SelectedRows, treeView1.SelectedNode.FullPath), treeView1.SelectedNode.FullPath);
                frm.ShowDialog();

                TreeViewHandler.RefreshNode(treeView1.SelectedNode);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void cmRefresh_Click(object sender, EventArgs e)
        {
            TreeViewHandler.RefreshNode(treeView1.SelectedNode, true);
        }

        #endregion Context Menu


        #region GridView
        private void dgvData_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && !dgvData.Rows[e.RowIndex].Selected && e.ColumnIndex >= 0)
            {
                //dgvData.Rows[e.RowIndex].Selected = true;
                dgvData.CurrentCell = dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvData.CurrentRow != null)
            {
                string file = PathUtils.GetFileName(dgvData.CurrentRow);
                if (PathUtils.IsAssembly(file))
                {
                    cmClassEditor_Click(sender, e);
                }
            }
        }

        private void dgvData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvData.SelectedRows.Count > 0)
            {
                cmDeleteFile_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Apps && dgvData.SelectedRows.Count > 0)
            {
                Rectangle rect = dgvData.GetCellDisplayRectangle(0, dgvData.SelectedRows[0].Index, true);
                Point point = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                point = dgvData.PointToScreen(point);
                cmAssembly.Show(point);
                e.Handled = true;
            }
        }

        DgvFilterManager filterManager = null;
        private void SetupGridViewFilter()
        {
            filterManager = new DgvFilterManager(dgvData);
        }

        #endregion GridView


        #region IHost Members

        public string PluginDir
        {
            get { return PluginHandler.PluginDir; }
        }

        public ITextInfo TextInfo { get; set; }

        #endregion

        #region IStatus Members

        public void SetStatusText(string info)
        {
            HostHandler.SetStatusText(info);
        }

        public void SetStatusText(string info, bool doEvents)
        {
            HostHandler.SetStatusText(info, doEvents);
        }

        #endregion

        #region IProgressBar Members

        public void InitProgress(int min, int max)
        {
            HostHandler.InitProgress(min, max);
        }

        public void SetProgress(int val)
        {
            HostHandler.SetProgress(val);
        }

        public void SetProgress(int val, bool doEvents)
        {
            HostHandler.SetProgress(val, doEvents);
        }

        public void ResetProgress()
        {
            HostHandler.ResetProgress();
        }

        #endregion

        #region IPropertyValue Members

        public bool AddProperty(string propertyName, object defaultValue, Type propertyType)
        {
            return HostHandler.AddProperty(propertyName, defaultValue, propertyType);
        }

        public void RemoveProperty(string propertyName)
        {
            HostHandler.RemoveProperty(propertyName);
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            HostHandler.SetPropertyValue(propertyName, value);
        }

        public object GetPropertyValue(string propertyName)
        {
            return HostHandler.GetPropertyValue(propertyName);
        }

        #endregion

        #region IAssemblyResolveDir Members

        public bool AddAssemblyResolveDir(string dir)
        {
            return HostHandler.AddAssemblyResolveDir(dir);
        }

        public bool RemoveAssemblyResolveDir(string dir)
        {
            return HostHandler.RemoveAssemblyResolveDir(dir);
        }

        #endregion

        #region ITextFileWatcher Members

        public event FileSystemEventHandler TextFileChanged;

        void SetupFileWatcher()
        {
            this.fileSystemWatcher1.Path = Application.StartupPath;
        }
      
        Dictionary<string, DateTime> lastWrites = new Dictionary<string, DateTime>();

        void HandleFileChange(FileSystemEventArgs e)
        {
            string key = e.Name.ToLower();
            if (lastWrites.ContainsKey(key))
            {
                if ((DateTime.Now - lastWrites[key]).TotalSeconds < 1.0)
                {
                    //#if DEBUG
                    //                  SimpleMessage.ShowInfo("Ignored file change event: " + e.Name);
                    //#endif
                    lastWrites[key] = DateTime.Now;
                    return;
                }
                else
                {
                    lastWrites[key] = DateTime.Now;
                }
            }
            else
            {
                lastWrites.Add(key, DateTime.Now);
            }

            if (e.Name.Equals(ToolFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                ToolFile.Default.Clear();
                ToolHandler.LoadTools();
            }
            else if (e.Name.Equals(PatternFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                PatternFile.Default.Clear();
            }
            else if (e.Name.Equals(RegexFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                RegexFile.Default.Clear();
            }
            else if (e.Name.Equals(ExceptionHandlerFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                ExceptionHandlerFile.Default.Clear();
            }
            else if (e.Name.Equals(KeywordFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                KeywordFile.Default.Clear();
            }
            else if (e.Name.Equals(IgnoredMethodFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                IgnoredMethodFile.Default.Clear();
            }
            else if (e.Name.Equals(IgnoredTypeFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                IgnoredTypeFile.Default.Clear();
            }
            else if (e.Name.Equals(AttributeFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                AttributeFile.Default.Clear();
            }
            else if (e.Name.Equals(ProfileFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                ProfileFile.Default.Clear();
            }
            else if (e.Name.Equals(RandomFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                RandomFile.Default.Clear();
            }

            if (TextFileChanged != null)
            {
                TextFileChanged(this, e);
            }
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            HandleFileChange(e);
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            HandleFileChange(e);
        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            HandleFileChange(e);
        }


        #endregion

        #region Utils

        public string HomeUrl = SimpleDotNet.WebSiteUrl + "/simple-assembly-explorer";

        public void CheckForUpdate(bool silent)
        {
            SimpleUpdater.CheckForUpdate(
                HomeUrl + "/version",
                HomeUrl,
                silent);
        }

        public void ShowAbout(bool force)
        {
            if (force || Config.ShowAbout)
            {
                SimpleMessage.ShowAbout("This FREE tool is provided \"AS IS\" with no warranties,\r\nand confers no rights.\r\n" +
                    "Use it at your own risk.\r\n\r\n" +
                    "Credits:\r\n.Net Reflector\r\nBe.HexEditor\r\nde4dot\r\nILMerge\r\nILSpy\r\nMicrosoft (gacutil, ilasm, ildasm, peverify, sn, CLR Profiler, ...)\r\nMono Cecil\r\n......"
                    );
                Config.ShowAbout = false;
            }
        }

        public void EmailAuthor()
        {
            SimpleDotNet.SendEmail("SAE Issue/Suggestion", "Any issue or suggestion?");
        }
        #endregion 


 
    }//end of class
}
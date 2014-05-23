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
    public class MainTreeViewHandler
    {
        frmMain _form;
        TreeView treeView1;
        DataGridView dgvData;

        public MainTreeViewHandler(frmMain form)
        {
            _form = form;
            treeView1 = _form.TreeView;
            dgvData = _form.AssemblyDataGrid;

            CreateFilesTable();
            AddDrives();
        }

        public void SetStatusText(string info)
        {
            _form.SetStatusText(info);
        }

        public string CurrentPath
        {
            get
            {
                TreeNode n = treeView1.SelectedNode;
                return n == null ? null : n.FullPath;
            }
        }

        public void FindPath(string path)
        {
            if (String.IsNullOrEmpty(path)) return;

            if (!Directory.Exists(path)) return;

            TreeNode n = FindTreeNode(path);
            if (n != null)
            {
                if (treeView1.SelectedNode.Equals(n))
                {
                }
                else
                {
                    treeView1.SelectedNode = n;
                }
            }
        }

        public void SetPath(string path, bool findPath)
        {
            _form.PathTextBox.Text = path;

            if (findPath)
            {
                FindPath(path);
            }
        }

        TreeNode CreateTreeNode(string text)
        {
            TreeNode n = new TreeNode(text);
            n.ContextMenuStrip = _form.FolderContextMenuStrip;
            return n;
        }

        private TreeNode FindTreeNode(string path)
        {
            TreeNode foundNode = null;
            using(new SimpleWaitCursor())
            {
                string[] sArr = path.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                if (sArr != null && sArr.Length > 0)
                {
                    TreeNode n = FindTreeNode(treeView1.Nodes, sArr[0]);
                    if (n != null)
                    {
                        if (sArr.Length == 1)
                        {
                            foundNode = n;
                        }
                        else
                        {
                            string searchPath = sArr[0];
                            for (int i = 1; i < sArr.Length; i++)
                            {
                                searchPath = String.Format("{0}{1}{2}", searchPath, Path.DirectorySeparatorChar, sArr[i]);
                                if (n.Nodes.Count == 0)
                                    AddDirectories(n);
                                n = FindTreeNode(n.Nodes, searchPath);

                                if (n == null) break;
                                if (i == sArr.Length - 1)
                                {
                                    foundNode = n;
                                    break;
                                }
                            }
                        }
                    }//end of n!=null
                }
            }
            return foundNode;
        }

        private TreeNode FindTreeNode(TreeNodeCollection nodes, string path)
        {
            TreeNode foundNode = null;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].FullPath.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                {
                    foundNode = nodes[i];
                    break;
                }
            }

            return foundNode;
        }

        void AddDrives()
        {
            DriveInfo[] dis = DriveInfo.GetDrives();

            treeView1.BeginUpdate();

            foreach (DriveInfo di in dis)
            {
                TreeNode node = CreateTreeNode(di.Name.Replace(SimplePath.DirectorySeparator, String.Empty));

                switch (di.Name)
                {
                    case "C:\\":
                        // The next statement causes the treeView1_AfterSelect Event to fire once on startup.
                        // This effect can be seen just after intial program load. C:\ node is selected
                        // Automatically on program load, expanding the C:\ treeView1 node.
                        treeView1.SelectedNode = node;
                        node.SelectedImageIndex = 1;
                        node.ImageIndex = 1;
                        break;

                    default:
                        switch (di.DriveType)
                        {
                            case DriveType.Fixed:
                            case DriveType.Ram:
                            case DriveType.NoRootDirectory:
                                node.SelectedImageIndex = 1;
                                node.ImageIndex = 1;
                                break;
                            case DriveType.CDRom:
                                node.SelectedImageIndex = 2;
                                node.ImageIndex = 2;
                                break;
                            case DriveType.Network:
                                node.SelectedImageIndex = 3;
                                node.ImageIndex = 3;
                                break;
                            case DriveType.Removable:
                                node.SelectedImageIndex = 0;
                                node.ImageIndex = 0;
                                break;
                            default:
                                node.SelectedImageIndex = 4;
                                node.ImageIndex = 4;
                                break;
                        }
                        break;
                }

                treeView1.Nodes.Add(node);
            }
            treeView1.EndUpdate();
        }

        private void AddDirectories(TreeNode node)
        {
            // This method is used to get directories (from disks, or from other directories)

            treeView1.BeginUpdate();

            try
            {
                escPressed = false;
                DirectoryInfo diRoot;

                diRoot = new DirectoryInfo(node.FullPath + Path.DirectorySeparatorChar);
                DirectoryInfo[] dirs = diRoot.GetDirectories();
                Array.Sort<DirectoryInfo>(dirs, new DirectoryInfoComparer());

                // Must clear this first, else the directories will get duplicated in treeview
                node.Nodes.Clear();

                // Add the sub directories to the treeView1
                foreach (DirectoryInfo dir in dirs)
                {
                    if (escPressed)
                    {
                        break;
                    }
                    SetStatusText(String.Format("Press ESC to cancel: {0}", dir.Name));
                    TreeNode subNode = CreateTreeNode(dir.Name);
                    subNode.ImageIndex = 5;
                    subNode.SelectedImageIndex = 6;
                    node.Nodes.Add(subNode);
                }

            }
            // Throw Exception when accessing directory: C:\System Volume Information	 // do nothing
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                treeView1.EndUpdate();
                SetStatusText(null);
            }
        }

        DataTable dtFiles = null;
        private void CreateFilesTable()
        {
            dtFiles = new DataTable("Files");
            dtFiles.Columns.Add("file_name", typeof(String));
            dtFiles.Columns.Add("assembly_name", typeof(String));
            dtFiles.Columns.Add("version", typeof(String));
            dtFiles.Columns.Add("public_token", typeof(String));
            dtFiles.Columns.Add("processor_architecture", typeof(String));
            dtFiles.Columns.Add("target_runtime", typeof(String));
            dtFiles.Columns.Add("full_name", typeof(String));
            _form.AssemblyDataGridBindingSource.DataSource = dtFiles;
        }

        bool escPressed = false;
        public bool EscPressed
        {
            get { return escPressed; }
            set { escPressed = value; }
        }

        private string GetProcessorArchitecture(ModuleDefinition module)
        {
            string pa = String.Empty;
            switch (module.Architecture)
            {
                case TargetArchitecture.I386:
                    if ((module.Attributes & ModuleAttributes.Required32Bit) == ModuleAttributes.Required32Bit)
                    {
                        pa = "X86";
                    }
                    break;
                case TargetArchitecture.AMD64:
                    pa = "Amd64";
                    break;
                case TargetArchitecture.IA64:
                    pa = "IA64";
                    break;
            }
            if ((module.Attributes & ModuleAttributes.ILOnly) == ModuleAttributes.ILOnly)
            {
                if (pa == String.Empty)
                    pa = "MSIL";
            }
            else
            {
                if (pa != String.Empty) pa += " | ";
                pa += "Mixed";
            }
            return pa;
        }

        private void AddFiles(string path, string match)
        {
            try
            {
                escPressed = false;

                DirectoryInfo di = new DirectoryInfo(path + Path.DirectorySeparatorChar);
                FileInfo[] files = di.GetFiles(match);
                bool isAssembly = PathUtils.IsAssembly(match);

                foreach (FileInfo file in files)
                {
                    if (escPressed)
                    {
                        break;
                    }
                    if (file.Length == 0) continue;

                    SetStatusText(String.Format("Press ESC to cancel: {0}", file.FullName));
                    try
                    {

                        if (isAssembly)
                        {
                            //try
                            //{
                            //    AssemblyName an = AssemblyName.GetAssemblyName(file.FullName);
                            //    DataRow dr = dtFiles.NewRow();
                            //    dr["file_name"] = file.Name;
                            //    dr["assembly_name"] = an.Name;
                            //    dr["version"] = an.Version.ToString();
                            //    dr["public_token"] = TokenUtils.GetPublicKeyTokenString(an);
                            //    dr["processor_architecture"] = an.ProcessorArchitecture == ProcessorArchitecture.None ? String.Empty : an.ProcessorArchitecture.ToString();
                            //    dr["full_name"] = an.FullName;
                            //    dtFiles.Rows.Add(dr);
                            //}
                            //catch (BadImageFormatException)

                            if (PathUtils.IsNetModule(file.FullName))
                            {
                                ModuleDefinition md = ModuleDefinition.ReadModule(file.FullName);
                                DataRow dr = dtFiles.NewRow();
                                dr["file_name"] = file.Name;
                                dr["assembly_name"] = String.Empty;
                                dr["version"] = String.Empty;
                                dr["public_token"] = null;
                                dr["target_runtime"] = md.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
                                dr["processor_architecture"] = GetProcessorArchitecture(md);
                                dr["full_name"] = md.FullyQualifiedName;
                                dtFiles.Rows.Add(dr);
                            }
                            else
                            {
                                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file.FullName);
                                AssemblyNameDefinition an = ad.Name;
                                DataRow dr = dtFiles.NewRow();
                                dr["file_name"] = file.Name;
                                dr["assembly_name"] = an.Name;
                                dr["version"] = an.Version.ToString();
                                dr["public_token"] = TokenUtils.GetPublicKeyTokenString(an.PublicKeyToken);
                                dr["target_runtime"] = ad.MainModule.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
                                dr["processor_architecture"] = GetProcessorArchitecture(ad.MainModule);
                                dr["full_name"] = an.FullName;
                                dtFiles.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            DataRow dr = dtFiles.NewRow();
                            dr["file_name"] = file.Name;
                            dr["full_name"] = file.FullName;
                            dtFiles.Rows.Add(dr);
                        }
                    }
                    catch
                    {
                        //not .net assembly
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SetStatusText(null);
            }
        }

        private void SetCurrentRow(int rowIndex)
        {
            if (dgvData.Rows.Count > rowIndex)
            {
                dgvData.CurrentCell = dgvData.Rows[rowIndex].Cells[0];
                dgvData.FirstDisplayedScrollingRowIndex = rowIndex > 10 ? rowIndex - 10 : 0;
            }
        }

        private void AddFiles(string path)
        {
            int saveCurrentRowIndex = -1;
            if (treeView1.SelectedNode != null
                && path == treeView1.SelectedNode.FullPath
                && this.dgvData.CurrentRow != null)
                saveCurrentRowIndex = dgvData.CurrentRow.Index;

            SimpleWaitCursor wc = new SimpleWaitCursor();
            try
            {
                _form.AssemblyDataGridBindingSource.DataSource = null;
                dtFiles.Clear();
                AddFiles(path, "*.dll");
                if (!escPressed) AddFiles(path, "*.exe");
                if (!escPressed) AddFiles(path, "*.netmodule");
                if (!escPressed) AddFiles(path, "*.il");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (String.IsNullOrEmpty(dtFiles.DefaultView.Sort))
                    dtFiles.DefaultView.Sort = dtFiles.Columns[0].ColumnName;
                _form.AssemblyDataGridBindingSource.DataSource = dtFiles;
                wc.Dispose();
            }

            if (saveCurrentRowIndex >= 0)
            {
                SetCurrentRow(saveCurrentRowIndex);
            }
        }


        bool _treeviewUpdating = false;
        public void RefreshNode(TreeNode node)
        {
            RefreshNode(node, false);
        }

        public void RefreshNode(TreeNode node, bool force)
        {
            if (node == null) return;
            if (_treeviewUpdating) return;

            try
            {
                treeView1.BeginUpdate();
                _treeviewUpdating = true;

                if (force || node.Nodes.Count == 0)
                {
                    AddDirectories(node);
                    node.Expand();
                }

                // Get files from disk, add to DataGridView control
                AddFiles(node.FullPath);

                SetPath(SimplePath.GetFullPathWithoutTrailingSeparator(node.FullPath), false);

                if (escPressed)
                {
                    SetStatusText("Operation cancelled.");
                }
                else
                {
                    SetStatusText(String.Format("{0} Folder{1}, {2} File{3}",
                        node.Nodes.Count, (node.Nodes.Count > 1 ? "s" : ""),
                        dgvData.Rows.Count, (dgvData.Rows.Count > 1 ? "s" : "")));
                }

                CheckDropFile(null);
            }
            catch (Exception ex)
            {
                SetStatusText(ex.Message);
            }
            finally
            {
                treeView1.EndUpdate();
                _treeviewUpdating = false;
            }
        }

        string _fileDrop = null;
        public void CheckDropFile(string droppedFile)
        {
            if (droppedFile != null)
            {
                _fileDrop = droppedFile;
            }
            try
            {
                if (!String.IsNullOrEmpty(_fileDrop))
                {
                    for (int i = 0; i < dgvData.Rows.Count; i++)
                    {
                        string fileName = PathUtils.GetFullFileName(dgvData.Rows, i, treeView1.SelectedNode.FullPath);
                        if (_fileDrop.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            dgvData.CurrentCell = dgvData.Rows[i].Cells[0];
                            if (Config.ClassEditorAutoOpenDroppedAssemblyEnabled)
                            {
                                _form.cmClassEditor_Click(null, null);
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _fileDrop = null;
            }
        }

      

    } //end of class
}

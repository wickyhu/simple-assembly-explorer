using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using System.Resources;

namespace SimpleAssemblyExplorer
{
    public class ClassEditTreeViewHandler
    {
        frmClassEdit _form;
        TreeView treeView1;

        public ObjectTypes ObjectType { get; set; }
        private List<TreeNode> _nodeHistory = new List<TreeNode>();
        private int _lastNodeIndex = -1;

        private List<MemberReference> _renamedObjects = new List<MemberReference>();
        public List<MemberReference> RenamedObjects
        {
            get { return _renamedObjects; }
        }

        public ClassEditTreeViewHandler(frmClassEdit form)
        {
            _form = form;
            treeView1 = _form.TreeView;

            InitTreeView();
        }

        private void AddFileNode(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                AddFileNode(file);
            }
        }

        public TreeNode AddFileNode(string file)
        {
            //foreach (TreeNode n in treeView1.Nodes)
            //{
            //    if (file == GetTreeNodeText(n))
            //        return null;
            //}

            TreeNode node = CreateTreeNode(file);
            node.ImageIndex = 150;
            node.SelectedImageIndex = node.ImageIndex;
            treeView1.Nodes.Add(node);

            //init whole tree, disabled for performance
            //InitNode(node); //level 0
            //foreach (TreeNode n1 in node.Nodes)
            //{
            //    InitNode(n1);//level 1
            //    foreach (TreeNode n2 in n1.Nodes) //level 2
            //    {
            //        if (n2.Nodes.Count > 0)
            //        {
            //            foreach (TreeNode n3 in n2.Nodes)
            //            {
            //                InitNode(n3);
            //            }
            //        }
            //    }
            //} 

            _form.LogHandler.LogLoad(file);
            return node;
        }

        public void ExpandFileNode(TreeNode n)
        {
            for (int i = 0; i < 2; i++)
            {
                InitNode(n);
                if (n.Nodes.Count > 0)
                {
                    n.Expand();
                    n = n.Nodes[0];
                }
                else
                    break;
            }
        }

        private void InitTreeView()
        {
            ObjectType = ObjectTypes.Constructor | ObjectTypes.Event | ObjectTypes.Field |
               ObjectTypes.Interface | ObjectTypes.Method | ObjectTypes.Property;

            treeView1.TreeViewNodeSorter = new TreeNodeSorter();

            if (_form.ShowStaticOnly)
            {
                _form.Text += ": Static";
            }
            if (_form.Rows.Length == 1)
            {
                _form.Text += " - " + _form.Rows[0];
            }

            AddFileNode(_form.Rows);
            if (treeView1.Nodes.Count > 0)
            {
                ExpandFileNode(treeView1.Nodes[0]);
            }
        }

        public TreeNode SelectedNode
        {
            get { return treeView1.SelectedNode; }
            set { treeView1.SelectedNode = value; }
        }

        public MethodDefinition SelectedMethod
        {
            get
            {
                MethodDefinition method;
                TreeNode node = this.SelectedNode;
                if (node == null || node.Tag == null)
                {
                    method = null;
                }
                else
                {
                    method = node.Tag as MethodDefinition;
                }
                return method;
            }
        }

        public void ClearRenamedObjects()
        {
            foreach (MemberReference mr in _renamedObjects)
            {
                InsUtils.RemoveOldMemberName(mr);
            }
            _renamedObjects.Clear();
        }

        public void NavigateBack()
        {
            try
            {
                _saveNodeHistory = false;
                int newIndex = _lastNodeIndex;
                if (newIndex < 1)
                    newIndex = _nodeHistory.Count - 1;
                else
                    newIndex--;

                if (newIndex >= 0 && newIndex < _nodeHistory.Count && _nodeHistory[newIndex] != null)
                {
                    treeView1.SelectedNode = _nodeHistory[newIndex];
                    _lastNodeIndex = newIndex;
                }
            }
            finally
            {
                _saveNodeHistory = true;
            }
        }

        public void NavigateForward()
        {
            try
            {
                _saveNodeHistory = false;
                int newIndex = _lastNodeIndex;
                if (newIndex == _nodeHistory.Count - 1)
                    newIndex = 0;
                else
                    newIndex++;
                if (newIndex >= 0 && newIndex < _nodeHistory.Count && _nodeHistory[newIndex] != null)
                {
                    treeView1.SelectedNode = _nodeHistory[newIndex];
                    _lastNodeIndex = newIndex;
                }
            }
            finally
            {
                _saveNodeHistory = true;
            }
        }

        private bool _saveNodeHistory = true;
        public void SaveNodeHistory(TreeNode node)
        {
            if (!_saveNodeHistory || IsBaseTypeNode(node) || IsDerivedTypeNode(node))
                return;
            if (_nodeHistory.Count > 0 && ClassEditBookmarkHandler.Bookmark.GetIndexPath(node) == ClassEditBookmarkHandler.Bookmark.GetIndexPath(_nodeHistory[_nodeHistory.Count - 1]))
                return;

            if (_nodeHistory.Count >= 50)
            {
                _nodeHistory.RemoveAt(0);
            }
            _nodeHistory.Add(node);
            _lastNodeIndex = _nodeHistory.Count - 1;
        }

        public TreeNode FindTopNode(TreeNode treeNode)
        {
            TreeNode n = treeNode;
            if (n == null)
            {
                n = treeView1.SelectedNode;
            }

            while (n != null && n.Level > 0)
            {
                n = n.Parent;
            }
            return n;
        }

        private void SaveTreeNode(MemberReference mr, TreeNode node)
        {
            mr.Tag = node;
        }
        private TreeNode GetTreeNode(MemberReference mr)
        {
            return mr.Tag as TreeNode;
        }

        public string GetTreeNodeText(TreeNode n)
        {
            //TreeNode will change & to && in file name
            return n.Text.Replace("&&", "&");
        }

        public bool InitNode(TreeNode n)
        {
            bool inited = false;
            AssemblyDefinition ad;

            bool showStaticOnly = _form.ShowStaticOnly;

            switch (n.Level)
            {
                case 0: // File Name -> Module
                    if (n.Nodes.Count == 0)
                    {
                        string file = GetTreeNodeText(n);
                        if (!File.Exists(file))
                        {
                            file = Path.Combine(_form.SourceDir, Path.GetFileName(GetTreeNodeText(n)));
                        }
                        bool isNetModule = PathUtils.IsNetModule(file);
                        if (isNetModule)
                        {
                            ModuleDefinition md = ModuleDefinition.ReadModule(file);
                            AddModuleNode(n, md);
                        }
                        else
                        {
                            ad = n.Tag as AssemblyDefinition;
                            if (ad == null)
                            {
                                ad = AssemblyDefinition.ReadAssembly(file);
                                n.Tag = ad;
                            }
                            // add all modules
                            foreach (ModuleDefinition md in ad.Modules)
                            {
                                AddModuleNode(n, md);
                            }
                        }
                        inited = true;
                    }
                    break;
                case 1: // Module -> Name Space -> Class,Enum,Reference,Resource
                    if (n.Nodes.Count == 0)
                    {
                        Dictionary<string, TreeNode> nameSpaces = new Dictionary<string, TreeNode>();
                        ModuleDefinition module = n.Tag as ModuleDefinition;

                        //add all references                        
                        if ((module.AssemblyReferences.Count > 0 || module.ModuleReferences.Count > 0) &&
                            !showStaticOnly)
                        {
                            AddReferencesNode(n, module);
                        }

                        //add all types
                        foreach (TypeDefinition td in module.AllTypes)
                        {
                            if (td.DeclaringType != null) continue;

                            if (showStaticOnly)
                            {
                                if (td.IsEnum || td.IsInterface) continue;
                            }

                            TreeNode nodeNameSpace = null;
                            if (nameSpaces.ContainsKey(td.Namespace))
                                nodeNameSpace = nameSpaces[td.Namespace];
                            if (nodeNameSpace == null)
                            {
                                nodeNameSpace = AddNameSpaceNode(n, td.Namespace);
                                nameSpaces.Add(td.Namespace, nodeNameSpace);
                            }

                            AddTypeNode(nodeNameSpace, td);
                        }

                        //add all resources
                        if (module.Resources.Count > 0 && !showStaticOnly)
                        {
                            AddResourcesNode(n, module);
                        }

                        inited = true;
                    }
                    break;
                case 2: // NameSpace -> Class
                    inited = false;
                    break;
                case 3: // Class -> Method, Property, Field, Event, Nested Classes ...
                    if (n.Nodes.Count == 0 && n.Tag != null)
                    {
                        TypeDefinition td = n.Tag as TypeDefinition;

                        AddTypeNodes(n, td);
                        inited = true;
                    }
                    break;
                default: // Nested Classes ...
                    if (n.Nodes.Count == 0)
                    {
                        if (n.Tag is TypeDefinition)
                        {
                            TypeDefinition td = n.Tag as TypeDefinition;

                            AddTypeNodes(n, td);
                            inited = true;
                        }
                    }
                    break;
            }
            return inited;
        }

        public void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode n = e.Node;
            if (InitNode(n))
            {
                if (!n.IsExpanded && e.Action != TreeViewAction.Collapse)
                    n.Expand();
            }

            if (n.Tag is MethodDefinition)
            {
                MethodDefinition md = n.Tag as MethodDefinition;
                _form.BodyGridHandler.InitBody(md);
            }
            else if (n.Tag is Resource)
            {
                ShowResourceNode(n);
            }
            else
            {
                //_form.ResourceHandler.ShowDetailsControl(ClassEditResourceHandler.DetailTypes.None);
                _form.BodyGridHandler.InitBody(null);
            }

            if (_form.ReflectorHandler.IsReady)
            {
                if (_form.TabControl.SelectedTab == _form.ReflectorTabPage)
                    _form.ReflectorHandler.InitRichText(n.Tag);
                else
                    _form.ReflectorHandler.NeedToRefresh = true;
            }
            if (_form.ILSpyHandler.IsReady)
            {
                if (_form.TabControl.SelectedTab == _form.ILSpyTabPage)
                    _form.ILSpyHandler.InitRichText(n.Tag);
                else
                    _form.ILSpyHandler.NeedToRefresh = true;
            }

            SaveNodeHistory(n);

            ClassEditBookmarkHandler.Bookmark bm = _form.BookmarkHandler.FindBookmark(n);
            if (bm != null)
            {
                _form.BookmarkComboBox.SelectedItem = bm;
            }
            else
            {
                _form.BookmarkComboBox.SelectedIndex = 0;
            }
        }

        public void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                TreeNode n = treeView1.SelectedNode;
                if (n != null)
                {
                    Clipboard.SetText(
                        String.Format("{0}: {1}",
                            n.ToString(),
                            n.Tag == null ? String.Empty : n.Tag.ToString()
                            )
                    );
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                TreeNode n = treeView1.SelectedNode;
                if (n != null && n.Tag is MethodDefinition)
                {
                    _form.BodyGridHandler.cmDeobf_Click(sender, e);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                TreeNode n = treeView1.SelectedNode;
                if (n == null || !CanBeRenamed(n))
                {
                    e.Handled = true;
                }
                else
                {
                    n.BeginEdit();
                }
            }
        }

        public TreeNode FindTreeNode(AssemblyDefinition ad)
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            TreeNode node = null;
            foreach (TreeNode n in nodes)
            {
                InitNode(n);
                if (ad.FullName == ((AssemblyDefinition)n.Tag).FullName)
                {
                    node = n;
                    break;
                }
            }
            return node;
        }

        public TreeNode FindTreeNode(TreeNode assemblyNode, ModuleDefinition mod)
        {
            //node parameter is an assembly node
            TreeNodeCollection nodes = assemblyNode.Nodes;
            TreeNode node = null;
            foreach (TreeNode n in nodes)
            {
                InitNode(n);
                if (mod.FullyQualifiedName == ((ModuleDefinition)n.Tag).FullyQualifiedName)
                {
                    node = n;
                    break;
                }
            }
            return node;
        }

        public TreeNode FindTreeNode(TreeNode moduleNode, TypeReference tr)
        {
            TreeNode node = null;
            foreach (TreeNode n in moduleNode.Nodes) // namespace
            {
                foreach (TreeNode n2 in n.Nodes)
                {
                    InitNode(n2);
                    if (n2.Tag is TypeReference)
                    {
                        TreeNode n3 = FindTreeNodeNested(n2, tr);
                        if (n3 != null)
                        {
                            node = n3;
                            break;
                        }
                    }
                    if (node != null)
                        break;
                }//end of each type

                if (node != null)
                    break;
            }//end of each namespace

            return node;
        }

        public TreeNode FindTreeNodeNested(TreeNode typeNode, TypeReference tr)
        {
            if (tr.FullName == ((TypeDefinition)typeNode.Tag).FullName)
                return typeNode;
            foreach (TreeNode n in typeNode.Nodes)
            {
                InitNode(n);
                if (n.Tag is TypeReference)
                {
                    TreeNode n2 = FindTreeNodeNested(n, tr);
                    if (n2 != null) return n2;
                }
            }
            return null;
        }

        public TreeNode FindTreeNode(TreeNode typeNode, MethodDefinition md)
        {
            TreeNode node = null;
            foreach (TreeNode n in typeNode.Nodes)
            {
                if (n.Tag is MethodDefinition)
                {
                    MethodDefinition tmp = (MethodDefinition)n.Tag;
                    if (md.FullName == tmp.FullName && md.MetadataToken.ToUInt32() == tmp.MetadataToken.ToUInt32())
                    {
                        node = n;
                        break;
                    }
                }
                else //property, event ...
                {
                    foreach (TreeNode n2 in n.Nodes)
                    {
                        if (n2.Tag is MethodDefinition)
                        {
                            MethodDefinition tmp = (MethodDefinition)n2.Tag;
                            if (md.FullName == tmp.FullName && md.MetadataToken.ToUInt32() == tmp.MetadataToken.ToUInt32())
                            {
                                node = n2;
                                break;
                            }
                        }
                    }
                }
            }
            return node;
        }

        public TreeNode FindTreeNode(TreeNode typeNode, FieldDefinition fd)
        {
            TreeNode node = null;
            foreach (TreeNode n in typeNode.Nodes)
            {
                if (n.Tag is FieldDefinition)
                {
                    FieldDefinition tmp = (FieldDefinition)n.Tag;
                    if (fd.FullName == tmp.FullName && fd.MetadataToken.ToUInt32() == tmp.MetadataToken.ToUInt32())
                    {
                        node = n;
                        break;
                    }
                }
            }
            return node;
        }

        public TreeNode FindTreeNode(TypeDefinition type)
        {
            TreeNode node = GetTreeNode(type);
            if (node != null) return node;

            ModuleDefinition mod = type.Module;
            AssemblyDefinition ad = mod.Assembly;
            node = FindTreeNode(ad);
            if (node == null) return null;

            node = FindTreeNode(node, mod);
            if (node == null) return null;

            node = FindTreeNode(node, type);
            if (node == null) return null;

            SaveTreeNode(type, node);
            return node;
        }

        public TreeNode FindTreeNode(MethodDefinition md)
        {
            TreeNode node = GetTreeNode(md);
            if (node != null) return node;

            //search for type node
            TypeDefinition td = md.DeclaringType;
            node = FindTreeNode(td);
            if (node == null) return null;

            node = FindTreeNode(node, md);
            if (node == null) return null;

            SaveTreeNode(md, node);

            return node;
        }

        public TreeNode FindTreeNode(FieldDefinition fd)
        {
            TreeNode node = GetTreeNode(fd);
            if (node != null) return node;

            //search for type node
            TypeDefinition td = fd.DeclaringType;
            node = FindTreeNode(td);
            if (node == null) return null;

            node = FindTreeNode(node, fd);
            if (node == null) return null;

            SaveTreeNode(fd, node);

            return node;
        }

        public List<AssemblyDefinition> GetLoadedAssemblyList()
        {
            List<AssemblyDefinition> list = new List<AssemblyDefinition>();
            foreach (TreeNode n in treeView1.Nodes)
            {
                InitNode(n);
                AssemblyDefinition ad = n.Tag as AssemblyDefinition;
                if (ad != null)
                    list.Add(ad);
            }
            return list;
        }

        public bool IsLoadedAssembly(AssemblyDefinition assembly)
        {
            List<AssemblyDefinition> list = GetLoadedAssemblyList();
            foreach (AssemblyDefinition ad in list)
            {
                if (ad.FullName == assembly.FullName)
                    return true;
            }
            return false;
        }

        public void ShowMethod(MethodReference mr)
        {
            MethodDefinition md = DeobfUtils.Resolve(mr, GetLoadedAssemblyList(), null);
            if (md == null) return;

            using (new SimpleWaitCursor())
            {
                TreeNode n = FindTreeNode(md);
                if (n != null)
                {
                    SaveNodeHistory(treeView1.SelectedNode);
                    treeView1.SelectedNode = n;
                }
                else //if (IsLoadedAssembly(md.Module.Assembly)) // whether allow to go out side?
                {
                    _form.BodyGridHandler.InitBody(md);
                }
            }
        }

        public void ShowField(FieldReference fr)
        {
            FieldDefinition fd = DeobfUtils.Resolve(fr, GetLoadedAssemblyList(), null);
            if (fd == null) return;

            using (new SimpleWaitCursor())
            {
                TreeNode n = FindTreeNode(fd);
                if (n != null)
                {
                    SaveNodeHistory(treeView1.SelectedNode);
                    treeView1.SelectedNode = n;
                }
            }
        }

        public TreeNode CreateTreeNode(string text)
        {
            if (text.Contains("&"))
            {
                text = text.Replace("&", "&&");
            }
            TreeNode n = new TreeNode(text);
            n.ContextMenuStrip = _form.TreeViewContextMenuStrip;
            return n;
        }

        private void AddTypeNode(TreeNode parentNode, TypeDefinition td)
        {
            TreeNode node = CreateTreeNode(InsUtils.GetTypeText(td));
            node.Tag = td;
            node.ToolTipText = TokenUtils.GetFullMetadataTokenString(td.MetadataToken);

            if (td.IsEnum)
            {
                if ((td.Attributes & TypeAttributes.Public) > 0)
                    node.ImageIndex = 24;
                else
                    node.ImageIndex = 27;
            }
            else if (td.IsInterface)
            {
                if ((td.Attributes & TypeAttributes.Public) > 0)
                    node.ImageIndex = 12;
                else
                    node.ImageIndex = 15;
            }
            else
            {
                if ((td.Attributes & TypeAttributes.Public) > 0)
                    node.ImageIndex = 6;
                else
                    node.ImageIndex = 9;
            }
            node.SelectedImageIndex = node.ImageIndex;
            parentNode.Nodes.Add(node);
        }

        private TreeNode AddMethodNode(TreeNode parentNode, MethodDefinition md)
        {
            TreeNode node = CreateTreeNode(InsUtils.GetMethodText(md));
            node.Tag = md;
            if (md.HasOverrides)
            {
                node.ToolTipText = String.Format("{0}, override {1}",
                    TokenUtils.GetFullMetadataTokenString(md.MetadataToken),
                    md.Overrides[0].ToString());
            }
            else
            {
                node.ToolTipText = TokenUtils.GetFullMetadataTokenString(md.MetadataToken);
            }

            if (md.IsConstructor)
            {
                if ((md.Attributes & MethodAttributes.Public) > 0)
                {
                    if (md.IsStatic)
                        node.ImageIndex = 54;
                    else
                        node.ImageIndex = 48;
                }
                else
                {
                    if (md.IsStatic)
                        node.ImageIndex = 57;
                    else
                        node.ImageIndex = 51;
                }
            }
            else
            {
                if ((md.Attributes & MethodAttributes.Public) > 0)
                {
                    if (md.IsStatic)
                        node.ImageIndex = 66;
                    else
                        node.ImageIndex = 60;
                }
                else
                {
                    if (md.IsStatic)
                        node.ImageIndex = 69;
                    else
                        node.ImageIndex = 63;
                }
            }
            node.SelectedImageIndex = node.ImageIndex;
            parentNode.Nodes.Add(node);
            
            return node;
        }

        private void AddPropertyNode(TreeNode parentNode, PropertyDefinition pd)
        {
            TreeNode node = CreateTreeNode(InsUtils.GetPropertyText(pd));
            node.Tag = pd;
            node.ImageIndex = 114;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = TokenUtils.GetFullMetadataTokenString(pd.MetadataToken);

            parentNode.Nodes.Add(node);

            bool showStaticOnly = _form.ShowStaticOnly;
            if (pd.GetMethod != null)
            {
                if (!showStaticOnly || (showStaticOnly && pd.GetMethod.IsStatic))
                {
                    TreeNode node1 = CreateTreeNode(InsUtils.GetMethodText(pd.GetMethod));
                    node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(pd.GetMethod.MetadataToken);
                    node1.Tag = pd.GetMethod;
                    if ((pd.GetMethod.Attributes & MethodAttributes.Public) > 0)
                    {
                        if (pd.GetMethod.IsStatic)
                            node1.ImageIndex = 78;
                        else
                            node1.ImageIndex = 72;
                    }
                    else
                    {
                        if (pd.GetMethod.IsStatic)
                            node1.ImageIndex = 81;
                        else
                            node1.ImageIndex = 75;
                    }
                    node1.SelectedImageIndex = node1.ImageIndex;
                    node.Nodes.Add(node1);
                }
            }
            if (pd.SetMethod != null)
            {
                if (!showStaticOnly || (showStaticOnly && pd.SetMethod.IsStatic))
                {
                    TreeNode node1 = CreateTreeNode(InsUtils.GetMethodText(pd.SetMethod));
                    node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(pd.SetMethod.MetadataToken);
                    node1.Tag = pd.SetMethod;
                    if ((pd.SetMethod.Attributes & MethodAttributes.Public) > 0)
                    {
                        if (pd.SetMethod.IsStatic)
                            node1.ImageIndex = 78;
                        else
                            node1.ImageIndex = 72;
                    }
                    else
                    {
                        if (pd.SetMethod.IsStatic)
                            node1.ImageIndex = 81;
                        else
                            node1.ImageIndex = 75;
                    }
                    node1.SelectedImageIndex = node1.ImageIndex;
                    node.Nodes.Add(node1);
                }
            }
        }

        private void AddFieldNode(TreeNode parentNode, FieldDefinition fd)
        {
            TreeNode node = CreateTreeNode(InsUtils.GetFieldText(fd));
            node.Tag = fd;

            if ((fd.Attributes & Mono.Cecil.FieldAttributes.HasFieldRVA) != 0)
            {
                node.ToolTipText = String.Format("{0} at D_{1:x08}\r\n{2}",
                    TokenUtils.GetFullMetadataTokenString(fd.MetadataToken),
                    fd.RVA,
                    BytesUtils.BytesToHexStringBlock(fd.InitialValue));
            }
            else
            {
                node.ToolTipText = TokenUtils.GetFullMetadataTokenString(fd.MetadataToken);
            }

            if ((fd.Attributes & Mono.Cecil.FieldAttributes.Public) > 0)
            {
                if (fd.IsStatic)
                    node.ImageIndex = 90;
                else
                    node.ImageIndex = 84;
            }
            else
            {
                if (fd.IsStatic)
                    node.ImageIndex = 93;
                else
                    node.ImageIndex = 87;
            }
            node.SelectedImageIndex = node.ImageIndex;
            parentNode.Nodes.Add(node);
        }

        private void AddEventNode(TreeNode parentNode, EventDefinition ed)
        {
            TreeNode node = CreateTreeNode(InsUtils.GetEventText(ed));
            node.Tag = ed;
            node.ImageIndex = 138;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = TokenUtils.GetFullMetadataTokenString(ed.MetadataToken);

            parentNode.Nodes.Add(node);

            if (ed.AddMethod != null)
            {
                TreeNode node1 = CreateTreeNode(InsUtils.GetMethodText(ed.AddMethod));
                node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(ed.AddMethod.MetadataToken);
                node1.Tag = ed.AddMethod;
                if ((ed.AddMethod.Attributes & MethodAttributes.Public) > 0)
                {
                    if (ed.AddMethod.IsStatic)
                        node1.ImageIndex = 78;
                    else
                        node1.ImageIndex = 72;
                }
                else
                {
                    if (ed.AddMethod.IsStatic)
                        node1.ImageIndex = 81;
                    else
                        node1.ImageIndex = 75;
                }
                node1.SelectedImageIndex = node1.ImageIndex;
                node.Nodes.Add(node1);
            }
            if (ed.RemoveMethod != null)
            {
                TreeNode node1 = CreateTreeNode(InsUtils.GetMethodText(ed.RemoveMethod));
                node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(ed.RemoveMethod.MetadataToken);
                node1.Tag = ed.RemoveMethod;
                if ((ed.RemoveMethod.Attributes & MethodAttributes.Public) > 0)
                {
                    if (ed.RemoveMethod.IsStatic)
                        node1.ImageIndex = 78;
                    else
                        node1.ImageIndex = 72;
                }
                else
                {
                    if (ed.RemoveMethod.IsStatic)
                        node1.ImageIndex = 81;
                    else
                        node1.ImageIndex = 75;
                }
                node1.SelectedImageIndex = node1.ImageIndex;
                node.Nodes.Add(node1);
            }
            if (ed.InvokeMethod != null)
            {
                TreeNode node1 = CreateTreeNode(InsUtils.GetMethodText(ed.InvokeMethod));
                node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(ed.InvokeMethod.MetadataToken);
                node1.Tag = ed.InvokeMethod;
                if ((ed.InvokeMethod.Attributes & MethodAttributes.Public) > 0)
                {
                    if (ed.InvokeMethod.IsStatic)
                        node1.ImageIndex = 78;
                    else
                        node1.ImageIndex = 72;
                }
                else
                {
                    if (ed.InvokeMethod.IsStatic)
                        node1.ImageIndex = 81;
                    else
                        node1.ImageIndex = 75;
                }
                node1.SelectedImageIndex = node1.ImageIndex;
                node.Nodes.Add(node1);
            }
        }

        public bool IsBaseTypeNode(TreeNode n)
        {
            if (n == null || n.Parent == null)
                return false;
            return "Base Types" == GetTreeNodeText(n) || "Base Types" == GetTreeNodeText(n.Parent);
        }

        private void AddBaseTypesNode(TreeNode parentNode, TypeDefinition type)
        {
            TreeNode baseType = CreateTreeNode("Base Types");
            baseType.ImageIndex = 162;
            baseType.SelectedImageIndex = baseType.ImageIndex;
            parentNode.Nodes.Add(baseType);

            if (type.BaseType != null)
            {
                TreeNode node1 = CreateTreeNode(InsUtils.GetTypeText(type.BaseType));
                node1.Tag = type.BaseType.Name; // set name but not type reference here to bypass search and expand
                node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(type.BaseType.MetadataToken);
                node1.ImageIndex = 6;
                node1.SelectedImageIndex = node1.ImageIndex;
                baseType.Nodes.Add(node1);
            }

            //interfaces
            foreach (TypeReference tr in type.Interfaces)
            {
                TreeNode node = CreateTreeNode(InsUtils.GetTypeText(tr));
                node.Tag = tr.Name; // set name but not type reference here to bypass search and expand
                node.ToolTipText = TokenUtils.GetFullMetadataTokenString(tr.MetadataToken);

                node.ImageIndex = 12;
                node.SelectedImageIndex = node.ImageIndex;
                baseType.Nodes.Add(node);
            }

        }

        public bool IsDerivedTypeNode(TreeNode n)
        {
            if (n == null || n.Parent == null)
                return false;
            return "Derived Types" == GetTreeNodeText(n) || "Derived Types" == GetTreeNodeText(n.Parent);
        }

        private void AddDerivedTypesNode(TreeNode parentNode, TypeDefinition type)
        {
            AssemblyDefinition ad = GetCurrentAssembly();
            if (ad == null) return;
            Dictionary<string, AssemblyDefinition> assemblies = new Dictionary<string, AssemblyDefinition>();
            assemblies.Add(ad.FullName, ad);

            List<TypeDefinition> list = DeobfUtils.FindDerivedTypes(type, 0, assemblies);
            if (list.Count > 0)
            {
                TreeNode derivedType = CreateTreeNode("Derived Types");
                derivedType.ImageIndex = 162;
                derivedType.SelectedImageIndex = derivedType.ImageIndex;
                parentNode.Nodes.Add(derivedType);

                foreach (TypeDefinition td in list)
                {
                    TreeNode node1 = CreateTreeNode(InsUtils.GetTypeText(td));
                    node1.Tag = td.Name; // set name but not type reference here to bypass search and expand
                    node1.ToolTipText = TokenUtils.GetFullMetadataTokenString(td.MetadataToken);
                    node1.ImageIndex = 6;
                    node1.SelectedImageIndex = node1.ImageIndex;
                    derivedType.Nodes.Add(node1);
                }
            }
        }

        private void AddTypeNodes(TreeNode parentNode, TypeDefinition td)
        {
            if (td == null) return;

            bool showStaticOnly = _form.ShowStaticOnly;
            ObjectTypes objectType = this.ObjectType;

            if (showStaticOnly)
            {
                if (td.IsEnum || td.IsInterface) return;
            }

            //base type
            if ((td.BaseType != null || td.Interfaces.Count > 0) && !showStaticOnly)
            {
                AddBaseTypesNode(parentNode, td);
            }

            if (!showStaticOnly)
            {
                AddDerivedTypesNode(parentNode, td);
            }

            //nested classes
            if (td.NestedTypes.Count > 0)
            {
                foreach (TypeDefinition td1 in td.NestedTypes)
                {
                    if (showStaticOnly)
                    {
                        if (td.IsEnum || td.IsInterface) continue;
                    }

                    AddTypeNode(parentNode, td1);
                }
            }

            //constructors
            if ((objectType & ObjectTypes.Constructor) > 0)
            {
                foreach (MethodDefinition md in td.Methods)
                {
                    if (!md.IsConstructor) continue;
                    if (showStaticOnly && !md.IsStatic) continue;
                    AddMethodNode(parentNode, md);
                }
            }
            //methods
            if ((objectType & ObjectTypes.Method) > 0)
            {
                foreach (MethodDefinition md in td.Methods)
                {
                    if (md.IsConstructor || md.IsGetter || md.IsSetter || md.IsAddOn || md.IsRemoveOn || md.IsFire)
                        continue;
                    if (showStaticOnly && !md.IsStatic) continue;

                    AddMethodNode(parentNode, md);
                }
            }
            //properties
            if ((objectType & ObjectTypes.Property) > 0)
            {
                foreach (PropertyDefinition pd in td.Properties)
                {
                    if (showStaticOnly)
                    {
                        if ((pd.GetMethod != null && pd.GetMethod.IsStatic) || (pd.SetMethod != null && pd.SetMethod.IsStatic))
                        {
                            AddPropertyNode(parentNode, pd);
                        }
                    }
                    else
                    {
                        AddPropertyNode(parentNode, pd);
                    }
                }
            }
            //fields
            if ((objectType & ObjectTypes.Field) > 0 && !showStaticOnly)
            {
                foreach (FieldDefinition fd in td.Fields)
                {
                    AddFieldNode(parentNode, fd);
                }
            }
            //events
            if ((objectType & ObjectTypes.Event) > 0 && !showStaticOnly)
            {
                foreach (EventDefinition ed in td.Events)
                {
                    AddEventNode(parentNode, ed);
                }
            }
        }

        private void AddModuleNode(TreeNode parentNode, ModuleDefinition md)
        {
            TreeNode node = CreateTreeNode(md.Name);
            node.Tag = md;
            node.ImageIndex = 152;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = TokenUtils.GetFullMetadataTokenString(md.MetadataToken);
            parentNode.Nodes.Add(node);
        }

        private void AddReferencesNode(TreeNode parentNode, ModuleDefinition md)
        {
            TreeNode refs = CreateTreeNode("References");
            refs.ImageIndex = 154;
            refs.SelectedImageIndex = refs.ImageIndex;
            parentNode.Nodes.Add(refs);

            foreach (AssemblyNameReference anr in md.AssemblyReferences)
            {
                TreeNode node = CreateTreeNode(anr.Name);
                node.Tag = anr;
                node.ImageIndex = 151;
                node.SelectedImageIndex = node.ImageIndex;
                node.ToolTipText = TokenUtils.GetFullMetadataTokenString(anr.MetadataToken);
                refs.Nodes.Add(node);
            }

            foreach (ModuleReference mr in md.ModuleReferences)
            {
                TreeNode node = CreateTreeNode(mr.Name);
                node.Tag = mr;
                node.ImageIndex = 153;
                node.SelectedImageIndex = node.ImageIndex;
                node.ToolTipText = TokenUtils.GetFullMetadataTokenString(mr.MetadataToken);
                refs.Nodes.Add(node);
            }
        }

        private TreeNode AddNameSpaceNode(TreeNode parentNode, string nameSpace)
        {
            TreeNode nodeNameSpace = CreateTreeNode(nameSpace);
            nodeNameSpace.ImageIndex = 0;
            nodeNameSpace.SelectedImageIndex = nodeNameSpace.ImageIndex;
            nodeNameSpace.Tag = new TreeNodeSorter.NameSpace(nameSpace);
            parentNode.Nodes.Add(nodeNameSpace);
            return nodeNameSpace;
        }

        private void AddResourcesNode(TreeNode parentNode, ModuleDefinition md)
        {
            TreeNode res = CreateTreeNode("Resources");
            res.ImageIndex = 155;
            res.SelectedImageIndex = res.ImageIndex;
            res.Tag = new TreeNodeSorter.Resources();
            parentNode.Nodes.Add(res);

            foreach (Resource r in md.Resources)
            {
                TreeNode node = CreateTreeNode(r.Name);
                node.Tag = r;
                node.ImageIndex = 157;
                node.SelectedImageIndex = node.ImageIndex;
                res.Nodes.Add(node);
            }
        }

        public AssemblyDefinition GetCurrentAssembly()
        {
            AssemblyDefinition ad = null;
            MethodDefinition method = _form.BodyGridHandler.CurrentMethod;
            if (method != null)
            {
                ad = method.DeclaringType.Module.Assembly;
            }
            else
            {
                TreeNode n = treeView1.SelectedNode;
                if (n != null)
                {
                    while (n.Parent != null) n = n.Parent;
                    if (n.Tag is AssemblyDefinition)
                        ad = n.Tag as AssemblyDefinition;
                }
            }
            return ad;
        }

        public TreeNode FindTreeNode(string searchFor)
        {
            using (new SimpleWaitCursor())
            {
                //_host.SetStatusText(String.Format("Searching: {0}", searchFor));

                TreeNode foundNode = null;

                //try to start search from current node
                //if current node is null, start from root
                TreeNode current = treeView1.SelectedNode;
                TreeNodeCollection tnc;
                if (current != null)
                {
                    tnc = current.Nodes;
                }
                else
                {
                    tnc = treeView1.Nodes;
                }

                //for string search, first try to search in current method
                if (_form.SearchHandler.SearchType == ClassEditSearchHandler.SearchTypes.String && current != null)
                {
                    foundNode = FindTreeNode(current, searchFor);
                    if (foundNode != null && foundNode.Equals(current))
                    {
                        //found in same method, just jmp to instruction
                        //node select event not fired
                        _form.BodyGridHandler.SetCurrentRow(_form.BodyGridHandler.CurrentRowIndex);
                        _form.BodyGridHandler.CurrentRowIndex = -1;

                        return foundNode;
                    }
                    if (foundNode != null) return foundNode;
                }

                //search for each sub-node
                foreach (TreeNode n in tnc)
                {
                    foundNode = FindTreeNode(n, searchFor);
                    if (foundNode != null) return foundNode;
                }

                if (current != null)
                {
                    //search for next node
                    while (current != null)
                    {
                        TreeNode nextNode = current.NextNode;
                        while (nextNode != null)
                        {
                            foundNode = FindTreeNode(nextNode, searchFor);
                            if (foundNode != null) return foundNode;
                            nextNode = nextNode.NextNode;
                        }
                        current = current.Parent;
                    }

                    //nothing found, restart from root
                    treeView1.SelectedNode = null;
                    foundNode = FindTreeNode(searchFor);
                    if (foundNode != null) return foundNode;
                }
            }
            return null;
        }

        private bool IsMatchedMemeber(TreeNode node, string searchFor)
        {
            if (node.Text.StartsWith(searchFor, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (searchFor.StartsWith("0x") && node.ToolTipText.StartsWith(searchFor.Substring(2), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public TreeNode FindTreeNode(TreeNode node, string searchFor)
        {
            InitNode(node);

            ClassEditSearchHandler.SearchTypes searchType = _form.SearchHandler.SearchType;
            if (searchType == ClassEditSearchHandler.SearchTypes.Name)
            {
                if (node.Text.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return node;
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.String)
            {
                if (node.Tag is MethodDefinition)
                {
                    MethodDefinition md = (MethodDefinition)node.Tag;
                    if (md.HasBody)
                    {
                        Collection<Instruction> ic = md.Body.Instructions;
                        int start = 0;
                        if (md == _form.BodyGridHandler.CurrentMethod && _form.BodyDataGrid.CurrentRow != null)
                        {
                            start = _form.BodyDataGrid.CurrentRow.Index; // start from next row
                        }
                        for (int i = start; i < ic.Count; i++)
                        {
                            if (ic[i].OpCode.OperandType != OperandType.InlineString)
                                continue;
                            string op = ic[i].Operand as string;
                            if (op == null) continue;
                            if (op.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                _form.BodyGridHandler.CurrentRowIndex = i + 1; // scroll to correct instruction after refresh
                                return node;
                            }
                        }
                    }
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.ClassName)
            {
                if (node.Tag is TypeDefinition && IsMatchedMemeber(node, searchFor))
                {
                    return node;
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.MethodName)
            {
                if (node.Tag is MethodDefinition && IsMatchedMemeber(node, searchFor))
                {
                    return node;
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.PropertyName)
            {
                if (node.Tag is PropertyDefinition && IsMatchedMemeber(node, searchFor))
                {
                    return node;
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.FieldName)
            {
                if (node.Tag is FieldDefinition && IsMatchedMemeber(node, searchFor))
                {
                    return node;
                }
            }
            else if (searchType == ClassEditSearchHandler.SearchTypes.EventName)
            {
                if (node.Tag is EventDefinition && IsMatchedMemeber(node, searchFor))
                {
                    return node;
                }
            }

            foreach (TreeNode n in node.Nodes)
            {
                TreeNode n2 = FindTreeNode(n, searchFor);
                if (n2 != null) return n2;
            }

            return null;
        }

        public void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private bool IsValidName(string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z_]{1}[0-9a-zA-Z_$.]*$"))
            {
                return true;
            }
            return false;
        }

        public void treeView1_DoubleClick(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (IsBaseTypeNode(n) || IsDerivedTypeNode(n))
            {
                string name = n.Tag as string;
                if (String.IsNullOrEmpty(name) || name == "Object")
                    return;

                string searchFor;
                if (n.ToolTipText.StartsWith("02")) // TypeDefinition
                {
                    if (IsValidName(name))
                        searchFor = name;
                    else
                        searchFor = "0x" + n.ToolTipText;
                }
                else
                {
                    searchFor = name;
                }

                _form.SearchTypeComboBox.SelectedIndex = (int)ClassEditSearchHandler.SearchTypes.ClassName;
                _form.SearchTextComboBox.Text = searchFor;
                _form.SearchHandler.cboSearch_SelectedIndexChanged(sender, e);
            }
        }

        public void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (!CanBeRenamed(n))
            {
                e.CancelEdit = true;
                return;
            }

        }

        public void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            if (!CanBeRenamed(e.Node))
            {
                e.CancelEdit = true;
                return;
            }

            RenameNode(e.Node, e.Label);
            e.CancelEdit = true;
            return;
        }

        public bool IsResourceTypeNode(TreeNode n)
        {
            if (n == null || n.Parent == null)
                return false;
            return "Resources" == GetTreeNodeText(n) || "Resources" == GetTreeNodeText(n.Parent);
        }

        public bool CanBeRenamed(TreeNode n)
        {
            return n.Tag is MemberReference || n.Tag is Resource ||
                n.Tag is TreeNodeSorter.NameSpace;
        }

        private void RenameNode(TreeNode node, string newName)
        {
            if (node.Tag is MemberReference)
                RenameMemberReference(node, newName);
            else if (node.Tag is Resource)
                RenameResource(node, newName);
            else if (node.Tag is TreeNodeSorter.NameSpace)
                RenameNameSpace(node, newName);
        }

        private void RenameMemberReference(TreeNode node, string newName)
        {
            MemberReference mr = (MemberReference)node.Tag;
            if (!InsUtils.OldMemberNameExists(mr))
            {
                InsUtils.SaveOldMemberName(mr);
                _renamedObjects.Add(mr);
            }

            string oldName = mr.ToString();
            mr.Name = newName;
            _form.LogHandler.LogRename(oldName, mr.ToString());
            node.Text = InsUtils.GetMemberText(mr);
        }

        private void RenameResource(TreeNode node, string newName)
        {
            Resource r = (Resource)node.Tag;
            string oldName = r.Name;
            r.Name = newName;
            _form.LogHandler.LogRename(oldName, r.Name);
            node.Text = r.Name;
        }

        private void RenameNameSpace(TypeDefinition type, string newName)
        {
            type.Namespace = newName;
            if (type.HasNestedTypes)
            {
                foreach (TypeDefinition td in type.NestedTypes)
                {
                    RenameNameSpace(td, newName);
                }
            }
        }

        private void RenameNameSpace(TreeNode node, string newName)
        {
            node.Text = newName;
            foreach (TreeNode n in node.Nodes)
            {
                if (n.Tag is TypeDefinition)
                {
                    RenameNameSpace((TypeDefinition)n.Tag, newName);
                }
            }
        }

        private void ExpandOrCollapseAll(TreeNode node, bool isExpand, int levels)
        {
            if (node == null)
                return;

            DateTime startTime = DateTime.Now;

            try
            {
                // seems only a little useful on first expand, then slow down later operations
                //treeView1.BeginUpdate();
                //treeView1.SuspendLayout();

                ExpandOrCollapseAllRecursive(node, isExpand, levels);
            }
            catch
            {
                throw;
            }
            finally
            {
                node.EnsureVisible();

                //treeView1.EndUpdate();
                //treeView1.ResumeLayout();
            }

            DateTime endTime = DateTime.Now;
            _form.SetStatusText(String.Format("TreeView loaded in {0} seconds", (endTime - startTime).TotalSeconds));
        }

        private void ExpandOrCollapse(TreeNode node, bool isExpand)
        {
            if (isExpand)
            {
                if (!node.IsExpanded)
                {
                    node.Expand();
                }
            }
            else
            {
                if (node.IsExpanded)
                {
                    node.Collapse(true);
                }
            }
        }

        private void ExpandOrCollapseAllRecursive(TreeNode node, bool isExpand, int levels)
        {
            int nextLevels = levels > 0 ? levels - 1 : levels;
            if (isExpand)
            {
                InitNode(node);
                if (nextLevels != 0)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        ExpandOrCollapseAllRecursive(n, isExpand, nextLevels);
                    }
                }
                ExpandOrCollapse(node, isExpand);
            }
            else
            {
                ExpandOrCollapse(node, isExpand);
                if (nextLevels != 0)
                {
                    foreach (TreeNode n in node.Nodes)
                    {
                        ExpandOrCollapseAllRecursive(n, isExpand, nextLevels);
                    }
                }
            }
        }

        public void cmExpand_Click(object sender, EventArgs e)
        {
            TreeNode node = _form.TreeViewHandler.SelectedNode;
            ExpandOrCollapseAll(node, true, 2);
        }

        public void cmCollapse_Click(object sender, EventArgs e)
        {
            TreeNode node = _form.TreeViewHandler.SelectedNode;
            ExpandOrCollapseAll(node, false, 2);
        }

        public void cmExpandAll_Click(object sender, EventArgs e)
        {
            TreeNode node = _form.TreeViewHandler.SelectedNode;
            ExpandOrCollapseAll(node, true, -1);
        }

        public void cmCollapseAll_Click(object sender, EventArgs e)
        {
            TreeNode node = _form.TreeViewHandler.SelectedNode;
            ExpandOrCollapseAll(node, false, -1);
        }

        public void cmCopyName_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null) return;
            Clipboard.SetText(GetTreeNodeText(n));
        }

        public void cmCopyNameAsHex_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null) return;
            string nText = GetTreeNodeText(n);
            if (!String.IsNullOrEmpty(nText))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(nText);
                Clipboard.SetText(BytesUtils.BytesToHexString(bytes, true));
            }
        }

        public void cmRename_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null) return;
            if (!CanBeRenamed(n))
                return;

            frmClassEditRename f = new frmClassEditRename(n.Tag);
            if (f.ShowDialog() == DialogResult.OK)
            {
                RenameNode(n, f.NewName);
            }
        }

        public void cmCustomAttributes_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null) return;
            ICustomAttributeProvider cap = n.Tag as ICustomAttributeProvider;
            if (cap == null) return;

            bool resolveDirAdded = false;
            try
            {
                resolveDirAdded = _form.Host.AddAssemblyResolveDir(_form.SourceDir);

                foreach (CustomAttribute ca in cap.CustomAttributes)
                {
                    if (ca.IsResolvable && ca.HasFields) continue; //ensure resolved
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (resolveDirAdded)
                    _form.Host.RemoveAssemblyResolveDir(_form.SourceDir);
            }

            frmClassEditCustomAttributes f = new frmClassEditCustomAttributes(cap);
            if (f.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private MethodDefinition GetEntryPoint(object tag)
        {
            MethodDefinition entryPoint = null;
            if (tag is AssemblyDefinition)
            {
                entryPoint = ((AssemblyDefinition)tag).EntryPoint;
            }
            else if (tag is ModuleDefinition)
            {
                entryPoint = ((ModuleDefinition)tag).EntryPoint;
            }
            return entryPoint;
        }

        public void cmGotoEntryPoint_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null) return;

            MethodDefinition entryPoint = GetEntryPoint(n.Tag);
            if (entryPoint != null)
            {
                TreeNode entryNode = FindTreeNode(entryPoint);
                if (entryNode != null)
                {
                    treeView1.SelectedNode = entryNode;
                }
            }
        }

        private void SaveResourceFile(EmbeddedResource er, string outputFile)
        {
            byte[] bytes = er.GetResourceData();
            using (FileStream fs = File.Create(outputFile))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            _form.SetStatusText(String.Format("{0} saved.", outputFile));

            if (PathUtils.IsResourceExt(outputFile))
            {
                using (ResourceReader rr = new ResourceReader(er.GetResourceStream()))
                {
                    string resxFile = Path.ChangeExtension(outputFile, ".resx");
                    using (ResXResourceWriter xw = new ResXResourceWriter(resxFile))
                    {
                        IDictionaryEnumerator de = rr.GetEnumerator();
                        while (de.MoveNext())
                        {
                            bool handled = false;
                            if (de.Value != null)
                            {
                                Type type = de.Value.GetType();
                                if (type.FullName.EndsWith("Stream"))
                                {
                                    Stream s = de.Value as Stream;
                                    if (s != null)
                                    {
                                        byte[] tmpBytes = new byte[s.Length];
                                        if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
                                        s.Read(tmpBytes, 0, tmpBytes.Length);
                                        xw.AddResource(de.Key.ToString(), new MemoryStream(tmpBytes));
                                        handled = true;
                                    }
                                }
                            }
                            if (handled) continue;
                            xw.AddResource(de.Key.ToString(), de.Value);
                        }
                    }
                }
            }
            else if (PathUtils.IsBamlExt(outputFile))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(Path.ChangeExtension(outputFile, ".xaml")))
                    {
                        sw.WriteLine(_form.ResourceHandler.DecompileBaml(new MemoryStream(bytes)));
                    }
                }
                catch (Exception ex)
                {
                    _form.SetStatusText(String.Format("Failed to translate {0}: {1}", er.Name, ex.Message));
                }
            }
        }

        public string GetOutputResourceFileName(string resourceName)
        {
            if (PathUtils.IsValidFileName(resourceName))
                return resourceName;
            else
                return PathUtils.FixFileName(resourceName);
        }

        public void cmSaveResourceAs_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node.Tag is EmbeddedResource)
            {
                EmbeddedResource er = (EmbeddedResource)node.Tag;

                string initDir = Config.ClassEditorSaveAsDir;
                string defaultFileName = GetOutputResourceFileName(er.Name);
                if (!Directory.Exists(initDir))
                    initDir = Environment.CurrentDirectory;

                //string ext = Path.GetExtension(er.Name);
                string ext = null;
                int p = er.Name.LastIndexOf(".");
                if (p > 0) ext = er.Name.Substring(p);
                if (String.IsNullOrEmpty(ext))
                    ext = ".*";

                string fileFilter = String.Format("Resource files (*{0})|*{0}", ext);
                //defaultFileName = FileUtils.FixFileName(defaultFileName);
                string path = SimpleDialog.OpenFile("Save Embedded Resource",
                    fileFilter,
                    ext == ".*" ? "" : ext,
                    false,
                    initDir,
                    defaultFileName);

                if (String.IsNullOrEmpty(path))
                {
                    return;
                }
                else
                {
                    Config.ClassEditorSaveAsDir = Path.GetDirectoryName(path);
                }
                
                SaveResourceFile(er, path);
                return;
            }

            if (IsResourceTypeNode(node) && !(node.Tag is Resource))
            {
                string initDir = Config.ClassEditorSaveAsDir;
                if (!Directory.Exists(initDir))
                    initDir = Environment.CurrentDirectory;
                string path = SimpleDialog.OpenFolder(initDir);
                if (!String.IsNullOrEmpty(path))
                {
                    Config.ClassEditorSaveAsDir = path;
                }
                if (String.IsNullOrEmpty(path)) return;

                int count = 0;
                foreach (TreeNode n in node.Nodes)
                {
                    if (n.Tag is EmbeddedResource)
                    {
                        EmbeddedResource er = (EmbeddedResource)n.Tag;
                        string outputFile = Path.Combine(path, GetOutputResourceFileName(er.Name));
                        SaveResourceFile(er, outputFile);
                        count++;
                    }
                    _form.SetStatusText(String.Format("{0} file(s) saved.", count));
                }
                return;
            }

        }

        public string[] SelectResourceFile()
        {
            string initDir = Config.ClassEditorSaveAsDir;
            string defaultFileName = null;
            if (!Directory.Exists(initDir))
                initDir = Environment.CurrentDirectory;

            string fileFilter = "All files (*.*)|*.*";
            SimpleDialog.OpenFile("Import Resource",
                fileFilter,
                String.Empty,
                true,
                initDir,
                defaultFileName,
                true);

            return SimpleDialog.FileNames;
        }

        public void cmImportResourceFromFile_Click(object sender, EventArgs e)
        {
            var fileNames = SelectResourceFile();

            if (fileNames == null || fileNames.Length == 0)
                return;

            TreeNode node = null;
            EmbeddedResource newEr = null;

            var resourceNode = _form.TreeView.SelectedNode;
            if (IsResourceTypeNode(resourceNode) && !(resourceNode.Tag is Resource))
            {
                //this is resource root
            }
            else if (resourceNode.Tag is Resource)
            {
                resourceNode = resourceNode.Parent;
            }

            var ad = _form.TreeViewHandler.GetCurrentAssembly();

            foreach (string path in fileNames)
            {
                if (String.IsNullOrEmpty(path))
                    continue;

                node = null;
                newEr = null;

                string ext = Path.GetExtension(path);
                byte[] bytes = null;
                if (ext == ".resx")
                {
                    var xr = new ResXResourceReader(path);
                    var ms = new MemoryStream();
                    var rw = new ResourceWriter(ms);
                    var de = xr.GetEnumerator();
                    while (de.MoveNext())
                    {
                        rw.AddResource((string)de.Key, de.Value);
                    }
                    rw.Generate();
                    bytes = ms.ToArray();
                    rw.Close();
                    ms.Close();
                    xr.Close();
                }
                else
                {
                    bytes = File.ReadAllBytes(path);
                }

                if (bytes == null)
                    continue;

                string name = Path.GetFileName(path);
                if (ext == ".resx")
                {
                    name = Path.ChangeExtension(name, ".resources");
                }

                foreach (TreeNode n in resourceNode.Nodes)
                {
                    if (name.Equals(_form.TreeViewHandler.GetTreeNodeText(n), StringComparison.Ordinal))
                    {
                        node = n;
                        break;
                    }
                }

                EmbeddedResource er = (node == null ? null : (EmbeddedResource)node.Tag);
                newEr = new EmbeddedResource(name, er == null ? ManifestResourceAttributes.Public : er.Attributes, bytes);

                if (er == null)
                {
                    node = CreateTreeNode(name);
                    ad.MainModule.Resources.Add(newEr);
                    node.Tag = newEr;
                    node.ImageIndex = 157;
                    node.SelectedImageIndex = node.ImageIndex;
                    resourceNode.Nodes.Add(node);
                }
                else
                {
                    ad.MainModule.Resources.Remove(er);
                    ad.MainModule.Resources.Add(newEr);
                    node.Tag = newEr;
                }
            }

            if (node != null && newEr != null)
            {
                if (_form.TreeView.SelectedNode == node)
                {
                    _form.ResourceHandler.InitResource(newEr);
                }
                else
                {
                    _form.TreeView.SelectedNode = node;
                }
            }            
        }

        public void cmRemoveResource_Click(object sender, EventArgs e)
        {
            var resourceNode = _form.TreeView.SelectedNode;
            Resource r = resourceNode.Tag as Resource;
            if (r == null)
                return;

            var ad = _form.TreeViewHandler.GetCurrentAssembly();
            ad.MainModule.Resources.Remove(r);
            resourceNode.Tag = null;
            //_form.TreeView.SelectedNode = resourceNode.Parent;
            resourceNode.Parent.Nodes.Remove(resourceNode);
        }

        private void ShowResourceNode(TreeNode node)
        {
            _form.BodyGridHandler.InitBody(null);
            _form.ResourceHandler.InitResource((Resource)node.Tag);
        }

        public void cmViewResourceAsNormal_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            ShowResourceNode(node);
        }

        public void cmViewResourceAsBinary_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node.Tag is EmbeddedResource)
            {
                EmbeddedResource er = (EmbeddedResource)node.Tag;
                _form.ResourceHandler.ShowBinary(er, false);
            }
        }

        public void cmAssembly_Opening(object sender, CancelEventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null || n.Tag == null || IsBaseTypeNode(n) || IsDerivedTypeNode(n))
            {
                e.Cancel = true;
                return;
            }

            ContextMenuStrip cms = sender as ContextMenuStrip;

            cms.Items["cmRename"].Enabled = CanBeRenamed(n);
            cms.Items["cmCopyName"].Enabled = (n != null);

            cms.Items["cmExpand"].Enabled = (n != null);
            cms.Items["cmCollapse"].Enabled = (n != null);
            cms.Items["cmExpandAll"].Enabled = (n != null);
            cms.Items["cmCollapseAll"].Enabled = (n != null);

            cms.Items["cmGotoEntryPoint"].Enabled = GetEntryPoint(n.Tag) != null;

            cms.Items["cmCustomAttributes"].Enabled = (n.Tag is ICustomAttributeProvider) && ((ICustomAttributeProvider)n.Tag).HasCustomAttributes;
            cms.Items["cmViewInReflector"].Enabled = (n.Tag != null && !AssemblyUtils.IsInternalType(n.Tag.GetType()));

            cms.Items["cmImportMethodFromAssembly"].Enabled = (n.Tag is TypeDefinition);
            cms.Items["cmReadMethodFromAssembly"].Enabled = (n.Tag is MethodDefinition);
            cms.Items["cmReadMethodFromFile"].Enabled = (n.Tag is MethodDefinition);
            cms.Items["cmRestoreMethodFromImage"].Enabled = (n.Tag is MethodDefinition);
            cms.Items["cmWriteMethodToFile"].Enabled = (n.Tag is MethodDefinition);
            cms.Items["cmReadMethodsFromFolder"].Enabled = (n.Tag is ModuleDefinition);			

            cms.Items["cmSaveResourceAs"].Enabled = IsResourceTypeNode(n);
            cms.Items["cmViewResourceAsBinary"].Enabled = (n.Tag is EmbeddedResource);
            cms.Items["cmViewResourceAsNormal"].Enabled = (n.Tag is EmbeddedResource);
            cms.Items["cmImportResourceFromFile"].Enabled = IsResourceTypeNode(n);
            cms.Items["cmRemoveResource"].Enabled = (n.Tag is Resource);

            if (_form.BookmarkHandler.FindBookmark(n) == null)
            {
                cms.Items["cmBookmark"].Text = "Bookmark";
            }
            else
            {
                cms.Items["cmBookmark"].Text = "Remove Bookmark";
            }
        }

        public string FindAssemblyFile(TreeNode n)
        {
            TreeNode top = FindTopNode(n);
            string assemblyFile = _form.ReflectorHandler.GetAssemblyPath(top.Index);
            if (!String.IsNullOrEmpty(assemblyFile) && File.Exists(assemblyFile))
                return assemblyFile;
            return top.FullPath;
        }

        public void cmViewInReflector_Click(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            if (n == null || n.Tag == null)
                return;

            _form.ReflectorHandler.Init();

            object o = _form.ReflectorHandler.Cecil2Reflector(n.Tag);
            if (o == null) return;

            CodeIdentifier ci = new CodeIdentifier(o);
            string path = Config.Reflector;
            if (!File.Exists(path))
            {
                path = SimpleReflector.OpenReflector();
            }
            if (!File.Exists(path)) return;

            try
            {
                string assemblyFile = FindAssemblyFile(n);

                if (RemoteController.Available)
                {
                    RemoteController.LoadAssembly(assemblyFile);
                    RemoteController.Select(ci.Identifier);
                }
                else
                {
                    Process p = new Process();
                    p.StartInfo.FileName = path;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);

                    p.StartInfo.Arguments = String.Format("/select:{0} \"{1}\"", ci.Identifier, assemblyFile);
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        #region Read Method From File, contributed by HaRpY
        public void cmWriteMethodToFile_Click(object sender, EventArgs e)
        {
            MethodDefinition md = treeView1.SelectedNode.Tag as MethodDefinition;
            if (md == null) return;

            string initBinDir = Config.LastBinDir;
            if (!Directory.Exists(initBinDir))
                initBinDir = Environment.CurrentDirectory;
            string file = String.Format("{0}.bin",String.Format("{0:X}", md.MetadataToken.ToUInt32()).PadLeft(8, '0'));
            file = SimpleDialog.OpenFile("Save Binary File", "Binary File (*.bin)|*.bin|All Files(*.*)|*.*", "", false, initBinDir,file);
            if (!String.IsNullOrEmpty(file))
            {
                if (File.Exists(file)) File.Delete(file);
                WriteMethodToFile(md,file);
                Config.LastBinDir = Path.GetDirectoryName(file);
            }
        }

        bool WriteMethodToFile(MethodDefinition md, string file)
        {
            byte[] bytes = WriteMethodToBytes(md);
            if (bytes == null || bytes.Length == 0)
                return false;
            File.WriteAllBytes(file, bytes);
            return true;
        }

        byte[] WriteMethodToBytes(MethodDefinition md)
        {
            return Mono.Cecil.Cil.MethodBodyHelper.ToBytes(md);
        }



        public void cmReadMethodsFromFolder_Click(object sender, EventArgs e)
        {
            ModuleDefinition module = treeView1.SelectedNode.Tag as ModuleDefinition;
            if (module == null) return;

            string initBinDir = Config.LastBinDir;
            if (!Directory.Exists(initBinDir))
                initBinDir = Environment.CurrentDirectory;

            string folder = SimpleDialog.OpenFolder(initBinDir);
            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*.bin");
            if (files == null || files.Length == 0)
            {
                SimpleMessage.ShowInfo("Can't find binary files (*.bin).");
                return;
            }

            Config.LastBinDir = folder;

            Application.DoEvents();
            var listBinFiles = new List<String>();
            var listRVA = new List<int>();
            int j;
            for (int i = 0; i < files.Length; i++)
            {
                j = Int32.Parse(Path.GetFileNameWithoutExtension(files[i]), NumberStyles.AllowHexSpecifier);
                if (j > 0)
                {
                    listBinFiles.Add(files[i]);
                    listRVA.Add(j);
                }
            }

            //Get methods and filenames by RVAs
            var listMethods = new List<MethodDefinition>();
            var listMethodFiles = new List<String>();

            int readCount = 0;
            using (new SimpleWaitCursor())
            {
                FindMethodsToBeReadByRVA(module.Types, listBinFiles, listRVA, listMethodFiles, listMethods);
                for (int i = 0; i < listMethodFiles.Count; i++)
                {
                    _form.SetStatusText(String.Format("Reading {0} of {1} methods ...", i + 1, listMethodFiles.Count));
                    if (ReadMethodFromFile(listMethods[i], listMethodFiles[i]))
                        readCount++;
                    Application.DoEvents();
                }
                _form.SetStatusText(null);
            }

            SimpleMessage.ShowInfo(
                String.Format("Not matched: {0}\r\nMatched: {1}\r\nRead: {2}\r\n",
                listBinFiles.Count, listMethodFiles.Count, readCount),
                "Files Information");

        }

        private void FindMethodsToBeReadByRVA(Collection<TypeDefinition> types, List<String> srcFiles, List<int> srcRVAs,
            List<String> destFiles, List<MethodDefinition> destMethods)
        {
            if (srcFiles.Count == 0)
                return;

            foreach (TypeDefinition t in types)
            {
                foreach (MethodDefinition md in t.Methods)
                {
                    if (md.RVA == 0) continue;

                    for (int i = 0; i < srcRVAs.Count; i++)
                    {
                        //search by RVA or Token
                        if ((md.RVA == srcRVAs[i]) || (md.MetadataToken.ToInt32() == srcRVAs[i]))
                        {
                            destFiles.Add(srcFiles[i]);
                            destMethods.Add(md);
                            srcFiles.RemoveAt(i);
                            srcRVAs.RemoveAt(i);
                            if (srcFiles.Count == 0)
                                return;
                            break;
                        }
                    }
                }

                if (t.HasNestedTypes)
                {
                    FindMethodsToBeReadByRVA(t.NestedTypes, srcFiles, srcRVAs, destFiles, destMethods);
                }
            }

            return;
        }

        public void cmReadMethodFromFile_Click(object sender, EventArgs e)
        {
            MethodDefinition md = treeView1.SelectedNode.Tag as MethodDefinition;
            if (md == null) return;

            string initBinDir = Config.LastBinDir;
            if (!Directory.Exists(initBinDir))
                initBinDir = Environment.CurrentDirectory;

            string file = SimpleDialog.OpenFile("Open Binary File", "Binary File (*.bin)|*.bin|All Files(*.*)|*.*", "", false, initBinDir);
            if (File.Exists(file))
            {
                if (ReadMethodFromFile(md, file))
                {
                    _form.BodyGridHandler.InitBody(md);                    
                }
                Config.LastBinDir = Path.GetDirectoryName(file);
            }
        }

        public void cmRestoreMethodFromImage_Click(object sender, EventArgs e)
        {
            MethodDefinition md = treeView1.SelectedNode.Tag as MethodDefinition;
            if (md == null) return;

            Mono.Cecil.Cil.MethodBodyHelper.FromImage(md);
            _form.BodyGridHandler.InitBody(md);
        }

        bool ReadMethodFromFile(MethodDefinition md, string file)
        {
            byte[] bytes = File.ReadAllBytes(file);
            if (bytes == null || bytes.Length == 0)
                return false;
            ReadMethodFromBytes(md, bytes);
            return true;
        }

        void ReadMethodFromBytes(MethodDefinition md, byte[] bytes)
        {
            Mono.Cecil.Cil.MethodBodyHelper.FromBytes(md, bytes);
        }

        private void ImportMethodTypes(ModuleDefinition module, MethodDefinition method)
        {
            module.Import(method.ReturnType);
            foreach (Instruction ins in method.Body.Instructions)
            {
                switch (ins.OpCode.OperandType)
                {
                    case OperandType.InlineMethod:
                        MethodReference mr = ins.Operand as MethodReference;
                        ins.Operand = module.Import(mr);
                        break;
                    case OperandType.InlineType:
                        TypeReference tr = ins.Operand as TypeReference;
                        ins.Operand = module.Import(tr);
                        break;
                    case OperandType.InlineField:
                        FieldReference fr = ins.Operand as FieldReference;
                        ins.Operand = module.Import(fr);
                        break;
                    default:
                        break;
                }
            }
            foreach (VariableDefinition vd in method.Body.Variables)
            {
                module.Import(vd.VariableType);
            }
        }

        private MethodDefinition SelectMethodFromAssembly()
        {
            frmClassEdit f = new frmClassEdit(
                new ClassEditParams() {
                    Host = _form.Host,
                    Rows = new string[0],
                    SourceDir = _form.SourceDir,
                    ObjectType = ObjectTypes.Method,
                    ShowStaticOnly = false,
                    ShowSelectButton = true
                });
            f.ShowDialog();
            return f.SelectedMethod;
        }

        public void cmReadMethodFromAssembly_Click(object sender, EventArgs e)
        {
            MethodDefinition md = treeView1.SelectedNode.Tag as MethodDefinition;
            if (md == null) return;

            MethodDefinition selectedMethod = SelectMethodFromAssembly();
            if (selectedMethod == null)
                return;

            md.Body = selectedMethod.Body;
            ImportMethodTypes(md.Module, selectedMethod);
            _form.BodyGridHandler.InitBody(md);
        }

        public void cmImportMethodFromAssembly_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            TypeDefinition td = node.Tag as TypeDefinition;
            if (td == null) return;

            MethodDefinition selectedMethod = SelectMethodFromAssembly();
            if (selectedMethod == null)
                return;

            selectedMethod.DeclaringType = td;
            td.Methods.Add(selectedMethod);
            TreeNode n = AddMethodNode(node, selectedMethod);
            treeView1.SelectedNode = n;
        }

        #endregion Read Method From File        

    } // end of class
}

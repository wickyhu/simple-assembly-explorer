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
    public class ClassEditBookmarkHandler
    {
        frmClassEdit _form;
        ToolStripComboBox cboBookmark;

        public ClassEditBookmarkHandler(frmClassEdit form)        
        {
            _form = form;
            cboBookmark = _form.BookmarkComboBox;

            InitBookmarks();
        }

        private void InitBookmarks()
        {
            cboBookmark.Items.Clear();
            cboBookmark.Sorted = true;

            AddBookMark(null);
            cboBookmark.SelectedIndex = 0;

            LoadBookmark();
        }

        public void NavigatePrevious()
        {
            int preIndex = cboBookmark.SelectedIndex - 1;
            if (preIndex < 1)
                preIndex = cboBookmark.Items.Count - 1;
            if (preIndex > 0)
            {
                cboBookmark.SelectedIndex = preIndex;
            }
        }

        public void NavigateNext()
        {
            int nextIndex = cboBookmark.SelectedIndex + 1;
            if (nextIndex >= cboBookmark.Items.Count)
                nextIndex = 1;
            if (nextIndex < cboBookmark.Items.Count)
            {
                cboBookmark.SelectedIndex = nextIndex;
            }
        }

        public Bookmark FindBookmark(TreeNode node)
        {
            string indexPath = Bookmark.GetIndexPath(node);
            foreach (Bookmark bm in cboBookmark.Items)
            {
                if (bm.IndexPath == indexPath)
                {
                    return bm;
                }
            }
            return null;
        }

        public bool RemoveBookmark(TreeNode node)
        {
            Bookmark bm = FindBookmark(node);
            if (bm != null)
            {
                cboBookmark.Items.Remove(bm);
                return true;
            }
            return false;
        }

        public void AddBookMark(TreeNode node)
        {
            Bookmark newbm = new Bookmark(node);
            int index = cboBookmark.Items.Add(newbm);
            cboBookmark.SelectedIndex = index;
        }

        public void AddBookMark(string indexPath, string name)
        {
            Bookmark newbm = new Bookmark(indexPath, name);
            cboBookmark.Items.Add(newbm); // don't set selected here
        }

        public void cboBookmark_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bookmark bm = cboBookmark.SelectedItem as Bookmark;
            TreeNode n = FindTreeNode(bm);
            if (n != null)
            {
                _form.TreeView.SelectedNode = n;
            }
        }

        public void tbSaveBookmark_Click(object sender, EventArgs e)
        {
            SaveBookmark();
        }

        public TreeNode FindTreeNode(Bookmark bm)
        {
            if (bm == null || String.IsNullOrEmpty(bm.IndexPath))
                return null;

            if (bm.Node != null)
                return bm.Node;

            try
            {
                string[] indexes = bm.IndexPath.Split(new char[] { '.' });

                int index = Convert.ToInt32(indexes[0]);
                TreeView treeView1 = _form.TreeView;
                TreeNode node = treeView1.Nodes[index];
                for (int i = 1; i < indexes.Length; i++)
                {
                    index = Convert.ToInt32(indexes[i]);
                    _form.TreeViewHandler.InitNode(node);
                    node = node.Nodes[index];
                }

                bm.Node = node; //mh ...                
                //cboBookmark.Items.Remove(bm);
                //cboBookmark.Items.Add(bm);

                return node;
            }
            catch
            {
#if DEBUG
                throw;
#else
                return null;
#endif
            }
        }

        private string GetBookmarkFile()
        {
            if (_form.Rows.Length == 0) 
                return String.Empty;
            string assemblyFile = _form.Rows[0]; // how about multiple files?
            string bookmarkFile = assemblyFile + ".bookmark.txt";
            return bookmarkFile;
        }

        public void SaveBookmark()
        {
            string bookmarkFile = GetBookmarkFile();

            if (cboBookmark.Items.Count <= 1)
            {
                if (File.Exists(bookmarkFile))
                {
                    File.Delete(bookmarkFile);
                }
                return;
            }

            using (StreamWriter sw = File.CreateText(bookmarkFile))
            {
                for (int i = 1; i < cboBookmark.Items.Count; i++)
                {
                    Bookmark bm = cboBookmark.Items[i] as Bookmark;
                    sw.Write(bm.IndexPath);
                    sw.Write(";");
                    sw.WriteLine(bm.Name);
                }
            }
        }

        private void LoadBookmark()
        {
            string bookmarkFile = GetBookmarkFile();
            if (!File.Exists(bookmarkFile))
                return;

            using (StreamReader sr = File.OpenText(bookmarkFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    int p = line.IndexOf(";");
                    string indexPath = line.Substring(0, p);
                    string name = line.Substring(p + 1);

                    AddBookMark(indexPath, name);
                }
            }
        }

        public void cmBookmark_Click(object sender, EventArgs e)
        {
            TreeNode node = _form.TreeViewHandler.SelectedNode;
            if (node != null)
            {
                if (RemoveBookmark(node))
                {
                }
                else
                {
                    AddBookMark(node);
                }
            }
        }

        public class Bookmark
        {
            public string IndexPath { get; private set; }

            private TreeNode _node;
            public TreeNode Node
            {
                get { return _node; }
                set
                {
                    _node = value;
                    this.IndexPath = GetIndexPath(value);
                    _name = null;
                }
            }

            public Bookmark(string indexPath, string name)
            {
                this.IndexPath = indexPath;
                _name = name;
            }

            public Bookmark(TreeNode node)
            {
                this.Node = node;
            }

            private string _name;
            public string Name
            {
                get
                {
                    if (_name == null)
                    {
                        if (this.Node != null)
                        {
                            object tag = this.Node.Tag;
                            if (tag != null)
                            {
                                string typeName = tag.GetType().Name;
                                if (typeName.EndsWith("Definition"))
                                    typeName = typeName.Substring(0, typeName.Length - "Definition".Length);
                                typeName = typeName + ":";

                                string memberName;
                                if (tag is MemberReference)
                                {
                                    MemberReference mr = (MemberReference)tag;
                                    memberName = String.Format("{0}{1}",
                                        mr.DeclaringType == null ? String.Empty : mr.DeclaringType.Name + ".",
                                        mr.Name);
                                }
                                else
                                {
                                    memberName = tag.ToString();
                                }

                                _name = String.Format("{0,-9} {1}", typeName, memberName);
                            }
                            else
                            {
                                _name = this.Node.ToString();
                            }
                        }
                        else
                        {
                            _name = this.IndexPath;
                        }
                    }
                    return _name;
                }

            }

            public override string ToString()
            {
                return this.Name;
            }

            public override bool Equals(object obj)
            {
                if (obj is Bookmark)
                {
                    Bookmark tmp = (Bookmark)obj;
                    return tmp.IndexPath == this.IndexPath;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.IndexPath.GetHashCode();
            }

            public static string GetIndexPath(TreeNode node)
            {
                if (node == null)
                    return "";

                StringBuilder sb = new StringBuilder();
                TreeNode n = node;
                while (n != null)
                {
                    sb.Insert(0, String.Format(".{0}", n.Index));
                    n = n.Parent;
                }
                sb.Remove(0, 1);
                return sb.ToString();
            }
        }//end of Bookmark

    } // end of class
}

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

namespace SimpleAssemblyExplorer
{
    public class ClassEditDecompilerHandler
    {
        protected frmClassEdit _form;
        protected string _name;

        protected ComboBox cboLanguage;
        protected RichTextBox rtbText;
        protected TabPage tabPage;

        public ClassEditDecompilerHandler(string name, frmClassEdit form)
        {
            _form = form;
            _name = name;

            Initializing = false;
            Unloading = false;
        }

        string[] _assemblyPath;
        public string[] AssemblyPath { get { return _assemblyPath; } }
        object[] _assembly;
        public object[] Assembly { get { return _assembly; } }

        public virtual string GetAssemblyPath(int i)
        {
            if (_assemblyPath == null || i < 0 || i >= _assemblyPath.Length)
                return null;
            return _assemblyPath[i];
        }

        public virtual void InitAssemblyPath()
        {

            string[] saved = _assemblyPath;
            _assemblyPath = new string[_form.Rows.Length];
            Array.Copy(_form.Rows, _assemblyPath, _assemblyPath.Length);
            if (saved != null)
            {
                Array.Copy(saved, _assemblyPath, saved.Length > _assemblyPath.Length ? _assemblyPath.Length : saved.Length);
            }
            LoadAssemblies();

        }

        public virtual void LoadAssemblies()
        {
            if (IsReady)
            {
                int count = _assemblyPath.Length;
                _assembly = new object[count];
                for (int i = 0; i < count; i++)
                {
                    LoadAssembly(i, _assemblyPath[i]);
                }

                InitRichText(null);
            }
        }

        public virtual void LoadAssembly(int i, string path)
        {
            if (i < 0 || i >= _assemblyPath.Length) return;
            if (_assembly[i] != null)
            {
                UnloadAssembly(i);
            }
            _assembly[i] = LoadAssembly(path);
            _assemblyPath[i] = path;
        }

        public virtual object LoadAssembly(string path)
        {
            return null;
        }

        public virtual void UnloadAssembly(int i)
        {
            if (_assembly == null) return;

            if (i < 0 || i >= _assembly.Length) return;
            if (_assembly[i] != null)
            {
                UnloadAssembly(_assembly[i]);
                _assembly[i] = null;
            }
        }

        public virtual void UnloadAssembly(object assembly)
        {
            return;
        }

        public virtual void UnloadAssemblies()
        {
            try
            {
                Unloading = true;
                if (_assembly != null && _assembly.Length > 0)
                {
                    for (int i = 0; i < _assembly.Length; i++)
                    {
                        UnloadAssembly(i);
                    }
                }
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
            finally
            {
                Unloading = false;
            }
        }

        public virtual void Unload()
        {
            UnloadAssemblies();
        }

        public int FindAssemblyIndex(string path)
        {
            int index = -1;
            if (String.IsNullOrEmpty(path))
                return index;
            if (_form.Rows != null && _form.Rows.Length > 0)
            {
                for (int i = 0; i < _form.Rows.Length; i++)
                {
                    if (path.Equals(_form.Rows[i], StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (this.AssemblyPath != null && this.AssemblyPath.Length > 0)
            {
                for (int i = 0; i < this.AssemblyPath.Length; i++)
                {
                    if (path.Equals(this.AssemblyPath[i], StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }
            }
          
            return index;
        }
       

        public virtual bool IsReady
        {
            get { return cboLanguage.Items.Count > 0; }
        }
        
        public bool Initializing { get; set; }
        public bool Unloading { get; set; }
        public bool NeedToRefresh { get; set; }

        public virtual void InitControls()
        {
        }

        public virtual void Init()
        {
            if (!IsReady)
            {
                SimpleWaitCursor wc = new SimpleWaitCursor();
                try
                {
                    _form.SetStatusText(String.Format("Loading {0} ...", _name));
                    Initializing = true;
                    InitControls();
                    InitAssemblyPath();
                }
                catch
                {
                    _form.TabControl.SelectedTab = _form.DetailsTabPage;
                    throw;
                }
                finally
                {
                    wc.Dispose();
                    Initializing = false;
                    _form.SetStatusText(null);
                }
            }
        }

        public void tabPage_Enter(object sender, EventArgs e)
        {
            try
            {
                Init();

                if (NeedToRefresh)
                {
                    if (_form.TreeViewHandler.SelectedNode != null)
                        InitRichText(_form.TreeViewHandler.SelectedNode.Tag);
                    NeedToRefresh = false;
                }
            }
            catch
            {
                _form.TabControl.SelectedTab = _form.DetailsTabPage;
                throw;
            }
            finally
            {
            }
        }

        public virtual object GetDecompileObject(object cecilObject)
        {
            if (!IsReady) return null;
            if (cecilObject is string) return null;
            if (cecilObject is Resource) return null;

            object o = cecilObject;
            if (o == null)
            {
                TreeNode n = _form.TreeViewHandler.SelectedNode;
                if (n != null && n.Tag != null)
                    o = n.Tag;
            }

            if (o == null) return null;
            if (AssemblyUtils.IsInternalType(o.GetType())) return null;
            return o;
        }

        public virtual void DecompileObject(object decompileObject)
        {
            rtbText.Rtf = GetRtf(decompileObject == null ? String.Empty : decompileObject.ToString(), null);
        }

        public virtual void InitRichText(object cecilObject)
        {
            rtbText.Clear();

            object o = GetDecompileObject(cecilObject);
            if (o == null) return;

            SimpleWaitCursor wc = new SimpleWaitCursor();
            try
            {
                highlightText = null;
                rtbText.SuspendLayout();
                DecompileObject(o);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                rtbText.Rtf = GetRtf(ex.Message, ex.StackTrace);
            }
            finally
            {
                rtbText.ResumeLayout();
                wc.Dispose();
            }
        }

        public string GetRtf(string message, string trace)
        {
            RichTextWriter writer = new RichTextWriter("C#");

            string[] strs;

            if (!String.IsNullOrEmpty(message))
            {
                strs = message.Split('\r', '\n');
                foreach (string s in strs)
                {
                    writer.WriteDeclaration(s);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }

            if (!String.IsNullOrEmpty(trace))
            {
                strs = trace.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in strs)
                {
                    writer.Write(s);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }

            return writer.ToString();
        }

        public void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitRichText(null);
        }       

        public AssemblyDefinition FindAssemblyDefinition(string path)
        {
            AssemblyDefinition ad = null;
            if (String.IsNullOrEmpty(path))
            {
                TreeNode n = _form.TreeViewHandler.FindTopNode(null);
                if (n != null)
                {
                    ad = n.Tag as AssemblyDefinition;
                }
            }
            else
            {
                foreach (TreeNode n in _form.TreeView.Nodes)
                {
                    if (n.FullPath == path)
                    {
                        ad = n.Tag as AssemblyDefinition;
                        break;
                    }
                }
            }
            return ad;
        }

        public void FindCurrentAssembly(out string file, out AssemblyDefinition ad, out int index)
        {
            file = null;
            ad = null;
            index = -1;

            TreeNode n = _form.TreeViewHandler.FindTopNode(null);
            if (n != null)
            {
                file = Path.Combine(_form.SourceDir, _form.TreeViewHandler.GetTreeNodeText(n));
                ad = n.Tag as AssemblyDefinition;
                //index = n.Index;
                index = FindAssemblyIndex(file);
            }
        }        

        public void btnLoad_Click(object sender, EventArgs e)
        {
            string currentAssemblyFile;
            AssemblyDefinition ad;
            int index;
            FindCurrentAssembly(out currentAssemblyFile, out ad, out index);
            if (index < 0) return;

            string file = SimpleDialog.OpenFile("Load Assembly",
                Consts.FilterAssemblyFile,
                Path.GetExtension(currentAssemblyFile),
                true,
                Path.GetDirectoryName(currentAssemblyFile),
                Path.GetFileName(currentAssemblyFile));

            if (!String.IsNullOrEmpty(file) && File.Exists(file))
            {
                UnloadAssembly(index);
                LoadAssembly(index, file);
                _form.LogHandler.LogLoad(file);

                btnLoad_Clicked(ad, file);

                InitRichText(null);
                
            }
        }

        public virtual void btnLoad_Clicked(AssemblyDefinition currentAssembly, string loadedFile)
        {
        }

        public bool isHighlighting = false;
        private string highlightText;
        public void rtbText_Hightlight(object sender, EventArgs e)
        {
            if (isHighlighting)
                return;

            try
            {
                isHighlighting = true;
                RichTextBox rtb = sender as RichTextBox;
                if (rtb == null)
                    return;

                string text = rtb.SelectedText;
                if (string.IsNullOrWhiteSpace(text))
                    return;
                text = text.Trim();

                var helper = new RichTextFormatterHelper(rtb);
                helper.Highlight(highlightText, rtb.BackColor);
                helper.Highlight(text, Color.BurlyWood);
                highlightText = text;
            }
            finally
            {
                isHighlighting = false;
            }
        }


    }//end of class
}

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
    public class ClassEditReflectorHandler : ClassEditDecompilerHandler
    {
        private ComboBox cboOptimization;
        private SimpleReflector _reflector;
        //Reflector.CodeModel.IAssembly

        public ClassEditReflectorHandler(frmClassEdit form)
            : base("Reflector", form)
        {
            cboLanguage = _form.LanguageComboBox;
            cboOptimization = _form.OptimizationComboBox;
            rtbText = _form.ReflectorTextBox;
            tabPage = _form.ReflectorTabPage;
        }

        public override void InitControls()
        {
            //TODO: better to new instance, but seems resource can't be released
            if (_reflector == null)
            {
                _reflector = SimpleReflector.Default;
                _reflector.FormatterType = typeof(RichTextFormatter);
            }

            foreach (string l in _reflector.Languages)
            {
                int index = cboLanguage.Items.Add(l);
                if (l == "C#")
                {
                    cboLanguage.SelectedIndex = index;
                }
            }
            foreach (string op in SimpleReflector.OptimizationList)
            {
                int index = cboOptimization.Items.Add(op);
                if (op == "2.0")
                {
                    cboOptimization.SelectedIndex = index;
                }
            }

            if (cboLanguage.Items.Count > 0 && cboLanguage.SelectedIndex < 0)
                cboLanguage.SelectedIndex = 0;

            if (IsReady)
            {
                tabPage.Text = String.Format("{0} {1}.{2}", _name, SimpleReflector.Version.Major, SimpleReflector.Version.Minor);

                cboLanguage.SelectedIndexChanged += new EventHandler(base.cboLanguage_SelectedIndexChanged);
                cboOptimization.SelectedIndexChanged += new EventHandler(cboOptimization_SelectedIndexChanged);
            }
        }       

        object _currentReflectorObject = null;
        public object Cecil2Reflector(object cecilObject)
        {
            object reflectorObject = null;

            if (cecilObject is AssemblyNameReference || cecilObject is ModuleReference)
            {
                ModuleDefinition md = null;
                if (cecilObject is ModuleDefinition)
                {
                    md = (ModuleDefinition)cecilObject;
                }
                else
                {
                    TreeNode n = _form.TreeViewHandler.SelectedNode;
                    if (n != null) n = n.Parent;
                    if (n != null) n = n.Parent;
                    if (n != null && n.Tag != null && n.Tag is ModuleDefinition)
                        md = (ModuleDefinition)n.Tag;
                }

                if (md != null)
                {
                    if (cecilObject is AssemblyNameReference)
                        reflectorObject = CecilHelper.Cecil2Reflector((AssemblyNameReference)cecilObject, md);
                    else if (cecilObject is ModuleDefinition)
                        reflectorObject = CecilHelper.Cecil2Reflector(md);
                    else if (cecilObject is ModuleReference)
                        reflectorObject = CecilHelper.Cecil2Reflector((ModuleReference)cecilObject, md);
                }
            }
            else if (cecilObject is TypeDefinition)
            {
                reflectorObject = CecilHelper.Cecil2Reflector(cecilObject);
            }
            else if (cecilObject is TypeReference && ((TypeReference)cecilObject).Scope is AssemblyNameReference)
            {
                TypeReference tr2 = (TypeReference)cecilObject;
                reflectorObject = CecilHelper.Cecil2Reflector((AssemblyNameReference)tr2.Scope, tr2.Module);
            }
            else if (cecilObject is TypeReference && ((TypeReference)cecilObject).Scope is ModuleReference)
            {
                TypeReference tr2 = (TypeReference)cecilObject;
                reflectorObject = CecilHelper.Cecil2Reflector((ModuleReference)tr2.Scope, tr2.Module);
            }
            else if (cecilObject is Resource)
            {
                TreeNode n = _form.TreeViewHandler.SelectedNode;
                if (n != null) n = n.Parent;
                if (n != null) n = n.Parent;
                ModuleDefinition md = null;
                if (n != null && n.Tag != null && n.Tag is ModuleDefinition)
                    md = (ModuleDefinition)n.Tag;
                if (md != null)
                {
                    reflectorObject = CecilHelper.Cecil2Reflector((Resource)cecilObject, md);
                }
            }
            else
            {
                reflectorObject = CecilHelper.Cecil2Reflector(cecilObject);
            }

            return reflectorObject;
        }

        public override void DecompileObject(object decompileObject)
        {
            _currentReflectorObject = null;
            object reflectorObject = Cecil2Reflector(decompileObject);
            if (reflectorObject != null)
            {
                _currentReflectorObject = reflectorObject;

                bool resolveDirAdded = _form.Host.AddAssemblyResolveDir(_form.SourceDir);
                try
                {
                    rtbText.Rtf = _reflector.Decompile(
                        cboLanguage.SelectedIndex,
                        reflectorObject);
                    try
                    {
                        isHighlighting = true; //disable hightligh
                        new RichTextFormatterHelper(rtbText).DetectLinks();
                    }
                    finally
                    {
                        isHighlighting = false;
                    }
                }
                finally
                {
                    if (resolveDirAdded)
                        _form.Host.RemoveAssemblyResolveDir(_form.SourceDir);
                }
            }
            else
            {
                _currentReflectorObject = null;
                rtbText.Rtf = GetRtf("Cannot find matched Reflector object!", null);
            }
        }       

        public void cboOptimization_SelectedIndexChanged(object sender, EventArgs e)
        {
            _reflector.Optimization = cboOptimization.Text;
            InitRichText(null);
        }

        private void CheckSameAssembly()
        {
            List<string> processed = new List<string>();
            for (int i = 0; i < this.Assembly.Length - 1; i++)
            {
                Reflector.CodeModel.IAssembly a1 = (Reflector.CodeModel.IAssembly)this.Assembly[i];
                if (a1 == null) continue;
                string name = a1.ToString();
                for (int j = i + 1; j < this.Assembly.Length; j++)
                {
                    Reflector.CodeModel.IAssembly a2 = (Reflector.CodeModel.IAssembly)this.Assembly[j];
                    if (a2 == null) continue;                    
                    if (name == a2.ToString()
                        && !processed.Contains(name))
                    {
                        SimpleMessage.ShowInfo(String.Format("You are loading multiple instances of Assembly:\n\"{0}\".\n\n.Net Reflector will only work for the last loaded one, or the one explicit loaded with Load button.", name));
                        processed.Add(name);
                    }
                }
            }
        }

        public override object LoadAssembly(string path)
        {            
            return _reflector.LoadAssembly(path);
        }       

        public override void LoadAssemblies()
        {
            if (IsReady)
            {
                base.LoadAssemblies();
                CheckSameAssembly();
            }
        }

        public override void UnloadAssembly(object assembly)
        {
            Reflector.CodeModel.IAssembly a = (Reflector.CodeModel.IAssembly)assembly;
            
            string path = a.Location;
            AssemblyDefinition ad = FindAssemblyDefinition(path);
            if (ad != null)
            {
                CecilHelper.RemoveAssemblyCache(ad);
            }
            _reflector.AssemblyManager.Unload(a);
        }

        public override void Unload()
        {
            if (_reflector != null)
            {
                base.Unload();
                _reflector = null;
            }
        }

        //public override bool IsMatchedAssembly(string path, object assembly)
        //{
        //    Reflector.CodeModel.IAssembly a = (Reflector.CodeModel.IAssembly)assembly;
        //    return a.Location.Equals(path, StringComparison.OrdinalIgnoreCase);
        //}

        public override void btnLoad_Clicked(AssemblyDefinition currentAssembly, string loadedFile)
        {
            if (currentAssembly != null)
                CecilHelper.RemoveAssemblyCache(currentAssembly);
            _form.ReflectorLoadedFile.Text = loadedFile;
        }

    }//end of class
}

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
using ICSharpCode.ILSpy;

namespace SimpleAssemblyExplorer
{
    public class ClassEditILSpyHandler : ClassEditDecompilerHandler
    {
        private SimpleILSpy _ilSpy;

        public ClassEditILSpyHandler(frmClassEdit form) : base("ILSpy", form)
        {
            cboLanguage = _form.ILSpyLanguageComboBox;
            rtbText = _form.ILSpyTextBox;
            tabPage = _form.ILSpyTabPage;
        }

        public override void InitControls()
        {
            if (_ilSpy == null)
            {
                _ilSpy = new SimpleILSpy();
            }

            base.InitControls();

            foreach (string l in _ilSpy.Languages)
            {
                int index = cboLanguage.Items.Add(l);
                if (l == "C#")
                {
                    cboLanguage.SelectedIndex = index;
                }
            }
            if (cboLanguage.Items.Count > 0 && cboLanguage.SelectedIndex < 0)
                cboLanguage.SelectedIndex = 0;

            if (IsReady)
            {
                tabPage.Text = String.Format("{0} {1}.{2}", _name, SimpleILSpy.Version.Major, SimpleILSpy.Version.Minor);

                cboLanguage.SelectedIndexChanged += new EventHandler(cboLanguage_SelectedIndexChanged);
            }
        }

        public override void DecompileObject(object decompileObject)
        {
            if (!(decompileObject is IMetadataTokenProvider))
            {
                rtbText.Clear();
                return;
            }

            string currentAssemblyFile;
            AssemblyDefinition ad;
            int index;            
            FindCurrentAssembly(out currentAssemblyFile, out ad, out index);
            if (index < 0) return;

            IMetadataTokenProvider mtp = (IMetadataTokenProvider)decompileObject;
            object o = decompileObject;
            
            //use loaded assembly
            if (this.Assembly[index] != null && 
                !(decompileObject is Mono.Cecil.AssemblyDefinition) &&  
                !(decompileObject is Mono.Cecil.ModuleDefinition) && 
                !(decompileObject is Mono.Cecil.ModuleReference) && 
                !(decompileObject is Mono.Cecil.AssemblyNameReference)
                )
            {
                ad = (AssemblyDefinition)this.Assembly[index];

                //LookupToken doesn't handle PropertyDefinition & EventDefinition?
                if (decompileObject is Mono.Cecil.PropertyDefinition)
                {
                    PropertyDefinition property = (PropertyDefinition)decompileObject;
                    TypeDefinition type = DeobfUtils.Resolve(property.DeclaringType, null, null);
                    if (type != null)
                    {
                        foreach (PropertyDefinition pd in type.Properties)
                        {
                            if (pd.MetadataToken.ToInt32() == property.MetadataToken.ToInt32())
                            {
                                o = pd;
                                break;
                            }
                        }
                    }
                }
                else if (decompileObject is Mono.Cecil.EventDefinition)
                {
                    EventDefinition eventDef = (EventDefinition)decompileObject;
                    TypeDefinition type = DeobfUtils.Resolve(eventDef.DeclaringType, null, null);
                    if (type != null)
                    {
                        foreach (EventDefinition ed in type.Events)
                        {
                            if (ed.MetadataToken.ToInt32() == eventDef.MetadataToken.ToInt32())
                            {
                                o = ed;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //TODO: maybe search by name will be better?                
                    o = ad.MainModule.LookupToken(mtp.MetadataToken);
                }                
            }

            if (o == null || decompileObject.ToString() != o.ToString())
            {
                rtbText.Rtf = GetRtf("Cannot find matched Cecil object!", null);
                return;
            }

            bool resolveDirAdded = _form.Host.AddAssemblyResolveDir(_form.SourceDir);

            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            bool savedRaiseResolveException = true;
            try
            {
                if (bar != null)
                {
                    savedRaiseResolveException = bar.RaiseResolveException;
                    bar.RaiseResolveException = false;
                }
                rtbText.Rtf = _ilSpy.Decompile(cboLanguage.SelectedItem as string, o);
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
                if (bar != null)
                    bar.RaiseResolveException = savedRaiseResolveException;
            }
        }

        public override void LoadAssembly(int i, string path)
        {
            if (!File.Exists(path))
                return;
            
            this.Assembly[i] = LoadAssembly(path);
            this.AssemblyPath[i] = path;
        }

        public override object LoadAssembly(string path)
        {
            if (PathUtils.IsNetModule(path))
                return null;
            return AssemblyDefinition.ReadAssembly(path);
        }

        public override void UnloadAssembly(int i)
        {
            this.Assembly[i] = null;            
        }

        public override void UnloadAssembly(object assembly)
        {
            return;
        }

        //public override bool IsMatchedAssembly(string path, object assembly)
        //{
        //    AssemblyDefinition ad = (AssemblyDefinition)assembly;
        //    return ad.MainModule.FullyQualifiedName.Equals(path, StringComparison.OrdinalIgnoreCase);            
        //}

        public override void btnLoad_Clicked(AssemblyDefinition currentAssembly, string loadedFile)
        {
            _form.ILSpyLoadedFile.Text = loadedFile;
        }        

    }//end of class
}

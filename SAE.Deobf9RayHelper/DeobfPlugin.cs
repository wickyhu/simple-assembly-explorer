using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SimpleAssemblyExplorer.Plugin;
using System.Resources;
using SimpleUtils;
using Mono.Cecil;
using SimpleAssemblyExplorer;

namespace SAE.Deobf9RayHelper
{
    public class DeobfPluginSample : DefaultDeobfPlugin
    {

        public DeobfPluginSample(IHost host)
            : base(host)
        {
        }

        public override DeobfPluginInfo PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                {
                    _pluginInfo = new DeobfPluginInfo();
                    _pluginInfo.Author = SimpleDotNet.Author;
                    _pluginInfo.Contact = SimpleDotNet.EmailAddress;
                    _pluginInfo.Url = SimpleDotNet.WebSiteUrl;
                    _pluginInfo.Title = "Deobfuscator 9Ray Helper";
                }
                return _pluginInfo;
            }
        }

        AssemblyDefinition _assembly = null;

        System.Reflection.MethodInfo _encodingMethod = null;

        private bool IsBaseType(string baseTypeFullName, Type type)
        {
            if (type == null)
                return false;
            if (type.FullName == baseTypeFullName)
                return true;
            return IsBaseType(baseTypeFullName, type.BaseType);
        }

        private bool IsBaseType(string baseTypeFullName, TypeDefinition type)
        {
            if (type == null)
                return false;
            if (type.FullName == baseTypeFullName)
                return true;
            TypeDefinition baseType = null;
            if (type.BaseType != null)
            {
                try
                {
                    baseType = type.BaseType.Resolve();
                }
                catch { }
            }
            return IsBaseType(baseTypeFullName, baseType);
        }

        private void FindResourceNameEncodingMethod()
        {
            System.Reflection.Assembly a = AssemblyUtils.LoadAssemblyFile(_assembly.MainModule.FullyQualifiedName);
            Type[] types = a.GetTypes();
            if (types != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (IsBaseType("System.Resources.ResourceManager", type))
                    {
                        System.Reflection.MethodInfo[] methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        if (methods != null)
                        {
                            for (int j = 0; j < methods.Length; j++)
                            {
                                System.Reflection.MethodInfo method = methods[j];
                                if (method.ReturnType.FullName == "System.String")
                                {
                                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                                    if (parameters != null && parameters.Length == 1 && parameters[0].ParameterType.FullName == "System.String")
                                    {
                                        _encodingMethod = method;
                                        break;
                                    }
                                }
                            }
                        }
                        if (_encodingMethod != null)
                            break;
                    }
                }
            }

        }

        public override void BeforeHandleAssembly(AssemblyDefinition assemblyDef)
        {
            _assembly = assemblyDef;
            FindResourceNameEncodingMethod();
        }


        public override void AfterHandleType(TypeDefinition typeDef)
        {
            if (_encodingMethod == null) return;
            if (typeDef.FullName == "<Module>") return;
            if (!IsBaseType("System.Windows.Forms.Form", typeDef))
                return;                

            string resName = typeDef.OriginalName;
            //if (!String.IsNullOrEmpty(typeDef.OriginalNamespace))
            //{
            //    resName = resName.Substring(typeDef.OriginalNamespace.Length + 1);
            //}
            //resName = resName.Replace("/", ".");
            resName = resName + ".resources";
            string encodedName = (string)_encodingMethod.Invoke(null, new object[] { resName });

            foreach (Resource r in _assembly.MainModule.Resources)
            {
                if (r.Name == encodedName || r.Name.EndsWith("." + encodedName))
                {
                    resName = typeDef.Name;
                    //if (!String.IsNullOrEmpty(typeDef.Namespace))
                    //{
                    //    resName = resName.Substring(typeDef.Namespace.Length + 1);
                    //}
                    //resName = resName.Replace("/", ".");
                    resName = resName + ".resources";
                    string newEncodedName = (string)_encodingMethod.Invoke(null, new object[] { resName });
                    r.Name = r.Name.Replace(encodedName, newEncodedName);
                }
            }
        }

        public override void Configure()
        {
            frmConfig frm = new frmConfig();
           
            if (frm.ShowDialog() == DialogResult.OK)
            {
            }
        }
    } //end of class
}

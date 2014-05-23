using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using SimpleUtils;
using SimpleUtils.Win;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;

namespace SimpleAssemblyExplorer
{
    public class AssemblyUtils
    { 
        
        private static Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        public static Assembly LoadAssembly(string assemblyString)
        {
            if (_assemblies.ContainsKey(assemblyString))
                return _assemblies[assemblyString];

            try
            {
                Assembly a = Assembly.Load(assemblyString);
                _assemblies.Add(assemblyString, a);
                return a;
            }
            catch
            {
                _assemblies.Add(assemblyString, null);
            }
            return null;
        }

        public static Assembly LoadAssemblyFile(string assemblyFile)
        {
            if (_assemblies.ContainsKey(assemblyFile))
                return _assemblies[assemblyFile];

            Assembly a = Assembly.LoadFile(assemblyFile);
            _assemblies.Add(assemblyFile, a);
            return a;
        }

        public static Assembly ResolveAssemblyFile(string assemblyFullName, string path, string searchPattern)
        {
            string[] files = Directory.GetFiles(path, searchPattern);

            Assembly a = null;
            if (files != null && files.Length > 0)
            {
                foreach (string file in files)
                {
                    try
                    {
                        a = LoadAssemblyFile(file);
                    }
                    catch
                    {
                        a = null;
                    }
                    if (a.FullName == assemblyFullName) break;
                }
            }
            return a;
        }

        public static Assembly ResolveAssemblyFile(string assemblyFullName, string dir)
        {
            string[] s = assemblyFullName.Split(new char[] { ',' });
            Assembly a = ResolveAssemblyFile(assemblyFullName, dir, s[0] + ".dll");
            if (a != null) return a;
            a = ResolveAssemblyFile(assemblyFullName, dir, s[0] + ".exe");
            if (a != null) return a;            
            return a;
        }

        private static Module FindModule(Assembly a, TypeDefinition type)
        {            
            ModuleDefinition md = type.Module;
            AssemblyDefinition ad = md.Assembly;
            Module m = null;
            Assembly tmp = a;
            if (tmp.FullName != ad.FullName)
            {
                tmp = AssemblyUtils.LoadAssembly(ad.FullName);                
            }
            Module[] modules = tmp.GetModules();
            string moduleName = md.FullyQualifiedName;
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i].FullyQualifiedName == moduleName)
                {
                    m = modules[i];
                    break;
                }
            }

            if (m == null)
            {
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i].Name == md.Name && modules[i].ModuleVersionId.Equals(md.Mvid))
                    {
                        m = modules[i];
                        break;
                    }
                }
            }
            return m;
        }

        public static FieldInfo ResolveField(Assembly a, FieldDefinition field)
        {
            Module m = FindModule(a, field.DeclaringType);
            if (m == null)
            {
                throw new ApplicationException("Cannot locate module: " + field.DeclaringType.Module.Name);
            }
            FieldInfo fi;
            try
            {
                fi = m.ResolveField((int)field.MetadataToken.ToUInt32());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Cannot resolve field: {0}\r\n{1}", field.FullName, ex.Message));
            }
            return fi;
        }

        public static MethodBase ResolveMethod(Assembly a, MethodDefinition method)
        {
            Module m = FindModule(a, method.DeclaringType);            
            if (m == null)
            {
                throw new ApplicationException("Cannot locate module: " + method.DeclaringType.Module.Name);
            }

            MethodBase mb;
            try
            {
                mb = m.ResolveMethod(method.MetadataToken.ToInt32());
                if (mb == null || mb.Name != (method.OriginalName ?? method.Name) || mb.MetadataToken != method.MetadataToken.ToInt32())
                    throw new ApplicationException("Mismatch method found.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Cannot resolve method: {0}\r\n{1}", method.FullName, ex.Message));
            }
            return mb;
        }

        public static MethodBase ResolveMethod(MethodDefinition method)
        {
            string assemblyFile = method.Module.FullyQualifiedName;
            return ResolveMethod(LoadAssemblyFile(assemblyFile), method);
        }

        public static bool IsInternalType(Type t)
        {
            return t.FullName.StartsWith("SimpleAssemblyExplorer");
        }

        public static bool IsSystemAssembly(string assemblyFullName)
        {
            return assemblyFullName.IndexOf("PublicKeyToken=b77a5c561934e089", StringComparison.OrdinalIgnoreCase) > 0;
        }

        public static List<string> GetEnumNames(Type enumType)
        {
            List<string> list = new List<string>();
            list.AddRange(Enum.GetNames(enumType));
            //System.Reflection.FieldInfo[] fields = enumType.GetFields();
            //foreach (System.Reflection.FieldInfo f in fields)
            //{
            //    if (f.FieldType.Equals(enumType))
            //    {
            //        string s = Enum.Parse(enumType, f.Name, true).ToString();
            //        list.Add(s);
            //    }
            //}
            return list;
        }

    }//end of class
}

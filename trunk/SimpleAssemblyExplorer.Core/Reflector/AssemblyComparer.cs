using System;
using System.Collections.Generic;
using System.Text;
using Reflector;
using Reflector.CodeModel;
using System.Reflection;

namespace SimpleAssemblyExplorer.LutzReflector
{
    public class AssemblyComparer : IAssemblyComparer
    {
        public bool Equals(IAssemblyReference assemblyReference1, IAssemblyReference assemblyReference2)
        {
            return assemblyReference1 != null && 
                assemblyReference2 != null && 
                assemblyReference1.Name != null &&
                assemblyReference1.Name == assemblyReference2.Name &&
                assemblyReference1.Culture == assemblyReference2.Culture;
        }
    }


}

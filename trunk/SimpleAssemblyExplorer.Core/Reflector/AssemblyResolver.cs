using System;
using System.Collections.Generic;
using System.Text;
using Reflector;
using Reflector.CodeModel;
using System.Reflection;
using System.IO;

namespace SimpleAssemblyExplorer.LutzReflector
{
    public class AssemblyResolver : IAssemblyResolver
    {
        private IAssemblyManager _assemblyManager;
        //private IAssemblyResolver _originalResolver;

        public AssemblyResolver(IAssemblyManager assemblyManager)
        {
            //_originalResolver = assemblyManager.Resolver;
            _assemblyManager = assemblyManager;
        }

        public IAssembly Resolve(IAssemblyReference value, string localPath)
        {
            foreach (IAssembly ia in _assemblyManager.Assemblies)
            {
                if (ia.CompareTo(value) == 0)
                    return ia;
            }

            string assemblyString = value.ToString();
            Assembly assembly = null;
            try
            {
                assembly = AssemblyUtils.ResolveAssemblyFile(assemblyString, localPath);
                if (assembly == null)
                {
                    assembly = AssemblyUtils.LoadAssembly(assemblyString);
                }
            }
            catch
            {
            }

            if (assembly != null)
            {
                IAssembly ia = _assemblyManager.LoadFile(assembly.Location);
                return ia;
            }
            
            return null;
        }

    }//end of class
}

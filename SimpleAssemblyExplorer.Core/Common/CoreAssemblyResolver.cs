using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using Mono.Cecil;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public class CoreAssemblyResolver : IDisposable
    {
        ResolveEventHandler _resolveEventHandler = null;
        List<string> _resolveDirs = null;

        public CoreAssemblyResolver()
            : this(null)
        {
        }

        public CoreAssemblyResolver(ResolveEventHandler resolveEventHandler)
        {
            SetupAssemblyResolve(resolveEventHandler);
        }

        void SetupAssemblyResolve(ResolveEventHandler resolveEventHandler)
        {
            if (resolveEventHandler == null)
            {
                _resolveEventHandler = new ResolveEventHandler(DefaultResolveEventHandler);
            }
            else
            {
                _resolveEventHandler = resolveEventHandler;
            }
            _resolveDirs = new List<string>();
            AppDomain.CurrentDomain.AssemblyResolve += _resolveEventHandler;


            string reflector = Config.Reflector;
            if (!String.IsNullOrEmpty(reflector) && File.Exists(reflector))
            {
                AddAssemblyResolveDir(Path.GetDirectoryName(reflector));
            }
            
        }

        void RemoveAssemblyResolve()
        {
            if (_resolveEventHandler != null)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= _resolveEventHandler;
                _resolveEventHandler = null;
            }
            if (_resolveDirs != null)
            {
                _resolveDirs.Clear();
                _resolveDirs = null;
            }
        }

        public void Dispose()
        {
            RemoveAssemblyResolve();
        }

        public bool AddAssemblyResolveDir(string dir)
        {
            if (_resolveDirs.Contains(dir) || !Directory.Exists(dir))
                return false;
            
            _resolveDirs.Add(dir);

            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            if (bar != null)
                bar.AddSearchDirectory(dir);

            return true;
        }

        public bool RemoveAssemblyResolveDir(string dir)
        {
            if (_resolveDirs.Contains(dir))
            {
                _resolveDirs.Remove(dir);

                BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
                if (bar != null)
                    bar.RemoveSearchDirectory(dir);

                return true;
            }
            return false;
        }

        public Assembly ResolveAssembly(string assemblyFullName)
        {
            foreach (string dir in _resolveDirs)
            {
                Assembly a = AssemblyUtils.ResolveAssemblyFile(assemblyFullName, dir);
                if (a != null) return a;
            }
            return null;
        }

        public Assembly DefaultResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly a = ResolveAssembly(args.Name);
            return a;
        }       

    } //end of class
}

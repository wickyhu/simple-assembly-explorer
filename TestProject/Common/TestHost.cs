using System;
using System.Collections.Generic;
using System.Text;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.Plugin;
using System.IO;

namespace TestProject
{
    public class TestHost : IHost 
    {
        CoreAssemblyResolver _coreAssemblyResolver = new CoreAssemblyResolver();

#pragma warning disable 0067
        public event FileSystemEventHandler TextFileChanged;
#pragma warning restore 0067

        public TestHost()
        {
        }

        public string PluginDir
        {
            get { return Global.AssemblyDir; }
        }

        public ITextInfo TextInfo { get; set; }

        public void SetStatusText(string info)
        {
            //Utils.DebugOutput(info);
        }

        public void SetStatusText(string info, bool doEvents)
        {
            //Utils.DebugOutput(info);
        }

        public void InitProgress(int min, int max)
        {            
        }

        public void SetProgress(int val)
        {            
        }

        public void SetProgress(int val, bool doEvents)
        {            
        }

        public void ResetProgress()
        {            
        }

        Dictionary<string, object> _properties = new Dictionary<string, object>();
        public bool AddProperty(string propertyName, object defaultValue, Type propertyType)
        {
            if (_properties.ContainsKey(propertyName)) return false;
            _properties.Add(propertyName, defaultValue);
            return true;
        }

        public void RemoveProperty(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
                _properties.Remove(propertyName);
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            _properties[propertyName] = value;
        }

        public object GetPropertyValue(string propertyName)
        {
            return _properties[propertyName];
        }

        public bool AddAssemblyResolveDir(string dir)
        {
            return _coreAssemblyResolver.AddAssemblyResolveDir(dir);
        }

        public bool RemoveAssemblyResolveDir(string dir)
        {
            return _coreAssemblyResolver.RemoveAssemblyResolveDir(dir);
        }

    }
}

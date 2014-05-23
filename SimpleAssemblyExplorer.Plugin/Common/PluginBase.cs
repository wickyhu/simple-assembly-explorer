using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleAssemblyExplorer.Plugin
{
    public abstract class PluginBase : IPluginBase 
    {
        public abstract PluginInfoBase PluginInfoBase { get; }

        public IHost Host { get; private set; }

        public PluginBase(IHost host)
        {
            this.Host = host;
        }

    }// end of class
}


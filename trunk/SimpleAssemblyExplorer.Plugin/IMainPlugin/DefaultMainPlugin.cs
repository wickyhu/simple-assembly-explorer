using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleAssemblyExplorer.Plugin
{
    public abstract class DefaultMainPlugin : PluginBase, IMainPlugin
    {
        protected MainPluginInfo _pluginInfo;
        public virtual MainPluginInfo PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                {
                    _pluginInfo = new MainPluginInfo();                   
                }
                return _pluginInfo;
            }
        }

        public override PluginInfoBase PluginInfoBase
        {
            get { return this.PluginInfo; }
        }

        public DefaultMainPlugin(IHost host)
            : base(host)
        {
        }

        public virtual PluginReturns Run(PluginArgument arg)
        {
            return PluginReturns.None;
        }

    }// end of class
}


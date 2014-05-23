using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IPluginBase
    {
        /// <summary>
        /// Plugin information
        /// </summary>
        PluginInfoBase PluginInfoBase { get; }
    }
   
}

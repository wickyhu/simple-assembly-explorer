using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IMainPlugin : IPluginBase
    {
        /// <summary>
        /// Plugin information
        /// </summary>
        MainPluginInfo PluginInfo { get; }

        /// <summary>
        /// Plugin interface
        /// </summary>
        /// <param name="arg">plugin argument</param>
        /// <returns>to tell main form what to do after plugin returns</returns>
        PluginReturns Run(PluginArgument arg);
    }
}

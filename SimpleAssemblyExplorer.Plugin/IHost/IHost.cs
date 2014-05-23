using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IHost : IStatus, IProgressBar, IPropertyValue, IAssemblyResolveDir, ITextFileWatcher
    {
        /// <summary>
        /// Plugin directory
        /// </summary>
        string PluginDir { get; }

        /// <summary>
        /// Used for feedback message to child form
        /// </summary>
        ITextInfo TextInfo { get; set; }
    }
}

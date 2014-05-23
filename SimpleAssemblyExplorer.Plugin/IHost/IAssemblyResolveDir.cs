using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IAssemblyResolveDir
    {
        /// <summary>
        /// Add a directory to assembly lookup directory list
        /// </summary>
        /// <param name="dir">full path</param>        
        bool AddAssemblyResolveDir(string dir);

        /// <summary>
        /// Remove a directory from assembly lookup directory list
        /// </summary>
        /// <param name="dir">full path</param>
        bool RemoveAssemblyResolveDir(string dir);
    }
}

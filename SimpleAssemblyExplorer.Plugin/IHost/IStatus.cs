using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IStatus
    {
        /// <summary>
        /// Set information shown on status bar
        /// </summary>
        /// <param name="info">Information to be shown on status bar</param>
        void SetStatusText(string info);
        
        /// <summary>
        /// Set information shown on status bar
        /// </summary>
        /// <param name="info">Information to be shown on status bar</param>
        /// <param name="doEvents">whether to call Application.DoEvents</param>
        void SetStatusText(string info, bool doEvents);

    }
}

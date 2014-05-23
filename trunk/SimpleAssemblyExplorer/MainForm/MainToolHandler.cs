using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using Mono.Cecil;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using DgvFilterPopup;

namespace SimpleAssemblyExplorer
{
    public class MainToolHandler
    {
        frmMain _form;
        ToolStripMenuItem mnuTools;

        public MainToolHandler(frmMain form)
        {
            _form = form;
            mnuTools = _form.ToolMenu;
        }

        int toolsStartIndex = -1;
        public void LoadTools()
        {
            if (!File.Exists(ToolFile.Default.FileName)) return;

            if (toolsStartIndex < 0)
                toolsStartIndex = mnuTools.DropDownItems.Count;

            int count = mnuTools.DropDownItems.Count;
            for (int i = toolsStartIndex; i < count; i++)
            {
                mnuTools.DropDownItems.RemoveAt(toolsStartIndex);
            }

            try
            {
                foreach (ToolItem ti in ToolFile.Default.ToolItems)
                {
                    ToolStripItem tsi;

                    if (ti.ExeFile.Equals("-"))
                    {
                        tsi = new ToolStripSeparator();
                    }
                    else
                    {
                        tsi = new ToolStripMenuItem(ti.Title);
                        ProcessStartInfo psi = new ProcessStartInfo(ti.ExeFile, ti.Argument);
                        psi.UseShellExecute = false;
                        tsi.Tag = psi;
                        tsi.Click += new EventHandler(Tool_Click);
                    }
                    mnuTools.DropDownItems.Add(tsi);
                }

            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
            finally
            {
            }
        }

        private void Tool_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            ProcessStartInfo psi = mi.Tag as ProcessStartInfo;
            if (psi == null) return;

            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }
        

    } //end of class
}

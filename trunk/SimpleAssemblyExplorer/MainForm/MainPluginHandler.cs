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
    public class MainPluginHandler 
    {
        frmMain _form;
        ContextMenuStrip cmAssembly;
        ToolStripMenuItem cmMore;

        public MainPluginHandler(frmMain form)
        {
            _form = form;
            cmAssembly = _form.AssemblyContextMenuStrip;
            cmMore = _form.AssemblyMoreMenu;
        }

        public string PluginDir
        {
            get { return PluginUtils.PluginDir; }
        }

        private int GetPluginMenuStartIndex()
        {
            return cmAssembly.Items.IndexOfKey("cmPluginSepStart") + 1;
        }

        private int GetPluginMenuEndIndex()
        {
            return cmAssembly.Items.IndexOfKey("cmPluginSepEnd") - 2;
        }

        private string GetDynPluginName(string name)
        {
            return String.Format("PLUG_{0}", name);
        }

        private bool IsDynPlugin(string name)
        {
            return name.StartsWith("PLUG_");
        }

        private void AddPluginMenu(ref int seq, ref int index, string pluginName)
        {
            PluginData pd = PluginUtils.Plugins[pluginName];
            if (pd.PluginType != PluginTypes.Main) return;
            IMainPlugin plugin = pd.PluginBase as IMainPlugin;
            if (plugin == null) return;
            MainPluginInfo pi = plugin.PluginInfo;

            string dynName = GetDynPluginName(pi.Title);

            if (cmAssembly.Items.ContainsKey(dynName)) return;
            if (cmMore.DropDownItems.ContainsKey(dynName)) return;

            ToolStripMenuItem mi = new ToolStripMenuItem();
            mi.Name = dynName;
            mi.Text = pi.Title;
            mi.Tag = plugin;
            mi.Click += new EventHandler(pluginMenu_Click);
            Bitmap bmp = pd.Icon;
            if (bmp != null)
            {
                //mi.ImageTransparentColor = Color.Fuchsia;
                mi.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                mi.Image = bmp;
            }

            if (seq >= Config.RecentPluginList)
            {
                cmMore.DropDownItems.Add(mi);
            }
            else
            {
                cmAssembly.Items.Insert(index, mi);
                seq++;
                index++;
            }
        }

        public void ClearPlugins()
        {
            cmMore.DropDownItems.Clear();

            int index = GetPluginMenuStartIndex();
            ToolStripMenuItem mi = cmAssembly.Items[index] as ToolStripMenuItem;
            while (mi != null)
            {
                if (IsDynPlugin(mi.Name))
                {
                    cmAssembly.Items.RemoveAt(index);
                    mi = cmAssembly.Items[index] as ToolStripMenuItem;
                }
                else
                {
                    break;
                }
            }
        }

        public void LoadPlugins()
        {
            PluginUtils.InitPlugins(_form);

            int seq = 0;
            int index = GetPluginMenuStartIndex();
            foreach (string recentPluginName in Config.RecentPlugins)
            {
                if (String.IsNullOrEmpty(recentPluginName)) continue;
                if (!PluginUtils.Plugins.ContainsKey(recentPluginName)) continue;

                AddPluginMenu(ref seq, ref index, recentPluginName);
            }

            foreach (string pluginName in PluginUtils.Plugins.Keys)
            {
                AddPluginMenu(ref seq, ref index, pluginName);
            }

            cmMore.Visible = cmMore.DropDownItems.Count > 0;

            //we need to reload settings here for new added plugin properties
            //Config.Reload();
        }

        void pluginMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            IMainPlugin plugin = mi.Tag as IMainPlugin;
            if (plugin == null) return;

            //prepare parameters
            string sourceDir = _form.TreeView.SelectedNode.FullPath;

            string[] rows = PathUtils.GetFullFileNames(_form.AssemblyDataGrid.SelectedRows, sourceDir);

            //run plugin
            bool resolveDirAdded = false;
            try
            {
                resolveDirAdded = _form.AddAssemblyResolveDir(PluginUtils.PluginDir);
                Config.AddRecentPlugin(plugin.PluginInfo.Title);
                PluginArgument pa = new PluginArgument(_form, rows, sourceDir);
                PluginReturns pr = plugin.Run(pa);
                if (pr == PluginReturns.Refresh)
                {
                    _form.TreeViewHandler.RefreshNode(_form.TreeView.SelectedNode, true);
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
            finally
            {
                if (resolveDirAdded)
                    _form.RemoveAssemblyResolveDir(PluginUtils.PluginDir);
            }

            //dynamic adjust menu position
            int startIndex = GetPluginMenuStartIndex();
            if (cmAssembly.Items.ContainsKey(mi.Name))
            {
                int index = cmAssembly.Items.IndexOf(mi);
                if (startIndex != index)
                {
                    cmAssembly.Items.RemoveAt(index);
                    cmAssembly.Items.Insert(startIndex, mi);
                }
            }
            else
            {
                cmMore.DropDownItems.Remove(mi);
                ToolStripMenuItem miLast = (ToolStripMenuItem)cmAssembly.Items[GetPluginMenuEndIndex()];
                cmAssembly.Items.Remove(miLast);
                cmAssembly.Items.Insert(startIndex, mi);
                cmMore.DropDownItems.Insert(0, miLast);
            }
        }


    } //end of class
}

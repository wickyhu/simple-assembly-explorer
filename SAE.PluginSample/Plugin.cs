using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;

namespace SAE.PluginSample
{
    public class Plugin : DefaultMainPlugin
    {
        public Plugin(IHost host)
            : base(host)
        {
        }      

        public override MainPluginInfo PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                {
                    _pluginInfo = new MainPluginInfo();
                    _pluginInfo.Author = SimpleDotNet.Author;
                    _pluginInfo.Contact = SimpleDotNet.EmailAddress;
                    _pluginInfo.Url = SimpleDotNet.WebSiteUrl;
                    _pluginInfo.Title = "Main Plugin Sample";
                    _pluginInfo.SourceType = SourceTypes.Assembly;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        public override PluginReturns Run(PluginArgument arg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Selected directory: \n  {0}\n\n", arg.SourceDir);

            if (arg.Rows != null || arg.Rows.Length > 0)
            {
                sb.AppendFormat("Selected file{0}:\n", (arg.Rows != null && arg.Rows.Length > 1) ? "s" : "");
                foreach (string file in arg.Rows)
                {
                    sb.AppendFormat("  {0}\n", file);
                }
            }
            
            MessageBox.Show(sb.ToString(), "Plugin Sample");
            return PluginReturns.None;
        }
    }
}

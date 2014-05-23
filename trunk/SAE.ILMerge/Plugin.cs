using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using SimpleUtils.Win;

namespace SAE.ILMerge
{
    public class Plugin : DefaultMainPlugin
    {
        public const string PropertyOutputDir = "ILMergeOutputDir";
        public const string PropertyStrongKeyFile = "ILMergeStrongKeyFile";

        public Plugin(IHost host)
            : base(host)
        {
            host.AddProperty(PropertyOutputDir, String.Empty, typeof(String));
            host.AddProperty(PropertyStrongKeyFile, String.Empty, typeof(String));
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
                    _pluginInfo.Title = "ILMerge";
                    _pluginInfo.SourceType = SourceTypes.Assembly;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        public override PluginReturns Run(PluginArgument arg)
        {
            if (arg.Rows == null || arg.Rows.Length < 2)
            {
                SimpleMessage.ShowInfo("Please select two or more assemblies to merge.");
                return PluginReturns.None;
            }

            bool resolveDirAdded = false;
            try
            {
                resolveDirAdded = arg.Host.AddAssemblyResolveDir(arg.SourceDir);
                frmILMerge frm = new frmILMerge(arg.Host, arg.Rows, arg.SourceDir);
                frm.ShowDialog();
            }
            catch
            {
                throw;
            }
            finally
            {
                if(resolveDirAdded)
                    arg.Host.RemoveAssemblyResolveDir(arg.SourceDir);
            }

            return PluginReturns.Refresh;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using SimpleUtils.Win;

namespace SAE.FileDisassembler
{
    public class Plugin : DefaultMainPlugin
    {
        public const string PropertyOutputDir = "FileDisassemblerOutputDir";

        public Plugin(IHost host)
            : base(host)
        {
            host.AddProperty(PropertyOutputDir, String.Empty, typeof(String));
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
                    _pluginInfo.Title = "File Disassembler";
                    _pluginInfo.SourceType = SourceTypes.Assembly;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        public override PluginReturns Run(PluginArgument arg)
        {
            bool resolveDirAdded = false;
            try
            {
                resolveDirAdded = arg.Host.AddAssemblyResolveDir(arg.SourceDir);
                frmFileDisassembler frm = new frmFileDisassembler(arg.Host, arg.Rows, arg.SourceDir);
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

            return PluginReturns.None;
        }

        
    }
}

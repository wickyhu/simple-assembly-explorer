using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using SimpleUtils.Win;
using System.IO;

namespace SAE.de4dot
{
    public class Plugin : DefaultMainPlugin
    {
        public const string Name = "de4dot";
        public const string PropertyRootDirectory = "de4dotRootDirectory";
        public const string PropertyAdditionalOptions = "de4dotAdditionalOptions";

        public Plugin(IHost host)
            : base(host)
        {
            host.AddProperty(PropertyRootDirectory, String.Empty, typeof(String));
            host.AddProperty(PropertyAdditionalOptions, String.Empty, typeof(String));
        }

        public string RootDirectory
        {
            get { return this.Host.GetPropertyValue(Plugin.PropertyRootDirectory) as string; }
            set { this.Host.SetPropertyValue(Plugin.PropertyRootDirectory, value); }
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
                    _pluginInfo.Title = Plugin.Name;
                    _pluginInfo.SourceType = SourceTypes.Assembly;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        public bool SelectRootDirectory(IHost host, bool force)
        {
            string path = this.RootDirectory;
            string initDir;
            if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                initDir = null;
            }
            else
            {
                initDir = path;
                if (!force && IsValidDir(initDir))
                {
                    return true;
                }
            }

            path = SimpleDialog.OpenFolder(initDir, "Please specify de4dot's root directory.");
            if (String.IsNullOrEmpty(path) || !Directory.Exists(path)) 
                return false;

            if (!IsValidDir(path)) 
                return false;

            this.RootDirectory = path;
            return true;
        }

        public bool IsValidDir(string rootDir)
        {
            var files = Directory.GetFiles(rootDir, Plugin.Name + "*.exe");
            if (files == null || files.Length == 0)
                return false;
            return true;
        }

        public override PluginReturns Run(PluginArgument arg)
        {
            try
            {
                if (!SelectRootDirectory(arg.Host, false))
                {
                    if (SimpleMessage.Confirm("Failed to locate de4dot! Do you want to download now?") == System.Windows.Forms.DialogResult.Yes)
                    {
                        SimpleProcess.Start("https://bitbucket.org/0xd4d/de4dot/downloads");
                    }
                    return PluginReturns.None;
                }

                var frm = new frmDe4dot(arg.Host, arg.Rows, arg.SourceDir);
                frm.ShowDialog();
                return PluginReturns.Refresh;
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }
        
    }
}

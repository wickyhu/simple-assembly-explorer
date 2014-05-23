using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.LutzReflector;
using SimpleAssemblyExplorer.Plugin;

namespace SAE.Reflector
{
     public class Plugin : DefaultMainPlugin
    {
        public const string PropertyReflector = Consts.Reflector;

        public Plugin(IHost host)
            : base(host)
        {
            host.AddProperty(PropertyReflector, String.Empty, typeof(String));
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
                    _pluginInfo.Title = "Reflector";
                    _pluginInfo.SourceType = SourceTypes.Assembly;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        private string GetReflector(IHost host)
        {
            return host.GetPropertyValue(Plugin.PropertyReflector) as string;            
        }

        private void SetReflector(IHost host, string path)
        {
            //already set in SimpleReflector.OpenReflector
            //host.SetPropertyValue(Plugin.PropertyReflector, path);

            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                host.AddAssemblyResolveDir(Path.GetDirectoryName(path));
            }
        }

        public bool SelectReflector(IHost host, bool force)
        {
            string path = GetReflector(host);
            string initDir;
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                initDir = null;
            }
            else
            {
                if (!force && File.Exists(path))
                {
                    return true;
                }
                initDir = Path.GetDirectoryName(path);
            }

            path = SimpleReflector.OpenReflector(initDir);
            if (!File.Exists(path)) 
                return false;

            SetReflector(host, path);
            return true;
        }      

        public override PluginReturns Run(PluginArgument arg)
        {
            if (arg.Rows == null || arg.Rows.Length < 1)
            {
                SimpleMessage.ShowInfo("Please select file to open with Reflector.");
                return PluginReturns.None;
            }

            try
            {
                if (!this.SelectReflector(arg.Host, false))
                {
                    if (SimpleMessage.Confirm("Failed to locate .Net Reflector! Do you want to download now?") == System.Windows.Forms.DialogResult.Yes)
                    {
                        SimpleProcess.Start("http://www.reflector.net/");
                    }
                    return PluginReturns.None;
                }

                string path = GetReflector(arg.Host);
                
                try
                {
                    if (!RemoteController.Available)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = path;
                        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                        p.Start();
                    }
                    
                    int count = 0;
                    while (!RemoteController.Available && count < 60)
                    {
                        Thread.Sleep(500);
                        count++;
                    }                    

                    if (RemoteController.Available)
                    {
                        for (int i = 0; i < arg.Rows.Length; i++)
                        {
                            RemoteController.LoadAssembly(Path.Combine(arg.SourceDir, arg.Rows[i]));
                        }                     
                    }
                }
                catch (Exception ex)
                {
                    SimpleMessage.ShowException(ex);
                }
            }
            catch
            {
                throw;
            }

            return PluginReturns.None;
        }
     
    }
}

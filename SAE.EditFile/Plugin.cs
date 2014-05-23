using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils.Win;
using SimpleUtils;

namespace SAE.EditFile
{
    public class Plugin : DefaultMainPlugin
    {
        public const string PropertyEditor = "Editor";

        public Plugin(IHost host)
            : base(host)
        {
            host.AddProperty(PropertyEditor, String.Empty, typeof(String));
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
                    _pluginInfo.Title = "Edit File";
                    _pluginInfo.SourceType = SourceTypes.Any;
                    _pluginInfo.RowType = RowTypes.Multiple;
                }
                return _pluginInfo;
            }
        }

        private string GetEditor(IHost host)
        {
            return host.GetPropertyValue(Plugin.PropertyEditor) as string;            
        }

        private void SetEditor(IHost host, string editor)
        {
            host.SetPropertyValue(Plugin.PropertyEditor, editor);
        }

        public bool SelectEditor(IHost host, bool force)
        {
            string path = GetEditor(host);
            string initDir;
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                initDir = null;
            }
            else
            {
                if (force)
                    initDir = Path.GetDirectoryName(path);
                else
                    return true;
            }

            path = SimpleDialog.OpenFile("Where is Editor?", "Exe files (*.exe)|*.exe", ".exe", true, initDir);
            if (String.IsNullOrEmpty(path)) return false;

            SetEditor(host, path);
            return true;
        }

        public override PluginReturns Run(PluginArgument arg)
        {
            if (arg.Rows == null || arg.Rows.Length < 1)
            {
                SimpleMessage.ShowInfo("Please select file to edit.");
                return PluginReturns.None;
            }

            try
            {
                if (!this.SelectEditor(arg.Host, false))
                    return PluginReturns.None;

                string path = GetEditor(arg.Host);
                
                try
                {                    
                    Process p = new Process();
                    p.StartInfo.FileName = path;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                    p.StartInfo.Arguments = String.Empty;
                    foreach (string file in arg.Rows)
                    {
                        p.StartInfo.Arguments = String.Format("{0} \"{1}\" ",
                            p.StartInfo.Arguments,
                            Path.Combine(arg.SourceDir,file));
                    }
                    p.Start();
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

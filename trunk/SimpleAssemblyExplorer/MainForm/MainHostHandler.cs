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
using Microsoft.WindowsAPICodePack.Taskbar;

namespace SimpleAssemblyExplorer
{
    public class MainHostHandler
    {
        frmMain _form;
        ToolStripStatusLabel statusInfo;
        ToolStripProgressBar statusProgress;

        public MainHostHandler(frmMain form)
        {
            _form = form;
            statusInfo = _form.StatusInfo;
            statusProgress = _form.StatusProgressBar;

            SetStatusVersion();
        }

        #region IStatus
        public void SetStatusText(string info)
        {
            SetStatusText(info, true);
        }

        public void SetStatusText(string info, bool doEvents)
        {
            if (String.IsNullOrEmpty(info)) info = "Ready.";
            statusInfo.Text = info.Replace('\r', ' ').Replace('\n', ' ');
            if (doEvents) 
                Application.DoEvents();
        }

        private void SetStatusVersion()
        {
            _form.StatusVersion.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public void AppendStatusText(string info)
        {
            AppendStatusText(info, true);
        }

        public void AppendStatusText(string info, bool doEvents)
        {
            if (String.IsNullOrEmpty(info)) return;
            statusInfo.Text += info;
            if (doEvents) 
                Application.DoEvents();
        }
        #endregion IStatus

        #region IProgressBar
        public void InitProgress(int min, int max)
        {
            statusProgress.Maximum = max;
            statusProgress.Minimum = min;

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                TaskbarManager.Instance.SetProgressValue(min, max);
            }
        }

        public void SetProgress(int val)
        {
            SetProgress(val, true);
        }

        public void SetProgress(int val, bool doEvents)
        {
            if (val < statusProgress.Minimum) val = statusProgress.Minimum;
            if (val > statusProgress.Maximum) val = statusProgress.Maximum;
            statusProgress.Value = val;

            if (val == statusProgress.Minimum)
            {
                statusProgress.Visible = true;
            }
            else if (val == statusProgress.Maximum)
            {
                statusProgress.Visible = false;
            }

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressValue(val, statusProgress.Maximum);
            }

            if (doEvents)
                Application.DoEvents();
        }

        public void ResetProgress()
        {
            statusProgress.Visible = false;
            InitProgress(0, 100);

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressValue(0, statusProgress.Maximum);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }

        }
        #endregion IProgressBar

        #region IAssemblyResolveDir
        public bool AddAssemblyResolveDir(string dir)
        {
            return _coreAssemblyResolver.AddAssemblyResolveDir(dir);
        }

        public bool RemoveAssemblyResolveDir(string dir)
        {
            return _coreAssemblyResolver.RemoveAssemblyResolveDir(dir);
        }

        CoreAssemblyResolver _coreAssemblyResolver = null;

        public void SetupAssemblyResolver()
        {
            _coreAssemblyResolver = new CoreAssemblyResolver(CurrentDomain_AssemblyResolve);
        }

        public void ReleaseAssemblyResolver()
        {
            _coreAssemblyResolver.Dispose();
            _coreAssemblyResolver = null;
        }

        private bool IsResourcesAssembly(string name)
        {
            if (name == null) 
                return false;
            string[] s = name.Split(',');
            if (s == null || s.Length < 1 || String.IsNullOrEmpty(s[0]))
                return false;
            string asmName = s[0].TrimEnd();
            return asmName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase);            
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly a = _coreAssemblyResolver.DefaultResolveEventHandler(sender, args);
            if (a != null) return a;

            a = AssemblyUtils.ResolveAssemblyFile(args.Name, _form.TreeViewHandler.CurrentPath);
            if (a != null) return a;

            if (args.Name.IndexOf("Reflector,") >= 0)
            {
                string path = SimpleReflector.OpenReflector();
                if (!String.IsNullOrEmpty(path))
                {
                    path = Path.GetDirectoryName(path);
                    AddAssemblyResolveDir(path);

                    a = AssemblyUtils.ResolveAssemblyFile(args.Name, path);
                    if (a != null) return a;
                }
            }

            if (_form.TextInfo != null && !IsResourcesAssembly(args.Name))
            {
                string message = "Could not resolve: " + args.Name;
                if (!_form.TextInfo.TextInfo.Contains(message))
                {
                    _form.TextInfo.AppendTextInfoLine(message);
                }
            }
            return null;
        }
        #endregion IAssemblyResolveDir

        #region IPropertyValue
        public bool AddProperty(string propertyName, object defaultValue, Type propertyType)
        {
            return Config.AddProperty(propertyName, defaultValue, propertyType);
        }

        public void RemoveProperty(string propertyName)
        {
            Config.RemoveProperty(propertyName);
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            Config.SetPropertyValue(propertyName, value);
        }

        public object GetPropertyValue(string propertyName)
        {
            return Config.GetPropertyValue(propertyName);
        }
        #endregion IPropertyValue

    } //end of class
}

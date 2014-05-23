using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class frmProfilerApp : frmBase
    {
        public frmProfilerApp(string appFile)
        {
            InitializeComponent();

            InitForm(appFile);
        }

        private void InitForm(string appFile)
        {
            if (!String.IsNullOrEmpty(appFile) && File.Exists(appFile))
            {
                txtApp.Text = appFile;
            }
            else
            {
                txtApp.Text = Config.ProfilerAppFile;
            }

            txtArg.Text = Config.ProfilerAppArgument;
            txtFilter.Text = Config.ProfilerAppFilter;
            txtLogPath.Text = Config.ProfilerAppLogPath;

            chkTraceEvent.Checked = Config.ProfilerAppTraceEvent;
            chkTraceParam.Checked = Config.ProfilerAppTraceParameter;
            chkIncludeSystem.Checked = Config.ProfilerAppIncludeSystem;


            if (Directory.Exists(txtLogPath.Text))
            {
                _logFile = Path.Combine(txtLogPath.Text, ProfilerUtils.PROFILER_LOG);
            }
            else if (File.Exists(txtApp.Text))
            {
                _logFile = Path.Combine(Path.GetDirectoryName(txtApp.Text), ProfilerUtils.PROFILER_LOG);
            }

            txtLogFile.Text = _logFile;
        }

        private void btnSelectApp_Click(object sender, EventArgs e)
        {
            string initDir = null;
            if (!String.IsNullOrEmpty(txtApp.Text) && File.Exists(txtApp.Text))
            {
                initDir = Path.GetDirectoryName(txtApp.Text);
            }

            string path = SimpleDialog.OpenFile(this.Text, Consts.FilterExeFile, ".exe", true, initDir);

            if (!String.IsNullOrEmpty(path))
            {
                txtApp.Text = path;                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
            this.Close();
        }

        private Process profiledProcess;

        private bool ProfiledProcessHasExited()
        {
            if (profiledProcess == null)
                return true;

            try
            {
                return profiledProcess.HasExited;
            }
            catch
            {
                return Process.GetProcessById(profiledProcess.Id) == null;
            }

        }

        private string _logFile = null;

        bool _inStarting = false;
        private void btnStart_Click(object sender, EventArgs e)
        {
            string appFile = txtApp.Text;

            if (String.IsNullOrEmpty(appFile) || !File.Exists(appFile))
            {
                SimpleMessage.ShowError("Cannot find application!");
                return;
            }
            if (!String.IsNullOrEmpty(txtLogPath.Text) && !Directory.Exists(txtLogPath.Text))
            {
                SimpleMessage.ShowError("Invalid log path!");
                return;
            }

            if (_inStarting) return;

            try
            {
                _inStarting = true;

                Config.ProfilerAppFile = txtApp.Text;
                Config.ProfilerAppArgument = txtArg.Text;
                Config.ProfilerAppFilter = txtFilter.Text;
                Config.ProfilerAppLogPath = txtLogPath.Text;

                Config.ProfilerAppTraceEvent = chkTraceEvent.Checked;
                Config.ProfilerAppTraceParameter = chkTraceParam.Checked;
                Config.ProfilerAppIncludeSystem = chkIncludeSystem.Checked;

                ProfilerUtils.RegisterProfiler();

                if (profiledProcess == null || ProfiledProcessHasExited())
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(appFile);
                    processStartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
                    processStartInfo.EnvironmentVariables["COR_PROFILER"] = Consts.ProfilerGuid;
                    if (!String.IsNullOrEmpty(txtArg.Text))
                        processStartInfo.Arguments = txtArg.Text;

                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(appFile);

                    if (!String.IsNullOrEmpty(txtLogPath.Text) && Directory.Exists(txtLogPath.Text))
                    {
                        _logFile = Path.Combine(txtLogPath.Text, ProfilerUtils.PROFILER_LOG);
                    }
                    else
                    {
                        _logFile = Path.Combine(processStartInfo.WorkingDirectory, ProfilerUtils.PROFILER_LOG);
                    }
                    txtLogFile.Text = _logFile;
                    processStartInfo.EnvironmentVariables["SP_LOG_PATH"] = Path.GetDirectoryName(txtLogFile.Text);

                    if (File.Exists(_logFile))
                    {
                        File.Delete(_logFile);
                    }

                    EnsureLogStatus();

                    if (!String.IsNullOrEmpty(txtFilter.Text))
                    {
                        processStartInfo.EnvironmentVariables["SP_FILTER"] = txtFilter.Text.Trim();
                    }

                    processStartInfo.EnvironmentVariables["SP_INCLUDE_SYSTEM"] = (chkIncludeSystem.Checked ? "1" : "0");
                    processStartInfo.EnvironmentVariables["SP_TRACE_PARAMETER"] = (chkTraceParam.Checked ? "1" : "0");
                    processStartInfo.EnvironmentVariables["SP_TRACE_EVENT"] = (chkTraceEvent.Checked ? "1" : "0");

                    processStartInfo.UseShellExecute = false;
                    profiledProcess = Process.Start(processStartInfo);

                    timer1.Enabled = true;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    btnDeleteLogFile.Enabled = false;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _inStarting = false;
            }

        }

        private void ProfilerStopped() 
        {
            timer1.Enabled = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnDeleteLogFile.Enabled = true;

            try
            {
                string path = GetStatusFilePath();
                ProfilerUtils.DeleteFile(path);
            }
            catch { }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblLogSize.Text = PathUtils.GetFileSizeInfo(_logFile);
            
            if (profiledProcess == null || profiledProcess.HasExited)
            {
                ProfilerStopped();
            }
        }

        private void frmProfilerApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnStop_Click(sender, e);
        }

        private void btnOpenLogFile_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_logFile) && File.Exists(_logFile))
            {
                SimpleProcess.Start(_logFile);
            }
        }

        bool _inStopping = false;
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_inStopping) return;
            try
            {
                _inStopping = true;

                timer1.Enabled = false;

                if (profiledProcess != null && !profiledProcess.HasExited)
                {
                    profiledProcess.Kill();
                    profiledProcess = null;
                }

                ProfilerStopped();
            }
            catch
            {
                throw;
            }
            finally
            {
                _inStopping = false;
            }
        }

        private void chkTraceParam_CheckedChanged(object sender, EventArgs e)
        {
            chkIncludeSystem.Enabled = chkTraceParam.Checked;
            txtFilter.Enabled = chkTraceParam.Checked;
        }

        private void btnDeleteLogFile_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_logFile) && File.Exists(_logFile))
            {
                File.Delete(_logFile);
                lblLogSize.Text = "N/A";
            }
        }

        private void txtLogPath_Leave(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLogPath.Text) && !txtLogPath.Text.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                txtLogPath.Text += Path.DirectorySeparatorChar.ToString();
            }
        }

        private void chkLogEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnStart.Enabled)
            {
                EnsureLogStatus();
            }
        }

        private string GetStatusFilePath()
        {
            string path = Path.GetDirectoryName(txtLogFile.Text);
            path = Path.Combine(path, ProfilerUtils.PROFILER_STATUS);
            return path;
        }

        private void EnsureLogStatus()
        {
            string path = GetStatusFilePath();
            if (chkLogEnabled.Checked)
            {
                ProfilerUtils.CreateFile(path);
            }
            else
            {
                ProfilerUtils.DeleteFile(path);
            }
        }

    } // end of class
}
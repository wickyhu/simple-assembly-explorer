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
using System.Threading;
using System.Xml;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class frmProfilerASPNet : frmBase
    {
        frmMain _mainForm = null;

        public frmProfilerASPNet(frmMain mainForm)
        {
            InitializeComponent();

            InitForm(mainForm);
        }

        private void InitForm(frmMain mainForm)
        {
            _mainForm = mainForm;

            txtFilter.Text = Config.ProfilerASPNetFilter;
            txtLogPath.Text = Config.ProfilerASPNetLogPath;

            chkTraceEvent.Checked = Config.ProfilerASPNetTraceEvent;
            chkTraceParam.Checked = Config.ProfilerASPNetTraceParameter;
            chkIncludeSystem.Checked = Config.ProfilerASPNetIncludeSystem;


            if (Directory.Exists(txtLogPath.Text))
            {
                _logFile = Path.Combine(txtLogPath.Text, ProfilerUtils.PROFILER_LOG);
            }

            txtLogFile.Text = _logFile;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
            this.Close();
        }

        private string _logFile = null;

        bool _inStarting = false;
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLogPath.Text) && !Directory.Exists(txtLogPath.Text))
            {
                SimpleMessage.ShowError("Invalid log path!");
                return;
            }

            if (_inStarting) return;

            try
            {
                _inStarting = true;

                // save config
                Config.ProfilerASPNetFilter = txtFilter.Text;
                Config.ProfilerASPNetLogPath = txtLogPath.Text;

                Config.ProfilerASPNetTraceEvent = chkTraceEvent.Checked;
                Config.ProfilerASPNetTraceParameter = chkTraceParam.Checked;
                Config.ProfilerASPNetIncludeSystem = chkIncludeSystem.Checked;


                //register profiler
                ProfilerUtils.RegisterProfiler();

                //stop iis
                StopIIS();

                //get log file
                string logDir = txtLogPath.Text;
                if (String.IsNullOrEmpty(logDir) || !Directory.Exists(logDir))
                {
                    logDir = PathUtils.GetTempDir();
                }
                _logFile = Path.Combine(logDir, ProfilerUtils.PROFILER_LOG);
                txtLogFile.Text = _logFile;

                if (File.Exists(_logFile))
                {
                    File.Delete(_logFile);
                }

                EnsureLogStatus();

                // set environment variables
                string[] profilerEnvironment = new string[]
                { 
                "COR_ENABLE_PROFILING=0x1",
                "COR_PROFILER="+Consts.ProfilerGuid,
                "SP_LOG_PATH="+logDir,
                "SP_FILTER="+txtFilter.Text.Trim(),
                "SP_INCLUDE_SYSTEM=" + (chkIncludeSystem.Checked?"1":"0"),
                "SP_TRACE_PARAMETER=" + (chkTraceParam.Checked ? "1" : "0"),
                "SP_TRACE_EVENT=" + (chkTraceEvent.Checked ? "1" : "0")
                };

                string[] baseEnvironment = ProfilerUtils.GetServicesEnvironment();
                baseEnvironment = ProfilerUtils.ReplaceTempDir(baseEnvironment, PathUtils.GetTempDir());
                string[] combinedEnvironment = ProfilerUtils.CombineEnvironmentVariables(baseEnvironment, profilerEnvironment);
                ProfilerUtils.SetEnvironmentVariables("IISADMIN", combinedEnvironment);
                ProfilerUtils.SetEnvironmentVariables("W3SVC", combinedEnvironment);
                ProfilerUtils.SetEnvironmentVariables("WAS", combinedEnvironment);

                string asp_netAccountName = GetASPNetAccountName();
                string asp_netAccountSid = null;
                if (asp_netAccountName != null)
                {
                    asp_netAccountSid = ProfilerUtils.LookupAccountSid(asp_netAccountName);
                    if (asp_netAccountSid != null)
                        ProfilerUtils.SetAccountEnvironment(asp_netAccountSid, profilerEnvironment);
                }

                if (StartIIS())
                {
                    //_mainForm.SetStatusText("It's time to load your ASP.Net page.");
                    lblLogSize.Text = "It's time to load your ASP.Net page.";

                    Thread.Sleep(1000);

                    timer1.Enabled = true;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    btnDeleteLogFile.Enabled = false;
                }

                /* Delete the environment variables as early as possible, so that even if CLRProfiler crashes, the user's machine
                 * won't be screwed up.
                 * */

                ProfilerUtils.DeleteEnvironmentVariables("IISADMIN");
                ProfilerUtils.DeleteEnvironmentVariables("W3SVC");
                ProfilerUtils.DeleteEnvironmentVariables("WAS");

                if (asp_netAccountSid != null)
                    ProfilerUtils.ResetAccountEnvironment(asp_netAccountSid, profilerEnvironment);
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
            string info = PathUtils.GetFileSizeInfo(_logFile);
            if (!String.IsNullOrEmpty(info))
            {
                lblLogSize.Text = info;
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

                if (btnStop.Enabled)
                {
                    StopIIS();
                    StartIIS();
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

        private void AppendStatusText(string info)
        {
            _mainForm.HostHandler.AppendStatusText(info);
        }

        private void StopIIS()
        {
            // stop IIS
            _mainForm.SetStatusText("Stopping IIS ");
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            if (Environment.OSVersion.Version.Major >= 6/*Vista*/)
                processStartInfo.Arguments = "/c net stop was /y";
            else
                processStartInfo.Arguments = "/c net stop iisadmin /y";
            //processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            Process process = Process.Start(processStartInfo);
            while (!process.HasExited)
            {
                AppendStatusText(".");
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            if (process.ExitCode != 0)
            {
                AppendStatusText(String.Format(" Error {0} occurred.", process.ExitCode));
            }
            else
                AppendStatusText("IIS stopped.");
        }

        private bool StartIIS()
        {
            _mainForm.SetStatusText("Starting IIS ");
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.Arguments = "/c net start w3svc";
            //processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            Process process = Process.Start(processStartInfo);
            while (!process.HasExited)
            {
                AppendStatusText(".");
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            if (process.ExitCode != 0)
            {
                AppendStatusText(string.Format(" Error {0} occurred.", process.ExitCode));
                return false;
            }
            AppendStatusText("IIS running.");
            return true;
        }

        private string GetASPNetAccountName()
        {
            try
            {
                XmlDocument machineConfig = new XmlDocument();
                string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
                string configPath = Path.Combine(runtimePath, @"CONFIG\machine.config");
                machineConfig.Load(configPath);
                XmlNodeList elemList = machineConfig.GetElementsByTagName("processModel");
                for (int i = 0; i < elemList.Count; i++)
                {
                    XmlAttributeCollection attributes = elemList[i].Attributes;
                    XmlAttribute userNameAttribute = attributes["userName"];
                    if (userNameAttribute != null)
                    {
                        string userName = userNameAttribute.InnerText;
                        if (userName == "machine")
                            return "ASPNET";
                        else if (userName == "SYSTEM")
                            return null;
                        else
                            return userName;
                    }
                }
            }
            catch
            {
                // swallow all exceptions here
            }
            return "ASPNET";
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
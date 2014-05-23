using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleUtils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public abstract class CommandTool
    {
        public OptionsBase Options { get; private set; }

        public CommandTool(OptionsBase options)
        {
            Options = options;
            ShowWindow = false;
        }

        public abstract string ExeFile { get; }

        public bool ShowWindow { get; set; }

        public virtual void SetTextInfo(string text)
        {
            Options.SetTextInfo(text);
        }

        public virtual void AppendTextInfo(string text)
        {
            text = Regex.Replace(text, "(?<!\r)\n", "\r\n");
            Options.AppendTextInfo(text);
        }

        public virtual void ShowStartTextInfo()
        {
            if (Options.ShowStartCompleteTextInfo)
                SetTextInfo(String.Format("=== Started at {0} ===\r\n\r\n", DateTime.Now));
        }

        public virtual void ShowCompleteTextInfo()
        {
            if (Options.ShowStartCompleteTextInfo)
                AppendTextInfo(String.Format("\r\n=== Completed at {0} ===\r\n\r\n", DateTime.Now));
        }

        public virtual void InitProgress(int max)
        {
            Options.Host.InitProgress(0, max);
            Options.Host.SetProgress(0);
        }

        public virtual void SetProgress(int v)
        {
            Options.Host.SetProgress(v, true);
        }

        public virtual void SetStatusText(string text)
        {
            Options.Host.SetStatusText(text, true);
        }

        public virtual void ResetProgress()
        {
            Options.Host.ResetProgress();
        }

        public virtual string PrepareArguments(string sourceFile)
        {
            return String.Empty;
        }

        public virtual void OnProcessStart(Process p)
        {
        }

        public virtual void OnProcessEnd(Process p)
        {
        }


        public virtual string GetSourceFile(string fileName)
        {
            return File.Exists(fileName) ? fileName : Path.Combine(Options.SourceDir, fileName);
        }

        public virtual int Go(string sourceFile)
        {
            Process p = CreateProcess();
            p.StartInfo.Arguments = PrepareArguments(sourceFile);
                     
            ShowProcessTextInfo(p);            
            OnProcessStart(p);
            p.Start();
            WaitForProcess(p);
            int r = p.ExitCode;
            if (r != 0)
            {
               Options.AppendTextInfoLine(String.Format("{0} exited with {1}", Path.GetFileNameWithoutExtension(ExeFile), r));
            }
            
            OnProcessEnd(p);
            return r;
        }

        public virtual void Go()
        {
            var rows = Options.Rows;
            
            //no rows
            if (rows == null || rows.Length == 0 || Options.IgnoreRows)
            {
                try
                {
                    ShowStartTextInfo();
                    Go(String.Empty);
                }
                catch { throw; }
                finally
                {
                   ShowCompleteTextInfo();
                }
                return;
            }

            //has rows
            bool showProgress = (rows != null && rows.Length > 1);
            try
            {
                ShowStartTextInfo();

                if (showProgress)
                {
                    InitProgress(rows.Length);
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    string fileName = PathUtils.GetFullFileName(rows, i, Options.SourceDir);
                    if (String.IsNullOrEmpty(fileName)) continue;

                    fileName = GetSourceFile(fileName);

                    SetStatusText(String.Format("Processing {0} ...", fileName));

                    if (Options.ShowFileNoTextInfo)
                    {
                        AppendTextInfo(String.Format("\r\nFile: {0}\r\n", i + 1));
                    }

                    Go(fileName);

                    //if (Options.ShowFileNoTextInfo)
                    //{
                        //AppendTextInfo("\r\n\r\n");
                    //}

                    if (showProgress)
                    {
                        SetProgress(i + 1);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (showProgress)
                    ResetProgress();
                ShowCompleteTextInfo();
            }
        }

        public virtual Process CreateProcess()
        {
            Process p = new Process();
            p.StartInfo.FileName = ExeFile;
            p.StartInfo.WorkingDirectory = Options.SourceDir;

            p.StartInfo.UseShellExecute = false;

            if (this.ShowWindow)
            {
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.RedirectStandardOutput = false;
            }
            else
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += OutputDataReceived;
                p.ErrorDataReceived += ErrorDataReceived;
            }
            
            return p;
        }       

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Options.AppendTextInfoLine(e.Data);
            }
        }
        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Options.AppendTextInfoLine(e.Data);
            }
        }
		
         public virtual void ShowProcessOutput(Process p)
        {
            //if (p.StartInfo.RedirectStandardOutput)
            //{
            //    StreamReader sr = p.StandardOutput;
            //    string output = sr.ReadToEnd();
			// if (!String.IsNullOrEmpty(output))
			//	{
            //    AppendTextInfo(output);
            //    AppendTextInfo("\r\n");
			//	}
            //}

            //if (p.StartInfo.RedirectStandardError)
            //{
            //    StreamReader sr = p.StandardError;
            //    string output = sr.ReadToEnd();
            //    if (!String.IsNullOrEmpty(output))
            //    {
            //        AppendTextInfo("\r\n");
            //        AppendTextInfo(output);
            //        AppendTextInfo("\r\n");
            //    }
            //}
        }        

        public virtual void WaitForProcess(Process p)
        {
            if (!this.ShowWindow)
            {
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }
            for (int i = 0; i < 3600 * 100; i++ )
            {
                //p.WaitForExit(10);
                Thread.Sleep(10);
                Application.DoEvents();
                //p.Refresh();
                if (p.HasExited)
                {
                    break;
                }
            }

            if (p.HasExited)
            {                
                Thread.Sleep(100);
                Application.DoEvents();
            }
            else
            {
                p.Kill();
                AppendTextInfo("Process killed.");
            }

            ShowProcessOutput(p);
            Application.DoEvents();
        }

        public virtual void ShowProcessTextInfo(Process p)
        {
            AppendTextInfo(String.Format("\r\n{0} {1}\r\n\r\n", p.StartInfo.FileName, p.StartInfo.Arguments));
        }

        public virtual string HelpArgument
        {
            get { return "/?"; }
        }

        public virtual void Help()
        {
            var savedShowWindow = this.ShowWindow;
            this.ShowWindow = false;
            Process p = CreateProcess();
            this.ShowWindow = savedShowWindow;

            p.StartInfo.Arguments = this.HelpArgument;            

            SetTextInfo(String.Empty);
            ShowProcessTextInfo(p);
            p.Start();
            WaitForProcess(p);

            Options.ScrollTextInfoToTop();
            Application.DoEvents();
        }


    } //end of class
}

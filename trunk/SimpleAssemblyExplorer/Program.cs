using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Configuration;
using SimpleUtils;
using SimpleUtils.Win;
using System.Runtime.InteropServices;

namespace SimpleAssemblyExplorer
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
       
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Config.Init();
                PathUtils.SetupFrameworkSDKPath();

                Application.Run(new frmMain(args));

                Config.Save();
            }
            catch (ConfigurationException)
            { 
                //ignore configuration save error
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SimpleMessage.ShowException(e.ExceptionObject as Exception);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            SimpleMessage.ShowException(e.Exception);
        }//end of Main

    }//end of class
}
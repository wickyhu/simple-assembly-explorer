using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Web;

namespace SimpleUtils
{
    public class SimpleProcess
    {
        public static Process Start(string fileName)
        {
            return Start(fileName, null);
        }

        public static Process Start(string fileName, string arguments)
        {
            Process p = new Process();
            p.StartInfo.FileName = fileName;
            if (!String.IsNullOrEmpty(arguments))
                p.StartInfo.Arguments = arguments;
            p.Start();
            return p;
        }

        public static void OpenWebSite(string url)
        {
            SimpleProcess.Start(url);
        }

    }//end of class
}

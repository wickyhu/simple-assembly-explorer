using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace SimpleUtils
{
    public class SimpleSettings
    {
        public static void RemovePreviousConfigFile(LocalFileSettingsProvider lfsp)
        {
            if (lfsp == null) return;

            Type t = lfsp.GetType();

            object o = t.InvokeMember("GetPreviousConfigFileName",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,
                null,
                lfsp,
                new object[] { true }
            );

            string configFile = o as string;
            if (!String.IsNullOrEmpty(configFile) && File.Exists(configFile))
            {
                Directory.Delete(Path.GetDirectoryName(configFile), true);
            }

            o = t.InvokeMember("GetPreviousConfigFileName",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,
                null,
                lfsp,
                new object[] { false }
            );
            configFile = o as string;
            if (!String.IsNullOrEmpty(configFile) && File.Exists(configFile))
            {
                Directory.Delete(Path.GetDirectoryName(configFile), true);
            }
        }

    }//end of class
}

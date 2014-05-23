using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TestProject
{
    public class Utils
    {
        public static void ConsoleOutput(IEnumerable<string> messages)
        {
            foreach (string s in messages)
                ConsoleOutput(s);
        }

        public static void ConsoleOutput(string format, params object[] args)
        {
            if (format == null) return;
            Console.WriteLine(String.Format(format, args));
        }


        public static void DebugOutput(IEnumerable<string> messages)
        {
            foreach (string s in messages)
                DebugOutput(s);
        }

        public static void DebugOutput(string format, params object[] args)
        {
            if (format == null) return;
            Debug.WriteLine(String.Format(format, args));
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static void DeleteFile(string[] files)
        {
            foreach (string file in files)
            {
                DeleteFile(file);
            }
        }
       

    }//end of class
}

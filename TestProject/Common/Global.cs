using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssemblyExplorer;

namespace TestProject
{
    [TestClass]
    public class Global
    {
        public static string SolutionDir { get; private set; }
        public static string MainProjectDir { get; private set; }
        public static string TestProjectDir { get; private set; }
        public static string TestSampleDir { get; private set; }
        public static string AssemblyDir { get; private set; }
        public static string TempDir { get; private set; }

        public static string Reflector { get; private set; }
        public static string TestKeyFile { get; private set; }
        public static string TestSampleFile { get; private set; }

        public static Assembly EntryAssembly { get; private set; }
        public static TestHost DefaultHost { get; private set; }

        [AssemblyInitialize()]
        public static void TestInit(TestContext testContext)
        {
            EntryAssembly = Assembly.GetExecutingAssembly();
            int p = EntryAssembly.Location.IndexOf("TestResults");
            if (p < 0)
            {
                p = EntryAssembly.Location.IndexOf("TestProject\\bin");
            }
            string rootPath = EntryAssembly.Location.Substring(0, p);

            if (Directory.Exists(rootPath))
            {
                SolutionDir = rootPath;
                MainProjectDir = Path.Combine(SolutionDir, "SimpleAssemblyExplorer");
                TestProjectDir = Path.Combine(SolutionDir, "TestProject");
                TestSampleDir = Path.Combine(SolutionDir, "TestSample");
                AssemblyDir = Path.Combine(TestProjectDir, "Assembly");
                TempDir = Path.Combine(TestProjectDir, "Temp");
                Reflector = Path.Combine(SolutionDir, "support\\Reflector.exe");
                TestKeyFile = Path.Combine(SolutionDir, "support\\test.snk");
                TestSampleFile = Path.Combine(SolutionDir, "TestSample\\bin\\TestSample.dll");

                if (!Directory.Exists(TempDir))
                {
                    Directory.CreateDirectory(TempDir);
                }

                DefaultHost = new TestHost();
                DefaultHost.AddAssemblyResolveDir(AssemblyDir);
                DefaultHost.AddAssemblyResolveDir(Path.GetDirectoryName(Reflector));

                TextFile.ResourceAssembly = EntryAssembly;

                string[] txtFiles = Directory.GetFiles(MainProjectDir, "*.txt");
                foreach (string txtFile in txtFiles)
                {
                    string destFile = Path.Combine(Path.GetDirectoryName(EntryAssembly.Location), Path.GetFileName(txtFile));
                    File.Copy(txtFile, destFile, true);
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to detect root path of test project.");
            }

            SimpleAssemblyExplorer.PathUtils.SetupFrameworkSDKPath();
        }

        public static void SetOptions(OptionsBase options)
        {
            options.Host = Global.DefaultHost;
            options.SourceDir = Global.AssemblyDir;
            options.OutputDir = Global.TempDir;            
        }

    } //end of class

}

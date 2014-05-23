using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using SimpleUtils;
using SimpleUtils.Win;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;

namespace SimpleAssemblyExplorer
{
    public class PathUtils
    {
        public static bool IsExt(string fileName, string extName)
        {
            string ext = Path.GetExtension(fileName);
            return ext.ToLowerInvariant() == extName.ToLowerInvariant();
        }

        public static bool IsDll(string fileName)
        {
            return IsExt(fileName, ".dll");
        }

        public static bool IsExe(string fileName)
        {
            return IsExt(fileName, ".exe");
        }

        public static bool IsNetModule(string fileName)
        {
            return IsExt(fileName, ".netmodule");
        }

        public static bool IsAssembly(string fileName)
        {
            bool isAssembly = false;
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".dll":
                case ".exe":
                case ".netmodule":
                    isAssembly = true;
                    break;
                default:
                    break;
            }
            return isAssembly;
        }

        //static Regex regexValidFileName = new Regex(@"^[0-9a-zA-Z_$.\u4E00-\u9FA5]+$");
        //static Regex regexInvalidFileNameChars = new Regex("[^0-9a-zA-Z_$.\u4E00-\u9FA5]+");

        public static string FixFileName(string fileName)
        {
            string s = fileName;
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char ch in invalidChars)
            {
                s = s.Replace(ch, '_');
            }
            return s;
            //return regexInvalidFileNameChars.Replace(fileName, "_");
        }    

        public static bool IsValidFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return fileName.IndexOfAny(invalidChars) == -1;
            //return regexValidFileName.IsMatch(fileName);
        }

        public static string GetFileSizeInfo(string fileName)
        {
            string info = String.Empty;
            if (!File.Exists(fileName)) 
                return info;

            FileInfo fi = new FileInfo(fileName);
            if (fi.Length > 1024 * 1024 * 1024)
            {
                info = String.Format("{0:N2}GB", Math.Round(fi.Length / 1024.0 / 1024.0 / 1024.0, 2));
            }
            else if (fi.Length > 1024 * 1024)
            {
                info = String.Format("{0:N2}MB", Math.Round(fi.Length / 1024.0 / 1024.0, 2));
            }
            else
            {
                info = String.Format("{0:N2}KB", Math.Round(fi.Length / 1024.0, 2));
            }

            return info;
        }

        public static string GetTempDir()
        {
            string tempDir = null;

            tempDir = Environment.GetEnvironmentVariable("TEMP");
            if (tempDir == null)
            {
                tempDir = Environment.GetEnvironmentVariable("TMP");
                if (tempDir == null)
                {
                    string winDir = Environment.GetEnvironmentVariable("WINDIR");
                    if (winDir != null)
                    {
                        tempDir = winDir + @"\TEMP";
                        if (!Directory.Exists(tempDir))
                            tempDir = null;
                    }
                }

                if (tempDir == null)
                    tempDir = @"C:\TEMP";
                if (!Directory.Exists(tempDir))
                    tempDir = @"C:\";
            }

            return tempDir;
        }

        public static string[] GetFullFileNames(IList list, string sourceDir)
        {
            string[] strs = new string[list.Count];
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = GetFullFileName(list, i, sourceDir);
            }
            return strs;
        }

        public static string GetFullFileName(IList list, int i, string sourceDir)
        {
            string fileName = null;
            if (list == null || list.Count <= i)
                return fileName;

            object o = list[i];
            if (o is DataGridViewRow)
            {
                DataGridViewRow row = (DataGridViewRow)o;
                fileName = GetFileName(row, sourceDir);
            }
            else if (o is string)
            {
                fileName = (string)o;
            }
            else if (o is FileInfo)
            {
                FileInfo fi = (FileInfo)o;
                fileName = fi.FullName;
            }

            return fileName;
        }

        public static string GetFileName(DataGridViewRow row)
        {
            return row.Cells["dgcFileName"].Value.ToString();
        }

        public static string GetFileName(DataGridViewRow row, string sourceDir)
        {
            return Path.Combine(SimplePath.GetFullPath(sourceDir), GetFileName(row));
        }

        //public static bool IsImageExt(string name)
        //{
        //    return
        //        name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".emf", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase)
        //        ;
        //}

        //public static bool IsTextExt(string name)
        //{
        //    return name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase) ||
        //        name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);
        //}

        public static bool IsStringExt(string name)
        {
            return name.EndsWith(".string", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsIconExt(string name)
        {
            return name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsCursorExt(string name)
        {
            return name.EndsWith(".cur", StringComparison.OrdinalIgnoreCase);
        }        

        public static bool IsResourceExt(string name)
        {
            return name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsResxExt(string name)
        {
            return name.EndsWith(".resx", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBamlExt(string name)
        {
            return name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);
        }

        public static int IndexOfDot(string s, int count)
        {
            int p = s.IndexOf('.', 0);
            while (p > 0 && count > 1 && p + 1 < s.Length)
            {
                p = s.IndexOf('.', p + 1);
                count--;
            }
            return p;
        }

        #region Framework Directories
        [DllImport("mscoree.dll")]
        private static extern int GetCORSystemDirectory(
        [MarshalAs(UnmanagedType.LPWStr)]StringBuilder pbuffer, int cchBuffer, ref int dwlength);

        public static string GetFrameworkInstalledDir()
        {
            string dir = String.Empty;
            try
            {
                int MAX_PATH = 260;
                StringBuilder sb = new StringBuilder(MAX_PATH);
                GetCORSystemDirectory(sb, MAX_PATH, ref MAX_PATH);
                //sb.Remove(sb.Length - 1, 1);
                dir = sb.ToString();
            }
            catch
            {
            }
            return dir;
        }

        public static string GetFrameworkSDKInstalledDir()
        {
            string dir = null;
            string name = "InstallationFolder";
            string regKey;
            RegistryKey key;
                
            //3.5, 4.0, 4.5
            regKey = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows";
            key = Registry.LocalMachine.OpenSubKey(regKey); 
            if (key != null) 
            {
                var subKeyNames = key.GetSubKeyNames();
                if (subKeyNames != null && subKeyNames.Length > 0)
                {
                    Array.Sort<string>(subKeyNames);
                    var subKey = key.OpenSubKey(subKeyNames[subKeyNames.Length - 1]);
                    var o = subKey.GetValue(name);
                    if (o != null)
                    {
                        dir = o.ToString();
                    }
                    subKey.Close();
                }
                key.Close();
            }

            if (Directory.Exists(dir)) 
                return dir;

            //2.0, 3.0
            regKey = @"SOFTWARE\Microsoft\.NETFramework";
            name = "sdkInstallRootv2.0";
            key = Registry.LocalMachine.OpenSubKey(regKey);
            if (key != null)
            {
                var o = key.GetValue(name);
                if (o != null)
                {
                    dir = o.ToString();
                }
                key.Close();
            }

            if (Directory.Exists(dir))
                return dir;

            return String.Empty;
        }

        public static string GetFrameworkSDKBinDir()
        {
            string dir = GetFrameworkSDKInstalledDir();
            if(String.IsNullOrEmpty(dir))
                return dir;

            var files = Directory.GetFiles(Path.Combine(dir, "bin"), "peverify.exe", SearchOption.AllDirectories);
            if(files != null && files.Length>0) {
                return Path.GetDirectoryName(files[0]);
            }

            return String.Empty;
        }

        public static void SetupFrameworkSDKPath()
        {
            #region Set Path
            StringBuilder sb = new StringBuilder();
            string dir;

            dir = PathUtils.GetFrameworkSDKBinDir();
            if (!String.IsNullOrEmpty(dir))
            {
                sb.Append(dir);
                sb.Append(";");
            }
            dir = PathUtils.GetFrameworkInstalledDir();
            if (!String.IsNullOrEmpty(dir))
            {
                sb.Append(dir);
                sb.Append(";");
            }
            sb.Append(Environment.GetEnvironmentVariable("PATH"));
            Environment.SetEnvironmentVariable("PATH", sb.ToString());

            #endregion Set Path
        }
        #endregion Framework Directories

    }//end of class
}

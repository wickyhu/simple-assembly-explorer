using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Security.Principal;

namespace SimpleAssemblyExplorer
{
    public class ShellExtUtils
    {
        public static void Register(string fileType,
           string shellKeyName, string menuText, string menuCommand)
        {
            // create path to registry location
            string regPath = string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key =
                   Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }

        public static string GetRegPath(string fileType, string shellKeyName)
        {
            return string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);
        }

        public static void Unregister(string fileType, string shellKeyName)
        {
            if (string.IsNullOrEmpty(fileType) ||
                string.IsNullOrEmpty(shellKeyName))
                return;

            // path to the registry location
            string regPath = GetRegPath(fileType, shellKeyName);

            // remove context menu from the registry            
            Registry.ClassesRoot.DeleteSubKeyTree(regPath, false);
        }

        const string menuText = "Open with SAE";
        static string[] fileTypes = new string[] { "exefile", "dllfile" };

        public static void RegisterSAE()
        {
            string menuCommand = string.Format("\"{0}\" \"%L\"",
                                               System.Windows.Forms.Application.ExecutablePath);
            foreach (string fileType in fileTypes)
            {
                ShellExtUtils.Register(fileType, menuText, menuText, menuCommand);
            }            
        }

        public static void UnregisterSAE()
        {
            foreach (string fileType in fileTypes)
            {
                ShellExtUtils.Unregister(fileType, menuText);
            }
        }

        public static bool IsSAERegistered()
        {
            foreach (string fileType in fileTypes)
            {
                string regPath = GetRegPath(fileType, menuText);
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(regPath);
                if (rk == null)
                    return false;
                rk.Close();
            }
            return true;
        }

        public static bool IsAdministrator()
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            return hasAdministrativeRight;
        }

    }//end of class
}

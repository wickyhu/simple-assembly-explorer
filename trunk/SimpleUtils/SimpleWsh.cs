using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using IWshRuntimeLibrary;

namespace SimpleUtils
{
    public class SimpleWsh
    {
        public enum SpecialFolders
        {
            AllUsersDesktop,
            AllUsersStartMenu,
            AllUsersPrograms,
            AllUsersStartup,
            Desktop,
            Favorites,
            Fonts,
            MyDocuments,
            NetHood,
            PrintHood,
            Programs,
            Recent,
            SendTo,
            StartMenu,
            Startup,
            Templates
        }

        private static string GetSpecialFolder(WshShell ws, SpecialFolders specialFolder)
        {
            object o = specialFolder.ToString();
            string folder = ws.SpecialFolders.Item(ref o) as string;
            return folder;
        }

        private static string GetShortcutPath(WshShell ws, SpecialFolders specialFolder, string name)
        {
            string folder = GetSpecialFolder(ws, specialFolder);
            string path = Path.Combine(folder, name + ".lnk");
            return path;
        }

        public static void CreateShortcut(SpecialFolders specialFolder, string name, string exePath)
        {
            WshShell ws = new WshShellClass();
            string path = GetShortcutPath(ws, specialFolder, name);
            WshShortcut shortcut = ws.CreateShortcut(path) as WshShortcut;
            shortcut.TargetPath = exePath;
            shortcut.WindowStyle = 1;
            //shortcut.Hotkey = "CTRL+SHIFT+F";
            shortcut.IconLocation = exePath + ", 0";
            shortcut.Description = name;
            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
            shortcut.Save();
        }

        public static void DeleteShortcut(SpecialFolders specialFolder, string name)
        {
            WshShell ws = new WshShellClass();
            string path = GetShortcutPath(ws, specialFolder, name);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

    }//end of class
}

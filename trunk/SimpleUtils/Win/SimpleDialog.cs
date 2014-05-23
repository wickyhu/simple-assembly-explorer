using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SimpleUtils.Win
{
    public class SimpleDialog
    {
        #region OpenFile
        protected static OpenFileDialog openFileDialog = new OpenFileDialog();

        protected static DialogResult ShowOpenFileDialog(string title, string filter, string ext, bool checkFile, string initDir, string initFileName)
        {
            if (!String.IsNullOrEmpty(title))
                openFileDialog.Title = title;

            openFileDialog.RestoreDirectory = true;
            if (filter != null) openFileDialog.Filter = filter;
            if (ext != null) openFileDialog.DefaultExt = ext;
            openFileDialog.CheckFileExists = checkFile;            
            if (String.IsNullOrEmpty(initDir))
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            else
                openFileDialog.InitialDirectory = initDir;
            if (String.IsNullOrEmpty(initFileName))
                openFileDialog.FileName = String.Empty;
            else
                openFileDialog.FileName = initFileName;

            DialogResult dr = openFileDialog.ShowDialog();
            //if (dr != DialogResult.Cancel)
            //{
            //}
            return dr;
        }

        public static string OpenFile(string title, string filter, string ext, bool checkFile, string initDir)
        {
            return OpenFile(title, filter, ext, checkFile, initDir, null);
        }

        public static string OpenFile(string title, string filter, string ext, bool checkFile, string initDir, string initFileName, bool multiSelect = false)
        {
            openFileDialog.Multiselect = multiSelect;
            if (ShowOpenFileDialog(title, filter, ext, checkFile, initDir, initFileName) == DialogResult.Cancel)
                return null;
            return openFileDialog.FileName;
        }

        public static string FileName
        {
            get { return openFileDialog.FileName; }
        }

        public static string[] FileNames
        {
            get { return openFileDialog.FileNames; }
        }

        #endregion OpenFile

        #region OpenFolder
        private static FolderBrowserDialog _fileBrowserDialog = new FolderBrowserDialog();
        public static string OpenFolder()
        {
            return OpenFolder(null);
        }
        public static string OpenFolder(string initDir, string description = null)
        {
            //_fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (initDir != null && Directory.Exists(initDir))
            {
                _fileBrowserDialog.SelectedPath = initDir;
            }
            else
            {
                _fileBrowserDialog.SelectedPath = Environment.CurrentDirectory;
            }
            if (!String.IsNullOrEmpty(description))
            {
                _fileBrowserDialog.Description = description;
            }
            if (_fileBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return _fileBrowserDialog.SelectedPath;
            }
            return null;
        }
        #endregion OpenFolder

    }//end of class
}

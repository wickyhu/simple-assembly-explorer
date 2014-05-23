using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SimpleAssemblyExplorer
{
    public class DirectoryInfoComparer : IComparer<DirectoryInfo>
    {
        public int Compare(DirectoryInfo x, DirectoryInfo y)
        {
            if (x == null || y == null) 
                return 0;
            return x.Name.CompareTo(y.Name);
        }

    }//end of class
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SimpleUtils
{
    public class SimplePath
    {
        public static string DirectorySeparator = Path.DirectorySeparatorChar.ToString();
        public static string GetFullPath(string path)
        {
            if (path.EndsWith(DirectorySeparator)) return path;
            return path + DirectorySeparator;
        }

        public static string GetFullPathWithoutTrailingSeparator(string path)
        {
            if (path.EndsWith(DirectorySeparator))
                return path.Substring(0, path.Length - DirectorySeparator.Length);
            return path;
        }        
    }
}

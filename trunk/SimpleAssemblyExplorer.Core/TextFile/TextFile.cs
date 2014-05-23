using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class TextFile
    {
        public TextFile(string fileName)
        {
            _fileName = fileName;
        }

        string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        bool _fileReady = false;
        public bool FileReady
        {
            get { return _fileReady; }
            set { _fileReady = value; }
        }

        bool _linesReady = false;
        List<string> _lines;
        public List<string> Lines
        {
            get
            {
                ReadLines();
                return _lines;
            }
        }

        public void Clear(IList list)
        {
            if (list != null)
            {
                list.Clear();
            }
        }
        public void Clear(IDictionary dic)
        {
            if (dic != null)
            {
                dic.Clear();
            }
        }      

        public virtual void Clear()
        {
            Clear(_lines);
            _linesReady = false;
            _fileReady = false;
        }        

        public virtual void ReadFile()
        {
            if (!FileReady)
            {
                ReadLines();
                ParseLines();
                FileReady = true;
            }
        }

        public virtual void ParseLines()
        {
        }

        static Assembly _resourceAssembly = null;
        public static Assembly ResourceAssembly
        {
            get
            {
                if (_resourceAssembly == null)
                {
                    _resourceAssembly = Assembly.GetEntryAssembly();
                }
                return _resourceAssembly;
            }
            set
            {
                _resourceAssembly = value;
            }
        }

        protected void ReadLines()
        {
            if (_linesReady)
                return;

            _lines = new List<string>();

            Stream s = null;

            string path;
            string filePath;

            Assembly ea = Assembly.GetEntryAssembly();
            if (ea != null)
            {
                path = Path.GetDirectoryName(ea.Location);
                filePath = Path.Combine(path, FileName);
                if (!File.Exists(filePath))
                {
                    path = Environment.CurrentDirectory;
                    filePath = Path.Combine(path, FileName);
                }
            }
            else
            {
                path = Environment.CurrentDirectory;
                filePath = Path.Combine(path, FileName);
            }
            if (!File.Exists(filePath))
            {
                filePath = FileName;
            }
            if (File.Exists(filePath))
            {
                s = File.Open(filePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                if (ResourceAssembly != null)
                {
                    s = ResourceAssembly.GetManifestResourceStream(String.Format("{0}.{1}", ResourceAssembly.GetName().Name, FileName));
                }
            }

            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.StartsWith("#")) continue;

                        _lines.Add(line);
                    }
                }

                _linesReady = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Close();
                s = null;
            }

        }

        public List<Regex> CreateRegexList(List<string> lines)
        {
            List<Regex> list = new List<Regex>(lines.Count);
            foreach (string s in lines)
            {
                list.Add(new Regex(s));
            }
            return list;
        }

        public bool ParseSetting(string line, out string name, out string value)
        {
            string[] s = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 2)
            {
                name = s[0];
                value = s[1];
                return true;
            }
            else
            {
                name = null;
                value = null;
                return false;
            }
        }

    }//end of class

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class RegexFile : TextFile 
    {
        static RegexFile _default;
        public static RegexFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new RegexFile("Regex.txt"), null);
                }
                return _default;
            }
        }

        public RegexFile(string fileName)
            : base(fileName)
        {
        }

        List<Regex> _blackRegexList;
        public List<Regex> BlackRegexList
        {
            get
            {
                ReadFile();
                return _blackRegexList;
            }
            set { _blackRegexList = value; }
        }

        List<Regex> _whiteRegexList;
        public List<Regex> WhiteRegexList
        {
            get
            {
                ReadFile();
                return _whiteRegexList;
            }
            set { _whiteRegexList = value; }
        }

        private void AddBlackRegex(string pattern)
        {
            Regex regex = new Regex(pattern);
            _blackRegexList.Add(regex);
        }

        private void AddWhiteRegex(string pattern)
        {
            Regex regex = new Regex(pattern);
            _whiteRegexList.Add(regex);
        }

        public bool IsBlackWord(string name)
        {
            foreach (Regex regex in BlackRegexList)
            {
                if (regex.IsMatch(name))
                    return true;
            }
            return false;
        }

        public bool IsWhiteWord(string name)
        {
            foreach (Regex regex in WhiteRegexList)
            {
                if (regex.IsMatch(name))
                    return true;
            }
            return false;
        }

        public bool IsRandomName(string name)
        {
            if (IsWhiteWord(name))
                return false;
            if (IsBlackWord(name))
                return true;
            return false;
        }

        public override void ParseLines()
        {
            string profileName = null;

            _whiteRegexList = new List<Regex>();
            _blackRegexList = new List<Regex>();

            foreach (string line in this.Lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    profileName = line.Substring(1, line.Length - 2);
                }
                else if (profileName == "WhiteRegexList")
                {
                    AddWhiteRegex(line);
                }
                else if (profileName == "BlackRegexList")
                {
                    AddBlackRegex(line);
                }
                else
                {
                    AddBlackRegex(line);
                }
            }//end foreach
        }

        public override void Clear()
        {
            Clear(_whiteRegexList);
            Clear(_blackRegexList);
            base.Clear();
       }       


    }//end of class   

}

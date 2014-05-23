using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class KeywordFile : TextFile
    {
        static KeywordFile _default;
        public static KeywordFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new KeywordFile("Keyword.txt"), null);
                }
                return _default;
            }
        }

        public KeywordFile(string fileName)
            : base(fileName)
        {
        }

       Dictionary<string, int> _keywords = null;
        public Dictionary<string, int> Keywords
        {
            get
            {
                ReadFile();
                return _keywords;
            }
        }

        public bool IsKeyword(string input)
        {
            return Keywords.ContainsKey(input);
        }

        public override void ParseLines()
        {
            _keywords = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in this.Lines)
            {
                _keywords.Add(key, key.GetHashCode());
            }
        }

        public override void Clear()
        {
            Clear(_keywords);
            base.Clear();
       }


    }//end of class   

}

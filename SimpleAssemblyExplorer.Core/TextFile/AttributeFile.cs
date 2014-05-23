using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace SimpleAssemblyExplorer
{
    public class AttributeFile : TextFile
    {
        static AttributeFile _default;
        public static AttributeFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new AttributeFile("Attribute.txt"), null);
                }
                return _default;
            }
        }

        public AttributeFile(string fileName)
            : base(fileName)
        {
        }

        List<string> _assemblyLevelStrings;
        List<string> _classLevelStrings;
        List<string> _allLevelStrings;

        private List<string> ParseStrings(string level)
        {
            List<string> list = new List<string>();
            foreach (string str in this.Lines)
            {
                string[] s = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (String.IsNullOrEmpty(level))
                {
                    if (s.Length == 1)
                    {
                        list.Add(str.Trim());
                    }
                }
                else
                {
                    if (s.Length > 1)
                    {
                        if (s[1].Contains(level))
                        {
                            list.Add(s[0].Trim());
                        }
                    }
                }
            }
            return list;
        }

        public override void ParseLines()
        {
            _assemblyLevelStrings = ParseStrings("a");
            _classLevelStrings = ParseStrings("c");
            _allLevelStrings = ParseStrings(null);
        }

        public List<string> AssemblyLevelStrings
        {
            get
            {
                ReadFile();
                return _assemblyLevelStrings;
            }
        }

        public List<string> ClassLevelStrings
        {
            get
            {
                ReadFile();
                return _classLevelStrings;
            }
        }

        public List<string> AllLevelStrings
        {
            get
            {
                ReadFile();
                return _allLevelStrings;
            }
        }

        private string GetTokenString(MetadataToken token)
        {
            return "0x" + TokenUtils.GetFullMetadataTokenString(token);
        }

        public bool IsMatchAssemblyLevel(TypeDefinition type)
        {
            string name = InsUtils.GetOldMemberName(type);
            if (IsMatchAssemblyLevel(name))
                return true;
            name = GetTokenString(type.MetadataToken);
            if (IsMatchAssemblyLevel(name))
                return true;
            return false;
        }

        public bool IsMatchAssemblyLevel(string input)
        {
            foreach (string key in AssemblyLevelStrings)
            {
                if (input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public bool IsMatchClassLevel(TypeDefinition type)
        {
            string name = InsUtils.GetOldMemberName(type);
            if (IsMatchClassLevel(name))
                return true;
            name = GetTokenString(type.MetadataToken);
            if (IsMatchClassLevel(name))
                return true;
            return false;
        }

        public bool IsMatchClassLevel(string input)
        {
            foreach (string key in ClassLevelStrings)
            {
                if (input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public bool IsMatchAllLevel(TypeDefinition type)
        {
            string name = InsUtils.GetOldMemberName(type);
            if (IsMatchAllLevel(name))
                return true;
            name = GetTokenString(type.MetadataToken);
            if (IsMatchAllLevel(name))
                return true;
            return false;
        }

        public bool IsMatchAllLevel(string input)
        {
            foreach (string key in AllLevelStrings)
            {
                if (input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public override void Clear()
        {
            Clear(_assemblyLevelStrings);
            Clear(_classLevelStrings);
            Clear(_allLevelStrings);
            base.Clear();
        }       

    }//end of class   

}

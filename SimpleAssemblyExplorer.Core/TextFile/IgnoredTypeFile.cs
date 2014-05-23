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
    public class IgnoredTypeFile : TextFile
    {
        static IgnoredTypeFile _default;
        public static IgnoredTypeFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new IgnoredTypeFile("IgnoredType.txt"), null);
                }
                return _default;
            }
        }

        public IgnoredTypeFile(string fileName)
            : base(fileName)
        {
        }

        List<Regex> _regexes = null;

        public List<Regex> Regexes
        {
            get
            {
                ReadFile();
                return _regexes;
            }
        }

        public override void ParseLines()
        {
            _regexes = CreateRegexList(Lines);
        }

        public bool IsMatch(TypeDefinition type)
        {
            string name = type.FullName;
            if (IsMatch(name))
                return true;
            name = "0x" + TokenUtils.GetFullMetadataTokenString(type.MetadataToken);
            if (IsMatch(name))
                return true;
            return false;
        }

        public bool IsMatch(string input)
        {
            if (Regexes.Count == 0)
                return false;

            foreach (Regex r in Regexes)
            {
                if (r.IsMatch(input))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Clear()
        {
            Clear(_regexes);
            base.Clear();
        }

    }//end of class   

}

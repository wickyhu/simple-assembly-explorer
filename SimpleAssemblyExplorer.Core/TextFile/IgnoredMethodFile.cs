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
    public class IgnoredMethodFile : TextFile
    {
        static IgnoredMethodFile _default;
        public static IgnoredMethodFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new IgnoredMethodFile("IgnoredMethod.txt"), null);
                }
                return _default;
            }
        }

        public IgnoredMethodFile(string fileName)
            : base(fileName)
        {
        }

        List<Regex> _regexes = null;
        Dictionary<string, bool> _blackList = new Dictionary<string, bool>();

        int _maxInstructionCount = 0;
        public int MaxInstructionCount
        {
            get
            {
                ReadFile();
                return _maxInstructionCount;
            }
            set
            {
                if (value < 0)
                    _maxInstructionCount = 0;
                else
                    _maxInstructionCount = value;
            }
        }

        public const int DefaultMaxCallError = int.MaxValue;
        int _maxCallError = DefaultMaxCallError;
        public int MaxCallError
        {
            get
            {
                ReadFile();
                return _maxCallError;
            }
            set
            {
                if (value < 1)
                    _maxCallError = DefaultMaxCallError;
                else
                    _maxCallError = value;
            }
        }

        bool _ignoreSystemMethods = false;
        public bool IgnoreSystemMethods
        {
            get
            {
                ReadFile();
                return _ignoreSystemMethods;
            }
            set
            {
                _ignoreSystemMethods = value;
            }
        }

        bool _ignorePInvokeMethods = false;
        public bool IgnorePInvokeMethods
        {
            get
            {
                ReadFile();
                return _ignorePInvokeMethods;
            }
            set
            {
                _ignorePInvokeMethods = value;
            }
        }

        bool _ignoreMethodsWithoutParam = false;
        public bool IgnoreMethodsWithoutParam
        {
            get
            {
                ReadFile();
                return _ignoreMethodsWithoutParam;
            }
            set
            {
                _ignoreMethodsWithoutParam = value;
            }
        }

        bool _ignoreMethodsReturnPath = false;
        public bool IgnoreMethodsReturnPath
        {
            get
            {
                ReadFile();
                return _ignoreMethodsReturnPath;
            }
            set
            {
                _ignoreMethodsReturnPath = value;
            }
        }

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
            _regexes = new List<Regex>();

            string profileName = null;
            string propertyName;
            string propertyValue;

            foreach (string line in Lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    profileName = line.Substring(1, line.Length - 2);
                }
                else if (line.Equals("IGNORE.SYSTEM.METHODS", StringComparison.OrdinalIgnoreCase))
                {
                    _ignoreSystemMethods = true;
                }
                else if (line.Equals("IGNORE.PINVOKE.METHODS", StringComparison.OrdinalIgnoreCase))
                {
                    _ignorePInvokeMethods = true;
                }
                else if (line.Equals("IGNORE.METHODS.WITHOUT.PARAM", StringComparison.OrdinalIgnoreCase))
                {
                    _ignoreMethodsWithoutParam = true;
                }
                else if (line.Equals("IGNORE.METHODS.RETURN.PATH", StringComparison.OrdinalIgnoreCase))
                {
                    _ignoreMethodsReturnPath = true;
                }
                else if (line.StartsWith("MAX.CALL.ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    if(ParseSetting(line, out propertyName, out propertyValue))
                    {
                        int i;
                        if (int.TryParse(propertyValue, out i))
                        {
                            this.MaxCallError = i;
                        }
                        else
                        {
                            this.MaxCallError = DefaultMaxCallError;
                        }
                    }
                }
                else if (line.StartsWith("MAX.INSTRUCTION.COUNT", StringComparison.OrdinalIgnoreCase))
                {
                    if (ParseSetting(line, out propertyName, out propertyValue))
                    {
                        int i;
                        if (int.TryParse(propertyValue, out i))
                        {
                            this.MaxInstructionCount = i;
                        }
                        else
                        {
                            this.MaxInstructionCount = 0;
                        }
                    }
                }
                else
                {
                    _regexes.Add(new Regex(line));
                }

            }
        }

        public void Ignore(string input)
        {
            if (_blackList.ContainsKey(input))
                return;
            _blackList.Add(input, true);
        }

        public bool IsSystemMethod(MethodDefinition method)
        {
            if (method.DeclaringType == null) return false;
            string typeName = method.DeclaringType.FullName;
            if (Utils.IsSystemType(typeName))
                return true;
            //TODO: maybe check assembly name or metatoken in future
            return false;
        }

        public bool IsMatch(MethodDefinition method)
        {
            if (IgnoreSystemMethods && IsSystemMethod(method))
                return true;
            if(IgnorePInvokeMethods && method.IsPInvokeImpl)
                return true;
            if (IsExceedMaxInstructionCount(method))
                return true;

            if (method.ReturnType.FullName == "System.String")
            {
                if (IgnoreMethodsWithoutParam && method.Parameters.Count == 0)
                    return true;
            }

            string name = method.ToString();
            if (IsMatch(name))
                return true;
            name = "0x" + TokenUtils.GetFullMetadataTokenString(method.MetadataToken);
            if (IsMatch(name))
                return true;
            return false;
        }

        public bool IsExceedMaxInstructionCount(MethodDefinition method)
        {
            if (MaxInstructionCount > 0 && method.HasBody && method.Body.Instructions.Count >= MaxInstructionCount)
                return true;
            return false;
        }

        public bool IsMatch(string input)
        {
            if (_blackList.ContainsKey(input))
                return true;

            if (Regexes.Count == 0)
                return false;

            foreach (Regex r in Regexes)
            {
                if (r.IsMatch(input))
                {
                    Ignore(input);
                    return true;
                }
            }
            return false;
        }

        //Squall...@gmail.com.patch.start
        public bool IsMatch(MethodInfo method)
        {
            if (method.DeclaringType == null) 
                return false;
            if (IgnoreSystemMethods && Utils.IsSystemMethod(method))
                return true;
            if (IgnorePInvokeMethods && method.Attributes.HasFlag(System.Reflection.MethodAttributes.PinvokeImpl))
                return true;

            if (method.ReturnType.FullName == "System.String")
            {
                if (IgnoreMethodsWithoutParam && method.GetParameters().Length == 0)
                    return true;
            }
            
            string name;
            //name = method.ToString();
            //TODO MethodInfo.ToSting() is different like MethodDefinition.ToString() 
            //if (IsMatch(name)) 
            //    return true; 
            name = "0x" + method.MetadataToken.ToString("x08");
            if (IsMatch(name))
                return true;
            return false;
        }
        //Squall...@gmail.com.patch.end

        public override void Clear()
        {
            Clear(_regexes);
            Clear(_blackList);
            _ignoreSystemMethods = false;
            _ignorePInvokeMethods = false;
            _maxCallError = DefaultMaxCallError;
            base.Clear();
       }
       

    }//end of class   

}

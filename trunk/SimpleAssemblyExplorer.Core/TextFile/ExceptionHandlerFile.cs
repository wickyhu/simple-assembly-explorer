using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class ExceptionHandlerFile  : TextFile
    {
        static ExceptionHandlerFile _default;
        public static ExceptionHandlerFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new ExceptionHandlerFile("ExceptionHandler.txt"), null);
                }
                return _default;
            }
        }

        public ExceptionHandlerFile(string fileName)
            : base(fileName)
        {
        }

        public override void ParseLines()
        {
            foreach (string line in Keywords)
            {
                if (line.Equals("REMOVE.GLOBAL.EXCEPTION.HANDLER", StringComparison.OrdinalIgnoreCase))
                {
                    _removeGlobalExceptionHandler = true;
                    break;
                }
            }
        }

        bool _removeGlobalExceptionHandler = false;
        public bool RemoveGlobalExceptionHandler
        {
            get
            {
                ReadFile();
                return _removeGlobalExceptionHandler;
            }
            set
            {
                _removeGlobalExceptionHandler = value;
            }
        }

        public List<string> Keywords
        {
            get
            {
                return this.Lines;
            }
        }

        public bool IsMatch(string input)
        {
            foreach (string key in Keywords)
            {
                if (input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public override void Clear()
        {
            base.Clear();
            _removeGlobalExceptionHandler = false;
        }       

    }//end of class   

}

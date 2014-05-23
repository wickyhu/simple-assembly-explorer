using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class ToolItem
    {
        public string Title { get; set; }
        public string ExeFile { get; set; }
        public string Argument { get; set; }

        public ToolItem(string title, string exe, string arg)
        {
            this.Title = title;
            this.ExeFile = exe;
            this.Argument = arg;
        }
    }

    public class ToolFile  : TextFile
    {
        static ToolFile _default;
        public static ToolFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new ToolFile("Tools.txt"), null);
                }
                return _default;
            }
        }

        public ToolFile(string fileName)
            : base(fileName)
        {
        }

        List<ToolItem> _toolItems;
        public List<ToolItem> ToolItems
        {
            get
            {
                ReadFile();
                return _toolItems;
            }
        }

        public override void ParseLines()
        {
            _toolItems = new List<ToolItem>();

            foreach (string str in this.Lines)
            {
                string line = str;
                int p = line.IndexOf(";");
                if (p < 0) continue;

                string title = line.Substring(0, p);
                if (String.IsNullOrEmpty(title)) continue;

                if (line.Length <= p + 1) continue;
                line = line.Substring(p + 1).Trim();

                string exe;
                string arg = string.Empty;

                if (line.StartsWith("\""))
                {
                    p = line.IndexOf("\"", 1);
                    if (p < 0) continue;
                    exe = line.Substring(1, p - 1);
                    if (line.Length > p + 1)
                        arg = line.Substring(p + 1);
                }
                else
                {
                    p = line.IndexOf(" ");
                    if (p < 0)
                    {
                        exe = line;
                    }
                    else
                    {
                        exe = line.Substring(0, p);
                        if (line.Length > p + 1)
                            arg = line.Substring(p + 1);
                    }
                }

                if (String.IsNullOrEmpty(exe))
                    continue;

                _toolItems.Add(new ToolItem(title, exe, arg));
            }

        }        

        public override void Clear()
        {
            Clear(_toolItems);
            base.Clear();
        }
    }//end of class   

}

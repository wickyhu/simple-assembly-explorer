using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleUtils;
using SimpleAssemblyExplorer;

namespace SAE.de4dot
{
    public class de4dot : CommandTool 
    {
        public new de4dotOptions Options
        {
            get { return (de4dotOptions)base.Options; }
        }

        public de4dot()
            : this(new de4dotOptions())
        {
        }

        public de4dot(de4dotOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                string root = this.Options.Host.GetPropertyValue(Plugin.PropertyRootDirectory) as string;
                return Path.Combine(root, this.Options.ExeFile);
            }
        }        

        private string GetArguments()
        {
            StringBuilder sb = new StringBuilder();
            if (Options.Verbose) sb.Append("-v ");
            if (Options.Action == de4dotOptions.Actions.Detect) 
            {
                sb.AppendFormat("-d ");
            }

            if (Options.ScanDir)
            {
                if (Options.Action == de4dotOptions.Actions.Detect)
                {
                    sb.AppendFormat("-r . ");
                }
                else if (Options.Action == de4dotOptions.Actions.Deobfuscate)
                {
                    sb.AppendFormat("-r . -ro {0} ", Plugin.Name);
                }
                if (Options.IgnoreUnsupported)
                {
                    sb.Append("-ru ");
                }
            }
            else
            {
                foreach (string file in Options.Rows)
                {
                    string name = Path.GetFileName(file);
                    string ext = Path.GetExtension(file);
                    string outExt = String.Format(".{0}{1}", Plugin.Name, ext);
                    string outName = Path.ChangeExtension(name, outExt);

                    if (Options.Action == de4dotOptions.Actions.Detect)
                    {
                        sb.AppendFormat("\"{0}\" ", name);
                    }
                    else if (Options.Action == de4dotOptions.Actions.Deobfuscate)
                    {
                        if (Options.CreateOutputDir)
                        {
                            sb.AppendFormat("-f \"{0}\" -o \"{1}\\{0}\" ", name, Plugin.Name);
                        }
                        else
                        {
                            sb.AppendFormat("-f \"{0}\" -o \"{1}\" ", name, outName);
                        }
                    }
                }
            }

            if (Options.StringDecrypterType != de4dotOptions.StringDecrypterTypes.Default)
            {
                sb.AppendFormat("--strtyp {0} ", Options.StringDecrypterType.ToString().ToLowerInvariant());
                if (Options.StringDecrypterType != de4dotOptions.StringDecrypterTypes.None 
                    && !String.IsNullOrWhiteSpace(Options.StringDecrypterMethod))
                {
                    sb.AppendFormat("--strtok {0} ", Options.StringDecrypterMethod);
                }
            }

            if (Options.PreserveTokens) sb.Append("--preserve-tokens ");
            if (Options.DontRename) sb.Append("--dont-rename ");
            if (Options.KeepTypes) sb.Append("--keep-types ");

            if (!String.IsNullOrEmpty(Options.AdditionalOptions))
            {
                sb.Append(Options.AdditionalOptions);
            }
            return sb.ToString();
        }

        public override void Go()
        {
            string[] rows = Options.Rows;

            ShowStartTextInfo();

            Process p = CreateProcess();
            p.StartInfo.Arguments = GetArguments();

            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;

            ShowProcessTextInfo(p);            
            p.Start();
            WaitForProcess(p);            

            ShowCompleteTextInfo();

        }

        public override string HelpArgument
        {
            get
            {
                return "-h";
            }
        }

    } //end of class
}

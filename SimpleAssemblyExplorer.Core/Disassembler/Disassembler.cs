using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public class Disassembler : CommandTool 
    {
        public new DasmOptions Options
        {
            get { return (DasmOptions)base.Options; }
        }

        public Disassembler()
            : this(new DasmOptions())
        {
        }

        public Disassembler(DasmOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                //string ildasm = String.Format("{0}ildasm.exe", RuntimeEnvironment.GetRuntimeDirectory());
                string ildasm = "ildasm.exe";
                return ildasm;
            }
        }       

        public override string PrepareArguments(string sourceFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);
            string dasmDir = Path.Combine(Options.OutputDir, fileName);
            string ilFile = String.Format("{0}.il", fileName);

            if (!Directory.Exists(dasmDir))
            {
                Directory.CreateDirectory(dasmDir);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\"{0}\" ", sourceFile);
            sb.AppendFormat("/out=\"{0}\" ", Path.Combine(dasmDir, ilFile));
            if (Options.chkBytesChecked) sb.Append("/byt ");
            if (Options.chkTokensChecked) sb.Append("/tok ");
            if (Options.chkTypeListChecked) sb.Append("/typelist ");
            if (Options.chkCAVerbalChecked) sb.Append("/caverbal ");
            if (Options.chkClassListChecked) sb.Append("/classlist ");
            if (Options.chkUnicodeChecked) sb.Append("/unicode ");
            else if (Options.chkUTF8Checked) sb.Append("/utf8 ");
            if (!String.IsNullOrEmpty(Options.AdditionalOptions))
                sb.Append(Options.AdditionalOptions);

            return sb.ToString();
        }       


    } //end of class
}

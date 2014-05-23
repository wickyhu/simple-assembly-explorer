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
    public class PEVerifier : CommandTool 
    {
        public new PEVerifyOptions Options
        {
            get { return (PEVerifyOptions)base.Options; }
        }

        public PEVerifier()
            : this(new PEVerifyOptions())
        {
        }

        public PEVerifier(PEVerifyOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                return "PEVerify.exe";
            }
        }
        

        public override string PrepareArguments(string sourceFile)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\"{0}\" ", sourceFile);
            if (Options.chkHResultChecked) sb.Append("/hresult ");
            if (Options.chkILChecked) sb.Append("/il ");
            if (Options.chkMDChecked) sb.Append("/md ");
            if (Options.chkUniqueChecked) sb.Append("/unique ");
            if (Options.chkVerboseChecked) sb.Append("/verbose ");
            if (Options.chkClockChecked) sb.Append("/clock ");
            if (Options.chkNoLogoChecked) sb.Append("/nologo ");
            if (!String.IsNullOrEmpty(Options.AdditionalOptions))
                sb.Append(Options.AdditionalOptions);
            return sb.ToString();
        }        


    } //end of class
}

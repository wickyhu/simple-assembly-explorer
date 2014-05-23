using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public class SN : CommandTool 
    {
        public new SNOptions Options
        {
            get { return (SNOptions)base.Options; }
        }

        public SN()
            : this(new SNOptions())
        {
        }

        public SN(SNOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                return "sn.exe";
            }
        }


        public override string PrepareArguments(string sourceFile)
        {
            StringBuilder sb = new StringBuilder();
            if (Options.chkQuietChecked) sb.Append("-q ");

            if (Options.rbVlChecked) sb.Append("-Vl ");
            if (Options.rbVxChecked) sb.Append("-Vx ");
            if (Options.rbVrChecked) sb.Append("-Vr ");
            if (Options.rbvfChecked) sb.Append("-vf ");
            if (Options.rbVuChecked) sb.Append("-Vu ");
            if (Options.rbRaChecked) sb.Append("-Ra ");

            if (Options.rbCustomChecked && !String.IsNullOrEmpty(Options.AdditionalOptions))
            {
                sb.Append(Options.AdditionalOptions);
            }
            else
            {

                if (!Options.IgnoreRows && !String.IsNullOrEmpty(sourceFile))
                {
                    sb.AppendFormat("\"{0}\" ", sourceFile);
                }
                if (Options.rbRaChecked && !String.IsNullOrEmpty(Options.KeyFile))
                {
                    sb.AppendFormat("\"{0}\" ", Options.KeyFile);
                }
            }
            return sb.ToString();
        }

        public override void Go()
        {
            bool resolveDirAdded = false;
            try
            {
                if (Options.Host != null)
                {
                    resolveDirAdded = Options.Host.AddAssemblyResolveDir(Options.SourceDir);
                }

                if (Options.rbVxChecked || Options.rbVlChecked || Options.rbCustomChecked)
                {
                    Options.IgnoreRows = true;
                }
                else
                {
                    Options.IgnoreRows = false;
                }

                base.Go();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (resolveDirAdded)
                {
                    Options.Host.RemoveAssemblyResolveDir(Options.SourceDir);
                }
            }
        }

    } //end of class
}

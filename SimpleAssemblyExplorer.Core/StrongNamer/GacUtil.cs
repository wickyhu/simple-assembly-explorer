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
using Mono.Cecil;

namespace SimpleAssemblyExplorer
{
    public class GacUtil : CommandTool 
    {
        public new GacUtilOptions Options
        {
            get { return (GacUtilOptions)base.Options; }
        }

        public GacUtil()
            : this(new GacUtilOptions())
        {
        }

        public GacUtil(GacUtilOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                return "gacutil.exe";
            }
        }

        private string GetAssemblyName(string sourceFile)
        {
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(sourceFile);
            return String.Format("{0}, Version={1}", ad.Name.Name, ad.Name.Version.ToString());
        }

        public override string PrepareArguments(string sourceFile)
        {
            string adName = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (Options.rbInstallChecked)
            {
                sb.Append("/i ");
                adName = Path.GetFileName(sourceFile);
            }
            if (Options.rbUninstallChecked)
            {
                sb.Append("/u ");
                adName = GetAssemblyName(sourceFile);
            }
            if (Options.rbListChecked)
            {
                sb.Append("/l ");
                //adName = Path.GetFileName(sourceFile);
            }

            if (!String.IsNullOrEmpty(adName))
            {
                sb.AppendFormat("\"{0}\" ", adName);
            }

            if (Options.chkForceCheck) sb.Append("/f ");
            if (Options.chkNologoChecked) sb.Append("/nologo ");
            if (Options.chkSilentChecked) sb.Append("/silent ");

            if (!String.IsNullOrEmpty(Options.AdditionalOptions))
                sb.Append(Options.AdditionalOptions);

            return sb.ToString();
        }

        public override void Go()
        {
            bool resolveDirAdded = false;
            try
            {
                resolveDirAdded = Options.Host.AddAssemblyResolveDir(Options.SourceDir);

                if (Options.rbListChecked)
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
                    Options.Host.RemoveAssemblyResolveDir(Options.SourceDir);
            }
        }

    } //end of class
}

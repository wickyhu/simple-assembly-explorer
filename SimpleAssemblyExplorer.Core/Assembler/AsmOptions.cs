using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class AsmOptions : OptionsBase
    {
        public bool chkQuietChecked { get; set; }
        public bool rbDllChecked { get; set; }
        public bool rbExeChecked { get; set; }
        public bool rbDebugChecked { get; set; }
        public bool rbDebugImplChecked { get; set; }
        public bool rbDebugOptChecked { get; set; }
        public bool chkClockChecked { get; set; }
        public bool chkNoLogoChecked { get; set; }
        public bool chkOptimizeChecked { get; set; }
        public bool chkItaniumChecked { get; set; }
        public bool chkX64Checked { get; set; }

        public string KeyFile { get; set; }
        public bool rbRemoveStrongNameChecked { get; set; }
        public bool chkRemoveLicenseProviderChecked { get; set; }
        public bool chkReplaceTokenChecked { get; set; }
        public string txtOldTokenText { get; set; }
        public string txtNewTokenText { get; set; }
        public string ReplKeyFile { get; set; }

        public AsmOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            chkQuietChecked = true;
            rbDllChecked = false;
            rbExeChecked = false;
            rbDebugChecked = false;
            rbDebugImplChecked = false;
            rbDebugOptChecked = false;
            chkClockChecked = false;
            chkNoLogoChecked = true;
            chkOptimizeChecked = false;
            chkItaniumChecked = false;
            chkX64Checked = false;

            KeyFile = String.Empty;
            ReplKeyFile = String.Empty;
            rbRemoveStrongNameChecked = false;
            chkRemoveLicenseProviderChecked = false;
            chkReplaceTokenChecked = false;
            txtOldTokenText = String.Empty;
            txtNewTokenText = String.Empty;
        }

    }
}
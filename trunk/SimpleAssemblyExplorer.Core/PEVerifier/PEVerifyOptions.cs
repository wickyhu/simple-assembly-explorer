using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class PEVerifyOptions : OptionsBase
    {
        public bool chkHResultChecked { get; set; }
        public bool chkVerboseChecked { get; set; }
        public bool chkUniqueChecked { get; set; }
        public bool chkILChecked { get; set; }
        public bool chkMDChecked { get; set; }
        public bool chkClockChecked { get; set; }
        public bool chkNoLogoChecked { get; set; }

        public PEVerifyOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            chkHResultChecked = true;
            chkVerboseChecked = true;
            chkUniqueChecked = false;
            chkILChecked = false;
            chkMDChecked = false;
            chkClockChecked = false;
            chkNoLogoChecked = true;
        }

    }
}
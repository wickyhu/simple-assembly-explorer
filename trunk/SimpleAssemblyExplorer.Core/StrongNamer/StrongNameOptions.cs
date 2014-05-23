using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class StrongNameOptions : SNOptions
    {
        public bool chkOverwriteOriginalFileChecked { get; set; }
        public bool rbRemoveChecked { get; set; }
        public bool rbSignChecked { get; set; }
        public bool rbGacInstallChecked { get; set; }
        public bool rbGacRemoveChecked { get; set; }

        public StrongNameOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            chkOverwriteOriginalFileChecked = false;
            rbRemoveChecked = false;
            rbSignChecked = false;
            rbGacInstallChecked = false;
            rbGacRemoveChecked = false;
        }

    }
}
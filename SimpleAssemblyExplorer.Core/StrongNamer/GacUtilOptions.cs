using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class GacUtilOptions : OptionsBase
    {
        public bool rbInstallChecked { get; set; }
        public bool rbUninstallChecked { get; set; }
        public bool rbListChecked { get; set; }
        public bool chkForceCheck { get; set; }
        public bool chkNologoChecked { get; set; }
        public bool chkSilentChecked { get; set; }

        public GacUtilOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            rbInstallChecked = false;
            rbUninstallChecked = false;
            rbListChecked = false;
            chkForceCheck = false;
            chkNologoChecked = true;
            chkSilentChecked = false;
        }

    }
}
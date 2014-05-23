using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class SNOptions : OptionsBase
    {
        public bool chkQuietChecked { get; set; }
        public bool rbVrChecked { get; set; }
        public bool rbVlChecked { get; set; }
        public bool rbvfChecked { get; set; }
        public bool rbVxChecked { get; set; }
        public bool rbVuChecked { get; set; }
        public bool rbRaChecked { get; set; }
        public bool rbCustomChecked { get; set; }

        public string KeyFile { get; set; }

        public SNOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            ShowFileNoTextInfo = false;

            chkQuietChecked = true;
            rbVrChecked = false;
            rbVlChecked = false;
            rbvfChecked = false;
            rbVxChecked = false;
            rbVuChecked = false;
            rbRaChecked = false;
            rbCustomChecked = false;
            KeyFile = String.Empty;

        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    public class DasmOptions : OptionsBase
    {
        public bool chkBytesChecked { get; set; }
        public bool chkTokensChecked { get; set; }
        public bool chkTypeListChecked { get; set; }
        public bool chkCAVerbalChecked { get; set; }
        public bool chkClassListChecked { get; set; }
        public bool chkUnicodeChecked { get; set; }
        public bool chkUTF8Checked { get; set; }

        public DasmOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();

            chkBytesChecked = true;
            chkTokensChecked = true;
            chkTypeListChecked = true;
            chkCAVerbalChecked = false;
            chkClassListChecked = false;
            chkUnicodeChecked = false;
            chkUTF8Checked = true;
        }


    }
}
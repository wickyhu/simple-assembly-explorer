using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace SAE.DeobfPluginSample
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public bool HandleAssemblyEvent
        {
            get { return chkAssemblyEvent.Checked; }
            set { chkAssemblyEvent.Checked = value; }
        }

        public bool HandleModuleEvent
        {
            get { return chkModuleEvent.Checked; }
            set { chkModuleEvent.Checked = value; }
        }

        public bool HandleTypeEvent
        {
            get { return chkTypeEvent.Checked; }
            set { chkTypeEvent.Checked = value; }
        }

        public bool HandleMethodEvent
        {
            get { return chkMethodEvent.Checked; }
            set { chkMethodEvent.Checked = value; }
        }

        public bool HandlePropertyEvent
        {
            get { return chkPropertyEvent.Checked; }
            set { chkPropertyEvent.Checked = value; }
        }

        public bool HandleFieldEvent
        {
            get { return chkFieldEvent.Checked; }
            set { chkFieldEvent.Checked = value; }
        }

        public bool HandleEventEvent
        {
            get { return chkEventEvent.Checked; }
            set { chkEventEvent.Checked = value; }
        }

        public bool ShowNameList
        {
            get { return chkNameList.Checked; }
            set { chkNameList.Checked = value; }
        }

        public bool ShowRenameList
        {
            get { return chkRenameList.Checked; }
            set { chkRenameList.Checked = value; }
        }

        private void EnableEvents(bool enabled)
        {
            chkAssemblyEvent.Checked = enabled;
            chkModuleEvent.Checked = enabled;
            chkTypeEvent.Checked = enabled;
            chkMethodEvent.Checked = enabled;
            chkPropertyEvent.Checked = enabled;
            chkFieldEvent.Checked = enabled;
            chkEventEvent.Checked = enabled;
        }

        private void EnableSpecials(bool enabled)
        {
            chkNameList.Checked = enabled;
            chkRenameList.Checked = enabled;
        }

        private void chkNameList_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNameList.Checked)
            {
                EnableEvents(false);
            }
        }

        private void chkRenameList_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRenameList.Checked)
            {
                EnableEvents(false);
            }
        }
       

    } // end of class
}
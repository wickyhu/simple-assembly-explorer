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
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class frmVerify : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public frmVerify(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            txtAdditionalOptions.Text = Config.PEVerifyAdditionalOptions;
        }

        //protected override void OnShown(EventArgs e)
        //{
            //base.OnShown(e);

            //btnOK_Click(btnOK, null);
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Config.PEVerifyAdditionalOptions = txtAdditionalOptions.Text;

            try
            {
                Utils.EnableUI(this.Controls, false);

                PEVerifyOptions options = new PEVerifyOptions();

                options.Host = _host;
                options.Rows = _rows;
                options.SourceDir = _sourceDir;
                options.TextInfoBox = txtInfo;

                options.chkClockChecked = chkClock.Checked;
                options.chkHResultChecked = chkHResult.Checked;
                options.chkILChecked = chkIL.Checked;
                options.chkMDChecked = chkMD.Checked;
                options.chkUniqueChecked = chkUnique.Checked;
                options.chkVerboseChecked = chkVerbose.Checked;
                options.chkNoLogoChecked = chkNoLogo.Checked;
                options.AdditionalOptions = txtAdditionalOptions.Text;

                new PEVerifier(options).Go();

            }
            catch
            {
                throw;
            }
            finally
            {
                Utils.EnableUI(this.Controls, true);
            }

            //this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            PEVerifyOptions options = new PEVerifyOptions();

            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;

            new PEVerifier(options).Help();
        }
       
        
    } // end of class
}
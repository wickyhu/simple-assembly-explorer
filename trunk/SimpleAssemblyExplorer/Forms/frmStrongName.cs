using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System.Reflection;
using System.Resources;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public partial class frmStrongName : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public frmStrongName(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            if (!String.IsNullOrEmpty(Config.SNOutputDir))
            {
                txtOutputDir.Text = Config.SNOutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }

            if (!String.IsNullOrEmpty(Config.StrongKeyFile))
            {
                txtKeyFile.Text = Config.StrongKeyFile;
            }

            txtAdditionalOptions.Text = Config.SNAdditionalOptions;

            OptionChanged();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //if (SimpleMessage.ShowInfo("Are you sure to remove Strong Name of selected assemblies?",
            //    "Confirm",
            //    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            //    MessageBoxDefaultButton.Button2) == DialogResult.No)
            //{
            //    return;
            //}

            if (_sourceDir != null && _sourceDir.Equals(txtOutputDir.Text))
                Config.SNOutputDir = String.Empty;
            else
                Config.SNOutputDir = txtOutputDir.Text;


            if (rbSign.Checked)
            {
                string keyFile = txtKeyFile.Text;
                if (String.IsNullOrEmpty(keyFile))
                {
                    SimpleMessage.ShowInfo("Please choose key file.");
                    return;
                }

                if (!String.IsNullOrEmpty(keyFile) && !File.Exists(keyFile))
                {
                    SimpleMessage.ShowInfo("Invalid key file");
                    return;
                }

                if (!String.IsNullOrEmpty(keyFile))
                {
                    Config.StrongKeyFile = keyFile;
                }
            }

            Config.SNAdditionalOptions = txtAdditionalOptions.Text;

            bool resolveDirAdded1 = false;
            bool resolveDirAdded2 = false;
            try
            {
                resolveDirAdded1 = _host.AddAssemblyResolveDir(this._sourceDir);
                resolveDirAdded2 = _host.AddAssemblyResolveDir(txtOutputDir.Text);

                Utils.EnableUI(this.Controls, false);

                StrongNameOptions options = new StrongNameOptions();

                options.Host = _host;
                options.Rows = _rows;
                options.SourceDir = _sourceDir;
                options.OutputDir = txtOutputDir.Text;
                options.TextInfoBox = txtInfo;
                options.AdditionalOptions = txtAdditionalOptions.Text;

                options.chkQuietChecked = chkQuiet.Checked;
                options.chkOverwriteOriginalFileChecked = chkOverwrite.Checked;
                options.rbRemoveChecked = rbRemoveSN.Checked;
                options.rbSignChecked = rbSign.Checked;
                options.rbVrChecked = rbVr.Checked;
                options.rbVlChecked = rbVl.Checked;
                options.rbvfChecked = rbvf.Checked;
                options.rbVxChecked = rbVx.Checked;
                options.rbVuChecked = rbVu.Checked;
                options.rbRaChecked = false;
                options.rbCustomChecked = rbCustom.Checked;
                options.rbGacInstallChecked = rbGacInstall.Checked;
                options.rbGacRemoveChecked = rbGacRemove.Checked;
                options.KeyFile = txtKeyFile.Text;

                new StrongNamer(options).Go();

            }
            catch(Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
            finally
            {
                if(resolveDirAdded1)
                    _host.RemoveAssemblyResolveDir(this._sourceDir);
                if(resolveDirAdded2)
                    _host.RemoveAssemblyResolveDir(txtOutputDir.Text);
                Utils.EnableUI(this.Controls, true);
            }
        }

        private void OptionChanged()
        {
            txtKeyFile.Enabled = rbSign.Checked;
            btnSelFile.Enabled = txtKeyFile.Enabled;

            chkOverwrite.Enabled = rbSign.Checked || rbRemoveSN.Checked;

            txtOutputDir.Enabled = chkOverwrite.Enabled && !chkOverwrite.Checked;
            lblOutputDir.Enabled = txtOutputDir.Enabled;
            btnSelectOutput.Enabled = txtOutputDir.Enabled;
        }

        private void rbRemoveSN_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void rbSign_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void rbVr_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void rbVu_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFile(null, "Key Files (*.snk)|*.snk|All Files (*.*)|*.*", ".snk", true, Config.StrongKeyFile);

            if (!String.IsNullOrEmpty(path))
            {
                txtKeyFile.Text = path;
                Config.StrongKeyFile = path;
            }
        }

        private void btnSelectOutput_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFolder();
            if (!String.IsNullOrEmpty(path))
            {
                txtOutputDir.Text = path;
                Config.SNOutputDir = path;
            }
        }

        private void chkOverwrite_CheckedChanged(object sender, EventArgs e)
        {
            OptionChanged();
        }

        private void txtOutputDir_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtOutputDir.Text))
            {
                txtOutputDir.Text = _sourceDir;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            SNOptions options = new SNOptions();

            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;

            new SN(options).Help();
        }

        private void rbCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCustom.Checked)
            {
                txtAdditionalOptions.SelectAll();
                txtAdditionalOptions.Focus();
            }
        }


    } // end of class
}
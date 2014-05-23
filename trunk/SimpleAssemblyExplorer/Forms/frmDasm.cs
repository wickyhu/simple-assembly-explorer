using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public partial class frmDasm : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public frmDasm(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            if (!String.IsNullOrEmpty(Config.DasmOutputDir))
            {
                txtOutputDir.Text = Config.DasmOutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }
            txtAdditionalOptions.Text = Config.DasmAdditionalOptions;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_rows == null || _rows.Length == 0)
            {
                SimpleMessage.ShowInfo("No file found");
                return;
            }

            string outputDir = txtOutputDir.Text;
            if (!Directory.Exists(outputDir))
            {
                SimpleMessage.ShowInfo("Please choose output directory.");
                return;
            }

            if (_sourceDir != null && _sourceDir.Equals(outputDir))
                Config.DasmOutputDir = String.Empty;
            else 
                Config.DasmOutputDir = outputDir;
            
            Config.DasmAdditionalOptions = txtAdditionalOptions.Text;

            try
            {
                Utils.EnableUI(this.Controls, false);

                DasmOptions options = new DasmOptions();

                options.Host = _host;
                options.Rows = _rows;
                options.SourceDir = _sourceDir;
                options.OutputDir = outputDir;
                options.TextInfoBox = txtInfo;

                options.chkBytesChecked = chkBytes.Checked;
                options.chkTokensChecked = chkTokens.Checked;
                options.chkTypeListChecked = chkTypeList.Checked;
                options.chkCAVerbalChecked = chkCAVerbal.Checked;
                options.chkClassListChecked = chkClassList.Checked;
                options.chkUnicodeChecked = chkUnicode.Checked;
                options.chkUTF8Checked = chkUTF8.Checked;
                options.AdditionalOptions = txtAdditionalOptions.Text;

                new Disassembler(options).Go();

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


        private void btnSelectOutputDir_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFolder();

            if (!String.IsNullOrEmpty(path))
            {
                txtOutputDir.Text = path;
                Config.DasmOutputDir = path;
            }

        }

        private void txtOutputDir_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtOutputDir.Text))
            {
                txtOutputDir.Text = _sourceDir;
            }
        }

        private void chkUTF8_CheckedChanged(object sender, EventArgs e)
        {
            chkUnicode.Checked = !chkUTF8.Checked;
        }

        private void chkUnicode_CheckedChanged(object sender, EventArgs e)
        {
            chkUTF8.Checked = !chkUnicode.Checked;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            DasmOptions options = new DasmOptions();

            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;

            new Disassembler(options).Help();
        }     


    } // end of class
}
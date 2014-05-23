using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using SimpleAssemblyExplorer.Plugin;
using System.Windows.Forms;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer;

namespace SAE.de4dot
{
    public partial class frmDe4dot : Form
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public string RootDirectory
        {
            get { return _host.GetPropertyValue(Plugin.PropertyRootDirectory) as string; }
            //set { _host.SetPropertyValue(Plugin.PropertyRootDirectory, value); }
        }

        public string AdditionalOptions
        {
            get { return _host.GetPropertyValue(Plugin.PropertyAdditionalOptions) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyAdditionalOptions, value); }
        }

        public frmDe4dot(IHost host, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = host;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            InitForm();
        }

        private void InitForm()
        {
            #region find executables
            string[] exeFiles = Directory.GetFiles(this.RootDirectory, Plugin.Name + "*.exe");
            if (exeFiles != null && exeFiles.Length > 0)
            {
                int selected = -1;
                foreach (string exeFile in exeFiles)
                {
                    string name = Path.GetFileName(exeFile);
                    int index = cboExe.Items.Add(name);
                    if (Path.GetFileNameWithoutExtension(name) == Plugin.Name)
                        selected = index;
                }
                cboExe.SelectedIndex = (selected < 0 ? 0 : selected);
            }
            #endregion find executables

            if (_rows != null && _rows.Length > 1)
                chkCreateOutputDir.Checked = true;
            else
                chkCreateOutputDir.Checked = false;

            cboStringDecrypter.Items.Add(de4dotOptions.StringDecrypterTypes.Default);
            cboStringDecrypter.Items.Add(de4dotOptions.StringDecrypterTypes.Static);
            cboStringDecrypter.Items.Add(de4dotOptions.StringDecrypterTypes.Delegate);
            cboStringDecrypter.Items.Add(de4dotOptions.StringDecrypterTypes.Emulate);
            cboStringDecrypter.Items.Add(de4dotOptions.StringDecrypterTypes.None);
            cboStringDecrypter.SelectedIndex = 0;

            chkIgnoreUnsupported.Checked = true;
            chkIgnoreUnsupported.Enabled = false;
            txtAdditional.Text = this.AdditionalOptions;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //txtInfo.Text = String.Format("=== Started at {0} ===\r\n\r\n", DateTime.Now);
            //Application.DoEvents();

            this.AdditionalOptions = txtAdditional.Text;

            try
            {
                Utils.EnableUI(this.Controls, false);

                var options = GetOptions();
                new de4dot(options).Go();

            }
            catch (Exception ex)
            {
                SimpleTextbox.AppendText(txtInfo, String.Format("{0}\r\n\r\n", ex.Message));
            }
            finally
            {
                _host.ResetProgress();
                _host.SetStatusText(null);

                Utils.EnableUI(this.Controls, true);
            }

            //SimpleTextbox.AppendText(txtInfo, String.Format("\r\n=== Completed at {0} ===\r\n\r\n", DateTime.Now));
        }        

        private de4dotOptions GetOptions()
        {
            var options = new de4dotOptions();

            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;
            options.Rows = _rows;

            if (rbDeobf.Checked) options.Action = de4dotOptions.Actions.Deobfuscate;
            else if (rbDetect.Checked) options.Action = de4dotOptions.Actions.Detect;

            options.Verbose = chkVerbose.Checked;
            options.CreateOutputDir = chkCreateOutputDir.Checked;
            options.ScanDir = chkScanDir.Checked;

            options.StringDecrypterType = (de4dotOptions.StringDecrypterTypes)cboStringDecrypter.SelectedItem;
            options.StringDecrypterMethod = txtStringDecrypterMethod.Text;

            options.IgnoreUnsupported = chkIgnoreUnsupported.Checked;
            options.PreserveTokens = chkPreserveTokens.Checked;
            options.DontRename = chkDontRename.Checked;
            options.KeepTypes = chkKeepTypes.Checked;

            options.AdditionalOptions = txtAdditional.Text;

            return options;
        }


        private void btnHelp_Click(object sender, EventArgs e)
        {
            var options = new de4dotOptions();
            
            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;
            options.ExeFile = cboExe.SelectedItem as string;

            new de4dot(options).Help();
        }

        private void chkScanDir_CheckedChanged(object sender, EventArgs e)
        {
            if (chkScanDir.Checked)
            {
                chkCreateOutputDir.Checked = true;
                chkIgnoreUnsupported.Enabled = true;
            }
            else
            {
                chkIgnoreUnsupported.Enabled = false;
            }
        }

        private void btnSelectMethod_Click(object sender, EventArgs e)
        {
            var type = (SAE.de4dot.de4dotOptions.StringDecrypterTypes)cboStringDecrypter.SelectedItem;
            bool showStaticOnly = (type == de4dotOptions.StringDecrypterTypes.Default ||
                type == de4dotOptions.StringDecrypterTypes.Static);

            var p = new ClassEditParams()
            {
                Host = _host,
                Rows = _rows,
                SourceDir = _sourceDir,
                ObjectType = ObjectTypes.Method,
                ShowStaticOnly = showStaticOnly,
                ShowSelectButton = true
            };

            var selectedMethod = ClassEditUtils.Run(p);
            if (selectedMethod != null)
            {
                txtStringDecrypterMethod.Text = TokenUtils.GetFullMetadataTokenString(selectedMethod.MetadataToken);
            }
            else
            {
                txtStringDecrypterMethod.Text = String.Empty;
            }
        }


    } // end of class
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public partial class frmAsm : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public frmAsm(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            if (!String.IsNullOrEmpty(Config.AsmOutputDir))
            {
                txtOutputDir.Text = Config.AsmOutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }
            if (!String.IsNullOrEmpty(Config.StrongKeyFile))
            {
                txtKeyFile.Text = Config.StrongKeyFile;
                SetReplFile(Config.StrongKeyFile, false);
            }

            txtAdditionalOptions.Text = Config.AsmAdditionalOptions;
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
                SimpleMessage.ShowInfo("Please choose output directory");
                return;
            }

            string keyFile = rbSNSign.Checked ? txtKeyFile.Text : String.Empty;
            if (!String.IsNullOrEmpty(keyFile) && !File.Exists(keyFile))
            {
                SimpleMessage.ShowInfo("Invalid key file");
                return;
            }

            if (_sourceDir != null && _sourceDir.Equals(outputDir))
                Config.AsmOutputDir = String.Empty;
            else
                Config.AsmOutputDir = outputDir;

            if (!String.IsNullOrEmpty(keyFile) && File.Exists(keyFile))
            {
                Config.StrongKeyFile = keyFile;
            }
            

            if (chkReplaceToken.Checked)
            {
                if (txtOldToken.Text.Trim().Length != 16 ||
                    txtNewToken.Text.Trim().Length != 16
                    )
                {
                    SimpleMessage.ShowInfo("Invalid public key token");
                    return;
                }
                if (txtOldToken.Text.Trim().Equals(txtNewToken.Text.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    SimpleMessage.ShowInfo("Public key token cannot be same.");
                    txtNewToken.Text = String.Empty;
                    return;
                }
            }


            Config.AsmAdditionalOptions = txtAdditionalOptions.Text;

            try
            {
                Utils.EnableUI(this.Controls, false);

                AsmOptions options = new AsmOptions();

                options.Host = _host;
                options.Rows = _rows;
                options.SourceDir = _sourceDir;
                options.OutputDir = outputDir;
                options.TextInfoBox = txtInfo;

                options.chkQuietChecked = chkQuiet.Checked;
                options.rbDllChecked = rbTypeDll.Checked;
                options.rbExeChecked = rbTypeExe.Checked;
                options.rbDebugChecked = rbDbg.Checked;
                options.rbDebugImplChecked = rbDbgImpl.Checked;
                options.rbDebugOptChecked = rbDbgOpt.Checked;
                options.chkClockChecked = chkClock.Checked;
                options.chkNoLogoChecked = chkNoLogo.Checked;
                options.chkOptimizeChecked = chkOptimize.Checked;
                options.chkItaniumChecked = chkItanium.Checked;
                options.chkX64Checked = chkX64.Checked;

                if (rbSNSign.Checked)
                {
                    options.KeyFile = txtKeyFile.Text;
                }
                options.rbRemoveStrongNameChecked = rbSNRemove.Checked;
                options.chkRemoveLicenseProviderChecked = chkRemoveLicenseProvider.Checked;
                options.chkReplaceTokenChecked = chkReplaceToken.Checked;
                if (chkReplaceToken.Checked)
                {
                    options.txtOldTokenText = txtOldToken.Text;
                    options.txtNewTokenText = txtNewToken.Text;
                    options.ReplKeyFile = _replKeyFile;
                }
                options.AdditionalOptions = txtAdditionalOptions.Text;

                new Assembler(options).Go();

            }
            catch
            {
                throw;
            }
            finally
            {
                Utils.EnableUI(this.Controls, true);
            }
            
        }  
       

        private void btnSelectOutputDir_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFolder();
            if (!String.IsNullOrEmpty(path))
            {
                txtOutputDir.Text = path;
                Config.AsmOutputDir = path;
            }
        }

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFile(null, "Key Files (*.snk)|*.snk|All Files (*.*)|*.*", ".snk", true, Config.StrongKeyFile);

            if (!String.IsNullOrEmpty(path))
            {
                txtKeyFile.Text = path;
                Config.StrongKeyFile = path;

                SetReplFile(txtKeyFile.Text, false);
            }
        }

         private void chkReplaceToken_CheckedChanged(object sender, EventArgs e)
        {
            txtOldToken.Enabled = chkReplaceToken.Checked;
            //txtNewToken.Enabled = txtOldToken.Enabled;
            btnSelReplFile.Enabled = chkReplaceToken.Checked;

            if (chkReplaceToken.Checked)
            {
                SetReplFile(txtKeyFile.Text, false);
            }
        }

        private string GetFirstPublicKeyToken(string file)
        {
            string token = null;
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                int count = 0;
                bool startCount = false;
                while ((line = sr.ReadLine()) != null && count < 300)
                {                    
                    if (line.StartsWith(".assembly extern"))
                    {
                        startCount = true;

                        if (line.EndsWith("mscorlib") || 
                            line.EndsWith("System") || 
                            line.IndexOf("System.") > 0 || 
                            line.IndexOf("Microsoft.") > 0
                            )
                            continue;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line == "}") break;

                            string tmp = ".publickeytoken = (";
                            int p = line.IndexOf(tmp);
                            if (p >= 0)
                            {                                
                                token = line.Substring(p + tmp.Length, 8 * 3).Replace(" ", "").ToLower();
                                break;
                            }
                        }

                        if(token != null) 
                            break;
                    }
                    if (startCount) count++;
                }
            }
            return token;
        }

        private void FindToken()
        {
            try
            {
                if (String.IsNullOrEmpty(txtOldToken.Text))
                {
                    string file = PathUtils.GetFullFileName(_rows, 0, _sourceDir);
                    if (!String.IsNullOrEmpty(file))
                    {
                        string token = GetFirstPublicKeyToken(file);
                        if (!String.IsNullOrEmpty(token))
                        {
                            txtOldToken.Text = token;
                        }
                    }
                }

                string fileName = _replKeyFile;
                if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    txtNewToken.Text = TokenUtils.GetPublicKeyTokenString(TokenUtils.GetPublicKeyTokenFromKeyFile(fileName));
                }
                else
                {
                    txtNewToken.Text = String.Empty;
                }

            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void rbSNRemove_CheckedChanged(object sender, EventArgs e)
        {
            txtKeyFile.Enabled = !rbSNRemove.Checked;
            btnSelFile.Enabled = txtKeyFile.Enabled;
        }

        private void rbSNSign_CheckedChanged(object sender, EventArgs e)
        {
            txtKeyFile.Enabled = rbSNSign.Checked;
            btnSelFile.Enabled = txtKeyFile.Enabled;
        }

        string _replKeyFile = null;
        private void SetReplFile(string fileName, bool force)
        {
            if (String.IsNullOrEmpty(_replKeyFile) || force)
            {
                _replKeyFile = fileName;
                FindToken();
            }
        }

        private void btnSelReplFile_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFile(null, "Key Files (*.snk)|*.snk|All Files (*.*)|*.*", ".snk", true, Config.StrongKeyFile);

            if (!String.IsNullOrEmpty(path))
            {
                SetReplFile(path, true);
                Config.StrongKeyFile = path;
            }
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
            AsmOptions options = new AsmOptions();

            options.Host = _host;
            options.SourceDir = _sourceDir;
            options.TextInfoBox = txtInfo;

            new Assembler(options).Help();
        }

        private void chkX64_CheckedChanged(object sender, EventArgs e)
        {
            if (chkX64.Checked)
            {
                if (chkItanium.Checked)
                    chkItanium.Checked = false;
            }
        }

        private void chkItanium_CheckedChanged(object sender, EventArgs e)
        {
            if (chkItanium.Checked)
            {
                if (chkX64.Checked)
                    chkX64.Checked = false;
            }
        }

      
     

    } // end of class
}
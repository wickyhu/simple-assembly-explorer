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
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System.Reflection;
using SimpleUtils.Win;
using SimpleUtils;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public partial class frmDeobf : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public frmDeobf(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            InitForm(mainForm, rows, sourceDir);
            InitFormOnce();
        }

        private void InitFormOnce()
        {
            cboProfile.SelectedIndexChanged += new EventHandler(cboProfile_SelectedIndexChanged);

            _textFileChangedHandler = new FileSystemEventHandler(_host_TextFileChanged);
            _host.TextFileChanged += _textFileChangedHandler;

            LoadProfiles();
            PluginUtils.InitPluginGrid(dgvPlugin);

            #region Init values
            nudLoopCount.Value = Config.DeobfFlowOptionBranchLoopCount;
            nudMaxRefCount.Value = Config.DeobfFlowOptionMaxRefCount;
            txtRegex.Text = Config.LastRegex;

            InitBranchDirection(cboDirection);

            if (Config.DeobfFlowOptionBranchDirection >= 0 && Config.DeobfFlowOptionBranchDirection < cboDirection.Items.Count)
            {
                cboDirection.SelectedIndex = Config.DeobfFlowOptionBranchDirection;
            }
            else
            {
                cboDirection.SelectedIndex = 0;
            }
            #endregion
        }

        public void InitForm(IHost mainForm, string[] rows, string sourceDir)
        {
            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            if (_rows.Length != 1)
            {
                txtMethod.Enabled = false;
                btnSelectMethod.Enabled = false;
            }

            if (!String.IsNullOrEmpty(Config.DeobfOutputDir))
            {
                txtOutputDir.Text = Config.DeobfOutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }

            txtInfo.Clear();
        }      

        FileSystemEventHandler _textFileChangedHandler;
        void _host_TextFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name.Equals(ProfileFile.Default.FileName, StringComparison.OrdinalIgnoreCase))
            {
                LoadProfiles();
            }
        }

        private void LoadProfiles()
        {
            cboProfile.Items.Clear();

            foreach (DeobfProfile profile in ProfileFile.Default.Profiles)
            {
                cboProfile.Items.Add(profile);
            }
           
            if (cboProfile.Items.Count > 0)
            {
                if (Config.DeobfProfile < cboProfile.Items.Count)
                {
                    cboProfile.SelectedIndex = Config.DeobfProfile;
                }
                else
                {
                    cboProfile.SelectedIndex = 0;
                }
            }
        }

        private void InitBranchDirection(ComboBox cbo)
        {
            cbo.Items.Clear();

            Type t = typeof(BranchDirections);
            string[] names = Enum.GetNames(t);
            cbo.Items.AddRange(Enum.GetNames(t));
            cbo.SelectedIndex = 0;
        }

        void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeobfProfile profile = cboProfile.SelectedItem as DeobfProfile;
            if (profile != null)
            {
                profile.Options.ApplyTo(this);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isCancelPending = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            switch (btnOK.Text)
            {
                case Consts.Stop:
                    _isCancelPending = true;
                    return;
                default:
                    break;
            }

            string outputDir = txtOutputDir.Text;
            if (!Directory.Exists(outputDir))
            {
                SimpleMessage.ShowInfo("Please choose output directory.");
                return;
            }

            if (_sourceDir != null && _sourceDir.Equals(outputDir))
                Config.DeobfOutputDir = String.Empty;
            else
                Config.DeobfOutputDir = outputDir;

            Config.DeobfFlowOptionBranchLoopCount = (int)nudLoopCount.Value;
            Config.DeobfFlowOptionMaxRefCount = (int)nudMaxRefCount.Value;
            Config.LastRegex = txtRegex.Text;
            Config.DeobfProfile = cboProfile.SelectedIndex;
            Config.DeobfFlowOptionBranchDirection = cboDirection.SelectedIndex;

            try
            {
                btnOK.Text = Consts.Stop;
                _isCancelPending = false;

                Utils.EnableUI(this.Controls, false);

                DeobfOptions options = GetOptions();
                _host.TextInfo = options;
                Deobfuscator deobf = new Deobfuscator(options);
                deobf.Go();

                List<DeobfError> errors = deobf.Errors;
                if (errors.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(DeobfError error in errors) 
                    {
                        sb.Append(error.ToString());
                        sb.Append("\r\n\r\n");
                    }
                    SimpleMessage.ShowError(sb.ToString());
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _host.TextInfo = null;
                btnOK.Text = Consts.OK;

                Utils.EnableUI(this.Controls, true);
            }
        }

        private bool _isCancelPending;
        private bool IsCancelPending()
        {
            return _isCancelPending;
        }

        private DeobfOptions GetOptions()
        {
            DeobfOptions options = new DeobfOptions();

            options.Host = _host;
            options.Rows = _rows;
            options.SourceDir = _sourceDir;
            options.OutputDir = txtOutputDir.Text;
            options.TextInfoBox = txtInfo;
            options.IsCancelPendingFunction = new DeobfOptions.IsCancelPendingDelegate(this.IsCancelPending);

            options.ApplyFrom(this);

            options.StringOptionSearchForMethod = (
                _selectedMethod != null &&
                !String.IsNullOrEmpty(txtMethod.Text)) ? _selectedMethod : null;

            options.StringOptionCalledMethod = (
                _selectedCalledMethod != null &&
                !String.IsNullOrEmpty(txtCalledMethod.Text)) ? _selectedCalledMethod : null;

            options.txtRegexText = txtRegex.Text;
            options.BranchDirection = (BranchDirections)cboDirection.SelectedIndex;
            options.PluginList = PluginUtils.GetSelectedPluginFromGrid(dgvPlugin);

            return options;
        }

        private void btnSelectOutputDir_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFolder();

            if (!String.IsNullOrEmpty(path))
            {
                txtOutputDir.Text = path;
                Config.DeobfOutputDir = path;
            }

        }

        MethodDefinition _selectedMethod = null;
        MethodDefinition _selectedCalledMethod = null;

        private void btnSelectMethod_Click(object sender, EventArgs e)
        {            
             frmClassEdit f = new frmClassEdit(
                    new ClassEditParams() {
                        Host = _host,
                        Rows = _rows,
                        SourceDir = _sourceDir,
                        ObjectType = ObjectTypes.Method,
                        ShowStaticOnly = true,
                        ShowSelectButton = true
                    });
            f.ShowDialog();

            if (f.SelectedMethod != null)
            {
                txtMethod.Text = f.SelectedPath;
                _selectedMethod = f.SelectedMethod;
            }
            else
            {
                txtMethod.Text = String.Empty;
                _selectedMethod = null;
            }
        }

        private void btnSelectCalledMethod_Click(object sender, EventArgs e)
        {            
            frmClassEdit f = new frmClassEdit(
                new ClassEditParams() {
                        Host = _host,
                        Rows = new string[0],
                        SourceDir = _sourceDir,
                        ObjectType = ObjectTypes.Method,
                        ShowStaticOnly = true,
                        ShowSelectButton = true
                    });
            f.ShowDialog();

            if (f.SelectedMethod != null)
            {
                txtCalledMethod.Text = f.SelectedPath;
                _selectedCalledMethod = f.SelectedMethod;
            }
            else
            {
                txtCalledMethod.Text = String.Empty;
                _selectedCalledMethod = null;
            }

        }

        private void chkAutoString_CheckedChanged(object sender, EventArgs e)
        {
            if (_rows.Length == 1)
            {
                txtMethod.Enabled = !chkAutoString.Checked;
                btnSelectMethod.Enabled = txtMethod.Enabled;
                
                txtCalledMethod.Enabled = txtMethod.Enabled;
                btnSelectCalledMethod.Enabled = txtMethod.Enabled;
            }
            else
            {
                txtMethod.Enabled = false;
                btnSelectMethod.Enabled = false;

                txtCalledMethod.Enabled = false;
                btnSelectCalledMethod.Enabled = false;
            }
        }

        private void chkSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSwitch.Checked)
                chkBranch.Checked = true;
        }

        private void chkCondBranchDown_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCondBranchDown.Checked)
                chkBranch.Checked = true;
        }

        private void chkCondBranchUp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCondBranchDown.Checked)
                chkBranch.Checked = true;
        }

        private void chkBlockCopy_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCondBranchDown.Checked)
                chkBranch.Checked = true;
        }

        private void chkBranch_CheckedChanged(object sender, EventArgs e)
        {
            //nudLoopCount.Enabled = chkBranch.Checked;
            nudMaxRefCount.Enabled = chkBranch.Checked;
        }

        private void txtOutputDir_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtOutputDir.Text))
            {
                txtOutputDir.Text = _sourceDir;
            }
        }

        private void chkRegex_CheckedChanged(object sender, EventArgs e)
        {
            txtRegex.Enabled = chkRegex.Checked;
        }

        private void lblRegexFile_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", RegexFile.Default.FileName);
        }

        private void lblPatternFile_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", PatternFile.Default.FileName);
        }

        private void lblRemoveExceptionHandler_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", ExceptionHandlerFile.Default.FileName);
        }

        private void lblStringOption_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", IgnoredMethodFile.Default.FileName);
        }

        private void lblBooleanFunction_Click(object sender, EventArgs e)
        {
            lblStringOption_Click(sender, e);
        }

        private void lblRemoveAttribute_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", AttributeFile.Default.FileName);
        }

        private void lblProfile_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", ProfileFile.Default.FileName);
        }

        private void frmDeobf_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_textFileChangedHandler != null)
            {
                _host.TextFileChanged -= _textFileChangedHandler;
            }
        }

        private void lblIgnoredTypeFile_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", IgnoredTypeFile.Default.FileName);
        }
       
        private void lblRandom_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", RandomFile.Default.FileName);
        }

        private void dgvPlugin_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPlugin.SelectedRows.Count < 1) return;
            IDeobfPlugin dp = PluginUtils.GetSelectedPluginFromGrid(dgvPlugin.SelectedRows[0]);
            if (dp == null) return;
            dp.Configure();
        }          
        

    } // end of class
}
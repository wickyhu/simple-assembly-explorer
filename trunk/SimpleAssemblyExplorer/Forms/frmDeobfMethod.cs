using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System.Reflection;
using SimpleUtils;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public partial class frmDeobfMethod : frmDialogBase
    {
        MethodDefinition _method = null;
        MethodDefinition[] _staticMethods = new MethodDefinition[20];
        int _staticMethodCount = 0;

        public frmDeobfMethod(MethodDefinition method)
        {
            InitializeComponent();

            InitForm(method);
            InitFormOnce();
        }

        private void InitFormOnce()
        {
            InitBranchDirection(cboDirection);
            LoadProfiles();
            PluginUtils.InitPluginGrid(dgvPlugin);

            nudLoopCount.Value = Config.DeobfFlowOptionBranchLoopCount;
            nudMaxRefCount.Value = Config.DeobfFlowOptionMaxRefCount;
            nudMaxMoveCount.Value = Config.DeobfFlowOptionMaxMoveCount;

            if (Config.DeobfFlowOptionBranchDirection >= 0 && Config.DeobfFlowOptionBranchDirection < cboDirection.Items.Count)
            {
                cboDirection.SelectedIndex = Config.DeobfFlowOptionBranchDirection;
            }
            else
            {
                cboDirection.SelectedIndex = 0;
            }
        }

        public void InitForm(MethodDefinition method)
        {
            if (method == _method) return;
            if (!method.HasBody) return;

            //clear last method info
            cboMethods.Items.Clear();
            _staticMethodCount = 0;
            for (int i = 0; i < _staticMethods.Length; i++)
                _staticMethods[i] = null;

            //init current method
            _method = method;
            cboMethods.Items.Add("N/A");
            cboMethods.Items.Add("Automatic");

            Collection<Instruction> ic = _method.Body.Instructions;

            for (int i = 0; i < ic.Count; i++)
            {
                if (ic[i].Operand is MethodDefinition)
                {
                    MethodDefinition md = ic[i].Operand as MethodDefinition;
                    if (md.IsStatic && md.ReturnType.Name == "String")
                    {
                        string text = InsUtils.GetMethodText(md);
                        if (!cboMethods.Items.Contains(text))
                        {
                            _staticMethods[_staticMethodCount] = md;
                            _staticMethodCount++;
                            cboMethods.Items.Add(text);
                            if (_staticMethodCount >= _staticMethods.Length)
                                break;
                        }
                    }
                }
            }

            if (cboMethods.Items.Contains(_selectedText))
            {
                cboMethods.SelectedItem = _selectedText;
            }
            else
            {
                //if (cboMethods.Items.Count > 2)
                //    cboMethods.SelectedIndex = 1;
                //else
                    cboMethods.SelectedIndex = 0;
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

        private void LoadProfiles()
        {
            cboProfile.Items.Clear();

            foreach (DeobfProfile profile in ProfileFile.Default.Profiles)
            {
                cboProfile.Items.Add(profile);
            }

            if (cboProfile.Items.Count > 0)
            {
                DeobfProfile profile = ProfileFile.Default.GetProfile("Flow without Boolean Function");
                if (profile != null && cboProfile.Items.Contains(profile))
                {
                    cboProfile.SelectedItem = profile;
                }
                else if (Config.DeobfProfile < cboProfile.Items.Count)
                {
                    cboProfile.SelectedIndex = Config.DeobfProfile;
                }
                else
                {
                    cboProfile.SelectedIndex = 0;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        string _selectedText = String.Empty;
        MethodDefinition _selectedMethod = null;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboMethods.SelectedIndex > 1)
            {
                _selectedText = cboMethods.Text;
                _selectedMethod = _staticMethods[cboMethods.SelectedIndex - 2];
            }
            else if (cboMethods.SelectedIndex == 1)
            {
                _selectedText = cboMethods.Text;
                _selectedMethod = null;
            }
            else
            {
                _selectedText = String.Empty;
                _selectedMethod = null;
            }

            Config.DeobfFlowOptionBranchLoopCount = (int) nudLoopCount.Value;
            Config.DeobfFlowOptionMaxMoveCount = (int)nudMaxMoveCount.Value;
            Config.DeobfFlowOptionMaxRefCount = (int)nudMaxRefCount.Value;
            Config.DeobfFlowOptionBranchDirection = cboDirection.SelectedIndex;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public MethodDefinition SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
        }

        public bool AutoFlowSwitch
        {
            get
            {
                return chkSwitch.Checked;
            }
        }

        public bool AutoFlowBranch
        {
            get
            {
                return chkBranch.Checked;
            }
        }

        public bool AutoFlowConditionalBranchDown
        {
            get
            {
                return chkCondBranchDown.Checked;
            }
        }

        public bool AutoFlowConditionalBranchUp
        {
            get
            {
                return chkCondBranchUp.Checked;
            }
        }

        public bool AutoFlowBlockMove
        {
            get
            {
                return chkBlockMove.Checked;
            }
        }

        public bool AutoFlowBoolFunction
        {
            get
            {
                return chkBoolFunction.Checked;
            }
        }

        public bool AutoFlowPattern
        {
            get
            {
                return chkPattern.Checked;
            }
        }

        public bool AutoFlowUnreachable
        {
            get
            {
                return chkUnreachable.Checked;
            }
        }

        public bool AutoFlowExceptionHandler
        {
            get
            {
                return chkRemoveExceptionHandler.Checked;
            }
        }

        public bool AutoFlowDelegateCall
        {
            get
            {
                return chkDelegateCall.Checked;
            }
        }

        public bool AutoFlowDirectCall
        {
            get
            {
                return chkDirectCall.Checked;
            }
        }

        public bool AutoFlowReflectorFix
        {
            get
            {
                return chkReflectorFix.Checked;
            }
        }

        public bool AutoFlowRemoveInvalidInstruction
        {
            get
            {
                return chkRemoveInvalidInstruction.Checked;
            }
        }

        public bool AutoString
        {
            get
            {
                return cboMethods.SelectedIndex == 1;
            }
        }

        public BranchDirections BranchDirection
        {
            get
            {
                return (BranchDirections)cboDirection.SelectedIndex;
            }
        }

        public bool InitLocalVars
        {
            get
            {
                return chkInitLocalVars.Checked;
            }
        }

        public List<IDeobfPlugin> PluginList
        {
            get
            {
                return PluginUtils.GetSelectedPluginFromGrid(dgvPlugin);
            }
        }

        private void chkSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSwitch.Checked)
                chkBranch.Checked = true;
        }

        private void chkBranch_CheckedChanged(object sender, EventArgs e)
        {
            //nudLoopCount.Enabled = chkBranch.Checked;
            nudMaxRefCount.Enabled = chkBranch.Checked;
        }

        private void chkCondBranch_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCondBranchDown.Checked)
                chkBranch.Checked = true;
        }

        private void lblRemoveExceptionHandler_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", ExceptionHandlerFile.Default.FileName);
        }

        private void lblPatternFile_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", PatternFile.Default.FileName);
        }

        private void lblBooleanFunction_Click(object sender, EventArgs e)
        {
            lblStringOption_Click(sender, e);
        }

        private void lblStringOption_Click(object sender, EventArgs e)
        {
            SimpleProcess.Start("notepad.exe", IgnoredMethodFile.Default.FileName);
        }

        private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeobfProfile profile = cboProfile.SelectedItem as DeobfProfile;
            if (profile != null)
            {
                profile.Options.ApplyTo(this);
            }
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

        private void dgvPlugin_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPlugin.SelectedRows.Count < 1) return;
            IDeobfPlugin dp = PluginUtils.GetSelectedPluginFromGrid(dgvPlugin.SelectedRows[0]);
            if (dp == null) return;
            dp.Configure();
        }

    } // end of class
}
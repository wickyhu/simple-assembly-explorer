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
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class frmClassEditInstruction : frmDialogBase
    {
        public enum EditModes 
        {
            Edit,
            InsertBefore,
            InsertAfter
        }

        ILProcessor _ilp = null;
        MethodDefinition _method = null;
        Instruction _ins = null;
        int _insIndex = -1;
        EditModes _mode = EditModes.Edit;
        frmClassEdit _mainForm;

        public frmClassEditInstruction(frmClassEdit mainForm, MethodDefinition md, int insIndex, EditModes mode)
        {
            InitializeComponent();

            InitForm(mainForm, md, insIndex, mode);
        }

        public void InitForm(frmClassEdit mainForm, MethodDefinition md, int insIndex, EditModes mode)
        {
            if (_method == md && _insIndex == insIndex) return;

            _mainForm = mainForm;
            _method = md;
            _ilp = md.Body.GetILProcessor();
            _insIndex = insIndex;
            _mode = mode;
            Collection<Instruction> ic = _method.Body.Instructions;

            switch (mode)
            {
                case EditModes.Edit:
                    _ins = ic[_insIndex];
                    if (_ins.Previous == null)
                    {
                        txtPrevIns.Text = String.Empty;
                    }
                    else
                    {
                        txtPrevIns.Text = InsUtils.GetInstructionText(ic, ic.IndexOf(_ins.Previous));
                    }
                    if (_ins.Next == null)
                    {
                        txtNextIns.Text = String.Empty;
                    }
                    else
                    {
                        txtNextIns.Text = InsUtils.GetInstructionText(ic, ic.IndexOf(_ins.Next));
                    }
                    break;
                case EditModes.InsertAfter:
                    _ins = _ilp.Create(OpCodes.Nop);
                    txtPrevIns.Text = InsUtils.GetInstructionText(ic, _insIndex);
                    if (ic[_insIndex].Next == null)
                    {
                        txtNextIns.Text = String.Empty;
                    }
                    else
                    {
                        txtNextIns.Text = InsUtils.GetInstructionText(ic, ic.IndexOf(ic[_insIndex].Next));
                    }
                    break;
                case EditModes.InsertBefore:
                    _ins = _ilp.Create(OpCodes.Nop);
                    txtNextIns.Text = InsUtils.GetInstructionText(ic, _insIndex);
                    if (ic[_insIndex].Previous == null)
                    {
                        txtPrevIns.Text = String.Empty;
                    }
                    else
                    {
                        txtPrevIns.Text = InsUtils.GetInstructionText(ic, ic.IndexOf(ic[_insIndex].Previous));
                    }
                    break;
            }


            InitOpCodes(cboOpCode);

        }

        private void InitOpCodes(ComboBox cbo)
        {
            cbo.Items.Clear();
            cbo.Sorted = true;

            Type t = typeof(OpCodes);
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType.Name != "OpCode") continue;
                OpCode op = (OpCode)t.InvokeMember(fi.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField,
                    null, null, null);
                cbo.Items.Add(op);
            }

            cbo.SelectedIndex = cbo.Items.IndexOf(_ins.OpCode);                
        }

        private void DisableOperand()
        {
            cboOperand.Enabled = false;
            cboOperand.Items.Clear();
            cboOperand.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void cboOpCode_TextChanged(object sender, EventArgs e)
        {
            if (cboOpCode.Text.Length > 0)
            {
                int index = cboOpCode.FindStringExact(cboOpCode.Text);
                if (index >= 0)
                {
                    if (index != cboOpCode.SelectedIndex)
                        cboOpCode.SelectedIndex = index;
                }
                else
                {
                    DisableOperand();
                }
            }
            else
            {
                DisableOperand();
            }
        }

        private void cboOpCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelectMethod.Enabled = false;

            if (cboOpCode.SelectedItem == null)
            {
                DisableOperand();
                return;
            }

            OpCode op = (OpCode)cboOpCode.SelectedItem;
            lblOpCodeInfo.Text = String.Format("FlowControl: {0}\nOpCodeType: {1}\nOperandType: {2}", op.FlowControl.ToString(), op.OpCodeType.ToString(), op.OperandType.ToString());
            Collection<Instruction> ic = _method.Body.Instructions;
            switch (op.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    InitInstructions(cboOperand);
                    break;
                case OperandType.InlineString:
                    cboOperand.Enabled = true;
                    cboOperand.Items.Clear();
                    cboOperand.DropDownStyle = ComboBoxStyle.DropDown;
                    if (_ins.Operand != null)
                    {
                        cboOperand.Items.Add(_ins.Operand);
                        cboOperand.SelectedIndex = 0;
                    }
                    break;
                case OperandType.InlineNone:
                    DisableOperand();
                    break;
                case OperandType.InlineI:
                case OperandType.InlineI8:
                case OperandType.ShortInlineI:
                case OperandType.InlineR:
                case OperandType.ShortInlineR:
                    cboOperand.Enabled = true;
                    cboOperand.Items.Clear();
                    cboOperand.DropDownStyle = ComboBoxStyle.DropDown;
                    cboOperand.Items.Add(InsUtils.GetOperandText(ic, _insIndex));
                    cboOperand.SelectedIndex = 0;
                    break;
                case OperandType.InlineSwitch:
                    cboOperand.Enabled = true;
                    cboOperand.Items.Clear();
                    cboOperand.DropDownStyle = ComboBoxStyle.DropDown;
                    cboOperand.Items.Add(InsUtils.GetOperandText(ic, _insIndex));
                    cboOperand.SelectedIndex = 0;
                    Instruction[] switchIns = _ins.Operand as Instruction[];
                    cboOperand.Items.Add("----------------------------------------");
                    for (int i = 0; i < switchIns.Length; i++)
                    {
                        cboOperand.Items.Add(
                            String.Format("{0} (0x{0:x}): {1}", i, InsUtils.GetInstructionText(ic, ic.IndexOf(switchIns[i])))
                            );
                    }
                    break;
                case OperandType.InlineField:
                    if (_ins.Operand == null || _method.DeclaringType.Fields.Contains(_ins.Operand as FieldDefinition))
                    {
                        InitFields(cboOperand);
                    }
                    else
                    {
                        cboOperand.Enabled = true;
                        cboOperand.Items.Clear();
                        cboOperand.DropDownStyle = ComboBoxStyle.DropDownList;
                        cboOperand.Items.Add(InsUtils.GetOperandText(ic, _insIndex));
                        cboOperand.SelectedIndex = 0;
                    }
                    break;
                case OperandType.InlineVar:
                case OperandType.ShortInlineVar:
                    InitVars(cboOperand);
                    break;
                case OperandType.ShortInlineArg:
                case OperandType.InlineArg:
                    InitParams(cboOperand);
                    break;
                case OperandType.InlineMethod:
                    InitMethods(cboOperand);
                    btnSelectMethod.Enabled = true;
                    break;
                case OperandType.InlineType:
                    cboOperand.Enabled = true;
                    cboOperand.Items.Clear();
                    cboOperand.DropDownStyle = ComboBoxStyle.DropDown;
                    if (_ins.Operand != null)
                    {
                        cboOperand.Items.Add(_ins.Operand);
                        cboOperand.SelectedIndex = 0;
                    }
                    ClassEditHelper.InitDropdownTypes(cboOperand, _method.Module);
                    break;
                default:
                    cboOperand.Enabled = true;
                    cboOperand.Items.Clear();
                    cboOperand.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (_ins.Operand != null)
                    {
                        cboOperand.Items.Add(_ins.Operand);
                        cboOperand.SelectedIndex = 0;
                    }
                    break;
            }            
        }

        private void cboOperand_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpCode op = (OpCode) cboOpCode.SelectedItem;
            switch (op.OperandType)
            {
                case OperandType.InlineSwitch:
                    if (cboOperand.SelectedIndex > 0)
                    {
                        SimpleMessage.ShowInfo("Invalid switch operand.");
                        cboOperand.SelectedIndex = 0;
                    }
                    break;
            }
        }

        private void InitInstructions(ComboBox cbo)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            Collection<Instruction> ic = _method.Body.Instructions;
            for (int i = 0; i < ic.Count; i++)
            {
                cbo.Items.Add(InsUtils.GetInstructionText(ic, i));
            }

            if (cbo.Items.Count > 0)
            {
                if (_ins.Operand is Instruction)
                    cbo.SelectedIndex = ic.IndexOf(_ins.Operand as Instruction);
                else
                    cbo.SelectedIndex = 0;
            }
        }

        private void InitFields(ComboBox cbo)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            Collection<FieldDefinition> fdc = _method.DeclaringType.Fields;
            for (int i = 0; i < fdc.Count; i++)
            {
                cbo.Items.Add(fdc[i]);
            }

            if (cbo.Items.Count > 0)
            {
                if (_ins.Operand is FieldDefinition)
                    cbo.SelectedIndex = fdc.IndexOf(_ins.Operand as FieldDefinition);
                else
                    cbo.SelectedIndex = 0;
            }
        }

        private void InitVars(ComboBox cbo)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            Collection<VariableDefinition> vc = _method.Body.Variables;
            for (int i = 0; i < vc.Count; i++)
            {
                cbo.Items.Add(vc[i]);
            }

            if (cbo.Items.Count > 0)
            {
                if (_ins.Operand is VariableDefinition)
                    cbo.SelectedIndex = vc.IndexOf(_ins.Operand as VariableDefinition);
                else
                    cbo.SelectedIndex = 0;
            }
        }

        private void InitParams(ComboBox cbo)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            Collection<ParameterDefinition> pc = _method.Parameters;
            for (int i = 0; i < pc.Count; i++)
            {
                cbo.Items.Add(pc[i]);
            }

            if (cbo.Items.Count > 0)
            {
                if (_ins.Operand is ParameterDefinition)
                    cbo.SelectedIndex = pc.IndexOf(_ins.Operand as ParameterDefinition);
                else
                    cbo.SelectedIndex = 0;
            }
        }

        const string METHOD_MESSAGEBOX = "System.Windows.Forms.MessageBox.Show(String)";

        private void InitMethods(ComboBox cbo)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            cbo.Items.Add(METHOD_MESSAGEBOX);

            if (_ins.Operand is MethodReference && !cbo.Items.Contains(_ins.Operand))
            {
                cbo.Items.Insert(0, _ins.Operand);
            }
            if (_mainForm.SelectedMethodHistory.Count > 0)
            {
                foreach (MethodReference mr in _mainForm.SelectedMethodHistory)
                {
                    if (cbo.Items.Contains(mr))
                        continue;
                    cbo.Items.Add(mr);
                }
            }
            if (cbo.Items.Count > 0)
            {
                cbo.SelectedIndex = 0;
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboOpCode.SelectedItem == null)
            {
                SimpleMessage.ShowInfo("Please select proper OpCode.");
                return;
            }

            try
            {
                OpCode op = (OpCode)cboOpCode.SelectedItem;

                switch (op.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        _ins.OpCode = op;
                        _ins.Operand = _method.Body.Instructions[cboOperand.SelectedIndex];
                        break;
                    case OperandType.InlineString:
                        _ins.OpCode = op;
                        _ins.Operand = cboOperand.Text;
                        break;
                    case OperandType.InlineNone:
                        _ins.OpCode = op;
                        _ins.Operand = null;
                        break;
                    case OperandType.InlineI:
                        _ins.OpCode = op;
                        _ins.Operand = SimpleParse.ParseInt(cboOperand.Text);
                        break;
                    case OperandType.ShortInlineI:
                        _ins.OpCode = op;
                        if (op.Code == Code.Ldc_I4_S)
                        {
                            _ins.Operand = SimpleParse.ParseSByte(cboOperand.Text);
                        }
                        else
                        {
                            _ins.Operand = SimpleParse.ParseByte(cboOperand.Text);
                        }
                        break;
                    case OperandType.InlineR:
                        _ins.OpCode = op;
                        _ins.Operand = Double.Parse(cboOperand.Text);
                        break;
                    case OperandType.ShortInlineR:
                        _ins.OpCode = op;
                        _ins.Operand = Single.Parse(cboOperand.Text);
                        break;
                    case OperandType.InlineI8:
                        _ins.OpCode = op;
                        _ins.Operand = SimpleParse.ParseLong(cboOperand.Text);
                        break;
                    case OperandType.InlineField:
                        _ins.OpCode = op;
                        _ins.Operand = cboOperand.SelectedItem;
                        break;
                    case OperandType.InlineVar:
                    case OperandType.ShortInlineVar:
                        _ins.OpCode = op;
                        _ins.Operand = cboOperand.SelectedItem;
                        break;
                    case OperandType.ShortInlineArg:
                    case OperandType.InlineArg:
                        _ins.OpCode = op;
                        _ins.Operand = cboOperand.SelectedItem;
                        break;
                    case OperandType.InlineMethod:
                        _ins.OpCode = op;
                        if (cboOperand.SelectedItem is MethodReference)
                        {
                            MethodReference mr = (MethodReference)cboOperand.SelectedItem;
                            if (mr.DeclaringType.Module.FullyQualifiedName != _method.DeclaringType.Module.FullyQualifiedName)
                            {
                                _ins.Operand = _method.DeclaringType.Module.Import(mr);
                            }
                            else
                            {
                                _ins.Operand = mr;
                            }
                        }
                        else
                        {
                            MethodInfo mi;
                            MethodReference mr;
                            switch (cboOperand.Text)
                            {                               
                                case METHOD_MESSAGEBOX:
                                    mi = typeof(System.Windows.Forms.MessageBox).GetMethod("Show", new Type[] { typeof(string) });
                                    mr = _method.DeclaringType.Module.Import(mi);
                                    _ins.Operand = mr;
                                    break;
                            }
                        }
                        break;
                    case OperandType.InlineSwitch:
                        {
                            string opStr = cboOperand.Text;
                            if (opStr != null)
                            {
                                string[] ops = opStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (ops != null && ops.Length > 0)
                                {
                                    Instruction[] opIns = new Instruction[ops.Length];
                                    for (int i = 0; i < ops.Length; i++)
                                    {
                                        int opIndex = -1;
                                        if (int.TryParse(ops[i], out opIndex))
                                        {
                                            if (opIndex >= 0 && opIndex < _method.Body.Instructions.Count)
                                            {
                                                opIns[i] = _method.Body.Instructions[opIndex];
                                            }
                                            else
                                            {
                                                opIndex = -1;
                                            }
                                        }
                                        if (opIndex == -1)
                                        {
                                            throw new Exception("Invalid instruction index" + ops[i]);
                                        }
                                    }
                                    _ins.Operand = null;
                                    _ins.Operand = opIns;
                                }
                            }
                        }
                        break;
                    case OperandType.InlineType:
                        _ins.OpCode = op;
                        if (cboOperand.SelectedItem == null)
                            _ins.Operand = ClassEditHelper.ParseTypeReference(cboOperand.Text, _method.Module);
                        else
                            _ins.Operand = ClassEditHelper.ParseTypeReference(cboOperand.SelectedItem, _method.Module);
                        break;
                    default:
                        if (_ins.OpCode.OperandType != op.OperandType)
                        {
                            SimpleMessage.ShowInfo(op.Name + " not implemented");
                            return;
                        }
                        else
                        {
                            _ins.OpCode = op;
                            if (cboOperand.SelectedIndex >= 0)
                                _ins.Operand = cboOperand.SelectedItem;
                            else
                                _ins.Operand = null;
                        }
                        break;
                }


                switch (_mode)
                {
                    case EditModes.Edit:
                        break;
                    case EditModes.InsertAfter:
                        _ilp.InsertAfter(_method.Body.Instructions[_insIndex], _ins);
                        break;
                    case EditModes.InsertBefore:
                        _ilp.InsertBefore(_method.Body.Instructions[_insIndex], _ins);
                        break;
                }

                InsUtils.ComputeOffsets(_method.Body.Instructions);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        frmClassEdit _frmClassView = null;
        private void btnSelectMethod_Click(object sender, EventArgs e)
        {
            if (_frmClassView == null)
                _frmClassView = new frmClassEdit(
                    new ClassEditParams() {
                        Host = _mainForm.Host,
                        Rows = _mainForm.Rows,
                        SourceDir = _mainForm.SourceDir,
                        ObjectType = ObjectTypes.Method,
                        ShowStaticOnly = false,
                        ShowSelectButton = true
                    });
            _frmClassView.ShowDialog();

            if (_frmClassView.SelectedMethod != null)
            {                
                MethodDefinition md = _frmClassView.SelectedMethod;

                MethodReference mr;
                if(md.DeclaringType.Module.FullyQualifiedName == _method.DeclaringType.Module.FullyQualifiedName) 
                    mr = md;
                else
                    mr = _method.DeclaringType.Module.Import(md);
                
                if(!cboOperand.Items.Contains(mr))
                    cboOperand.Items.Add(mr);
                if (!_mainForm.SelectedMethodHistory.Contains(mr))
                    _mainForm.SelectedMethodHistory.Add(mr);
                cboOperand.SelectedItem = mr;
            }
        }



    } // end of class
}
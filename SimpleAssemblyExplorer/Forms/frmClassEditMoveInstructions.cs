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
    public partial class frmClassEditMoveInstructions : frmDialogBase
    {
        MethodDefinition _method = null;
        int _start = -1;
        int _end = -1;
        int _displayStartIndex = -1;

        public frmClassEditMoveInstructions(MethodDefinition md, int start, int end)
        {
            InitializeComponent();

            InitForm(md, start, end);
        }

        public void InitForm(MethodDefinition md, int start, int end)
        {
            _method = md;
            _start = start;
            _end = end;

            Collection<Instruction> ic = _method.Body.Instructions;
            InsUtils.InitInstructionsCombobox(cboStart, _method, ic[_start], ref instructionsStr);
            InsUtils.InitInstructionsCombobox(cboEnd, _method, ic[_end], ref instructionsStr);
            
            InsUtils.InitInstructionsCombobox(cboTo, _method, null, ref instructionsStr);
        }
     
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private bool Save()
        {
            int start = cboStart.SelectedIndex - 1;
            int end = cboEnd.SelectedIndex - 1;
            int to = cboTo.SelectedIndex - 1;

            if (start < 0 || end < 0 || start > end)
            {
                SimpleMessage.ShowError("Invalid instruction range.");
                return false;
            }

            if (to < 0 || (_start <= to && to <= _end))
            {
                SimpleMessage.ShowError("Invalid target instruction.");
                return false;
            }

            if (to < start)
            {
                _displayStartIndex = to + 1;
            }
            else
            {
                _displayStartIndex = to - (end - start + 1) + 1;
            }
            if (rbBefore.Checked)
            {
                if (to > 0)
                {
                    DeobfUtils.MoveBlock(_method.Body, to - 1, start, end);
                }
                else
                {
                    DeobfUtils.MoveBlock(_method.Body, 0, start, end);
                    DeobfUtils.MoveBlock(_method.Body, end - start + 1, 0, 0);
                }
            }
            else
            {
                DeobfUtils.MoveBlock(_method.Body, to, start, end);
                _displayStartIndex++;
            }

            _start = start;
            _end = end;
            return true;
        }

        string[] instructionsStr;

        public int DisplayStartIndex
        {
            get { return _displayStartIndex; }
        }

        public int DisplayLength
        {
            get { return _end - _start + 1; }
        }

    } // end of class
}
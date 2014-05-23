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
    public partial class frmClassEditExceptionHandler : frmDialogBase
    {
        MethodDefinition _method = null;
        int _ehIndex = -1;
        ExceptionHandler _eh;

        public frmClassEditExceptionHandler(MethodDefinition md, int ehIndex)
        {
            InitializeComponent();

            InitForm(md, ehIndex);
        }

        public void InitForm(MethodDefinition md, int ehIndex)
        {
            if (_method == md && _ehIndex == ehIndex) return;

            _method = md;
            _ehIndex = ehIndex;
            if (_ehIndex < 0)
            {
                _eh = new ExceptionHandler(ExceptionHandlerType.Catch);
                _eh.CatchType = _method.Module.Import(typeof(System.Exception));                
                InitTypes(ExceptionHandlerType.Catch);
            }
            else
            {
                _eh = _method.Body.ExceptionHandlers[_ehIndex];
                if(_eh.CatchType == null)
                    _eh.CatchType = _method.Module.Import(typeof(System.Exception));                
                InitTypes(_eh.HandlerType);
            }

            txtCatchType.Text = _eh.CatchType.FullName;

            InsUtils.InitInstructionsCombobox(cboTryStart, _method, _eh.TryStart, ref instructionsStr);
            InsUtils.InitInstructionsCombobox(cboTryEnd, _method, _eh.TryEnd, ref instructionsStr);

            InsUtils.InitInstructionsCombobox(cboHandlerStart, _method, _eh.HandlerStart, ref instructionsStr);
            InsUtils.InitInstructionsCombobox(cboHandlerEnd, _method, _eh.HandlerEnd, ref instructionsStr);

            InsUtils.InitInstructionsCombobox(cboFilterStart, _method, _eh.FilterStart, ref instructionsStr);
            //InsUtils.InitInstructionsCombobox(cboFilterEnd, _method, _eh.FilterEnd, ref instructionsStr);

        }

        private void InitTypes(ExceptionHandlerType selected)
        {
            cboType.Items.Clear();
            foreach (string s in AssemblyUtils.GetEnumNames(typeof(ExceptionHandlerType)))
            {
                cboType.Items.Add(s);
            }
            cboType.SelectedIndex = (int)selected;
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
                Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void Save()
        {
            _eh.HandlerType = (ExceptionHandlerType)cboType.SelectedIndex;

            Collection<Instruction> ic = _method.Body.Instructions;
            _eh.TryStart = cboTryStart.SelectedIndex == 0 ? null : ic[cboTryStart.SelectedIndex - 1];
            _eh.TryEnd = cboTryEnd.SelectedIndex == 0 ? null : ic[cboTryEnd.SelectedIndex - 1];

            _eh.HandlerStart = cboHandlerStart.SelectedIndex == 0 ? null : ic[cboHandlerStart.SelectedIndex - 1];
            _eh.HandlerEnd = cboHandlerEnd.SelectedIndex == 0 ? null : ic[cboHandlerEnd.SelectedIndex - 1];

            _eh.FilterStart = cboFilterStart.SelectedIndex == 0 ? null : ic[cboFilterStart.SelectedIndex - 1];
            //_eh.FilterEnd = cboFilterEnd.SelectedIndex == 0 ? null : ic[cboFilterEnd.SelectedIndex - 1];

            if (_ehIndex < 0)
            {
                _method.Body.ExceptionHandlers.Add(_eh);
                _ehIndex = _method.Body.ExceptionHandlers.IndexOf(_eh);
            }
        }

        string[] instructionsStr;

       
    } // end of class
}
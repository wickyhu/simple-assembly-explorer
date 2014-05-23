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
    public partial class frmClassEditVariable : frmDialogBase
    {
        public enum EditModes 
        {
            Append,
            InsertBefore,
            InsertAfter,
            Edit,
            Duplicate
        }

        MethodDefinition _method = null;
        int _varIndex = -1;
        EditModes _mode = EditModes.Edit;

        public frmClassEditVariable(MethodDefinition md, int varIndex, EditModes mode)
        {
            InitializeComponent();

            InitForm(md, varIndex, mode);
        }

        private void InitTypeSpecification()
        {
            cboSpecification.Items.Clear();
            cboSpecification.Items.Add("Default");
            cboSpecification.Items.Add("Array");
            cboSpecification.Items.Add("Reference");
            cboSpecification.Items.Add("Pointer");
            cboSpecification.SelectedIndex = 0;
        }

        public void InitForm(MethodDefinition md, int varIndex, EditModes mode)
        {
            if (_method == md && _varIndex == varIndex) return;

            _method = md;
            _varIndex = varIndex;
            _mode = mode;

            Collection<VariableDefinition> vdc = _method.Body.Variables;

            VariableDefinition vd = null;

            InitTypeSpecification();

            if (_varIndex >= 0 && _varIndex < vdc.Count)
            {
                vd = vdc[_varIndex];
                cboType.Items.Add(vd.VariableType);

                if (vd.VariableType.IsArray)
                {
                    cboSpecification.SelectedItem = "Array";
                }
                else if (vd.VariableType.IsByReference)
                {
                    cboSpecification.SelectedItem = "Reference";
                }
                else if (vd.VariableType.IsPointer)
                {
                    cboSpecification.SelectedItem = "Pointer";
                }
            }

            switch (mode)
            {
                case EditModes.Edit:
                    if (vd != null)
                    {
                        txtIndex.Text = vd.Index.ToString();
                        txtName.Text = vd.ToString();
                        cboType.SelectedItem = vd.VariableType;
                    }
                    break;
                case EditModes.Duplicate:
                    if (vd != null)
                    {
                        cboType.SelectedItem = vd.VariableType;
                    }
                    break;
                default:
                    break;
            }
        }
       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private TypeReference ParseTypeReference()
        {
            TypeReference tr = ClassEditHelper.ParseTypeReference(cboType.SelectedItem, _method.Module);
            if (tr == null)
                tr = ClassEditHelper.ParseTypeReference(cboType.Text.Trim(), _method.Module);
            if (tr != null)
            {
                switch (cboSpecification.SelectedItem as string)
                {
                    case "Array":
                        tr = new ArrayType(tr);
                        break;
                    case "Reference":
                        tr = new ByReferenceType(tr);
                        break;
                    case "Pointer":
                        tr = new PointerType(tr);
                        break;
                    default:
                        break;
                }
            }

            return tr;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            //if (String.IsNullOrEmpty(name))
            //{
            //    SimpleMessage.ShowInfo("Please enter variable name.");
            //    return;
            //}

            TypeReference tr = ParseTypeReference();
            if (tr == null)
            {
                SimpleMessage.ShowInfo("Please select or enter variable type.");
                return;
            }

            try
            {
                Collection<VariableDefinition> vdc = _method.Body.Variables;
                VariableDefinition vd;

                switch (_mode)
                {
                    case EditModes.Append:
                        vd = new VariableDefinition(name, tr);
                        vdc.Add(vd);
                        break;
                    case EditModes.Edit:
                        vd = vdc[_varIndex];
                        vd.Name = name;
                        vd.VariableType = tr;                        
                        break;
                    case EditModes.InsertAfter:
                        vd = new VariableDefinition(name, tr);
                        vdc.Insert(_varIndex + 1, vd);
                        break;
                    case EditModes.InsertBefore:
                        vd = new VariableDefinition(name, tr);
                        vdc.Insert(_varIndex, vd);
                        break;
                    case EditModes.Duplicate:
                        vd = new VariableDefinition(name, tr);
                        vdc.Add(vd);
                        break;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ClassEditHelper.InitDropdownTypes(cboType, _method.Module);
            cboType.DroppedDown = true;
        }


    } // end of class
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SimpleUtils.Win;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public partial class frmClassEditCustomAttributes : frmDialogBase
    {
        ICustomAttributeProvider _cap = null;
        //TypeDescriptionProvider[] _providers;

        public frmClassEditCustomAttributes(ICustomAttributeProvider cap)
        {
            InitializeComponent();

            InitForm(cap);            
        }

        private void InitForm(ICustomAttributeProvider cap)
        {
            _cap = cap;
            LoadCustomAttribute();
        }

        private void LoadCustomAttribute()
        {
            //_providers = new TypeDescriptionProvider[_cap.CustomAttributes.Count];

            cboList.Items.Clear();
            for (int i = 0; i < _cap.CustomAttributes.Count; i++)
            {
                CustomAttribute ca = _cap.CustomAttributes[i];
                if (ca.Constructor != null && ca.Constructor.DeclaringType != null)
                {
                    cboList.Items.Add(ca.Constructor.DeclaringType.FullName);
                }
            }
            if (cboList.Items.Count > 0)
            {
                cboList.SelectedIndex = 0;
            }
            else
            {
                propertyGrid1.SelectedObject = null;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cboList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cboList.SelectedIndex;
            CustomAttribute ca = _cap.CustomAttributes[index];
            //if (_providers[index] == null)
            //{
            //    _providers[index] = TypeDescriptor.AddAttributes(ca, new Attribute[] { new ReadOnlyAttribute(true) });
            //}
            propertyGrid1.SelectedObject = ca;
        }      

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cboList.SelectedIndex >= 0)
            {
                _cap.CustomAttributes.RemoveAt(cboList.SelectedIndex);
                LoadCustomAttribute();
            }
        }

        private void frmClassEditCustomAttributes_FormClosing(object sender, FormClosingEventArgs e)
        {
            //for(int i=0; i<_providers.Length; i++)
            //{
            //    TypeDescriptionProvider provider = _providers[i];
            //    if (provider != null)
            //    {
            //        CustomAttribute ca = _cap.CustomAttributes[i];
            //        TypeDescriptor.RemoveProvider(provider, ca);
            //    }
            //}
        }
      
    }//end of class
}

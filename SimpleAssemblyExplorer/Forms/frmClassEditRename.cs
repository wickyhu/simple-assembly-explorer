using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SimpleUtils.Win;
using Mono.Cecil;
using Mono.Cecil.Metadata;

namespace SimpleAssemblyExplorer
{
    public partial class frmClassEditRename : frmDialogBase
    {
        public frmClassEditRename(object o)
        {
            InitializeComponent();

            InitForm(o);
        }

        private void InitForm(object o)
        {
            if (o is MemberReference)
            {
                MemberReference mr = (MemberReference)o;
                txtOriginalName.Text = InsUtils.GetOldMemberName(mr);
                txtCurrentName.Text = mr.Name;
            }
            else if (o is Resource)
            {
                Resource r = (Resource)o;
                txtOriginalName.Text = String.Empty;
                txtCurrentName.Text = r.Name;
            }
            else
            {
                txtOriginalName.Text = String.Empty;
                txtCurrentName.Text = o.ToString();
            }

            if(!String.IsNullOrEmpty(txtOriginalName.Text))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(txtOriginalName.Text);
                txtOriginalNameHex.Text = BytesUtils.BytesToHexString(bytes, true);
            }

            if (!String.IsNullOrEmpty(txtCurrentName.Text))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(txtCurrentName.Text);
                txtCurrentNameHex.Text = BytesUtils.BytesToHexString(bytes, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNewName.Text))
            {
                SimpleMessage.ShowInfo("Please input New Name.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public string NewName
        {
            get
            {
                return txtNewName.Text;
            }
        }

        private void txtNewName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        
    }
}

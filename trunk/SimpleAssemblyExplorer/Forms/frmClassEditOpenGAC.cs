using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Fusion;

namespace SimpleAssemblyExplorer
{
    public partial class frmClassEditOpenGAC : frmDialogBase
    {
        public frmClassEditOpenGAC()
        {
            InitializeComponent();

            InitForm();
        }

        const int ListViewGroupFiltered = 0;
        const int ListViewGroupOthers = 1;

        private void InitForm()
        {
            using (new SimpleUtils.SimpleWaitCursor())
            {
                List<GacInterop.AssemblyListEntry> list = GacInterop.GetAssemblyList();

                ListViewItem lvi;
                ListViewItem.ListViewSubItem lvsi;

                foreach (GacInterop.AssemblyListEntry le in list)
                {
                    lvi = new ListViewItem();
                    lvi.Text = le.Name;
                    lvi.Tag = le.FullName;
                    lvi.ToolTipText = le.FullName;
                    lvi.Group = listView1.Groups[ListViewGroupFiltered];

                    lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = le.Version;
                    lvi.SubItems.Add(lvsi);

                    lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = le.PublicKeyToken;
                    lvi.SubItems.Add(lvsi);

                    listView1.Items.Add(lvi);
                }
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

        string[] _selectedAssemblies;
        public string[] SelectedAssemblies
        {
            get
            {
                if (_selectedAssemblies == null)
                {
                    _selectedAssemblies = new string[listView1.SelectedItems.Count];
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                    {
                        ListViewItem lvi = listView1.SelectedItems[i];
                        _selectedAssemblies[i] = lvi.Tag as string;
                    }
                }
                return _selectedAssemblies;
            }
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string filter = txtFilter.Text.Trim();

                ListViewGroup lvgOthers = listView1.Groups[ListViewGroupOthers];
                ListViewGroup lvgFiltered = listView1.Groups[ListViewGroupFiltered];

                if (String.IsNullOrEmpty(filter))
                {
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.Group.Name == lvgOthers.Name)
                        {
                            lvi.Group = lvgFiltered;
                        }
                    }
                }
                else
                {
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.Text.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                            lvi.Group = lvgFiltered;
                        else
                            lvi.Group = lvgOthers;
                    }
                }

                if (lvgFiltered.Items.Count > 0)
                {
                    lvgFiltered.Items[0].EnsureVisible();
                }
                else if (lvgOthers.Items.Count > 0)
                {
                    lvgOthers.Items[0].EnsureVisible();
                }
            }            
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                btnOK_Click(sender, e);
            }
        }
    }
}

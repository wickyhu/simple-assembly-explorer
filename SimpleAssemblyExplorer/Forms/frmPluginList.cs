using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class frmPluginList : frmBase
    {
        private DataTable _dtPluginList = null;

        public frmPluginList()
        {
            InitializeComponent();

            InitForm();
        }

        private void CreateDataTable()
        {
            _dtPluginList = new DataTable("PluginList");
            _dtPluginList.Columns.Add("Title", typeof(string));
            _dtPluginList.Columns.Add("PluginType", typeof(String));
            _dtPluginList.Columns.Add("Version", typeof(String));
            _dtPluginList.Columns.Add("Author", typeof(String));
            _dtPluginList.Columns.Add("Contact", typeof(String));
            _dtPluginList.Columns.Add("Url", typeof(String));           
        }

        private void InsertRow(PluginData pd)
        {
            PluginInfoBase pib = pd.PluginBase.PluginInfoBase;
            DataRow dr = _dtPluginList.NewRow();
            dr["Title"] = pib.Title;
            dr["PluginType"] = pd.PluginType.ToString();
            dr["Version"] = pd.Version;
            dr["Author"] = pib.Author;
            dr["Contact"] = pib.Contact;
            dr["Url"] = pib.Url;
            _dtPluginList.Rows.Add(dr);
        }

        public void InitForm()
        {
            CreateDataTable();

            foreach (PluginData pd in PluginUtils.Plugins.Values)
            {
                InsertRow(pd);
            }

            dgvData.AutoGenerateColumns = false;
            dgvData.AutoSize = true;
            dgvData.DataSource = _dtPluginList;
        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) 
                return;
            try
            {
                switch (dgvData.Columns[e.ColumnIndex].Name)
                {
                    case "dgcContact":
                        string email = dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                        if (!String.IsNullOrEmpty(email))
                        {
                            SimpleProcess.Start(String.Format("mailto:{0}", email));
                        }
                        break;

                    default:
                        string url = dgvData.Rows[e.RowIndex].Cells["dgcUrl"].Value as string;
                        if (!String.IsNullOrEmpty(url))
                        {
                            SimpleProcess.Start(url);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
        }
      
    } // end of class
}
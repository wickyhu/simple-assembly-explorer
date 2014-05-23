using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ILMerging;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;

namespace SAE.ILMerge
{
    public partial class frmILMerge : Form
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;

        public string OutputDir
        {
            get { return _host.GetPropertyValue(Plugin.PropertyOutputDir) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyOutputDir, value); }
        }

        public string StrongKeyFile
        {
            get { return _host.GetPropertyValue(Plugin.PropertyStrongKeyFile) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyStrongKeyFile, value); }
        }

        public frmILMerge(IHost host, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = host;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            if (!String.IsNullOrEmpty(OutputDir))
            {
                txtOutputDir.Text = OutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }
            if (!String.IsNullOrEmpty(StrongKeyFile))
            {
                txtKeyFile.Text = StrongKeyFile;
            }

            lbAssemblies.Items.Clear();
            int selected = -1;
            for (int i = 0; i < _rows.Length; i++)
            {
                string file = _rows[i];
                if (!String.IsNullOrEmpty(file))
                {
                    int index = lbAssemblies.Items.Add(file);
                    if (Path.GetExtension(file).ToLower() == ".exe")
                    {
                        lbAssemblies.SelectedIndex = index;
                        selected = index;
                    }
                }
            }
            if (selected < 0)
            {
                lbAssemblies.SelectedIndex = 0;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string keyFile = txtKeyFile.Text;
            if (!String.IsNullOrEmpty(keyFile) && !File.Exists(keyFile))
            {
                SimpleMessage.ShowInfo("Invalid key file");
                return;
            }
            string outputDir = txtOutputDir.Text;
            if (!Directory.Exists(outputDir))
            {
                SimpleMessage.ShowInfo("Please choose output directory");
                return;
            }

            if (_sourceDir != null && _sourceDir.Equals(outputDir))
                OutputDir = String.Empty;
            else 
                OutputDir = outputDir;

            StrongKeyFile = keyFile;

            txtInfo.Text = String.Format("=== Started at {0} ===\r\n\r\n", DateTime.Now);
            Application.DoEvents();
            
            ILMerge();

            SimpleTextbox.AppendText(txtInfo, String.Format("=== Completed at {0} ===\r\n\r\n", DateTime.Now));
            //this.Close();
        }

        private void ILMerge()
        {
            try
            {
                btnOK.Enabled = false;

                ILMerging.ILMerge ilm = new ILMerging.ILMerge();
                //ilm.Closed = true;

                if (!String.IsNullOrEmpty(txtKeyFile.Text))
                    ilm.KeyFile = txtKeyFile.Text;

                string[] assemblies = new string[lbAssemblies.Items.Count];
                for (int i = 0, j = 1; i < lbAssemblies.Items.Count; i++)
                {
                    if (i == lbAssemblies.SelectedIndex)
                    {
                        assemblies[0] = Path.Combine(_sourceDir, lbAssemblies.Items[i].ToString());
                    }
                    else
                    {
                        assemblies[j++] = Path.Combine(_sourceDir, lbAssemblies.Items[i].ToString());
                    }
                }
                ilm.SetInputAssemblies(assemblies);

                string outputFile = Path.GetFileName(assemblies[0]);
                outputFile = Path.Combine(txtOutputDir.Text, outputFile);
                if (File.Exists(outputFile))
                {
                    string ext = Path.GetExtension(outputFile);
                    outputFile = Path.ChangeExtension(outputFile, ".ILMerge" + ext);
                }
                ilm.OutputFile = outputFile;

                ilm.Closed = chkClosed.Checked;
                ilm.DebugInfo = chkDebug.Checked;
                if (chkDuplicate.Checked) ilm.AllowDuplicateType(null);
                
                //ilm.Log = true;
                ilm.Merge();
                ilm = null;

                SimpleTextbox.AppendText(txtInfo, String.Format("Output File: {0}\r\n\r\n", outputFile));

            }
            catch (Exception ex)
            {
                SimpleTextbox.AppendText(txtInfo, String.Format("{0}\r\n\r\n", ex.Message));
            }
            finally
            {
                btnOK.Enabled = true;
            }

            Application.DoEvents();
        }

        private void btnSelectOutputDir_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFolder();
            if (!String.IsNullOrEmpty(path))
            {
                txtOutputDir.Text = path;
                OutputDir = path;
            }
        }

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            string path = SimpleDialog.OpenFile(null, "Key Files (*.snk)|*.snk|All Files (*.*)|*.*", ".snk", true, StrongKeyFile);

            if (!String.IsNullOrEmpty(path))
            {
                txtKeyFile.Text = path;
                StrongKeyFile = path;
            }    
        }

        private void txtOutputDir_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtOutputDir.Text))
            {
                txtOutputDir.Text = _sourceDir;
            }
        }
      
    } // end of class
}
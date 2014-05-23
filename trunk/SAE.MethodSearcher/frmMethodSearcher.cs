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
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using SimpleUtils;
using SimpleAssemblyExplorer;

namespace SAE.MethodSearcher
{
    public partial class frmMethodSearcher : Form
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;
        private SimpleReflector _reflector;

        public string LastSearchFor
        {
            get { return _host.GetPropertyValue(Plugin.PropertyLastSearchFor) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyLastSearchFor, value); }
        }

        public string LogTo
        {
            get { return _host.GetPropertyValue(Plugin.PropertyLogTo) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyLogTo, value); }
        }

        public frmMethodSearcher(IHost host, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = host;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            cboSearchFor.Items.Add("// This item is obfuscated and can not be translated.");
            cboSearchFor.Items.Add("// This item appears to be generated and can not be translated.");
            cboSearchFor.Items.Add("using (enumerator =");
            if (!String.IsNullOrEmpty(this.LastSearchFor))
            {
                cboSearchFor.Text = this.LastSearchFor;
            }
            else
            {
                cboSearchFor.SelectedIndex = 0;
            }
            switch (this.LogTo)
            {
                case "Screen":
                    rbToScreen.Checked = true;
                    break;
                case "File":
                    rbToFile.Checked = true;
                    break;
                default:
                    break;
            }

            _reflector = SimpleReflector.Default;
            //_reflector.FormatterType = typeof(TextFormatter);
            _reflector.FormatterTypeName = "SimpleAssemblyExplorer.LutzReflector.TextFormatter";

            foreach (string l in _reflector.Languages)
            {
                int index = cboLanguage.Items.Add(l);
                if (l == "C#")
                {
                    cboLanguage.SelectedIndex = index;
                }
            }
            //foreach (string op in SimpleReflector.OptimizationList)
            //{
            //    int index = cboOptimization.Items.Add(op);
            //    if (op == "2.0")
            //    {
            //        cboOptimization.SelectedIndex = index;
            //    }
            //}
        }

        const string btnOkText = "OK";
        const string btnStopText = "Stop";
        private bool _isCancelPending;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isCancelPending = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            switch (btnOK.Text)
            {
                case btnStopText:
                    _isCancelPending = true;
                    return;
                default:
                    break;
            }

            if (String.IsNullOrEmpty(cboSearchFor.Text.Trim()))
            {
                SimpleMessage.ShowInfo("Please enter Search For.");
                return;
            }

            this.LastSearchFor = cboSearchFor.Text;
            if (rbToScreen.Checked)
            {
                this.LogTo = "Screen";
            }
            else if (rbToFile.Checked)
            {
                this.LogTo = "File";
            }

            _reflector.Language = cboLanguage.SelectedItem as string;

            txtInfo.Text = String.Format("=== Started at {0} ===\r\n\r\n", DateTime.Now);
            Application.DoEvents();

            DoSearch();

            SimpleTextbox.AppendText(txtInfo, String.Format("\r\n=== Completed at {0} ===\r\n\r\n", DateTime.Now));
            //this.Close();
        }
        
        private void DoSearch()
        {

            try
            {
                _isCancelPending = false;
                btnOK.Text = btnStopText;

                Utils.EnableUI(this.Controls, false);

                for (int i = 0; i < _rows.Length; i++)
                {
                    if (_isCancelPending) break;

                    string fileName = _rows[i];
                    
                    _host.SetStatusText(String.Format("Seaching {0} ...", fileName));
                    SimpleTextbox.AppendText(txtInfo, String.Format("File {0}: {1}\r\n", i + 1, fileName));

                    DoSearch(fileName);
                    Application.DoEvents();
                }

                if (_isCancelPending)
                    SimpleTextbox.AppendText(txtInfo, "User breaked.\r\n");
            }
            catch (Exception ex)
            {
                SimpleTextbox.AppendText(txtInfo, String.Format("{0}\r\n\r\n", ex.Message));
            }
            finally
            {
                _host.ResetProgress();
                _host.SetStatusText(null);
                btnOK.Text = btnOkText;

                Utils.EnableUI(this.Controls, true);
            }
        }

        private string GetLogFileName(string fileName)
        {
            return Path.ChangeExtension(fileName, ".log");
        }

        private bool IsCancelPending()
        {
            return _isCancelPending;
        }

        private void DoSearch(string fileName)
        {
            List<MethodDeclarationInfo> list = _reflector.FindMethods(new string[] { fileName },
                new string[] { cboSearchFor.Text.Trim() },
                null,
                _host,
                new SimpleReflector.IsCancelPendingDelegate(IsCancelPending));

            SimpleTextbox.AppendText(txtInfo, String.Format("Total {0} errors found.\r\n\r\n", list.Count));

            if (rbToScreen.Checked)
            {
                foreach (MethodDeclarationInfo info in list)
                {
                    SimpleTextbox.AppendText(txtInfo, info.Name);
                    SimpleTextbox.AppendText(txtInfo, "\r\n");
                    SimpleTextbox.AppendText(txtInfo, info.Body);
                    SimpleTextbox.AppendText(txtInfo, "\r\n\r\n\r\n");
                    Application.DoEvents();
                }
            }

            if (rbToFile.Checked)
            {
                string outputFile = GetLogFileName(fileName);
                using (StreamWriter sw = File.CreateText(outputFile))
                {
                    sw.WriteLine(String.Format("File {0}", fileName));
                    sw.WriteLine(String.Format("Total {0} errors found.\r\n\r\n", list.Count));

                    foreach (MethodDeclarationInfo info in list)
                    {
                        sw.WriteLine(info.Name);
                        sw.WriteLine(info.Body);
                        sw.WriteLine();
                        sw.WriteLine();
                        Application.DoEvents();
                    }                    
                }
            }

        }

        private void lblLog_Click(object sender, EventArgs e)
        {
            if (_rows.Length == 1)
            {
                string outputFile = GetLogFileName(_rows[0]);
                if (File.Exists(outputFile))
                    SimpleProcess.Start(outputFile);
            }
            else if (_rows.Length > 1)
            {
                string outputDir = _sourceDir;
                if(Directory.Exists(outputDir))
                    SimpleProcess.Start(outputDir);
            }
        }

    } // end of class
}
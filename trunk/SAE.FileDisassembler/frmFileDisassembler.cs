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
using SimpleUtils;
using SimpleAssemblyExplorer.LutzReflector;

namespace SAE.FileDisassembler
{
    public partial class frmFileDisassembler : Form
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;
        private SimpleReflector _reflector;

        public string OutputDir
        {
            get { return _host.GetPropertyValue(Plugin.PropertyOutputDir) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyOutputDir, value); }
        }
     

        public frmFileDisassembler(IHost host, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = host;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            cboLanguage.SelectedIndex = 0;

            foreach (string op in SimpleReflector.OptimizationList)
            {
                int index = cboOptimization.Items.Add(op);
                if (op == "2.0")
                {
                    cboOptimization.SelectedIndex = index;
                }
            }

            if (!String.IsNullOrEmpty(OutputDir))
            {
                txtOutputDir.Text = OutputDir;
            }
            else
            {
                txtOutputDir.Text = _sourceDir;
            }           

            lbAssemblies.Items.Clear();            
            for (int i = 0; i < _rows.Length; i++)
            {
                string file = _rows[i];
                if (!String.IsNullOrEmpty(file))
                {
                    int index = lbAssemblies.Items.Add(file);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isCancelPending = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            const string btnOkText = "OK";
            const string btnStopText = "Stop";

            switch (btnOK.Text)
            {
                case btnStopText:
                    _isCancelPending = true;
                    return;
                default:
                    break;
            }

            string outputDir = txtOutputDir.Text;
            //if (!Directory.Exists(outputDir))
            //{
            //    SimpleMessage.ShowInfo("Please choose output directory");
            //    return;
            //}

            if (_sourceDir != null && _sourceDir.Equals(outputDir))
                OutputDir = String.Empty;
            else 
                OutputDir = outputDir;

            try
            {
                btnOK.Text = btnStopText;
                SimpleAssemblyExplorer.Utils.EnableUI(this.Controls, false);
                Application.DoEvents();

                if (_reflector == null)
                {
                    //TODO: better to new instance, but seems resource can't be released
                    _reflector = SimpleReflector.Default;
                }
                _reflector.Optimization = cboOptimization.Text;

                txtInfo.Text = String.Empty;
                WriteLine(String.Format("=== Started at {0} ===\r\n", DateTime.Now));
                Application.DoEvents();

                _isCancelPending = false;
                _host.ResetProgress();

                _reflector.LanguageManager.ActiveLanguage = _reflector.GetLanguage(cboLanguage.SelectedItem as string);

                for (int i = 0; i < lbAssemblies.Items.Count; i++)
                {
                    string file = lbAssemblies.Items[i] as string;
                    _reflector.LoadAssembly(file);
                }

                for (int i = 0; i < lbAssemblies.Items.Count; i++)
                {
                    if (_isCancelPending) break;

                    string file = lbAssemblies.Items[i] as string;
                    _host.SetStatusText(String.Format("Handling {0} ...", file));
                    DisassembleFile(file);
                    Application.DoEvents();
                }

                for (int i = 0; i < lbAssemblies.Items.Count; i++)
                {
                    string file = lbAssemblies.Items[i] as string;
                    _reflector.UnloadAssembly(file);
                }

                if (_isCancelPending)
                    WriteLine("User breaked.\r\n");

            }
            catch (Exception ex)
            {
                WriteLine(String.Format("{0}\r\n\r\n", ex.Message));
            }
            finally
            {
                btnOK.Text = btnOkText;

                SimpleAssemblyExplorer.Utils.EnableUI(this.Controls, true);
                
                _host.SetStatusText(null);
                _host.ResetProgress();

                if (_reflector != null && _reflector.LanguageManager != null)
                {
                    _reflector.LanguageManager.ActiveLanguage = _reflector.GetLanguage("C#");
                }
            }

            WriteLine(String.Format("=== Completed at {0} ===\r\n", DateTime.Now));
            //this.Close();
        }

        private void DisassembleFile(string fileName)
        {
            WriteLine(String.Format("{0}: ", fileName));

            string outputDir = Path.Combine(txtOutputDir.Text, Path.GetFileName(fileName)) + "_Source";
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Reflector.CodeModel.IAssembly assembly = null;
            bool unloadAssembly = false;
            try
            {
                assembly = _reflector.FindAssembly(fileName);
                if (assembly == null)
                {
                    assembly = _reflector.LoadAssembly(fileName);
                    unloadAssembly = true;
                }
                _host.SetProgress(0);

                FileDisassemblerHelper helper = new FileDisassemblerHelper(
                    _reflector,
                    fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? ProjectTypes.Library : ProjectTypes.WinExe,
                    outputDir,
                    new FileDisassemblerHelper.WriteLineDelegate(WriteLine),
                    new FileDisassemblerHelper.SetProgressBarDelegate(SetProgressBar),
                    new FileDisassemblerHelper.IsCancelPendingDelegate(IsCancelPending));

                int exceptions = helper.GenerateCode(assembly);

                WriteLine(string.Format("Done. {0} error(s).\r\n", exceptions));

            }
            catch
            {
                throw;
            }
            finally
            {
                if (assembly != null && unloadAssembly)
                {
                    _reflector.AssemblyManager.Unload(assembly);
                }
                _host.SetProgress(100);
            }
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

        private bool _isCancelPending;
        private bool IsCancelPending()
        {
            return _isCancelPending;
        }

        private void SetProgressBar(int pos)
        {
            _host.SetProgress(pos);
        }

        private void WriteLine(string text)
        {
            SimpleTextbox.AppendText(txtInfo, String.Format("{0}\r\n", text));
        }

        private void lblOutputDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtOutputDir.Text))
            {
                Process.Start(txtOutputDir.Text);
            }
        }

        private void frmFileDisassembler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_reflector != null)
            {
                _reflector = null;
            }
        }

    } // end of class
}
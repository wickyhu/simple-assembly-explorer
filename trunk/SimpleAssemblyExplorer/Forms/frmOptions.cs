using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Principal;

namespace SimpleAssemblyExplorer
{
    public partial class frmOptions : frmBase
    {
        frmMain _mainForm;

        public frmOptions(frmMain mainForm)
        {
            InitializeComponent();

            InitForm(mainForm);
        }

        public void InitForm(frmMain mainForm)
        {
            _mainForm = mainForm;

            int period = (int)(Config.CheckUpdatePeriod / 7) - 1;
            if (period < 0 || period > 3) period = 1;
            
            cboCheckUpdatePeriod.SelectedIndex = period;            
            chkCheckUpdateEnabled.Checked = Config.CheckUpdateEnabled;           

            nudRecentPluginList.Value = Config.RecentPluginList;

            chkAutoSaveBookmarkEnabled.Checked = Config.ClassEditorAutoSaveBookmarkEnabled;
            chkAutoOpenDroppedAssembly.Checked = Config.ClassEditorAutoOpenDroppedAssemblyEnabled;
            lblRtbFont.Font = Config.ClassEditorRichTextBoxFont;

            if (ShellExtUtils.IsSAERegistered())
                chkIntegrateWithExplorer.Checked = true;
            else
                chkIntegrateWithExplorer.Checked = false;

            cboBamlTranslator.SelectedIndex = Config.ClassEditorBamlTranslator;

            chkIntegrateWithExplorer.Enabled = ShellExtUtils.IsAdministrator();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Config.CheckUpdateEnabled = chkCheckUpdateEnabled.Checked;
            Config.CheckUpdatePeriod = (cboCheckUpdatePeriod.SelectedIndex + 1) * 7;
            
            Config.ClassEditorAutoSaveBookmarkEnabled = chkAutoSaveBookmarkEnabled.Checked;
            Config.ClassEditorAutoOpenDroppedAssemblyEnabled = chkAutoOpenDroppedAssembly.Checked;
            Config.ClassEditorRichTextBoxFont = lblRtbFont.Font;
            Config.ClassEditorBamlTranslator = cboBamlTranslator.SelectedIndex;

            int recentPluginList = (int)nudRecentPluginList.Value;
            if (recentPluginList != Config.RecentPluginList)
            {
                _mainForm.PluginHandler.ClearPlugins();
                Config.RecentPluginList = recentPluginList;
                _mainForm.PluginHandler.LoadPlugins();
            }

            if (chkIntegrateWithExplorer.Enabled)
            {
                if (chkIntegrateWithExplorer.Checked)
                    ShellExtUtils.RegisterSAE();
                else
                    ShellExtUtils.UnregisterSAE();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkCheckUpdateEnabled_CheckedChanged(object sender, EventArgs e)
        {
            cboCheckUpdatePeriod.Enabled = chkCheckUpdateEnabled.Checked;
        }

		private void lblRtbFontSelect_Click(object sender, EventArgs e)
		{
            fontDialog1.Font = lblRtbFont.Font;
			if (fontDialog1.ShowDialog() == DialogResult.OK)
			{
                lblRtbFont.Font = fontDialog1.Font;
			}
		}

        private void lblRtbFontReset_Click(object sender, EventArgs e)
        {
            lblRtbFont.Font = Config.DefaultFont;
        }

    } // end of class
}
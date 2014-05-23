using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SimpleUtils.Win
{
    public partial class frmSimpleError : frmSimpleDialogBase
    {
        private string _reportUrl;

        public frmSimpleError(Exception ex, string url)
        {
            InitializeComponent();

            StringBuilder sb = new StringBuilder();
            Exception innerEx = ex;
            while (innerEx != null)
            {
                AppendError(sb, innerEx);
                innerEx = innerEx.InnerException;
            }

            txtInfo.Text = sb.ToString();
            
            _reportUrl = url;
            if (String.IsNullOrEmpty(_reportUrl))
            {
                btnEmail.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetSubject()
        {
            string fullName = Assembly.GetEntryAssembly().FullName;
            string name = fullName.Contains(",") ? fullName.Split(',')[0] : fullName;
            string subject = String.Format("{0} Error Report", name);
            return subject;
        }

        private void AppendError(StringBuilder sb, Exception ex)
        {
            sb.AppendFormat("{0}\r\n{1}\r\n\r\n", ex.Message, ex.StackTrace);
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            SimpleDotNet.SendEmail(GetSubject(), txtInfo.Text);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_reportUrl))
            {
                SimpleDotNet.SendEmail(GetSubject(), txtInfo.Text);
            }
            else
            {
                SimpleProcess.Start(_reportUrl);
            }            
        }
      

    } // end of class
}
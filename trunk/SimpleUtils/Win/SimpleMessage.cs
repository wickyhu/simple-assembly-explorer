using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SimpleUtils.Win
{
    public class SimpleMessage
    {
        public static void ShowAbout(string description)
        {
            frmSimpleAbout f = new frmSimpleAbout(description);
            f.ShowDialog();
        }

        public static void ShowException(Exception ex)
        {
            frmSimpleError f = new frmSimpleError(ex, SimpleDotNet.ReportUrl);
            f.ShowDialog();
        }

        public static void ShowException(Exception ex, string url)
        {
            frmSimpleError f = new frmSimpleError(ex, url);
            f.ShowDialog();
        }

        public static void ShowInfo(string info)
        {
            ShowInfo(info, "Information");
        }

        public static void ShowInfo(string info, string caption)
        {
            MessageBox.Show(info, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string error)
        {
            ShowError(error, "Error");
        }

        public static void ShowError(string error, string caption)
        {
            MessageBox.Show(error, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult Confirm(string question)
        {
            return MessageBox.Show(question, "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
        }

    }//end of class
}

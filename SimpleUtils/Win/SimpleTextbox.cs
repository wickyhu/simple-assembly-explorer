using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleUtils.Win
{
    public class SimpleTextbox
    {
        #region TextBox
        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam,
        IntPtr lParam);

        public static void ScrollToBottom(TextBoxBase tb)
        {
            const int WM_VSCROLL = 277;
            const int SB_BOTTOM = 7;

            IntPtr ptrWparam = new IntPtr(SB_BOTTOM);
            IntPtr ptrLparam = new IntPtr(0);
            SendMessage(tb.Handle, WM_VSCROLL, ptrWparam, ptrLparam);

            //another solution
            //tb.SelectionStart = tb.TextLength;
            //tb.ScrollToCaret();
        }

        public static void ScrollToTop(TextBoxBase tb)
        {
            tb.SelectionStart = 0;
            tb.ScrollToCaret();
        }

        public delegate void AppendTextDelegate(TextBoxBase tb, string text);
        public static void AppendText(TextBoxBase tb, string text)
        {
            if (tb == null) return;
            tb.SuspendLayout();
            if (tb.InvokeRequired)
            {
                tb.Invoke(new AppendTextDelegate(AppendText), new object[] { tb, text });
            }
            else
            {
                tb.SelectionStart = tb.TextLength;
                tb.SelectedText = text;
            }
            tb.ResumeLayout();
        }
        #endregion TextBox
    }
}

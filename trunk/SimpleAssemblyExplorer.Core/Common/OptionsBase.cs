using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public abstract class OptionsBase : ITextInfo
    {
        public IHost Host { get; set; }
        public string[] Rows { get; set; }
        public string SourceDir { get; set; }
        public string OutputDir { get; set; }
        public TextBox TextInfoBox { get; set; }
        public string AdditionalOptions { get; set; }

        public string[] OutputFiles { get; set; }

        public bool IgnoreRows { get; set; }
        public bool ShowStartCompleteTextInfo { get; set; }
        public bool ShowFileNoTextInfo { get; set; }

        public OptionsBase()
        {
            InitDefaults();
        }

        public virtual void Clear()
        {
            this.InitDefaults();
        }

        public virtual void InitDefaults()
        {
            AdditionalOptions = String.Empty;
            TextInfoBox = null;

            IgnoreRows = false;
            ShowStartCompleteTextInfo = true;
            ShowFileNoTextInfo = true;
        }

        public virtual string TextInfo
        {
            get
            {
                if (this.TextInfoBox == null)
                    return String.Empty;
                return this.TextInfoBox.Text;
            }
        }

        public virtual void SetTextInfo(string text)
        {
            if (TextInfoBox == null) return;
            TextInfoBox.Text = text;
        }

        public virtual void AppendTextInfo(string text)
        {
            if (TextInfoBox == null) return;
            SimpleTextbox.AppendText(TextInfoBox, text);
        }

        public virtual void AppendTextInfoLine(string text)
        {
            AppendTextInfoLine(text, false);
        }

        public virtual void AppendTextInfoLine(string text, bool noDuplicate)
        {
            if (TextInfoBox == null) return;
            if (noDuplicate)
            {
                if (TextInfoBox.Text.Contains(text))
                    return;
            }
            SimpleTextbox.AppendText(TextInfoBox, text);
            SimpleTextbox.AppendText(TextInfoBox, "\r\n");
        }

        public void ScrollTextInfoToTop()
        {
            if (TextInfoBox == null) return;
            SimpleTextbox.ScrollToTop(TextInfoBox);
        }

        public void ScrollTextInfoToBottom()
        {
            if (TextInfoBox == null) return;
            SimpleTextbox.ScrollToBottom(TextInfoBox);
        }

    }   
}
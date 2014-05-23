using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssemblyExplorer
{
    public interface ITextInfo
    {
        string TextInfo { get; }
        void SetTextInfo(string text);
        void AppendTextInfo(string text);
        void AppendTextInfoLine(string text);
    }   
}
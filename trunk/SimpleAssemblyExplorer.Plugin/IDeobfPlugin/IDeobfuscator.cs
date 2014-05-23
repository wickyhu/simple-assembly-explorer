using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IDeobfuscator
    {
        bool IsCancelPending { get; }
        void AppendTextInfoLine(string text);

        //bool IsValidName(string name);

    }//end of class
}

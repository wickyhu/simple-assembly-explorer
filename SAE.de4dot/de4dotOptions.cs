using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SimpleAssemblyExplorer;

namespace SAE.de4dot
{
    public class de4dotOptions : OptionsBase
    {
        public enum Actions
        {
            Deobfuscate,
            Detect
        }

        public enum StringDecrypterTypes
        {
            None,             //Don't decrypt strings
            Default,          //Use default string decrypter type (usually static)
            Static,           //Use static string decrypter if available
            Delegate,         //Use a delegate to call the real string decrypter
            Emulate          //Call real string decrypter and emulate certain instructions
        }

        public string ExeFile { get; set; }
        public Actions Action { get; set; }
        public bool Verbose { get; set; }
        public bool CreateOutputDir { get; set; }
        public bool ScanDir { get; set; }

        public StringDecrypterTypes StringDecrypterType { get; set; }
        public string StringDecrypterMethod { get; set; }

        public bool IgnoreUnsupported { get; set; }
        public bool PreserveTokens { get; set; }
        public bool DontRename { get; set; }
        public bool KeepTypes { get; set; }

        public de4dotOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            
            this.ExeFile = Plugin.Name + ".exe";
            this.Action = Actions.Deobfuscate;
            this.Verbose = false;
            this.CreateOutputDir = true;
            this.ScanDir = false;

            this.StringDecrypterType = StringDecrypterTypes.Default;
            this.StringDecrypterMethod = null;

            this.IgnoreUnsupported = true;
            this.PreserveTokens = false;
            this.DontRename = false;
            this.KeepTypes = false;
        }

    }
}
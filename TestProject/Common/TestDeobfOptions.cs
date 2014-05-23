using System;
using System.Collections.Generic;
using System.Text;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestDeobfOptions : DeobfOptions
    {

        public TestDeobfOptions()
            : base()
        {            
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            Global.SetOptions(this);

            _text = String.Empty;

            //Testing
            //this.BranchDirection = BranchDirections.WalkThrough;
        }

        string _text;

        public override string TextInfo
        {
            get { return _text; }
        }

        public override void SetTextInfo(string text)
        {
            _text = (text == null ? String.Empty : text);
            Utils.DebugOutput(text);
        }

        public override void AppendTextInfo(string text)
        {
            AppendTextInfoLine(text);
        }

        public override void AppendTextInfoLine(string text)
        {
            if (TextInfo.Contains(text))
                return;

            _text = String.Format("{0}\r\n{1}", _text, text);
            Utils.DebugOutput(text);
        }

    }   
}
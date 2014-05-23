using System;
using System.Collections.Generic;
using System.Text;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestPEVerifyOptions : PEVerifyOptions
    {

        public TestPEVerifyOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            Global.SetOptions(this);
        }

    }   
}
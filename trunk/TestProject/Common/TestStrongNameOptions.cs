using System;
using System.Collections.Generic;
using System.Text;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestStrongNameOptions : StrongNameOptions
    {

        public TestStrongNameOptions()
            : base()
        {
        }

        public override void InitDefaults()
        {
            base.InitDefaults();
            Global.SetOptions(this);
            this.KeyFile = Global.TestKeyFile;
        }

    }   
}
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestDeobfuscator : Deobfuscator
    {
        public TestDeobfuscator()
            : this(new TestDeobfOptions())
        {
        }

        public TestDeobfuscator(TestDeobfOptions options)
            : base(options)
        {
        }

    }
}

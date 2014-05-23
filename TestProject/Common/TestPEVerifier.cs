using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestPEVerifier : PEVerifier
    {
        public TestPEVerifier()
            : this(new TestPEVerifyOptions())
        {
        }

        public TestPEVerifier(TestPEVerifyOptions options)
            : base(options)
        {
        }

    }
}

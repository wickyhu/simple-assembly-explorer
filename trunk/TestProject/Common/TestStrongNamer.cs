using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleAssemblyExplorer;

namespace TestProject
{
    public class TestStrongNamer : StrongNamer
    {
        public TestStrongNamer()
            : this(new TestStrongNameOptions())
        {
        }

        public TestStrongNamer(TestStrongNameOptions options)
            : base(options)
        {
        }

    }
}

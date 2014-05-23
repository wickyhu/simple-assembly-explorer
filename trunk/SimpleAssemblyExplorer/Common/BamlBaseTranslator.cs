using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;

namespace SimpleAssemblyExplorer
{
    public abstract class BamlBaseTranslator
    {
        public AssemblyDefinition Assembly { get; private set; }
        public Stream Data { get; private set; }

        public BamlBaseTranslator(AssemblyDefinition ad, Stream data)
        {
            this.Assembly = ad;
            this.Data = data;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}

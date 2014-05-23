using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Mono.Cecil;
using System.IO;
using ILSpy.BamlDecompiler;
using Ricciolo.StylesExplorer.MarkupReflection;

namespace SimpleAssemblyExplorer
{
    public class BamlILSpyTranslator : BamlBaseTranslator
    {
        public BamlILSpyTranslator(AssemblyDefinition ad, Stream stream)
            : base(ad, stream)
        {
        }

        public override string ToString()
        {
            return new BamlResourceEntryNode(this.Assembly, this.Data).ToString();
        }
    }
}

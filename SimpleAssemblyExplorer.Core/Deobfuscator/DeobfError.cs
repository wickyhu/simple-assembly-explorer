using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace SimpleAssemblyExplorer
{
    public class DeobfError
    {
        public List<string> NotHandledTypes { get; private set; }
        public string ModuleName { get; private set; }
        public string AssemblyName { get; private set; }

        public DeobfError(ModuleDefinition module, List<string> notHandledTypes)
        {
            this.ModuleName = module.FullyQualifiedName;
            this.AssemblyName = module.Assembly.FullName;
            this.NotHandledTypes = notHandledTypes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();            
            sb.AppendFormat("{0}\r\n{1}\r\n{2} type(s) not handled:\r\n",
                this.AssemblyName, this.ModuleName, this.NotHandledTypes.Count);
            foreach (string typeStr in this.NotHandledTypes)
            {
                sb.Append(typeStr);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    } //end of class
}
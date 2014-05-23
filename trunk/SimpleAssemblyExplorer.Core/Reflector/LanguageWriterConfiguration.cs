using System;
using System.Collections.Generic;
using System.Text;
using Reflector;
using Reflector.CodeModel;

namespace SimpleAssemblyExplorer.LutzReflector
{
    public class LanguageWriterConfiguration : ILanguageWriterConfiguration
    {
        private IVisibilityConfiguration visibility;
        private Dictionary<string, string> table = new Dictionary<string, string>();

        public IVisibilityConfiguration Visibility
        {
            get
            {
                return this.visibility;
            }

            set
            {
                this.visibility = value;
            }
        }

        public string this[string name]
        {
            get
            {
                if(this.table.ContainsKey(name))
                    return this.table[name];
                //NumberFormat
                //ShowCustomAttributes
                return null;
            }

            set
            {
                this.table[name] = value;
            }
        }
    } //end of LanguageWriterConfiguration
}

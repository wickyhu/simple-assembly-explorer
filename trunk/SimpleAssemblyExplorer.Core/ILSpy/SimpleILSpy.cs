using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy
{
    public class SimpleILSpy
    {
        static Version _version;
        public static Version Version
        {
            get
            {
                if(_version == null) 
                    _version = typeof(DecompilerSettings).Assembly.GetName().Version;
                return _version;
            }
        }

        static string[] _languages = new string[] { "IL", "C#", "Visual Basic" };
        static Type[] _languageTypes = new Type[] { typeof(ILLanguage), typeof(CSharpLanguage), typeof(VB.VBLanguage)};
        public string[] Languages
        {
            get
            {
                return _languages;
            }
        }

        public int GetLanguageIndex(string language)
        {
            for (int i = 0; i < Languages.Length; i++)
            {
                if (Languages[i].Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        public SimpleILSpy()
        {
        }

        public Language CreateLanguage(string language)
        {
            int index = GetLanguageIndex(language);
            if (index < 0) return null;
            return (Language)Activator.CreateInstance(_languageTypes[index]);
        }

        public string Decompile(string language, object o)
        {
            if (o == null) return String.Empty;
            Language l = CreateLanguage(language);
            if (l == null) return String.Format("Can't create language: {0}", language);

            ITextOutput output = new RtfTextOutput();
            DecompilationOptions options = new DecompilationOptions();
            
            if (o is AssemblyDefinition)
                l.DecompileAssembly((AssemblyDefinition)o, output, options);
            else if (o is TypeDefinition)
                l.DecompileType((TypeDefinition)o, output, options);
            else if (o is MethodDefinition)
                l.DecompileMethod((MethodDefinition)o, output, options);
            else if (o is FieldDefinition)
                l.DecompileField((FieldDefinition)o, output, options);
            else if (o is PropertyDefinition)
                l.DecompileProperty((PropertyDefinition)o, output, options);
            else if (o is EventDefinition)
                l.DecompileEvent((EventDefinition)o, output, options);
            else if (o is AssemblyNameReference)
            {
                output.Write("// Assembly Reference ");
                output.WriteDefinition(o.ToString(), null);
                output.WriteLine();
            }
            else if(o is ModuleReference)
            {
                output.Write("// Module Reference ");
                output.WriteDefinition(o.ToString(), null);
                output.WriteLine();
            }
            else
            {
                output.Write(String.Format("// {0} ", o.GetType().Name));
                output.WriteDefinition(o.ToString(), null);
                output.WriteLine();
            }                

            return output.ToString();
        }

    }//end of class
}

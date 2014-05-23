using System;
using System.Collections.Generic;
using System.Text;
using SimpleUtils;
using System.Windows.Forms;
using Mono.Cecil;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public class ClassEditHelper
    {
        public static void InitDropdownTypes(ComboBox cbo, ModuleDefinition module)
        {
            using (new SimpleWaitCursor())
            {
                object savedItem = cbo.SelectedItem;

                cbo.Items.Clear();
                cbo.Sorted = true;

                if (module != null)
                {
                    foreach (TypeDefinition td in module.AllTypes)
                    {
                        if (td.FullName.StartsWith("<")) continue;
                        cbo.Items.Add(td);
                    }
                }

                Type[] systemTypes = typeof(System.Int32).Module.GetTypes();
                foreach (Type systemType in systemTypes)
                {
                    if (systemType.FullName.StartsWith("<")) continue;
                    cbo.Items.Add(systemType);
                }

                if (savedItem != null)
                {
                    if (!cbo.Items.Contains(savedItem))
                    {
                        cbo.Items.Add(savedItem);
                    }
                    cbo.SelectedItem = savedItem;
                }
            }
        }

        public static TypeReference ParseTypeReference(object o, ModuleDefinition module)
        {
            TypeReference tr = null;

            if (o is TypeReference)
            {
                tr = module.Import((TypeReference)o);
            }
            else if (o is Type)
            {
                tr = module.Import((Type)o);
            }
            else if(o is string)
            {
                string typeName = o as string;
                Type type = Type.GetType(typeName, false, true);
                if (type != null)
                {
                    tr = module.Import(type);
                }
            }            

            return tr;
        }
      
        public static void CopyRtbTextToClipboard(RichTextBox rtb)
        {
            Clipboard.Clear();
            string text = rtb.SelectedText;

        }

    }//end of class
}

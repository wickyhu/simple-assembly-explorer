using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Xml;
using System.IO;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public class CustomAttributeUtils
    {
        //static Type caaType = typeof(Mono.Cecil.CustomAttributeArgument);        
        //static Type canaType = typeof(Mono.Cecil.CustomAttributeNamedArgument);        
        
        static Type canaCollectionType = typeof(Collection<Mono.Cecil.CustomAttributeNamedArgument>);
        static FieldInfo canaItems = canaCollectionType.GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);

        static Type caaCollectionType = typeof(Collection<Mono.Cecil.CustomAttributeArgument>);
        static FieldInfo caaItems = caaCollectionType.GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);

        static Mono.Cecil.CustomAttributeArgument[] GetCaaItems(Collection<Mono.Cecil.CustomAttributeArgument> c)
        {
            return (Mono.Cecil.CustomAttributeArgument[])caaItems.GetValue(c);
        }

        static Mono.Cecil.CustomAttributeNamedArgument[] GetCanaItems(Collection<Mono.Cecil.CustomAttributeNamedArgument> c)
        {
            return (Mono.Cecil.CustomAttributeNamedArgument[])canaItems.GetValue(c);
        }

        public static void SetConstructorArgumentValue(Mono.Cecil.CustomAttribute ca, int index, object value)
        {
            Mono.Cecil.CustomAttributeArgument[] items = GetCaaItems(ca.ConstructorArguments);
            items[index].Value = value;
        }

        public static void SetFieldName(Mono.Cecil.CustomAttribute ca, int index,  string name)
        {
            Mono.Cecil.CustomAttributeNamedArgument[] items = GetCanaItems(ca.Fields);
            items[index].Name = name;
        }

        public static void SetPropertyName(Mono.Cecil.CustomAttribute ca, int index, string name)
        {
            Mono.Cecil.CustomAttributeNamedArgument[] items = GetCanaItems(ca.Properties);
            items[index].Name = name;
        }

    }//end of class
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;

namespace SimpleAssemblyExplorer
{
    public class TreeNodeSorter : IComparer
    {
        public enum SortTypes
        {
            Object,
            GenericInstanceType,
            References,
            NameSpace,
            BaseTypes,
            TypeDefinition,
            TypeReference,
            MethodDefinition,
            EventDefinition,
            PropertyDefinition,
            FieldDefinition,
            AssemblyDefinition,
            AssemblyNameReference,
            ModuleReference,
            MemberReference,
            Resources,
            AssemblyLinkedResource,
            EmbeddedResource,
            LinkedResource,
            String
        }

        public class NameSpace
        {
            string _name;
            public NameSpace(string name)
            {
                _name = name;
            }
            public string Name
            {
                get { return _name; }
            }

            public override string ToString()
            {
                return this.Name;
            }
        }

        public class Resources
        {            
            public override string ToString()
            {
                return "Resources Node";
            }
        }

        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;
            object tagx = tx.Tag;
            object tagy = ty.Tag;
            int ret;
            SortTypes stx;
            SortTypes sty;

            if (tagx == null)
            {
                stx = SortTypes.Object;
            }
            else
            {
                stx = (SortTypes)Enum.Parse(typeof(SortTypes), tagx.GetType().Name);
            }

            if (tagy == null)
            {
                sty = SortTypes.Object;
            }
            else
            {
                sty = (SortTypes)Enum.Parse(typeof(SortTypes), tagy.GetType().Name);
            }

            if (stx == sty)
            {
                ret = string.Compare(tx.Text, ty.Text);
            }
            else
            {
                ret = (int)stx - (int)sty;
            }
            return ret;
        }

    }//end of class
}

using Mono.Cecil;
using SimpleAssemblyExplorer.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SimpleAssemblyExplorer
{
    [Flags]
    public enum ObjectTypes
    {
        None,
        Constructor,
        Event,
        Field,
        Interface,
        Method,
        Property,
        Type,
        All = ObjectTypes.Constructor | ObjectTypes.Event | ObjectTypes.Field |
               ObjectTypes.Interface | ObjectTypes.Method | ObjectTypes.Property | 
               ObjectTypes.Type
    }

    public class ClassEditParams
    {
        public IHost Host { get; set; }
        public string[] Rows { get; set;  }
        public string SourceDir { get; set; }
        public ObjectTypes ObjectType { get; set;  }
        public bool ShowStaticOnly { get; set;  }
        public bool ShowSelectButton { get; set;  }

        public ClassEditParams()
        {
            ObjectType = ObjectTypes.None;
            ShowStaticOnly = false;
            ShowSelectButton = false;
        }
    }

    public class ClassEditUtils
    {
        public static Form Create(ClassEditParams p)
        {
            var a = Assembly.GetEntryAssembly();
            var t = a.GetType(a.GetName().Name + ".frmClassEdit");
            return (Form)Activator.CreateInstance(t, new object[] { p });
        }
        
        public static MethodDefinition Run(ClassEditParams p)
        {
            var f = Create(p);
            f.ShowDialog();
            
            var t = f.GetType();
            var r = t.GetProperty("SelectedMethod");
            return (MethodDefinition) r.GetValue(f, null);
        }
    }
}

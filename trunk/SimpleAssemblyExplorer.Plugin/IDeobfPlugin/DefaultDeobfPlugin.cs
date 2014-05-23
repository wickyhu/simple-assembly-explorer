using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mono.Cecil;

namespace SimpleAssemblyExplorer.Plugin
{
    public abstract class DefaultDeobfPlugin : PluginBase, IDeobfPlugin
    {
        protected DeobfPluginInfo _pluginInfo;
        public virtual DeobfPluginInfo PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                {
                    _pluginInfo = new DeobfPluginInfo();
                }
                return _pluginInfo;
            }
        }

        public override PluginInfoBase PluginInfoBase
        {
            get { return this.PluginInfo; }
        }

        public DefaultDeobfPlugin(IHost host)
            : base(host)
        {
        }

        public IDeobfuscator Deobfuscator { get; set; }

        public virtual void BeforeHandleAssembly(AssemblyDefinition assemblyDef)
        {
            return;
        }

        public virtual void AfterHandleAssembly(AssemblyDefinition assemblyDef)
        {
            return;
        }

        public virtual void BeforeHandleModule(ModuleDefinition moduleDef)
        {
            return;
        }

        public virtual void AfterHandleModule(ModuleDefinition moduleDef)
        {
            return;
        }

        public virtual void BeforeHandleType(TypeDefinition typeDef)
        {
            return;
        }

        public virtual void AfterHandleType(TypeDefinition typeDef)
        {
            return;
        }

        public virtual void BeforeHandleProperty(PropertyDefinition propertyDef)
        {
            return;
        }

        public virtual void AfterHandleProperty(PropertyDefinition propertyDef)
        {
            return;
        }

        public virtual void BeforeHandleField(FieldDefinition fieldDef)
        {
            return;
        }

        public virtual void AfterHandleField(FieldDefinition fieldDef)
        {
            return;
        }

        public virtual void BeforeHandleEvent(EventDefinition eventDef)
        {
            return;
        }

        public virtual void AfterHandleEvent(EventDefinition eventDef)
        {
            return;
        }

        public virtual int BeforeHandleMethod(MethodDefinition methodDef)
        {
            return 0;
        }

        public virtual int AfterHandleMethod(MethodDefinition methodDef)
        {
            return 0;
        }

        public virtual void Configure()
        {
            this.Deobfuscator.AppendTextInfoLine(String.Format("No configuration available for \"{0}\".", this.PluginInfo.Title));
            return;
        }

    }// end of class
}


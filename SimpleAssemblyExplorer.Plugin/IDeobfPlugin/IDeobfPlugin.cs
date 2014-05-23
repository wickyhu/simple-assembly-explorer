using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IDeobfPlugin : IPluginBase
    {     
        /// <summary>
        /// Plugin information
        /// </summary>
        DeobfPluginInfo PluginInfo { get; }

        IDeobfuscator Deobfuscator { get; set; }        

        void BeforeHandleAssembly(AssemblyDefinition assemblyDef);
        void AfterHandleAssembly(AssemblyDefinition assemblyDef);

        void BeforeHandleModule(ModuleDefinition moduleDef);
        void AfterHandleModule(ModuleDefinition moduleDef);

        void BeforeHandleType(TypeDefinition typeDef);
        void AfterHandleType(TypeDefinition typeDef);

        void BeforeHandleProperty(PropertyDefinition propertyDef);
        void AfterHandleProperty(PropertyDefinition propertyDef);

        void BeforeHandleField(FieldDefinition fieldDef);
        void AfterHandleField(FieldDefinition fieldDef);

        void BeforeHandleEvent(EventDefinition eventDef);
        void AfterHandleEvent(EventDefinition eventDef);

        int BeforeHandleMethod(MethodDefinition methodDef);
        int AfterHandleMethod(MethodDefinition methodDef);

        void Configure();

    }
}

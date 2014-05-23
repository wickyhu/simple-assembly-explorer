using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SimpleAssemblyExplorer.Plugin;
using SimpleUtils;
using Mono.Cecil;

namespace SAE.DeobfPluginSample
{
    public class DeobfPluginSample : DefaultDeobfPlugin
    {

        public DeobfPluginSample(IHost host)
            : base(host)
        {
        }

        public override DeobfPluginInfo PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                {
                    _pluginInfo = new DeobfPluginInfo();
                    _pluginInfo.Author = SimpleDotNet.Author;
                    _pluginInfo.Contact = SimpleDotNet.EmailAddress;
                    _pluginInfo.Url = SimpleDotNet.WebSiteUrl;
                    _pluginInfo.Title = "Deobfuscator Plugin Sample";
                }
                return _pluginInfo;
            }
        }
        
        public override void AfterHandleAssembly(AssemblyDefinition assemblyDef)
        {
            if (handleAssemblyEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleAssembly: {0} (0x{1:x8})", assemblyDef.FullName, assemblyDef.MetadataToken.ToUInt32()));
            }
        }

        public override void BeforeHandleAssembly(AssemblyDefinition assemblyDef)
        {
            if (handleAssemblyEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleAssembly: {0} (0x{1:x8})", assemblyDef.FullName, assemblyDef.MetadataToken.ToUInt32()));
            }            
        }



        public override void AfterHandleModule(ModuleDefinition moduleDef)
        {
            if (handleModuleEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleModule: {0} (0x{1:x8})", moduleDef.Name, moduleDef.MetadataToken.ToUInt32()));
            }
        }

        public override void BeforeHandleModule(ModuleDefinition moduleDef)
        {
            if (handleModuleEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleModule: {0} (0x{1:x8})", moduleDef.Name, moduleDef.MetadataToken.ToUInt32()));
            }
        }



        public override void AfterHandleType(TypeDefinition typeDef)
        {
            if (handleTypeEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleType: {0} (0x{1:x8})", typeDef.FullName, typeDef.MetadataToken.ToUInt32()));
            }
            if (showRenameList)
            {
                if (typeDef.OriginalName != null && typeDef.OriginalName != typeDef.Name)
                {
                    this.Deobfuscator.AppendTextInfoLine(String.Format("Type Name changed: {0} => {1}", typeDef.OriginalFullTypeName, typeDef.FullName));
                }
            }
        }

        public override void BeforeHandleType(TypeDefinition typeDef)
        {
            if (handleTypeEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleType: {0} (0x{1:x8})", typeDef.FullName, typeDef.MetadataToken.ToUInt32()));
            }
            if (showNameList)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("Type Name: {0} (0x{1:x8})", typeDef.FullName, typeDef.MetadataToken.ToUInt32()));
            }
        }



        public override int AfterHandleMethod(MethodDefinition methodDef)
        {
            if (handleMethodEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleMethod: {0} (0x{1:x8})", methodDef.Name, methodDef.MetadataToken.ToUInt32()));
            }
            if (showRenameList)
            {
                if (methodDef.OriginalName != null && methodDef.OriginalName != methodDef.Name)
                {
                    this.Deobfuscator.AppendTextInfoLine(String.Format("Method Name changed: {0} => {1}", methodDef.OriginalName, methodDef.Name));
                }
            }
            return 0;
        }

        public override int BeforeHandleMethod(MethodDefinition methodDef)
        {
            if (handleMethodEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleMethod: {0} (0x{1:x8})", methodDef.Name, methodDef.MetadataToken.ToUInt32()));
            }
            if (showNameList)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("Method Name: {0} (0x{1:x8})", methodDef.Name, methodDef.MetadataToken.ToUInt32()));
            }
            return 0;
        }



        public override void AfterHandleProperty(PropertyDefinition propertyDef)
        {
            if (handlePropertyEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleProperty: {0} (0x{1:x8})", propertyDef.Name, propertyDef.MetadataToken.ToUInt32()));
            }
            if (showRenameList)
            {
                if (propertyDef.OriginalName != null && propertyDef.OriginalName != propertyDef.Name)
                {
                    this.Deobfuscator.AppendTextInfoLine(String.Format("Property Name changed: {0} => {1}", propertyDef.OriginalName, propertyDef.Name));
                }
            }
        }

        public override void BeforeHandleProperty(PropertyDefinition propertyDef)
        {
            if (handlePropertyEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleProperty: {0} (0x{1:x8})", propertyDef.Name, propertyDef.MetadataToken.ToUInt32()));
            }
            if (showNameList)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("Property Name: {0} (0x{1:x8})", propertyDef.Name, propertyDef.MetadataToken.ToUInt32()));
            }
        }



        public override void AfterHandleField(FieldDefinition fieldDef)
        {
            if (handleFieldEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleField: {0} (0x{1:x8})", fieldDef.Name, fieldDef.MetadataToken.ToUInt32()));
            }          
            if (showRenameList)
            {
                if (fieldDef.OriginalName != null && fieldDef.OriginalName != fieldDef.Name)
                {
                    this.Deobfuscator.AppendTextInfoLine(String.Format("Field Name changed: {0} => {1}", fieldDef.OriginalName, fieldDef.Name));
                }
            }
        }

        public override void BeforeHandleField(FieldDefinition fieldDef)
        {
            if (handleFieldEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleField: {0} (0x{1:x8})", fieldDef.Name, fieldDef.MetadataToken.ToUInt32()));
            }
            if (showNameList)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("Field Name: {0} (0x{1:x8})", fieldDef.Name, fieldDef.MetadataToken.ToUInt32()));
            }
        }



        public override void AfterHandleEvent(EventDefinition eventDef)
        {
            if (handleEventEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("AfterHandleEvent: {0} (0x{1:x8})", eventDef.Name, eventDef.MetadataToken.ToUInt32()));
            }
            if (showRenameList)
            {
                if (eventDef.OriginalName != null && eventDef.OriginalName != eventDef.Name)
                {
                    this.Deobfuscator.AppendTextInfoLine(String.Format("Event Name changed: {0} => {1}", eventDef.OriginalName, eventDef.Name));
                }
            }
        }

        public override void BeforeHandleEvent(EventDefinition eventDef)
        {
            if (handleEventEvent)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("BeforeHandleEvent: {0} (0x{1:x8})", eventDef.Name, eventDef.MetadataToken.ToUInt32()));
            }
            if (showNameList)
            {
                this.Deobfuscator.AppendTextInfoLine(String.Format("Event Name: {0} (0x{1:x8})", eventDef.Name, eventDef.MetadataToken.ToUInt32()));
            }
        }

        bool handleAssemblyEvent = true;
        bool handleModuleEvent = true;
        bool handleTypeEvent = true;
        bool handleMethodEvent = true;
        bool handlePropertyEvent = true;
        bool handleFieldEvent = true;
        bool handleEventEvent = true;
        bool showNameList = false;
        bool showRenameList = false;

        public override void Configure()
        {
            frmConfig frm = new frmConfig();
            frm.HandleAssemblyEvent = handleAssemblyEvent;
            frm.HandleModuleEvent = handleModuleEvent;
            frm.HandleTypeEvent = handleTypeEvent;
            frm.HandleMethodEvent = handleMethodEvent;
            frm.HandlePropertyEvent = handlePropertyEvent;
            frm.HandleFieldEvent = handleFieldEvent;
            frm.HandleEventEvent = handleEventEvent;
            frm.ShowNameList = showNameList;
            frm.ShowRenameList = showRenameList;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                handleAssemblyEvent = frm.HandleAssemblyEvent;
                handleModuleEvent = frm.HandleModuleEvent;
                handleTypeEvent = frm.HandleTypeEvent;
                handleMethodEvent = frm.HandleMethodEvent;
                handlePropertyEvent = frm.HandlePropertyEvent;
                handleFieldEvent = frm.HandleFieldEvent;
                handleEventEvent = frm.HandleEventEvent;
                showNameList = frm.ShowNameList;
                showRenameList = frm.ShowRenameList;
            }
        }
    } //end of class
}

// ---------------------------------------------------------
// Lutz Roeder's .NET Reflector
// Copyright (c) 2000-2007 Lutz Roeder. All rights reserved.
// ---------------------------------------------------------
namespace SimpleAssemblyExplorer.LutzReflector
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;    
    using Reflector.CodeModel;
    using Reflector.CodeModel.Memory;

    public class CodeIdentifier
    {
        private CodeIdentifierType type;
        private string identifier;
        private string assembly;
        private string item;
        private string member;

        private enum CodeIdentifierType
        {
            None, Assembly, Module, Resource, Type, Field, Method, Property, Event
        }

        public CodeIdentifier(object value)
        {
            this.type = CodeIdentifierType.None;
            this.identifier = string.Empty;
            this.assembly = string.Empty;
            this.item = string.Empty;
            this.member = string.Empty;

            if (value is IAssemblyReference)
            {
                this.assembly = this.GetAssemblyReferenceText(value as IAssemblyReference);
                this.type = CodeIdentifierType.Assembly;
            }

            if (value is IModule)
            {
                IModule module = (IModule)value;
                this.assembly = this.GetAssemblyReferenceText(module.Assembly);
                this.item = module.Name;
                this.type = CodeIdentifierType.Module;
            }

            if (value is IResource)
            {
                IResource resource = (IResource)value;
                this.assembly = this.GetAssemblyReferenceText(resource.Module.Assembly);
                this.item = resource.Name;
                this.type = CodeIdentifierType.Resource;
            }

            if (value is ITypeReference)
            {
                ITypeReference typeReference = (ITypeReference)value;
                this.SetTypeDeclaration(typeReference);
                this.type = CodeIdentifierType.Type;
            }

            if (value is IMemberReference)
            {
                IMemberReference memberReference = (IMemberReference)value;
                this.SetTypeDeclaration(memberReference.DeclaringType);

                if (value is IFieldReference)
                {
                    this.member = this.GetFieldReferenceText(value as IFieldReference);
                    this.type = CodeIdentifierType.Field;
                }

                if (value is IMethodReference)
                {
                    this.member = this.GetMethodReferenceText(value as IMethodReference);
                    this.type = CodeIdentifierType.Method;
                }

                if (value is IPropertyReference)
                {
                    this.member = this.GetPropertyReferenceText(value as IPropertyReference);
                    this.type = CodeIdentifierType.Property;
                }

                if (value is IEventReference)
                {
                    this.member = this.GetEventReferenceText(value as IEventReference);
                    this.type = CodeIdentifierType.Event;
                }
            }
        }

        public CodeIdentifier(string value)
        {
            this.type = CodeIdentifierType.None;
            this.identifier = value;

            this.assembly = string.Empty;
            this.item = string.Empty;
            this.member = string.Empty;

            value = value.Trim();

            value = value.Replace("%20", " ");
            value = value.Replace("%5b", "[");
            value = value.Replace("%5d", "]");

            string codeIdentifier = "code://";

            if (value.StartsWith(codeIdentifier))
            {
                value = value.Substring(codeIdentifier.Length);

                while (value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }

                string[] parts = value.Split(new char[] { '/' });

                if (parts.Length > 0)
                {
                    this.assembly = parts[0];
                    this.type = CodeIdentifierType.Assembly;

                    if (parts.Length > 1)
                    {
                        if (parts[1].StartsWith("resource:"))
                        {
                            this.item = parts[1].Substring(9);
                            this.type = CodeIdentifierType.Resource;
                        }
                        else if (parts[1].StartsWith("module:"))
                        {
                            this.item = parts[1].Substring(7);
                            this.type = CodeIdentifierType.Module;
                        }
                        else
                        {
                            this.item = parts[1];
                            this.type = CodeIdentifierType.Type;

                            if (parts.Length > 2)
                            {
                                if (parts[2].StartsWith("property:"))
                                {
                                    this.member = parts[2].Substring(9);
                                    this.type = CodeIdentifierType.Property;
                                }
                                else if (parts[2].StartsWith("event:"))
                                {
                                    this.member = parts[2].Substring(6);
                                    this.type = CodeIdentifierType.Event;
                                }
                                else
                                {
                                    if ((parts[2].IndexOf("(") != -1) && (parts[2].IndexOf(")") != -1))
                                    {
                                        this.member = parts[2];
                                        this.type = CodeIdentifierType.Method;
                                    }
                                    else
                                    {
                                        this.member = parts[2];
                                        this.type = CodeIdentifierType.Field;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public object Resolve(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            switch (this.type)
            {
                case CodeIdentifierType.None:
                    return null;

                case CodeIdentifierType.Assembly:
                    return this.ResolveAssembly(assemblyManager, assemblyCache);

                case CodeIdentifierType.Module:
                    return this.ResolveModule(assemblyManager, assemblyCache);

                case CodeIdentifierType.Resource:
                    return this.ResolveResource(assemblyManager, assemblyCache);

                case CodeIdentifierType.Type:
                    return this.ResolveTypeDeclaration(assemblyManager, assemblyCache);

                case CodeIdentifierType.Field:
                    return this.ResolveFieldDeclaration(assemblyManager, assemblyCache);

                case CodeIdentifierType.Method:
                    return this.ResolveMethodDeclaration(assemblyManager, assemblyCache);

                case CodeIdentifierType.Property:
                    return this.ResolvePropertyDeclaration(assemblyManager, assemblyCache);

                case CodeIdentifierType.Event:
                    return this.ResolveEventDeclaration(assemblyManager, assemblyCache);
            }

            throw new NotSupportedException("Unable to resolve code identifier.");
        }

        public string Identifier
        {
            get
            {
                switch (this.type)
                {
                    case CodeIdentifierType.None:
                        return this.identifier;

                    case CodeIdentifierType.Assembly:
                        return "code://" + this.assembly;

                    case CodeIdentifierType.Module:
                        return "code://" + this.assembly + "/module:" + this.item;

                    case CodeIdentifierType.Resource:
                        return "code://" + this.assembly + "/resource:" + this.item;

                    case CodeIdentifierType.Type:
                        return "code://" + this.assembly + "/" + this.item;

                    case CodeIdentifierType.Field:
                        return "code://" + this.assembly + "/" + this.item + "/" + this.member;

                    case CodeIdentifierType.Method:
                        return "code://" + this.assembly + "/" + this.item + "/" + this.member;

                    case CodeIdentifierType.Property:
                        return "code://" + this.assembly + "/" + this.item + "/property:" + this.member;

                    case CodeIdentifierType.Event:
                        return "code://" + this.assembly + "/" + this.item + "/event:" + this.member;
                }

                throw new NotSupportedException("Unable to concatenate code identifier.");
            }
        }

        private void SetTypeDeclaration(IType type)
        {
            ITypeReference typeReference = type as ITypeReference;
            if (typeReference != null)
            {
                IAssemblyReference assemblyReference = this.GetAssemblyReference(typeReference);
                if (assemblyReference != null)
                {
                    this.assembly = this.GetAssemblyReferenceText(assemblyReference);
                }

                this.item = this.GetTypeReferenceText(typeReference);
            }

            IArrayType arrayType = type as IArrayType;
            if (arrayType != null)
            {
                this.assembly = string.Empty;
                this.item = this.GetTypeText(type);
            }
        }

        private string GetTypeReferenceText(ITypeReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                if (value.Owner is ITypeReference)
                {
                    // declaring type
                    writer.Write(this.GetTypeReferenceText(value.Owner as ITypeReference));
                    writer.Write(".");
                }
                else if ((value.Namespace != null) && (value.Namespace.Length > 0))
                {
                    writer.Write(value.Namespace);
                    writer.Write(".");
                }

                bool hasGenericType = (value.GenericType != null);

                writer.Write(value.Name);
                writer.Write(this.GetGenericArgumentListText(value.GenericArguments, hasGenericType));
                return writer.ToString();
            }
        }

        private string GetFieldReferenceText(IFieldReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write(value.Name);

                if (!value.DeclaringType.Equals(value.FieldType))
                {
                    writer.Write(":");
                    writer.Write(this.GetTypeText(value.FieldType));
                }

                return writer.ToString();
            }
        }

        private string GetMethodReferenceText(IMethodReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                bool hasGenericMethod = (value.GenericMethod != null);

                writer.Write(value.Name);
                writer.Write(this.GetGenericArgumentListText(value.GenericArguments, hasGenericMethod));
                writer.Write("(");
                writer.Write(this.GetParameterDeclarationListText(value.Parameters, value.CallingConvention));
                writer.Write(")");

                if (!IsType(value.ReturnType.Type, "System", "Void"))
                {
                    writer.Write(":");
                    writer.Write(this.GetTypeText(value.ReturnType.Type));
                }

                return writer.ToString();
            }
        }

        private string GetPropertyReferenceText(IPropertyReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write(value.Name);
                if (value.Parameters.Count > 0)
                {
                    writer.Write("(");
                    writer.Write(this.GetParameterDeclarationListText(value.Parameters, MethodCallingConvention.Default));
                    writer.Write(")");
                }
                writer.Write(":");
                writer.Write(this.GetTypeText(value.PropertyType));
                return writer.ToString();
            }
        }

        private string GetEventReferenceText(IEventReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write(value.Name);
                writer.Write(":");
                writer.Write(this.GetTypeText(value.EventType));
                return writer.ToString();
            }
        }

        private string GetParameterDeclarationListText(IParameterDeclarationCollection value, MethodCallingConvention callingConvention)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    IType parameterType = value[i].ParameterType;
                    if ((parameterType != null) || ((i + 1) != value.Count))
                    {
                        if (i != 0)
                        {
                            writer.Write(",");
                        }

                        if (parameterType == null)
                        {
                            writer.Write("...");
                        }
                        else
                        {
                            writer.Write(this.GetTypeText(parameterType));
                        }
                    }
                }

                if (callingConvention == MethodCallingConvention.VariableArguments)
                {
                    if (value.Count > 0)
                    {
                        writer.Write(",");
                    }

                    writer.Write("...");
                }

                return writer.ToString();
            }
        }

        private string GetPublicKeyTokenText(byte[] value)
        {
            if ((value != null) && (value.Length != 0))
            {
                string[] parts = new string[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    parts[i] = value[i].ToString("x2", CultureInfo.InvariantCulture);
                }

                return string.Concat(parts);
            }

            return "null";
        }

        private string GetAssemblyReferenceText(IAssemblyReference value)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write(value.Name);

                Version version = new Version(0, 0, 0, 0);
                if ((value.Version != null) && (!value.Version.Equals(version)))
                {
                    writer.Write(":");
                    writer.Write(value.Version.ToString());
                }
                else if (((value.PublicKeyToken != null) && (value.PublicKeyToken.Length > 0)) || ((value.Culture != null) && (value.Culture.Length > 0)))
                {
                    writer.Write(":");
                }

                if ((value.PublicKeyToken != null) && (value.PublicKeyToken.Length > 0))
                {
                    writer.Write(":");
                    writer.Write(this.GetPublicKeyTokenText(value.PublicKeyToken));
                }
                else if ((value.Culture != null) && (value.Culture.Length > 0))
                {
                    writer.Write(":");
                }

                if ((value.Culture != null) && (value.Culture.Length > 0))
                {
                    writer.Write(":");
                    writer.Write(value.Culture);
                }

                return writer.ToString();
            }
        }

        private string GetGenericArgumentListText(ITypeCollection value, bool instance)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                if (value.Count > 0)
                {
                    writer.Write("<");

                    for (int i = 0; i < value.Count; i++)
                    {
                        if (i != 0)
                        {
                            writer.Write(",");
                        }

                        if (instance)
                        {
                            writer.Write(this.GetTypeText(value[i]));
                        }
                    }

                    writer.Write(">");
                }

                return writer.ToString();
            }
        }

        private string GetTypeText(IType value)
        {
            ITypeReference typeReference = value as ITypeReference;
            if (typeReference != null)
            {
                string specialName = this.GetTypeSpecialNameText(typeReference);
                if ((specialName != null) && (specialName.Length > 0))
                {
                    return specialName;
                }

                string typeReferenceText = this.GetTypeReferenceText(typeReference);
                return typeReferenceText;
            }

            IPointerType pointerType = value as IPointerType;
            if (pointerType != null)
            {
                return this.GetTypeText(pointerType.ElementType) + "*";
            }

            IReferenceType referenceType = value as IReferenceType;
            if (referenceType != null)
            {
                return this.GetTypeText(referenceType.ElementType) + "&";
            }

            IArrayType arrayType = value as IArrayType;
            if (arrayType != null)
            {
                using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    writer.Write(this.GetTypeText(arrayType.ElementType));
                    writer.Write("[");

                    IArrayDimensionCollection dimensions = arrayType.Dimensions;
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        if (i != 0)
                        {
                            writer.Write(",");
                        }

                        int lowerBound = dimensions[i].LowerBound;
                        int upperBound = dimensions[i].UpperBound;

                        if (lowerBound != -1)
                        {
                            writer.Write(lowerBound.ToString(CultureInfo.InvariantCulture));
                        }

                        if ((lowerBound != -1) || (upperBound != -1))
                        {
                            writer.Write(":");
                        }

                        if (upperBound != -1)
                        {
                            writer.Write(upperBound.ToString(CultureInfo.InvariantCulture));
                        }
                    }

                    writer.Write("]");
                    return writer.ToString();
                }
            }

            IOptionalModifier optionalModifier = value as IOptionalModifier;
            if (optionalModifier != null)
            {
                return "{optional:" + this.GetTypeText(optionalModifier.Modifier) + "}" + this.GetTypeText(optionalModifier.ElementType);
            }

            IRequiredModifier requiredModifier = value as IRequiredModifier;
            if (requiredModifier != null)
            {
                return "{required:" + this.GetTypeText(requiredModifier.Modifier) + "}" + this.GetTypeText(requiredModifier.ElementType);
            }

            IFunctionPointer functionPointer = value as IFunctionPointer;
            if (functionPointer != null)
            {
                using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    writer.Write("*");
                    writer.Write("(");

                    for (int i = 0; i < functionPointer.Parameters.Count; i++)
                    {
                        if (i != 0)
                        {
                            writer.Write(",");
                        }

                        writer.Write(this.GetTypeText(functionPointer.Parameters[i].ParameterType));
                    }

                    writer.Write(")");
                    writer.Write(":");
                    writer.Write(this.GetTypeText(functionPointer.ReturnType.Type));
                    return writer.ToString();
                }
            }

            IGenericParameter genericParameter = value as IGenericParameter;
            if (genericParameter != null)
            {
                return genericParameter.Name;
            }

            IGenericArgument genericArgument = value as IGenericArgument;
            if (genericArgument != null)
            {
                ITypeReference genericType = genericArgument.Owner as ITypeReference;
                if (genericType != null)
                {
                    return "<!" + genericArgument.Position.ToString(CultureInfo.InvariantCulture) + ">";
                }

                IMethodReference genericMethod = genericArgument.Owner as IMethodReference;
                if (genericMethod != null)
                {
                    return "<!!" + genericArgument.Position.ToString(CultureInfo.InvariantCulture) + ">";
                }
            }

            throw new NotSupportedException("Invalid type in code identifier.");
        }

        private string GetTypeSpecialNameText(ITypeReference value)
        {
            IAssemblyReference assemblyReference = value.Owner as IAssemblyReference;
            if ((assemblyReference != null) && (assemblyReference.Name == "mscorlib") && (value.GenericArguments.Count == 0))
            {
                if (value.Namespace == "System")
                {
                    switch (value.Name)
                    {
                        case "Void": return "Void";
                        case "Object": return "Object";
                        case "Boolean": return "Boolean";
                        case "SByte": return "SByte";
                        case "Byte": return "Byte";
                        case "Int16": return "Int16";
                        case "UInt16": return "UInt16";
                        case "Int32": return "Int32";
                        case "UInt32": return "UInt32";
                        case "Int64": return "Int64";
                        case "UInt64": return "UInt64";
                        case "Single": return "Single";
                        case "Double": return "Double";
                        case "String": return "String";
                        case "Char": return "Char";
                        case "IntPtr": return "IntPtr";
                        case "UIntPtr": return "UIntPtr";
                    }
                }
            }

            return null;
        }

        private IAssembly ResolveAssembly(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            IAssemblyReference assemblyReference = this.ParseAssemblyReference(this.assembly, assemblyManager);

            for (int i = 0; i < assemblyManager.Assemblies.Count; i++)
            {
                IAssembly assembly = assemblyManager.Assemblies[i];
                if (assembly.Equals(assemblyReference))
                {
                    return assembly;
                }
            }

            if (assemblyCache != null)
            {
                string location = assemblyCache.QueryLocation(assemblyReference, null);
                if ((location != null) && (location.Length > 0))
                {
                    IAssembly assembly = assemblyManager.LoadFile(location);
                    return assembly;
                }
            }

            return null;
        }

        private IModule ResolveModule(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            IAssembly assembly = this.ResolveAssembly(assemblyManager, assemblyCache);
            if (assembly != null)
            {
                string moduleName = this.item;

                foreach (IModule module in assembly.Modules)
                {
                    if (moduleName == module.Name)
                    {
                        return module;
                    }
                }
            }

            return null;
        }

        private IResource ResolveResource(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            IAssembly assembly = this.ResolveAssembly(assemblyManager, assemblyCache);
            if (assembly != null)
            {
                string resourceName = this.item;

                foreach (IResource resource in assembly.Resources)
                {
                    if (resourceName == resource.Name)
                    {
                        return resource;
                    }
                }
            }

            return null;
        }

        private ITypeDeclaration ResolveTypeDeclaration(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            IAssembly[] assemblyList = new IAssembly[assemblyManager.Assemblies.Count];
            assemblyManager.Assemblies.CopyTo(assemblyList, 0);

            if ((this.assembly != null) && (this.assembly.Length > 0))
            {
                IAssembly assembly = this.ResolveAssembly(assemblyManager, assemblyCache);
                assemblyList = (assembly != null) ? new IAssembly[] { assembly } : new IAssembly[0];
            }

            string typeName = this.item;

            foreach (IAssembly assembly in assemblyList)
            {
                foreach (IModule module in assembly.Modules)
                {
                    foreach (ITypeDeclaration typeDeclaration in module.Types)
                    {
                        foreach (ITypeDeclaration currentType in GetNestedTypeList(typeDeclaration))
                        {
                            string text = this.GetTypeReferenceText(currentType);
                            if (typeName == text)
                            {
                                return currentType;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private IFieldDeclaration ResolveFieldDeclaration(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            ITypeDeclaration typeDeclaration = this.ResolveTypeDeclaration(assemblyManager, assemblyCache);
            if (typeDeclaration != null)
            {
                string fieldName = this.member;

                foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
                {
                    string text = this.GetFieldReferenceText(fieldDeclaration);
                    if (fieldName == text)
                    {
                        return fieldDeclaration;
                    }
                }
            }

            return null;
        }

        private IMethodDeclaration ResolveMethodDeclaration(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            ITypeDeclaration typeDeclaration = this.ResolveTypeDeclaration(assemblyManager, assemblyCache);
            if (typeDeclaration != null)
            {
                string methodName = this.member;

                foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
                {
                    string text = this.GetMethodReferenceText(methodDeclaration);
                    if (methodName == text)
                    {
                        return methodDeclaration;
                    }
                }
            }

            return null;
        }

        private IPropertyDeclaration ResolvePropertyDeclaration(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            ITypeDeclaration typeDeclaration = this.ResolveTypeDeclaration(assemblyManager, assemblyCache);
            if (typeDeclaration != null)
            {
                string propertyName = this.member;

                foreach (IPropertyDeclaration propertyDeclaration in typeDeclaration.Properties)
                {
                    string text = this.GetPropertyReferenceText(propertyDeclaration);
                    if (propertyName == text)
                    {
                        return propertyDeclaration;
                    }
                }
            }

            return null;
        }

        private IEventDeclaration ResolveEventDeclaration(IAssemblyManager assemblyManager, IAssemblyCache assemblyCache)
        {
            ITypeDeclaration typeDeclaration = this.ResolveTypeDeclaration(assemblyManager, assemblyCache);
            if (typeDeclaration != null)
            {
                string eventName = this.member;

                foreach (IEventDeclaration eventDeclaration in typeDeclaration.Events)
                {
                    string text = this.GetEventReferenceText(eventDeclaration);
                    if (eventName == text)
                    {
                        return eventDeclaration;
                    }
                }
            }

            return null;
        }

        private static bool CompareAssemblyReference(IAssemblyReference assemblyReference, string value)
        {
            string[] parts = value.Split(new char[] { ':' });

            if (parts.Length > 0)
            {
                string name = parts[0];
                if (!assemblyReference.Name.Equals(name))
                {
                    return false;
                }
            }

            if (parts.Length > 1)
            {
                Version version = new Version((parts[1].Length != 0) ? parts[1] : "0.0.0.0");
                if (!assemblyReference.Version.Equals(version))
                {
                    return false;
                }
            }

            if (parts.Length > 2)
            {
                byte[] publicKeyToken = assemblyReference.PublicKeyToken;
                int length = parts[2].Length / 2;
                if (length != publicKeyToken.Length)
                {
                    return false;
                }

                for (int i = 0; i < length; i++)
                {
                    byte item = byte.Parse(parts[2].Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (item != publicKeyToken[i])
                    {
                        return false;
                    }
                }
            }

            if (parts.Length > 3)
            {
                string culture = parts[3];
                if (!assemblyReference.Culture.Equals(culture))
                {
                    return false;
                }
            }

            return true;
        }

        private IAssemblyReference GetAssemblyReference(ITypeReference value)
        {
            if (value.Owner is ITypeReference)
            {
                return this.GetAssemblyReference(value.Owner as ITypeReference);
            }
            else if (value.Owner is IAssemblyReference)
            {
                return (value.Owner as IAssemblyReference);
            }
            else if (value.Owner is IModuleReference)
            {
                IModuleReference moduleReference = (IModuleReference)value.Owner;
                IModule module = moduleReference.Resolve();
                if (module != null)
                {
                    return module.Assembly;
                }

                return null;
            }

            throw new NotSupportedException("Unable to get assembly reference for type of code identifier.");
        }

        private static ICollection GetNestedTypeList(ITypeDeclaration value)
        {
            ITypeDeclarationCollection nestedTypes = value.NestedTypes;
            if (nestedTypes.Count != 0)
            {
                ArrayList list = new ArrayList();

                list.Add(value);

                foreach (ITypeDeclaration nestedType in nestedTypes)
                {
                    list.AddRange(GetNestedTypeList(nestedType));
                }

                return list;
            }

            return new object[] { value };
        }

        private IAssemblyReference ParseAssemblyReference(string value, IAssemblyManager assemblyManager)
        {
            string[] assemblyNameParts = this.assembly.Split(new char[] { ':' });

            IAssembly assembly = new Assembly();
            assembly.AssemblyManager = assemblyManager;

            IModule module = new Module();
            module.Assembly = assembly;

            IAssemblyReference assemblyReference = new AssemblyReference();
            assemblyReference.Context = module;
            assemblyReference.Name = assemblyNameParts[0];
            assemblyReference.Version = ((assemblyNameParts.Length > 1) && (assemblyNameParts[1].Length > 0)) ? new Version(assemblyNameParts[1]) : new Version(0, 0, 0, 0);
            assemblyReference.PublicKeyToken = ((assemblyNameParts.Length > 2) && (assemblyNameParts[2].Length > 0)) ? ParseByteArray(assemblyNameParts[2]) : new byte[0];
            assemblyReference.Culture = ((assemblyNameParts.Length > 3) && (assemblyNameParts[3].Length > 0)) ? assemblyNameParts[3] : string.Empty;

            return assemblyReference;
        }

        private static byte[] ParseByteArray(string value)
        {
            byte[] buffer = new byte[value.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return buffer;
        }

        private static bool IsType(IType value, string namespaceName, string name)
        {
            ITypeReference typeReference = value as ITypeReference;
            if (typeReference != null)
            {
                return ((typeReference.Name == name) && (typeReference.Namespace == namespaceName) && (typeReference.GenericArguments.Count == 0));
            }

            IRequiredModifier requiredModifier = value as IRequiredModifier;
            if (requiredModifier != null)
            {
                return IsType(requiredModifier.ElementType, namespaceName, name);
            }

            IOptionalModifier optionalModifier = value as IOptionalModifier;
            if (optionalModifier != null)
            {
                return IsType(optionalModifier.ElementType, namespaceName, name);
            }

            return false;
        }
    }
}

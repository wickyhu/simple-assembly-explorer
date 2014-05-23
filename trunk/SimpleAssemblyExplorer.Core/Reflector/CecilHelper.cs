using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;

namespace SimpleAssemblyExplorer.LutzReflector
{
    public class CecilHelper
    {
        private static Dictionary<int, Dictionary<int, object>> _cache = new Dictionary<int, Dictionary<int, object>>();

        private static int GetAssemblyCacheKey(AssemblyDefinition ad)
        {
            return ad.Name.FullName.GetHashCode();
        }

        public static Dictionary<int, object> GetAssemblyCache(AssemblyDefinition ad)
        {
            int adKey = GetAssemblyCacheKey(ad);
            if (!_cache.ContainsKey(adKey))
            {
                CreateAssemblyCache(ad);
            }
            return _cache[adKey];
        }

        //public static bool AssemblyCacheExists(AssemblyDefinition ad)
        //{
        //    int adKey = GetAssemblyCacheKey(ad);
        //    return _cache.ContainsKey(adKey);
        //}

        public static void CreateAssemblyCache(AssemblyDefinition ad)
        {
            int adKey = GetAssemblyCacheKey(ad);
            if (!_cache.ContainsKey(adKey))
                _cache.Add(adKey, new Dictionary<int, object>());
        }

        public static void RemoveAssemblyCache(AssemblyDefinition ad)
        {
            if (ad == null) return;
            int adKey = GetAssemblyCacheKey(ad);
            if (_cache.ContainsKey(adKey))
            {
                Dictionary<int, object> adCache = _cache[adKey];
                adCache.Clear();
                _cache.Remove(adKey);
                adCache = null;
            }
        }

        public static object Cecil2Reflector(object cecilObject)
        {
            if (cecilObject is IMetadataTokenProvider)
            {
                AssemblyDefinition ad;
                if (cecilObject is TypeReference)
                    ad = ((TypeReference)cecilObject).Module.Assembly;
                else if (cecilObject is MemberReference)
                    ad = ((MemberReference)cecilObject).DeclaringType.Module.Assembly;
                else if (cecilObject is ModuleDefinition)
                    ad = ((ModuleDefinition)cecilObject).Assembly;
                else ad = null;

                if (ad != null)
                {
                    Dictionary<int, object> adCache = GetAssemblyCache(ad);
                    if (adCache != null)
                    {
                        IMetadataTokenProvider mtp = (IMetadataTokenProvider)cecilObject;
                        int objectKey = mtp.GetHashCode();
                        if (adCache.ContainsKey(objectKey))
                        {
                            return adCache[objectKey];
                        }
                        object reflectorObject = Cecil2ReflectorWithoutCache(cecilObject);
#if !DEBUG
                        adCache.Add(objectKey, reflectorObject);
#endif
                        return reflectorObject;
                    }
                    //else // GetAssemblyCache will create one if not found
                    //{
                    //    CreateAssemblyCache(ad);
                    //}
                }
            }

            return Cecil2ReflectorWithoutCache(cecilObject);
        }

        public static object Cecil2ReflectorWithoutCache(object cecilObject)
        {
            object reflectorObject;

            if (cecilObject is MethodDefinition)
                reflectorObject = Cecil2Reflector((MethodDefinition)cecilObject);
            else if (cecilObject is PropertyDefinition)
                reflectorObject = Cecil2Reflector((PropertyDefinition)cecilObject);
            else if (cecilObject is FieldDefinition)
                reflectorObject = Cecil2Reflector((FieldDefinition)cecilObject);
            else if (cecilObject is TypeDefinition)
                reflectorObject = Cecil2Reflector((TypeDefinition)cecilObject);
            else if (cecilObject is EventDefinition)
                reflectorObject = Cecil2Reflector((EventDefinition)cecilObject);
            else if (cecilObject is ModuleDefinition)
                reflectorObject = Cecil2Reflector((ModuleDefinition)cecilObject);
            else if (cecilObject is AssemblyDefinition)
                reflectorObject = Cecil2Reflector((AssemblyDefinition)cecilObject);
            else if (cecilObject is TypeReference)
                reflectorObject = Cecil2Reflector((TypeReference)cecilObject);
            //else if (cecilObject is EmbeddedResource)
            //    reflectorObject = Cecil2Reflector((EmbeddedResource)cecilObject);
            else reflectorObject = null;

            return reflectorObject;
        }

        public static Reflector.CodeModel.IAssembly Cecil2Reflector(AssemblyDefinition ad)
        {
            if (ad == null) 
                return null;
            string adName = ad.ToString();
            string adLocation = ad.MainModule.FullyQualifiedName;
            foreach (Reflector.CodeModel.IAssembly a in SimpleReflector.Default.AssemblyManager.Assemblies)
            {
                if (a.Location == adLocation || a.ToString() == adName)
                    return a;
            }
            return null;
        }

        public static Reflector.CodeModel.IAssembly Cecil2Reflector(string location)
        {
            foreach (Reflector.CodeModel.IAssembly a in SimpleReflector.Default.AssemblyManager.Assemblies)
            {
                if (a.Location == location)
                    return a;
            }
            return null;
        }

        public static Reflector.CodeModel.IModule Cecil2Reflector(ModuleDefinition md)
        {
            AssemblyDefinition ad = md.Assembly;
            Reflector.CodeModel.IAssembly a = (ad != null ? Cecil2Reflector(ad) : Cecil2Reflector(md.FullyQualifiedName));
            if (a == null) return null;

            string mdName = md.Name;
            foreach (Reflector.CodeModel.IModule m in a.Modules)
            {
                if (m.Name == mdName)
                    return m;
            }
            return null;
        }

        public static Reflector.CodeModel.IAssemblyReference Cecil2Reflector(AssemblyNameReference anr, ModuleDefinition md)
        {
            Reflector.CodeModel.IModule m = Cecil2Reflector(md);
            if (m == null) return null;

            string anrName = anr.ToString();
            foreach (Reflector.CodeModel.IAssemblyReference ar in m.AssemblyReferences)
            {
                if (ar.ToString() == anrName) 
                    return ar;
            }
            return null;
        }

        public static Reflector.CodeModel.IModuleReference Cecil2Reflector(ModuleReference mr, ModuleDefinition md)
        {
            Reflector.CodeModel.IModule m = Cecil2Reflector(md);
            if (m == null) return null;

            string mrName = mr.Name;
            foreach (Reflector.CodeModel.IModuleReference mrTmp in m.ModuleReferences)
            {
                if (mrTmp.Name == mrName)
                    return mrTmp;
            }
            return null;
        }

        public static Reflector.CodeModel.IResource Cecil2Reflector(Resource r, ModuleDefinition md)
        {
            Reflector.CodeModel.IModule m = Cecil2Reflector(md);
            if (m == null) return null;

            string erName = r.Name;
            
            foreach (Reflector.CodeModel.IResource r2 in m.Assembly.Resources)
            {
                if (r2.Name == erName)
                {
                    return r2;
                }
            }
            return null;
        }     

        public static bool IsEqual(Reflector.CodeModel.IType t, TypeReference tr)
        {
            if (t is Reflector.CodeModel.ITypeReference)
            {
                return IsEqual((Reflector.CodeModel.ITypeReference)t, tr);
            }
            else if ((t is Reflector.CodeModel.IArrayType) && (tr is ArrayType))
            {
                Reflector.CodeModel.IArrayType t2 = (Reflector.CodeModel.IArrayType)t;
                ArrayType tr2 = (ArrayType)tr;
                if (
                    !(t2.Dimensions.Count == 0 && tr2.Dimensions.Count == 1) // one dimension
                    && (t2.Dimensions.Count != tr2.Dimensions.Count) // two or more dimension 
                    ) return false;
                
                return IsEqual(t2.ElementType, tr2.ElementType);
            }
            else if ((t is Reflector.CodeModel.IReferenceType) && (tr is ByReferenceType))
            {
                return IsEqual(((Reflector.CodeModel.IReferenceType)t).ElementType, ((ByReferenceType)tr).ElementType);
            }
            else if ((t is Reflector.CodeModel.IPointerType) && (tr is PointerType))
            {
                return IsEqual(((Reflector.CodeModel.IPointerType)t).ElementType, ((PointerType)tr).ElementType);
            }
            else if ((t is Reflector.CodeModel.IOptionalModifier) && (tr is OptionalModifierType))
            {
                Reflector.CodeModel.IOptionalModifier t2 = (Reflector.CodeModel.IOptionalModifier)t;
                OptionalModifierType tr2 = (OptionalModifierType)tr;
                return IsEqual(t2.Modifier, tr2.ModifierType) && IsEqual(t2.ElementType, tr2.ElementType);
            }
            else if ((t is Reflector.CodeModel.IFunctionPointer) && (tr is FunctionPointerType))
            {
                Reflector.CodeModel.IFunctionPointer t2 = (Reflector.CodeModel.IFunctionPointer)t;
                FunctionPointerType tr2 = (FunctionPointerType)tr;
                if (t2.Parameters.Count == tr2.Parameters.Count && IsEqual(t2.ReturnType.Type, tr2.ReturnType))
                {
                    for (int i = 0; i < t2.Parameters.Count; i++)
                    {
                        if (!IsEqual(t2.Parameters[i].ParameterType, tr2.Parameters[i].ParameterType))
                            return false;
                    }
                    return true;
                }
                return false;
            }
            else if ((t is Reflector.CodeModel.IRequiredModifier) && (tr is RequiredModifierType))
            {
                Reflector.CodeModel.IRequiredModifier t2 = (Reflector.CodeModel.IRequiredModifier)t;
                RequiredModifierType tr2 = (RequiredModifierType)tr;
                return IsEqual(t2.Modifier, tr2.ModifierType) && IsEqual(t2.ElementType, tr2.ElementType);
            }
            else
            {
                string trName = InsUtils.GetOldMemberName(tr);
                //when tr is GenericParameter, it may be !0 or something, no clue what it is, no luck ...
                if ("!0" == trName && t.ToString() == "T")
                    return true;
                return t.ToString().Equals(trName);
            }
        }

        public static bool IsEqual(Reflector.CodeModel.ITypeReference t, TypeReference tr)
        {
            if (t == null && tr == null) return true;
            if ((t == null && tr != null) || (t != null && tr == null)) 
                return false;
            
            if (tr.Namespace != t.Namespace) return false;

            //if (tr is ByReferenceType)
            //    tr = ((ByReferenceType)tr).ElementType;
            //if (tr is ArrayType)
            //    tr = ((ArrayType)tr).ElementType;
            
            string trName = InsUtils.GetOldMemberName(tr);

            if (tr is IGenericInstance)
            {
                if (!trName.StartsWith(t.Name+"`") && !trName.Equals(t.Name)) 
                    return false;
                IGenericInstance gi = ((IGenericInstance)tr);
                if (gi.GenericArguments.Count != t.GenericArguments.Count) return false;
                for(int i=0; i<t.GenericArguments.Count; i++)
                {
                    if(!IsEqual(t.GenericArguments[i], 
                        gi.GenericArguments[i])) 
                        return false;
                }
            }
            else if (tr.HasGenericParameters)
            {
                if (!trName.StartsWith(t.Name + "`") && !trName.Equals(t.Name)) 
                    return false;
                if (tr.GenericParameters.Count != t.GenericArguments.Count) return false;
                for (int i = 0; i < t.GenericArguments.Count; i++)
                {
                    if (!IsEqual(t.GenericArguments[i],
                        tr.GenericParameters[i]))
                        return false;
                }
            }
            else
            {
                if (trName != t.Name) return false;
            }
            
            if (t.Owner == null && tr.DeclaringType == null) 
                return true;
            if (t.Owner is Reflector.CodeModel.ITypeReference)
            {
                if(tr.DeclaringType != null)
                    return IsEqual((Reflector.CodeModel.ITypeReference)t.Owner, tr.DeclaringType);
                if(tr is Mono.Cecil.TypeSpecification)
                    return IsEqual((Reflector.CodeModel.ITypeReference)t.Owner, ((TypeSpecification)tr).ElementType.DeclaringType);
                return false;
            }
            if (t.Owner == null && tr.DeclaringType != null) return false;
            //now owner is not null but not ITypeReference, and DeclaringType is null?
            return true;
        }

        public static bool IsEqual(TypeReference tr1, TypeReference tr2)
        {
            if (tr1 == null && tr2 == null) return true;
            if ((tr1 == null && tr2 != null) || (tr1 != null && tr2 == null)) return false;

            string trName1 = InsUtils.GetOldMemberName(tr1);
            string trName2 = InsUtils.GetOldMemberName(tr2);

            if (trName1 != trName2 || tr1.Namespace != tr2.Namespace) return false;
            
            if (tr1 is IGenericInstance)
            {
                IGenericInstance gi1 = tr1 as IGenericInstance;
                IGenericInstance gi2 = tr2 as IGenericInstance;
                if (gi2 == null) return false;
                if (gi1.GenericArguments.Count != gi2.GenericArguments.Count) return false;
                for (int i = 0; i < gi1.GenericArguments.Count; i++)
                {
                    if (!IsEqual(gi1.GenericArguments[i], gi2.GenericArguments[i])) return false;
                }
            }

            if (tr1.DeclaringType == null && tr2.DeclaringType == null) return true;
            if ((tr1.DeclaringType == null && tr2.DeclaringType != null)
                || (tr1.DeclaringType != null && tr2.DeclaringType == null)) return false;
            return IsEqual(tr1.DeclaringType, tr2.DeclaringType);
        }

        public static Reflector.CodeModel.ITypeDeclaration Cecil2Reflector(TypeDefinition td, Reflector.CodeModel.ITypeDeclarationCollection types)
        {
            foreach (Reflector.CodeModel.ITypeDeclaration t in types)
            {
                if (IsEqual(t, td))
                    return t;
                if (t.NestedTypes.Count > 0)
                {
                    Reflector.CodeModel.ITypeDeclaration t2 = Cecil2Reflector(td, t.NestedTypes);
                    if (t2 != null) return t2;
                }
            }
            return null;
        }

        public static Reflector.CodeModel.ITypeDeclaration Cecil2Reflector(TypeDefinition td)
        {
            Reflector.CodeModel.IModule m = Cecil2Reflector(td.Module);
            if (m == null) return null;

            foreach (Reflector.CodeModel.ITypeDeclaration t in m.Types)
            {
                if (IsEqual(t, td))
                        return t;
                if (t.NestedTypes.Count > 0)
                {
                    Reflector.CodeModel.ITypeDeclaration t2 = Cecil2Reflector(td, t.NestedTypes);
                    if (t2 != null) return t2;
                }
            }
            return null;
        }

        public static TypeDefinition FindTypeDefinition(TypeReference tr, Collection<TypeDefinition> types)
        {
            foreach (TypeDefinition t in types)
            {
                if (IsEqual(t, tr))
                    return t;
                if (t.NestedTypes.Count > 0)
                {
                    TypeDefinition t2 = FindTypeDefinition(tr, t.NestedTypes);
                    if (t2 != null) return t2;
                }
            }
            return null;
        }

        public static Reflector.CodeModel.ITypeDeclaration Cecil2Reflector(TypeReference tr)
        {
            TypeDefinition td = null;

            if (tr.Scope is ModuleDefinition)
            {
                ModuleDefinition module = tr.Module;

                foreach (TypeDefinition td1 in module.AllTypes)
                {
                    if (IsEqual(td1, tr))
                    {
                        td = td1;
                        break;
                    }
                    if (td1.NestedTypes.Count > 0)
                    {
                        TypeDefinition td2 = FindTypeDefinition(td, td1.NestedTypes);
                        if (td2 != null)
                        {
                            td = td2;
                            break;
                        }
                    }
                }
            }
            //else if (tr.Scope is AssemblyNameReference)
            //{
            //}

            if (td == null) return null;
            return Cecil2Reflector(td);
        }

        public static bool IsEqual(Reflector.CodeModel.IMethodDeclaration m, MethodDefinition md)
        {
            if (IsEqual((Reflector.CodeModel.IMethodReference)m, (MethodReference)md))
            {
                if (m.Overrides.Count == md.Overrides.Count)
                {
                    bool matched = true;
                    for (int i = 0; i < md.Overrides.Count; i++)
                    {
                        if (!IsEqual(m.Overrides[i].DeclaringType, md.Overrides[i].DeclaringType)
                            || !IsEqual(m.Overrides[i], md.Overrides[i], false)
                            )
                        {
                            matched = false;
                            break;
                        }
                    }
                    return matched;
                }
            }
            return false;
        }

        public static bool IsEqual(Reflector.CodeModel.IMethodReference m, MethodReference mr)
        {
            return IsEqual(m, mr, true);
        }

        public static bool IsEqual(Reflector.CodeModel.IMethodReference m, MethodReference mr, bool checkReturnType) 
        {
            string mrName = InsUtils.GetOldMemberName(mr);
            if (m.Name == mrName
                && (!checkReturnType || IsEqual(m.ReturnType.Type, mr.ReturnType))
                && m.Parameters.Count == mr.Parameters.Count
                )
            {
                bool matched = true;
                for (int i = 0; i < mr.Parameters.Count; i++)
                {
                    if (!IsEqual(m.Parameters[i].ParameterType,
                        mr.Parameters[i].ParameterType))
                    {
                        matched = false;
                        break;
                    }
                }
                return matched;
            }
            return false;
        }

        public static Reflector.CodeModel.IMethodDeclaration Cecil2Reflector(MethodDefinition md)
        {
            Reflector.CodeModel.ITypeDeclaration t = Cecil2Reflector(md.DeclaringType);
            if (t == null) return null;

            foreach (Reflector.CodeModel.IMethodDeclaration m in t.Methods)
            {
                if (IsEqual(m, md))
                        return m;
            }
            return null;
        }

        public static bool IsEqual(Reflector.CodeModel.IPropertyDeclaration p, PropertyDefinition pd)
        {
            string pdName = InsUtils.GetOldMemberName(pd);
            if (p.Name == pdName
                && IsEqual(p.PropertyType, pd.PropertyType)
                && p.Parameters.Count == pd.Parameters.Count)
            {
                bool matched = true;
                for (int i = 0; i < pd.Parameters.Count; i++)
                {
                    if (!IsEqual(p.Parameters[i].ParameterType,
                        pd.Parameters[i].ParameterType))
                    {
                        matched = false;
                        break;
                    }
                }
                return matched;
            }
            return false;
        }

        public static Reflector.CodeModel.IPropertyDeclaration Cecil2Reflector(PropertyDefinition pd)
        {
            Reflector.CodeModel.ITypeDeclaration t = Cecil2Reflector(pd.DeclaringType);
            if (t == null) return null;           

            foreach (Reflector.CodeModel.IPropertyDeclaration p in t.Properties)
            {
                if (IsEqual(p, pd))
                     return p;
            }
            return null;
        }

        public static bool IsEqual(Reflector.CodeModel.IEventDeclaration e, EventDefinition ed)
        {
            string edName = InsUtils.GetOldMemberName(ed);

            if (e.Name == edName
               && IsEqual(e.EventType, ed.EventType))
            {
                return true;
            }
            return false;
        }

        public static Reflector.CodeModel.IEventDeclaration Cecil2Reflector(EventDefinition ed)
        {
            Reflector.CodeModel.ITypeDeclaration t = Cecil2Reflector(ed.DeclaringType);
            if (t == null) return null;
       
            foreach (Reflector.CodeModel.IEventDeclaration e in t.Events)
            {
                if (IsEqual(e, ed))
                {
                    return e;
                }
            }
            return null;
        }

        public static bool IsEqual(Reflector.CodeModel.IFieldDeclaration f, FieldDefinition fd)
        {
            string fdName = InsUtils.GetOldMemberName(fd);
            if (f.Name == fdName
                && IsEqual(f.FieldType, fd.FieldType))
            {
                return true;
            }
            return false;
        }

        public static Reflector.CodeModel.IFieldDeclaration Cecil2Reflector(FieldDefinition fd)
        {
            Reflector.CodeModel.ITypeDeclaration t = Cecil2Reflector(fd.DeclaringType);
            if (t == null) return null;

            foreach (Reflector.CodeModel.IFieldDeclaration f in t.Fields)
            {
                if (IsEqual(f, fd))
                {
                    return f;
                }
            }
            return null;
        }

    }//end of class
}

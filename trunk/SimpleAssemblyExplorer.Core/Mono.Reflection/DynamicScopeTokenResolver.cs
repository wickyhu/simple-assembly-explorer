/*
Credit:
Haibo Luo
IL Visualizer
http://blogs.msdn.com/b/haibo_luo/archive/2006/11/16/take-two-il-visualizer.aspx
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

namespace Mono.Reflection
{
    internal class DynamicScopeTokenResolver : ITokenResolver
    {
        private static PropertyInfo dynamicScopePropertyItem;
        private static FieldInfo dynamicILGeneratorFieldScope;

        private static Type genericMethodInfoType;
        private static FieldInfo genericMethodInfoFieldMethodHandle, genericMethodInfoFieldContext;

        private static Type varArgMethodType;
        private static FieldInfo varArgMethodFieldMethod, varArgMethodFieldSignature;

        private static Type genericFieldInfoType;
        private static FieldInfo genericFieldInfoFieldFieldHandle, genericFieldInfoFieldContext;

        static DynamicScopeTokenResolver()
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            dynamicScopePropertyItem = Type.GetType("System.Reflection.Emit.DynamicScope").GetProperty("Item", flags);
            dynamicILGeneratorFieldScope = Type.GetType("System.Reflection.Emit.DynamicILGenerator").GetField("m_scope", flags);

            varArgMethodType = Type.GetType("System.Reflection.Emit.VarArgMethod");
            varArgMethodFieldMethod = varArgMethodType.GetField("m_method", flags);
            varArgMethodFieldSignature = varArgMethodType.GetField("m_signature", flags);

            genericMethodInfoType = Type.GetType("System.Reflection.Emit.GenericMethodInfo");
            genericMethodInfoFieldMethodHandle = genericMethodInfoType.GetField("m_method", flags); //use m_methodHandle in .Net 4.0
            if (genericMethodInfoFieldMethodHandle == null)
                genericMethodInfoFieldMethodHandle = genericMethodInfoType.GetField("m_methodHandle", flags);
            genericMethodInfoFieldContext = genericMethodInfoType.GetField("m_context", flags);

            genericFieldInfoType = Type.GetType("System.Reflection.Emit.GenericFieldInfo", false);
            if (genericFieldInfoType != null)
            {
                genericFieldInfoFieldFieldHandle = genericFieldInfoType.GetField("m_field", flags); //use m_fieldHandle in .Net 4.0
                if (genericFieldInfoFieldFieldHandle == null)
                    genericFieldInfoFieldFieldHandle = genericFieldInfoType.GetField("m_fieldHandle", flags);
                genericFieldInfoFieldContext = genericFieldInfoType.GetField("m_context", flags);
            }
            else
            {
                genericFieldInfoFieldFieldHandle = genericFieldInfoFieldContext = null;
            }
        }        

        object scope = null;
        internal object this[int token]
        {
            get
            {
                return dynamicScopePropertyItem.GetValue(scope, new object[] { token });
            }
        }

        public DynamicScopeTokenResolver(DynamicMethod dm)
        {
            scope = dynamicILGeneratorFieldScope.GetValue(dm.GetILGenerator());
        }

        public String ResolveString(int token)
        {
            return this[token] as string;
        }

        public FieldInfo ResolveField(int token)
        {
            if (this[token] is RuntimeFieldHandle)
                return FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)this[token]);

            if (this[token].GetType() == genericFieldInfoType)
            {
                return FieldInfo.GetFieldFromHandle(
                        (RuntimeFieldHandle)genericFieldInfoFieldFieldHandle.GetValue(this[token]),
                        (RuntimeTypeHandle)genericFieldInfoFieldContext.GetValue(this[token]));
            }

            Debug.Assert(false, string.Format("unexpected type: {0}", this[token].GetType()));
            return null;
        }

        public Type ResolveType(int token)
        {
            return Type.GetTypeFromHandle((RuntimeTypeHandle)this[token]);
        }

        public MethodBase ResolveMethod(int token)
        {
            if (this[token] is DynamicMethod)
                return this[token] as DynamicMethod;

            if (this[token] is RuntimeMethodHandle)
                return MethodBase.GetMethodFromHandle((RuntimeMethodHandle)this[token]);

            if (this[token].GetType() == genericMethodInfoType)
                return MethodBase.GetMethodFromHandle(
                    (RuntimeMethodHandle)genericMethodInfoFieldMethodHandle.GetValue(this[token]),
                    (RuntimeTypeHandle)genericMethodInfoFieldContext.GetValue(this[token]));

            if (this[token].GetType() == varArgMethodType)
                return (MethodInfo)varArgMethodFieldMethod.GetValue(this[token]);

            Debug.Assert(false, string.Format("unexpected type: {0}", this[token].GetType()));
            return null;
        }

        public MemberInfo ResolveMember(int token)
        {
            if ((token & 0x02000000) == 0x02000000)
                return this.ResolveType(token);
            if ((token & 0x06000000) == 0x06000000)
                return this.ResolveMethod(token);
            if ((token & 0x04000000) == 0x04000000)
                return this.ResolveField(token);

            Debug.Assert(false, string.Format("unexpected token type: {0:x8}", token));
            return null;
        }

        public byte[] ResolveSignature(int token)
        {
            return this[token] as byte[];
        }
    }
}
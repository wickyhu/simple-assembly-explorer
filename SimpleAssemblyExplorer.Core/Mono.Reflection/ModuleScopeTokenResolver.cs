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
    public class ModuleScopeTokenResolver : ITokenResolver
    {
        private Module m_module;
        private MethodBase m_enclosingMethod;
        private Type[] m_methodContext;
        private Type[] m_typeContext;

        public ModuleScopeTokenResolver(MethodBase method)
        {
            m_enclosingMethod = method;
            m_module = method.Module;
            m_methodContext = (method is ConstructorInfo) ? null : method.GetGenericArguments();
            m_typeContext = (method.DeclaringType == null) ? null : method.DeclaringType.GetGenericArguments();
        }

        public MethodBase ResolveMethod(int token)
        {
            return m_module.ResolveMethod(token, m_typeContext, m_methodContext);
        }

        public FieldInfo ResolveField(int token)
        {
            return m_module.ResolveField(token, m_typeContext, m_methodContext);
        }

        public Type ResolveType(int token)
        {
            return m_module.ResolveType(token, m_typeContext, m_methodContext);
        }

        public MemberInfo ResolveMember(int token)
        {
            return m_module.ResolveMember(token, m_typeContext, m_methodContext);
        }

        public string ResolveString(int token)
        {
            return m_module.ResolveString(token);
        }

        public byte[] ResolveSignature(int token)
        {
            return m_module.ResolveSignature(token);
        }
    }
}
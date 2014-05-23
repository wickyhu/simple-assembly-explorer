using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mono.Reflection
{
    public interface ITokenResolver
    {
        MethodBase ResolveMethod(int token);
        FieldInfo ResolveField(int token);
        Type ResolveType(int token);
        String ResolveString(int token);
        MemberInfo ResolveMember(int token);
        byte[] ResolveSignature(int token);
    }
}


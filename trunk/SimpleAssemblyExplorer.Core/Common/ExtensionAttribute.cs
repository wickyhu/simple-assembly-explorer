
using System;

#if !NET_3_5 && !NET_4_0

namespace System.Runtime.CompilerServices
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ExtensionAttribute : Attribute
    {
    }
}

#endif

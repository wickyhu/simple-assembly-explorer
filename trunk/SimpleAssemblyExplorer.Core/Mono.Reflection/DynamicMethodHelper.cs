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

namespace Mono.Reflection
{
    public class DynamicMethodHelper
    {
        static Type dynamicMethodType;
        static Type rtDynamicMethodType;
        static FieldInfo rtDynamicMethodTypeOwner;

        static FieldInfo ilGeneratorFieldLength;
        static FieldInfo ilGeneratorFieldStream;
        static MethodInfo ilGeneratorMethodBakeByteArray;

        static DynamicMethodHelper() 
        {
            dynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod");
            rtDynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod+RTDynamicMethod");
            rtDynamicMethodTypeOwner = rtDynamicMethodType.GetField("m_owner", BindingFlags.Instance | BindingFlags.NonPublic);

            Type ilGeneratorType = typeof(ILGenerator);
            ilGeneratorFieldLength = ilGeneratorType.GetField("m_length", BindingFlags.NonPublic | BindingFlags.Instance);
            ilGeneratorFieldStream = ilGeneratorType.GetField("m_ILStream", BindingFlags.NonPublic | BindingFlags.Instance);
            ilGeneratorMethodBakeByteArray = ilGeneratorType.GetMethod("BakeByteArray", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static byte[] GetILAsByteArray(DynamicMethod method)
        {
            byte[] bytes;
            ILGenerator ilgen = method.GetILGenerator();

            try
            {
                bytes = (byte[])ilGeneratorMethodBakeByteArray.Invoke(ilgen, null);
                if (bytes == null) bytes = new byte[0];
            }
            catch (TargetInvocationException)
            {
                int length = (int)ilGeneratorFieldLength.GetValue(ilgen);
                bytes = new byte[length];
                Array.Copy((byte[])ilGeneratorFieldStream.GetValue(ilgen), bytes, length);
            }

            return bytes;
        }

        public static DynamicMethod GetDynamicMethod(MethodBase method)
        {
            DynamicMethod dm;
            if (IsRTDynamicMethod(method))
            {
                dm = rtDynamicMethodTypeOwner.GetValue(method) as DynamicMethod;
            }
            else
            {
                dm = method as DynamicMethod;
            }
            return dm;
        }

        public static bool IsDynamicMethod(MethodBase method)
        {
            return dynamicMethodType.Equals(method.GetType());
        }

        public static bool IsRTDynamicMethod(MethodBase method)
        {
            return rtDynamicMethodType.Equals(method.GetType());
        }

        public static bool IsDynamicOrRTDynamicMethod(MethodBase method)
        {
            Type type = method.GetType();
            return rtDynamicMethodType.Equals(type) || dynamicMethodType.Equals(type);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TestSample.DynamicMethodTest
{
    public class DynamicMethodTestClass
    {
        public class WriteLineClass
        {
            public void WriteLine(string msg)
            {
                if (this == null)
                    throw new InvalidProgramException();
                Console.WriteLine(msg);
            }
        }

        private delegate void WriteLineInvoker(string msg);
        private delegate void InstanceWriteLineInvoker(object instance, string msg);

        static DynamicMethod dmCallStaticWriteLine;
        static DynamicMethod dmCallInstanceWriteLine;
        
        static WriteLineInvoker staticWriteLineDelegate;
        static InstanceWriteLineInvoker instanceWriteLineDelegate;

        static DynamicMethodTestClass()
        {
            Type[] writeLineArgs = { typeof(string) };

            //dynamic method to call static writeline
            dmCallStaticWriteLine = new DynamicMethod("WriteLineStatic",
                null,
                writeLineArgs,
                typeof(DynamicMethodTestClass).Module);

            MethodInfo consoleWriteLine =
                typeof(Console).GetMethod("WriteLine", writeLineArgs);

            ILGenerator ilCallStatic = dmCallStaticWriteLine.GetILGenerator();
            ilCallStatic.Emit(OpCodes.Ldarg_0);
            ilCallStatic.EmitCall(OpCodes.Call, consoleWriteLine, null);
            ilCallStatic.Emit(OpCodes.Ret);

            staticWriteLineDelegate = (WriteLineInvoker)dmCallStaticWriteLine.CreateDelegate(typeof(WriteLineInvoker));

            //dynamic method to call instance writeline
            Type[] instanceWriteLineArgs = { typeof(object), typeof(string) };

            dmCallInstanceWriteLine = new DynamicMethod("WriteLineInstance",
                null,
                instanceWriteLineArgs,
                typeof(DynamicMethodTestClass).Module);

            MethodInfo instanceWriteLine =
                            typeof(WriteLineClass).GetMethod("WriteLine", writeLineArgs);

            ILGenerator ilCallInstance = dmCallInstanceWriteLine.GetILGenerator();
            ilCallInstance.Emit(OpCodes.Ldarg_0);
            ilCallInstance.Emit(OpCodes.Ldarg_1);
            ilCallInstance.EmitCall(OpCodes.Call, instanceWriteLine, null);
            ilCallInstance.Emit(OpCodes.Ret);

            instanceWriteLineDelegate = (InstanceWriteLineInvoker)dmCallInstanceWriteLine.CreateDelegate(typeof(InstanceWriteLineInvoker));

        }

        const string HelloWorld = "Hello, World!";

        //public void WriteLine(string msg)
        //{
        //    Console.WriteLine(msg);
        //}

        public void CallWriteLine10()
        {
            Console.WriteLine(HelloWorld);
        }

        public void CallWriteLine20()
        {
            WriteLineClass wlc = new WriteLineClass();
            wlc.WriteLine(HelloWorld);
        }

        public void CallDelegate10()
        {
            staticWriteLineDelegate(HelloWorld);
        }

        public void CallDelegate20()
        {
            WriteLineClass wlc = new WriteLineClass();
            instanceWriteLineDelegate(wlc, HelloWorld);
        }

        public void CallDelegate30()
        {
            object[] invokeArgs = { HelloWorld };
            dmCallStaticWriteLine.Invoke(null, invokeArgs);
        }

    }
}
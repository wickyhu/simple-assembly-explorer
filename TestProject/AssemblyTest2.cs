using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.LutzReflector;

namespace TestProject
{
    
    /// <summary>
    ///This is a test class for DeobfuscatorTest and is intended
    ///to contain all DeobfuscatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AssemblyTest2
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Notes:
        /// 1) Pattern: Invalid opcode
        /// 2) Branch, Unreachable
        /// </summary>
        [TestMethod()]
        public void Test021()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly021.exe" };

            options.ApplyFrom("Flow without Boolean Function");
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            TestUtils.CheckAndOutput(options.OutputFiles);
            Utils.DeleteFile(options.OutputFiles);
        }

        /// <summary>
        /// Notes:
        /// 1) Completely full test, no obfuscated comment
        /// 2) Foreach statement should be handled properly, no "using (enumerator ="
        /// 3) SmartAssembly global exception handler should be removed in second Go
        /// 4) Delegate Call removed
        /// </summary>
        [TestMethod()]
        public void Test022()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly022.dll" };
            options.ApplyFrom("Default");
            options.chkDelegateCallChecked = true;
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            string outputFile = options.OutputFiles[0];
            TestDeobfOptions options2 = new TestDeobfOptions();
            options2.SourceDir = Path.GetDirectoryName(outputFile);
            options2.Rows = new string[] { outputFile };
            options2.ApplyFrom("Flow without Boolean Function");
            options2.LoopCount = 2;
            options2.ExceptionHandlerFile = new ExceptionHandlerFile(ExceptionHandlerFile.Default.FileName);
            options2.ExceptionHandlerFile.Keywords.Add("NS004.c00003e::m000");
            TestDeobfuscator deobf2 = new TestDeobfuscator(options2);
            deobf2.Go();

            string outputFile2 = options2.OutputFiles[0];
            TestUtils.CheckAndOutput(new string[] { outputFile2 },
                new string[] { TestUtils.ObfuscatedCommentText, 
                    TestUtils.ObfuscatedForeachText }
                );

            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(outputFile2);
            string typeName = "NS012.c0000cb";
            TypeDefinition td = TestUtils.FindType(ad, typeName);
            Assert.IsNotNull(td, "Failed to find type: " + typeName);
            string methodName = "m00002e";
            MethodDefinition md = TestUtils.FindMethod(td, methodName,
                new string[] { "System.String" }, "System.String");
            Assert.IsNotNull(md, "Failed to find method: " + methodName);
            Collection<Instruction> instructions = md.Body.Instructions;            
            
            Instruction ins1 = instructions[1];
            Assert.AreEqual(Code.Ldstr, ins1.OpCode.Code, "Unexpected opcode " + ins1.OpCode.Code.ToString());
            Assert.AreEqual(@"Software\Red Gate\", ins1.Operand, "Unexpected operand " + ins1.Operand as string);
            
            Instruction ins2 = instructions[2];
            Assert.AreEqual(Code.Callvirt, ins2.OpCode.Code, "Unexpected opcode " + ins2.OpCode.Code.ToString());
            Assert.AreEqual(@"Microsoft.Win32.RegistryKey Microsoft.Win32.RegistryKey::OpenSubKey(System.String)", ins2.Operand.ToString(), "Unexpected operand " + ins2.Operand as string);

            Utils.DeleteFile(outputFile);
            Utils.DeleteFile(outputFile2);
        }

        /// <summary>
        /// Notes:
        /// 1) 9Ray (9Ray plugin not verified here)
        /// 2) Direct Call
        /// 3) Duplicated property name
        /// 4) String option: decode function return object but not string (not verified here)
        /// </summary>
        [TestMethod()]
        public void Test024()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly024.exe" };
            options.ApplyFrom("Name and String");
            options.chkDirectCallChecked = true;
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            string outputFile = options.OutputFiles[0];
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(outputFile);

            string typeName;
            TypeDefinition td;
            MethodDefinition md;

            //duplicate property name
            typeName = "NS014.c000031";
            td = TestUtils.FindType(ad, typeName);
            Assert.IsNotNull(td, "Failed to find type: " + typeName);

            Assert.AreEqual(5, td.Properties.Count);
            Assert.AreEqual("p000001", td.Properties[0].Name);
            Assert.AreEqual("p00002e", td.Properties[1].Name);
            Assert.AreEqual("p00002f", td.Properties[2].Name);
            Assert.AreEqual("p000030", td.Properties[3].Name);
            Assert.AreEqual("p000031", td.Properties[4].Name);

            //direct call
            typeName = "NS002.c000047";
            td = TestUtils.FindType(ad, typeName);
            Assert.IsNotNull(td, "Failed to find type: " + typeName);
            md = TestUtils.FindMethod(td, ".cctor", null, null);
            Assert.IsNotNull(td, "Failed to find .cctor: " + typeName);
            Collection<Instruction> ic = md.Body.Instructions;
            Assert.AreEqual(15, ic.Count);
            Assert.AreEqual("System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)", ic[6].Operand.ToString());
            Assert.AreEqual("System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)", ic[12].Operand.ToString());

            TestUtils.CheckAndOutput(options.OutputFiles);
            Utils.DeleteFile(options.OutputFiles);
        }

    } //end of class
}

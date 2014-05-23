using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssemblyExplorer;
using Mono.Cecil;

namespace TestProject
{
    
    /// <summary>
    ///This is a test class for DeobfuscatorTest and is intended
    ///to contain all DeobfuscatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GenericTest
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

        [TestMethod()]
        public void MethodRefDeclaredOnGenericsStaticAssembly()
        {
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(Global.TestSampleFile);
            ModuleDefinition module = ad.MainModule;
            MethodRefDeclaredOnGenerics(module);
        }


        [TestMethod()]
        public void MethodRefDeclaredOnGenericsDynamicCompiled()
        {
            string sourceFile = Path.Combine(Global.TestSampleDir, "Generics.cs");
            string assemblyFile = CompilationService.CompileResource(sourceFile);

            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assemblyFile);
            ModuleDefinition module = ad.MainModule;
            MethodRefDeclaredOnGenerics(module);

            Utils.DeleteFile(assemblyFile);
        }

        public void MethodRefDeclaredOnGenerics(ModuleDefinition module)
        {
            var type = module.GetType("TestSample.GenericTest.Tamtam");
            var beta = GetMethod(type, "Beta");
            var charlie = GetMethod(type, "Charlie");

            int index = 0;
            for (; index < beta.Body.Instructions.Count; index++)
            {
                if (beta.Body.Instructions[index].OpCode.Code == Mono.Cecil.Cil.Code.Newobj)
                    break;
            }
            var new_list_beta = (MethodReference)beta.Body.Instructions[index].Operand;
            var new_list_charlie = (MethodReference)charlie.Body.Instructions[index].Operand;

            Assert.AreEqual("System.Collections.Generic.List`1<TBeta>", new_list_beta.DeclaringType.FullName);
            Assert.AreEqual("System.Collections.Generic.List`1<TCharlie>", new_list_charlie.DeclaringType.FullName);
        }

        public MethodDefinition GetMethod(TypeDefinition type, string name)
        {
            foreach (MethodDefinition md in type.Methods)
            {
                if (md.Name == name) return md;
            }
            return null;
        }

    }
}

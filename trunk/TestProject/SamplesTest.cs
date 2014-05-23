using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssemblyExplorer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace TestProject
{
    
    /// <summary>
    ///This is a test class for DeobfuscatorTest and is intended
    ///to contain all DeobfuscatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SamplesTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DeobfuscateTestSample();
        }
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

        static TestDeobfuscator deobf = new TestDeobfuscator();
        static void DeobfuscateTestSample()
        {
            deobf.Options.Rows = new string[] { Global.TestSampleFile };
            deobf.Options.txtRegexText = "RenameMe";
            deobf.Options.chkDelegateCallChecked = true;
            deobf.Options.chkDirectCallChecked = true;
            deobf.Go();            
        }

        [TestMethod()]
        public void DirectCallTest()
        {
            string outputFile = deobf.Options.OutputFiles[0];

            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(outputFile);
            string typeName = "TestSample.DirectCallTest.DirectCallTestClass";
            TypeDefinition td = TestUtils.FindType(ad, typeName);
            Assert.IsNotNull(td, "Failed to find type: " + typeName);

            MethodDefinition md1 = TestUtils.FindMethod(td, "TestMethod", null, null);
            MethodDefinition md2 = TestUtils.FindMethod(td, "TestMethodCompare", null, null);
            Collection<Instruction> ic1 = md1.Body.Instructions;
            Collection<Instruction> ic2 = md2.Body.Instructions;
            int TotalCallCount = 4;
            for (int i = 0, j = 0, count = 0; count < TotalCallCount; )
            {
                if (ic1[i].OpCode.Code == Code.Nop)
                    i++;
                if (ic2[j].OpCode.Code == Code.Nop)
                    j++;
                if (ic1[i].OpCode.Code != Code.Nop && ic2[j].OpCode.Code != Code.Nop)
                {
                    Assert.AreEqual(ic2[j].OpCode, ic1[i].OpCode);
                    Assert.AreEqual(ic2[j].Operand, ic1[i].Operand);
                    
                    if(ic1[i].OpCode.Code == Code.Call)
                        count++;

                    i++;
                    j++;
                }
            }
        }


        [TestMethod()]
        public void DynamicMethodTest()
        {
            string outputFile = deobf.Options.OutputFiles[0];

            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(outputFile);
            string typeName = "TestSample.DynamicMethodTest.DynamicMethodTestClass";
            TypeDefinition td = TestUtils.FindType(ad, typeName);
            Assert.IsNotNull(td, "Failed to find type: " + typeName);

            CheckMethodLastCall(td, "CallDelegate10", @"System.Void System.Console::WriteLine(System.String)");
            CheckMethodLastCall(td, "CallDelegate20", @"System.Void TestSample.DynamicMethodTest.DynamicMethodTestClass/WriteLineClass::WriteLine(System.String)");
            CheckMethodLastCall(td, "CallDelegate30", @"System.Object System.Reflection.MethodBase::Invoke(System.Object,System.Object[])");
        }

        [TestMethod()]
        public void StringTest()
        {
            string outputFile = deobf.Options.OutputFiles[0];
            ModuleDefinition targetMd = AssemblyDefinition.ReadAssembly(outputFile).MainModule;
            foreach (TypeDefinition targetTd in targetMd.AllTypes)
            {
                if (targetTd.Name == "StringsTestClass")
                {
                    foreach (MethodDefinition md in targetTd.Methods)
                    {
                        if (md.IsStatic || md.IsConstructor) continue;
                        int count = 0;
                        foreach (Instruction ins in md.Body.Instructions)
                        {
                            if (ins.OpCode.Code == Code.Ldstr)
                            {
                                Assert.AreEqual(
                                    String.Format("String{0}", count + 1),
                                    (string)ins.Operand,
                                    String.Format("String option error in {0}", md.FullName));
                                count++;
                            }
                        }
                        Assert.AreEqual(3, count, String.Format("String option error in {0}", md.FullName));
                    }
                    break;
                }
            }
        }

        [TestMethod()]
        public void RenameMeTest()
        {
            string sourceFile = deobf.Options.Rows[0];
            string outputFile = deobf.Options.OutputFiles[0];

            TestPEVerifyOptions vo = new TestPEVerifyOptions();
            vo.Rows = new string[] { outputFile };
            TestUtils.PEVerify(vo);

            List<string> errors = new List<string>();
            ModuleDefinition sourceMd = AssemblyDefinition.ReadAssembly(sourceFile).MainModule;
            ModuleDefinition targetMd = AssemblyDefinition.ReadAssembly(outputFile).MainModule;
            foreach (TypeDefinition sourceTd in sourceMd.AllTypes)
            {
                if (!IsRenameMe(sourceTd.Name)) continue;

                string targetTypeName = GetTargetTypeName(deobf, sourceTd);
                if (targetMd.AllTypesDictionary.ContainsKey(targetTypeName))
                {
                    TypeDefinition targetTd = targetMd.AllTypesDictionary[targetTypeName];

                    CheckMembers(errors, sourceTd.Properties, targetTd.Properties, "p00", targetTypeName, "Property");
                    CheckMembers(errors, sourceTd.Fields, targetTd.Fields, "f00", targetTypeName, "Fields");
                    CheckMembers(errors, sourceTd.Methods, targetTd.Methods, "m00", targetTypeName, "Methods");
                    CheckMembers(errors, sourceTd.Events, targetTd.Events, "e00", targetTypeName, "Events");

                    CheckCustomAttributes(errors, sourceTd, targetTd, deobf);

                    //TODO: generic parameters ...
                }
                else
                {
                  AddError(errors, sourceTd.FullName, targetTypeName);
                }
            }

            Utils.ConsoleOutput(errors);
            Assert.AreEqual(0, errors.Count, "Failed to rename something!");            
        }

        #region RenameMe Utils
        private bool IsRenameMe(string name)
        {
            return name.Contains("RenameMe");
        }

        private void AddError(List<string> errors, string text)
        {
            errors.Add(text);
        }
        private void AddError(List<string> errors, string source, string target)
        {
            string errorFormat = "Failed to rename {0} to {1}.";
            errors.Add(String.Format(errorFormat, source, target == null ? "?" : target));
        }

        private void CheckMembers(List<string> errors,
            IList sourceMembers,
            IList targetMembers,
            string targetPrefix,
            string targetTypeName,
            string collectionName)
        {
            int sourceCount;
            int targetCount;

            sourceCount = 0; targetCount = 0;
            foreach (IMemberDefinition md in sourceMembers)
            {
                if (IsRenameMe(md.Name) && !md.IsSpecialName)
                    sourceCount++;
            }
            foreach (IMemberDefinition md in targetMembers)
            {
                if (IsRenameMe(md.Name))
                    AddError(errors, md.Name, null);

                if (md.Name.StartsWith(targetPrefix))
                    targetCount++;
            }
            if (sourceCount != targetCount)
                AddError(errors, String.Format("{0}: {1} count doesn't match ({2}!={3})!",
                    targetTypeName, collectionName, sourceCount, targetCount));
        }

        private string GetTargetTypeName(Deobfuscator deobf, TypeDefinition sourceTd)
        {
            return String.Format("NS001.{0}", deobf.GetNewTypeName(sourceTd));
        }

        private void CheckCustomAttributes(List<string> errors,
            TypeDefinition sourceTd,
            TypeDefinition targetTd,
            Deobfuscator deobf)
        {
            if (sourceTd.CustomAttributes.Count > 0)
            {
                for (int i = 0; i < sourceTd.CustomAttributes.Count; i++)
                {
                    CustomAttribute ca = sourceTd.CustomAttributes[i];
                    
                    for (int j = 0; j < ca.ConstructorArguments.Count; j++)
                    {
                        CustomAttributeArgument caa = ca.ConstructorArguments[j];
                        TypeDefinition valueType = caa.Value as TypeDefinition;
                        if (caa.Type.FullName == "System.Type" && IsRenameMe(valueType.Name))
                        {
                            string targetTypeName = GetTargetTypeName(deobf, valueType);
                            TypeDefinition targetValueType = targetTd.CustomAttributes[i].ConstructorArguments[j].Value as TypeDefinition;
                            if (targetTypeName != targetValueType.FullName)
                            {
                                errors.Add(String.Format("Unmatched custom attribute type \"{0}\"->\"{1}\".", valueType.FullName, targetValueType.FullName));
                            }
                        }
                    }

                    for (int j = 0; j < ca.Fields.Count; j++)
                    {
                        CustomAttributeNamedArgument cana = ca.Fields[j];
                        if (IsRenameMe(cana.Name))
                        {
                            string targetName = targetTd.CustomAttributes[i].Fields[j].Name;
                            if (cana.Name == targetName)
                            {
                                errors.Add(String.Format("Unrenamed custom attribute named argument found \"{0}\".", cana.Name));
                            }
                        }
                    }

                    for (int j = 0; j < ca.Properties.Count; j++)
                    {
                        CustomAttributeNamedArgument cana = ca.Properties[j];
                        if (IsRenameMe(cana.Name))
                        {
                            string targetName = targetTd.CustomAttributes[i].Properties[j].Name;
                            if (cana.Name == targetName)
                            {
                                errors.Add(String.Format("Unrenamed custom attribute named argument found \"{0}\".", cana.Name));
                            }
                        }
                    }

                    continue;
                }
            }

        }

        #endregion RenameMe Utils

        #region DynamicMethod Utils
        private void CheckMethodLastCall(TypeDefinition td, string methodName, string lastCallOperand)
        {
            MethodDefinition md = TestUtils.FindMethod(td, methodName, null, null);
            Collection<Instruction> instructions = md.Body.Instructions;
            Assert.IsNotNull(md, "Failed to find method: " + methodName);
            Instruction insCall = null;
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                if (instructions[i].OpCode.Code == Code.Callvirt ||
                    instructions[i].OpCode.Code == Code.Call)
                {
                    insCall = instructions[i];
                }
            }
            Assert.AreEqual(lastCallOperand, insCall.Operand.ToString());
        }
        #endregion DynamicMethod Utils

    }//end of class
}

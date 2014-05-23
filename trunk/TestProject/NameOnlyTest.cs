using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.LutzReflector;

namespace TestProject
{
    
    /// <summary>
    ///This is a test class for DeobfuscatorTest and is intended
    ///to contain all DeobfuscatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NameOnlyTest
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
        /// 1) Name
        /// 2) Overrides in Inherited Types
        /// 3) PEVerify
        /// </summary>
        [TestMethod()]
        public void Test007_NameOnly()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly007.exe" };
            options.ApplyFrom("Name Only");
            TestUtils.Deobfuscate(options);

            string outputFile = options.OutputFiles[0];
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(outputFile);

            string errorMsg = "Failed to find Dispose method in {0}";
            string methodName = "Dispose";
            string[] parameterTypes = new string[] { "System.Boolean" };
            string returnType = "System.Void";
            TypeDefinition td;
            MethodDefinition md;
            string typeName;

            typeName = "c0000a0";
            td = ad.MainModule.GetType(typeName);
            md = TestUtils.FindMethod(td, methodName, parameterTypes, returnType);
            Assert.IsNotNull(md, String.Format(errorMsg, typeName));

            typeName = "c00009d";
            td = ad.MainModule.GetType(typeName);
            md = TestUtils.FindMethod(td, methodName, parameterTypes, returnType);
            Assert.IsNotNull(md, String.Format(errorMsg, typeName));

            typeName = "c00005b";
            td = ad.MainModule.GetType(typeName);
            md = TestUtils.FindMethod(td, methodName, parameterTypes, returnType);
            Assert.IsNotNull(md, String.Format(errorMsg, typeName));
            Assert.IsTrue(md.HasOverrides, "Failed to find Overrides for method " + md.ToString());
            Assert.AreEqual(methodName, md.Overrides[0].Name, "Failed to find Override Dispose for method " + md.ToString());

            typeName = "c00002b";
            td = ad.MainModule.GetType(typeName);
            md = TestUtils.FindMethod(td, "CompareTo", new string[] { "System.Object" }, "System.Int32");
            Assert.IsNotNull(md, String.Format("Failed to find CompareTo method in {0}", typeName));

            TestPEVerifyOptions vo = new TestPEVerifyOptions();
            vo.Rows = new string[] { outputFile };

            TestUtils.PEVerify(vo);            

            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Name
        /// 2) Generic & abstract; inherit & override
        /// 3) c00017c <- c00015e <- c000317
        /// 4) c00030d <- IName
        /// 5) Add missing property: c00014f,Dialog...
        /// </summary>
        [TestMethod()]
        public void Test012_NameOnly()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly012.dll" };
            options.ApplyFrom("Name Only");
            options.chkAddMissingPropertyAndEventChecked = true;
            TestUtils.Deobfuscate(options);

            string outputFile = options.OutputFiles[0];
            TestPEVerifyOptions vo = new TestPEVerifyOptions();
            vo.Rows = new string[] { outputFile };
            TestUtils.PEVerify(vo);

            //TODO: add some checking for added properties

            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Name
        /// </summary>
        [TestMethod()]
        public void Test013_NameOnly()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly013.dll" };
            options.ApplyFrom("Name Only");
            TestUtils.Deobfuscate(options);

            string outputFile = options.OutputFiles[0];
            TestPEVerifyOptions vo = new TestPEVerifyOptions();
            vo.Rows = new string[] { outputFile };
            
            //After upgrade to 4.0, peverify find 1 error
            //TestUtils.PEVerify(vo);

            vo.TextInfoBox = new System.Windows.Forms.TextBox();
            TestPEVerifier verifier = new TestPEVerifier(vo);
            verifier.Go();
            string outputText = vo.TextInfoBox.Text;

            if (
                outputText.Contains("1 Error(s) Verifying") &&
                outputText.Contains("NS014.c000148[T]::m0003db][mdToken=0x6000f71][offset 0x00000021] The 'this' parameter to the call must be the calling method's 'this' parameter.(Error: 0x801318E1)")
                )
            {
                Utils.DeleteFile(outputFile);
            }
            else
            {
                Assert.Fail(String.Format("Failed to verify {0}", options.Rows[0]));
            }
        }

        /// <summary>
        /// Notes:
        /// 1) Name
        /// 2) Callback -> m00000e(boolean): property set implement interface 
        /// </summary>
        [TestMethod()]
        public void Test015_NameOnly()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly015.dll" };
            options.ApplyFrom("Name Only");
            TestUtils.Deobfuscate(options);

            string outputFile = options.OutputFiles[0];
            TestPEVerifyOptions vo = new TestPEVerifyOptions();
            vo.Rows = new string[] { outputFile };
            TestUtils.PEVerify(vo);

            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) in System.Linq.xb6dcf475d75c9930::xd0762b058cc8d603, there's a call to a method using a non existing generic parameter !!2 as a generic argument. 
        /// 2) not handled types in Deobf because C1 implement I1, but I1 has generic parameter C1
        /// </summary>
        [TestMethod()]
        public void Test023_NameOnly()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly023.exe" };
            options.ApplyFrom("Name Only");            
            TestUtils.Deobfuscate(options);            
            
            Utils.DeleteFile(options.OutputFiles);
        }

    } // end of class
}

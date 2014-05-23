using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssemblyExplorer;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace TestProject
{
    
    /// <summary>
    ///This is a test class for DeobfuscatorTest and is intended
    ///to contain all DeobfuscatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ValidNameTest
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
        public void ValidNameTest001()
        {
            string target;
            bool result;
            TestDeobfOptions options = new TestDeobfOptions();

            target = "delegate037";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "m0000a5";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "A4";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "runMethod2";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "get_AssemblyName";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "CloseAllOpeningFiles";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "UserCommentMaxLength";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "get_RefersToR1C1";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "CompareTo";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "Superscript";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "SyncRoot";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "CheckLayout";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "SW_MINIMIZE";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "CreateAQF1Completed";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);            
            
        }

        [TestMethod()]
        public void ValidNameTest002()
        {
            string target;
            bool result;
            TestDeobfOptions options = new TestDeobfOptions();

            target = "MAX_LENGTH";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "PDF417"; 
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "PDFGetFileName";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "SBConfiguration";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "ISBNumber";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "get_ISBNumber";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "GetISBNumber";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "GetASPXCode";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);
        }

        [TestMethod()]
        public void ValidNameTest003()
        {
            string target;
            bool result;
            TestDeobfOptions options = new TestDeobfOptions();

            target = "O6Lu9fRJkae1mNEaZw";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "OvsG5kiWa2ig952vSH";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "NqVmlsUVgAvZE5umojm";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "AGEPbiSxbxaFCCuINB";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "PDbM101lmyWjObGHZX";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "uERBbksmWQHZojOlDO";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "mikr9QhOsU7eoLY2vO";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "x20fdba2900edec1d";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);      
            
        }

        [TestMethod()]
        public void ValidNameTest004()
        {
            string target;
            bool result;
            TestDeobfOptions options = new TestDeobfOptions();         

            target = "Aj4QrTN"; //TODO?
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "AJLlWT"; //TODO?
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "A05qgyfk1";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "auTALepob"; //TODO?
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsTrue(result, target);

            target = "Rb6hGgFem";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "t97Xh2M3j";
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "LpHIjhc6t"; 
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "GpQNp4q1a"; 
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);

            target = "WGlD8fXCb"; 
            result = DeobfUtils.IsValidName(target, options);
            Assert.IsFalse(result, target);            
        }
    }//end of class
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
    public class AssemblyTest
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
        /// 1) .Net Reflector can't read attribute "cvi4cGFBAypGRlpQZB" of the method properly, so we remove it to let .Net Reflector can decompile the method
        /// 2) BranchDirections.TopDown is suitable for this method, the output of default setting is good 
        /// 3) String option is slow so we skip it
        /// </summary>
        [TestMethod()]
        public void Test001()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly001.exe" };

            options.ApplyFrom("Flow");
            options.chkRemoveAttributeChecked = true;
            options.AttributeFile = new AttributeFile(AttributeFile.Default.FileName);
            options.AttributeFile.AllLevelStrings.Add("cvi4cGFBAypGRlpQZB");
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            string outputFile = options.OutputFiles[0];
            TestUtils.CheckAndOutput(new string[] { outputFile },
                new string[] { TestUtils.ObfuscatedCommentText },
                new string[] { "AMGtDMpoFL3aEFIsnt", "HKE8WcJreWA12f9Lr8" }
                );

            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Resources saved in .reloc section can't be processed by Cecil 0.9, amended in ImageRead.cs
        /// 2) Output file will contains 14 methods couldn't be decompiled, it's caused by missing method arguments and local variables 
        /// </summary>
        [TestMethod()]
        public void Test002()
        {            
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly002.exe" };

            options.ApplyFrom("Default");
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) XenoCode assembly
        /// </summary>
        [TestMethod()]
        public void Test003()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly003.dll" };
            options.ApplyFrom("Default");
            options.LoopCount = 3;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) XenoCode assembly
        /// 2) Nop pop if it's first instruction of exceptopn handler (NOP.INVALID.POP of pattern file)
        /// </summary>
        [TestMethod()]
        public void Test004()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly004.dll" };
            options.ApplyFrom("Default");
            options.LoopCount = 2;
            options.PatternFile = new PatternFile(PatternFile.Default.FileName);
            options.PatternFile.NopInvalidPop = true;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) String and Pattern
        /// </summary>
        [TestMethod()]
        public void Test005()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly005.exe" };
            options.ApplyFrom("Default");
            options.LoopCount = 2;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) XenoCode
        /// 2) Pattern
        /// 3) Branch
        /// 4) Conditional Branch
        /// </summary>
        [TestMethod()]
        public void Test006()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly006.dll" };
            options.ApplyFrom("Default");
            options.LoopCount = 4;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }


        /// <summary>
        /// Notes:
        /// 1) RemoveAttribute
        /// 2) Flow options
        /// </summary>
        [TestMethod()]
        public void Test008()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly008.exe" };
            options.ApplyFrom("Name and Flow");
            options.chkRemoveAttributeChecked = true;
            options.LoopCount = 2;
            options.AttributeFile = new AttributeFile(AttributeFile.Default.FileName);
            options.AttributeFile.AllLevelStrings.Add("kv7DOvvcwZoqq2qW3V");            
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow options
        /// </summary>
        [TestMethod()]
        public void Test009()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly009.exe" };
            options.ApplyFrom("Name and Flow");
            options.LoopCount = 2;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow options
        /// </summary>
        [TestMethod()]
        public void Test010()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly010.dll" };
            options.ApplyFrom("Name and Flow");
            options.LoopCount = 2;
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow options
        /// </summary>
        [TestMethod()]
        public void Test011()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly011.dll" };
            options.ApplyFrom("Name and Flow");
            options.LoopCount = 2;
            
            TestUtils.Deobfuscate(options);
            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);

            int unexpected = 0;
            int expected = 0;

            foreach (MethodDeclarationInfo mdi in list)
            {
                switch (mdi.Name)
                {
                    case "c000071.m0000a9(c00005e) : enum072 (0x06000348)":
                        //Expression stack is empty at offset 0015
                        //use loop count 1 for this method ...
                        expected++;
                        break;
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected++;
                        break;
                }
            }

            TestUtils.AssertFail(1, expected, unexpected);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }


        /// <summary>
        /// Notes:
        /// 1) Default options
        /// 2) Ignored Type File
        /// </summary>
        [TestMethod()]
        public void Test014()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly014.exe" };
            options.ApplyFrom("Default");
            options.LoopCount = 3;
            options.IgnoredTypeFile.Regexes.Add(new Regex("DevExpress"));
            TestUtils.DeobfuscateAndCheck(options);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow options
        /// </summary>
        [TestMethod()]
        public void Test015()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly015.dll" };
            options.ApplyFrom("Name and Flow");
            options.LoopCount = 7;

            TestUtils.Deobfuscate(options);
            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);

            int unexpected = 0;
            int expected = 0;

            foreach (MethodDeclarationInfo mdi in list)
            {
                switch (mdi.Name)
                {
                    //case "<Module>.m000001(String, Int32) : String (0x06000001)":
                        //seems reflector bug, fixed in 7.4
                        //expected++;
                        //break;
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected++;
                        break;
                }
            }

            TestUtils.AssertFail(0, expected, unexpected);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow: Pattern, Boolean ...
        /// </summary>
        [TestMethod()]
        public void Test016()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly016.dll" };
            options.ApplyFrom("Name and Flow");
            options.LoopCount = 2;

            TestUtils.Deobfuscate(options);
            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);

            int unexpected = 0;
            int expected = 0;

            foreach (MethodDeclarationInfo mdi in list)
            {
                switch (mdi.Name)
                {
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected++;
                        break;
                }
            }

            TestUtils.AssertFail(0, expected, unexpected);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
            
            //There is a temp file generated and loaded, can't be deleted
            //string tempFile = Path.Combine(options.SourceDir, "HWISD_nat.dll");
            //Utils.DeleteFile(tempFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Flow options
        /// 2) Invalid Pop
        /// 3) Branch First and then Conditional Branch
        /// </summary>
        [TestMethod()]
        public void Test017()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly017.exe" };

            options.ApplyFrom("Flow without Boolean Function");
            options.LoopCount = 2;
            TestUtils.Deobfuscate(options);

            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);
            
            int unexpected = 0;
            int expected = 0;

            foreach (MethodDeclarationInfo mdi in list)
            {
                switch (mdi.Name)
                {
                    case "ak.g() : Int32 (0x060000a5)":
                    //case "ap.a(bv, ar&, String) : Void (0x0600042c)":
                        //nop invalid pop
                        expected ++;
                        break;
                    //case "ap.am(Object, EventArgs) : Void":
                    case "n.a(MailMessage, IPAddress, Boolean, ArrayList, String, String, String, Boolean, String, Hashtable, ah) : String[] (0x06000592)":
                    case "dd.a(DataSet, String, Boolean) : Void (0x060005fd)":
                        //flow without conditional branch 3 => conditional branch only 1
                        expected ++;
                        break;
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected ++;
                        break;
                }
            }

            TestUtils.AssertFail(3, expected, unexpected);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }


        /// <summary>
        /// Notes:
        /// 1) Flow
        /// 2) Goto statement target does not exist.
        /// 3) Expression stack is empty at xxx.
        /// 4) some no idea?? cases
        /// 
        /// Special Note: 
        /// if block move enabled, at least SchemaHelper.UpgradeFile will make Reflector infinite loop when decompiling
        /// </summary>
        [TestMethod()]
        public void Test018()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly018.dll" };
            options.ApplyFrom("Flow");
            options.LoopCount = 2;
            TestUtils.Deobfuscate(options);

            TestDeobfOptions options2 = new TestDeobfOptions();
            options2.Rows = options.OutputFiles;
            options2.SourceDir = Path.GetDirectoryName(options.OutputFiles[0]);
            options2.ApplyFrom("Flow without Boolean Function");
            options2.chkCondBranchUpChecked = true;
            options2.chkReflectorFixChecked = true;
            options2.LoopCount = 2;
            TestUtils.Deobfuscate(options2);

            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options2.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);
            
            int unexpected = 0;
            int expected = 0;

            foreach (MethodDeclarationInfo mdi in list)
            {
                switch (mdi.Name)
                {
                    //case "<Module>.a(String, Int32) : String (0x06000001)":
                        //seems reflector bug, fixed in 7.4
                        //expected++;
                        //break;
                    //case "l.a(String, String&) : String":
                        //use branch only 2, then +cond branch 2
                        //expected++;
                        //break;
                    case "b5.a(Object, Type, m, LicenseContext, StackTrace) : SecureLicense (0x06000949)":
                        //use branch only 2, make branch on instruction 124
                        expected++;
                        break;
                    case "aj.f(ParsedAttributeDictionary) : Void (0x06000ecd)":
                        //use branch 6, then you get "Expression stack is empty at xxx."
                        //second exception handler, nop 267 stloc.3
                        //it's not exactly correct becase local variable index 3 doesn't contains the current exception any more
                        expected++;
                        break;
                    case "Util.GetMergeType(String) : Type (0x060020ca)":
                        //use branch 6
                        expected++;
                        break;
                    case "AdoHelper<TFactory>.FillDataset(IDbCommand, DataSet, String[]) : Void (0x06001fa0)":
                        //use branch 6, then conditional branch 1, finally you can block move 1
                        expected++;
                        break;
                    case "ZipEntry.a(ZipFile) : ZipEntry (0x060011ba)":
                        //use default 2
                        expected++;
                        break;
                    case "aj.a() : Void (0x06000f03)":
                    case "cv.a(Int32) : Int32 (0x06000f7b)":
                    case "ZipEntry.a(ZipEntry, Encoding) : Boolean (0x06001228)":
                        //use block move option                        
                        expected++;
                        break;
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected++; 
                        break;
                }
            }

            TestUtils.AssertFail(8, expected, unexpected);

            Utils.DeleteFile(options.OutputFiles);
            Utils.DeleteFile(options2.OutputFiles);

        }


        /// <summary>
        /// Notes:
        /// 1) Remove attribute by token string 
        /// 2) recursive declaringtype:
        /// http://groups.google.com/group/mono-cecil/browse_thread/thread/b67aa54cc5d8f11
        /// http://code.google.com/p/simple-assembly-explorer/issues/detail?id=48
        /// </summary>
        [TestMethod()]
        public void Test019()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly019.dll" };

            options.ApplyFrom("Flow");
            options.chkRemoveAttributeChecked = true;
            options.AttributeFile = new AttributeFile(AttributeFile.Default.FileName);
            options.AttributeFile.AllLevelStrings.Add("0x02000068");
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            string outputFile = options.OutputFiles[0];
            TestUtils.CheckAndOutput(new string[] { outputFile });

            Utils.DeleteFile(outputFile);
        }

        /// <summary>
        /// Notes:
        /// 1) Name options
        /// 2) Flow -> Pattern + Branch + Conditional Branch
        /// </summary>
        [TestMethod()]
        public void Test020()
        {
            TestDeobfOptions options = new TestDeobfOptions();
            options.Rows = new string[] { "Assembly020.exe" };

            options.ApplyFrom("Default");
            options.chkAutoStringChecked = false;
            options.chkBoolFunctionChecked = false;
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();

            List<MethodDeclarationInfo> list = TestUtils.FindMethods(options.OutputFiles,
                new string[] { TestUtils.ObfuscatedCommentText },
                null);
            
            int unexpected = 0;
            int expected = 0;
            foreach (MethodDeclarationInfo mdi in list)
            {                
                switch (mdi.MethodDeclaration.Token)
                {
                    case 0x06000012:
                    //case 0x06000155:
                    case 0x06000199:
                    case 0x060001ce:
                    case 0x060001cf:
                    case 0x06000774:
                        //use branch only 
                        expected++;
                        break;
                    case 0x060003ad:
                    //case 0x06000413:
                        //use brach only 2 + conditional brach 2
                        expected++;
                        break;
                    default:
                        TestUtils.Output(mdi, null);
                        unexpected++;
                        break;
                }
            }

            TestUtils.AssertFail(6, expected, unexpected);

            string outputFile = options.OutputFiles[0];
            Utils.DeleteFile(outputFile);
        }

    } //end of class
}

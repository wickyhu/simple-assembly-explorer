using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.LutzReflector;
using Mono.Cecil;

namespace TestProject
{
    public class TestUtils
    {
        public const string ObfuscatedCommentText = "// This item is obfuscated and can not be translated.";
        public const string ObfuscatedForeachText = "using (enumerator =";

        public static SimpleReflector CreateReflector()
        {
            //Memory not released?
            //SimpleReflector reflector = new SimpleReflector();
            //reflector.FormatterType = typeof(TextFormatter);
            //return reflector;

            Type formatterType = typeof(TextFormatter);
            if (formatterType != SimpleReflector.Default.FormatterType)
            {
                SimpleReflector.Default.FormatterType = formatterType;
            }
            return SimpleReflector.Default;
        }

        public static void Deobfuscate(TestDeobfOptions options)
        {
            if (options == null || options.Rows == null || options.Rows.Length == 0) return;
            TestDeobfuscator deobf = new TestDeobfuscator(options);
            deobf.Go();
            if (deobf.Errors.Count > 0)
            {
                foreach (DeobfError de in deobf.Errors)
                {
                    Utils.ConsoleOutput("{0}\r\n\r\n", de.ToString());
                }
                Assert.AreEqual(0, deobf.Errors.Count, "Deobfuscate error found.");
            }
        }

        public static void DeobfuscateFlow(TestDeobfOptions options, MethodDefinition method)
        {
            if (options == null || options.Rows == null || options.Rows.Length == 0) return;
            TestDeobfuscator deobf = new TestDeobfuscator(options);

            deobf.DeobfFlow(options.Rows[0], method);
        }

        public static void PEVerify(TestPEVerifyOptions options)
        {
            if (options == null || options.Rows == null || options.Rows.Length == 0) 
                return;
            TestPEVerifier verifier = new TestPEVerifier(options);
            options.TextInfoBox = new TextBox();
            verifier.Go();

            ///All Classes and Methods in E:\....\Assembly007.exe Verified.
            //11 Error(s) Verifying E:\....\Assembly007.exe

            string outputText = options.TextInfoBox.Text;
            bool verified = !outputText.Contains(" Error(s) Verifying ") &&
                outputText.Contains("All Classes and Methods in ") && outputText.Contains(" Verified.");

            if (!verified)
            {
                Utils.ConsoleOutput(outputText);
            }
            
            Assert.IsTrue(verified, String.Format("Failed to verify {0}", options.Rows[0]));
        }

        public static void DeobfuscateAndCheck(TestDeobfOptions options)
        {
            DeobfuscateAndCheck(options, new string[] { ObfuscatedCommentText });
        }

        public static void DeobfuscateAndCheck(TestDeobfOptions options, string[] searchFors)
        {
            Deobfuscate(options);
            CheckAndOutput(options.OutputFiles, searchFors);
        }


        public static void CheckAndOutput(string[] files)
        {
            CheckAndOutput(files, new string[] { ObfuscatedCommentText }, null, null);
        }

        public static void CheckAndOutput(string[] files, string[] searchFors)
        {
            CheckAndOutput(files, searchFors, null, null);
        }

        public static void CheckAndOutput(string[] files, string[] searchFors, string[] searchForTypes)
        {
            CheckAndOutput(files, searchFors, searchForTypes, null);
        }

        public static void CheckAndOutput(string[] files, string[] searchFors, string[] searchForTypes, TextWriter tw)
        {
            if (files == null || files.Length == 0)
                return;

            List<MethodDeclarationInfo> list = FindMethods(files, searchFors, searchForTypes);

            Output(list, tw);

            Assert.AreEqual(0, list.Count, "Obfuscated methods found.");
        }

        public static void Output(List<MethodDeclarationInfo> list)
        {
            Output(list, null);
        }

        public static void Output(List<MethodDeclarationInfo> list, TextWriter tw)
        {
            if (list.Count > 0)
            {
                foreach (MethodDeclarationInfo item in list)
                {
                    Output(item, tw);
                }
            }
        }

        public static void Output(MethodDeclarationInfo mdi, TextWriter tw)
        {
            TextWriter output = tw;
            if (output == null)
                output = Console.Out;            
            output.WriteLine("{0}:\r\n{1}\r\n", 
                mdi.Name, 
                mdi.Body);
        }

        public static List<MethodDeclarationInfo> FindMethods(string[] files, string[] searchFors, string[] searchForTypes)
        {            
            SimpleReflector reflector = CreateReflector();
            try
            {
                return reflector.FindMethods(files, searchFors, searchForTypes, null, null);
            }
            catch
            {
                throw;
            }
            finally
            {
                foreach (string file in files)
                {
                    reflector.UnloadAssembly(file);
                }
            }
            
        }

        public static MethodDefinition FindMethod(TypeDefinition type, string methodName, string[] parameterTypes, string returnType)
        {
            foreach (MethodDefinition md in type.Methods)
            {
                if (md.Name != methodName) 
                    continue;
                
                if (parameterTypes != null)
                {
                    if (parameterTypes.Length != md.Parameters.Count) 
                        continue;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (md.Parameters[i].ParameterType.FullName != parameterTypes[i])
                            continue;
                    }
                }

                if (returnType != null)
                {
                    if (md.ReturnType.FullName != returnType)
                        continue;
                }

                return md;
            }

            return null;
        }

        public static TypeDefinition FindType(AssemblyDefinition ad, string typeName)
        {
            bool searchToken = typeName.StartsWith("0x02");
            if (searchToken)
            {
                typeName = typeName.Substring(2);
            }
            foreach (ModuleDefinition module in ad.Modules)
            {
                foreach (TypeDefinition td in module.AllTypes)
                {
                    if (searchToken)
                    {
                        string token = SimpleAssemblyExplorer.TokenUtils.GetFullMetadataTokenString(td.MetadataToken);
                        if (token == typeName)
                            return td;
                    }
                    else
                    {
                        if (td.FullName == typeName)
                            return td;
                    }
                }
            }

            return null;
        }

        public static MethodDefinition FindConstructor(TypeDefinition type)
        {
            foreach (MethodDefinition md in type.Methods)
            {
                if (md.IsConstructor && md.Parameters.Count == 0)
                {
                    return md;
                }
            }
            return null;
        }

        public static void AssertFail(int expected, int actual, int unexpected)
        {
            if (unexpected > 0 || actual != expected)
            {
                Assert.Fail(String.Format("{0} unexpected and {1} of {2} expected obfuscated method found.", unexpected, actual, expected));
            }
        }

    }//end of class
}

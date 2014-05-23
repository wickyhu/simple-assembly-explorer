using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleUtils;

namespace SimpleAssemblyExplorer
{
    public class Assembler : CommandTool 
    {
        public new AsmOptions Options
        {
            get { return (AsmOptions)base.Options; }
        }

        public Assembler()
            : this(new AsmOptions())
        {
        }

        public Assembler(AsmOptions options)
            : base(options)
        {
        }

        public override string ExeFile
        {
            get
            {
                return "ilasm.exe";
            }
        }        

        string sourceFile = String.Empty;
        bool deleteSourceFile = false;
   
        public override string PrepareArguments(string file)
        {
            sourceFile = String.Copy(file);

            string resFile = Path.ChangeExtension(sourceFile, ".res");
            if (!File.Exists(resFile)) resFile = String.Empty;

            string moduleName = GetModuleName(sourceFile);

            string type = Options.rbDllChecked ? "dll" : (Options.rbExeChecked ? "exe" : null);
            if (String.IsNullOrEmpty(type))
            {
                try
                {
                    type = Path.GetExtension(moduleName).Substring(1);
                }
                catch
                {
                    //moduleName = Path.ChangeExtension(sourceFile, ".exe");
                    type = "exe";
                }
            }

            //sometimes, module name may not same as original exe/dll name
            //here we assume user don't change il file name
            //so the il name should be same as original exe/dll name
            //
            //string outputFile = Path.Combine(outputDir, moduleName);
            string outputFile = Path.Combine(Options.OutputDir, Path.GetFileNameWithoutExtension(sourceFile)) + "." + type;

            deleteSourceFile = false;

            if (Options.rbRemoveStrongNameChecked)
            {
                if (StrongNameExists(sourceFile))
                {
                    string newFile = RemoveStrongName(sourceFile);
                    deleteSourceFile = true;
                    sourceFile = newFile;
                }
            }

            if (Options.chkRemoveLicenseProviderChecked)
            {
                string newFile = RemoveLicenseProvider(sourceFile);
                if (deleteSourceFile)
                {
                    File.Delete(sourceFile);
                }
                deleteSourceFile = true;
                sourceFile = newFile;
            }

            if (Options.chkReplaceTokenChecked)
            {
                string newFile = ReplaceReferencesToken(sourceFile);
                if (deleteSourceFile)
                {
                    File.Delete(sourceFile);
                }
                deleteSourceFile = true;
                sourceFile = newFile;

                //.resources files no backup ...
                string sourcePath = Path.GetDirectoryName(sourceFile);
                string[] resourcesFiles = Directory.GetFiles(sourcePath, "*.resources");
                if (resourcesFiles != null && resourcesFiles.Length > 0)
                {
                    foreach (string resourcesFile in resourcesFiles)
                    {
                        using (FileStream fs = new FileStream(resourcesFile, FileMode.Open, FileAccess.ReadWrite))
                        {
                            byte[] bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            ReplaceResourcesToken(bytes);
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\"{0}\" ", sourceFile);
            sb.AppendFormat("/{0} ", type);
            if (!Options.rbRemoveStrongNameChecked && !String.IsNullOrEmpty(Options.KeyFile))
            {
                sb.AppendFormat("/key=\"{0}\" ", Options.KeyFile);
            }
            sb.AppendFormat("/output=\"{0}\" ", outputFile);
            if (!String.IsNullOrEmpty(resFile))
            {
                sb.AppendFormat("/resource=\"{0}\" ", resFile);
            }            

            if (Options.chkQuietChecked) sb.Append("/quiet ");
            if (Options.rbDebugChecked) sb.Append("/debug ");
            if (Options.rbDebugImplChecked) sb.Append("/debug=impl ");
            if (Options.rbDebugOptChecked) sb.Append("/debug=opt ");
            if (Options.chkOptimizeChecked) sb.Append("/optimize ");
            if (Options.chkClockChecked) sb.Append("/clock ");
            if (Options.chkNoLogoChecked) sb.Append("/nologo ");
            if (Options.chkItaniumChecked) sb.Append("/pe64 /itanium ");
            if (Options.chkX64Checked) sb.Append("/pe64 /x64 ");

            if (!String.IsNullOrEmpty(Options.AdditionalOptions))
                sb.Append(Options.AdditionalOptions);

            return sb.ToString();
        }

        public override void OnProcessEnd(Process p)
        {
            if (deleteSourceFile && File.Exists(sourceFile))
            {
                File.Delete(sourceFile);
                deleteSourceFile = false;
            }

            base.OnProcessEnd(p);
        }

        private string GetModuleName(string file)
        {
            string module = null;
            StreamReader sr = new StreamReader(file);
            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(".module") && !line.StartsWith(".module extern"))
                    {
                        module = line.Substring(8).Trim();
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
            }

            if (module == null) return module;
            return module.Replace("'", "");
        }


        #region Strong Name
        private bool StrongNameExists(string file)
        {
            bool exists = false;
            StreamReader sr = new StreamReader(file);
            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(".assembly") && !line.StartsWith(".assembly extern"))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf(".publickey = (") >= 0)
                            {
                                exists = true;
                                break;
                            }
                            else if (line == "}")
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
            }
            return exists;
        }

        private string RemoveStrongName(string file)
        {
            string newFile = Path.ChangeExtension(file, ".NoSN.il");
            if (File.Exists(newFile)) File.Delete(newFile);

            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {

                sr = new StreamReader(file);
                sw = new StreamWriter(newFile, false, System.Text.Encoding.Unicode);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(".assembly") && !line.StartsWith(".assembly extern"))
                    {
                        sw.WriteLine(line);
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf(".publickey = (") >= 0)
                            {
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.IndexOf(".hash algorithm") >= 0)
                                        break;
                                }
                            }
                            else
                            {
                                sw.WriteLine(line);
                            }
                        }
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (sw != null) sw.Close();
            }
            return newFile;
        }
        #endregion Strong Name

        #region Replace Token
        private string GetAssemblyReferenceString(string token)
        {
            //.publickeytoken = (9B 17 1C 9F D6 4D A1 D1 )
            string s = token.Trim().ToUpper();

            StringBuilder sb = new StringBuilder(".publickeytoken = (");
            for (int i = 0; i < 8; i++)
            {
                sb.Append(s.Substring(i * 2, 2));
                sb.Append(" ");
            }
            sb.Append(")");

            return sb.ToString();
        }

        private string GetCAReferenceString(string token)
        {
            //PublicKeyToken=9b171c9fd64da1d1
            string s = token.Trim().ToLower();
            return String.Format("PublicKeyToken={0}", s);
        }

        private string GetCAInternalVisibleToReferenceString(string keyFile)
        {
            //PublicKey=00240000048000009400000006020000002400005253413100040000010001007b92cbd52d63863dde18c3fcbfcb11380ad0030031dec9f977ed149dcc4c802b4e6e9701c64bb6fd9a4031a2c8c02877b0a698c994a6c26fea913b2e01f52a0db7e6d00a4048ecab3f7de211a6c23cbf5d74eec6318c001395599ceb92a715ae9d4c1431fc31324c1163adfa623842aed9a70218ba9f0e9442c7a98e2c930f9e
            byte[] pubKey = TokenUtils.GetPublicKeyFromKeyFile(keyFile);
            string s = BytesUtils.BytesToHexString(pubKey);
            return String.Format("PublicKey={0}", s);
        }

        private string ReplaceReferencesToken(string file)
        {
            string newFile = Path.ChangeExtension(file, ".Repl.il");
            if (File.Exists(newFile)) File.Delete(newFile);

            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {
                string oldToken = Options.txtOldTokenText;
                string newToken = Options.txtNewTokenText;

                string oldStr1 = GetAssemblyReferenceString(oldToken);
                string newStr1 = GetAssemblyReferenceString(newToken);

                string oldStr2 = GetCAReferenceString(oldToken);
                string newStr2 = GetCAReferenceString(newToken);

                string newStr3 = GetCAInternalVisibleToReferenceString(Options.ReplKeyFile);
                Regex rgStr3 = new Regex(@"PublicKey=[0123456789abcdefABCDEF]*");

                string oldBin2 = BytesUtils.BytesToHexString(Encoding.ASCII.GetBytes(oldStr2), true);
                string newBin2 = BytesUtils.BytesToHexString(Encoding.ASCII.GetBytes(newStr2), true);

                string newBin3 = BytesUtils.BytesToHexString(Encoding.ASCII.GetBytes(newStr3), true);
                //PublicKey=50 75 62 6C 69 63 4B 65 79 3D
                Regex rgBin3 = new Regex(@"50 75 62 6C 69 63 4B 65 79 3D [\d\s]+");

                sr = new StreamReader(file);
                sw = new StreamWriter(newFile, false, System.Text.Encoding.Unicode);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(".assembly extern"))
                    {
                        sw.WriteLine(line);
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf(oldStr1) >= 0)
                                line = line.Replace(oldStr1, newStr1);

                            sw.WriteLine(line);

                            if (line == "}") break;
                        }
                    }//end of if .assembly extern
                    else if (line.StartsWith("  .custom"))
                    {
                        StringBuilder sb = new StringBuilder();
                        //string lineStart = null;
                        string lineEnd = null;
                        bool isBinary = true;

                        while (line != null)
                        {
                            //remove comment
                            int p = line.LastIndexOf(" //");
                            if (p >= 0) line = line.Substring(0, p);

                            line = line.Trim();
                            sb.Append(line);
                            sb.Append(" ");

                            if (lineEnd == null)
                            {
                                if (line.IndexOf("= {") >= 0)
                                {
                                    //lineStart = "= {";
                                    lineEnd = "}";
                                    isBinary = false;
                                }
                                else if (line.IndexOf("= (") >= 0)
                                {
                                    //lineStart = "= (";
                                    lineEnd = ")";
                                    isBinary = true;
                                }
                            }

                            if (lineEnd != null && line.EndsWith(lineEnd))
                                break;

                            line = sr.ReadLine();
                        }

                        line = sb.ToString();

                        if (isBinary)
                        {
                            if (line.IndexOf(oldBin2) > 0)
                            {
                                line = line.Replace(oldBin2, newBin2);
                            }
                            else
                            {
                                Match m = rgBin3.Match(line);
                                if (m.Success)
                                {
                                    string pubKeyString = m.Value.Trim().Substring("50 75 62 6C 69 63 4B 65 79 3D ".Length);
                                    pubKeyString = pubKeyString.Substring(0, pubKeyString.Length - 6); //remove " 00 00"
                                    byte[] pubKey = BytesUtils.HexStringToBytes(Encoding.ASCII.GetString(BytesUtils.HexStringToBytes(pubKeyString)));
                                    string token = TokenUtils.GetPublicKeyTokenString(TokenUtils.GetPublicKeyToken(pubKey, Mono.Cecil.AssemblyHashAlgorithm.SHA1));
                                    if (token.Equals(oldToken, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        line = rgBin3.Replace(line, newBin3 + "00 00 ");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (line.IndexOf(oldStr2) > 0)
                            {
                                line = line.Replace(oldStr2, newStr2);
                            }
                            else
                            {
                                Match m = rgStr3.Match(line);
                                if (m.Success)
                                {
                                    string pubKeyString = m.Value.Substring("PublicKey=".Length);
                                    byte[] pubKey = BytesUtils.HexStringToBytes(pubKeyString);
                                    string token = TokenUtils.GetPublicKeyTokenString(TokenUtils.GetPublicKeyToken(pubKey, Mono.Cecil.AssemblyHashAlgorithm.SHA1));
                                    if (token.Equals(oldToken, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        line = rgStr3.Replace(line, newStr3);
                                    }
                                }
                            }
                        }

                        sw.WriteLine(line);
                    }//end of if .custom
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (sw != null) sw.Close();
            }
            return newFile;
        }
        #endregion Replace Token

        private string RemoveLicenseProvider(string file)
        {
            string newFile = Path.ChangeExtension(file, ".NoLP.il");
            if (File.Exists(newFile)) File.Delete(newFile);

            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {

                sr = new StreamReader(file);
                sw = new StreamWriter(newFile, false, System.Text.Encoding.Unicode);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("  .custom")
                        && line.IndexOf("System.ComponentModel.LicenseProviderAttribute") > 0)
                    {
                        string lineEnd = null;

                        while (line != null)
                        {
                            //remove comment
                            int p = line.LastIndexOf(" //");
                            if (p >= 0) line = line.Substring(0, p);

                            line = line.Trim();

                            if (lineEnd == null)
                            {
                                if (line.IndexOf("= {") >= 0)
                                {
                                    //lineStart = "= {";
                                    lineEnd = "}";
                                    //isBinary = false;
                                }
                                else if (line.IndexOf("= (") >= 0)
                                {
                                    //lineStart = "= (";
                                    lineEnd = ")";
                                    //isBinary = true;
                                }
                            }

                            if (lineEnd != null && line.EndsWith(lineEnd))
                                break;

                            line = sr.ReadLine();
                        }
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (sw != null) sw.Close();
            }
            return newFile;
        }

        private void ReplaceResourcesToken(byte[] bytes)
        {
            string searchToken = "PublicKeyToken=" + Options.txtOldTokenText;
            byte[] searchTokenBytes = Encoding.UTF8.GetBytes(searchToken);

            string replaceToken = "PublicKeyToken=" + Options.txtNewTokenText;
            byte[] replaceTokenBytes = Encoding.UTF8.GetBytes(replaceToken);

            int index = BytesUtils.ByteSearch(bytes, searchTokenBytes, 0, bytes.Length);
            while (index > 0)
            {
                for (int i = 0; i < replaceTokenBytes.Length; i++)
                {
                    bytes[index + i] = replaceTokenBytes[i];
                }
                index = BytesUtils.ByteSearch(bytes, searchTokenBytes, index + searchTokenBytes.Length, bytes.Length - index - searchTokenBytes.Length);
            }
        }

    } //end of class
}

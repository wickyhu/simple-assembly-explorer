using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using SimpleUtils;
using SimpleUtils.Win;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public class StrongNamer 
    {
        public StrongNameOptions Options { get; set; }

        public StrongNamer()
            : this(new StrongNameOptions())
        {
        }

        public StrongNamer(StrongNameOptions options)
        {
            this.Options = options;
        }

        public void Go()
        {
            //bool resolveDirAdded = false;
            string[] rows = Options.Rows;
            try
            {
                //resolveDirAdded = Options.Host.AddAssemblyResolveDir(Options.SourceDir);

                if (Options.rbGacInstallChecked)
                {
                    GacUtil();
                }
                else if (Options.rbGacRemoveChecked)
                {
                    GacUtil();
                }
                else if (Options.rbRemoveChecked)
                {
                    RemoveSN();
                }
                else if (Options.rbSignChecked)
                {
                    Sign();
                }
                else
                {
                    SN();
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                _changedAssemblies.Clear();
                _allAssemblies.Clear();

                //if (resolveDirAdded)
                //    Options.Host.RemoveAssemblyResolveDir(Options.SourceDir);
            }
        }

        private void SN()
        {
            SN sn = new SN(this.Options);
            sn.Go();
        }

        private void GacUtil()
        {
            GacUtilOptions options = new GacUtilOptions();
            GacUtil u = new GacUtil(options);

            options.Host = Options.Host;
            options.Rows = Options.Rows;
            options.SourceDir = Options.SourceDir;
            options.TextInfoBox = Options.TextInfoBox;

            if (Options.rbGacInstallChecked) options.rbInstallChecked = true;
            else if (Options.rbGacRemoveChecked) options.rbUninstallChecked = true;
            
            u.Go();
        }

        Dictionary<string, int> _changedAssemblies = new Dictionary<string, int>();
        Dictionary<string, AssemblyDefinition> _allAssemblies = new Dictionary<string, AssemblyDefinition>();

        private void Sign()
        {
            SN sn = new SN(Options);
            sn.ShowStartTextInfo();

            string keyFile = Options.KeyFile;

            StrongNameKeyPair snkp = null;
            try
            {
                using (FileStream fs = new FileStream(keyFile, FileMode.Open, FileAccess.Read))
                {
                    snkp = new StrongNameKeyPair(fs);
                }

            }
            catch
            {
                throw;
            }

            string[] rows = Options.Rows;
            for (int i = 0; i < rows.Length; i++)
            {
                string file = PathUtils.GetFullFileName(rows, i, Options.SourceDir);
                Sign(file, snkp);
            }

            foreach (string file in _allAssemblies.Keys)
            {
                AssemblyDefinition ad = _allAssemblies[file];

                FixAssemblyNameReference(ad, snkp);

                ad = null;
            }

            //we free something to release memory, use lots of memory...
            _changedAssemblies.Clear();

            string[] keys = new string[_allAssemblies.Count];
            int count = 0;
            foreach (object key in _allAssemblies.Keys)
            {
                keys[count] = key as string;
                count++;
            }
            for (int i = 0; i < keys.Length; i++)
            {
                string file = keys[i];
                AssemblyDefinition ad = _allAssemblies[file];

                #region get output file
                string outputFile;

                if (Options.chkOverwriteOriginalFileChecked)
                    outputFile = file;
                else if (Options.SourceDir == Options.OutputDir)
                    outputFile = Path.ChangeExtension(file, ".SN" + Path.GetExtension(file));
                else
                    outputFile = Path.Combine(Options.OutputDir, Path.GetFileName(file));
                #endregion get output file

                Options.AppendTextInfo(String.Format("Saving assembly: {0}\r\n", ad.Name.Name));
                ad.Write(outputFile);

                Resign(outputFile, keyFile);

                _allAssemblies.Remove(file);
                ad = null;
            }

            sn.ShowCompleteTextInfo();
        }

        private void Sign(string file, StrongNameKeyPair sn)
        {
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file);
            AssemblyNameDefinition adName = ad.Name;
            _allAssemblies.Add(file, ad);

            adName.HashAlgorithm = AssemblyHashAlgorithm.SHA1;
            adName.PublicKey = sn.PublicKey;
            ad.Name.Attributes &= AssemblyAttributes.PublicKey;
            //foreach (ModuleDefinition md in ad.Modules)
            //{
            //    md.Image.CLIHeader.Flags &= ~Mono.Cecil.Binary.RuntimeImage.StrongNameSigned;
            //}

            _changedAssemblies.Add(adName.Name, 1);
            Options.AppendTextInfo(String.Format("Assembly signed: {0}\r\n", adName.FullName));

            Application.DoEvents();
        }

        private void RemoveSN()
        {
            SN sn = new SN(Options);            
            sn.ShowStartTextInfo();

            string[] rows = Options.Rows;
            for (int i = 0; i < rows.Length; i++)
            {
                string file = rows[i];
                RemoveSN(file);
            }

            foreach (string file in _allAssemblies.Keys)
            {
                AssemblyDefinition ad = _allAssemblies[file];

                FixAssemblyNameReference(ad, null);
            }

            _changedAssemblies.Clear();

            string[] keys = new string[_allAssemblies.Count];
            int count = 0;
            foreach (object key in _allAssemblies.Keys)
            {
                keys[count] = key as string;
                count++;
            }
            for (int i = 0; i < keys.Length; i++)
            {
                string file = keys[i];
                AssemblyDefinition ad = _allAssemblies[file];

                #region get output file
                string outputFile;

                if (Options.chkOverwriteOriginalFileChecked)
                    outputFile = file;
                else if (Options.SourceDir == Options.OutputDir)
                    outputFile = Path.ChangeExtension(file, ".NoSN" + Path.GetExtension(file));
                else
                    outputFile = Path.Combine(Options.OutputDir, Path.GetFileName(file));
                #endregion get output file

                Options.AppendTextInfo(String.Format("Saving assembly: {0}\r\n", ad.Name.Name));
                ad.Write(outputFile);

                _allAssemblies.Remove(file);
                ad = null;
            }

            sn.ShowCompleteTextInfo();
        }

        private void RemoveSN(string file)
        {
            AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file);
            AssemblyNameDefinition adName = ad.Name;
            _allAssemblies.Add(file, ad);

            if (adName.PublicKey != null && adName.PublicKey.Length > 0)
            {
                //ad.MainModule.Image.CLIHeader.Flags &= ~Mono.Cecil.Binary.RuntimeImage.StrongNameSigned;
                //ad.MainModule.Image.CLIHeader.StrongNameSignature.Size = 0;            
                //RVA rva = ad.MainModule.Image.CLIHeader.StrongNameSignature.VirtualAddress;
                //rva.Value = 0;

                SetAssemblyNamePublicKey(adName, null);
                foreach (ModuleDefinition module in ad.Modules)
                {
                    module.Attributes &= ~ModuleAttributes.StrongNameSigned; 
                }

                Options.AppendTextInfo(String.Format("Public Token removed: {0}\r\n", adName.FullName));

                if (!_changedAssemblies.ContainsKey(adName.Name))
                    _changedAssemblies.Add(adName.Name, 1);
            }
            else
            {
                Options.AppendTextInfo(String.Format("Public Token not found: {0}\r\n", adName.FullName));
            }

            Application.DoEvents();
        }

        private void SetAssemblyNamePublicKey(AssemblyNameReference anr, StrongNameKeyPair sn)
        {
            if (sn == null)
            {
                anr.Attributes &= ~AssemblyAttributes.PublicKey;
                anr.HashAlgorithm = AssemblyHashAlgorithm.None;
                anr.PublicKey = null;
            }
            else
            {
                anr.Attributes &= AssemblyAttributes.PublicKey;
                anr.HashAlgorithm = AssemblyHashAlgorithm.SHA1;
                //PublicKey must be null when save, or AssemblyNameReference saved wrong
                anr.PublicKey = null;
                anr.PublicKeyToken = TokenUtils.GetPublicKeyToken(sn.PublicKey, AssemblyHashAlgorithm.SHA1);
            }
        }

        private string FixInternalsVisibleToAttributeReference(string ps, string adName, byte[] pubKey)
        {
            string result = null;
            if (ps == adName || ps.StartsWith(adName + ","))
            {
                if (pubKey == null)
                    result = adName;
                else
                    result = String.Format("{0}, PublicKey={1}", adName, TokenUtils.GetReferenceTokenString(pubKey));
            }//searchName found
            return result;
        }
        private string FixReference(string ps, string adName, byte[] token)
        {
            string searchName = String.Format(", {0},", adName);
            string searchToken = "PublicKeyToken=";
            string searchNull = "null";
            string tokenString = TokenUtils.GetReferenceTokenString(token);
            string result = null;

            int index = ps.IndexOf(searchName);
            if (index >= 0)
            {
                index = ps.IndexOf(searchToken, index + searchName.Length);
                if (index > 0)
                {
                    index = index + searchToken.Length;
                    if (token == null)
                    {
                        if (ps.IndexOf(searchNull, index) == index)
                        {
                        }
                        else
                        {
                            result = String.Format("{0}null{1}", ps.Substring(0, index),
                                index + 16 >= ps.Length ? "" : ps.Substring(index + 16));
                        }
                    }
                    else
                    {
                        if (ps.IndexOf(searchNull, index) == index)
                        {
                            result = String.Format("{0}{1}{2}", ps.Substring(0, index),
                                tokenString,
                                index + searchNull.Length >= ps.Length ? "" : ps.Substring(index + searchNull.Length));
                        }
                        else
                        {
                            result = String.Format("{0}{1}{2}", ps.Substring(0, index),
                                tokenString,
                                index + tokenString.Length >= ps.Length ? "" : ps.Substring(index + tokenString.Length));
                        }
                    }
                }//searchToken found
            }//searchName found
            return result;
        }

        private void FixByteReference(byte[] bytes, string adName, byte[] token)
        {
            const int TOKEN_LEN = 16;

            string searchName = String.Format("{0}, Version=", adName);
            byte[] searchBytes = Encoding.UTF8.GetBytes(searchName);

            string searchToken = "PublicKeyToken=";
            byte[] searchTokenBytes = Encoding.UTF8.GetBytes(searchToken);

            string searchNull = "null";
            byte[] searchNullBytes = Encoding.UTF8.GetBytes(searchNull);

            string searchNullLong = "null            ";//16 bytes
            byte[] searchNullLongBytes = Encoding.UTF8.GetBytes(searchNullLong);

            string tokenString = TokenUtils.GetReferenceTokenString(token);
            byte[] tokenStringBytes = Encoding.UTF8.GetBytes(tokenString);

            int index = BytesUtils.ByteSearch(bytes, searchBytes, 0, bytes.Length);
            while (index > 0)
            {
                index = BytesUtils.ByteSearch(bytes, searchTokenBytes, index + searchBytes.Length, bytes.Length - index - searchBytes.Length);
                if (index > 0)
                {
                    index = index + searchTokenBytes.Length;

                    if (token == null)
                    {
                        if (BytesUtils.ByteSearch(bytes, searchNullBytes, index, searchNullBytes.Length) == index)
                        {
                        }
                        else
                        {
                            for (int i = 0; i < searchNullBytes.Length; i++)
                            {
                                bytes[index + i] = searchNullBytes[i];
                            }
                            index += searchNullBytes.Length;
                            for (int i = 0; i < TOKEN_LEN - searchNullBytes.Length; i++)
                            {
                                bytes[index + i] = 0x20;
                            }
                        }
                    }
                    else
                    {
                        if (BytesUtils.ByteSearch(bytes, searchNullBytes, index, searchNullBytes.Length) == index)
                        {
                            if (BytesUtils.ByteSearch(bytes, searchNullLongBytes, index, searchNullLongBytes.Length) == index)
                            {
                                for (int i = 0; i < tokenStringBytes.Length; i++)
                                {
                                    bytes[index + i] = tokenStringBytes[i];
                                }
                            }
                            else
                            {
                                #region complicated, found PublicKeyToken=null and need to extend space to sign, ignore now
                                /*
                                //1. extend bytes array
                                //2. amend bytes length for custom attribute or resource 
                                //
                                // below is old code for custom attribute only
                                {
                                    int endIndex = index + 4;
                                    int sizeIndex = 2;
                                    bool ok = false;
                                    if (endIndex - sizeIndex - 1 == (int)bytes[sizeIndex])
                                    {
                                        ok = true;
                                    }
                                    else
                                    {
                                        sizeIndex++;
                                        if (endIndex - sizeIndex - 1 == (int)bytes[sizeIndex])
                                        {
                                            ok = true;
                                        }
                                    }

                                    if (ok)//need to extend
                                    {
                                        byte[] newBytes = new byte[bytes.Length + 12];
                                        Array.Copy(bytes, newBytes, index);
                                        Array.Copy(tokenStringBytes, newBytes, tokenStringBytes.Length);
                                        for (int i = index + 4; i < bytes.Length; i++)
                                        {
                                            newBytes[i + 12] = bytes[i];
                                        }
                                        newBytes[sizeIndex] += 12;

                                        result = newBytes;
                                        bytes = newBytes;
                                    }
                                    else
                                    {
                                        endIndex = index + 16;
                                        sizeIndex = 2;
                                        ok = false;
                                        if (endIndex - sizeIndex - 1 == (int)bytes[sizeIndex])
                                        {
                                            ok = true;
                                        }
                                        else
                                        {
                                            sizeIndex++;
                                            if (endIndex - sizeIndex - 1 == (int)bytes[sizeIndex])
                                            {
                                                ok = true;
                                            }
                                        }

                                        if (ok) //enough room
                                        {
                                            for (int i = 0; i < tokenStringBytes.Length; i++)
                                            {
                                                bytes[index + i] = tokenStringBytes[i];
                                            }
                                        }
                                    }
                                }
                                */
                                #endregion complicated, found PublicKeyToken=null and need to extend space to sign
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tokenStringBytes.Length; i++)
                            {
                                bytes[index + i] = tokenStringBytes[i];
                            }
                        }
                    }

                    index = BytesUtils.ByteSearch(bytes, searchBytes, index + searchTokenBytes.Length, bytes.Length - index - searchTokenBytes.Length);
                }//end if searchToken found

            }//end if searchName found

        }

        private void FixCustomAttributeReference(Collection<CustomAttribute> cac, string adName, byte[] token, byte[] pubKey)
        {
            if (cac == null || cac.Count < 1) return;

            foreach (CustomAttribute ca in cac)
            {
                byte[] blob = ca.GetBlob();
                if (blob == null || blob.Length == 0)
                {
                    if (ca.Constructor.DeclaringType.Name == "InternalsVisibleToAttribute")
                    {
                        string ps = ca.ConstructorArguments[0].Value as string;
                        string result = FixReference(ps, adName, pubKey);
                        if (result != null)
                        {
                            //CustomAttributeArgument caa = ca.ConstructorArguments[0];
                            //caa.Value = result;
                            CustomAttributeUtils.SetConstructorArgumentValue(ca, 0, result);
                        }
                        continue;
                    }

                    for (int i = 0; i < ca.ConstructorArguments.Count; i++)
                    {
                        if (ca.ConstructorArguments[i].Value is string)
                        {
                            string ps = ca.ConstructorArguments[i].Value as string;
                            string result = FixReference(ps, adName, token);
                            if (result != null)
                            {
                                //CustomAttributeArgument caa = ca.ConstructorArguments[i];
                                //caa.Value = result;
                                CustomAttributeUtils.SetConstructorArgumentValue(ca, i, result);
                            }
                        }
                    }//each CustomAttribute
                }
                else
                {
                    //only handle easy case
                    //if (token == null || ca.Constructor.Parameters.Count == 1)
                    //{
                    FixByteReference(ca.GetBlob(), adName, token);
                    //}
                }
            }//end for each CustomAttribute
        }

        private void FixAssemblyNameReference(AssemblyDefinition ad, StrongNameKeyPair sn)
        {
            Options.AppendTextInfo(String.Format("Fixing assembly reference: {0}\r\n", ad.Name.FullName));
            foreach (string name in _changedAssemblies.Keys)
            {
                if (name == ad.Name.Name) continue;

                byte[] token = null;
                byte[] pubKey = null;

                if (sn != null)
                {
                    pubKey = sn.PublicKey;
                    token = TokenUtils.GetPublicKeyToken(pubKey, AssemblyHashAlgorithm.SHA1);
                }

                FixCustomAttributeReference(ad.CustomAttributes, name, token, pubKey);

                foreach (ModuleDefinition module in ad.Modules)
                {
                    foreach (AssemblyNameReference anr in module.AssemblyReferences)
                    {
                        if (anr.Name == name)
                        {
                            SetAssemblyNamePublicKey(anr, sn);
                            break;
                        }
                    }

                    foreach (TypeDefinition td in module.AllTypes)
                    {
                        FixCustomAttributeReference(td.CustomAttributes, name, token, pubKey);

                        foreach (PropertyDefinition pd in td.Properties)
                        {
                            FixCustomAttributeReference(pd.CustomAttributes, name, token, pubKey);
                        }
                        foreach (MethodDefinition method in td.Methods)
                        {
                            FixCustomAttributeReference(method.CustomAttributes, name, token, pubKey);
                        }
                        foreach (FieldDefinition fd in td.Fields)
                        {
                            FixCustomAttributeReference(fd.CustomAttributes, name, token, pubKey);
                        }
                        foreach (EventDefinition ed in td.Events)
                        {
                            FixCustomAttributeReference(ed.CustomAttributes, name, token, pubKey);
                        }

                    }

                    foreach (Resource r in module.Resources)
                    {
                        if (r is EmbeddedResource)
                        {
                            EmbeddedResource er = (EmbeddedResource)r;
                            FixByteReference(er.GetResourceData(), name, token);
                        }
                        else if (r is AssemblyLinkedResource)
                        {
                            AssemblyLinkedResource alr = (AssemblyLinkedResource)r;
                            if (alr.Assembly.Name == name)
                            {
                                SetAssemblyNamePublicKey(alr.Assembly, sn);
                            }
                        }
                        //else if (r is LinkedResource)
                        //{
                        //    LinkedResource lr = (LinkedResource)r;
                        //}
                    }

                }//end of each module
            }

            Application.DoEvents();
        }

        private void Resign(string file, string keyFile)
        {
            SN sn = new SN();
            sn.Options.SourceDir = Options.SourceDir;
            sn.Options.Host = Options.Host;
            sn.Options.TextInfoBox = Options.TextInfoBox;

            sn.Options.Rows = new string[] { file };
            sn.Options.rbRaChecked = true;
            sn.Options.KeyFile = keyFile;
            sn.Options.ShowStartCompleteTextInfo = false;
            sn.Go();
        }
        


    } //end of class
}

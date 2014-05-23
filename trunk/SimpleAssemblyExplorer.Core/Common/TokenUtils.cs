using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using SimpleUtils;
using SimpleUtils.Win;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;

namespace SimpleAssemblyExplorer
{
    public class TokenUtils
    {
        public static byte[] GetPublicKeyTokenFromKeyFile(string keyFile)
        {
            byte[] pubKey = GetPublicKeyFromKeyFile(keyFile);
            return GetPublicKeyToken(pubKey, AssemblyHashAlgorithm.SHA1);
        }

        public static byte[] GetPublicKeyFromKeyFile(string keyFile)
        {
            StrongNameKeyPair sn = null;
            using (FileStream fs = new FileStream(keyFile, FileMode.Open, FileAccess.Read))
            {
                sn = new StrongNameKeyPair(fs);
                return sn.PublicKey;
            }
        }

        public static byte[] GetPublicKeyToken(byte[] publicKey, AssemblyHashAlgorithm hashAlgo)
        {
            byte[] token = null;

            if (publicKey != null && publicKey.Length > 0)
            {
                HashAlgorithm ha;
                switch (hashAlgo)
                {
                    case AssemblyHashAlgorithm.Reserved:
                        ha = MD5.Create(); break;
                    default:
                        ha = SHA1.Create(); break;
                }
                byte[] hash = ha.ComputeHash(publicKey);
                // we need the last 8 bytes in reverse order
                token = new byte[8];
                Array.Copy(hash, (hash.Length - 8), token, 0, 8);
                Array.Reverse(token, 0, 8);
            }
            return token;
        }

        public static byte[] GetPublicKeyToken(AssemblyName an)
        {
            return an.GetPublicKeyToken();
        }

        public static byte[] GetPublicKeyToken(string fileName)
        {
            AssemblyName an = AssemblyName.GetAssemblyName(fileName);
            return GetPublicKeyToken(an);
        }

        public static string GetPublicKeyTokenString(AssemblyName an)
        {
            return GetPublicKeyTokenString(GetPublicKeyToken(an));
        }

        public static string GetPublicKeyTokenString(byte[] token)
        {
            return BytesUtils.BytesToHexString(token);
        }

        public static string GetPublicKeyTokenString(string fileName)
        {
            return GetPublicKeyTokenString(GetPublicKeyToken(fileName));
        }

        public static string GetFullMetadataTokenString(MetadataToken mt)
        {
            return UintToHexString(mt.ToUInt32(), 8);
        }

        public static string GetMetadataTokenString(MetadataToken mt)
        {
            return UintToHexString(mt.RID, 6);
        }

        public static string GetReferenceTokenString(byte[] token)
        {
            if (token != null && token.Length > 0)
            {
                return BytesUtils.BytesToHexString(token);
            }
            return "null";
        }

        public static string UintToHexString(uint ui, int digits)
        {
            return ui.ToString(String.Format("x0{0}", digits)).ToLower();
        }       

    }//end of class
}

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
    public class BytesUtils
    {
        public static string BytesToHexStringBlock(byte[] bytes)
        {
            if (bytes == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.AppendFormat("{0:x02} ", bytes[i]);
                if ((i + 1) % 16 == 0)
                {
                    sb.Append("\r\n");
                }
            }
            if (bytes.Length % 16 != 0)
            {
                sb.Append("\r\n");
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] >= 0x20 && bytes[i] <= 0x7e)
                {
                    sb.AppendFormat("{0}", (char)bytes[i]);
                }
                else
                {
                    sb.Append(".");
                }
                if ((i + 1) % 48 == 0)
                {
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        public static string BytesToHexString(byte[] bytes)
        {
            return BytesToHexString(bytes, false);
        }

        public static string BytesToHexString(byte[] bytes, bool space)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.AppendFormat("{0:x02}{1}", bytes[i], space ? " " : "");
            }
            return sb.ToString().ToUpperInvariant();
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            string hs = hexString.Replace(" ", "");
            int length = hs.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                string s = hs.Substring(i * 2, 2);
                bytes[i] = Byte.Parse(s, NumberStyles.HexNumber);
            }
            return bytes;
        }    
    
        public static int ByteSearch(byte[] bytes, byte[] search, int start, int length)
        {
            int index = -1;
            int end = start + length;

            if (start < 0) start = 0;
            if (end >= bytes.Length) end = bytes.Length;

            for (int i = start; i < end; i++)
            {
                if (end - i < search.Length) break;
                
                if (bytes[i] == search[0])
                {
                    bool match = true;
                    for (int j = 1; j < search.Length; j++)
                    {
                        if (i + j < end)
                        {
                            if (search[j] != bytes[i + j])
                            {
                                match = false;
                                break;
                            }
                        }
                        else
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        index = i;
                        break;
                    }
                }//end of start match
            }
            return index;
        }

        public static void ByteReplace(byte[] bytes, byte[] searchBytes, byte[] replaceBytes)
        {
            int start = 0;
            int index = ByteSearch(bytes, searchBytes, start, bytes.Length);
            while (index >= 0)
            {
                byte[] newBytes = new byte[bytes.Length + replaceBytes.Length - searchBytes.Length];
                for (int i = 0; i < index; i++)
                {
                    newBytes[i] = bytes[i];
                }
                for (int i = 0; i < replaceBytes.Length; i++)
                {
                    newBytes[i] = replaceBytes[i];
                }
                for (int i = start + searchBytes.Length; i < bytes.Length; i++)
                {
                    newBytes[i] = bytes[i];
                }
                bytes = newBytes;
                index = ByteSearch(bytes, searchBytes, start + replaceBytes.Length, bytes.Length);
            }
        }


    }//end of class
}

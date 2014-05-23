using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace SimpleUtils
{
    public class SimpleParse
    {
        public static sbyte ParseSByte(string byteString)
        {
            sbyte sb = 0;
            if (byteString.StartsWith("0x"))
            {
                sbyte.TryParse(byteString.Substring(2), NumberStyles.HexNumber, null, out sb);
            }
            else
            {
                sbyte.TryParse(byteString, out sb);
            }
            return sb;
        }

        public static byte ParseByte(string byteString)
        {
            byte b = 0;
            if (byteString.StartsWith("0x"))
            {
                byte.TryParse(byteString.Substring(2), NumberStyles.HexNumber, null, out b);
            }
            else
            {
                byte.TryParse(byteString, out b);
            }
            return b;
        }

        public static int ParseInt(string intString)
        {
            int i = 0;
            if (intString.StartsWith("0x"))
            {
                int.TryParse(intString.Substring(2), NumberStyles.HexNumber, null, out i);
            }
            else
            {
                int.TryParse(intString, out i);
            }
            return i;
        }

        public static long ParseLong(string longString)
        {
            long l = 0;
            if (longString.StartsWith("0x"))
            {
                long.TryParse(longString.Substring(2), NumberStyles.HexNumber, null, out l);
            }
            else
            {
                long.TryParse(longString, out l);
            }
            return l;
        }

        public static string ParseUnicodeString(string unicode)
        {
            //\u3b55\u3957\u3459\u3b5b\u325d\u055f\u1661\u0c63\u0f65\u1b67
            return ParseUnicodeString(unicode, "\\u", 4);
        }

        public static string ParseUnicodeString(string unicode, string prefix, int dataLength)
        {            
            StringBuilder sb = new StringBuilder();
            int prefixLength = prefix.Length;
            try
            {
                int i = 0;
                while (i + prefixLength + dataLength <= unicode.Length)
                {
                    Char c = (Char)UInt16.Parse(unicode.Substring(i + prefixLength, dataLength), NumberStyles.AllowHexSpecifier);
                    sb.Append(c);
                    i += prefixLength + dataLength;
                }
            }
            catch
            {
                throw;
            }
            return sb.ToString();
        }

    }//end of class
}

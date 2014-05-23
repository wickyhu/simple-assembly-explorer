using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUtils
{
    
    public class SimpleEncoding
    {
        public static UnicodeEncoding Encoding = new UnicodeEncoding();
    }

    /// <summary>
    /// This class comes from .net 1.1
    /// .net 2.0 cannot handle some illegal unicode characters
    /// </summary>
    public class UnicodeEncoding
    {
        private bool _bigEndian;
        public bool BigEndian
        {
            get { return _bigEndian; }
            set { _bigEndian = value; }
        }

        public UnicodeEncoding()
        {
            _bigEndian = false;
        }

        public UnicodeEncoding(bool bigEndian)
        {
            _bigEndian = bigEndian;
        }

        #region GetBytes
        public byte[] GetBytes(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s", ("ArgumentNull_String"));
            }
            char[] chars = s.ToCharArray();
            return this.GetBytes(chars, 0, chars.Length);
        }

        public byte[] GetBytes(char[] chars, int index, int count)
        {
            byte[] bytes = new byte[this.GetByteCount(chars, index, count)];
            this.GetBytes(chars, index, count, bytes, 0);
            return bytes;
        }

        public int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars", ("ArgumentNull_Array"));
            }
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", ("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((chars.Length - index) < count)
            {
                throw new ArgumentOutOfRangeException("chars", ("ArgumentOutOfRange_IndexCountBuffer"));
            }
            int num = count * 2;
            if (num < 0)
            {
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_GetByteCountOverflow"));
            }
            return num;
        }

        public int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            int count = charCount * 2;
            if ((chars == null) || (bytes == null))
            {
                throw new ArgumentNullException((chars == null) ? "chars" : "bytes", ("ArgumentNull_Array"));
            }
            if ((charIndex < 0) || (charCount < 0))
            {
                throw new ArgumentOutOfRangeException((charIndex < 0) ? "charIndex" : "charCount", ("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((chars.Length - charIndex) < charCount)
            {
                throw new ArgumentOutOfRangeException("chars", ("ArgumentOutOfRange_IndexCountBuffer"));
            }
            if ((byteIndex < 0) || (byteIndex > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("byteIndex", ("ArgumentOutOfRange_Index"));
            }
            if ((bytes.Length - byteIndex) < count)
            {
                throw new ArgumentException(("Argument_ConversionOverflow"));
            }
            if (this._bigEndian)
            {
                int num2 = charIndex + charCount;
                while (charIndex < num2)
                {
                    char ch = chars[charIndex++];
                    bytes[byteIndex++] = (byte)(ch >> 8);
                    bytes[byteIndex++] = (byte)ch;
                }
                return count;
            }
            Buffer.BlockCopy(chars, charIndex * 2, bytes, byteIndex, count);
            return count;
        }
        #endregion GetBytes

        #region GetString
        public string GetString(byte[] bytes, int index, int count)
        {
            return new string(this.GetChars(bytes, index, count));
        }

        public char[] GetChars(byte[] bytes, int index, int count)
        {
            char[] chars = new char[this.GetCharCount(bytes, index, count)];
            this.GetChars(bytes, index, count, chars, 0);
            return chars;
        }

        public int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes", ("ArgumentNull_Array"));
            }
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", ("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((bytes.Length - index) < count)
            {
                throw new ArgumentOutOfRangeException("bytes", ("ArgumentOutOfRange_IndexCountBuffer"));
            }
            return (count / 2);
        }

        public int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int num = byteCount / 2;
            if ((bytes == null) || (chars == null))
            {
                throw new ArgumentNullException((bytes == null) ? "bytes" : "chars", ("ArgumentNull_Array"));
            }
            if ((byteIndex < 0) || (byteCount < 0))
            {
                throw new ArgumentOutOfRangeException((byteIndex < 0) ? "byteIndex" : "byteCount", ("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((bytes.Length - byteIndex) < byteCount)
            {
                throw new ArgumentOutOfRangeException("bytes", ("ArgumentOutOfRange_IndexCountBuffer"));
            }
            if ((charIndex < 0) || (charIndex > chars.Length))
            {
                throw new ArgumentOutOfRangeException("charIndex", ("ArgumentOutOfRange_Index"));
            }
            if ((chars.Length - charIndex) < num)
            {
                throw new ArgumentException(("Argument_ConversionOverflow"));
            }
            byteCount = num * 2;
            if (this._bigEndian)
            {
                int num2 = byteIndex + byteCount;
                while (byteIndex < num2)
                {
                    int num3 = bytes[byteIndex++];
                    int num4 = bytes[byteIndex++];
                    chars[charIndex++] = (char)((num3 << 8) | num4);
                }
                return num;
            }
            Buffer.BlockCopy(bytes, byteIndex, chars, charIndex * 2, byteCount);
            return num;
        }
        #endregion GetBytes
    }//end of class
    
}

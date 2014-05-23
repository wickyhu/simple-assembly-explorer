using System;
using System.Collections.Generic;
using System.Text;

namespace TestSample.StringsTest
{
    public class StringsTestClass
    {
        public const string S1 = "String1";
        public const string S2 = "String2";
        public const string S3 = "String3";

        public static string GetString(string s)
        {
            switch (s)
            {
                case "0":
                    return S1;
                case "1":
                    return S2;
                default:
                    return S3;
            }
        }

        public static string GetString(int i)
        {
            switch (i)
            {
                case 0:
                case 0xa0:
                    return S1;
                case 1:
                case 0xa1:
                    return S2;
                default:
                    return S3;
            }
        }

        public static string GetString(string s, int i)
        {
            switch (s + i.ToString())
            {
                case "00":
                    return S1;
                case "11":
                    return S2;
                default:
                    return S3;
            }
        }

        delegate string GetStringDelegate(int i);

        static GetStringDelegate GetStringByDelegate;

        static StringsTestClass()
        {
            GetStringByDelegate = GetString;
        }


        public static object GetString0()
        {
            return GetString(0);
        }

        public static object GetString1()
        {
            return GetString(1);
        }

        public static object GetString2()
        {
            return GetString(2);
        }


        public string Method10()
        {
            return GetString("0") + GetString("1") + GetString("3");
        }

        public string Method20()
        {
            return GetString(0) + GetString(1) + GetString(3);
        }

        public string Method21()
        {
            return GetString(0xa0) + GetString(0xa1) + GetString(0xa3);
        }

        public string Method22()
        {
            return GetStringByDelegate(0xa0) + GetStringByDelegate(0xa1) + GetStringByDelegate(0xa3);
        }

        public string Method30()
        {
            return GetString("0", 0) + GetString("1", 1) + GetString("3", 3);
        }

        public string Method40()
        {
            return GetString("0") + GetString(1) + GetString("3", 3);
        }

        public string Method50()
        {
            return (string)GetString0() + (string)GetString1() + (string)GetString2();
        }

    }
}
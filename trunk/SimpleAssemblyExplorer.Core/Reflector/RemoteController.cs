
namespace SimpleAssemblyExplorer.LutzReflector
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class RemoteController
    {
        private static IntPtr targetWindow = IntPtr.Zero;

        private RemoteController()
        {
        }

        public static bool Available
        {
            get { return SendCopyDataMessage("Available\n4.0.0.0"); }
        }

        public static bool Select(string value)
        {
            return SendCopyDataMessage("Select\n" + value);
        }

        public static bool LoadAssembly(string fileName)
        {
            return SendCopyDataMessage("LoadAssembly\n" + fileName);
        }

        public static bool UnloadAssembly(string fileName)
        {
            return SendCopyDataMessage("UnloadAssembly\n" + fileName);
        }

        private static bool SendCopyDataMessage(string message)
        {
            targetWindow = IntPtr.Zero;
            NativeMethods.EnumWindows(new NativeMethods.EnumWindowsCallback(EnumWindow), 0);
            if (targetWindow != IntPtr.Zero)
            {
                char[] chars = message.ToCharArray();
                NativeMethods.CopyDataStruct data = new NativeMethods.CopyDataStruct();
                data.Padding = IntPtr.Zero;
                data.Size = chars.Length * 2;
                data.Buffer = Marshal.AllocHGlobal(data.Size);
                Marshal.Copy(chars, 0, data.Buffer, chars.Length);
                bool result = NativeMethods.SendMessage(targetWindow, 0x004A, IntPtr.Zero, ref data); // WM_COPYDATA
                Marshal.FreeHGlobal(data.Buffer);
                return result;
            }

            return false;
        }

        private static bool EnumWindow(IntPtr handle, int lparam)
        {
            StringBuilder titleBuilder = new StringBuilder(256);
            NativeMethods.GetWindowText(handle, titleBuilder, 256);
            string title = titleBuilder.ToString();
            //if (title.StartsWith("Lutz Roeder's .NET Reflector"))
            //{
            //    targetWindow = handle;
            //}
            //if (title.StartsWith("Red Gate's .NET Reflector"))
            //{
            //    targetWindow = handle;
            //}
            if (title.IndexOf(".NET Reflector") >= 0)
            {
                targetWindow = handle;
                return false;
            }
            return true;
        }

        private sealed class NativeMethods
        {
            public delegate bool EnumWindowsCallback(IntPtr hwnd, int lparam);

            private NativeMethods()
            {
            }

            [DllImport("user32.dll")]
            public static extern int EnumWindows(EnumWindowsCallback callback, int lparam);

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

            [DllImport("user32.dll")]
            public static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref CopyDataStruct lParam);

            public struct CopyDataStruct
            {
                public IntPtr Padding;
                public int Size;
                public IntPtr Buffer;

                public CopyDataStruct(IntPtr padding, int size, IntPtr buffer)
                {
                    this.Padding = padding;
                    this.Size = size;
                    this.Buffer = buffer;
                }
            }
        }
    }
}

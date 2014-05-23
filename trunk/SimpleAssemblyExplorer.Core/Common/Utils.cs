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
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleAssemblyExplorer
{
    public class Utils
    {

        #region IL Help
        static Dictionary<string,string> _il = null;
        
        public static string GetILHelp(string opCode)
        {            
            if (opCode == null) return String.Empty;
            if (_il == null) LoadILHelp();

            string s = null;
            if(_il.ContainsKey(opCode)) 
                s = _il[opCode];
            if (s == null) s = opCode;
            return s;
        }

        public static void LoadILHelp()
        {
            if (_il != null && _il.Count > 0 ) return;

            _il = new Dictionary<string, string>();
            using (Stream s = Assembly.GetEntryAssembly().GetManifestResourceStream("SimpleAssemblyExplorer.MSIL.txt"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        int p = line.IndexOf('=');
                        if (p > 0)
                        {
                            _il.Add(line.Substring(0, p), line.Substring(p + 1));
                        }
                        line = sr.ReadLine();
                    }                    
                }
            }

        }
        #endregion IL Help

        public static bool IsSystemType(string typeFullName)
        {
            if (String.IsNullOrEmpty(typeFullName))
                return false;
            string name;
            if (typeFullName.EndsWith("."))
                name = typeFullName;
            else
                name = String.Format("{0}.", typeFullName);
            return name.StartsWith("System.") || name.StartsWith("Microsoft.");
        }

        //Squall...@gmail.com.patch.start
        public static bool IsSystemMethod(MethodInfo method)
        {
            if (method.DeclaringType == null) 
                return false;
            string typeName = method.DeclaringType.FullName;
            if (Utils.IsSystemType(typeName))
                return true;
            //TODO: maybe check assembly name or metatoken in future 
            return false;
        }
        //Squall...@gmail.com.patch.end

        //public static string BackupFile(string file)
        //{
        //    string bakExt = String.Format(".bak.{0}{1}", DateTime.Now.ToString("yyyyMMdd.HHmmss"), Path.GetExtension(file));
        //    string bakFile = Path.ChangeExtension(file, bakExt);
        //    if (File.Exists(bakFile))
        //        File.Delete(bakFile);
        //    File.Copy(file, bakFile);
        //    return bakFile;
        //}

        public static void EnableUI(Control.ControlCollection controls, bool enabled)
        {
            foreach (Control c in controls)
            {
                if (c.Name == "txtInfo" || 
                    c.Name == "btnCancel" || 
                    c.Name == "btnClose" || 
                    c.Name == "btnStop")
                    continue;
                
                Type t = c.GetType();                
                
                if(t.Name == "Label" && !c.Font.Underline)
                    continue;
                if (t.Name == "Button" && (c.Text == "Stop" || c.Text == "Close" || c.Text == "Cancel"))
                    continue;
                
                if (t.Name == "DataGridView")
                {
                    if (c.Enabled == enabled)
                        continue;
                    if (enabled && c.Tag == null)
                        continue;

                    DataGridView dgv = (DataGridView)c;
                    dgv.Enabled = enabled;
                    
                    if (enabled)
                    {
                        dgv.Tag = null;
                        dgv.DefaultCellStyle.BackColor = SystemColors.Window;                        
                    }
                    else
                    {
                        dgv.Tag = true;
                        dgv.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    continue;
                }

                if (c.Controls.Count > 0)
                {
                    EnableUI(c.Controls, enabled);
                }
                else
                {
                    if (c.Enabled == enabled)
                        continue;
                    if (enabled && c.Tag == null)
                        continue;
                    
                    c.Enabled = enabled;

                    if (enabled)
                        c.Tag = null;
                    else
                        c.Tag = true;
                }
            }
        }

		public static string SerializeFont(Font font)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter serializer = new BinaryFormatter();
				serializer.Serialize(ms, font);
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		public static Font DeserializeFont(string serializedFont)
		{
			byte[] bytes = Convert.FromBase64String(serializedFont);
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				BinaryFormatter serializer = new BinaryFormatter();
				Font f = (Font)serializer.Deserialize(ms);
				return f;
			}
		}

    }//end of class
}

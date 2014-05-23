using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Drawing;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public enum PluginTypes
    {
        None,
        Main,
        Deobfuscator
    }

    public class PluginData
    {
        public string FileName { get; set; }
        public Bitmap Icon { get; set; }
        public PluginTypes PluginType { get; set; }
        public Type Type { get; set; }
        public string Version { get; set; }
        public IPluginBase PluginBase { get; set; }
    }

    public class PluginUtils
    {
        static Dictionary<string, PluginData> _plugins;

        public static Dictionary<string, PluginData> Plugins 
		{
			get { return _plugins; }
		}

        public static string PluginDir
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            }
        }       

		public static void InitPlugins(IHost host) 
		{
            _plugins = new Dictionary<string, PluginData>();

            if (!Directory.Exists(PluginDir))
            {
                return;
            }

			try 
			{
                string[] assemblies = Directory.GetFiles(PluginDir, "*.dll");
                
                Type pluginInterface = typeof(IPluginBase);
                Type mainPluginInterface = typeof(IMainPlugin);
                Type deobfPluinInterface = typeof(IDeobfPlugin);

                for (int i = 0; i < assemblies.Length; i++)
				{
                    string assemblyFile = assemblies[i];
                    host.SetStatusText(String.Format("Loading {0} ...", assemblyFile));

                    //Assembly a = Assembly.LoadFile(assemblyFile);
                    Assembly a = AssemblyUtils.LoadAssemblyFile(assemblyFile);
                    if (a == null) 
                        continue;

                    Type[] types = null;

                    try
                    {
                        types = a.GetExportedTypes();
                    }
                    catch
                    {
                        types = new Type[2];
                        int index = 0;
                        string fileName = Path.GetFileNameWithoutExtension(assemblyFile);
                        string typeName = fileName + ".Plugin";
                        Type t = a.GetType(typeName, false, true);
                        if (t != null)
                        {
                            types[index] = t;
                            index++;
                        }

                        typeName = fileName + ".DeobfPlugin";
                        t = a.GetType(typeName, false, true);
                        if (t != null)
                        {
                            types[index] = t;
                        }
                    }

                    if (types == null) 
                        continue;

                    foreach (Type type in types)
                    {
                        if (type == null) continue;
                        if (pluginInterface.IsAssignableFrom(type))
                        {
                            PluginData pd = new PluginData();
                            pd.FileName = assemblyFile;
                            pd.Type = type;
                            pd.Version = a.GetName().Version.ToString();
                            if (mainPluginInterface.IsAssignableFrom(type))
                            {
                                pd.PluginType = PluginTypes.Main;
                                pd.Icon = GetPluginIcon(type.FullName, a);
                                pd.PluginBase = (IPluginBase)Activator.CreateInstance(type, host);

                            }
                            else if (deobfPluinInterface.IsAssignableFrom(type))
                            {
                                pd.PluginType = PluginTypes.Deobfuscator;
                                pd.PluginBase = (IPluginBase)Activator.CreateInstance(type, host);
                            }
                            else
                            {
                                pd.PluginType = PluginTypes.None;
                            }
                            string title = pd.PluginBase.PluginInfoBase.Title;
                            _plugins.Add(title, pd);
                        }
                    }
                    continue;
				}
			}
			catch(Exception ex)
			{
                SimpleMessage.ShowException(ex);
			}

            host.SetStatusText(null);
		}

        private static Bitmap GetPluginIcon(string typeName, Assembly a)
        {
            Stream s = null;
            Bitmap bmp = null;
            string iconName = String.Format("{0}.", typeName);
            string resName = String.Empty;

            try
            {
                string[] resNames = a.GetManifestResourceNames();
                foreach (string rName in resNames)
                {
                    if (rName.StartsWith(iconName))
                    {
                        resName = rName;
                        s = a.GetManifestResourceStream(resName);
                        break;
                    }
                }

                if (s != null)
                {
                    if (PathUtils.IsIconExt(resName))
                    {
                        Icon ico = new Icon(s);
                        bmp = ico.ToBitmap();
                    }
                    else
                    {
                        bmp = Bitmap.FromStream(s) as Bitmap;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    s = null;
                }
            }
            return bmp;
        }

        public static void InitPluginGrid(DataGridView dgv)
        {
            DataTable dt = new DataTable("PluginList");
            dt.Columns.Add("Selected", typeof(bool));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Version", typeof(String));
            dt.Columns.Add("Author", typeof(String));

            foreach (PluginData pd in PluginUtils.Plugins.Values)
            {
                if (pd.PluginType != PluginTypes.Deobfuscator)
                    continue;

                PluginInfoBase pib = pd.PluginBase.PluginInfoBase;
                DataRow dr = dt.NewRow();
                dr["Selected"] = false;
                dr["Title"] = pib.Title;
                dr["Version"] = pd.Version;
                dr["Author"] = pib.Author;
                dt.Rows.Add(dr);
            }

            dgv.AutoGenerateColumns = false;
            dgv.DataSource = dt;
        }

        public static List<IDeobfPlugin> GetSelectedPluginFromGrid(DataGridView dgv)
        {
            List<IDeobfPlugin> list = new List<IDeobfPlugin>();
            DataTable dt = dgv.DataSource as DataTable;
            foreach (DataRow dr in dt.Rows)
            {
                if ((bool)dr["Selected"])
                {
                    string title = (string)dr["Title"];
                    list.Add(PluginUtils.Plugins[title].PluginBase as IDeobfPlugin);
                }
            }
            return list;
        }

        public static IDeobfPlugin GetSelectedPluginFromGrid(DataGridViewRow dgvr)
        {
            string title = dgvr.Cells["dgcTitle"].Value as string;
            IDeobfPlugin dp = PluginUtils.Plugins[title].PluginBase as IDeobfPlugin;
            return dp;
        }

    }//end of class
}

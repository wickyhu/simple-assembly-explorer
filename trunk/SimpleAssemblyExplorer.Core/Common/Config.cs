using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public class Property 
    {
        string _name;
        Type _type;
        object _defaultValue;

        public string Name { get { return _name; } }
        
        public Type Type { get { return _type; } }
        
        public object DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public Property(string name, Type type, object defaultValue)
        {
            _name = name;
            _type = type;
            _defaultValue = defaultValue;
        }
    }

    public class Config
    {
        static Dictionary<string, Property> _propertyList = new Dictionary<string, Property>();
        static Configuration _config;
        static ClientSettingsSection _css;

        public static void Init()
        {
            SimpleDotNet.ReportUrl = @"http://code.google.com/p/simple-assembly-explorer/issues/list?can=2";

            string configFile = Path.ChangeExtension(Application.ExecutablePath, ".exe.config");
            if (!File.Exists(configFile))
            {
                throw new ApplicationException(String.Format("Application config file is missing: {0}.", Path.GetFileName(configFile)));
            }

            if (!File.Exists("user.config"))
            {
                using (StreamWriter sw = new StreamWriter("user.config"))
                {
                    sw.WriteLine("<SimpleAssemblyExplorer.Properties.Settings>");
                    sw.WriteLine("</SimpleAssemblyExplorer.Properties.Settings>");
                }
            }

            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _css = (ClientSettingsSection)_config.SectionGroups["saeSettings"].Sections[0];

            AddProperty(Consts.ShowAbout, true, typeof(bool));
            AddProperty(Consts.CheckUpdatePeriod, 14, typeof(int));
            AddProperty(Consts.CheckUpdateEnabled, true, typeof(bool));
            AddProperty(Consts.CheckUpdateLastDate, new DateTime(2007, 1, 1), typeof(DateTime));
            AddProperty(Consts.ClassEditorAutoSaveBookmarkEnabled, true, typeof(bool));
            AddProperty(Consts.ClassEditorAutoOpenDroppedAssemblyEnabled, true, typeof(bool));
			AddProperty(Consts.ClassEditorRichTextBoxFont, String.Empty, typeof(String));
            AddProperty(Consts.ClassEditorBamlTranslator, 0, typeof(int));
            AddProperty(Consts.LastPath, String.Empty, typeof(String));
            AddProperty(Consts.LastSaveDir, String.Empty, typeof(String));
            AddProperty(Consts.LastBinDir, String.Empty, typeof(String));
            AddProperty(Consts.LastRegex, String.Empty, typeof(String));
            AddProperty(Consts.MarkBlocks, true, typeof(bool));
            AddProperty(Consts.SNOutputDir, String.Empty, typeof(String));
            AddProperty(Consts.DeobfProfile, 0, typeof(int));
            AddProperty(Consts.DeobfOutputDir, String.Empty, typeof(String));
            AddProperty(Consts.DeobfFlowOptionBranchLoopCount, 2, typeof(int));
            AddProperty(Consts.DeobfFlowOptionMaxMoveCount, 0, typeof(int));
            AddProperty(Consts.DeobfFlowOptionMaxRefCount, 2, typeof(int));
            AddProperty(Consts.DeobfFlowOptionBranchDirection, 0, typeof(int));            
            AddProperty(Consts.DasmOutputDir, String.Empty, typeof(String));
            AddProperty(Consts.DasmAdditionalOptions, String.Empty, typeof(String));
            AddProperty(Consts.AsmAdditionalOptions, String.Empty, typeof(String));
            AddProperty(Consts.PEVerifyAdditionalOptions, String.Empty, typeof(String));
            AddProperty(Consts.SNAdditionalOptions, String.Empty, typeof(String));
            AddProperty(Consts.AsmOutputDir, String.Empty, typeof(String));
            AddProperty(Consts.ClassEditorSaveAsDir, String.Empty, typeof(String));
            AddProperty(Consts.ClassEditorLastSearch, String.Empty, typeof(String));            
            AddProperty(Consts.StrongKeyFile, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerAppFile, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerAppArgument, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerAppFilter, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerAppLogPath, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerAppTraceEvent, false, typeof(bool));
            AddProperty(Consts.ProfilerAppTraceParameter, true, typeof(bool));
            AddProperty(Consts.ProfilerAppIncludeSystem, false, typeof(bool));
            AddProperty(Consts.ProfilerASPNetFilter, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerASPNetLogPath, String.Empty, typeof(String));
            AddProperty(Consts.ProfilerASPNetTraceEvent, false, typeof(bool));
            AddProperty(Consts.ProfilerASPNetTraceParameter, true, typeof(bool));
            AddProperty(Consts.ProfilerASPNetIncludeSystem, false, typeof(bool));
            AddProperty(Consts.RecentPluginList, 10, typeof(int));
            AddProperty(Consts.RecentPlugins, null, typeof(StringCollection));
        }

        public static void Save()
        {
            _config.Save(ConfigurationSaveMode.Minimal, true);
        }

        public static bool ContainsKey(string key)
        {
            return _css.Settings.Get(key) != null;
        }

        public static bool ContainsProperty(string propertyName)
        {
            return _propertyList.ContainsKey(propertyName);
        }

        private static bool IsStringCollection(Type t)
        {
            return t.Name == "StringCollection";
        }

        public static bool AddProperty(string propertyName, object defaultValue, Type propertyType)
        {
            bool added = false;

            if (!ContainsProperty(propertyName))
            {
                Property p = new Property(propertyName, propertyType, defaultValue);
                _propertyList.Add(propertyName, p);
                added = true;
            }

            if (!ContainsKey(propertyName))
            {
                if (IsStringCollection(propertyType))
                {
                    SettingElement se = new SettingElement(propertyName, SettingsSerializeAs.Xml);
                    SettingValueElement sve = new SettingValueElement();
                    XmlDocument doc = new XmlDocument();
                    XmlNode n = doc.CreateNode(XmlNodeType.Element, "value", String.Empty);
                    sve.ValueXml = n;
                    se.Value = sve;
                    _css.Settings.Add(se);
                    SetPropertyValue(propertyName, null);
                }
                else
                {
                    SettingElement se = new SettingElement(propertyName,SettingsSerializeAs.String);
                    SettingValueElement sve = new SettingValueElement();
                    XmlDocument doc = new XmlDocument();
                    XmlNode n = doc.CreateNode(XmlNodeType.Element, "value", String.Empty);
                    n.InnerText = (defaultValue == null ? String.Empty : defaultValue.ToString());
                    sve.ValueXml = n;
                    se.Value = sve;
                    _css.Settings.Add(se);
                }
                added = true;
            }
            
            return added;
        }

        public static void RemoveProperty(string propertyName)
        {
            if (ContainsProperty(propertyName))
            {
                _propertyList.Remove(propertyName);                
            }
            if (ContainsKey(propertyName))
            {
                SettingElement se = _css.Settings.Get(propertyName);
                _css.Settings.Remove(se);
            }
        }

        public static void SetPropertyValue(string key, object value)
        {
            if (!ContainsProperty(key)) return;

            Property p = _propertyList[key];
            SettingElement se = _css.Settings.Get(key);

            if (IsStringCollection(p.Type))
            {
                XmlDocument doc = new XmlDocument();
                XmlNode n = doc.CreateNode(XmlNodeType.Element, "ArrayOfString", String.Empty);
                if (value == null)
                {
                    XmlNode c = doc.CreateNode(XmlNodeType.Element, "string", String.Empty);
                    c.InnerText = String.Empty;
                    n.AppendChild(c);
                }
                else
                {
                    foreach (string s in (StringCollection)value)
                    {
                        XmlNode c = doc.CreateNode(XmlNodeType.Element, "string", String.Empty);
                        c.InnerText = s;
                        n.AppendChild(c);
                    }
                }
                se.Value.ValueXml.InnerXml = n.OuterXml;
            }
            else
            {
                se.Value.ValueXml.InnerText = (value == null ? String.Empty : value.ToString());
            }

        }

        public static DateTime GetDateTimeValue(string key)
        {
            try
            {
                return (DateTime)GetPropertyValue(key);
            }
            catch
            {
                return new DateTime(2007, 1, 1);
            }
        }

        public static object GetPropertyValue(string key)
        {
            if (!ContainsProperty(key)) return null;

            SettingElement se = _css.Settings.Get(key);
            Property p = _propertyList[key];

            if (p.Type.Name == "StringCollection")
            {
                StringCollection sc = new StringCollection();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(se.Value.ValueXml.InnerXml);
                XmlNodeList nl = doc.SelectNodes("/ArrayOfString/string");
                foreach (XmlNode n in nl)
                {
                    if (String.IsNullOrEmpty(n.InnerText)) continue;
                    sc.Add(n.InnerText);
                }
                return sc;
            }

            if (se.SerializeAs == SettingsSerializeAs.Xml)
                return se.Value.ValueXml.InnerXml.Trim();

            return Convert.ChangeType(se.Value.ValueXml.InnerText.Replace("\r\n","").Trim(), p.Type);
        }

        public readonly static Font DefaultFont = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        public static Font ClassEditorRichTextBoxFont
        {
            get
            {
                string s = Config.GetPropertyValue(Consts.ClassEditorRichTextBoxFont) as string;
                Font f = null;
                if (!String.IsNullOrWhiteSpace(s))
                {
                    try
                    {
                        f = Utils.DeserializeFont(s);
                    }
                    catch { }
                }
                if (f == null)
                {
                    f = DefaultFont;
                }
                return f;
            }
            set
            {
                Font f = value;
                if (f == null)
                    f = DefaultFont;
                
                string s;
                if (f.ToString() == DefaultFont.ToString())
                    s = String.Empty;
                else
                    s = Utils.SerializeFont(f);
                Config.SetPropertyValue(Consts.ClassEditorRichTextBoxFont, s);
            }
        }

        public static int ClassEditorBamlTranslator
        {
            get
            {
                return (int) Config.GetPropertyValue(Consts.ClassEditorBamlTranslator);
            }
            set
            {
                Config.SetPropertyValue(Consts.ClassEditorBamlTranslator, value);
            }
        }


        public static string LastPath
        {
            get
            {
                return Config.GetPropertyValue(Consts.LastPath) as string;
            }
            set
            {
                Config.SetPropertyValue(Consts.LastPath, value);
            }
        }

        public static string LastSaveDir
        {
            get
            {
                return GetPropertyValue(Consts.LastSaveDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.LastSaveDir, value);
            }
        }

        public static string LastBinDir
        {
            get
            {
                return GetPropertyValue(Consts.LastBinDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.LastBinDir, value);
            }
        }

        public static string LastRegex
        {
            get
            {
                return Config.GetPropertyValue(Consts.LastRegex) as string;
            }
            set
            {
                Config.SetPropertyValue(Consts.LastRegex, value);
            }
        }

        public static string SNOutputDir
        {
            get
            {
                return GetPropertyValue(Consts.SNOutputDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.SNOutputDir, value);
            }
        }

        public static string DeobfOutputDir
        {
            get
            {
                return GetPropertyValue(Consts.DeobfOutputDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.DeobfOutputDir, value);
            }
        }

        public static string DasmOutputDir
        {
            get
            {
                return GetPropertyValue(Consts.DasmOutputDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.DasmOutputDir, value);
            }
        }

        public static string DasmAdditionalOptions
        {
            get
            {
                return GetPropertyValue(Consts.DasmAdditionalOptions) as string;
            }
            set
            {
                SetPropertyValue(Consts.DasmAdditionalOptions, value);
            }
        }

        public static string AsmAdditionalOptions
        {
            get
            {
                return GetPropertyValue(Consts.AsmAdditionalOptions) as string;
            }
            set
            {
                SetPropertyValue(Consts.AsmAdditionalOptions, value);
            }
        }

        public static string PEVerifyAdditionalOptions
        {
            get
            {
                return GetPropertyValue(Consts.PEVerifyAdditionalOptions) as string;
            }
            set
            {
                SetPropertyValue(Consts.PEVerifyAdditionalOptions, value);
            }
        }

        public static string SNAdditionalOptions
        {
            get
            {
                return GetPropertyValue(Consts.SNAdditionalOptions) as string;
            }
            set
            {
                SetPropertyValue(Consts.SNAdditionalOptions, value);
            }
        }

        public static string AsmOutputDir
        {
            get
            {
                return GetPropertyValue(Consts.AsmOutputDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.AsmOutputDir, value);
            }
        }


        public static string ClassEditorLastSearch
        {
            get
            {
                return GetPropertyValue(Consts.ClassEditorLastSearch) as string;
            }
            set
            {
                SetPropertyValue(Consts.ClassEditorLastSearch, value);
            }
        }

        public static string ClassEditorSaveAsDir
        {
            get
            {
                return GetPropertyValue(Consts.ClassEditorSaveAsDir) as string;
            }
            set
            {
                SetPropertyValue(Consts.ClassEditorSaveAsDir, value);
            }
        }        

        public static string StrongKeyFile
        {
            get
            {
                return GetPropertyValue(Consts.StrongKeyFile) as string;
            }
            set
            {
                SetPropertyValue(Consts.StrongKeyFile, value);
            }
        }

        public static string ProfilerAppFile
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerAppFile) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppFile, value);
            }
        }

        public static string ProfilerAppArgument
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerAppArgument) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppArgument, value);
            }
        }

        public static string ProfilerAppFilter
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerAppFilter) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppFilter, value);
            }
        }

        public static string ProfilerAppLogPath
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerAppLogPath) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppLogPath, value);
            }
        }

        public static bool ProfilerAppTraceEvent
        {
            get
            {
                return (bool) GetPropertyValue(Consts.ProfilerAppTraceEvent);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppTraceEvent, value);
            }
        }

        public static bool ProfilerAppTraceParameter
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ProfilerAppTraceParameter);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppTraceParameter, value);
            }
        }

        public static bool ProfilerAppIncludeSystem
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ProfilerAppIncludeSystem);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerAppIncludeSystem, value);
            }
        }

        public static string ProfilerASPNetFilter
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerASPNetFilter) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerASPNetFilter, value);
            }
        }

        public static string ProfilerASPNetLogPath
        {
            get
            {
                return GetPropertyValue(Consts.ProfilerASPNetLogPath) as string;
            }
            set
            {
                SetPropertyValue(Consts.ProfilerASPNetLogPath, value);
            }
        }

        public static bool ProfilerASPNetTraceEvent
        {
            get
            {
                return (bool) GetPropertyValue(Consts.ProfilerASPNetTraceEvent);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerASPNetTraceEvent, value);
            }
        }

        public static bool ProfilerASPNetTraceParameter
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ProfilerASPNetTraceParameter);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerASPNetTraceParameter, value);
            }
        }

        public static bool ProfilerASPNetIncludeSystem
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ProfilerASPNetIncludeSystem);
            }
            set
            {
                SetPropertyValue(Consts.ProfilerASPNetIncludeSystem, value);
            }
        }

        public static int DeobfProfile
        {
            get
            {
                return (int)GetPropertyValue(Consts.DeobfProfile);
            }
            set
            {
                SetPropertyValue(Consts.DeobfProfile, value);
            }
        }

        public static int DeobfFlowOptionBranchLoopCount
        {
            get
            {
                return (int)GetPropertyValue(Consts.DeobfFlowOptionBranchLoopCount);
            }
            set
            {
                SetPropertyValue(Consts.DeobfFlowOptionBranchLoopCount, value);
            }
        }
        
        public static int DeobfFlowOptionMaxMoveCount
        {
            get
            {
                return (int)GetPropertyValue(Consts.DeobfFlowOptionMaxMoveCount);
            }
            set
            {
                SetPropertyValue(Consts.DeobfFlowOptionMaxMoveCount, value);
            }
        }

        public static int DeobfFlowOptionBranchDirection
        {
            get
            {
                return (int)GetPropertyValue(Consts.DeobfFlowOptionBranchDirection);
            }
            set
            {
                SetPropertyValue(Consts.DeobfFlowOptionBranchDirection, value);
            }
        }

        public static int DeobfFlowOptionMaxRefCount
        {
            get
            {
                return (int)GetPropertyValue(Consts.DeobfFlowOptionMaxRefCount);
            }
            set
            {
                SetPropertyValue(Consts.DeobfFlowOptionMaxRefCount, value);
            }
        }

        public static string Reflector
        {
            get
            {
                return GetPropertyValue(Consts.Reflector) as string;
            }
            set
            {
                SetPropertyValue(Consts.Reflector, value);
            }
        }

        public static int RecentPluginList
        {
            get
            {
                return (int) GetPropertyValue(Consts.RecentPluginList);
            }
            set
            {
                SetPropertyValue(Consts.RecentPluginList, value);
            }
        }
        
        public static bool MarkBlocks
        {
            get
            {
                return (bool)GetPropertyValue(Consts.MarkBlocks);
            }
            set
            {
                SetPropertyValue(Consts.MarkBlocks, value);
            }
        }

        public static bool CheckUpdateEnabled
        {
            get
            {
                return (bool)GetPropertyValue(Consts.CheckUpdateEnabled);
            }
            set
            {
                SetPropertyValue(Consts.CheckUpdateEnabled, value);
            }
        }

        public static bool ClassEditorAutoSaveBookmarkEnabled
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ClassEditorAutoSaveBookmarkEnabled);
            }
            set
            {
                SetPropertyValue(Consts.ClassEditorAutoSaveBookmarkEnabled, value);
            }
        }

        public static bool ClassEditorAutoOpenDroppedAssemblyEnabled
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ClassEditorAutoOpenDroppedAssemblyEnabled);
            }
            set
            {
                SetPropertyValue(Consts.ClassEditorAutoOpenDroppedAssemblyEnabled, value);
            }
        }

        public static bool ShowAbout
        {
            get
            {
                return (bool)GetPropertyValue(Consts.ShowAbout);
            }
            set
            {
                SetPropertyValue(Consts.ShowAbout, value);
            }
        }

        public static int CheckUpdatePeriod
        {
            get
            {
                return (int) GetPropertyValue(Consts.CheckUpdatePeriod);
            }
            set
            {
                SetPropertyValue(Consts.CheckUpdatePeriod, value);
            }
        }

        public static DateTime CheckUpdateLastDate
        {
            get
            {
                return GetDateTimeValue(Consts.CheckUpdateLastDate);
            }
            set
            {
                SetPropertyValue(Consts.CheckUpdateLastDate, value);
            }
        }


        public static void AddRecentPlugin(string pluginName)
        {
            if (RecentPlugins.Contains(pluginName))
            {
               RecentPlugins.Remove(pluginName);
            }
            RecentPlugins.Insert(0, pluginName);

            SetPropertyValue(Consts.RecentPlugins, RecentPlugins);            
        }

        static StringCollection _recentPlugins = null;

        public static StringCollection RecentPlugins
        {
            get
            {
                if (_recentPlugins == null)
                {
                    _recentPlugins = (StringCollection) GetPropertyValue(Consts.RecentPlugins);
                }
                return _recentPlugins;
            }
        }
    }//end of class
}

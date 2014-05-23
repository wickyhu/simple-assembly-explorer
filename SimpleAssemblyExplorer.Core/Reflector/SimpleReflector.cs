using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Reflector;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer.LutzReflector
{  

    public class SimpleReflector : IDisposable
    {
        #region static instance
        static SimpleReflector _default;

        public static Version Version
        {
            get
            {
                return typeof(ApplicationManager).Assembly.GetName().Version;
            }
        }

        public const string OptimizationDefault = "2.0";
        static List<string> _optimizationList = new List<string>(new string[] { "None", "1.0", "2.0", "3.5", "4.0", "4.5" });

        public string Optimization {
            get
            {
                return this.ConfigurationManager["Disassembler"].GetProperty("Optimization", OptimizationDefault);
            }
            set
            {
                string current = this.ConfigurationManager["Disassembler"].GetProperty("Optimization", OptimizationDefault);
                if (current == value) return;
                if (_optimizationList.Contains(value))
                    SetOptimization(value);
                else
                    SetOptimization(OptimizationDefault);                
                ReloadAssemblies();
            }
        }
        public static List<string> OptimizationList
        {
            get { return _optimizationList; }
        }

        private void SetOptimization(string op)
        {
            //this._langurageWriterConfiguration["Optimization"] = op; //don't set this, when optimization=4.0, something's wrong
            this.ConfigurationManager["Disassembler"].SetProperty("Optimization", op, OptimizationDefault);
        }

        public static SimpleReflector Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new SimpleReflector(), null);
                }
                return _default;
            }
        }

        public void ReloadAssemblies()
        {
            IAssembly[] assemblies = new IAssembly[_assemblyManager.Assemblies.Count];
            for (int i = 0; i < assemblies.Length; i++)
            {
                assemblies[i] = _assemblyManager.Assemblies[i];
            }
            for (int i = 0; i < assemblies.Length; i++)
            {
                string location = assemblies[i].Location;
                _assemblyManager.Unload(assemblies[i]);
                _assemblyManager.LoadFile(location);
            }
        }

        public static string OpenReflector()
        {
            return OpenReflector(null);
        }

        public static string OpenReflector(string initDir)
        {
            string path = SimpleDialog.OpenFile("Where is .Net Reflector?", "Reflector.exe|Reflector.exe|Exe files (*.exe)|*.exe", ".exe", true, initDir);
            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                Config.Reflector = path;
            }
            return path;
        }

        #endregion static instance

        #region Constructor
        ApplicationManager _applicationManager;
        IServiceProvider _serviceProvider;
        LanguageWriterConfiguration _langurageWriterConfiguration;
        Type _formatterType;        
        string _language;
        int _languageIndex;

        public SimpleReflector()
        {
            _applicationManager = new ApplicationManager(null);            
            _serviceProvider = _applicationManager;

            _langurageWriterConfiguration = new LanguageWriterConfiguration();
            _langurageWriterConfiguration.Visibility = this.VisibilityConfiguration;

            _langurageWriterConfiguration["ShowCustomAttributes"] = "true";
            _langurageWriterConfiguration["ShowNamespaceImports"] = "true";
            _langurageWriterConfiguration["ShowNamespaceBody"] = "true";
            _langurageWriterConfiguration["ShowTypeDeclarationBody"] = "true";
            _langurageWriterConfiguration["ShowMethodDeclarationBody"] = "true";
            _langurageWriterConfiguration["ShowDocumentation"] = "false";

            IConfiguration configAssemblyBrowser = this.ConfigurationManager["AssemblyBrowser"];
            configAssemblyBrowser.SetProperty("AutoResolve", "false");
            configAssemblyBrowser.SetProperty("FlattenNamespaces", "false");
            configAssemblyBrowser.SetProperty("ShowInheritedMembers", "false");
            configAssemblyBrowser.SetProperty("SideBySideVersioning", "false");
            configAssemblyBrowser.SetProperty("Visibility", "*"); //*,PublicOnly

            IConfiguration configDisassembler = this.ConfigurationManager["Disassembler"];
            configDisassembler.SetProperty("ShowSymbols", "true");
            configDisassembler.SetProperty("Indentation", "4");
            configDisassembler.SetProperty("NumberFormat", "Auto");//Auto,Hexadecimal,Decimal
            configDisassembler.SetProperty("Documentation", "None");//Formatted,Xml,None
            
            //configDisassembler.SetProperty("Optimization", OptimizationDefault);//None,1.0,2.0,3.5,4.0
            SetOptimization(OptimizationDefault);

            AssemblyManager.Resolver = new AssemblyResolver(this.AssemblyManager);
            AssemblyManager.Comparer = configAssemblyBrowser.GetProperty("SideBySideVersioning", "false") == "true" ? null : new AssemblyComparer();

            string[] strArray = new string[] { @"%SystemRoot%\Microsoft.net", @"%ProgramFiles%\Reference Assemblies", @"%ProgramFiles%\Microsoft.net", @"%ProgramFiles%\Microsoft Silverlight" };
            AssemblyCache.Directories.AddRange(strArray);

            this.Language = "C#";
            this.FormatterType = typeof(RichTextFormatter);

            //LoadSystemAssemblies();
        }

        ~SimpleReflector()
        {
            Dispose();
        }

        public ApplicationManager ApplicationManager
        {
            get { return _applicationManager; }
        }

        public string FormatterTypeName
        {
            get { return _formatterType == null ? null : _formatterType.FullName; }
            set
            {
                Type t = null;
                if (value != null)
                {
                    t = Type.GetType(value);
                }
                this.FormatterType = t;
            }
        }

        public Type FormatterType
        {
            get
            {
                return _formatterType;
            }
            set
            {
                if (value != null && value.GetInterface("Reflector.CodeModel.IFormatter") != null)
                {
                    _formatterType = value;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Invalid formatter type: {0}",
                        value == null ? "null" : value.FullName));
                }
            }
        }

        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                int index = GetLanguageIndex(value);
                if (index >= 0)
                {
                    _language = value;
                    _languageIndex = index;
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Invalid language: {0}", value));
                }
            }
        }

        #endregion Constructor

        #region Services
        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        protected T GetService<T>()
        {
            return ((T)(ServiceProvider.GetService(typeof(T))));
        }

        IWindowManager _windowManager;
        public IWindowManager WindowManager
        {
            get
            {
                if (_windowManager == null)
                    _windowManager = GetService<IWindowManager>();
                return _windowManager;
            }
        }
        
        IAssemblyBrowser _assemblyBrowser;
        public IAssemblyBrowser AssemblyBrowser
        {
            get
            {
                if (_assemblyBrowser == null)
                    _assemblyBrowser = GetService<IAssemblyBrowser>();
                return _assemblyBrowser;
            }
        }

        IAssemblyManager _assemblyManager;
        public IAssemblyManager AssemblyManager
        {
            get
            {
                if (_assemblyManager == null)
                    _assemblyManager = GetService<IAssemblyManager>();
                return _assemblyManager;
            }
        }

        IAssemblyCache _assemblyCache;
        public IAssemblyCache AssemblyCache
        {
            get
            {
                if (_assemblyCache == null)
                    _assemblyCache = GetService<IAssemblyCache>();
                return _assemblyCache;
            }
        }

        ILanguageManager _languageManager;
        public ILanguageManager LanguageManager
        {
            get
            {
                if (_languageManager == null)
                    _languageManager = GetService<ILanguageManager>();
                return _languageManager;
            }
        }
        
        
        ITranslatorManager _translatorManager;
        public ITranslatorManager TranslatorManager
        {
            get
            {
                if (_translatorManager == null)
                    _translatorManager = GetService<ITranslatorManager>();
                return _translatorManager;
            }
        }            
            
        IVisibilityConfiguration _visibilityConfiguration;
        public IVisibilityConfiguration VisibilityConfiguration
        {
            get
            {
                if (_visibilityConfiguration == null)
                    _visibilityConfiguration = GetService<IVisibilityConfiguration>();
                return _visibilityConfiguration;
            }
        }

        IConfigurationManager _configurationManager;
        public IConfigurationManager ConfigurationManager
        {
            get
            {
                if (_configurationManager == null)
                {
                    _configurationManager = GetService<IConfigurationManager>();
                }
                return _configurationManager;
            }
        }

        public ILanguageWriterConfiguration LanguageWriterConfiguration
        {
            get { return _langurageWriterConfiguration;  }
        }
        #endregion Services

        #region Utils
        private bool IsIL(int languageIndex)
        {
            return languageIndex == 0;
        }

        string[] _languages;
        public string[] Languages
        {
            get
            {
                if (_languages == null)
                {
                    ILanguageCollection lc = this.LanguageManager.Languages;
                    _languages = new string[lc.Count];
                    for (int i = 0; i < lc.Count; i++)
                    {
                        Reflector.CodeModel.ILanguage l = lc[i];
                        _languages[i] = l.Name;
                    }
                }
                return _languages;
            }
        }

        public int GetLanguageIndex(string language)
        {
            for (int i = 0; i < Languages.Length; i++)
            {
                if (Languages[i].Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        public ILanguage GetLanguage(string language)
        {
            int index = GetLanguageIndex(language);
            if (index >= 0)
            {
                ILanguageCollection lc = this.LanguageManager.Languages;
                return lc[index];
            }            
            return null;
        }

        public IFormatter CreateFormatter(string language)
        {
            return Activator.CreateInstance(this.FormatterType, new object[] { language }) as IFormatter;
        }

        public string Decompile(object o)
        {
            return Decompile(_languageIndex, o);
        }

        public string Decompile(int languageIndex, object o)
        {
            if (o == null) return String.Empty;

            IFormatter formatter = CreateFormatter(this.LanguageManager.Languages[languageIndex].Name);
            ILanguageWriter writer = this.LanguageManager.Languages[languageIndex].GetWriter(formatter, _langurageWriterConfiguration);
            ITranslator translator = this.TranslatorManager.CreateDisassembler(null, null);

            if (o is IMethodDeclaration)
            {
                IMethodDeclaration m2;
                if (IsIL(languageIndex))
                {
                    m2 = (IMethodDeclaration)o;
                }
                else
                {
                    m2 = translator.TranslateMethodDeclaration((IMethodDeclaration)o);
                }
                writer.WriteMethodDeclaration(m2);
                
            }
            else if (o is IPropertyDeclaration)
            {
                IPropertyDeclaration p2 = translator.TranslatePropertyDeclaration((IPropertyDeclaration)o);
                writer.WritePropertyDeclaration(p2);
            }
            else if (o is IFieldDeclaration)
            {
                IFieldDeclaration f2 = translator.TranslateFieldDeclaration((IFieldDeclaration)o);
                writer.WriteFieldDeclaration(f2);
            }
            else if (o is ITypeDeclaration)
            {
                ITypeDeclaration t2 = translator.TranslateTypeDeclaration((ITypeDeclaration)o, true, false);
                writer.WriteTypeDeclaration(t2);
            }
            else if (o is IEventDeclaration)
            {
                IEventDeclaration e2 = translator.TranslateEventDeclaration((IEventDeclaration)o);
                writer.WriteEventDeclaration(e2);
            }
            else if (o is IModule)
            {
                IModule m2 = translator.TranslateModule((IModule)o, true);
                writer.WriteModule(m2);
            }
            else if (o is IModuleReference)
            {
                IModuleReference mr2 = translator.TranslateModuleReference((IModuleReference)o);
                writer.WriteModuleReference(mr2);
            }
            else if (o is IAssembly)
            {
                IAssembly a2 = translator.TranslateAssembly((IAssembly)o, true);
                writer.WriteAssembly(a2);
            }
            else if (o is IAssemblyReference)
            {
                IAssemblyReference ar2 = translator.TranslateAssemblyReference((IAssemblyReference)o);
                writer.WriteAssemblyReference(ar2);
            }
            else if (o is IResource)
            {
                writer.WriteResource((IResource)o);
            }
            else if (o is INamespace)
            {
                writer.WriteNamespace((INamespace)o);
            }

            return formatter.ToString();
        }

        public IAssemblyCollection Assemblies
        {
            get
            {
                return this.AssemblyManager.Assemblies;
            }
        }

        public IAssembly LoadAssembly(string assemblyFile)
        {
            return LoadAssembly(assemblyFile, true);
        }

        public IAssembly LoadAssembly(string assemblyFile, bool force)
        {
            if (File.Exists(assemblyFile))
            {
                if (force)
                {
                    UnloadAssembly(assemblyFile); // keep last one?
                }
                return AssemblyManager.LoadFile(assemblyFile);
            }
            return null;
        }

        public void UnloadAssembly(string assemblyFile)
        {
            IAssembly a = FindAssembly(assemblyFile);
            if (a != null)
            {
                AssemblyManager.Unload(a);
            }
        }

        public IAssembly FindAssembly(string assemblyFile)
        {
            if (File.Exists(assemblyFile))
            {
                //Mono.Cecil.AssemblyDefinition ad = Mono.Cecil.AssemblyDefinition.ReadAssembly(assemblyFile);
                //string fullName = ad.Name.FullName;

                string fullName;
                if (PathUtils.IsNetModule(assemblyFile))
                {
                    fullName = assemblyFile;
                }
                else
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(assemblyFile);
                    fullName = an.FullName;
                }

                for (int i = 0; i < this.Assemblies.Count; i++)
                {
                    IAssembly a = this.Assemblies[i];
                    if (a.Location.Equals(assemblyFile, StringComparison.OrdinalIgnoreCase) ||
                        fullName.Equals(a.ToString()))
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        private void LoadSystemAssemblies()
        {
            //populate system assemblies 
            string path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.net\Framework\v2.0.50727");
            LoadAssembly(Path.Combine(path, "mscorlib.dll"));
            LoadAssembly(Path.Combine(path, "System.dll"));
            LoadAssembly(Path.Combine(path, "System.Xml.dll"));
            LoadAssembly(Path.Combine(path, "System.Data.dll"));
            LoadAssembly(Path.Combine(path, "System.Web.dll"));
            LoadAssembly(Path.Combine(path, "System.Drawing.dll"));
            LoadAssembly(Path.Combine(path, "System.Windows.Forms.dll"));
        }
        #endregion Utils

        #region FindMethod
        private void FindMethods(ITypeDeclaration typeDeclaration, string[] searchFors, List<MethodDeclarationInfo> list)
        {
            foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
            {
                try
                {
                    string methodText = this.Decompile(methodDeclaration);
                    bool found = false;
                    foreach (string s in searchFors)
                    {
                        if (methodText.Contains(s))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        list.Add(new MethodDeclarationInfo(methodDeclaration, methodText));
                    }
                }
                catch (Exception ex)
                {
                    list.Add(new MethodDeclarationInfo(methodDeclaration, ex.Message));
                }
            }

            foreach (ITypeDeclaration nestedType in typeDeclaration.NestedTypes)
            {
                FindMethods(nestedType, searchFors, list);
            }
        }

        public List<MethodDeclarationInfo> FindMethods(string fileName, string[] searchFors, IProgressBar progress, IsCancelPendingDelegate isCancelPendingDelegate)
        {
            return FindMethods(fileName, searchFors, null, progress, isCancelPendingDelegate);
        }

        public List<MethodDeclarationInfo> FindMethods(string fileName, string[] searchFors, string[] searchForTypes, IProgressBar progress, IsCancelPendingDelegate isCancelPendingDelegate)
        {
            List<MethodDeclarationInfo> list = new List<MethodDeclarationInfo>();

            IAssembly assembly = this.LoadAssembly(fileName);
            if (assembly == null)
            {
                list.Add(new MethodDeclarationInfo(null, String.Format("Can't load assembly: {0}", fileName)));
                return list;                
            }
            if (!String.IsNullOrEmpty(assembly.Status))
            {
                list.Add(new MethodDeclarationInfo(null, String.Format("Error when loading assembly: {0} ({1})", fileName, assembly.Status)));
                return list;
            }

            foreach (IModule module in assembly.Modules)
            {
                int count = 0;

                if (isCancelPendingDelegate != null && isCancelPendingDelegate()) break;

                if (progress != null)
                {
                    progress.InitProgress(0, module.Types.Count);
                    progress.SetProgress(0);
                }

                foreach (ITypeDeclaration typeDeclaration in module.Types)
                {
                    if (isCancelPendingDelegate != null && isCancelPendingDelegate()) break;

                    string typeName = typeDeclaration.Name;
                    if (searchForTypes != null)
                    {
                        foreach (string searchForType in searchForTypes)
                        {
                            if (typeName.IndexOf(searchForType, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                FindMethods(typeDeclaration, searchFors, list);
                                break;
                            }
                        }
                    }
                    else
                    {
                        FindMethods(typeDeclaration, searchFors, list);
                    }

                    count++;
                    if (progress != null)
                    {
                        progress.SetProgress(count);
                    }
                }
            }

            return list;
        }

        public delegate bool IsCancelPendingDelegate();      

        public List<MethodDeclarationInfo> FindMethods(string[] files, string[] searchFors, string[] searchForTypes, IProgressBar progress, IsCancelPendingDelegate isCancelPendingDelegate)
        {            
            if (files == null || files.Length == 0) return null;

            List<MethodDeclarationInfo> list = new List<MethodDeclarationInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                if (isCancelPendingDelegate != null && isCancelPendingDelegate()) break;

                list.AddRange(this.FindMethods(files[i], searchFors, searchForTypes, progress, isCancelPendingDelegate));
            }

            return list;
        }
        #endregion FindMethod

        #region IDisposable Members

        public void Dispose()
        {
            if (_applicationManager != null)
            {
                _applicationManager.Dispose();
                _applicationManager = null;
            }
            _langurageWriterConfiguration = null;
            _serviceProvider = null;
        }

        #endregion
    }//end of class 

}
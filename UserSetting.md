# Save user settings in a SAE plugin #

**Steps:**

1. Register properties which need to be saved in plugin's constructor

Sample:
```
        public const string PropertyOutputDir = "ILMergeOutputDir";
        public const string PropertyStrongKeyFile = "ILMergeStrongKeyFile";

        public Plugin(IHost host)
        {
            host.AddProperty(PropertyOutputDir, String.Empty, typeof(String));
            host.AddProperty(PropertyStrongKeyFile, String.Empty, typeof(String));
        }
```

2. Use properties

Sample:
```
        private IHost _host;

        public string OutputDir
        {
            get { return _host.GetPropertyValue(Plugin.PropertyOutputDir) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyOutputDir, value); }
        }

        public string StrongKeyFile
        {
            get { return _host.GetPropertyValue(Plugin.PropertyStrongKeyFile) as string; }
            set { _host.SetPropertyValue(Plugin.PropertyStrongKeyFile, value); }
        }
```
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class DeobfProfile {

        public string Name { get; set; }
        public DeobfOptions Options { get; set; }

        public DeobfProfile(string name, DeobfOptions options) {
            this.Name = name;
            this.Options = options;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class ProfileFile : TextFile
    {
        static ProfileFile _default;
        public static ProfileFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new ProfileFile("Profile.txt"), null);
                }
                return _default;
            }
        }

        public ProfileFile(string fileName)
            : base(fileName)
        {
        }

        private List<DeobfProfile> _profiles;
        public List<DeobfProfile> Profiles
        {
            get
            {
                ReadFile();
                return _profiles;
            }
        }

        public DeobfProfile GetProfile(string name)
        {
            foreach (DeobfProfile profile in Profiles)
            {
                if (profile.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return profile;
            }
            return null;
        }

        public override void ParseLines()
        {
            _profiles = new List<DeobfProfile>();

            string profileName = null;
            DeobfOptions options = null;
            Type optionsType = typeof(DeobfOptions);

            foreach (string line in this.Lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (profileName != null)
                    {
                        _profiles.Add(new DeobfProfile(profileName, options));
                    }

                    profileName = line.Substring(1, line.Length - 2);
                    options = new DeobfOptions();
                }
                else if (profileName != null)
                {
                    string[] s = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        string propertyName = s[0];
                        string propertyValue = s[1];

                        PropertyInfo pi = optionsType.GetProperty(propertyName);
                        if (pi != null)
                        {
                            if (propertyName.EndsWith("Checked"))
                            {
                                pi.SetValue(options, Convert.ToBoolean(propertyValue), null);
                            }
                            else if (propertyName.EndsWith("Count"))
                            {
                                pi.SetValue(options, Convert.ToInt32(propertyValue), null);
                            }
                            else
                            {
                                pi.SetValue(options, propertyValue, null);
                            }
                        }
                    }
                }
            }

            if (profileName != null)
            {
                _profiles.Add(new DeobfProfile(profileName, options));
            }
            
        }      

        public override void Clear()
        {
            Clear(_profiles);
            base.Clear();
        }       

    }//end of class   

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
//using nBayes;

namespace SimpleAssemblyExplorer
{
    public class RandomFile : TextFile
    {
        static RandomFile _default;
        public static RandomFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new RandomFile("Random.txt"), null);
                }
                return _default;
            }
        }

        public RandomFile(string fileName)
            : base(fileName)
        {
        }

        const int defaultValidNameMaxLength = 15;
        const int defaultRandomNameMinLength = 5;
        const double defaultAbnormalPercentage = 0.6;

        const string vowels = @"[aeiouy]";
        const string consonants = @"[bcdfghjklmnpqrstvwxz]";
        char[] charsSeparators = new char[] { ' ', '_' };

        Regex regexDigits = new Regex(@"[0-9]");
        Regex regexUpperLetters = new Regex(@"[A-Z]");
        Regex regexReplaceUpperLetters = new Regex(@"([A-Z])");
        Regex regexLowerLetters = new Regex(@"[a-z]");
        Regex regexDefaultRules = new Regex(@"DefaultRule(\d+)Enabled");
        Regex regexVowels = new Regex(vowels);
        Regex regexConsonants = new Regex(consonants);
        Regex regexMaxConsecutiveVowels = new Regex(vowels + "{4,}");
        Regex regexMaxConsecutiveConsonants = new Regex(consonants + "{5,}");
        Regex regexDigitBeforeLetter = new Regex(@"[0-9]+[a-z]+");
        Regex regexChinese = new Regex(@"^[\u4E00-\u9FA5]+$");

        string _dictionaryFile = "Dictionary.txt";
        public string DictionaryFile
        {
            get { return _dictionaryFile; }
            set { _dictionaryFile = value; }
        }

        private string[] SplitNameByCapital(string name)
        {
            string preparedName = regexReplaceUpperLetters.Replace(name, " $1");
            string[] splittedNames = preparedName.Split(charsSeparators, StringSplitOptions.RemoveEmptyEntries);
            return splittedNames;
        }

        public bool IsRandomName(string name)
        {
            if (name.Length < RandomNameMinLength)
                return false;

            string lowerCaseName = name.ToLowerInvariant();
            if (IsWhiteWord(name))
                return false;
            if (IsBlackWord(name))
                return true;

            if (CustomRulesEnabled)
            {
                if (IsWhiteWordByCustomRules(name))
                    return false;
                if (IsBlackWordByCustomRules(name))
                    return true;
            }

            if (DefaultRuleEnabled[1])
            {
                if (IsRandomNameByDefaultRule1(name))
                    return true;
            }

            if (DefaultRuleEnabled[2])
            {
                if (IsRandomNameByDefaultRule2(name))
                    return true;
            }

            //if (nBayesEnabled)
            //{
            //    if (IsRandomNameByBayes(name))
            //        return true;
            //}

            return false;
        }

        public bool IsRandomNameByDefaultRule1(string name)
        {
            int length = name.Length;
            if (name.StartsWith("get_") || name.StartsWith("set_") || name.StartsWith("add_"))
                length -= 4;
            else if (name.StartsWith("remove_") || name.StartsWith("invoke_"))
                length -= 7;

            if (length > ValidNameMaxLength && regexDigitBeforeLetter.IsMatch(name))
                return true;
            return false;
        }

        public bool IsRandomNameByDefaultRule2(string name)
        {
            int abnormalCount = 0;
            int singleLetterCount = 0;
            string[] splittedNames;
            bool allUpperCase;

            if (regexLowerLetters.IsMatch(name)) //there is lower letter
            {
                allUpperCase = false;
                splittedNames = SplitNameByCapital(name);
            }
            else //all upper letters
            {
                allUpperCase = true;
                splittedNames = name.Split(charsSeparators, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (string splittedName in splittedNames)
            {
                string s = splittedName.ToLowerInvariant();

                if (regexChinese.IsMatch(s))
                    continue;

                if (IsBlackWord(s))
                {
                    //abnormalCount = splittedName.Length;
                    abnormalCount++;
                    continue;
                }

                if (IsWhiteWord(s))
                {
                    continue;
                }

                if (regexDigitBeforeLetter.IsMatch(s))
                {
                    abnormalCount++;
                    continue;
                }

                s = regexDigits.Replace(s, "");

                if (s.Length == 1 && regexLowerLetters.IsMatch(s))
                {
                    singleLetterCount++;
                    continue;
                }

                if (
                    (!allUpperCase && s.Length < 3) ||
                        (!allUpperCase && !regexVowels.IsMatch(s)) ||
                        (!allUpperCase && !regexConsonants.IsMatch(s)) ||
                        regexMaxConsecutiveConsonants.IsMatch(s) ||
                        regexMaxConsecutiveVowels.IsMatch(s)
                        )
                {
                    abnormalCount++;
                    continue;
                }
            }

            if (singleLetterCount > 4)
                abnormalCount += singleLetterCount;

            if (abnormalCount * 1.0 / splittedNames.Length >= AbnormalMinPercentage)
                return true;

            return false;
        }

        public bool IsMatchCustomRules(string name, List<string> rules)
        {
            foreach (string rule in rules)
            {
                string expr = rule.Replace("{name}", String.Format("\"{0}\"", name));
                object o = Evaluator.Eval(expr);
                if (Convert.ToBoolean(o))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsBlackWordByCustomRules(string name)
        {
            return IsMatchCustomRules(name, BlackCustomRules);
        }

        public bool IsWhiteWordByCustomRules(string name)
        {
            return IsMatchCustomRules(name, WhiteCustomRules);
        }

        //public bool IsRandomNameByBayes(string name)
        //{
        //    string[] splittedNames = SplitNameByCapital(name);
        //    string test = String.Join(" ", splittedNames);

        //    CategorizationResult result = this.nBayesAnalyzer.Categorize(
        //         Entry.FromString(test),
        //         nBayesBlackIndex,
        //         nBayesWhiteIndex);

        //    return result == CategorizationResult.First;
        //}

        List<string> _blackCustomRules;
        public List<string> BlackCustomRules
        {
            get
            {
                ReadFile();
                return _blackCustomRules;
            }
        }

        List<string> _whiteCustomRules;
        public List<string> WhiteCustomRules
        {
            get
            {
                ReadFile();
                return _whiteCustomRules;
            }
        }

        Dictionary<int, bool> _defaultRuleEnabled;
        public Dictionary<int, bool> DefaultRuleEnabled
        {
            get
            {
                ReadFile();
                return _defaultRuleEnabled;
            }
            set { _defaultRuleEnabled = value; }
        }

        int _validNameMaxLength = defaultValidNameMaxLength;
        public int ValidNameMaxLength
        {
            get
            {
                ReadFile();
                return _validNameMaxLength;
            }
            set
            {
                if (value >= 0)
                    _validNameMaxLength = value;
            }
        }

        int _randomNameMinLength = defaultRandomNameMinLength;
        public int RandomNameMinLength
        {
            get
            {
                ReadFile();
                return _randomNameMinLength;
            }
            set
            {
                if (value >= 0)
                    _randomNameMinLength = value;
            }
        }

        bool _customRulesEnabled = true;
        public bool CustomRulesEnabled
        {
            get
            {
                ReadFile();
                return _customRulesEnabled;
            }
            set { _customRulesEnabled = value; }
        }

        bool _dictionaryEnabled = true;
        public bool DictionaryEnabled
        {
            get
            {
                ReadFile();
                return _dictionaryEnabled;
            }
            set { _dictionaryEnabled = value; }
        }

        //bool _nBayesEnabled = false;
        //public bool nBayesEnabled
        //{
        //    get
        //    {
        //            ReadFile();
        //        return _nBayesEnabled;
        //    }
        //    set { _nBayesEnabled = value; }
        //}        

        double _abnormalMinPercentage = defaultAbnormalPercentage;
        public double AbnormalMinPercentage
        {
            get
            {
                ReadFile();
                return _abnormalMinPercentage;
            }
            set
            {
                if (value >= 0)
                    _abnormalMinPercentage = value;
            }
        }

        Dictionary<string, bool> _blackList;
        public Dictionary<string, bool> BlackList
        {
            get
            {
                ReadFile();
                return _blackList;
            }
            set { _blackList = value; }
        }

        Dictionary<string, bool> _whiteList;
        public Dictionary<string, bool> WhiteList
        {
            get
            {
                ReadFile();
                return _whiteList;
            }
            set { _whiteList = value; }
        }

        private void AddBlackWord(string name)
        {
            string word = name.ToLowerInvariant();
            if(_blackList.ContainsKey(word))
                return;
            _blackList.Add(word, true);
        }

        private void AddWhiteWord(string name)
        {
            string word = name.ToLowerInvariant();
            if (_whiteList.ContainsKey(word))
                return;
            _whiteList.Add(word, true);
        }

        public bool IsBlackWord(string name)
        {
            if (BlackList.ContainsKey(name.ToLowerInvariant()))
                return true;
            return false;
        }

        public bool IsWhiteWord(string name)
        {
            if (WhiteList.ContainsKey(name.ToLowerInvariant()))
                return true;
            return false;
        }

        //Analyzer _nBayesAnalyzer;
        //public Analyzer nBayesAnalyzer
        //{
        //    get
        //    {
        //        if (_nBayesAnalyzer == null)
        //        {
        //            _nBayesAnalyzer = new Analyzer();
        //        }
        //        return _nBayesAnalyzer;
        //    }
        //}

        //Index _nBayesBlackIndex;
        //public Index nBayesBlackIndex
        //{
        //    get
        //    {
        //            ReadFile();
        //        return _nBayesBlackIndex;
        //    }
        //}

        //Index _nBayesWhiteIndex;
        //public Index nBayesWhiteIndex
        //{
        //    get
        //    {
        //            ReadFile();
        //        return _nBayesWhiteIndex;
        //    }
        //}

        //private void AddWordToIndex(string name, Index index)
        //{
        //    index.Add(Entry.FromString(name));

        //    string[] splittedNames = SplitNameByCapital(name);
        //    foreach (string s in splittedNames)
        //    {
        //        index.Add(Entry.FromString(s));
        //    }
        //}

        public override void ParseLines()
        {
            string profileName = null;

            _blackCustomRules = new List<string>();
            _whiteCustomRules = new List<string>();
            _defaultRuleEnabled = new Dictionary<int, bool>();
            _whiteList = new Dictionary<string, bool>();
            _blackList = new Dictionary<string, bool>();

            Type randomFileType = typeof(RandomFile);

            foreach (string line in this.Lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    profileName = line.Substring(1, line.Length - 2);
                }
                else if (profileName == "WhiteList")
                {
                    AddWhiteWord(line);
                }
                else if (profileName == "BlackList")
                {
                    AddBlackWord(line);
                }
                else if (profileName == "BlackCustomRules")
                {
                    _blackCustomRules.Add(line);
                }
                else if (profileName == "WhiteCustomRules")
                {
                    _whiteCustomRules.Add(line);
                }
                else if (profileName == "Settings")
                {
                    string[] s = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        string propertyName = s[0];
                        string propertyValue = s[1];

                        Match m = regexDefaultRules.Match(propertyName);
                        if (m.Success)
                        {
                            _defaultRuleEnabled.Add(Convert.ToInt32(m.Groups[1].Value), Convert.ToBoolean(propertyValue));
                        }
                        else
                        {
                            PropertyInfo pi = randomFileType.GetProperty(propertyName);
                            if (pi != null)
                            {
                                if (propertyName.EndsWith("Enabled"))
                                {
                                    bool b;
                                    if (bool.TryParse(propertyValue, out b))
                                    {
                                        pi.SetValue(this, b, null);
                                    }
                                }
                                else if (propertyName.EndsWith("Length"))
                                {
                                    pi.SetValue(this, Convert.ToInt32(propertyValue), null);
                                }
                                else if (propertyName.EndsWith("Percentage"))
                                {
                                    double d;
                                    if (double.TryParse(propertyValue, out d))
                                    {
                                        pi.SetValue(this, d, null);
                                    }
                                    else if (double.TryParse(propertyValue, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out d))
                                    {
                                        pi.SetValue(this, d, null);
                                    }
                                }
                                else
                                {
                                    pi.SetValue(this, propertyValue, null);
                                }
                            }
                        }
                    }
                }

            }//end foreach

            
            //init default rules
            for (int i = 0; i < 1; i++)
            {
                if (!_defaultRuleEnabled.ContainsKey(i))
                {
                    _defaultRuleEnabled.Add(i, true);
                }
            }

            if(_validNameMaxLength < 0)
                _validNameMaxLength = defaultValidNameMaxLength;
            if (_randomNameMinLength < 0)
                _randomNameMinLength = defaultRandomNameMinLength;
            if (_abnormalMinPercentage < 0)
                _abnormalMinPercentage = defaultAbnormalPercentage;

            if (_dictionaryEnabled && File.Exists(this.DictionaryFile))
            {
                TextFile tf = new TextFile(DictionaryFile);
                foreach (string line in tf.Lines)
                {
                    AddWhiteWord(line);
                }
            }

            //if (nBayesEnabled)
            //{
            //    _nBayesWhiteIndex = Index.CreateMemoryIndex();
            //    foreach (string word in WhiteList.Keys)
            //    {
            //        AddWordToIndex(word, _nBayesWhiteIndex);
            //    }
            //    if (DictionaryEnabled && File.Exists(DictionaryFile))
            //    {
            //        TextFile tf = new TextFile(DictionaryFile);
            //        foreach (string line in tf.Lines)
            //        {
            //            AddWordToIndex(line, _nBayesWhiteIndex);
            //        }
            //    }

            //    _nBayesBlackIndex = Index.CreateMemoryIndex();
            //    foreach (string word in BlackList.Keys)
            //    {
            //        AddWordToIndex(word, _nBayesBlackIndex);
            //    }
            //}
        }

        public override void Clear()
        {
            Clear(_blackCustomRules);
            Clear(_whiteCustomRules);
            Clear(_defaultRuleEnabled);
            Clear(_whiteList);
            Clear(_blackList);            

            _validNameMaxLength = defaultValidNameMaxLength;
            _randomNameMinLength = defaultRandomNameMinLength;
            _abnormalMinPercentage = defaultAbnormalPercentage;
            _customRulesEnabled = false;            
            _dictionaryEnabled = true;
            
            //_nBayesEnabled = null;
            //_nBayesAnalyzer = null;
            //_nBayesBlackIndex = null;
            //_nBayesWhiteIndex = null;            

            base.Clear();
        }       

    }//end of class   

}

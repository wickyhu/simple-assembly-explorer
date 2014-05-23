using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Mono.Cecil;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    //The sequence should be same as dropdown list in frmDeobf and frmDeobfMethod
    public enum BranchDirections
    {
        TopDown,
        BottomUp,
        WalkThrough
    }

    public class DeobfOptions : OptionsBase
    {
        //Assumption: Count and Checked properties should be public and same name as controls in frmDeobf

        public int LoopCount { get; set; }
        public int MaxRefCount { get; set; }
        public int MaxMoveCount { get; set; }

        public bool chkNonAsciiChecked { get; set; }
        public bool chkRandomNameChecked { get; set; }
        public bool chkRegexChecked { get; set; }
        public bool chkAutoStringChecked { get; set; }
        public bool chkHexRenameChecked { get; set; }

        public bool chkBoolFunctionChecked { get; set; }
        public bool chkPatternChecked { get; set; }
        public bool chkBranchChecked { get; set; }
        public bool chkCondBranchDownChecked { get; set; }
        public bool chkCondBranchUpChecked { get; set; }
        
        public bool chkSwitchChecked { get; set; }
        public bool chkUnreachableChecked { get; set; }
        public bool chkRemoveExceptionHandlerChecked { get; set; }
        public bool chkDelegateCallChecked { get; set; }
        public bool chkDirectCallChecked { get; set; }
        public bool chkBlockMoveChecked { get; set; }
        public bool chkReflectorFixChecked { get; set; }
        public bool chkRemoveInvalidInstructionChecked { get; set; }

        public bool chkRemoveDummyMethodChecked { get; set; }
        public bool chkRemoveAttributeChecked { get; set; }
        public bool chkRemoveSealedChecked { get; set; }
        public bool chkInternalToPublicChecked { get; set; }
        public bool chkInitLocalVarsChecked { get; set; }
        public bool chkAddMissingPropertyAndEventChecked { get; set; }

        public MethodDefinition StringOptionSearchForMethod { get; set; }
        public MethodDefinition StringOptionCalledMethod { get; set; }

        public string txtRegexText { get; set; }
        public string OutputSuffix { get; set; }
        public BranchDirections BranchDirection { get; set; }

        #region Plugin
        public List<IDeobfPlugin> PluginList { get; set; }
        public bool HasPlugin 
        {
            get { return PluginList != null && PluginList.Count > 0; }
        }
        #endregion Plugin

        #region IsCancelPending
        public delegate bool IsCancelPendingDelegate();
        public IsCancelPendingDelegate IsCancelPendingFunction { get; set; }
        public bool IsCancelPending
        {
            get
            {
                if (this.IsCancelPendingFunction != null)
                    return IsCancelPendingFunction();
                return false;
            }
        }
        #endregion IsCancelPending

        #region Text Files
        private AttributeFile _attributeFile;
        public AttributeFile AttributeFile
        {
            get { return _attributeFile == null ? AttributeFile.Default : _attributeFile; }
            set { _attributeFile = value; }
        }
        private ExceptionHandlerFile _exceptionHandlerFile;
        public ExceptionHandlerFile ExceptionHandlerFile
        {
            get { return _exceptionHandlerFile == null ? ExceptionHandlerFile.Default : _exceptionHandlerFile; }
            set { _exceptionHandlerFile = value; }
        }
        private IgnoredMethodFile _ignoredMethodFile;
        public IgnoredMethodFile IgnoredMethodFile
        {
            get { return _ignoredMethodFile == null ? IgnoredMethodFile.Default : _ignoredMethodFile; }
            set { _ignoredMethodFile = value; }
        }
        private IgnoredTypeFile _ignoredTypeFile;
        public IgnoredTypeFile IgnoredTypeFile
        {
            get { return _ignoredTypeFile == null ? IgnoredTypeFile.Default : _ignoredTypeFile; }
            set { _ignoredTypeFile = value; }
        }
        private KeywordFile _keywordFile;
        public KeywordFile KeywordFile
        {
            get { return _keywordFile == null ? KeywordFile.Default : _keywordFile; }
            set { _keywordFile = value; }
        }
        private PatternFile _patternFile;
        public PatternFile PatternFile
        {
            get { return _patternFile == null ? PatternFile.Default : _patternFile; }
            set { _patternFile = value; }
        }
        private RegexFile _regexFile;
        public RegexFile RegexFile
        {
            get { return _regexFile == null ? RegexFile.Default : _regexFile; }
            set { _regexFile = value; }
        }
        private RandomFile _randomFile;
        public RandomFile RandomFile
        {
            get { return _randomFile == null ? RandomFile.Default : _randomFile; }
            set { _randomFile = value; }
        }
        #endregion Text Files

        #region Default
        public DeobfOptions()
            : base()
        {
        }

        public override void Clear()
        {
            base.Clear();

            this.LoopCount = 2;
            this.MaxRefCount = 2;
            this.MaxMoveCount = 0;

            this.chkNonAsciiChecked = false;
            this.chkRandomNameChecked = false;
            this.chkRegexChecked = false;
            this.chkHexRenameChecked = false;

            this.chkAutoStringChecked = false;
            this.chkBoolFunctionChecked = false;
            this.chkPatternChecked = false;
            this.chkBranchChecked = false;
            this.chkCondBranchDownChecked = false;
            this.chkCondBranchUpChecked = false;
            
            this.chkSwitchChecked = false;
            this.chkUnreachableChecked = false;
            this.chkRemoveExceptionHandlerChecked = false;
            this.chkDelegateCallChecked = false;
            this.chkDirectCallChecked = false;
            this.chkBlockMoveChecked = false;
            this.chkReflectorFixChecked = false;
            this.chkRemoveInvalidInstructionChecked = false;

            this.chkRemoveDummyMethodChecked = false;
            this.chkRemoveAttributeChecked = false;
            this.chkRemoveSealedChecked = false;
            this.chkInternalToPublicChecked = false;
            this.chkInitLocalVarsChecked = false;
            this.chkAddMissingPropertyAndEventChecked = false;

            this.StringOptionSearchForMethod = null;
            this.StringOptionCalledMethod = null;

            this.txtRegexText = String.Empty;            
            this.BranchDirection = BranchDirections.TopDown;

            this.AttributeFile = null;
            this.ExceptionHandlerFile = null;
            this.IgnoredMethodFile = null;
            this.KeywordFile = null;
            this.PatternFile = null;
            this.RegexFile = null;
        }

        public override void InitDefaults()
        {
            base.InitDefaults();

            this.LoopCount = 2;
            this.MaxRefCount = 2;
            this.MaxMoveCount = 0;

            this.chkNonAsciiChecked = true;
            this.chkRandomNameChecked = true;
            this.chkRegexChecked = true;
            this.chkHexRenameChecked = false;

            this.chkAutoStringChecked = true;
            this.chkBoolFunctionChecked = true;
            this.chkPatternChecked = true;
            this.chkBranchChecked = true;
            this.chkCondBranchDownChecked = true;
            this.chkCondBranchUpChecked = false;
            
            this.chkSwitchChecked = true;
            this.chkUnreachableChecked = true;
            this.chkRemoveExceptionHandlerChecked = true;
            this.chkDelegateCallChecked = false;
            this.chkDirectCallChecked = false;
            this.chkBlockMoveChecked = false;
            this.chkReflectorFixChecked = true;
            this.chkRemoveInvalidInstructionChecked = false;

            this.chkRemoveDummyMethodChecked = false;
            this.chkRemoveAttributeChecked = true;
            this.chkRemoveSealedChecked = false;
            this.chkInternalToPublicChecked = false;
            this.chkInitLocalVarsChecked = false;
            this.chkAddMissingPropertyAndEventChecked = false;

            this.StringOptionSearchForMethod = null;
            this.StringOptionCalledMethod = null;

            this.txtRegexText = String.Empty;
            this.BranchDirection = BranchDirections.TopDown;

            this.AttributeFile = null;
            this.ExceptionHandlerFile = null;
            this.IgnoredMethodFile = null;
            this.KeywordFile = null;
            this.PatternFile = null;
            this.RegexFile = null;
        }
        #endregion Default

        #region Apply
        public void ApplyFrom(string profileName)
        {
            DeobfProfile profile = ProfileFile.Default.GetProfile(profileName);
            if (profile == null) return;
            ApplyFrom(profile.Options);
        }

        public void ApplyFrom(DeobfOptions options)
        {
            Type optionsType = typeof(DeobfOptions);
            PropertyInfo[] pis = optionsType.GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                string propertyName = pi.Name;
                if (propertyName.EndsWith("Checked") || propertyName.EndsWith("Count"))
                {
                    pi.SetValue(this, pi.GetValue(options, null), null);
                }
            }
        }

        private void ApplyFrom(Control.ControlCollection controls)
        {
            if (controls == null || controls.Count <= 0)
                return;

            Type optionsType = typeof(DeobfOptions);
            foreach (Control ctrl in controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox chk = (CheckBox)ctrl;
                    PropertyInfo pi = optionsType.GetProperty(chk.Name + "Checked");
                    if (pi != null)
                    {
                        pi.SetValue(this, chk.Checked, null);
                    }
                }
                else if (ctrl is NumericUpDown)
                {
                    NumericUpDown nud = (NumericUpDown)ctrl;
                    PropertyInfo pi = optionsType.GetProperty(nud.Name.Substring(3)); //remove prefix "nud"
                    if (pi != null)
                    {
                        pi.SetValue(this, (int)nud.Value, null);
                    }
                }
                else if (ctrl.Controls != null && ctrl.Controls.Count > 0)
                {
                    ApplyFrom(ctrl.Controls);
                }
            }
        }

        public void ApplyFrom(Form form)
        {
            ApplyFrom(form.Controls);
        }

        public void ApplyTo(Form form)
        {
            Type optionsType = typeof(DeobfOptions);
            PropertyInfo[] pis = optionsType.GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                string propertyName = pi.Name;
                string suffix = "Checked";
                if (propertyName.EndsWith(suffix))
                {
                    string controlName = propertyName.Substring(0, propertyName.Length - suffix.Length);
                    Control[] controls = form.Controls.Find(controlName, true);
                    if (controls != null && controls.Length > 0)
                    {
                        CheckBox chk = controls[0] as CheckBox;
                        if (chk != null)
                        {
                            chk.Checked = Convert.ToBoolean(pi.GetValue(this, null));
                        }
                    }
                }
                else if (propertyName.EndsWith("Count"))
                {
                    string controlName = "nud" + propertyName;
                    Control[] controls = form.Controls.Find(controlName, true);
                    if (controls != null && controls.Length > 0)
                    {
                        NumericUpDown nud = controls[0] as NumericUpDown;
                        if (nud != null)
                        {
                            nud.Value = Convert.ToInt32(pi.GetValue(this, null));
                        }
                    }
                }
            }
        }

        #endregion Apply

    } //end of class
}
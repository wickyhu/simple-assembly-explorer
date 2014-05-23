namespace SimpleAssemblyExplorer
{
    partial class frmDeobf
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnSelectDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectMethod = new System.Windows.Forms.Button();
            this.txtMethod = new System.Windows.Forms.TextBox();
            this.chkNonAscii = new System.Windows.Forms.CheckBox();
            this.chkPattern = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkRandomName = new System.Windows.Forms.CheckBox();
            this.chkRemoveAttribute = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkAutoString = new System.Windows.Forms.CheckBox();
            this.chkBranch = new System.Windows.Forms.CheckBox();
            this.chkSwitch = new System.Windows.Forms.CheckBox();
            this.nudLoopCount = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chkUnreachable = new System.Windows.Forms.CheckBox();
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.txtRegex = new System.Windows.Forms.TextBox();
            this.chkBoolFunction = new System.Windows.Forms.CheckBox();
            this.lblRegexFile = new System.Windows.Forms.Label();
            this.lblPatternFile = new System.Windows.Forms.Label();
            this.chkRemoveDummyMethod = new System.Windows.Forms.CheckBox();
            this.chkCondBranchDown = new System.Windows.Forms.CheckBox();
            this.chkInternalToPublic = new System.Windows.Forms.CheckBox();
            this.chkRemoveSealed = new System.Windows.Forms.CheckBox();
            this.chkInitLocalVars = new System.Windows.Forms.CheckBox();
            this.chkRemoveExceptionHandler = new System.Windows.Forms.CheckBox();
            this.lblRemoveExceptionHandler = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nudMaxRefCount = new System.Windows.Forms.NumericUpDown();
            this.lblBooleanFunction = new System.Windows.Forms.Label();
            this.lblStringOption = new System.Windows.Forms.Label();
            this.lblRemoveAttribute = new System.Windows.Forms.Label();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.lblProfile = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkDelegateCall = new System.Windows.Forms.CheckBox();
            this.cboDirection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblIgnoredTypeFile = new System.Windows.Forms.Label();
            this.chkCondBranchUp = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chkHexRename = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCalledMethod = new System.Windows.Forms.TextBox();
            this.btnSelectCalledMethod = new System.Windows.Forms.Button();
            this.chkDirectCall = new System.Windows.Forms.CheckBox();
            this.chkBlockMove = new System.Windows.Forms.CheckBox();
            this.chkReflectorFix = new System.Windows.Forms.CheckBox();
            this.lblRandom = new System.Windows.Forms.Label();
            this.tabAdditional = new System.Windows.Forms.TabPage();
            this.chkAddMissingPropertyAndEvent = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dgvPlugin = new System.Windows.Forms.DataGridView();
            this.dgvcSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgcTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkRemoveInvalidInstruction = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudLoopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRefCount)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabAdditional.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugin)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Output Directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtOutputDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtOutputDir.Location = new System.Drawing.Point(105, 36);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(473, 21);
            this.txtOutputDir.TabIndex = 4;
            this.txtOutputDir.Leave += new System.EventHandler(this.txtOutputDir_Leave);
            // 
            // btnSelectDir
            // 
            this.btnSelectDir.Location = new System.Drawing.Point(585, 36);
            this.btnSelectDir.Name = "btnSelectDir";
            this.btnSelectDir.Size = new System.Drawing.Size(27, 21);
            this.btnSelectDir.TabIndex = 5;
            this.btnSelectDir.Text = "...";
            this.btnSelectDir.UseVisualStyleBackColor = true;
            this.btnSelectDir.Click += new System.EventHandler(this.btnSelectOutputDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(211, 291);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(322, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtInfo.Location = new System.Drawing.Point(0, 322);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(637, 238);
            this.txtInfo.TabIndex = 3;
            this.txtInfo.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Name Options";
            // 
            // btnSelectMethod
            // 
            this.btnSelectMethod.Enabled = false;
            this.btnSelectMethod.Location = new System.Drawing.Point(321, 86);
            this.btnSelectMethod.Name = "btnSelectMethod";
            this.btnSelectMethod.Size = new System.Drawing.Size(27, 21);
            this.btnSelectMethod.TabIndex = 17;
            this.btnSelectMethod.Text = "...";
            this.btnSelectMethod.UseVisualStyleBackColor = true;
            this.btnSelectMethod.Click += new System.EventHandler(this.btnSelectMethod_Click);
            // 
            // txtMethod
            // 
            this.txtMethod.Enabled = false;
            this.txtMethod.Location = new System.Drawing.Point(185, 87);
            this.txtMethod.Name = "txtMethod";
            this.txtMethod.Size = new System.Drawing.Size(130, 21);
            this.txtMethod.TabIndex = 16;
            // 
            // chkNonAscii
            // 
            this.chkNonAscii.AutoSize = true;
            this.chkNonAscii.Checked = true;
            this.chkNonAscii.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNonAscii.Location = new System.Drawing.Point(105, 64);
            this.chkNonAscii.Name = "chkNonAscii";
            this.chkNonAscii.Size = new System.Drawing.Size(70, 17);
            this.chkNonAscii.TabIndex = 7;
            this.chkNonAscii.Text = "Non-Ascii";
            this.chkNonAscii.UseVisualStyleBackColor = true;
            // 
            // chkPattern
            // 
            this.chkPattern.AutoSize = true;
            this.chkPattern.Checked = true;
            this.chkPattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPattern.Location = new System.Drawing.Point(219, 113);
            this.chkPattern.Name = "chkPattern";
            this.chkPattern.Size = new System.Drawing.Size(15, 14);
            this.chkPattern.TabIndex = 24;
            this.chkPattern.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Flow Options";
            // 
            // chkRandomName
            // 
            this.chkRandomName.AutoSize = true;
            this.chkRandomName.Checked = true;
            this.chkRandomName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRandomName.Location = new System.Drawing.Point(180, 65);
            this.chkRandomName.Name = "chkRandomName";
            this.chkRandomName.Size = new System.Drawing.Size(15, 14);
            this.chkRandomName.TabIndex = 8;
            this.chkRandomName.UseVisualStyleBackColor = true;
            // 
            // chkRemoveAttribute
            // 
            this.chkRemoveAttribute.AutoSize = true;
            this.chkRemoveAttribute.Checked = true;
            this.chkRemoveAttribute.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveAttribute.Location = new System.Drawing.Point(106, 12);
            this.chkRemoveAttribute.Name = "chkRemoveAttribute";
            this.chkRemoveAttribute.Size = new System.Drawing.Size(15, 14);
            this.chkRemoveAttribute.TabIndex = 1;
            this.chkRemoveAttribute.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Other Options";
            // 
            // chkAutoString
            // 
            this.chkAutoString.AutoSize = true;
            this.chkAutoString.Checked = true;
            this.chkAutoString.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoString.Location = new System.Drawing.Point(105, 89);
            this.chkAutoString.Name = "chkAutoString";
            this.chkAutoString.Size = new System.Drawing.Size(74, 17);
            this.chkAutoString.TabIndex = 15;
            this.chkAutoString.Text = "Automatic";
            this.toolTip1.SetToolTip(this.chkAutoString, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.chkAutoString.UseVisualStyleBackColor = true;
            this.chkAutoString.CheckedChanged += new System.EventHandler(this.chkAutoString_CheckedChanged);
            // 
            // chkBranch
            // 
            this.chkBranch.AutoSize = true;
            this.chkBranch.Checked = true;
            this.chkBranch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBranch.Location = new System.Drawing.Point(105, 135);
            this.chkBranch.Name = "chkBranch";
            this.chkBranch.Size = new System.Drawing.Size(59, 17);
            this.chkBranch.TabIndex = 26;
            this.chkBranch.Text = "Branch";
            this.chkBranch.UseVisualStyleBackColor = true;
            this.chkBranch.CheckedChanged += new System.EventHandler(this.chkBranch_CheckedChanged);
            // 
            // chkSwitch
            // 
            this.chkSwitch.AutoSize = true;
            this.chkSwitch.Checked = true;
            this.chkSwitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSwitch.Location = new System.Drawing.Point(441, 158);
            this.chkSwitch.Name = "chkSwitch";
            this.chkSwitch.Size = new System.Drawing.Size(57, 17);
            this.chkSwitch.TabIndex = 34;
            this.chkSwitch.Text = "Switch";
            this.chkSwitch.UseVisualStyleBackColor = true;
            this.chkSwitch.CheckedChanged += new System.EventHandler(this.chkSwitch_CheckedChanged);
            // 
            // nudLoopCount
            // 
            this.nudLoopCount.Location = new System.Drawing.Point(346, 226);
            this.nudLoopCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLoopCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLoopCount.Name = "nudLoopCount";
            this.nudLoopCount.Size = new System.Drawing.Size(44, 21);
            this.nudLoopCount.TabIndex = 44;
            this.nudLoopCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(279, 228);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Loop Count";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(447, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = ")";
            // 
            // chkUnreachable
            // 
            this.chkUnreachable.AutoSize = true;
            this.chkUnreachable.Checked = true;
            this.chkUnreachable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnreachable.Location = new System.Drawing.Point(105, 181);
            this.chkUnreachable.Name = "chkUnreachable";
            this.chkUnreachable.Size = new System.Drawing.Size(86, 17);
            this.chkUnreachable.TabIndex = 35;
            this.chkUnreachable.Text = "Unreachable";
            this.chkUnreachable.UseVisualStyleBackColor = true;
            // 
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Checked = true;
            this.chkRegex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRegex.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRegex.Location = new System.Drawing.Point(256, 65);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(15, 14);
            this.chkRegex.TabIndex = 10;
            this.chkRegex.UseVisualStyleBackColor = true;
            this.chkRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // txtRegex
            // 
            this.txtRegex.Location = new System.Drawing.Point(346, 62);
            this.txtRegex.Name = "txtRegex";
            this.txtRegex.Size = new System.Drawing.Size(152, 21);
            this.txtRegex.TabIndex = 12;
            // 
            // chkBoolFunction
            // 
            this.chkBoolFunction.AutoSize = true;
            this.chkBoolFunction.Checked = true;
            this.chkBoolFunction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoolFunction.Location = new System.Drawing.Point(105, 113);
            this.chkBoolFunction.Name = "chkBoolFunction";
            this.chkBoolFunction.Size = new System.Drawing.Size(15, 14);
            this.chkBoolFunction.TabIndex = 22;
            this.toolTip1.SetToolTip(this.chkBoolFunction, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.chkBoolFunction.UseVisualStyleBackColor = true;
            // 
            // lblRegexFile
            // 
            this.lblRegexFile.AutoSize = true;
            this.lblRegexFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRegexFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegexFile.Location = new System.Drawing.Point(277, 64);
            this.lblRegexFile.Name = "lblRegexFile";
            this.lblRegexFile.Size = new System.Drawing.Size(65, 13);
            this.lblRegexFile.TabIndex = 11;
            this.lblRegexFile.Text = "Regex (File)";
            this.lblRegexFile.Click += new System.EventHandler(this.lblRegexFile_Click);
            // 
            // lblPatternFile
            // 
            this.lblPatternFile.AutoSize = true;
            this.lblPatternFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPatternFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatternFile.Location = new System.Drawing.Point(240, 112);
            this.lblPatternFile.Name = "lblPatternFile";
            this.lblPatternFile.Size = new System.Drawing.Size(43, 13);
            this.lblPatternFile.TabIndex = 25;
            this.lblPatternFile.Text = "Pattern";
            this.lblPatternFile.Click += new System.EventHandler(this.lblPatternFile_Click);
            // 
            // chkRemoveDummyMethod
            // 
            this.chkRemoveDummyMethod.AutoSize = true;
            this.chkRemoveDummyMethod.Location = new System.Drawing.Point(281, 11);
            this.chkRemoveDummyMethod.Name = "chkRemoveDummyMethod";
            this.chkRemoveDummyMethod.Size = new System.Drawing.Size(140, 17);
            this.chkRemoveDummyMethod.TabIndex = 3;
            this.chkRemoveDummyMethod.Text = "Remove dummy method";
            this.chkRemoveDummyMethod.UseVisualStyleBackColor = true;
            // 
            // chkCondBranchDown
            // 
            this.chkCondBranchDown.AutoSize = true;
            this.chkCondBranchDown.Checked = true;
            this.chkCondBranchDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCondBranchDown.Location = new System.Drawing.Point(105, 158);
            this.chkCondBranchDown.Name = "chkCondBranchDown";
            this.chkCondBranchDown.Size = new System.Drawing.Size(153, 17);
            this.chkCondBranchDown.TabIndex = 32;
            this.chkCondBranchDown.Text = "Conditional Branch (Down)";
            this.chkCondBranchDown.UseVisualStyleBackColor = true;
            this.chkCondBranchDown.CheckedChanged += new System.EventHandler(this.chkCondBranchDown_CheckedChanged);
            // 
            // chkInternalToPublic
            // 
            this.chkInternalToPublic.AutoSize = true;
            this.chkInternalToPublic.Location = new System.Drawing.Point(281, 32);
            this.chkInternalToPublic.Name = "chkInternalToPublic";
            this.chkInternalToPublic.Size = new System.Drawing.Size(267, 17);
            this.chkInternalToPublic.TabIndex = 5;
            this.chkInternalToPublic.Text = "Change internal/private type/method/field to public";
            this.chkInternalToPublic.UseVisualStyleBackColor = true;
            // 
            // chkRemoveSealed
            // 
            this.chkRemoveSealed.AutoSize = true;
            this.chkRemoveSealed.Location = new System.Drawing.Point(106, 32);
            this.chkRemoveSealed.Name = "chkRemoveSealed";
            this.chkRemoveSealed.Size = new System.Drawing.Size(139, 17);
            this.chkRemoveSealed.TabIndex = 4;
            this.chkRemoveSealed.Text = "Remove sealed modifier";
            this.chkRemoveSealed.UseVisualStyleBackColor = true;
            // 
            // chkInitLocalVars
            // 
            this.chkInitLocalVars.AutoSize = true;
            this.chkInitLocalVars.Location = new System.Drawing.Point(106, 53);
            this.chkInitLocalVars.Name = "chkInitLocalVars";
            this.chkInitLocalVars.Size = new System.Drawing.Size(110, 17);
            this.chkInitLocalVars.TabIndex = 6;
            this.chkInitLocalVars.Text = "Init local variables";
            this.chkInitLocalVars.UseVisualStyleBackColor = true;
            // 
            // chkRemoveExceptionHandler
            // 
            this.chkRemoveExceptionHandler.AutoSize = true;
            this.chkRemoveExceptionHandler.Checked = true;
            this.chkRemoveExceptionHandler.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveExceptionHandler.Location = new System.Drawing.Point(441, 182);
            this.chkRemoveExceptionHandler.Name = "chkRemoveExceptionHandler";
            this.chkRemoveExceptionHandler.Size = new System.Drawing.Size(15, 14);
            this.chkRemoveExceptionHandler.TabIndex = 37;
            this.chkRemoveExceptionHandler.UseVisualStyleBackColor = true;
            // 
            // lblRemoveExceptionHandler
            // 
            this.lblRemoveExceptionHandler.AutoSize = true;
            this.lblRemoveExceptionHandler.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRemoveExceptionHandler.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemoveExceptionHandler.Location = new System.Drawing.Point(458, 182);
            this.lblRemoveExceptionHandler.Name = "lblRemoveExceptionHandler";
            this.lblRemoveExceptionHandler.Size = new System.Drawing.Size(135, 13);
            this.lblRemoveExceptionHandler.TabIndex = 38;
            this.lblRemoveExceptionHandler.Text = "Remove exception handler";
            this.lblRemoveExceptionHandler.Click += new System.EventHandler(this.lblRemoveExceptionHandler_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(169, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "(  Max. Ref.";
            // 
            // nudMaxRefCount
            // 
            this.nudMaxRefCount.Location = new System.Drawing.Point(239, 133);
            this.nudMaxRefCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxRefCount.Name = "nudMaxRefCount";
            this.nudMaxRefCount.Size = new System.Drawing.Size(44, 21);
            this.nudMaxRefCount.TabIndex = 28;
            this.nudMaxRefCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblBooleanFunction
            // 
            this.lblBooleanFunction.AutoSize = true;
            this.lblBooleanFunction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBooleanFunction.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBooleanFunction.Location = new System.Drawing.Point(121, 112);
            this.lblBooleanFunction.Name = "lblBooleanFunction";
            this.lblBooleanFunction.Size = new System.Drawing.Size(89, 13);
            this.lblBooleanFunction.TabIndex = 23;
            this.lblBooleanFunction.Text = "Boolean Function";
            this.toolTip1.SetToolTip(this.lblBooleanFunction, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.lblBooleanFunction.Click += new System.EventHandler(this.lblBooleanFunction_Click);
            // 
            // lblStringOption
            // 
            this.lblStringOption.AutoSize = true;
            this.lblStringOption.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStringOption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStringOption.Location = new System.Drawing.Point(11, 88);
            this.lblStringOption.Name = "lblStringOption";
            this.lblStringOption.Size = new System.Drawing.Size(75, 13);
            this.lblStringOption.TabIndex = 14;
            this.lblStringOption.Text = "String Options";
            this.lblStringOption.Click += new System.EventHandler(this.lblStringOption_Click);
            // 
            // lblRemoveAttribute
            // 
            this.lblRemoveAttribute.AutoSize = true;
            this.lblRemoveAttribute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRemoveAttribute.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemoveAttribute.Location = new System.Drawing.Point(122, 12);
            this.lblRemoveAttribute.Name = "lblRemoveAttribute";
            this.lblRemoveAttribute.Size = new System.Drawing.Size(91, 13);
            this.lblRemoveAttribute.TabIndex = 2;
            this.lblRemoveAttribute.Text = "Remove attribute";
            this.lblRemoveAttribute.Click += new System.EventHandler(this.lblRemoveAttribute_Click);
            // 
            // cboProfile
            // 
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.FormattingEnabled = true;
            this.cboProfile.Location = new System.Drawing.Point(105, 10);
            this.cboProfile.MaxDropDownItems = 20;
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(298, 21);
            this.cboProfile.TabIndex = 1;
            // 
            // lblProfile
            // 
            this.lblProfile.AutoSize = true;
            this.lblProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblProfile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfile.Location = new System.Drawing.Point(12, 16);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(37, 13);
            this.lblProfile.TabIndex = 0;
            this.lblProfile.Text = "Profile";
            this.lblProfile.Click += new System.EventHandler(this.lblProfile_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // chkDelegateCall
            // 
            this.chkDelegateCall.AutoSize = true;
            this.chkDelegateCall.Location = new System.Drawing.Point(105, 204);
            this.chkDelegateCall.Name = "chkDelegateCall";
            this.chkDelegateCall.Size = new System.Drawing.Size(89, 17);
            this.chkDelegateCall.TabIndex = 39;
            this.chkDelegateCall.Text = "Delegate Call";
            this.toolTip1.SetToolTip(this.chkDelegateCall, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.chkDelegateCall.UseVisualStyleBackColor = true;
            // 
            // cboDirection
            // 
            this.cboDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDirection.FormattingEnabled = true;
            this.cboDirection.Location = new System.Drawing.Point(346, 131);
            this.cboDirection.MaxDropDownItems = 20;
            this.cboDirection.Name = "cboDirection";
            this.cboDirection.Size = new System.Drawing.Size(98, 21);
            this.cboDirection.TabIndex = 30;
            this.toolTip1.SetToolTip(this.cboDirection, "TopDown is default direction");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(293, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Direction";
            // 
            // lblIgnoredTypeFile
            // 
            this.lblIgnoredTypeFile.AutoSize = true;
            this.lblIgnoredTypeFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblIgnoredTypeFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIgnoredTypeFile.Location = new System.Drawing.Point(487, 13);
            this.lblIgnoredTypeFile.Name = "lblIgnoredTypeFile";
            this.lblIgnoredTypeFile.Size = new System.Drawing.Size(91, 13);
            this.lblIgnoredTypeFile.TabIndex = 2;
            this.lblIgnoredTypeFile.Text = "Ignored Type File";
            this.lblIgnoredTypeFile.Click += new System.EventHandler(this.lblIgnoredTypeFile_Click);
            // 
            // chkCondBranchUp
            // 
            this.chkCondBranchUp.AutoSize = true;
            this.chkCondBranchUp.Location = new System.Drawing.Point(280, 158);
            this.chkCondBranchUp.Name = "chkCondBranchUp";
            this.chkCondBranchUp.Size = new System.Drawing.Size(139, 17);
            this.chkCondBranchUp.TabIndex = 33;
            this.chkCondBranchUp.Text = "Conditional Branch (Up)";
            this.chkCondBranchUp.UseVisualStyleBackColor = true;
            this.chkCondBranchUp.CheckedChanged += new System.EventHandler(this.chkCondBranchUp_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabAdditional);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(637, 288);
            this.tabControl1.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tabGeneral.Controls.Add(this.chkRemoveInvalidInstruction);
            this.tabGeneral.Controls.Add(this.chkHexRename);
            this.tabGeneral.Controls.Add(this.label10);
            this.tabGeneral.Controls.Add(this.txtCalledMethod);
            this.tabGeneral.Controls.Add(this.btnSelectCalledMethod);
            this.tabGeneral.Controls.Add(this.chkDirectCall);
            this.tabGeneral.Controls.Add(this.chkBlockMove);
            this.tabGeneral.Controls.Add(this.chkReflectorFix);
            this.tabGeneral.Controls.Add(this.chkDelegateCall);
            this.tabGeneral.Controls.Add(this.lblRandom);
            this.tabGeneral.Controls.Add(this.txtOutputDir);
            this.tabGeneral.Controls.Add(this.lblRegexFile);
            this.tabGeneral.Controls.Add(this.chkCondBranchUp);
            this.tabGeneral.Controls.Add(this.label7);
            this.tabGeneral.Controls.Add(this.lblIgnoredTypeFile);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.cboDirection);
            this.tabGeneral.Controls.Add(this.btnSelectDir);
            this.tabGeneral.Controls.Add(this.label4);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.lblProfile);
            this.tabGeneral.Controls.Add(this.txtMethod);
            this.tabGeneral.Controls.Add(this.cboProfile);
            this.tabGeneral.Controls.Add(this.btnSelectMethod);
            this.tabGeneral.Controls.Add(this.chkNonAscii);
            this.tabGeneral.Controls.Add(this.lblBooleanFunction);
            this.tabGeneral.Controls.Add(this.label3);
            this.tabGeneral.Controls.Add(this.lblStringOption);
            this.tabGeneral.Controls.Add(this.chkPattern);
            this.tabGeneral.Controls.Add(this.label8);
            this.tabGeneral.Controls.Add(this.chkRandomName);
            this.tabGeneral.Controls.Add(this.nudMaxRefCount);
            this.tabGeneral.Controls.Add(this.chkAutoString);
            this.tabGeneral.Controls.Add(this.lblRemoveExceptionHandler);
            this.tabGeneral.Controls.Add(this.chkBranch);
            this.tabGeneral.Controls.Add(this.chkRemoveExceptionHandler);
            this.tabGeneral.Controls.Add(this.chkSwitch);
            this.tabGeneral.Controls.Add(this.nudLoopCount);
            this.tabGeneral.Controls.Add(this.label6);
            this.tabGeneral.Controls.Add(this.chkUnreachable);
            this.tabGeneral.Controls.Add(this.chkCondBranchDown);
            this.tabGeneral.Controls.Add(this.chkRegex);
            this.tabGeneral.Controls.Add(this.txtRegex);
            this.tabGeneral.Controls.Add(this.lblPatternFile);
            this.tabGeneral.Controls.Add(this.chkBoolFunction);
            this.tabGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(629, 259);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General Options";
            // 
            // chkHexRename
            // 
            this.chkHexRename.AutoSize = true;
            this.chkHexRename.Location = new System.Drawing.Point(504, 64);
            this.chkHexRename.Name = "chkHexRename";
            this.chkHexRename.Size = new System.Drawing.Size(87, 17);
            this.chkHexRename.TabIndex = 13;
            this.chkHexRename.Text = "Hex Rename";
            this.chkHexRename.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(350, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "replacement call";
            // 
            // txtCalledMethod
            // 
            this.txtCalledMethod.Enabled = false;
            this.txtCalledMethod.Location = new System.Drawing.Point(435, 87);
            this.txtCalledMethod.Name = "txtCalledMethod";
            this.txtCalledMethod.Size = new System.Drawing.Size(143, 21);
            this.txtCalledMethod.TabIndex = 19;
            // 
            // btnSelectCalledMethod
            // 
            this.btnSelectCalledMethod.Enabled = false;
            this.btnSelectCalledMethod.Location = new System.Drawing.Point(584, 86);
            this.btnSelectCalledMethod.Name = "btnSelectCalledMethod";
            this.btnSelectCalledMethod.Size = new System.Drawing.Size(27, 21);
            this.btnSelectCalledMethod.TabIndex = 20;
            this.btnSelectCalledMethod.Text = "...";
            this.btnSelectCalledMethod.UseVisualStyleBackColor = true;
            this.btnSelectCalledMethod.Click += new System.EventHandler(this.btnSelectCalledMethod_Click);
            // 
            // chkDirectCall
            // 
            this.chkDirectCall.AutoSize = true;
            this.chkDirectCall.Location = new System.Drawing.Point(280, 204);
            this.chkDirectCall.Name = "chkDirectCall";
            this.chkDirectCall.Size = new System.Drawing.Size(74, 17);
            this.chkDirectCall.TabIndex = 40;
            this.chkDirectCall.Text = "Direct Call";
            this.chkDirectCall.UseVisualStyleBackColor = true;
            // 
            // chkBlockMove
            // 
            this.chkBlockMove.AutoSize = true;
            this.chkBlockMove.Location = new System.Drawing.Point(280, 181);
            this.chkBlockMove.Name = "chkBlockMove";
            this.chkBlockMove.Size = new System.Drawing.Size(79, 17);
            this.chkBlockMove.TabIndex = 36;
            this.chkBlockMove.Text = "Block Move";
            this.chkBlockMove.UseVisualStyleBackColor = true;
            // 
            // chkReflectorFix
            // 
            this.chkReflectorFix.AutoSize = true;
            this.chkReflectorFix.Checked = true;
            this.chkReflectorFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReflectorFix.Location = new System.Drawing.Point(105, 227);
            this.chkReflectorFix.Name = "chkReflectorFix";
            this.chkReflectorFix.Size = new System.Drawing.Size(87, 17);
            this.chkReflectorFix.TabIndex = 42;
            this.chkReflectorFix.Text = "Reflector Fix";
            this.chkReflectorFix.UseVisualStyleBackColor = true;
            // 
            // lblRandom
            // 
            this.lblRandom.AutoSize = true;
            this.lblRandom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRandom.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRandom.Location = new System.Drawing.Point(201, 64);
            this.lblRandom.Name = "lblRandom";
            this.lblRandom.Size = new System.Drawing.Size(46, 13);
            this.lblRandom.TabIndex = 9;
            this.lblRandom.Text = "Random";
            this.lblRandom.Click += new System.EventHandler(this.lblRandom_Click);
            // 
            // tabAdditional
            // 
            this.tabAdditional.BackColor = System.Drawing.SystemColors.Control;
            this.tabAdditional.Controls.Add(this.chkAddMissingPropertyAndEvent);
            this.tabAdditional.Controls.Add(this.label9);
            this.tabAdditional.Controls.Add(this.dgvPlugin);
            this.tabAdditional.Controls.Add(this.chkRemoveSealed);
            this.tabAdditional.Controls.Add(this.lblRemoveAttribute);
            this.tabAdditional.Controls.Add(this.label5);
            this.tabAdditional.Controls.Add(this.chkInitLocalVars);
            this.tabAdditional.Controls.Add(this.chkRemoveAttribute);
            this.tabAdditional.Controls.Add(this.chkRemoveDummyMethod);
            this.tabAdditional.Controls.Add(this.chkInternalToPublic);
            this.tabAdditional.Location = new System.Drawing.Point(4, 25);
            this.tabAdditional.Name = "tabAdditional";
            this.tabAdditional.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdditional.Size = new System.Drawing.Size(629, 259);
            this.tabAdditional.TabIndex = 1;
            this.tabAdditional.Text = "Additional Options";
            // 
            // chkAddMissingPropertyAndEvent
            // 
            this.chkAddMissingPropertyAndEvent.AutoSize = true;
            this.chkAddMissingPropertyAndEvent.Location = new System.Drawing.Point(281, 53);
            this.chkAddMissingPropertyAndEvent.Name = "chkAddMissingPropertyAndEvent";
            this.chkAddMissingPropertyAndEvent.Size = new System.Drawing.Size(155, 17);
            this.chkAddMissingPropertyAndEvent.TabIndex = 7;
            this.chkAddMissingPropertyAndEvent.Text = "Add missing property/event";
            this.chkAddMissingPropertyAndEvent.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(180, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Plugin List (double click to configure)";
            // 
            // dgvPlugin
            // 
            this.dgvPlugin.AllowUserToAddRows = false;
            this.dgvPlugin.AllowUserToDeleteRows = false;
            this.dgvPlugin.AllowUserToResizeRows = false;
            this.dgvPlugin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPlugin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlugin.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcSelected,
            this.dgcTitle,
            this.dgcVersion,
            this.dgcAuthor});
            this.dgvPlugin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvPlugin.Location = new System.Drawing.Point(3, 89);
            this.dgvPlugin.MultiSelect = false;
            this.dgvPlugin.Name = "dgvPlugin";
            this.dgvPlugin.RowHeadersVisible = false;
            this.dgvPlugin.RowHeadersWidth = 21;
            this.dgvPlugin.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlugin.Size = new System.Drawing.Size(623, 167);
            this.dgvPlugin.TabIndex = 9;
            this.dgvPlugin.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPlugin_CellDoubleClick);
            // 
            // dgvcSelected
            // 
            this.dgvcSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvcSelected.DataPropertyName = "Selected";
            this.dgvcSelected.HeaderText = "";
            this.dgvcSelected.MinimumWidth = 20;
            this.dgvcSelected.Name = "dgvcSelected";
            this.dgvcSelected.Width = 30;
            // 
            // dgcTitle
            // 
            this.dgcTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcTitle.DataPropertyName = "Title";
            this.dgcTitle.HeaderText = "Title";
            this.dgcTitle.MinimumWidth = 200;
            this.dgcTitle.Name = "dgcTitle";
            this.dgcTitle.ReadOnly = true;
            // 
            // dgcVersion
            // 
            this.dgcVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcVersion.DataPropertyName = "Version";
            this.dgcVersion.HeaderText = "Version";
            this.dgcVersion.MinimumWidth = 80;
            this.dgcVersion.Name = "dgcVersion";
            this.dgcVersion.ReadOnly = true;
            this.dgcVersion.Width = 80;
            // 
            // dgcAuthor
            // 
            this.dgcAuthor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgcAuthor.DataPropertyName = "Author";
            this.dgcAuthor.HeaderText = "Author";
            this.dgcAuthor.MinimumWidth = 100;
            this.dgcAuthor.Name = "dgcAuthor";
            this.dgcAuthor.ReadOnly = true;
            // 
            // chkRemoveInvalidInstruction
            // 
            this.chkRemoveInvalidInstruction.AutoSize = true;
            this.chkRemoveInvalidInstruction.Location = new System.Drawing.Point(441, 204);
            this.chkRemoveInvalidInstruction.Name = "chkRemoveInvalidInstruction";
            this.chkRemoveInvalidInstruction.Size = new System.Drawing.Size(155, 17);
            this.chkRemoveInvalidInstruction.TabIndex = 41;
            this.chkRemoveInvalidInstruction.Text = "Remove Invalid Instruction";
            this.chkRemoveInvalidInstruction.UseVisualStyleBackColor = true;
            // 
            // frmDeobf
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(637, 560);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmDeobf";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Deobfuscator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDeobf_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudLoopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRefCount)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabAdditional.ResumeLayout(false);
            this.tabAdditional.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnSelectDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectMethod;
        private System.Windows.Forms.TextBox txtMethod;
        private System.Windows.Forms.CheckBox chkNonAscii;
        private System.Windows.Forms.CheckBox chkPattern;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkRandomName;
        private System.Windows.Forms.CheckBox chkRemoveAttribute;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkAutoString;
        private System.Windows.Forms.CheckBox chkBranch;
        private System.Windows.Forms.CheckBox chkSwitch;
        private System.Windows.Forms.NumericUpDown nudLoopCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkUnreachable;
        private System.Windows.Forms.CheckBox chkRegex;
        private System.Windows.Forms.TextBox txtRegex;
        private System.Windows.Forms.CheckBox chkBoolFunction;
        private System.Windows.Forms.Label lblRegexFile;
        private System.Windows.Forms.Label lblPatternFile;
        private System.Windows.Forms.CheckBox chkRemoveDummyMethod;
        private System.Windows.Forms.CheckBox chkCondBranchDown;
        private System.Windows.Forms.CheckBox chkInternalToPublic;
        private System.Windows.Forms.CheckBox chkRemoveSealed;
        private System.Windows.Forms.CheckBox chkInitLocalVars;
        private System.Windows.Forms.CheckBox chkRemoveExceptionHandler;
        private System.Windows.Forms.Label lblRemoveExceptionHandler;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudMaxRefCount;
        private System.Windows.Forms.Label lblBooleanFunction;
        private System.Windows.Forms.Label lblStringOption;
        private System.Windows.Forms.Label lblRemoveAttribute;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboDirection;
        private System.Windows.Forms.Label lblIgnoredTypeFile;
        private System.Windows.Forms.CheckBox chkCondBranchUp;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabAdditional;
        private System.Windows.Forms.DataGridView dgvPlugin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAuthor;
        private System.Windows.Forms.Label lblRandom;
        private System.Windows.Forms.CheckBox chkAddMissingPropertyAndEvent;
        private System.Windows.Forms.CheckBox chkDelegateCall;
        private System.Windows.Forms.CheckBox chkReflectorFix;
        private System.Windows.Forms.CheckBox chkBlockMove;
        private System.Windows.Forms.CheckBox chkDirectCall;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtCalledMethod;
        private System.Windows.Forms.Button btnSelectCalledMethod;
        private System.Windows.Forms.CheckBox chkHexRename;
        private System.Windows.Forms.CheckBox chkRemoveInvalidInstruction;
    }
}
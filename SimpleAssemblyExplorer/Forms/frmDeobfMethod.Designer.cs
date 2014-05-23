namespace SimpleAssemblyExplorer
{
    partial class frmDeobfMethod
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkPattern = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboMethods = new System.Windows.Forms.ComboBox();
            this.chkBranch = new System.Windows.Forms.CheckBox();
            this.chkSwitch = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudLoopCount = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.chkUnreachable = new System.Windows.Forms.CheckBox();
            this.chkBoolFunction = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMaxMoveCount = new System.Windows.Forms.NumericUpDown();
            this.chkCondBranchDown = new System.Windows.Forms.CheckBox();
            this.lblRemoveExceptionHandler = new System.Windows.Forms.Label();
            this.chkRemoveExceptionHandler = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nudMaxRefCount = new System.Windows.Forms.NumericUpDown();
            this.lblPatternFile = new System.Windows.Forms.Label();
            this.lblStringOption = new System.Windows.Forms.Label();
            this.lblBooleanFunction = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkDelegateCall = new System.Windows.Forms.CheckBox();
            this.cboDirection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.chkCondBranchUp = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneralOptions = new System.Windows.Forms.TabPage();
            this.chkDirectCall = new System.Windows.Forms.CheckBox();
            this.chkBlockMove = new System.Windows.Forms.CheckBox();
            this.chkReflectorFix = new System.Windows.Forms.CheckBox();
            this.tabAdditionalOptions = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.chkInitLocalVars = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dgvPlugin = new System.Windows.Forms.DataGridView();
            this.dgvcSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgcTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkRemoveInvalidInstruction = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudLoopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMoveCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRefCount)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabGeneralOptions.SuspendLayout();
            this.tabAdditionalOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugin)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(184, 293);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(295, 293);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkPattern
            // 
            this.chkPattern.AutoSize = true;
            this.chkPattern.Checked = true;
            this.chkPattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPattern.Location = new System.Drawing.Point(292, 60);
            this.chkPattern.Name = "chkPattern";
            this.chkPattern.Size = new System.Drawing.Size(15, 14);
            this.chkPattern.TabIndex = 7;
            this.chkPattern.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Flow Options";
            // 
            // cboMethods
            // 
            this.cboMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMethods.FormattingEnabled = true;
            this.cboMethods.Location = new System.Drawing.Point(103, 34);
            this.cboMethods.Name = "cboMethods";
            this.cboMethods.Size = new System.Drawing.Size(432, 21);
            this.cboMethods.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cboMethods, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            // 
            // chkBranch
            // 
            this.chkBranch.AutoSize = true;
            this.chkBranch.Checked = true;
            this.chkBranch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBranch.Location = new System.Drawing.Point(103, 84);
            this.chkBranch.Name = "chkBranch";
            this.chkBranch.Size = new System.Drawing.Size(59, 17);
            this.chkBranch.TabIndex = 9;
            this.chkBranch.Text = "Branch";
            this.chkBranch.UseVisualStyleBackColor = true;
            this.chkBranch.CheckedChanged += new System.EventHandler(this.chkBranch_CheckedChanged);
            // 
            // chkSwitch
            // 
            this.chkSwitch.AutoSize = true;
            this.chkSwitch.Checked = true;
            this.chkSwitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSwitch.Location = new System.Drawing.Point(103, 130);
            this.chkSwitch.Name = "chkSwitch";
            this.chkSwitch.Size = new System.Drawing.Size(57, 17);
            this.chkSwitch.TabIndex = 18;
            this.chkSwitch.Text = "Switch";
            this.chkSwitch.UseVisualStyleBackColor = true;
            this.chkSwitch.CheckedChanged += new System.EventHandler(this.chkSwitch_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(168, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "(";
            // 
            // nudLoopCount
            // 
            this.nudLoopCount.Location = new System.Drawing.Point(171, 221);
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
            this.nudLoopCount.Size = new System.Drawing.Size(49, 21);
            this.nudLoopCount.TabIndex = 28;
            this.nudLoopCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(439, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = ")";
            // 
            // chkUnreachable
            // 
            this.chkUnreachable.AutoSize = true;
            this.chkUnreachable.Checked = true;
            this.chkUnreachable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnreachable.Location = new System.Drawing.Point(292, 130);
            this.chkUnreachable.Name = "chkUnreachable";
            this.chkUnreachable.Size = new System.Drawing.Size(86, 17);
            this.chkUnreachable.TabIndex = 19;
            this.chkUnreachable.Text = "Unreachable";
            this.chkUnreachable.UseVisualStyleBackColor = true;
            // 
            // chkBoolFunction
            // 
            this.chkBoolFunction.AutoSize = true;
            this.chkBoolFunction.Location = new System.Drawing.Point(103, 62);
            this.chkBoolFunction.Name = "chkBoolFunction";
            this.chkBoolFunction.Size = new System.Drawing.Size(15, 14);
            this.chkBoolFunction.TabIndex = 5;
            this.toolTip1.SetToolTip(this.chkBoolFunction, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.chkBoolFunction.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(101, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Loop Count";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(289, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Max. Move";
            // 
            // nudMaxMoveCount
            // 
            this.nudMaxMoveCount.Location = new System.Drawing.Point(355, 221);
            this.nudMaxMoveCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxMoveCount.Name = "nudMaxMoveCount";
            this.nudMaxMoveCount.Size = new System.Drawing.Size(49, 21);
            this.nudMaxMoveCount.TabIndex = 30;
            this.toolTip1.SetToolTip(this.nudMaxMoveCount, "For Branch and Conditional Branch, 0 for unlimited");
            // 
            // chkCondBranchDown
            // 
            this.chkCondBranchDown.AutoSize = true;
            this.chkCondBranchDown.Checked = true;
            this.chkCondBranchDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCondBranchDown.Location = new System.Drawing.Point(103, 107);
            this.chkCondBranchDown.Name = "chkCondBranchDown";
            this.chkCondBranchDown.Size = new System.Drawing.Size(153, 17);
            this.chkCondBranchDown.TabIndex = 16;
            this.chkCondBranchDown.Text = "Conditional Branch (Down)";
            this.chkCondBranchDown.UseVisualStyleBackColor = true;
            this.chkCondBranchDown.CheckedChanged += new System.EventHandler(this.chkCondBranch_CheckedChanged);
            // 
            // lblRemoveExceptionHandler
            // 
            this.lblRemoveExceptionHandler.AutoSize = true;
            this.lblRemoveExceptionHandler.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRemoveExceptionHandler.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemoveExceptionHandler.Location = new System.Drawing.Point(308, 153);
            this.lblRemoveExceptionHandler.Name = "lblRemoveExceptionHandler";
            this.lblRemoveExceptionHandler.Size = new System.Drawing.Size(135, 13);
            this.lblRemoveExceptionHandler.TabIndex = 22;
            this.lblRemoveExceptionHandler.Text = "Remove exception handler";
            this.lblRemoveExceptionHandler.Click += new System.EventHandler(this.lblRemoveExceptionHandler_Click);
            // 
            // chkRemoveExceptionHandler
            // 
            this.chkRemoveExceptionHandler.AutoSize = true;
            this.chkRemoveExceptionHandler.Checked = true;
            this.chkRemoveExceptionHandler.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveExceptionHandler.Location = new System.Drawing.Point(292, 154);
            this.chkRemoveExceptionHandler.Name = "chkRemoveExceptionHandler";
            this.chkRemoveExceptionHandler.Size = new System.Drawing.Size(15, 14);
            this.chkRemoveExceptionHandler.TabIndex = 21;
            this.chkRemoveExceptionHandler.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(177, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Max. Ref.";
            // 
            // nudMaxRefCount
            // 
            this.nudMaxRefCount.Location = new System.Drawing.Point(238, 81);
            this.nudMaxRefCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxRefCount.Name = "nudMaxRefCount";
            this.nudMaxRefCount.Size = new System.Drawing.Size(44, 21);
            this.nudMaxRefCount.TabIndex = 12;
            this.nudMaxRefCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblPatternFile
            // 
            this.lblPatternFile.AutoSize = true;
            this.lblPatternFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPatternFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatternFile.Location = new System.Drawing.Point(313, 61);
            this.lblPatternFile.Name = "lblPatternFile";
            this.lblPatternFile.Size = new System.Drawing.Size(43, 13);
            this.lblPatternFile.TabIndex = 8;
            this.lblPatternFile.Text = "Pattern";
            this.lblPatternFile.Click += new System.EventHandler(this.lblPatternFile_Click);
            // 
            // lblStringOption
            // 
            this.lblStringOption.AutoSize = true;
            this.lblStringOption.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStringOption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStringOption.Location = new System.Drawing.Point(9, 37);
            this.lblStringOption.Name = "lblStringOption";
            this.lblStringOption.Size = new System.Drawing.Size(75, 13);
            this.lblStringOption.TabIndex = 2;
            this.lblStringOption.Text = "String Options";
            this.lblStringOption.Click += new System.EventHandler(this.lblStringOption_Click);
            // 
            // lblBooleanFunction
            // 
            this.lblBooleanFunction.AutoSize = true;
            this.lblBooleanFunction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBooleanFunction.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBooleanFunction.Location = new System.Drawing.Point(119, 61);
            this.lblBooleanFunction.Name = "lblBooleanFunction";
            this.lblBooleanFunction.Size = new System.Drawing.Size(89, 13);
            this.lblBooleanFunction.TabIndex = 6;
            this.lblBooleanFunction.Text = "Boolean Function";
            this.toolTip1.SetToolTip(this.lblBooleanFunction, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.lblBooleanFunction.Click += new System.EventHandler(this.lblBooleanFunction_Click);
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
            this.chkDelegateCall.Location = new System.Drawing.Point(103, 175);
            this.chkDelegateCall.Name = "chkDelegateCall";
            this.chkDelegateCall.Size = new System.Drawing.Size(89, 17);
            this.chkDelegateCall.TabIndex = 23;
            this.chkDelegateCall.Text = "Delegate Call";
            this.toolTip1.SetToolTip(this.chkDelegateCall, "This option will load assembly into current appdomain, so you can\'t overwrite the" +
        " file until quit.");
            this.chkDelegateCall.UseVisualStyleBackColor = true;
            // 
            // cboDirection
            // 
            this.cboDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDirection.FormattingEnabled = true;
            this.cboDirection.Location = new System.Drawing.Point(342, 81);
            this.cboDirection.MaxDropDownItems = 20;
            this.cboDirection.Name = "cboDirection";
            this.cboDirection.Size = new System.Drawing.Size(95, 21);
            this.cboDirection.TabIndex = 14;
            this.toolTip1.SetToolTip(this.cboDirection, "TopDown is default direction");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(289, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Direction";
            // 
            // lblProfile
            // 
            this.lblProfile.AutoSize = true;
            this.lblProfile.Location = new System.Drawing.Point(10, 12);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(37, 13);
            this.lblProfile.TabIndex = 0;
            this.lblProfile.Text = "Profile";
            // 
            // cboProfile
            // 
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.FormattingEnabled = true;
            this.cboProfile.Location = new System.Drawing.Point(104, 7);
            this.cboProfile.MaxDropDownItems = 20;
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(275, 21);
            this.cboProfile.TabIndex = 1;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            // 
            // chkCondBranchUp
            // 
            this.chkCondBranchUp.AutoSize = true;
            this.chkCondBranchUp.Location = new System.Drawing.Point(292, 107);
            this.chkCondBranchUp.Name = "chkCondBranchUp";
            this.chkCondBranchUp.Size = new System.Drawing.Size(139, 17);
            this.chkCondBranchUp.TabIndex = 17;
            this.chkCondBranchUp.Text = "Conditional Branch (Up)";
            this.chkCondBranchUp.UseVisualStyleBackColor = true;
            this.chkCondBranchUp.CheckedChanged += new System.EventHandler(this.chkCondBranchUp_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabGeneralOptions);
            this.tabControl1.Controls.Add(this.tabAdditionalOptions);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(554, 289);
            this.tabControl1.TabIndex = 0;
            // 
            // tabGeneralOptions
            // 
            this.tabGeneralOptions.BackColor = System.Drawing.SystemColors.Control;
            this.tabGeneralOptions.Controls.Add(this.chkRemoveInvalidInstruction);
            this.tabGeneralOptions.Controls.Add(this.chkDirectCall);
            this.tabGeneralOptions.Controls.Add(this.chkBlockMove);
            this.tabGeneralOptions.Controls.Add(this.chkReflectorFix);
            this.tabGeneralOptions.Controls.Add(this.chkDelegateCall);
            this.tabGeneralOptions.Controls.Add(this.cboProfile);
            this.tabGeneralOptions.Controls.Add(this.label3);
            this.tabGeneralOptions.Controls.Add(this.chkCondBranchUp);
            this.tabGeneralOptions.Controls.Add(this.chkPattern);
            this.tabGeneralOptions.Controls.Add(this.lblProfile);
            this.tabGeneralOptions.Controls.Add(this.cboMethods);
            this.tabGeneralOptions.Controls.Add(this.chkBranch);
            this.tabGeneralOptions.Controls.Add(this.cboDirection);
            this.tabGeneralOptions.Controls.Add(this.chkSwitch);
            this.tabGeneralOptions.Controls.Add(this.label4);
            this.tabGeneralOptions.Controls.Add(this.label7);
            this.tabGeneralOptions.Controls.Add(this.lblBooleanFunction);
            this.tabGeneralOptions.Controls.Add(this.nudLoopCount);
            this.tabGeneralOptions.Controls.Add(this.lblStringOption);
            this.tabGeneralOptions.Controls.Add(this.label6);
            this.tabGeneralOptions.Controls.Add(this.lblPatternFile);
            this.tabGeneralOptions.Controls.Add(this.chkUnreachable);
            this.tabGeneralOptions.Controls.Add(this.label8);
            this.tabGeneralOptions.Controls.Add(this.chkBoolFunction);
            this.tabGeneralOptions.Controls.Add(this.nudMaxRefCount);
            this.tabGeneralOptions.Controls.Add(this.label1);
            this.tabGeneralOptions.Controls.Add(this.nudMaxMoveCount);
            this.tabGeneralOptions.Controls.Add(this.lblRemoveExceptionHandler);
            this.tabGeneralOptions.Controls.Add(this.label2);
            this.tabGeneralOptions.Controls.Add(this.chkRemoveExceptionHandler);
            this.tabGeneralOptions.Controls.Add(this.chkCondBranchDown);
            this.tabGeneralOptions.Location = new System.Drawing.Point(4, 25);
            this.tabGeneralOptions.Name = "tabGeneralOptions";
            this.tabGeneralOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneralOptions.Size = new System.Drawing.Size(546, 260);
            this.tabGeneralOptions.TabIndex = 0;
            this.tabGeneralOptions.Text = "General Options";
            // 
            // chkDirectCall
            // 
            this.chkDirectCall.AutoSize = true;
            this.chkDirectCall.Location = new System.Drawing.Point(292, 175);
            this.chkDirectCall.Name = "chkDirectCall";
            this.chkDirectCall.Size = new System.Drawing.Size(74, 17);
            this.chkDirectCall.TabIndex = 24;
            this.chkDirectCall.Text = "Direct Call";
            this.chkDirectCall.UseVisualStyleBackColor = true;
            // 
            // chkBlockMove
            // 
            this.chkBlockMove.AutoSize = true;
            this.chkBlockMove.Location = new System.Drawing.Point(103, 152);
            this.chkBlockMove.Name = "chkBlockMove";
            this.chkBlockMove.Size = new System.Drawing.Size(79, 17);
            this.chkBlockMove.TabIndex = 20;
            this.chkBlockMove.Text = "Block Move";
            this.chkBlockMove.UseVisualStyleBackColor = true;
            // 
            // chkReflectorFix
            // 
            this.chkReflectorFix.AutoSize = true;
            this.chkReflectorFix.Checked = true;
            this.chkReflectorFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReflectorFix.Location = new System.Drawing.Point(292, 198);
            this.chkReflectorFix.Name = "chkReflectorFix";
            this.chkReflectorFix.Size = new System.Drawing.Size(87, 17);
            this.chkReflectorFix.TabIndex = 26;
            this.chkReflectorFix.Text = "Reflector Fix";
            this.chkReflectorFix.UseVisualStyleBackColor = true;
            // 
            // tabAdditionalOptions
            // 
            this.tabAdditionalOptions.BackColor = System.Drawing.SystemColors.Control;
            this.tabAdditionalOptions.Controls.Add(this.label5);
            this.tabAdditionalOptions.Controls.Add(this.chkInitLocalVars);
            this.tabAdditionalOptions.Controls.Add(this.label9);
            this.tabAdditionalOptions.Controls.Add(this.dgvPlugin);
            this.tabAdditionalOptions.Location = new System.Drawing.Point(4, 25);
            this.tabAdditionalOptions.Name = "tabAdditionalOptions";
            this.tabAdditionalOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdditionalOptions.Size = new System.Drawing.Size(546, 260);
            this.tabAdditionalOptions.TabIndex = 1;
            this.tabAdditionalOptions.Text = "Additional Options";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Other Options";
            // 
            // chkInitLocalVars
            // 
            this.chkInitLocalVars.AutoSize = true;
            this.chkInitLocalVars.Location = new System.Drawing.Point(98, 6);
            this.chkInitLocalVars.Name = "chkInitLocalVars";
            this.chkInitLocalVars.Size = new System.Drawing.Size(110, 17);
            this.chkInitLocalVars.TabIndex = 1;
            this.chkInitLocalVars.Text = "Init local variables";
            this.chkInitLocalVars.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(180, 13);
            this.label9.TabIndex = 2;
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
            this.dgvPlugin.Location = new System.Drawing.Point(3, 42);
            this.dgvPlugin.MultiSelect = false;
            this.dgvPlugin.Name = "dgvPlugin";
            this.dgvPlugin.RowHeadersVisible = false;
            this.dgvPlugin.RowHeadersWidth = 21;
            this.dgvPlugin.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlugin.Size = new System.Drawing.Size(540, 215);
            this.dgvPlugin.TabIndex = 3;
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
            this.chkRemoveInvalidInstruction.Location = new System.Drawing.Point(103, 198);
            this.chkRemoveInvalidInstruction.Name = "chkRemoveInvalidInstruction";
            this.chkRemoveInvalidInstruction.Size = new System.Drawing.Size(155, 17);
            this.chkRemoveInvalidInstruction.TabIndex = 25;
            this.chkRemoveInvalidInstruction.Text = "Remove Invalid Instruction";
            this.chkRemoveInvalidInstruction.UseVisualStyleBackColor = true;
            // 
            // frmDeobfMethod
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(554, 324);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmDeobfMethod";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Deobfuscator";
            ((System.ComponentModel.ISupportInitialize)(this.nudLoopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMoveCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRefCount)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabGeneralOptions.ResumeLayout(false);
            this.tabGeneralOptions.PerformLayout();
            this.tabAdditionalOptions.ResumeLayout(false);
            this.tabAdditionalOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlugin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkPattern;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboMethods;
        private System.Windows.Forms.CheckBox chkBranch;
        private System.Windows.Forms.CheckBox chkSwitch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudLoopCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkUnreachable;
        private System.Windows.Forms.CheckBox chkBoolFunction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMaxMoveCount;
        private System.Windows.Forms.CheckBox chkCondBranchDown;
        private System.Windows.Forms.Label lblRemoveExceptionHandler;
        private System.Windows.Forms.CheckBox chkRemoveExceptionHandler;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudMaxRefCount;
        private System.Windows.Forms.Label lblPatternFile;
        private System.Windows.Forms.Label lblStringOption;
        private System.Windows.Forms.Label lblBooleanFunction;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cboDirection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.CheckBox chkCondBranchUp;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneralOptions;
        private System.Windows.Forms.TabPage tabAdditionalOptions;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dgvPlugin;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAuthor;
        private System.Windows.Forms.CheckBox chkDelegateCall;
        private System.Windows.Forms.CheckBox chkReflectorFix;
        private System.Windows.Forms.CheckBox chkBlockMove;
        private System.Windows.Forms.CheckBox chkDirectCall;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkInitLocalVars;
        private System.Windows.Forms.CheckBox chkRemoveInvalidInstruction;
    }
}
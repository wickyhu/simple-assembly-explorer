namespace SimpleAssemblyExplorer
{
    partial class frmProfilerApp
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
            this.btnSelectApp = new System.Windows.Forms.Button();
            this.txtApp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtArg = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.chkIncludeSystem = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLogSize = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnOpenLogFile = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.chkTraceParam = new System.Windows.Forms.CheckBox();
            this.chkTraceEvent = new System.Windows.Forms.CheckBox();
            this.btnDeleteLogFile = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkLogEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSelectApp
            // 
            this.btnSelectApp.Location = new System.Drawing.Point(541, 12);
            this.btnSelectApp.Name = "btnSelectApp";
            this.btnSelectApp.Size = new System.Drawing.Size(27, 21);
            this.btnSelectApp.TabIndex = 1;
            this.btnSelectApp.Text = "...";
            this.toolTip1.SetToolTip(this.btnSelectApp, "Select Application");
            this.btnSelectApp.UseVisualStyleBackColor = true;
            this.btnSelectApp.Click += new System.EventHandler(this.btnSelectApp_Click);
            // 
            // txtApp
            // 
            this.txtApp.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtApp.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtApp.Location = new System.Drawing.Point(99, 12);
            this.txtApp.Name = "txtApp";
            this.txtApp.Size = new System.Drawing.Size(436, 21);
            this.txtApp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Application";
            // 
            // txtArg
            // 
            this.txtArg.Location = new System.Drawing.Point(99, 39);
            this.txtArg.Name = "txtArg";
            this.txtArg.Size = new System.Drawing.Size(436, 21);
            this.txtArg.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Arguments";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(145, 221);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(85, 24);
            this.btnStart.TabIndex = 13;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(381, 221);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 24);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(99, 109);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(436, 21);
            this.txtFilter.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Class/Function";
            // 
            // txtLogPath
            // 
            this.txtLogPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtLogPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtLogPath.Location = new System.Drawing.Point(99, 136);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.Size = new System.Drawing.Size(436, 21);
            this.txtLogPath.TabIndex = 7;
            this.txtLogPath.Leave += new System.EventHandler(this.txtLogPath_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Log Path";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Log File";
            // 
            // txtLogFile
            // 
            this.txtLogFile.Location = new System.Drawing.Point(99, 163);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.ReadOnly = true;
            this.txtLogFile.Size = new System.Drawing.Size(403, 21);
            this.txtLogFile.TabIndex = 8;
            // 
            // chkIncludeSystem
            // 
            this.chkIncludeSystem.AutoSize = true;
            this.chkIncludeSystem.Location = new System.Drawing.Point(210, 86);
            this.chkIncludeSystem.Name = "chkIncludeSystem";
            this.chkIncludeSystem.Size = new System.Drawing.Size(188, 17);
            this.chkIncludeSystem.TabIndex = 5;
            this.chkIncludeSystem.Text = "Include System Classes/Functions";
            this.chkIncludeSystem.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(145, 191);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Log Size";
            // 
            // lblLogSize
            // 
            this.lblLogSize.AutoSize = true;
            this.lblLogSize.Location = new System.Drawing.Point(207, 192);
            this.lblLogSize.Name = "lblLogSize";
            this.lblLogSize.Size = new System.Drawing.Size(25, 13);
            this.lblLogSize.TabIndex = 12;
            this.lblLogSize.Text = "N/A";
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnOpenLogFile
            // 
            this.btnOpenLogFile.Location = new System.Drawing.Point(508, 162);
            this.btnOpenLogFile.Name = "btnOpenLogFile";
            this.btnOpenLogFile.Size = new System.Drawing.Size(27, 21);
            this.btnOpenLogFile.TabIndex = 9;
            this.btnOpenLogFile.Text = "...";
            this.toolTip1.SetToolTip(this.btnOpenLogFile, "Open Log File");
            this.btnOpenLogFile.UseVisualStyleBackColor = true;
            this.btnOpenLogFile.Click += new System.EventHandler(this.btnOpenLogFile_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(263, 221);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(85, 24);
            this.btnStop.TabIndex = 14;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // chkTraceParam
            // 
            this.chkTraceParam.AutoSize = true;
            this.chkTraceParam.Checked = true;
            this.chkTraceParam.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTraceParam.Location = new System.Drawing.Point(98, 86);
            this.chkTraceParam.Name = "chkTraceParam";
            this.chkTraceParam.Size = new System.Drawing.Size(106, 17);
            this.chkTraceParam.TabIndex = 4;
            this.chkTraceParam.Text = "Trace Parameter";
            this.chkTraceParam.UseVisualStyleBackColor = true;
            this.chkTraceParam.CheckedChanged += new System.EventHandler(this.chkTraceParam_CheckedChanged);
            // 
            // chkTraceEvent
            // 
            this.chkTraceEvent.AutoSize = true;
            this.chkTraceEvent.Location = new System.Drawing.Point(99, 66);
            this.chkTraceEvent.Name = "chkTraceEvent";
            this.chkTraceEvent.Size = new System.Drawing.Size(84, 17);
            this.chkTraceEvent.TabIndex = 3;
            this.chkTraceEvent.Text = "Trace Event";
            this.chkTraceEvent.UseVisualStyleBackColor = true;
            // 
            // btnDeleteLogFile
            // 
            this.btnDeleteLogFile.Location = new System.Drawing.Point(541, 162);
            this.btnDeleteLogFile.Name = "btnDeleteLogFile";
            this.btnDeleteLogFile.Size = new System.Drawing.Size(27, 21);
            this.btnDeleteLogFile.TabIndex = 10;
            this.btnDeleteLogFile.Text = "X";
            this.toolTip1.SetToolTip(this.btnDeleteLogFile, "Delete Log File");
            this.btnDeleteLogFile.UseVisualStyleBackColor = true;
            this.btnDeleteLogFile.Click += new System.EventHandler(this.btnDeleteLogFile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Options";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 192);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Log Enabled";
            // 
            // chkLogEnabled
            // 
            this.chkLogEnabled.AutoSize = true;
            this.chkLogEnabled.Checked = true;
            this.chkLogEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogEnabled.Location = new System.Drawing.Point(99, 191);
            this.chkLogEnabled.Name = "chkLogEnabled";
            this.chkLogEnabled.Size = new System.Drawing.Size(15, 14);
            this.chkLogEnabled.TabIndex = 11;
            this.chkLogEnabled.UseVisualStyleBackColor = true;
            this.chkLogEnabled.CheckedChanged += new System.EventHandler(this.chkLogEnabled_CheckedChanged);
            // 
            // frmProfilerApp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(598, 264);
            this.Controls.Add(this.chkLogEnabled);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnDeleteLogFile);
            this.Controls.Add(this.chkTraceEvent);
            this.Controls.Add(this.chkTraceParam);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnOpenLogFile);
            this.Controls.Add(this.txtArg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLogSize);
            this.Controls.Add(this.chkIncludeSystem);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.txtLogFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLogPath);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSelectApp);
            this.Controls.Add(this.txtApp);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmProfilerApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Profiler Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProfilerApp_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectApp;
        private System.Windows.Forms.TextBox txtApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtArg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.CheckBox chkIncludeSystem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblLogSize;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnOpenLogFile;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox chkTraceParam;
        private System.Windows.Forms.CheckBox chkTraceEvent;
        private System.Windows.Forms.Button btnDeleteLogFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkLogEnabled;

    }
}
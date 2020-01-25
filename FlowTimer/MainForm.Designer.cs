namespace FlowTimer {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.LabelTimer = new System.Windows.Forms.Label();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.ButtonSettings = new System.Windows.Forms.Button();
            this.ButtonLoadTimers = new System.Windows.Forms.Button();
            this.ButtonSaveTimers = new System.Windows.Forms.Button();
            this.PictureBoxPin = new System.Windows.Forms.PictureBox();
            this.LabelBeeps = new System.Windows.Forms.Label();
            this.LabelInterval = new System.Windows.Forms.Label();
            this.LabelName = new System.Windows.Forms.Label();
            this.LabelOffset = new System.Windows.Forms.Label();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPageFixedOffset = new System.Windows.Forms.TabPage();
            this.TabPageVariableOffset = new System.Windows.Forms.TabPage();
            this.ComboBoxFPS = new System.Windows.Forms.ComboBox();
            this.ButtonSubmit = new System.Windows.Forms.Button();
            this.LabelBeeps2 = new System.Windows.Forms.Label();
            this.TextBoxBeeps = new System.Windows.Forms.TextBox();
            this.labelInterval2 = new System.Windows.Forms.Label();
            this.TextBoxInterval = new System.Windows.Forms.TextBox();
            this.LabelOffset2 = new System.Windows.Forms.Label();
            this.TextBoxOffset = new System.Windows.Forms.TextBox();
            this.LabelFPS = new System.Windows.Forms.Label();
            this.LabelFrame = new System.Windows.Forms.Label();
            this.TextBoxFrame = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPin)).BeginInit();
            this.TabControl.SuspendLayout();
            this.TabPageFixedOffset.SuspendLayout();
            this.TabPageVariableOffset.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelTimer
            // 
            this.LabelTimer.AutoSize = true;
            this.LabelTimer.Font = new System.Drawing.Font("Century Gothic", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTimer.Location = new System.Drawing.Point(5, 4);
            this.LabelTimer.Name = "LabelTimer";
            this.LabelTimer.Size = new System.Drawing.Size(114, 44);
            this.LabelTimer.TabIndex = 0;
            this.LabelTimer.Text = "0.000";
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location = new System.Drawing.Point(12, 57);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(121, 25);
            this.ButtonStart.TabIndex = 1;
            this.ButtonStart.TabStop = false;
            this.ButtonStart.Text = "Start";
            this.ButtonStart.UseVisualStyleBackColor = true;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // ButtonStop
            // 
            this.ButtonStop.Location = new System.Drawing.Point(12, 85);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(121, 25);
            this.ButtonStop.TabIndex = 2;
            this.ButtonStop.TabStop = false;
            this.ButtonStop.Text = "Stop";
            this.ButtonStop.UseVisualStyleBackColor = true;
            this.ButtonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(0, 0);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(67, 23);
            this.ButtonAdd.TabIndex = 28;
            this.ButtonAdd.TabStop = false;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // ButtonSettings
            // 
            this.ButtonSettings.Location = new System.Drawing.Point(12, 113);
            this.ButtonSettings.Name = "ButtonSettings";
            this.ButtonSettings.Size = new System.Drawing.Size(121, 25);
            this.ButtonSettings.TabIndex = 30;
            this.ButtonSettings.TabStop = false;
            this.ButtonSettings.Text = "Settings";
            this.ButtonSettings.UseVisualStyleBackColor = true;
            this.ButtonSettings.Click += new System.EventHandler(this.ButtonSettings_Click);
            // 
            // ButtonLoadTimers
            // 
            this.ButtonLoadTimers.Location = new System.Drawing.Point(12, 141);
            this.ButtonLoadTimers.Name = "ButtonLoadTimers";
            this.ButtonLoadTimers.Size = new System.Drawing.Size(121, 25);
            this.ButtonLoadTimers.TabIndex = 31;
            this.ButtonLoadTimers.TabStop = false;
            this.ButtonLoadTimers.Text = "Load Timers";
            this.ButtonLoadTimers.UseVisualStyleBackColor = true;
            this.ButtonLoadTimers.Click += new System.EventHandler(this.ButtonLoadTimers_Click);
            // 
            // ButtonSaveTimers
            // 
            this.ButtonSaveTimers.Location = new System.Drawing.Point(12, 169);
            this.ButtonSaveTimers.Name = "ButtonSaveTimers";
            this.ButtonSaveTimers.Size = new System.Drawing.Size(121, 25);
            this.ButtonSaveTimers.TabIndex = 32;
            this.ButtonSaveTimers.TabStop = false;
            this.ButtonSaveTimers.Text = "Save Timers";
            this.ButtonSaveTimers.UseVisualStyleBackColor = true;
            this.ButtonSaveTimers.Click += new System.EventHandler(this.ButtonSaveTimers_Click);
            // 
            // PictureBoxPin
            // 
            this.PictureBoxPin.Location = new System.Drawing.Point(478, 4);
            this.PictureBoxPin.Name = "PictureBoxPin";
            this.PictureBoxPin.Size = new System.Drawing.Size(16, 16);
            this.PictureBoxPin.TabIndex = 33;
            this.PictureBoxPin.TabStop = false;
            this.PictureBoxPin.Click += new System.EventHandler(this.PictureBoxPin_Click);
            // 
            // LabelBeeps
            // 
            this.LabelBeeps.AutoSize = true;
            this.LabelBeeps.Location = new System.Drawing.Point(374, 14);
            this.LabelBeeps.Name = "LabelBeeps";
            this.LabelBeeps.Size = new System.Drawing.Size(37, 13);
            this.LabelBeeps.TabIndex = 16;
            this.LabelBeeps.Text = "Beeps";
            // 
            // LabelInterval
            // 
            this.LabelInterval.AutoSize = true;
            this.LabelInterval.Location = new System.Drawing.Point(303, 14);
            this.LabelInterval.Name = "LabelInterval";
            this.LabelInterval.Size = new System.Drawing.Size(42, 13);
            this.LabelInterval.TabIndex = 13;
            this.LabelInterval.Text = "Interval";
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(161, 14);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(35, 13);
            this.LabelName.TabIndex = 7;
            this.LabelName.Text = "Name";
            // 
            // LabelOffset
            // 
            this.LabelOffset.AutoSize = true;
            this.LabelOffset.Location = new System.Drawing.Point(232, 14);
            this.LabelOffset.Name = "LabelOffset";
            this.LabelOffset.Size = new System.Drawing.Size(35, 13);
            this.LabelOffset.TabIndex = 12;
            this.LabelOffset.Text = "Offset";
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.TabPageFixedOffset);
            this.TabControl.Controls.Add(this.TabPageVariableOffset);
            this.TabControl.Location = new System.Drawing.Point(-3, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(536, 250);
            this.TabControl.TabIndex = 34;
            // 
            // TabPageFixedOffset
            // 
            this.TabPageFixedOffset.BackColor = System.Drawing.SystemColors.Control;
            this.TabPageFixedOffset.Controls.Add(this.ButtonAdd);
            this.TabPageFixedOffset.Controls.Add(this.LabelTimer);
            this.TabPageFixedOffset.Controls.Add(this.PictureBoxPin);
            this.TabPageFixedOffset.Controls.Add(this.LabelBeeps);
            this.TabPageFixedOffset.Controls.Add(this.ButtonStart);
            this.TabPageFixedOffset.Controls.Add(this.LabelInterval);
            this.TabPageFixedOffset.Controls.Add(this.ButtonSaveTimers);
            this.TabPageFixedOffset.Controls.Add(this.LabelOffset);
            this.TabPageFixedOffset.Controls.Add(this.ButtonStop);
            this.TabPageFixedOffset.Controls.Add(this.LabelName);
            this.TabPageFixedOffset.Controls.Add(this.ButtonLoadTimers);
            this.TabPageFixedOffset.Controls.Add(this.ButtonSettings);
            this.TabPageFixedOffset.Location = new System.Drawing.Point(4, 22);
            this.TabPageFixedOffset.Name = "TabPageFixedOffset";
            this.TabPageFixedOffset.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageFixedOffset.Size = new System.Drawing.Size(528, 224);
            this.TabPageFixedOffset.TabIndex = 0;
            this.TabPageFixedOffset.Text = "Fixed Offset";
            // 
            // TabPageVariableOffset
            // 
            this.TabPageVariableOffset.BackColor = System.Drawing.SystemColors.Control;
            this.TabPageVariableOffset.Controls.Add(this.ComboBoxFPS);
            this.TabPageVariableOffset.Controls.Add(this.ButtonSubmit);
            this.TabPageVariableOffset.Controls.Add(this.LabelBeeps2);
            this.TabPageVariableOffset.Controls.Add(this.TextBoxBeeps);
            this.TabPageVariableOffset.Controls.Add(this.labelInterval2);
            this.TabPageVariableOffset.Controls.Add(this.TextBoxInterval);
            this.TabPageVariableOffset.Controls.Add(this.LabelOffset2);
            this.TabPageVariableOffset.Controls.Add(this.TextBoxOffset);
            this.TabPageVariableOffset.Controls.Add(this.LabelFPS);
            this.TabPageVariableOffset.Controls.Add(this.LabelFrame);
            this.TabPageVariableOffset.Controls.Add(this.TextBoxFrame);
            this.TabPageVariableOffset.Location = new System.Drawing.Point(4, 22);
            this.TabPageVariableOffset.Name = "TabPageVariableOffset";
            this.TabPageVariableOffset.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageVariableOffset.Size = new System.Drawing.Size(528, 224);
            this.TabPageVariableOffset.TabIndex = 1;
            this.TabPageVariableOffset.Text = "Variable Offset";
            // 
            // ComboBoxFPS
            // 
            this.ComboBoxFPS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxFPS.FormattingEnabled = true;
            this.ComboBoxFPS.Items.AddRange(new object[] {
            "59.7275",
            "59.8261",
            "60.0",
            "30.0",
            "25.0",
            "15.0"});
            this.ComboBoxFPS.Location = new System.Drawing.Point(240, 39);
            this.ComboBoxFPS.Name = "ComboBoxFPS";
            this.ComboBoxFPS.Size = new System.Drawing.Size(80, 21);
            this.ComboBoxFPS.TabIndex = 11;
            this.ComboBoxFPS.SelectedIndexChanged += new System.EventHandler(this.VariableTimer_DataChange);
            // 
            // ButtonSubmit
            // 
            this.ButtonSubmit.Location = new System.Drawing.Point(326, 12);
            this.ButtonSubmit.Name = "ButtonSubmit";
            this.ButtonSubmit.Size = new System.Drawing.Size(80, 22);
            this.ButtonSubmit.TabIndex = 10;
            this.ButtonSubmit.Text = "Submit";
            this.ButtonSubmit.UseVisualStyleBackColor = true;
            this.ButtonSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);
            // 
            // LabelBeeps2
            // 
            this.LabelBeeps2.AutoSize = true;
            this.LabelBeeps2.Location = new System.Drawing.Point(192, 120);
            this.LabelBeeps2.Name = "LabelBeeps2";
            this.LabelBeeps2.Size = new System.Drawing.Size(40, 13);
            this.LabelBeeps2.TabIndex = 9;
            this.LabelBeeps2.Text = "Beeps:";
            // 
            // TextBoxBeeps
            // 
            this.TextBoxBeeps.Location = new System.Drawing.Point(240, 117);
            this.TextBoxBeeps.Name = "TextBoxBeeps";
            this.TextBoxBeeps.Size = new System.Drawing.Size(80, 20);
            this.TextBoxBeeps.TabIndex = 8;
            this.TextBoxBeeps.TextChanged += new System.EventHandler(this.VariableTimer_DataChange);
            // 
            // labelInterval2
            // 
            this.labelInterval2.AutoSize = true;
            this.labelInterval2.Location = new System.Drawing.Point(192, 94);
            this.labelInterval2.Name = "labelInterval2";
            this.labelInterval2.Size = new System.Drawing.Size(45, 13);
            this.labelInterval2.TabIndex = 7;
            this.labelInterval2.Text = "Interval:";
            // 
            // TextBoxInterval
            // 
            this.TextBoxInterval.Location = new System.Drawing.Point(240, 91);
            this.TextBoxInterval.Name = "TextBoxInterval";
            this.TextBoxInterval.Size = new System.Drawing.Size(80, 20);
            this.TextBoxInterval.TabIndex = 6;
            this.TextBoxInterval.TextChanged += new System.EventHandler(this.VariableTimer_DataChange);
            // 
            // LabelOffset2
            // 
            this.LabelOffset2.AutoSize = true;
            this.LabelOffset2.Location = new System.Drawing.Point(192, 68);
            this.LabelOffset2.Name = "LabelOffset2";
            this.LabelOffset2.Size = new System.Drawing.Size(38, 13);
            this.LabelOffset2.TabIndex = 5;
            this.LabelOffset2.Text = "Offset:";
            // 
            // TextBoxOffset
            // 
            this.TextBoxOffset.Location = new System.Drawing.Point(240, 65);
            this.TextBoxOffset.Name = "TextBoxOffset";
            this.TextBoxOffset.Size = new System.Drawing.Size(80, 20);
            this.TextBoxOffset.TabIndex = 4;
            this.TextBoxOffset.TextChanged += new System.EventHandler(this.VariableTimer_DataChange);
            // 
            // LabelFPS
            // 
            this.LabelFPS.AutoSize = true;
            this.LabelFPS.Location = new System.Drawing.Point(192, 42);
            this.LabelFPS.Name = "LabelFPS";
            this.LabelFPS.Size = new System.Drawing.Size(30, 13);
            this.LabelFPS.TabIndex = 3;
            this.LabelFPS.Text = "FPS:";
            // 
            // LabelFrame
            // 
            this.LabelFrame.AutoSize = true;
            this.LabelFrame.Location = new System.Drawing.Point(192, 16);
            this.LabelFrame.Name = "LabelFrame";
            this.LabelFrame.Size = new System.Drawing.Size(39, 13);
            this.LabelFrame.TabIndex = 1;
            this.LabelFrame.Text = "Frame:";
            // 
            // TextBoxFrame
            // 
            this.TextBoxFrame.Location = new System.Drawing.Point(240, 13);
            this.TextBoxFrame.Name = "TextBoxFrame";
            this.TextBoxFrame.Size = new System.Drawing.Size(80, 20);
            this.TextBoxFrame.TabIndex = 0;
            this.TextBoxFrame.TextChanged += new System.EventHandler(this.VariableTimer_DataChange);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 228);
            this.Controls.Add(this.TabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "FlowTimer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPin)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.TabPageFixedOffset.ResumeLayout(false);
            this.TabPageFixedOffset.PerformLayout();
            this.TabPageVariableOffset.ResumeLayout(false);
            this.TabPageVariableOffset.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label LabelTimer;
        public System.Windows.Forms.Button ButtonStart;
        public System.Windows.Forms.Button ButtonStop;
        public System.Windows.Forms.Button ButtonAdd;
        public System.Windows.Forms.Button ButtonSettings;
        public System.Windows.Forms.Button ButtonLoadTimers;
        public System.Windows.Forms.Button ButtonSaveTimers;
        public System.Windows.Forms.PictureBox PictureBoxPin;
        public System.Windows.Forms.Label LabelBeeps;
        public System.Windows.Forms.Label LabelInterval;
        public System.Windows.Forms.Label LabelName;
        public System.Windows.Forms.Label LabelOffset;
        public System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage TabPageFixedOffset;
        private System.Windows.Forms.TabPage TabPageVariableOffset;
        public System.Windows.Forms.TextBox TextBoxFrame;
        public System.Windows.Forms.Label LabelFrame;
        public System.Windows.Forms.Label LabelBeeps2;
        public System.Windows.Forms.TextBox TextBoxBeeps;
        public System.Windows.Forms.Label labelInterval2;
        public System.Windows.Forms.TextBox TextBoxInterval;
        public System.Windows.Forms.Label LabelOffset2;
        public System.Windows.Forms.TextBox TextBoxOffset;
        public System.Windows.Forms.Label LabelFPS;
        public System.Windows.Forms.Button ButtonSubmit;
        public System.Windows.Forms.ComboBox ComboBoxFPS;
    }
}

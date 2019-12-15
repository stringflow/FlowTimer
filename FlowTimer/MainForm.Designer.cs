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
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPin)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelTimer
            // 
            this.LabelTimer.AutoSize = true;
            this.LabelTimer.Font = new System.Drawing.Font("Century Gothic", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTimer.Location = new System.Drawing.Point(5, 4);
            this.LabelTimer.Name = "LabelTimer";
            this.LabelTimer.Size = new System.Drawing.Size(0, 44);
            this.LabelTimer.TabIndex = 0;
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
            this.ButtonSaveTimers.Text = "SaveTimers";
            this.ButtonSaveTimers.UseVisualStyleBackColor = true;
            this.ButtonSaveTimers.Click += new System.EventHandler(this.ButtonSaveTimers_Click);
            // 
            // PictureBoxPin
            // 
            this.PictureBoxPin.Location = new System.Drawing.Point(479, 5);
            this.PictureBoxPin.Name = "PictureBoxPin";
            this.PictureBoxPin.Size = new System.Drawing.Size(16, 16);
            this.PictureBoxPin.TabIndex = 33;
            this.PictureBoxPin.TabStop = false;
            this.PictureBoxPin.Click += new System.EventHandler(this.PictureBoxPin_Click);
            // 
            // LabelBeeps
            // 
            this.LabelBeeps.AutoSize = true;
            this.LabelBeeps.Location = new System.Drawing.Point(375, 15);
            this.LabelBeeps.Name = "LabelBeeps";
            this.LabelBeeps.Size = new System.Drawing.Size(37, 13);
            this.LabelBeeps.TabIndex = 16;
            this.LabelBeeps.Text = "Beeps";
            // 
            // LabelInterval
            // 
            this.LabelInterval.AutoSize = true;
            this.LabelInterval.Location = new System.Drawing.Point(304, 15);
            this.LabelInterval.Name = "LabelInterval";
            this.LabelInterval.Size = new System.Drawing.Size(42, 13);
            this.LabelInterval.TabIndex = 13;
            this.LabelInterval.Text = "Interval";
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(162, 15);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(35, 13);
            this.LabelName.TabIndex = 7;
            this.LabelName.Text = "Name";
            // 
            // LabelOffset
            // 
            this.LabelOffset.AutoSize = true;
            this.LabelOffset.Location = new System.Drawing.Point(233, 15);
            this.LabelOffset.Name = "LabelOffset";
            this.LabelOffset.Size = new System.Drawing.Size(35, 13);
            this.LabelOffset.TabIndex = 12;
            this.LabelOffset.Text = "Offset";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 206);
            this.Controls.Add(this.PictureBoxPin);
            this.Controls.Add(this.ButtonSaveTimers);
            this.Controls.Add(this.ButtonLoadTimers);
            this.Controls.Add(this.ButtonSettings);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.LabelBeeps);
            this.Controls.Add(this.LabelInterval);
            this.Controls.Add(this.LabelOffset);
            this.Controls.Add(this.LabelName);
            this.Controls.Add(this.ButtonStop);
            this.Controls.Add(this.ButtonStart);
            this.Controls.Add(this.LabelTimer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "FlowTimer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}


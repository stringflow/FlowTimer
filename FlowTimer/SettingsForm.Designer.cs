namespace FlowTimer {
    partial class SettingsForm {
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
            this.LabelBeep = new System.Windows.Forms.Label();
            this.ComboBoxBeep = new System.Windows.Forms.ComboBox();
            this.ButtonImportBeep = new System.Windows.Forms.Button();
            this.ComboBoxKey = new System.Windows.Forms.ComboBox();
            this.LabelKey = new System.Windows.Forms.Label();
            this.TrackBarVolume = new System.Windows.Forms.TrackBar();
            this.TextBoxVolume = new System.Windows.Forms.TextBox();
            this.LabelVolume = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelBeep
            // 
            this.LabelBeep.AutoSize = true;
            this.LabelBeep.Location = new System.Drawing.Point(5, 40);
            this.LabelBeep.Name = "LabelBeep";
            this.LabelBeep.Size = new System.Drawing.Size(35, 13);
            this.LabelBeep.TabIndex = 20;
            this.LabelBeep.Text = "Beep:";
            // 
            // ComboBoxBeep
            // 
            this.ComboBoxBeep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxBeep.FormattingEnabled = true;
            this.ComboBoxBeep.Location = new System.Drawing.Point(52, 36);
            this.ComboBoxBeep.Name = "ComboBoxBeep";
            this.ComboBoxBeep.Size = new System.Drawing.Size(112, 21);
            this.ComboBoxBeep.TabIndex = 22;
            // 
            // ButtonImportBeep
            // 
            this.ButtonImportBeep.Location = new System.Drawing.Point(168, 35);
            this.ButtonImportBeep.Name = "ButtonImportBeep";
            this.ButtonImportBeep.Size = new System.Drawing.Size(114, 23);
            this.ButtonImportBeep.TabIndex = 23;
            this.ButtonImportBeep.Text = "Import Beep";
            this.ButtonImportBeep.UseVisualStyleBackColor = true;
            this.ButtonImportBeep.Click += new System.EventHandler(this.ButtonImportBeep_Click);
            // 
            // ComboBoxKey
            // 
            this.ComboBoxKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxKey.FormattingEnabled = true;
            this.ComboBoxKey.Location = new System.Drawing.Point(52, 88);
            this.ComboBoxKey.Name = "ComboBoxKey";
            this.ComboBoxKey.Size = new System.Drawing.Size(112, 21);
            this.ComboBoxKey.TabIndex = 25;
            // 
            // LabelKey
            // 
            this.LabelKey.AutoSize = true;
            this.LabelKey.Location = new System.Drawing.Point(5, 91);
            this.LabelKey.Name = "LabelKey";
            this.LabelKey.Size = new System.Drawing.Size(28, 13);
            this.LabelKey.TabIndex = 24;
            this.LabelKey.Text = "Key:";
            // 
            // TrackBarVolume
            // 
            this.TrackBarVolume.AutoSize = false;
            this.TrackBarVolume.Location = new System.Drawing.Point(45, 63);
            this.TrackBarVolume.Maximum = 100;
            this.TrackBarVolume.Name = "TrackBarVolume";
            this.TrackBarVolume.Size = new System.Drawing.Size(165, 21);
            this.TrackBarVolume.TabIndex = 28;
            this.TrackBarVolume.TickFrequency = 0;
            this.TrackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.TrackBarVolume.Value = 100;
            // 
            // TextBoxVolume
            // 
            this.TextBoxVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.7F);
            this.TextBoxVolume.Location = new System.Drawing.Point(208, 62);
            this.TextBoxVolume.Name = "TextBoxVolume";
            this.TextBoxVolume.Size = new System.Drawing.Size(73, 21);
            this.TextBoxVolume.TabIndex = 29;
            // 
            // LabelVolume
            // 
            this.LabelVolume.AutoSize = true;
            this.LabelVolume.Location = new System.Drawing.Point(6, 64);
            this.LabelVolume.Name = "LabelVolume";
            this.LabelVolume.Size = new System.Drawing.Size(45, 13);
            this.LabelVolume.TabIndex = 30;
            this.LabelVolume.Text = "Volume:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 115);
            this.Controls.Add(this.LabelVolume);
            this.Controls.Add(this.TextBoxVolume);
            this.Controls.Add(this.TrackBarVolume);
            this.Controls.Add(this.ComboBoxKey);
            this.Controls.Add(this.LabelKey);
            this.Controls.Add(this.ButtonImportBeep);
            this.Controls.Add(this.ComboBoxBeep);
            this.Controls.Add(this.LabelBeep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ComboBox ComboBoxBeep;
        public System.Windows.Forms.Label LabelBeep;
        public System.Windows.Forms.Button ButtonImportBeep;
        public System.Windows.Forms.ComboBox ComboBoxKey;
        public System.Windows.Forms.Label LabelKey;
        private System.Windows.Forms.TrackBar TrackBarVolume;
        private System.Windows.Forms.TextBox TextBoxVolume;
        public System.Windows.Forms.Label LabelVolume;
    }
}